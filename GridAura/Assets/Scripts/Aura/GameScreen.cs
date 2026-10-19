using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aura
{
    public class GameScreen : MonoBehaviour
    {
        const float ReferenceWidth = 390f;
        const float ReferenceHeight = 844f;

        GameState state = new GameState();
        LevelCatalog mainLevels;
        int levelIndex;
        Font font;
        Canvas canvas;
        CanvasScaler scaler;
        RectTransform root;
        RectTransform boardFrame;
        RectTransform grid;
        RectTransform placedLayer;
        RectTransform ghost;
        Image ghostImage;
        RectTransform drag;
        Image dragImage;
        Image frameImage;
        Sprite frameSprite;
        Sprite emptySprite;
        Sprite redSprite;
        Sprite blueSprite;
        Sprite yellowSprite;
        Sprite blackSprite;
        Text levelText;
        Text hintText;
        Text messageText;
        RectTransform winPanel;
        Text winText;
        RectTransform confirmPanel;
        RectTransform rotateButton;
        readonly List<TraySlot> slots = new List<TraySlot>();

        float pitch;
        float gap;
        Vector2 lastScreen;
        bool dragging;
        bool moved;
        PieceControl active;
        Graphic hiddenGraphic;
        PlacedPiece lifted;
        int dragH;
        int dragW;
        Vector2 lastPointer;
        Vector2 pointerStart;
        int grabCol;
        int grabRow;

        class TraySlot
        {
            public string id;
            public RectTransform root;
            public RectTransform slabRect;
            public Image slab;
            public Text label;
            public PieceControl control;
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        static void Boot()
        {
            Screen.autorotateToPortrait = true;
            Screen.autorotateToPortraitUpsideDown = false;
            Screen.autorotateToLandscapeLeft = false;
            Screen.autorotateToLandscapeRight = false;
            Screen.orientation = ScreenOrientation.Portrait;
            if (FindAnyObjectByType<GameScreen>() != null)
                return;
            new GameObject("GridAuraGame").AddComponent<GameScreen>();
        }

        void Awake()
        {
            font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            frameSprite = Resources.Load<Sprite>("Art/Frame");
            emptySprite = Resources.Load<Sprite>("Art/TileEmpty");
            redSprite = Resources.Load<Sprite>("Art/TileRed");
            blueSprite = Resources.Load<Sprite>("Art/TileBlue");
            yellowSprite = Resources.Load<Sprite>("Art/TileYellow");
            blackSprite = Resources.Load<Sprite>("Art/TileBlack");
            var levelsAsset = Resources.Load<TextAsset>("Data/levels");
            if (levelsAsset != null)
                mainLevels = JsonUtility.FromJson<LevelCatalog>(levelsAsset.text);
            var camera = Camera.main;
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
            }
            Build();
        }

        IEnumerator Start()
        {
            yield return null;
            if (mainLevels == null || mainLevels.levels == null || mainLevels.levels.Length == 0)
            {
                messageText.text = "Could not load levels.";
                yield break;
            }
            ShowLevel(0);
            Layout();
        }

        void Update()
        {
            float aspect = (float)Screen.width / Mathf.Max(1, Screen.height);
            scaler.matchWidthOrHeight = aspect > ReferenceWidth / ReferenceHeight ? 1f : 0f;
            var size = new Vector2(Screen.width, Screen.height);
            if (size != lastScreen)
            {
                lastScreen = size;
                if (state.level != null)
                    Refresh();
                else
                    Layout();
            }
            if (dragging && !Input.GetMouseButton(0) && Input.touchCount == 0)
                EndDrag();
        }

        void Build()
        {
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                var events = new GameObject("EventSystem");
                events.AddComponent<EventSystem>();
                events.AddComponent<StandaloneInputModule>();
            }

            var canvasGo = new GameObject("GameCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            root = MakeRect("Root", canvasGo.transform);
            Stretch(root);

            var background = root.gameObject.AddComponent<Image>();
            background.color = Color.black;
            background.raycastTarget = false;

            var top = MakeButton(root, "Hint", "yellow", out _);
            hintText = MakeText(top, "3", 16, Color.black);
            var pill = MakeButton(root, "Level", "white", out _);
            levelText = MakeText(pill, "Level 1", 22, Hex("1A1A1A"));
            levelText.supportRichText = true;
            MakeButton(root, "Gear", "blue", out _);

            boardFrame = MakeRect("Board", root);
            frameImage = boardFrame.gameObject.AddComponent<Image>();
            frameImage.sprite = frameSprite;
            frameImage.type = Image.Type.Sliced;
            frameImage.raycastTarget = false;
            grid = MakeRect("Grid", boardFrame);
            placedLayer = MakeRect("Placed", grid);
            Stretch(placedLayer);
            ghost = MakeRect("Ghost", grid);
            ghostImage = ghost.gameObject.AddComponent<Image>();
            ghostImage.raycastTarget = false;
            ghostImage.type = Image.Type.Sliced;
            ghost.gameObject.SetActive(false);

            var tray = MakeRect("Tray", root);
            tray.gameObject.AddComponent<Image>().color = Hex("1A1A1A");
            for (int i = 0; i < 8; i++)
            {
                var slot = new TraySlot();
                slot.root = MakeRect("Slot" + i, tray);
                slot.slabRect = MakeRect("Slab", slot.root);
                slot.slab = slot.slabRect.gameObject.AddComponent<Image>();
                slot.slab.type = Image.Type.Sliced;
                slot.slab.raycastTarget = true;
                slot.control = slot.slabRect.gameObject.AddComponent<PieceControl>();
                slot.control.screen = this;
                slot.label = MakeText(slot.root, "", 12, Color.white);
                slots.Add(slot);
            }

            MakeButton(root, "Undo", "blue", out var undoText);
            undoText.text = "Undo";
            undoText.color = Color.white;
            var undoButton = undoText.transform.parent.gameObject.AddComponent<Button>();
            undoButton.transition = Selectable.Transition.None;
            undoButton.onClick.AddListener(OnUndo);
            MakeButton(root, "Reset", "red", out var resetText);
            resetText.text = "Reset";
            resetText.color = Color.white;
            var resetButton = resetText.transform.parent.gameObject.AddComponent<Button>();
            resetButton.transition = Selectable.Transition.None;
            resetButton.onClick.AddListener(ShowReset);

            drag = MakeRect("Drag", root);
            dragImage = drag.gameObject.AddComponent<Image>();
            dragImage.raycastTarget = false;
            dragImage.type = Image.Type.Sliced;
            drag.gameObject.SetActive(false);

            rotateButton = MakeButton(root, "Rotate", "yellow", out var rotateText);
            rotateText.text = "Rotate";
            rotateText.color = Hex("1A1A1A");
            var rotate = rotateButton.gameObject.AddComponent<Button>();
            rotate.transition = Selectable.Transition.None;
            rotate.onClick.AddListener(RotateDragged);
            rotateButton.gameObject.SetActive(false);

            messageText = MakeText(root, "", 16, Color.white);
            winPanel = MakePopup(root, "Level complete", out winText);
            var next = MakeButton(winPanel, "Next", "yellow", out var nextText);
            nextText.text = "Next Level";
            nextText.color = Hex("1A1A1A");
            var nextButton = next.gameObject.AddComponent<Button>();
            nextButton.transition = Selectable.Transition.None;
            nextButton.onClick.AddListener(NextLevel);
            confirmPanel = MakePopup(root, "Reset this level?", out _);
            var yes = MakeButton(confirmPanel, "Yes", "red", out var yesText);
            yesText.text = "Reset";
            yesText.color = Color.white;
            var yesButton = yes.gameObject.AddComponent<Button>();
            yesButton.transition = Selectable.Transition.None;
            yesButton.onClick.AddListener(() =>
            {
                confirmPanel.gameObject.SetActive(false);
                state.ResetUnlocked();
                Refresh();
            });
            var no = MakeButton(confirmPanel, "No", "blue", out var noText);
            noText.text = "Cancel";
            noText.color = Color.white;
            var noButton = no.gameObject.AddComponent<Button>();
            noButton.transition = Selectable.Transition.None;
            noButton.onClick.AddListener(() => confirmPanel.gameObject.SetActive(false));
            winPanel.gameObject.SetActive(false);
            confirmPanel.gameObject.SetActive(false);
            var hintButton = Find("Hint").gameObject.AddComponent<Button>();
            hintButton.transition = Selectable.Transition.None;
            hintButton.onClick.AddListener(OnHint);
            next.anchorMin = new Vector2(0.12f, 0.08f);
            next.anchorMax = new Vector2(0.88f, 0.32f);
            next.offsetMin = next.offsetMax = Vector2.zero;
            yes.anchorMin = new Vector2(0.08f, 0.08f);
            yes.anchorMax = new Vector2(0.48f, 0.32f);
            yes.offsetMin = yes.offsetMax = Vector2.zero;
            no.anchorMin = new Vector2(0.52f, 0.08f);
            no.anchorMax = new Vector2(0.92f, 0.32f);
            no.offsetMin = no.offsetMax = Vector2.zero;
        }

        void OnHint()
        {
            if (state.level == null)
                return;
            if (!state.TryHint())
                messageText.text = state.hintsLeft <= 0 ? "No hints left" : "Almost there!";
            else
                messageText.text = "";
            Refresh();
        }

        public void PointerDown(PieceControl control, PointerEventData eventData)
        {
            if (dragging)
                EndDrag();
            active = control;
            pointerStart = eventData.position;
            moved = false;
            dragging = false;
        }

        public void PointerDrag(PieceControl control, PointerEventData eventData)
        {
            if (active != control)
                return;
            if (!moved && Vector2.Distance(eventData.position, pointerStart) > 12f)
            {
                moved = true;
                BeginDrag(control, eventData);
            }
            if (dragging)
                MoveDrag(eventData);
        }

        public void PointerUp(PieceControl control, PointerEventData eventData)
        {
            if (active != control)
                return;
            if (dragging || moved)
                EndDrag();
            else if (control.onBoard)
            {
                state.ReturnToTray(control.pieceId, true);
                Refresh();
            }
            else
            {
                state.RotateTray(control.pieceId);
                Refresh();
            }
            active = null;
        }

        void BeginDrag(PieceControl control, PointerEventData eventData)
        {
            if (control.onBoard && state.placed.ContainsKey(control.pieceId) && state.placed[control.pieceId].locked)
                return;
            dragging = true;
            int h;
            int w;
            if (control.onBoard && state.placed.ContainsKey(control.pieceId))
            {
                var placed = state.placed[control.pieceId];
                h = placed.h;
                w = placed.w;
                Vector2 local = BoardLocal(eventData.position);
                grabCol = Mathf.Clamp(Mathf.FloorToInt(local.x / pitch) - placed.col, 0, w - 1);
                grabRow = Mathf.Clamp(Mathf.FloorToInt(-local.y / pitch) - placed.row, 0, h - 1);
                lifted = new PlacedPiece { id = placed.id, row = placed.row, col = placed.col, h = placed.h, w = placed.w };
                state.placed.Remove(control.pieceId);
                RevealCells(lifted.col, lifted.row, lifted.w, lifted.h);
            }
            else
            {
                state.Oriented(control.pieceId, state.trayTurns[control.pieceId], out h, out w);
                grabCol = w / 2;
                grabRow = h / 2;
            }
            SetPiece(dragImage, ColorOf(control.pieceId));
            dragH = h;
            dragW = w;
            drag.sizeDelta = new Vector2(Span(w), Span(h));
            drag.gameObject.SetActive(true);
            drag.SetAsLastSibling();
            hiddenGraphic = control.GetComponent<Graphic>();
            if (hiddenGraphic != null)
            {
                hiddenGraphic.enabled = false;
                hiddenGraphic.canvasRenderer.SetAlpha(0f);
            }
            rotateButton.gameObject.SetActive(!control.onBoard);
            rotateButton.SetAsLastSibling();
            MoveDrag(eventData);
        }

        void MoveDrag(PointerEventData eventData)
        {
            lastPointer = eventData.position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, eventData.position, null, out var pointer);
            float cellsW = Mathf.Max(1, dragW);
            float cellsH = Mathf.Max(1, dragH);
            drag.anchorMin = drag.anchorMax = new Vector2(0.5f, 0.5f);
            drag.pivot = new Vector2((grabCol + 0.5f) / cellsW, 1f - (grabRow + 0.5f) / cellsH);
            drag.anchoredPosition = pointer + new Vector2(0f, pitch * 0.8f);

            Vector2 topLeft = BoardLocal(RectTransformUtility.WorldToScreenPoint(null, drag.TransformPoint(new Vector3(drag.rect.xMin, drag.rect.yMax, 0f))));
            int col = Mathf.RoundToInt(topLeft.x / pitch);
            int row = Mathf.RoundToInt(-topLeft.y / pitch);
            bool valid = state.CanPlace(active.pieceId, row, col, dragH, dragW);
            bool overBoard = row < GameState.Size && col < GameState.Size && row + dragH > 0 && col + dragW > 0;
            ghost.gameObject.SetActive(overBoard);
            PlaceOnGrid(ghost, col, row, dragW, dragH);
            SetPiece(ghostImage, ColorOf(active.pieceId));
            ghostImage.color = valid ? new Color(1f, 1f, 1f, 0.45f) : new Color(1f, 0.55f, 0.55f, 0.5f);
        }

        void EndDrag()
        {
            if (!dragging)
                return;
            var piece = active;
            var origin = lifted;
            lifted = null;
            Vector2 topLeft = BoardLocal(RectTransformUtility.WorldToScreenPoint(null, drag.TransformPoint(new Vector3(drag.rect.xMin, drag.rect.yMax, 0f))));
            int col = Mathf.RoundToInt(topLeft.x / pitch);
            int row = Mathf.RoundToInt(-topLeft.y / pitch);
            dragging = false;
            if (hiddenGraphic != null)
            {
                hiddenGraphic.enabled = true;
                hiddenGraphic.canvasRenderer.SetAlpha(1f);
                hiddenGraphic.color = Color.white;
                hiddenGraphic = null;
            }
            drag.gameObject.SetActive(false);
            ghost.gameObject.SetActive(false);
            rotateButton.gameObject.SetActive(false);
            if (piece == null)
            {
                Refresh();
                return;
            }
            if (origin != null && ReleasedOnTray())
            {
                state.Remember(origin);
            }
            else
            {
                bool placed = state.TryPlace(piece.pieceId, row, col, dragH, dragW, origin == null);
                if (placed && origin != null && (row != origin.row || col != origin.col || dragH != origin.h || dragW != origin.w))
                    state.Remember(origin);
                else if (!placed && origin != null)
                    state.TryPlace(origin.id, origin.row, origin.col, origin.h, origin.w, false);
            }
            Refresh();
        }

        bool ReleasedOnTray()
        {
            var tray = Find("Tray");
            return tray != null && RectTransformUtility.RectangleContainsScreenPoint(tray, lastPointer, null);
        }

        void RevealCells(int col, int row, int w, int h)
        {
            for (int r = row; r < row + h; r++)
            {
                for (int c = col; c < col + w; c++)
                {
                    if (state.CellOwner(r, c, null) != null)
                        continue;
                    AddEmptyCell(c, r);
                }
            }
            ghost.SetAsLastSibling();
        }

        void AddEmptyCell(int col, int row)
        {
            var cell = MakeRect("Empty", grid);
            cell.SetSiblingIndex(placedLayer.GetSiblingIndex());
            PlaceOnGrid(cell, col, row, 1, 1);
            var image = cell.gameObject.AddComponent<Image>();
            image.raycastTarget = false;
            image.sprite = emptySprite;
            image.type = Image.Type.Sliced;
            FitSlice(image, 18f);
        }

        void RotateDragged()
        {
            if (active == null || active.onBoard)
                return;
            state.RotateTray(active.pieceId);
            state.Oriented(active.pieceId, state.trayTurns[active.pieceId], out dragH, out dragW);
            drag.sizeDelta = new Vector2(Span(dragW), Span(dragH));
            grabCol = Mathf.Clamp(grabCol, 0, dragW - 1);
            grabRow = Mathf.Clamp(grabRow, 0, dragH - 1);
            var data = new PointerEventData(EventSystem.current) { position = lastPointer };
            MoveDrag(data);
        }

        void OnUndo()
        {
            state.Undo();
            Refresh();
        }

        void ShowReset()
        {
            if (state.placed.Count == 0)
                return;
            confirmPanel.gameObject.SetActive(true);
        }

        void NextLevel()
        {
            winPanel.gameObject.SetActive(false);
            ShowLevel((levelIndex + 1) % mainLevels.levels.Length);
        }

        void ShowLevel(int index)
        {
            levelIndex = index;
            state.Begin(mainLevels.levels[index], mainLevels.pieces);
            for (int i = 0; i < slots.Count && i < state.pieces.Length; i++)
            {
                var piece = state.pieces[i];
                slots[i].id = piece.id;
                slots[i].control.pieceId = piece.id;
                slots[i].label.text = piece.h + " × " + piece.w;
                SetPiece(slots[i].slab, piece.color);
                slots[i].root.gameObject.SetActive(SolutionUses(piece.id));
            }
            levelText.text = "Level <color=#1E7BFF>" + state.level.id + "</color>";
            messageText.text = "";
            Refresh();
        }

        void Refresh()
        {
            Layout();
            hintText.text = state.hintsLeft.ToString();
            foreach (var slot in slots)
            {
                bool placed = state.IsPlaced(slot.id);
                slot.slab.gameObject.SetActive(!placed);
                if (slot.slab.canvasRenderer != null)
                    slot.slab.canvasRenderer.SetAlpha(1f);
                slot.control.onBoard = false;
                slot.label.color = placed ? new Color(1f, 1f, 1f, 0.35f) : Color.white;
                if (!placed)
                {
                    state.Oriented(slot.id, state.trayTurns[slot.id], out int h, out int w);
                    FitTraySlab(slot, h, w);
                }
            }
            for (int i = placedLayer.childCount - 1; i >= 0; i--)
                Destroy(placedLayer.GetChild(i).gameObject);
            for (int i = grid.childCount - 1; i >= 0; i--)
            {
                var child = grid.GetChild(i);
                if (child == placedLayer || child == ghost)
                    continue;
                Destroy(child.gameObject);
            }
            for (int r = 0; r < GameState.Size; r++)
            {
                for (int c = 0; c < GameState.Size; c++)
                {
                    if (state.CellOwner(r, c, null) != null)
                        continue;
                    AddEmptyCell(c, r);
                }
            }
            foreach (var block in state.black)
            {
                var cell = MakeRect("Black", placedLayer);
                PlaceOnGrid(cell, block.col, block.row, block.w, block.h);
                var image = cell.gameObject.AddComponent<Image>();
                image.raycastTarget = false;
                image.sprite = blackSprite;
                image.type = Image.Type.Sliced;
                FitSlice(image, 20f);
            }
            foreach (var pair in state.placed)
            {
                var piece = pair.Value;
                var cell = MakeRect(piece.id, placedLayer);
                PlaceOnGrid(cell, piece.col, piece.row, piece.w, piece.h);
                var image = cell.gameObject.AddComponent<Image>();
                image.raycastTarget = true;
                SetPiece(image, ColorOf(piece.id));
                var control = cell.gameObject.AddComponent<PieceControl>();
                control.screen = this;
                control.pieceId = piece.id;
                control.onBoard = true;
            }
            if (state.IsComplete())
            {
                winText.text = "Level " + state.level.id + " complete";
                winPanel.gameObject.SetActive(true);
            }
        }

        void FitTraySlab(TraySlot slot, int h, int w)
        {
            float unit = Mathf.Min((slot.root.rect.width * 0.9f) / Mathf.Max(1, w), (slot.root.rect.height * 0.62f) / Mathf.Max(1, h));
            if (unit <= 1f)
                unit = 18f;
            slot.slabRect.anchorMin = slot.slabRect.anchorMax = new Vector2(0.5f, 0.62f);
            slot.slabRect.pivot = new Vector2(0.5f, 0.5f);
            slot.slabRect.sizeDelta = new Vector2(unit * w, unit * h);
            slot.label.rectTransform.anchorMin = new Vector2(0f, 0f);
            slot.label.rectTransform.anchorMax = new Vector2(1f, 0.28f);
            slot.label.rectTransform.offsetMin = Vector2.zero;
            slot.label.rectTransform.offsetMax = Vector2.zero;
        }

        void Layout()
        {
            if (root == null || root.rect.width < 10f)
                return;
            Rect safe = Screen.safeArea;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, safe.min, null, out var min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, new Vector2(safe.xMax, safe.yMax), null, out var max);
            float pad = 12f;
            float width = Mathf.Min(max.x - min.x - pad * 2f, 460f);
            float left = (min.x + max.x) * 0.5f - width * 0.5f;
            float top = max.y - pad;
            float bottom = min.y + pad;
            float topH = 58f;
            float buttonH = 54f;
            float trayH = Mathf.Clamp(width * 0.46f, 132f, 210f);
            float used = topH + buttonH + trayH + 42f;
            float boardBudget = top - bottom - used;
            float boardSize = Mathf.Min(width - 18f, boardBudget);
            boardSize = Mathf.Max(160f, boardSize);

            Place(Find("Hint"), left, top - topH, 58f, topH);
            Place(Find("Level"), left + 70f, top - topH, width - 140f, topH);
            Place(Find("Gear"), left + width - 58f, top - topH, 58f, topH);

            float boardTop = top - topH - 10f;
            float boardBottom = boardTop - boardSize;
            Place(boardFrame, left + 18f, boardBottom, boardSize, boardSize);
            float rim = Mathf.Clamp(boardSize * 0.028f, 7f, 12f);
            FitSlice(frameImage, 37f, rim);
            grid.anchorMin = grid.anchorMax = new Vector2(0f, 1f);
            grid.pivot = new Vector2(0f, 1f);
            grid.anchoredPosition = new Vector2(rim, -rim);
            grid.sizeDelta = new Vector2(boardSize - rim * 2f, boardSize - rim * 2f);
            pitch = grid.sizeDelta.x / GameState.Size;
            gap = 1.5f;

            Place(Find("Tray"), left, boardBottom - 10f - trayH, width, trayH);
            LayoutTray(Find("Tray"));

            float buttonTop = bottom + buttonH;
            Place(Find("Undo"), left, bottom, width * 0.48f, buttonH);
            Place(Find("Reset"), left + width * 0.52f, bottom, width * 0.48f, buttonH);
            Place(messageText.rectTransform, left, buttonTop + 4f, width, 22f);
            Place(rotateButton, left + width * 0.28f, bottom + buttonH + 8f, width * 0.44f, 42f);

            var win = winPanel;
            win.anchorMin = win.anchorMax = new Vector2(0.5f, 0.5f);
            win.sizeDelta = new Vector2(Mathf.Min(320f, width), 180f);
            confirmPanel.anchorMin = confirmPanel.anchorMax = new Vector2(0.5f, 0.5f);
            confirmPanel.sizeDelta = win.sizeDelta;

            DrawNumbers(left + 18f, boardTop, rim);
            foreach (var slot in slots)
            {
                if (slot.id != null && !state.IsPlaced(slot.id))
                {
                    state.Oriented(slot.id, state.trayTurns[slot.id], out int h, out int w);
                    FitTraySlab(slot, h, w);
                }
            }
        }

        void DrawNumbers(float frameLeft, float frameTop, float lip)
        {
            var old = root.Find("Numbers");
            if (old != null)
                Destroy(old.gameObject);
            var numbers = MakeRect("Numbers", root);
            Stretch(numbers);
            float originX = -root.rect.xMin;
            float originY = -root.rect.yMax;
            for (int i = 0; i < 8; i++)
            {
                var col = MakeText(numbers, (i + 1).ToString(), 12, Color.white);
                col.rectTransform.anchorMin = col.rectTransform.anchorMax = new Vector2(0f, 1f);
                col.rectTransform.pivot = new Vector2(0f, 1f);
                col.rectTransform.anchoredPosition = new Vector2(frameLeft + originX + lip + i * pitch, frameTop + originY);
                col.rectTransform.sizeDelta = new Vector2(pitch, lip);
                var row = MakeText(numbers, (i + 1).ToString(), 12, Color.white);
                row.rectTransform.anchorMin = row.rectTransform.anchorMax = new Vector2(0f, 1f);
                row.rectTransform.pivot = new Vector2(0f, 1f);
                row.rectTransform.anchoredPosition = new Vector2(frameLeft + originX - 2f, frameTop + originY - lip - i * pitch);
                row.rectTransform.sizeDelta = new Vector2(lip, pitch);
            }
        }

        bool SolutionUses(string id)
        {
            if (state.level == null || state.level.solution == null || state.level.solution.Length == 0)
                return true;
            foreach (var step in state.level.solution)
            {
                if (step.piece == id)
                    return true;
            }
            return false;
        }

        void LayoutTray(RectTransform tray)
        {
            int count = 0;
            foreach (var slot in slots)
            {
                if (slot.root.gameObject.activeSelf)
                    count++;
            }
            if (count == 0)
                return;
            int columns = count < 4 ? count : 4;
            int rows = Mathf.CeilToInt(count / (float)columns);
            float w = tray.rect.width / columns;
            float h = tray.rect.height / rows;
            int index = 0;
            foreach (var slot in slots)
            {
                if (!slot.root.gameObject.activeSelf)
                    continue;
                var rect = slot.root;
                rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.sizeDelta = new Vector2(w - 8f, h - 8f);
                rect.anchoredPosition = new Vector2(4f + (index % columns) * w, -4f - (index / columns) * h);
                index++;
            }
        }

        void PlaceOnGrid(RectTransform rect, int col, int row, int w, int h)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 1f);
            float pad = gap * 0.5f;
            rect.anchoredPosition = new Vector2(col * pitch + pad, -(row * pitch + pad));
            rect.sizeDelta = new Vector2(w * pitch - gap, h * pitch - gap);
        }

        float Span(int cells)
        {
            return Mathf.Max(1f, cells * pitch - gap);
        }

        Vector2 BoardLocal(Vector2 screen)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(grid, screen, null, out var local);
            return local;
        }

        string ColorOf(string id)
        {
            var piece = state.Find(id);
            return piece == null ? "black" : piece.color;
        }

        Sprite SpriteFor(string color)
        {
            switch (color)
            {
                case "red": return redSprite;
                case "blue": return blueSprite;
                case "yellow": return yellowSprite;
                case "white": return emptySprite;
                default: return blackSprite;
            }
        }

        void SetPiece(Image image, string color)
        {
            if (image == null)
                return;
            image.sprite = SpriteFor(color);
            image.type = Image.Type.Sliced;
            image.color = Color.white;
            FitSlice(image, color == "white" ? 16f : 18f);
        }

        void FitSlice(Image image, float borderPixels, float target = -1f)
        {
            if (image == null || image.sprite == null)
                return;
            image.type = Image.Type.Sliced;
            if (target < 0f)
                target = Mathf.Max(6f, pitch * 0.16f);
            float canvasPixels = image.canvas != null ? image.canvas.referencePixelsPerUnit : 100f;
            float spritePixels = image.sprite.pixelsPerUnit;
            image.pixelsPerUnitMultiplier = borderPixels * canvasPixels / Mathf.Max(0.01f, spritePixels * target);
        }

        void StyleSlab(SlabGraphic slab, string color, bool black)
        {
            Colors(color, out var face, out var hi, out var shade);
            slab.bevel = 6f;
            slab.corner = 10f;
            slab.SetColors(face, hi, shade, black || color == "black");
        }

        static void Colors(string color, out Color face, out Color highlight, out Color shade)
        {
            switch (color)
            {
                case "red":
                    face = Hex("F03030"); highlight = Hex("FF6B5E"); shade = Hex("B01E1E"); break;
                case "blue":
                    face = Hex("1E7BFF"); highlight = Hex("6FB0FF"); shade = Hex("0E4DB5"); break;
                case "yellow":
                    face = Hex("FFD21F"); highlight = Hex("FFE97A"); shade = Hex("C99A00"); break;
                case "white":
                    face = Hex("EFE9DC"); highlight = Hex("FFFFFF"); shade = Hex("BDB6A6"); break;
                default:
                    face = Hex("1C1C1E"); highlight = Hex("4A4A4E"); shade = Hex("050506"); break;
            }
        }

        RectTransform Find(string name)
        {
            return root.Find(name) as RectTransform;
        }

        void Place(RectTransform rect, float x, float y, float w, float h)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0f, 0f);
            rect.anchoredPosition = new Vector2(x - root.rect.xMin, y - root.rect.yMax);
            rect.sizeDelta = new Vector2(w, h);
        }

        RectTransform MakeButton(Transform parent, string name, string color, out Text label)
        {
            var rect = MakeRect(name, parent);
            var slab = rect.gameObject.AddComponent<SlabGraphic>();
            StyleSlab(slab, color, false);
            slab.corner = 18f;
            label = MakeText(rect, "", 18, Color.white);
            return rect;
        }

        RectTransform MakePopup(Transform parent, string title, out Text body)
        {
            var rect = MakeRect(title, parent);
            rect.gameObject.AddComponent<Image>().color = Hex("1A1A1A");
            var heading = MakeText(rect, title, 22, Color.white);
            heading.rectTransform.anchorMin = new Vector2(0f, 0.55f);
            heading.rectTransform.anchorMax = new Vector2(1f, 1f);
            body = MakeText(rect, "", 16, Color.white);
            body.rectTransform.anchorMin = new Vector2(0f, 0.35f);
            body.rectTransform.anchorMax = new Vector2(1f, 0.6f);
            return rect;
        }

        Text MakeText(Transform parent, string value, int size, Color color)
        {
            var rect = MakeRect("Label", parent);
            Stretch(rect);
            var text = rect.gameObject.AddComponent<Text>();
            text.font = font;
            text.text = value;
            text.fontSize = size;
            text.fontStyle = FontStyle.Bold;
            text.alignment = TextAnchor.MiddleCenter;
            text.color = color;
            text.raycastTarget = false;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            return text;
        }

        static RectTransform MakeRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        static void Stretch(RectTransform rect)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static Color Hex(string hex)
        {
            ColorUtility.TryParseHtmlString("#" + hex, out var color);
            return color;
        }
    }

    public class PieceControl : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public GameScreen screen;
        public string pieceId;
        public bool onBoard;

        public void OnPointerDown(PointerEventData eventData) { screen.PointerDown(this, eventData); }
        public void OnDrag(PointerEventData eventData) { screen.PointerDrag(this, eventData); }
        public void OnPointerUp(PointerEventData eventData) { screen.PointerUp(this, eventData); }
    }
}
