using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Aura
{
    public class GameScreen : MonoBehaviour
    {
        const float ReferenceWidth = 1080f;
        const float ReferenceHeight = 1920f;
        const float LevelPillWidth = 430f;
        const float LevelPillHeight = 125f;
        const float HeaderButtonSize = 125f;
        const float HintBadgeSize = 56f;
        const float CompleteTitleWidth = 545f;
        const float CompleteTitleHeight = 196f;

        GameState state = new GameState();
        LevelCatalog mainLevels;
        readonly List<LevelDef> allLevels = new List<LevelDef>();
        PieceDef[] catalogPieces;
        int levelIndex;
        Font font;
        Font fontBlack;
        Font fontExtraBold;
        Font fontBold;
        Font fontSemiBold;
        TMP_FontAsset tmpFont;
        TMP_FontAsset tmpBlack;
        TMP_FontAsset tmpExtraBold;
        TMP_FontAsset tmpBold;
        TMP_FontAsset tmpSemiBold;
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
        Sprite levelSprite;
        Sprite hintButtonSprite;
        Sprite hintBulbSprite;
        Sprite hintBadgeSprite;
        Sprite settingsButtonSprite;
        Sprite settingsGearSprite;
        Sprite traySprite;
        Image trayImage;
        Sprite undoButtonSprite;
        Sprite resetButtonSprite;
        Sprite undoIconSprite;
        Sprite resetIconSprite;
        Sprite closeSprite;
        Sprite yellowButtonSprite;
        Sprite blueButtonSprite;
        Sprite greenButtonSprite;
        Sprite popupCardSprite;
        Sprite levelCompleteSprite;
        Sprite starSprite;
        Sprite starGlowSprite;
        Sprite hintGlowSprite;
        TextMeshProUGUI levelWord;
        TextMeshProUGUI levelNumber;
        TextMeshProUGUI hintText;
        TextMeshProUGUI messageText;
        TextMeshProUGUI nextLabel;
        TextMeshProUGUI hintsUsedText;
        TextMeshProUGUI useHintLabel;
        RectTransform overlay;
        CanvasGroup overlayGroup;
        RectTransform winPanel;
        RectTransform hintPanel;
        readonly Image[] winStars = new Image[3];
        readonly Image[] winStarGlows = new Image[3];
        readonly RectTransform[] winStarRoots = new RectTransform[3];
        int winEarnedStars;
        Image hintPopupGlow;
        Image headerBulbGlow;
        RectTransform confirmPanel;
        RectTransform rotateButton;
        RectTransform coach;
        TextMeshProUGUI coachLabel;
        RectTransform aidLayer;
        readonly List<TraySlot> slots = new List<TraySlot>();
        bool coachHidden;
        bool fingerDragStopped;
        bool fingerTapStopped;
        Coroutine aidRoutine;
        Coroutine pulseRoutine;
        Coroutine overlayRoutine;
        Coroutine hintRoutine;
        Coroutine dragRoutine;
        Coroutine introRoutine;
        Coroutine rotateRoutine;
        bool motionBusy;
        bool celebrating;
        bool rotating;
        bool boardIntroActive;
        CanvasGroup rootGroup;
        CanvasGroup coachGroup;
        CanvasGroup undoGroup;
        RectTransform dragShadow;
        int ghostCol = -99;
        int ghostRow = -99;
        Vector2 ghostFrom;
        Vector2 ghostTo;
        float ghostBlend = 1f;
        readonly List<CanvasGroup> aidPulses = new List<CanvasGroup>();
        Sprite impactRingSprite;

        float pitch;
        float gap;
        Vector2 lastScreen;
        bool dragging;
        bool moved;
        PieceControl active;
        CanvasGroup hiddenGroup;
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
            public RectTransform hit;
            public RectTransform slabRect;
            public Image slab;
            public TextMeshProUGUI label;
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
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            if (FindAnyObjectByType<GameScreen>() != null)
                return;
            new GameObject("GridAuraGame").AddComponent<GameScreen>();
        }

        void Awake()
        {
            var builtin = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            fontBlack = Resources.Load<Font>("Fonts/Nunito-Black") ?? builtin;
            fontExtraBold = Resources.Load<Font>("Fonts/Nunito-ExtraBold") ?? builtin;
            fontBold = Resources.Load<Font>("Fonts/Nunito-Bold") ?? builtin;
            fontSemiBold = Resources.Load<Font>("Fonts/Nunito-SemiBold") ?? builtin;
            font = fontExtraBold;
            tmpBlack = CreateTmpFont(fontBlack);
            tmpExtraBold = CreateTmpFont(fontExtraBold);
            tmpBold = CreateTmpFont(fontBold);
            tmpSemiBold = CreateTmpFont(fontSemiBold);
            tmpFont = tmpExtraBold;
            frameSprite = Resources.Load<Sprite>("Art/Frame");
            levelSprite = Resources.Load<Sprite>("Art/LevelPill");
            hintButtonSprite = Resources.Load<Sprite>("Art/HintButton");
            hintBulbSprite = Resources.Load<Sprite>("Art/HintBulb");
            hintBadgeSprite = Resources.Load<Sprite>("Art/HintBadge");
            settingsButtonSprite = Resources.Load<Sprite>("Art/SettingsButton");
            settingsGearSprite = Resources.Load<Sprite>("Art/SettingsGear");
            traySprite = Resources.Load<Sprite>("Art/Tray");
            undoButtonSprite = Resources.Load<Sprite>("Art/UndoButton");
            resetButtonSprite = Resources.Load<Sprite>("Art/ResetButton");
            undoIconSprite = Resources.Load<Sprite>("Art/UndoIcon");
            resetIconSprite = Resources.Load<Sprite>("Art/ResetIcon");
            closeSprite = Resources.Load<Sprite>("Art/CloseButton");
            yellowButtonSprite = Resources.Load<Sprite>("Art/YellowButton");
            blueButtonSprite = Resources.Load<Sprite>("Art/BlueButton");
            greenButtonSprite = Resources.Load<Sprite>("Art/GreenButton");
            popupCardSprite = Resources.Load<Sprite>("Art/PopupCard");
            levelCompleteSprite = Resources.Load<Sprite>("Art/LevelComplete");
            starSprite = Resources.Load<Sprite>("Art/StarClear");
            starGlowSprite = Resources.Load<Sprite>("Art/StarGlow");
            hintGlowSprite = Resources.Load<Sprite>("Art/HintGlow");
            emptySprite = Resources.Load<Sprite>("Art/TileEmpty");
            redSprite = Resources.Load<Sprite>("Art/TileRed");
            blueSprite = Resources.Load<Sprite>("Art/TileBlue");
            yellowSprite = Resources.Load<Sprite>("Art/TileYellow");
            blackSprite = Resources.Load<Sprite>("Art/TileBlack");
            LoadCatalogs();
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
            if (allLevels.Count == 0)
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
                if (boardIntroActive)
                    Layout();
                else if (state.level != null)
                    Refresh();
                else
                    Layout();
            }
            if (!motionBusy && dragging && !Input.GetMouseButton(0) && Input.touchCount == 0)
                EndDrag();
            if (dragging)
                StepGhost();
            float pulse = 0.35f + 0.4f * (Mathf.Sin(Time.unscaledTime * 3.2f) * 0.5f + 0.5f);
            for (int i = 0; i < aidPulses.Count; i++)
            {
                if (aidPulses[i] != null)
                    aidPulses[i].alpha = pulse;
            }
            AnimateGlows();
            BreatheHint();
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
            rootGroup = root.gameObject.AddComponent<CanvasGroup>();

            var background = root.gameObject.AddComponent<Image>();
            background.color = Color.black;
            background.raycastTarget = false;

            var hint = MakeRect("Hint", root);
            var hintBg = hint.gameObject.AddComponent<Image>();
            hintBg.sprite = hintButtonSprite;
            hintBg.type = Image.Type.Sliced;
            hintBg.raycastTarget = true;
            var bulb = MakeRect("Bulb", hint);
            bulb.anchorMin = new Vector2(0.16f, 0.16f);
            bulb.anchorMax = new Vector2(0.84f, 0.84f);
            bulb.offsetMin = Vector2.zero;
            bulb.offsetMax = Vector2.zero;
            headerBulbGlow = AddGlowStack(bulb, hintGlowSprite, hintBulbSprite);
            var badge = MakeRect("Badge", hint);
            badge.anchorMin = badge.anchorMax = new Vector2(1f, 0f);
            badge.pivot = new Vector2(0.72f, 0.28f);
            badge.anchoredPosition = Vector2.zero;
            badge.sizeDelta = new Vector2(HintBadgeSize, HintBadgeSize);
            var badgeImage = badge.gameObject.AddComponent<Image>();
            badgeImage.sprite = hintBadgeSprite;
            badgeImage.raycastTarget = false;
            hintText = MakeText(badge, "∞", 32, Color.white, fontBlack);

            var pill = MakeRect("Level", root);
            var pillImage = pill.gameObject.AddComponent<Image>();
            pillImage.sprite = levelSprite;
            pillImage.type = Image.Type.Sliced;
            pillImage.raycastTarget = false;
            levelWord = MakeText(pill, "Level", 60, Hex("2B2B2B"), fontExtraBold);
            levelNumber = MakeText(pill, "1", 75, Hex("1E7BFF"), fontBlack);
            PinLabel(levelWord.rectTransform);
            PinLabel(levelNumber.rectTransform);

            var gear = MakeRect("Gear", root);
            var gearBg = gear.gameObject.AddComponent<Image>();
            gearBg.sprite = settingsButtonSprite;
            gearBg.type = Image.Type.Sliced;
            gearBg.raycastTarget = true;
            var gearIcon = MakeRect("Icon", gear);
            gearIcon.anchorMin = new Vector2(0.18f, 0.18f);
            gearIcon.anchorMax = new Vector2(0.82f, 0.82f);
            gearIcon.offsetMin = Vector2.zero;
            gearIcon.offsetMax = Vector2.zero;
            var gearImage = gearIcon.gameObject.AddComponent<Image>();
            gearImage.sprite = settingsGearSprite;
            gearImage.preserveAspect = true;
            gearImage.raycastTarget = false;

            boardFrame = MakeRect("Board", root);
            frameImage = boardFrame.gameObject.AddComponent<Image>();
            frameImage.raycastTarget = false;
            if (frameSprite != null)
            {
                frameImage.sprite = frameSprite;
                frameImage.type = Image.Type.Simple;
                frameImage.preserveAspect = false;
                frameImage.color = Color.white;
            }
            else
                frameImage.color = Hex("14171C");
            grid = MakeRect("Grid", boardFrame);
            placedLayer = MakeRect("Placed", grid);
            Stretch(placedLayer);
            ghost = MakeRect("Ghost", grid);
            ghostImage = ghost.gameObject.AddComponent<Image>();
            ghostImage.raycastTarget = false;
            ghostImage.type = Image.Type.Sliced;
            ghost.gameObject.SetActive(false);

            coach = MakeRect("Coach", root);
            var coachImage = coach.gameObject.AddComponent<Image>();
            coachImage.color = Hex("1A1A1A");
            coachImage.raycastTarget = true;
            coachLabel = MakeText(coach, "", 34, Color.white, fontBold);
            coachLabel.alignment = TextAlignmentOptions.Center;
            coachLabel.enableWordWrapping = true;
            coachLabel.overflowMode = TextOverflowModes.Overflow;
            Stretch(coachLabel.rectTransform, 20f);
            coachGroup = coach.gameObject.AddComponent<CanvasGroup>();
            BindButton(coach.gameObject, DismissCoach);
            coach.gameObject.SetActive(false);

            aidLayer = MakeRect("Aids", root);
            Stretch(aidLayer);
            var aidBlock = aidLayer.gameObject.AddComponent<Image>();
            aidBlock.color = Color.clear;
            aidBlock.raycastTarget = false;

            var tray = MakeRect("Tray", root);
            trayImage = tray.gameObject.AddComponent<Image>();
            trayImage.raycastTarget = true;
            if (traySprite != null)
            {
                trayImage.sprite = traySprite;
                trayImage.type = Image.Type.Sliced;
                trayImage.color = Color.white;
            }
            else
            {
                trayImage.color = Hex("282C33");
            }
            for (int i = 0; i < 8; i++)
            {
                var slot = new TraySlot();
                slot.root = MakeRect("Slot" + i, tray);
                slot.slabRect = MakeRect("Slab", slot.root);
                slot.slab = slot.slabRect.gameObject.AddComponent<Image>();
                slot.slab.type = Image.Type.Sliced;
                slot.slab.raycastTarget = false;
                slot.label = MakeText(slot.root, "", 32, Color.white, fontSemiBold);
                slot.hit = MakeRect("Hit", slot.root);
                Stretch(slot.hit);
                var hitImage = slot.hit.gameObject.AddComponent<Image>();
                hitImage.color = Color.clear;
                hitImage.raycastTarget = true;
                slot.control = slot.hit.gameObject.AddComponent<PieceControl>();
                slot.control.screen = this;
                slot.hit.SetAsLastSibling();
                slots.Add(slot);
            }

            var undo = MakeCandyButton(root, "Undo", undoButtonSprite, undoIconSprite, "Undo");
            undoGroup = undo.gameObject.AddComponent<CanvasGroup>();
            BindButton(undo.gameObject, OnUndo);
            var reset = MakeCandyButton(root, "Reset", resetButtonSprite, resetIconSprite, "Reset");
            BindButton(reset.gameObject, ShowReset);

            drag = MakeRect("Drag", root);
            dragShadow = MakeRect("Shadow", drag);
            Stretch(dragShadow, -10f);
            dragShadow.anchoredPosition = new Vector2(0f, -14f);
            var shadowImage = dragShadow.gameObject.AddComponent<Image>();
            shadowImage.color = new Color(0f, 0f, 0f, 0.28f);
            shadowImage.raycastTarget = false;
            var dragFace = MakeRect("Face", drag);
            Stretch(dragFace);
            dragImage = dragFace.gameObject.AddComponent<Image>();
            dragImage.raycastTarget = false;
            dragImage.type = Image.Type.Sliced;
            drag.gameObject.SetActive(false);

            rotateButton = MakeButton(root, "Rotate", "yellow", out var rotateText);
            rotateText.text = "Rotate";
            rotateText.color = Hex("1A1A1A");
            BindButton(rotateButton.gameObject, RotateDragged);
            rotateButton.gameObject.SetActive(false);

            messageText = MakeText(root, "", 36, Color.white, fontBold);
            BuildPopups();
            confirmPanel = MakePopup(root, "Reset this level?", out _);
            var yes = MakeButton(confirmPanel, "Yes", "red", out var yesText);
            yesText.text = "Reset";
            yesText.color = Color.white;
            BindButton(yes.gameObject, ConfirmReset);
            var no = MakeButton(confirmPanel, "No", "blue", out var noText);
            noText.text = "Cancel";
            noText.color = Color.white;
            BindButton(no.gameObject, CancelReset);
            confirmPanel.gameObject.SetActive(false);
            BindButton(Find("Hint").gameObject, ShowHintPopup);
            BindButton(Find("Gear").gameObject, OnGear);
            yes.anchorMin = new Vector2(0.08f, 0.08f);
            yes.anchorMax = new Vector2(0.48f, 0.32f);
            yes.offsetMin = yes.offsetMax = Vector2.zero;
            no.anchorMin = new Vector2(0.52f, 0.08f);
            no.anchorMax = new Vector2(0.92f, 0.32f);
            no.offsetMin = no.offsetMax = Vector2.zero;
        }

        void ShowHintPopup()
        {
            if (motionBusy || state.level == null || state.IsComplete())
                return;
            useHintLabel.text = state.hintsLeft > 1000 ? "Use Hint (Unlimited)" : "Use Hint (" + state.hintsLeft + " left)";
            ShowOverlay(hintPanel);
        }

        void UseHint()
        {
            if (motionBusy)
                return;
            if (hintRoutine != null)
                StopCoroutine(hintRoutine);
            hintRoutine = StartCoroutine(PlayHintGuide());
        }

        void ReplayLevel()
        {
            if (motionBusy)
                return;
            StartCoroutine(ChangeLevelRoutine(levelIndex));
        }

        void HideOverlay()
        {
            if (overlayRoutine != null)
                StopCoroutine(overlayRoutine);
            if (overlay == null || !overlay.gameObject.activeSelf)
            {
                HideOverlayNow();
                return;
            }
            overlayRoutine = StartCoroutine(HideOverlayRoutine());
        }

        void HideOverlayNow()
        {
            if (overlayRoutine != null)
            {
                StopCoroutine(overlayRoutine);
                overlayRoutine = null;
            }
            if (winPanel != null)
            {
                winPanel.gameObject.SetActive(false);
                winPanel.localScale = Vector3.one;
            }
            if (hintPanel != null)
            {
                hintPanel.gameObject.SetActive(false);
                hintPanel.localScale = Vector3.one;
            }
            if (overlay != null)
                overlay.gameObject.SetActive(false);
            if (overlayGroup != null)
            {
                overlayGroup.alpha = 0f;
                overlayGroup.blocksRaycasts = false;
            }
        }

        void ShowOverlay(RectTransform panel)
        {
            if (overlay == null || panel == null)
                return;
            if (winPanel != null)
                winPanel.gameObject.SetActive(panel == winPanel);
            if (hintPanel != null)
                hintPanel.gameObject.SetActive(panel == hintPanel);
            overlay.gameObject.SetActive(true);
            overlay.SetAsLastSibling();
            if (overlayGroup != null)
                overlayGroup.blocksRaycasts = true;
            if (overlayRoutine != null)
                StopCoroutine(overlayRoutine);
            overlayRoutine = StartCoroutine(ShowOverlayRoutine(panel));
        }

        IEnumerator ShowOverlayRoutine(RectTransform panel)
        {
            if (overlayGroup == null)
                overlayGroup = overlay.gameObject.AddComponent<CanvasGroup>();
            float from = overlayGroup.alpha;
            panel.localScale = Vector3.one * 0.84f;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * 6.5f;
                float u = Mathf.Clamp01(t);
                overlayGroup.alpha = Mathf.Lerp(from, 1f, EaseOutCubic(u));
                panel.localScale = Vector3.LerpUnclamped(Vector3.one * 0.84f, Vector3.one, EaseOutBack(u));
                yield return null;
            }
            overlayGroup.alpha = 1f;
            panel.localScale = Vector3.one;
            overlayRoutine = null;
        }

        IEnumerator HideOverlayRoutine()
        {
            if (overlay == null || overlayGroup == null || !overlay.gameObject.activeSelf)
            {
                HideOverlayNow();
                yield break;
            }
            var panel = winPanel != null && winPanel.gameObject.activeSelf ? winPanel : hintPanel;
            float from = overlayGroup.alpha;
            Vector3 fromScale = panel != null ? panel.localScale : Vector3.one;
            overlayGroup.blocksRaycasts = false;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * 8f;
                float u = Mathf.Clamp01(t);
                overlayGroup.alpha = Mathf.Lerp(from, 0f, EaseOutCubic(u));
                if (panel != null)
                    panel.localScale = Vector3.Lerp(fromScale, Vector3.one * 0.92f, u);
                yield return null;
            }
            HideOverlayNow();
        }

        IEnumerator PlayHintGuide()
        {
            if (state.level == null)
            {
                HideOverlay();
                yield break;
            }
            var step = state.PeekHint();
            if (step == null)
            {
                HideOverlay();
                messageText.text = state.IsComplete() ? "Board is already complete" : "Almost there!";
                hintRoutine = null;
                yield break;
            }

            motionBusy = true;
            if (overlayRoutine != null)
            {
                StopCoroutine(overlayRoutine);
                overlayRoutine = null;
            }
            fingerDragStopped = true;
            if (aidRoutine != null)
            {
                StopCoroutine(aidRoutine);
                aidRoutine = null;
            }
            yield return HideOverlayRoutine();

            var blockers = new List<string>();
            for (int r = step.row; r < step.row + step.h; r++)
            {
                for (int c = step.col; c < step.col + step.w; c++)
                {
                    var owner = state.CellOwner(r, c, step.piece);
                    if (owner != null && owner != "black" && state.placed.ContainsKey(owner) && !state.placed[owner].locked && !blockers.Contains(owner))
                        blockers.Add(owner);
                }
            }
            foreach (var blocker in blockers)
            {
                var board = placedLayer.Find(blocker) as RectTransform;
                var home = SlotById(blocker);
                if (board != null && home != null)
                {
                    var bump = MakeFlyer(blocker, PointInRoot(board), board.sizeDelta);
                    yield return Motion.Arc(bump, PointInRoot(board), PointInRoot(home.slabRect), board.sizeDelta, home.slabRect.sizeDelta, 20f, 0.16f);
                    Destroy(bump.gameObject);
                }
            }

            if (state.PrepareHint(step))
                Refresh();

            var slot = SlotById(step.piece);
            Vector2 from = slot != null ? PointInRoot(slot.slabRect) : GridPointInRoot(step.col, step.row, step.w, step.h);
            Vector2 fromSize = slot != null ? slot.slabRect.sizeDelta : new Vector2(Span(step.w), Span(step.h));
            Vector2 to = GridPointInRoot(step.col, step.row, step.w, step.h);
            Vector2 toSize = new Vector2(Span(step.w), Span(step.h));

            if (slot != null)
            {
                var trayGroup = slot.slabRect.GetComponent<CanvasGroup>();
                if (trayGroup == null)
                    trayGroup = slot.slabRect.gameObject.AddComponent<CanvasGroup>();
                trayGroup.alpha = 0.18f;
            }

            var mark = MakeRect("HintTarget", grid);
            PlaceOnGrid(mark, step.col, step.row, step.w, step.h);
            var markFill = MakeRect("Fill", mark);
            Stretch(markFill);
            var fillImage = markFill.gameObject.AddComponent<Image>();
            fillImage.color = new Color(1f, 0.82f, 0.12f, 0.28f);
            fillImage.raycastTarget = false;
            float thick = Mathf.Max(5f, pitch * 0.08f);
            AddTargetEdge(mark, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -thick));
            AddTargetEdge(mark, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, thick));
            AddTargetEdge(mark, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(thick, 0f));
            AddTargetEdge(mark, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-thick, 0f));
            var markGroup = mark.gameObject.AddComponent<CanvasGroup>();
            markGroup.blocksRaycasts = false;
            aidPulses.Add(markGroup);

            var flyer = MakeRect("HintFly", root);
            flyer.anchorMin = flyer.anchorMax = new Vector2(0.5f, 0.5f);
            flyer.pivot = new Vector2(0.5f, 0.5f);
            flyer.sizeDelta = fromSize;
            flyer.anchoredPosition = from;
            flyer.SetAsLastSibling();
            var flyerImage = flyer.gameObject.AddComponent<Image>();
            flyerImage.raycastTarget = false;
            SetPiece(flyerImage, ColorOf(step.piece));
            var flyerGroup = flyer.gameObject.AddComponent<CanvasGroup>();
            flyerGroup.blocksRaycasts = false;
            flyerGroup.alpha = 0.95f;

            for (int i = 0; i < 3; i++)
            {
                yield return Motion.Alpha(markGroup, 1f, 0.08f);
                yield return Motion.Alpha(markGroup, 0.35f, 0.08f);
            }
            markGroup.alpha = 1f;
            yield return Motion.Arc(flyer, from, to, fromSize, toSize, 48f, 0.42f);

            if (mark != null)
                Destroy(mark.gameObject);
            if (flyer != null)
                Destroy(flyer.gameObject);

            if (!state.CommitHint(step))
                messageText.text = "Almost there!";
            else
                messageText.text = "";
            Refresh();
            var landed = placedLayer.Find(step.piece) as RectTransform;
            if (landed != null)
                yield return PlayLandImpact(landed);
            if (state.IsComplete())
                ShowWin();
            motionBusy = false;
            hintRoutine = null;
        }

        public void PointerDown(PieceControl control, PointerEventData eventData)
        {
            if (boardIntroActive || motionBusy)
                return;
            if (dragging)
                EndDrag();
            active = control;
            pointerStart = eventData.position;
            moved = false;
            dragging = false;
        }

        public void PointerDrag(PieceControl control, PointerEventData eventData)
        {
            if (motionBusy || active != control)
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
            if (motionBusy || active != control)
                return;
            if (dragging || moved)
                EndDrag();
            else if (control.onBoard)
                StartCoroutine(TapReturnToTray(control));
            else if (state.MustRotate(control.pieceId) && state.RotateTray(control.pieceId))
            {
                fingerTapStopped = true;
                Refresh();
            }
            active = null;
        }

        void BeginDrag(PieceControl control, PointerEventData eventData)
        {
            if (control.onBoard && state.placed.ContainsKey(control.pieceId) && state.placed[control.pieceId].locked)
                return;
            fingerDragStopped = true;
            if (aidRoutine != null)
            {
                StopCoroutine(aidRoutine);
                aidRoutine = null;
            }
            if (aidLayer != null)
            {
                for (int i = aidLayer.childCount - 1; i >= 0; i--)
                    Destroy(aidLayer.GetChild(i).gameObject);
            }
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
            dragH = h;
            dragW = w;
            drag.sizeDelta = new Vector2(Span(w), Span(h));
            SetPiece(dragImage, ColorOf(control.pieceId));
            drag.localScale = Vector3.one * 1.06f;
            drag.localRotation = Quaternion.identity;
            drag.gameObject.SetActive(true);
            drag.SetAsLastSibling();
            ghostCol = -99;
            ghostRow = -99;
            ghostBlend = 1f;
            var slot = SlotOf(control);
            var hide = slot != null ? slot.slabRect : HideTarget(control);
            var group = hide.GetComponent<CanvasGroup>();
            if (group == null)
                group = hide.gameObject.AddComponent<CanvasGroup>();
            group.alpha = slot != null ? 0.28f : 0f;
            group.blocksRaycasts = false;
            hiddenGroup = group;
            rotateButton.gameObject.SetActive(!control.onBoard && state.MustRotate(control.pieceId));
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
            if (!overBoard)
                return;
            SetPiece(ghostImage, ColorOf(active.pieceId));
            if (valid)
            {
                Color c = ghostImage.color;
                ghostImage.color = new Color(c.r, c.g, c.b, 0.58f);
            }
            else
                ghostImage.color = new Color(1f, 0.36f, 0.42f, 0.55f);
            Vector2 next = GhostAnchor(col, row, dragW, dragH);
            ghost.sizeDelta = new Vector2(dragW * pitch - gap, dragH * pitch - gap);
            if (ghostCol != col || ghostRow != row)
            {
                ghostFrom = ghostCol < -50 ? next : ghost.anchoredPosition;
                ghostTo = next;
                ghostBlend = ghostCol < -50 ? 1f : 0f;
                ghostCol = col;
                ghostRow = row;
            }
            StepGhost();
        }

        Vector2 GhostAnchor(int col, int row, int w, int h)
        {
            float pad = gap * 0.5f;
            return new Vector2(col * pitch + pad, -(row * pitch + pad));
        }

        void StepGhost()
        {
            if (ghost == null || !ghost.gameObject.activeSelf)
                return;
            ghostBlend = Mathf.Min(1f, ghostBlend + Time.unscaledDeltaTime / 0.08f);
            ghost.anchorMin = ghost.anchorMax = new Vector2(0f, 1f);
            ghost.pivot = new Vector2(0f, 1f);
            ghost.anchoredPosition = Vector2.LerpUnclamped(ghostFrom, ghostTo, Motion.OutCubic(ghostBlend));
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
            ghost.gameObject.SetActive(false);
            rotateButton.gameObject.SetActive(false);
            if (dragRoutine != null)
                StopCoroutine(dragRoutine);
            dragRoutine = StartCoroutine(SettleDrag(piece, origin, row, col));
        }

        IEnumerator SettleDrag(PieceControl piece, PlacedPiece origin, int row, int col)
        {
            motionBusy = true;
            bool onTray = ReleasedOnTray();
            bool canPlace = piece != null && state.CanPlace(piece.pieceId, row, col, dragH, dragW);
            Vector2 dest = PointInRoot(drag);
            if (piece != null && origin != null && onTray)
            {
                var slot = SlotById(piece.pieceId);
                if (slot != null)
                    dest = PointInRoot(slot.slabRect);
            }
            else if (canPlace)
                dest = GridPointInRoot(col, row, dragW, dragH);
            else if (origin != null)
                dest = GridPointInRoot(origin.col, origin.row, origin.w, origin.h);
            else
            {
                var slot = piece != null ? SlotById(piece.pieceId) : null;
                if (slot != null)
                    dest = PointInRoot(slot.slabRect);
            }

            Vector2 start = PointInRoot(drag);
            drag.anchorMin = drag.anchorMax = new Vector2(0.5f, 0.5f);
            drag.pivot = new Vector2(0.5f, 0.5f);
            drag.anchoredPosition = start;
            drag.localScale = Vector3.one;
            if (!canPlace && !onTray)
            {
                yield return Motion.ShakeX(drag, 10f, 0.1f);
                yield return Motion.ColorTo(dragImage, new Color(1f, 0.4f, 0.42f, 1f), 0.06f);
            }
            if (onTray)
            {
                Vector2 endSize = drag.sizeDelta * 0.72f;
                StartCoroutine(Motion.Size(drag, endSize, 0.18f));
            }
            yield return Motion.Move(drag, dest, canPlace && !onTray ? 0.16f : 0.2f);

            if (hiddenGroup != null)
            {
                hiddenGroup.alpha = 1f;
                hiddenGroup.blocksRaycasts = true;
                hiddenGroup = null;
            }
            drag.gameObject.SetActive(false);
            if (piece == null)
            {
                motionBusy = false;
                Refresh();
                yield break;
            }
            if (origin != null && onTray)
                state.Remember(origin);
            else
            {
                bool placed = state.TryPlace(piece.pieceId, row, col, dragH, dragW, origin == null);
                if (placed)
                {
                    if (origin != null && (row != origin.row || col != origin.col || dragH != origin.h || dragW != origin.w))
                        state.Remember(origin);
                }
                else if (origin != null)
                    state.TryPlace(origin.id, origin.row, origin.col, origin.h, origin.w, false);
            }
            Refresh();
            if (canPlace && !onTray && piece != null)
            {
                var landed = placedLayer.Find(piece.pieceId) as RectTransform;
                if (landed != null)
                    yield return PlayLandImpact(landed);
            }
            if (state.IsComplete())
                ShowWin();
            motionBusy = false;
            dragRoutine = null;
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
            var cell = MakeRect("Empty_" + col + "_" + row, grid);
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
            if (motionBusy || rotating || active == null || active.onBoard || !state.MustRotate(active.pieceId))
            {
                if (active == null || active.onBoard)
                    rotateButton.gameObject.SetActive(false);
                return;
            }
            if (rotateRoutine != null)
                StopCoroutine(rotateRoutine);
            rotateRoutine = StartCoroutine(RotateDragRoutine());
        }

        IEnumerator RotateDragRoutine()
        {
            rotating = true;
            if (state.RotateTray(active.pieceId))
                fingerTapStopped = true;
            yield return Motion.RotateZ(drag, 0f, -90f, 0.14f);
            state.Oriented(active.pieceId, state.trayTurns[active.pieceId], out dragH, out dragW);
            drag.sizeDelta = new Vector2(Span(dragW), Span(dragH));
            drag.localRotation = Quaternion.identity;
            SetPiece(dragImage, ColorOf(active.pieceId));
            grabCol = Mathf.Clamp(grabCol, 0, dragW - 1);
            grabRow = Mathf.Clamp(grabRow, 0, dragH - 1);
            ghostCol = -99;
            rotateButton.gameObject.SetActive(state.MustRotate(active.pieceId));
            var data = new PointerEventData(EventSystem.current) { position = lastPointer };
            MoveDrag(data);
            rotating = false;
            rotateRoutine = null;
        }

        void OnUndo()
        {
            if (motionBusy || !state.CanUndo)
                return;
            StartCoroutine(UndoRoutine());
        }

        IEnumerator UndoRoutine()
        {
            motionBusy = true;
            var step = state.PeekUndo();
            if (step == null)
            {
                motionBusy = false;
                yield break;
            }
            bool restoring = step.wasPlaced;
            bool removing = !step.wasPlaced && state.IsPlaced(step.id);
            Vector2 from = Vector2.zero;
            Vector2 to = Vector2.zero;
            Vector2 fromSize = Vector2.one * 80f;
            Vector2 toSize = fromSize;
            if (removing && placedLayer.Find(step.id) is RectTransform onBoard)
            {
                from = PointInRoot(onBoard);
                fromSize = onBoard.sizeDelta;
                var slot = SlotById(step.id);
                to = slot != null ? PointInRoot(slot.slabRect) : from;
                toSize = slot != null ? slot.slabRect.sizeDelta : fromSize * 0.7f;
            }
            else if (restoring)
            {
                var slot = SlotById(step.id);
                from = slot != null ? PointInRoot(slot.slabRect) : Vector2.zero;
                fromSize = slot != null ? slot.slabRect.sizeDelta : fromSize;
                to = GridPointInRoot(step.col, step.row, step.w, step.h);
                toSize = new Vector2(Span(step.w), Span(step.h));
            }
            RectTransform flyer = null;
            if (removing || restoring)
            {
                flyer = MakeFlyer(step.id, from, fromSize);
                yield return Motion.Arc(flyer, from, to, fromSize, toSize, 28f, 0.2f);
            }
            if (flyer != null)
                Destroy(flyer.gameObject);
            state.Undo();
            Refresh();
            motionBusy = false;
        }

        void ShowReset()
        {
            if (motionBusy || !state.HasMovablePlaced())
                return;
            confirmPanel.gameObject.SetActive(true);
            confirmPanel.SetAsLastSibling();
            confirmPanel.localScale = Vector3.one * 0.84f;
            StartCoroutine(Motion.PopIn(confirmPanel, 0.24f));
        }

        void CancelReset()
        {
            if (confirmPanel != null)
                confirmPanel.gameObject.SetActive(false);
        }

        void ConfirmReset()
        {
            if (motionBusy)
                return;
            StartCoroutine(ResetRoutine());
        }

        IEnumerator ResetRoutine()
        {
            motionBusy = true;
            var ids = state.UnlockedIds();
            confirmPanel.gameObject.SetActive(false);
            foreach (var id in ids)
            {
                var board = placedLayer.Find(id) as RectTransform;
                var slot = SlotById(id);
                if (board == null || slot == null)
                    continue;
                var flyer = MakeFlyer(id, PointInRoot(board), board.sizeDelta);
                StartCoroutine(Motion.Arc(flyer, PointInRoot(board), PointInRoot(slot.slabRect), board.sizeDelta, slot.slabRect.sizeDelta, 24f, 0.18f));
                yield return Motion.Wait(0.04f);
            }
            yield return Motion.Wait(0.18f);
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                var child = root.GetChild(i);
                if (child.name == "HintFly")
                    Destroy(child.gameObject);
            }
            state.ResetUnlocked();
            RestoreCoach();
            Refresh();
            motionBusy = false;
        }

        IEnumerator TapReturnToTray(PieceControl control)
        {
            if (control == null || !state.IsPlaced(control.pieceId) || (state.placed.ContainsKey(control.pieceId) && state.placed[control.pieceId].locked))
                yield break;
            motionBusy = true;
            var board = placedLayer.Find(control.pieceId) as RectTransform;
            var slot = SlotById(control.pieceId);
            if (board != null && slot != null)
            {
                var flyer = MakeFlyer(control.pieceId, PointInRoot(board), board.sizeDelta);
                yield return Motion.Arc(flyer, PointInRoot(board), PointInRoot(slot.slabRect), board.sizeDelta, slot.slabRect.sizeDelta, 22f, 0.18f);
                Destroy(flyer.gameObject);
            }
            state.ReturnToTray(control.pieceId, true);
            Refresh();
            motionBusy = false;
        }

        RectTransform MakeFlyer(string id, Vector2 pos, Vector2 size)
        {
            var flyer = MakeRect("HintFly", root);
            flyer.anchorMin = flyer.anchorMax = new Vector2(0.5f, 0.5f);
            flyer.pivot = new Vector2(0.5f, 0.5f);
            flyer.anchoredPosition = pos;
            flyer.sizeDelta = size;
            flyer.SetAsLastSibling();
            var image = flyer.gameObject.AddComponent<Image>();
            image.raycastTarget = false;
            SetPiece(image, ColorOf(id));
            return flyer;
        }

        void OnGear()
        {
            var gear = Find("Gear");
            if (gear == null)
                return;
            var icon = gear.Find("Icon");
            if (icon != null)
                StartCoroutine(GearNudge(icon));
        }

        IEnumerator GearNudge(Transform icon)
        {
            yield return Motion.RotateZ(icon, 0f, -42f, 0.1f);
            yield return Motion.RotateZ(icon, -42f, 0f, 0.12f);
        }

        void DismissCoach()
        {
            coachHidden = true;
            StartCoroutine(HideCoach());
        }

        IEnumerator HideCoach()
        {
            if (coachGroup != null)
                yield return Motion.Alpha(coachGroup, 0f, 0.16f);
            if (coach != null)
                coach.gameObject.SetActive(false);
            Layout();
        }

        void BreatheHint()
        {
            var hint = Find("Hint");
            if (hint == null)
                return;
            var bulb = hint.Find("Bulb");
            if (bulb == null)
                return;
            float s = 1f + 0.03f * Mathf.Sin(Time.unscaledTime * 2.2f);
            bulb.localScale = Vector3.one * s;
        }

        void NextLevel()
        {
            if (motionBusy)
                return;
            int next = levelIndex >= allLevels.Count - 1 ? 0 : levelIndex + 1;
            StartCoroutine(ChangeLevelRoutine(next));
        }

        IEnumerator ChangeLevelRoutine(int index)
        {
            motionBusy = true;
            yield return HideOverlayRoutine();
            ShowLevel(index);
        }

        void ShowLevel(int index)
        {
            levelIndex = Mathf.Clamp(index, 0, Mathf.Max(0, allLevels.Count - 1));
            state.Begin(allLevels[levelIndex], catalogPieces);
            var trayIds = state.TrayIds();
            for (int i = 0; i < slots.Count; i++)
            {
                if (i < trayIds.Count && state.Find(trayIds[i]) != null)
                {
                    var piece = state.Find(trayIds[i]);
                    slots[i].id = piece.id;
                    slots[i].control.pieceId = piece.id;
                    slots[i].label.text = piece.h + " × " + piece.w;
                    SetPiece(slots[i].slab, piece.color);
                    slots[i].root.gameObject.SetActive(true);
                }
                else
                {
                    slots[i].id = null;
                    slots[i].root.gameObject.SetActive(false);
                }
            }
            levelWord.text = "Level";
            levelNumber.text = (levelIndex + 1).ToString();
            messageText.text = "";
            fingerDragStopped = false;
            fingerTapStopped = false;
            celebrating = false;
            boardIntroActive = true;
            motionBusy = true;
            HideOverlayNow();
            RestoreCoach();
            Refresh();
            if (introRoutine != null)
                StopCoroutine(introRoutine);
            introRoutine = StartCoroutine(LevelIntro());
        }

        void Refresh()
        {
            Layout();
            hintText.text = "∞";
            foreach (var slot in slots)
            {
                bool placed = slot.id != null && state.IsPlaced(slot.id);
                bool inTray = slot.id != null && !placed;
                slot.slab.gameObject.SetActive(inTray);
                slot.hit.gameObject.SetActive(inTray);
                var slabGroup = slot.slabRect.GetComponent<CanvasGroup>();
                if (slabGroup != null)
                {
                    slabGroup.alpha = 1f;
                    slabGroup.blocksRaycasts = true;
                }
                slot.control.pieceId = slot.id;
                slot.control.onBoard = false;
                slot.label.color = placed ? new Color(1f, 1f, 1f, 0.35f) : Color.white;
                if (inTray)
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
                image.raycastTarget = false;
                SetPiece(image, ColorOf(piece.id));
                cell.gameObject.AddComponent<CanvasGroup>();
                if (!piece.locked)
                    AddBoardHit(cell, piece);
                else if (!state.IsPreplaced(piece.id))
                    AddHintBadge(cell);
            }
            DrawTutorialAids();
            UpdateUndoVisual();
            if (state.IsComplete())
            {
                if (!motionBusy)
                    ShowWin();
            }
            else
            {
                celebrating = false;
                if (hintPanel == null || !hintPanel.gameObject.activeSelf)
                    HideOverlayNow();
            }
        }

        void UpdateUndoVisual()
        {
            if (undoGroup == null)
                return;
            bool can = state.CanUndo;
            undoGroup.alpha = can ? 1f : 0.42f;
            undoGroup.blocksRaycasts = can;
        }

        void AddBoardHit(RectTransform cell, PlacedPiece piece)
        {
            var hit = MakeRect("Hit", cell);
            Stretch(hit);
            float extraX = piece.w == 1 ? pitch * 0.4f : pitch * 0.12f;
            float extraY = piece.h == 1 ? pitch * 0.4f : pitch * 0.12f;
            hit.offsetMin = new Vector2(-extraX, -extraY);
            hit.offsetMax = new Vector2(extraX, extraY);
            var image = hit.gameObject.AddComponent<Image>();
            image.color = Color.clear;
            image.raycastTarget = true;
            var control = hit.gameObject.AddComponent<PieceControl>();
            control.screen = this;
            control.pieceId = piece.id;
            control.onBoard = true;
        }

        TraySlot SlotOf(PieceControl control)
        {
            foreach (var slot in slots)
            {
                if (slot.control == control)
                    return slot;
            }
            return null;
        }

        TraySlot SlotById(string id)
        {
            if (string.IsNullOrEmpty(id))
                return null;
            foreach (var slot in slots)
            {
                if (slot.id == id)
                    return slot;
            }
            return null;
        }

        RectTransform HideTarget(PieceControl control)
        {
            var slot = SlotOf(control);
            if (slot != null)
                return slot.root;
            if (control.onBoard && control.transform.parent is RectTransform parent)
                return parent;
            return control.transform as RectTransform;
        }

        void FitTraySlab(TraySlot slot, int h, int w)
        {
            float unit = Mathf.Min((slot.root.rect.width * 0.9f) / Mathf.Max(1, w), (slot.root.rect.height * 0.62f) / Mathf.Max(1, h));
            if (unit <= 1f)
                unit = 48f;
            slot.slabRect.anchorMin = slot.slabRect.anchorMax = new Vector2(0.5f, 0.62f);
            slot.slabRect.pivot = new Vector2(0.5f, 0.5f);
            slot.slabRect.anchoredPosition = Vector2.zero;
            slot.slabRect.sizeDelta = new Vector2(unit * w, unit * h);
            SetPiece(slot.slab, ColorOf(slot.id));
            slot.label.rectTransform.anchorMin = new Vector2(0f, 0f);
            slot.label.rectTransform.anchorMax = new Vector2(1f, 0.24f);
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
            float pad = 40f;
            float width = Mathf.Min(max.x - min.x - pad * 2f, 1000f);
            float left = (min.x + max.x) * 0.5f - width * 0.5f;
            float top = max.y - pad;
            float bottom = min.y + pad;
            float edge = 50f;
            float headerScale = Mathf.Min(1f, (max.x - min.x) / ReferenceWidth);
            float hintSize = HeaderButtonSize * headerScale;
            float pillW = LevelPillWidth * headerScale;
            float pillH = LevelPillHeight * headerScale;
            float topH = pillH;
            float buttonH = 128f;
            float numberH = 40f;
            int trayCount = 0;
            foreach (var slot in slots)
            {
                if (slot.root.gameObject.activeSelf)
                    trayCount++;
            }
            bool oneRow = trayCount <= 4;
            float trayH = oneRow ? Mathf.Clamp(width * 0.28f, 240f, 320f) : Mathf.Clamp(width * 0.42f, 360f, 480f);
            bool showCoach = coach != null && coach.gameObject.activeSelf;
            float coachH = showCoach ? 110f : 0f;
            float spacing = 20f;
            float used = topH + coachH + numberH + trayH + buttonH + spacing * (showCoach ? 5f : 4f);
            float boardBudget = top - bottom - used;
            float boardSize = Mathf.Min(width - numberH, boardBudget);
            boardSize = Mathf.Max(160f, boardSize);

            PlaceCentered(Find("Hint"), min.x + edge, top - hintSize, hintSize, hintSize);
            PlaceCentered(Find("Gear"), max.x - edge - hintSize, top - hintSize, hintSize, hintSize);
            Place(Find("Level"), (min.x + max.x) * 0.5f - pillW * 0.5f, top - pillH, pillW, pillH);
            var levelImage = Find("Level").GetComponent<Image>();
            FitSlice(levelImage, 68f, pillH * 0.5f);
            FitSlice(Find("Hint").GetComponent<Image>(), 36f, hintSize * 0.28f);
            FitSlice(Find("Gear").GetComponent<Image>(), 36f, hintSize * 0.28f);
            var badge = Find("Hint").Find("Badge") as RectTransform;
            float badgeSize = HintBadgeSize * headerScale;
            badge.sizeDelta = new Vector2(badgeSize, badgeSize);
            hintText.fontSize = Mathf.RoundToInt(32f * headerScale);
            LayoutLevelTitle(pillW, pillH, headerScale);

            float afterHeader = top - topH - spacing;
            if (showCoach)
            {
                Place(coach, left, afterHeader - coachH, width, coachH);
                afterHeader -= coachH + spacing;
            }
            float frameLeft = left + numberH;
            float boardTop = afterHeader - numberH;
            float boardBottom = boardTop - boardSize;
            Place(boardFrame, frameLeft, boardBottom, boardSize, boardSize);
            float rim = Mathf.Clamp(boardSize * 0.045f, 22f, 40f);
            if (frameImage.sprite != null)
            {
                frameImage.type = Image.Type.Simple;
                frameImage.preserveAspect = false;
            }
            grid.anchorMin = grid.anchorMax = new Vector2(0f, 1f);
            grid.pivot = new Vector2(0f, 1f);
            grid.anchoredPosition = new Vector2(rim, -rim);
            grid.sizeDelta = new Vector2(boardSize - rim * 2f, boardSize - rim * 2f);
            pitch = grid.sizeDelta.x / GameState.Size;
            gap = 4f;

            float trayTop = boardBottom - spacing;
            Place(Find("Tray"), left, trayTop - trayH, width, trayH);
            FitSlice(trayImage, 56f, 42f);
            LayoutTray(Find("Tray"));

            float buttonTop = trayTop - trayH - spacing;
            float buttonGap = Mathf.Max(20f, width * 0.04f);
            float buttonW = (width - buttonGap) * 0.5f;
            PlaceCentered(Find("Undo"), left, buttonTop - buttonH, buttonW, buttonH);
            PlaceCentered(Find("Reset"), left + buttonW + buttonGap, buttonTop - buttonH, buttonW, buttonH);
            LayoutCandyButton(Find("Undo"), buttonH);
            LayoutCandyButton(Find("Reset"), buttonH);
            Place(messageText.rectTransform, left, buttonTop - buttonH - 48f, width, 44f);
            Place(rotateButton, left + width * 0.28f, trayTop - 88f, width * 0.44f, 80f);

            if (overlay != null)
            {
                Stretch(overlay);
                overlay.SetAsLastSibling();
            }
            float cardW = Mathf.Min(860f, width * 0.9f);
            float cardH = Mathf.Min(1040f, cardW * 1.22f);
            SizeCentered(winPanel, cardW, cardH);
            SizeCentered(hintPanel, cardW, cardH);
            confirmPanel.anchorMin = confirmPanel.anchorMax = new Vector2(0.5f, 0.5f);
            confirmPanel.sizeDelta = new Vector2(Mathf.Min(820f, width), 420f);
            FitSlice(winPanel.GetComponent<Image>(), 48f, 36f);
            FitSlice(hintPanel.GetComponent<Image>(), 48f, 36f);
            LayoutCompleteTitle();
            LayoutWinStars();
            LayoutStackedGlow(hintPanel.Find("Bulb") as RectTransform);
            LayoutStackedGlow(Find("Hint").Find("Bulb") as RectTransform);
            LayoutPill(winPanel.Find("Next") as RectTransform, 110f);
            LayoutPill(winPanel.Find("Replay") as RectTransform, 110f);
            LayoutPill(hintPanel.Find("UseHint") as RectTransform, 110f);
            LayoutPill(hintPanel.Find("MoreHints") as RectTransform, 110f);

            DrawNumbers(frameLeft, boardTop, rim, numberH);
            foreach (var slot in slots)
            {
                if (slot.id != null && !state.IsPlaced(slot.id))
                {
                    state.Oriented(slot.id, state.trayTurns[slot.id], out int h, out int w);
                    FitTraySlab(slot, h, w);
                }
            }
        }

        void DrawNumbers(float frameLeft, float frameTop, float rim, float numberH)
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
                var col = MakeText(numbers, (i + 1).ToString(), 32, new Color(1f, 1f, 1f, 0.9f), fontSemiBold);
                col.rectTransform.anchorMin = col.rectTransform.anchorMax = new Vector2(0f, 1f);
                col.rectTransform.pivot = new Vector2(0f, 1f);
                col.rectTransform.anchoredPosition = new Vector2(frameLeft + originX + rim + i * pitch, frameTop + originY + numberH);
                col.rectTransform.sizeDelta = new Vector2(pitch, numberH);
                var row = MakeText(numbers, (i + 1).ToString(), 32, new Color(1f, 1f, 1f, 0.9f), fontSemiBold);
                row.rectTransform.anchorMin = row.rectTransform.anchorMax = new Vector2(0f, 1f);
                row.rectTransform.pivot = new Vector2(0f, 1f);
                row.rectTransform.anchoredPosition = new Vector2(frameLeft + originX - numberH, frameTop + originY - rim - i * pitch);
                row.rectTransform.sizeDelta = new Vector2(numberH, pitch);
            }
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
            int rows = count <= 4 ? 1 : 2;
            float insetX = Mathf.Clamp(tray.rect.width * 0.04f, 28f, 48f);
            float insetY = Mathf.Clamp(tray.rect.height * 0.08f, 18f, 32f);
            float innerW = Mathf.Max(8f, tray.rect.width - insetX * 2f);
            float innerH = Mathf.Max(8f, tray.rect.height - insetY * 2f);
            float cellW = innerW / 4f;
            float cellH = innerH / rows;
            int index = 0;
            foreach (var slot in slots)
            {
                if (!slot.root.gameObject.activeSelf)
                    continue;
                int row = count <= 4 ? 0 : index / 4;
                int col = count <= 4 ? index : index % 4;
                int inRow = count <= 4 ? count : (row == rows - 1 ? count - row * 4 : 4);
                float startX = insetX + (innerW - inRow * cellW) * 0.5f;
                var rect = slot.root;
                rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
                rect.pivot = new Vector2(0f, 1f);
                rect.sizeDelta = new Vector2(cellW - 10f, cellH - 10f);
                rect.anchoredPosition = new Vector2(startX + col * cellW + 5f, -(insetY + row * cellH + 5f));
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
                case "violet":
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
            image.color = PieceTint(color);
            FitSlice(image, color == "violet" || color == "white" ? 16f : 18f);
        }

        static Color PieceTint(string color)
        {
            if (color == "violet" || color == "white")
                return Hex("8B5CFF");
            return Color.white;
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

        void LoadCatalogs()
        {
            allLevels.Clear();
            var tutorialAsset = Resources.Load<TextAsset>("Data/tutorial_levels");
            var mainAsset = Resources.Load<TextAsset>("Data/levels");
            LevelCatalog tutorial = null;
            if (tutorialAsset != null)
                tutorial = JsonUtility.FromJson<LevelCatalog>(tutorialAsset.text);
            if (mainAsset != null)
                mainLevels = JsonUtility.FromJson<LevelCatalog>(mainAsset.text);
            if (tutorial != null && tutorial.levels != null)
                allLevels.AddRange(tutorial.levels);
            if (mainLevels != null && mainLevels.levels != null)
                allLevels.AddRange(mainLevels.levels);
            if (tutorial != null && tutorial.pieces != null)
                catalogPieces = tutorial.pieces;
            else if (mainLevels != null)
                catalogPieces = mainLevels.pieces;
        }

        void RestoreCoach()
        {
            coachHidden = false;
            if (coach == null)
                return;
            if (state.level != null && state.level.tutorial != null && !string.IsNullOrEmpty(state.level.tutorial.coachText))
            {
                coachLabel.text = state.level.tutorial.coachText;
                coach.gameObject.SetActive(true);
                if (coachGroup != null)
                    coachGroup.alpha = 0f;
            }
            else
            {
                coach.gameObject.SetActive(false);
            }
        }

        void AddHintBadge(RectTransform cell)
        {
            var badge = MakeRect("Hinted", cell);
            badge.anchorMin = badge.anchorMax = new Vector2(1f, 1f);
            badge.pivot = new Vector2(1f, 1f);
            badge.anchoredPosition = new Vector2(-6f, -6f);
            badge.sizeDelta = new Vector2(40f, 40f);
            var image = badge.gameObject.AddComponent<Image>();
            image.sprite = hintBulbSprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
        }

        void DrawTutorialAids()
        {
            aidPulses.Clear();
            if (aidRoutine != null)
            {
                StopCoroutine(aidRoutine);
                aidRoutine = null;
            }
            if (aidLayer != null)
            {
                for (int i = aidLayer.childCount - 1; i >= 0; i--)
                    Destroy(aidLayer.GetChild(i).gameObject);
            }
            ClearSlotDecorations();
            if (boardIntroActive || !state.IsTutorial || state.IsComplete())
                return;
            aidLayer.SetAsLastSibling();
            bool guided = state.GuidedDrag;
            if (guided && !dragging)
                fingerDragStopped = false;
            if (guided || state.HasAid("regionGlow") || state.HasAid("targetOutline") || state.HasAid("fingerDrag"))
                DrawTargetOutlines(guided || state.HasAid("fingerDrag"));
            if (guided)
                HighlightGuidePiece();
            if (state.HasAid("rotateArrow") && !fingerTapStopped)
                DrawRotateArrows();
            if ((guided || state.HasAid("fingerDrag")) && !fingerDragStopped && !dragging)
                aidRoutine = StartCoroutine(FingerDragLoop());
            else if (!guided && state.HasAid("fingerTap") && !fingerTapStopped)
                aidRoutine = StartCoroutine(FingerTapLoop());
        }

        void ClearSlotDecorations()
        {
            foreach (var slot in slots)
            {
                if (slot.root == null)
                    continue;
                for (int i = slot.root.childCount - 1; i >= 0; i--)
                {
                    var child = slot.root.GetChild(i);
                    if (child.name == "GuideRing" || child.name == "RotateAid")
                        Destroy(child.gameObject);
                }
            }
        }

        void DrawTargetOutlines(bool oneAtATime)
        {
            string focus = oneAtATime ? state.NextGuideId() : null;
            foreach (var id in state.TrayIds())
            {
                if (state.IsPlaced(id))
                    continue;
                if (focus != null && id != focus)
                    continue;
                var step = state.SolutionOf(id);
                if (step == null)
                    continue;
                DrawTargetHighlight(step);
            }
        }

        void HighlightGuidePiece()
        {
            string id = state.NextGuideId();
            if (id == null)
                return;
            foreach (var slot in slots)
            {
                if (slot.id != id)
                    continue;
                var group = slot.slabRect.GetComponent<CanvasGroup>();
                if (group == null)
                    group = slot.slabRect.gameObject.AddComponent<CanvasGroup>();
                group.blocksRaycasts = false;
                aidPulses.Add(group);
                break;
            }
        }

        void DrawTargetHighlight(SolutionPlacement step)
        {
            var wrap = MakeRect("AidTarget", grid);
            PlaceOnGrid(wrap, step.col, step.row, step.w, step.h);
            var fill = MakeRect("Fill", wrap);
            Stretch(fill);
            var fillImage = fill.gameObject.AddComponent<Image>();
            fillImage.color = new Color(1f, 0.82f, 0.12f, 0.2f);
            fillImage.raycastTarget = false;
            float thick = Mathf.Max(5f, pitch * 0.07f);
            AddTargetEdge(wrap, new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(0f, -thick));
            AddTargetEdge(wrap, new Vector2(0f, 0f), new Vector2(1f, 0f), new Vector2(0f, thick));
            AddTargetEdge(wrap, new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(thick, 0f));
            AddTargetEdge(wrap, new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-thick, 0f));
            var group = wrap.gameObject.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            aidPulses.Add(group);
        }

        void AddTargetEdge(RectTransform parent, Vector2 min, Vector2 max, Vector2 outset)
        {
            var edge = MakeRect("Edge", parent);
            edge.anchorMin = min;
            edge.anchorMax = max;
            edge.pivot = new Vector2(0.5f, 0.5f);
            edge.anchoredPosition = Vector2.zero;
            edge.offsetMin = new Vector2(Mathf.Min(0f, outset.x), Mathf.Min(0f, outset.y));
            edge.offsetMax = new Vector2(Mathf.Max(0f, outset.x), Mathf.Max(0f, outset.y));
            var image = edge.gameObject.AddComponent<Image>();
            image.color = Hex("FFD21F");
            image.raycastTarget = false;
        }

        void DrawRotateArrows()
        {
            foreach (var slot in slots)
            {
                if (slot.id == null || !state.MustRotate(slot.id))
                    continue;
                var mark = MakeRect("RotateAid", slot.root);
                mark.anchorMin = new Vector2(0.1f, 0.72f);
                mark.anchorMax = new Vector2(0.9f, 0.98f);
                mark.offsetMin = Vector2.zero;
                mark.offsetMax = Vector2.zero;
                var image = mark.gameObject.AddComponent<Image>();
                image.color = Hex("FFD21F");
                image.raycastTarget = false;
                var label = MakeText(mark, "TURN", 22, Hex("1A1A1A"), fontBlack);
                label.raycastTarget = false;
            }
        }

        IEnumerator FingerDragLoop()
        {
            TraySlot slot = null;
            foreach (var item in slots)
            {
                if (item.id != null && item.root.gameObject.activeSelf && !state.IsPlaced(item.id))
                {
                    slot = item;
                    break;
                }
            }
            if (slot == null)
                yield break;
            var step = state.SolutionOf(slot.id);
            if (step == null)
                yield break;
            var demo = MakeRect("DemoPiece", aidLayer);
            demo.anchorMin = demo.anchorMax = new Vector2(0.5f, 0.5f);
            demo.pivot = new Vector2(0.5f, 0.5f);
            var demoImage = demo.gameObject.AddComponent<Image>();
            demoImage.raycastTarget = false;
            SetPiece(demoImage, ColorOf(slot.id));
            var demoGroup = demo.gameObject.AddComponent<CanvasGroup>();
            demoGroup.blocksRaycasts = false;
            var finger = MakeFinger(aidLayer);
            Vector2 fromSize = slot.slabRect.sizeDelta;
            Vector2 toSize = new Vector2(Span(step.w), Span(step.h));
            while (demo != null && !fingerDragStopped)
            {
                Vector2 from = RootPoint(slot.slabRect);
                Vector2 to = GridRootPoint(step.col, step.row, step.w, step.h);
                var trayGroup = slot.slabRect.GetComponent<CanvasGroup>();
                if (trayGroup == null)
                    trayGroup = slot.slabRect.gameObject.AddComponent<CanvasGroup>();
                trayGroup.alpha = 0.25f;
                float t = 0f;
                while (t < 1f && demo != null && !fingerDragStopped)
                {
                    t += Time.unscaledDeltaTime * 0.62f;
                    float s = Mathf.SmoothStep(0f, 1f, Mathf.Clamp01(t));
                    demo.sizeDelta = Vector2.Lerp(fromSize, toSize, s);
                    demo.anchoredPosition = Vector2.Lerp(from, to, s);
                    if (finger != null)
                        finger.anchoredPosition = demo.anchoredPosition + new Vector2(28f, -36f);
                    yield return null;
                }
                float wait = 0f;
                while (wait < 0.45f && demo != null && !fingerDragStopped)
                {
                    wait += Time.unscaledDeltaTime;
                    yield return null;
                }
                if (demo != null)
                {
                    demo.sizeDelta = fromSize;
                    demo.anchoredPosition = from;
                }
                if (finger != null)
                    finger.anchoredPosition = from + new Vector2(28f, -36f);
                if (trayGroup != null)
                    trayGroup.alpha = 1f;
                wait = 0f;
                while (wait < 0.25f && demo != null && !fingerDragStopped)
                {
                    wait += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
            if (slot != null)
            {
                var trayGroup = slot.slabRect.GetComponent<CanvasGroup>();
                if (trayGroup != null)
                    trayGroup.alpha = 1f;
            }
            if (demo != null)
                Destroy(demo.gameObject);
            if (finger != null)
                Destroy(finger.gameObject);
        }

        IEnumerator FingerTapLoop()
        {
            TraySlot slot = null;
            foreach (var item in slots)
            {
                if (item.id != null && state.NeedsRotation(item.id) && !state.IsPlaced(item.id))
                {
                    slot = item;
                    break;
                }
            }
            if (slot == null)
            {
                foreach (var item in slots)
                {
                    if (item.id != null && !state.IsPlaced(item.id))
                    {
                        slot = item;
                        break;
                    }
                }
            }
            if (slot == null)
                yield break;
            var finger = MakeFinger(aidLayer);
            while (finger != null && !fingerTapStopped)
            {
                Vector2 at = RootPoint(slot.slabRect);
                float t = 0f;
                while (t < 1f && finger != null && !fingerTapStopped)
                {
                    t += Time.unscaledDeltaTime * 2.4f;
                    float s = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI);
                    finger.anchoredPosition = at + new Vector2(0f, 18f * s);
                    finger.localScale = Vector3.one * (1f - 0.12f * s);
                    yield return null;
                }
                float wait = 0f;
                while (wait < 0.28f && finger != null && !fingerTapStopped)
                {
                    wait += Time.unscaledDeltaTime;
                    yield return null;
                }
            }
            if (finger != null)
                Destroy(finger.gameObject);
        }

        RectTransform MakeFinger(Transform parent)
        {
            var finger = MakeRect("Finger", parent);
            finger.anchorMin = finger.anchorMax = new Vector2(0.5f, 0.5f);
            finger.pivot = new Vector2(0.3f, 0.9f);
            finger.sizeDelta = new Vector2(84f, 110f);
            var palm = MakeRect("Palm", finger);
            Stretch(palm);
            palm.offsetMin = new Vector2(8f, 0f);
            palm.offsetMax = new Vector2(-8f, -28f);
            var palmImage = palm.gameObject.AddComponent<Image>();
            palmImage.color = Hex("F5D0B0");
            palmImage.raycastTarget = false;
            var tip = MakeRect("Tip", finger);
            tip.anchorMin = new Vector2(0.32f, 0.58f);
            tip.anchorMax = new Vector2(0.68f, 1f);
            tip.offsetMin = tip.offsetMax = Vector2.zero;
            var tipImage = tip.gameObject.AddComponent<Image>();
            tipImage.color = Hex("F5D0B0");
            tipImage.raycastTarget = false;
            return finger;
        }

        Vector2 RootPoint(RectTransform rect)
        {
            Vector3 world = rect.TransformPoint(rect.rect.center);
            var space = aidLayer != null ? aidLayer : root;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(space, RectTransformUtility.WorldToScreenPoint(null, world), null, out var local);
            return local;
        }

        Vector2 GridRootPoint(int col, int row, int w, int h)
        {
            var local = new Vector2((col + w * 0.5f) * pitch, -((row + h * 0.5f) * pitch));
            Vector3 world = grid.TransformPoint(local);
            var space = aidLayer != null ? aidLayer : root;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(space, RectTransformUtility.WorldToScreenPoint(null, world), null, out var point);
            return point;
        }

        Vector2 PointInRoot(RectTransform rect)
        {
            if (rect == null)
                return Vector2.zero;
            Vector3 world = rect.TransformPoint(rect.rect.center);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, RectTransformUtility.WorldToScreenPoint(null, world), null, out var local);
            return local;
        }

        Vector2 GridPointInRoot(int col, int row, int w, int h)
        {
            var local = new Vector2((col + w * 0.5f) * pitch, -((row + h * 0.5f) * pitch));
            Vector3 world = grid.TransformPoint(local);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, RectTransformUtility.WorldToScreenPoint(null, world), null, out var point);
            return point;
        }

        static float EaseOutCubic(float t)
        {
            t = Mathf.Clamp01(t);
            return 1f - Mathf.Pow(1f - t, 3f);
        }

        static float EaseOutBack(float t)
        {
            t = Mathf.Clamp01(t);
            const float s = 1.70158f;
            t -= 1f;
            return t * t * ((s + 1f) * t + s) + 1f;
        }

        IEnumerator Wait(float seconds)
        {
            float t = 0f;
            while (t < seconds)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
        }

        IEnumerator MoveTo(RectTransform rect, Vector2 dest, float duration)
        {
            if (rect == null)
                yield break;
            Vector2 from = rect.anchoredPosition;
            float t = 0f;
            while (t < 1f && rect != null)
            {
                t += Time.unscaledDeltaTime / Mathf.Max(0.01f, duration);
                rect.anchoredPosition = Vector2.LerpUnclamped(from, dest, EaseOutCubic(t));
                yield return null;
            }
            if (rect != null)
                rect.anchoredPosition = dest;
        }

        IEnumerator PunchScale(Transform target, float peak, float duration)
        {
            if (target == null)
                yield break;
            Vector3 baseScale = Vector3.one;
            float up = duration * 0.4f;
            float t = 0f;
            while (t < up && target != null)
            {
                t += Time.unscaledDeltaTime;
                float s = EaseOutCubic(t / up);
                target.localScale = Vector3.LerpUnclamped(baseScale, baseScale * peak, s);
                yield return null;
            }
            t = 0f;
            float down = Mathf.Max(0.01f, duration - up);
            while (t < down && target != null)
            {
                t += Time.unscaledDeltaTime;
                float s = EaseOutCubic(t / down);
                target.localScale = Vector3.LerpUnclamped(baseScale * peak, baseScale, s);
                yield return null;
            }
            if (target != null)
                target.localScale = baseScale;
        }

        IEnumerator PopIn(RectTransform rect)
        {
            if (rect == null)
                yield break;
            rect.localScale = Vector3.one * 0.84f;
            float t = 0f;
            while (t < 1f && rect != null)
            {
                t += Time.unscaledDeltaTime * 7f;
                rect.localScale = Vector3.LerpUnclamped(Vector3.one * 0.84f, Vector3.one, EaseOutBack(Mathf.Clamp01(t)));
                yield return null;
            }
            if (rect != null)
                rect.localScale = Vector3.one;
        }

        IEnumerator PopStars()
        {
            for (int i = 0; i < winStarRoots.Length; i++)
            {
                if (winStarRoots[i] == null)
                    continue;
                winStarRoots[i].localScale = Vector3.zero;
                winStarRoots[i].localRotation = Quaternion.identity;
            }
            yield return Motion.Wait(0.2f);
            int[] order = { 1, 0, 2 };
            for (int n = 0; n < order.Length; n++)
            {
                int i = order[n];
                var star = winStarRoots[i];
                if (star == null)
                    continue;
                bool earned = i < winEarnedStars;
                yield return Motion.FallFromCamera(star, 0.38f, 620f, i == 1 ? 3.6f : 3.1f);
                Haptics.Impact(earned);
                if (winPanel != null)
                    StartCoroutine(Motion.Punch(winPanel, 1.03f, 0.14f));
                yield return Motion.ImpactSquash(star);
                yield return Motion.Wait(0.1f);
            }
        }

        IEnumerator PlayLandImpact(RectTransform landed)
        {
            if (landed == null)
                yield break;
            Haptics.Impact();
            var image = landed.GetComponent<Image>();
            Color flash = image != null ? Color.Lerp(image.color, Color.white, 0.55f) : Color.white;
            float size = Mathf.Max(landed.sizeDelta.x, landed.sizeDelta.y);
            StartCoroutine(BurstImpact(landed, flash, size));
            if (boardFrame != null)
                StartCoroutine(Motion.Punch(boardFrame, 1.03f, 0.14f));
            yield return Motion.ImpactSquash(landed);
        }

        IEnumerator BurstImpact(RectTransform target, Color color, float size)
        {
            if (target == null)
                yield break;
            var space = target.parent as RectTransform ?? target;
            var ring = MakeRect("Impact", space);
            ring.SetAsLastSibling();
            ring.pivot = new Vector2(0.5f, 0.5f);
            ring.anchorMin = ring.anchorMax = new Vector2(0.5f, 0.5f);
            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);
            Vector3 world = (corners[0] + corners[2]) * 0.5f;
            ring.anchoredPosition = space.InverseTransformPoint(world);
            ring.sizeDelta = new Vector2(size * 0.45f, size * 0.45f);
            var image = ring.gameObject.AddComponent<Image>();
            image.sprite = RingSprite();
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = color;
            var group = ring.gameObject.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            group.alpha = 0.9f;
            float t = 0f;
            while (t < 1f && ring != null)
            {
                t += Time.unscaledDeltaTime / 0.28f;
                float u = Mathf.Clamp01(t);
                float span = Mathf.Lerp(size * 0.4f, size * 2.15f, Motion.OutCubic(u));
                ring.sizeDelta = new Vector2(span, span);
                group.alpha = 0.95f * (1f - u);
                yield return null;
            }
            if (ring != null)
                Destroy(ring.gameObject);
        }

        Sprite RingSprite()
        {
            if (impactRingSprite != null)
                return impactRingSprite;
            const int s = 96;
            var tex = new Texture2D(s, s, TextureFormat.RGBA32, false);
            tex.wrapMode = TextureWrapMode.Clamp;
            tex.filterMode = FilterMode.Bilinear;
            float mid = (s - 1) * 0.5f;
            for (int y = 0; y < s; y++)
            {
                for (int x = 0; x < s; x++)
                {
                    float nx = (x - mid) / mid;
                    float ny = (y - mid) / mid;
                    float d = Mathf.Sqrt(nx * nx + ny * ny);
                    float ring = 1f - Mathf.Abs(d - 0.76f) / 0.18f;
                    float core = 1f - Mathf.Clamp01(d / 0.42f);
                    float a = Mathf.Clamp01(Mathf.Max(ring, core * 0.4f));
                    if (d > 1f)
                        a = 0f;
                    tex.SetPixel(x, y, new Color(1f, 1f, 1f, a));
                }
            }
            tex.Apply(false, true);
            impactRingSprite = Sprite.Create(tex, new Rect(0f, 0f, s, s), new Vector2(0.5f, 0.5f), 96f);
            return impactRingSprite;
        }

        IEnumerator DealTraySlots()
        {
            var waiting = new List<RectTransform>();
            foreach (var slot in slots)
            {
                if (slot.root == null || !slot.root.gameObject.activeSelf)
                    continue;
                if (slot.slab == null || !slot.slab.gameObject.activeSelf)
                    continue;
                waiting.Add(slot.root);
            }
            if (waiting.Count == 0)
                yield break;
            for (int i = 0; i < waiting.Count; i++)
            {
                StartCoroutine(Motion.DealIn(waiting[i], true, 0.34f, 120f + i * 12f));
                yield return Motion.Wait(0.1f);
            }
            yield return Motion.Wait(0.28f);
        }

        void HideTrayForIntro()
        {
            foreach (var slot in slots)
            {
                if (slot.root == null || !slot.root.gameObject.activeSelf)
                    continue;
                if (slot.slab == null || !slot.slab.gameObject.activeSelf)
                    continue;
                Motion.HideForDeal(slot.root);
            }
        }

        IEnumerator LevelIntro()
        {
            for (int r = 0; r < GameState.Size; r++)
            {
                for (int c = 0; c < GameState.Size; c++)
                    Motion.HideForSit(grid.Find("Empty_" + c + "_" + r) as RectTransform);
            }
            foreach (Transform child in placedLayer)
                Motion.HideForSit(child as RectTransform);
            HideTrayForIntro();
            boardFrame.localScale = Vector3.one * 0.96f;
            yield return Motion.Scale(boardFrame, Vector3.one, 0.16f);

            for (int diagonal = 0; diagonal <= GameState.Size * 2 - 2; diagonal++)
            {
                for (int r = 0; r < GameState.Size; r++)
                {
                    int c = diagonal - r;
                    if (c < 0 || c >= GameState.Size)
                        continue;
                    var empty = grid.Find("Empty_" + c + "_" + r) as RectTransform;
                    if (empty != null)
                        StartCoroutine(Motion.Sit(empty, 0.32f, 16f, 5f));
                }
                yield return Motion.Wait(0.028f);
            }
            yield return Motion.Wait(0.08f);
            foreach (Transform child in placedLayer)
            {
                if (child is RectTransform piece)
                    StartCoroutine(Motion.Sit(piece, 0.3f, 14f, 4f));
                yield return Motion.Wait(0.03f);
            }
            yield return Motion.Wait(0.12f);
            yield return DealTraySlots();
            var pill = Find("Level");
            if (pill != null)
            {
                pill.localScale = Vector3.one * 0.88f;
                yield return Motion.PopIn(pill, 0.18f);
            }
            if (coach != null && coach.gameObject.activeSelf && coachGroup != null)
                yield return Motion.Alpha(coachGroup, 1f, 0.18f);
            boardIntroActive = false;
            motionBusy = false;
            introRoutine = null;
            DrawTutorialAids();
            PulseGuideButtons();
        }

        void FinishIntroPose()
        {
            if (boardFrame != null)
                boardFrame.localScale = Vector3.one;
            var pill = Find("Level");
            if (pill != null)
                pill.localScale = Vector3.one;
            if (grid != null)
            {
                for (int r = 0; r < GameState.Size; r++)
                {
                    for (int c = 0; c < GameState.Size; c++)
                    {
                        var empty = grid.Find("Empty_" + c + "_" + r) as RectTransform;
                        if (empty == null)
                            continue;
                        PlaceOnGrid(empty, c, r, 1, 1);
                        empty.localScale = Vector3.one;
                        empty.localRotation = Quaternion.identity;
                    }
                }
            }
            if (placedLayer != null)
            {
                foreach (var pair in state.placed)
                {
                    var piece = placedLayer.Find(pair.Key) as RectTransform;
                    if (piece == null)
                        continue;
                    PlaceOnGrid(piece, pair.Value.col, pair.Value.row, pair.Value.w, pair.Value.h);
                    piece.localScale = Vector3.one;
                    piece.localRotation = Quaternion.identity;
                }
                foreach (Transform child in placedLayer)
                {
                    child.localScale = Vector3.one;
                    child.localRotation = Quaternion.identity;
                }
            }
            foreach (var slot in slots)
            {
                if (slot.root == null)
                    continue;
                slot.root.localScale = Vector3.one;
                slot.root.localRotation = Quaternion.identity;
                var group = slot.root.GetComponent<CanvasGroup>();
                if (group == null)
                    continue;
                group.alpha = 1f;
                group.blocksRaycasts = true;
            }
            if (coachGroup != null && coach != null && coach.gameObject.activeSelf)
                coachGroup.alpha = 1f;
        }

        void PulseGuideButtons()
        {
            if (pulseRoutine != null)
                StopCoroutine(pulseRoutine);
            if (!state.IsTutorial)
                return;
            if (state.level.id == 4)
                pulseRoutine = StartCoroutine(PulseOnce(Find("Undo")));
            else if (state.level.id == 5)
                pulseRoutine = StartCoroutine(PulseOnce(Find("Hint")));
        }

        IEnumerator PulseOnce(RectTransform rect)
        {
            if (rect == null)
                yield break;
            Vector3 baseScale = Vector3.one;
            for (int i = 0; i < 3; i++)
            {
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.unscaledDeltaTime * 3.2f;
                    float s = Mathf.Sin(Mathf.Clamp01(t) * Mathf.PI);
                    rect.localScale = Vector3.Lerp(baseScale, baseScale * 1.12f, s);
                    yield return null;
                }
            }
            rect.localScale = baseScale;
            pulseRoutine = null;
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
                case "violet":
                case "white":
                    face = Hex("8B5CFF"); highlight = Hex("C4A8FF"); shade = Hex("5A32C8"); break;
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

        void BuildPopups()
        {
            overlay = MakeRect("Overlay", root);
            Stretch(overlay);
            overlayGroup = overlay.gameObject.AddComponent<CanvasGroup>();
            overlayGroup.alpha = 0f;
            overlayGroup.blocksRaycasts = false;
            var dim = overlay.gameObject.AddComponent<Image>();
            dim.color = new Color(0f, 0f, 0f, 0.62f);
            dim.raycastTarget = true;

            winPanel = MakeCard("Win");
            var title = MakeRect("Title", winPanel);
            title.anchorMin = title.anchorMax = new Vector2(0.5f, 1f);
            title.pivot = new Vector2(0.5f, 1f);
            title.sizeDelta = new Vector2(CompleteTitleWidth, CompleteTitleHeight);
            title.anchoredPosition = new Vector2(0f, -64f);
            var titleImage = title.gameObject.AddComponent<Image>();
            titleImage.sprite = levelCompleteSprite;
            titleImage.preserveAspect = true;
            titleImage.raycastTarget = false;
            AddConfetti(winPanel);
            var stars = MakeRect("Stars", winPanel);
            Pin(stars, 0.08f, 0.42f, 0.92f, 0.68f);
            for (int i = 0; i < 3; i++)
            {
                var star = MakeRect("Star" + i, stars);
                winStarRoots[i] = star;

                var glow = MakeRect("Glow", star);
                var glowImage = glow.gameObject.AddComponent<Image>();
                glowImage.sprite = starGlowSprite;
                glowImage.preserveAspect = true;
                glowImage.raycastTarget = false;
                glow.gameObject.AddComponent<CanvasGroup>();
                winStarGlows[i] = glowImage;

                var face = MakeRect("Face", star);
                Stretch(face);
                var faceImage = face.gameObject.AddComponent<Image>();
                faceImage.sprite = starSprite;
                faceImage.preserveAspect = true;
                faceImage.raycastTarget = false;
                winStars[i] = faceImage;
            }
            hintsUsedText = MakeText(winPanel, "Hints used: 0", 50, new Color(1f, 1f, 1f, 0.85f), fontBold);
            Pin(hintsUsedText.rectTransform, 0.1f, 0.36f, 0.9f, 0.44f);
            var next = MakePill(winPanel, "Next", yellowButtonSprite, "Next Level", Hex("1A1A1A"), out nextLabel);
            nextLabel.font = tmpBlack;
            Pin(next, 0.1f, 0.21f, 0.9f, 0.35f);
            BindButton(next.gameObject, NextLevel);
            var replay = MakePill(winPanel, "Replay", blueButtonSprite, "Replay", Color.white, out var replayLabel);
            replayLabel.font = tmpBlack;
            Pin(replay, 0.1f, 0.06f, 0.9f, 0.20f);
            BindButton(replay.gameObject, ReplayLevel);

            hintPanel = MakeCard("Hint");
            var hintTitle = MakeText(hintPanel, "Need a Hint?", 52, Color.white, fontBlack);
            Pin(hintTitle.rectTransform, 0.08f, 0.80f, 0.92f, 0.94f);
            var close = MakeRect("Close", hintPanel);
            close.anchorMin = close.anchorMax = new Vector2(1f, 1f);
            close.pivot = new Vector2(1f, 1f);
            close.anchoredPosition = new Vector2(-18f, -18f);
            close.sizeDelta = new Vector2(64f, 64f);
            var closeImage = close.gameObject.AddComponent<Image>();
            closeImage.sprite = closeSprite;
            closeImage.preserveAspect = true;
            BindButton(close.gameObject, HideOverlay);
            var bulb = MakeRect("Bulb", hintPanel);
            Pin(bulb, 0.24f, 0.44f, 0.76f, 0.82f);
            hintPopupGlow = AddGlowStack(bulb, hintGlowSprite, hintBulbSprite);
            var hintBody = MakeText(hintPanel, "Place one piece for you", 34, new Color(1f, 1f, 1f, 0.9f), fontBold);
            Pin(hintBody.rectTransform, 0.08f, 0.36f, 0.92f, 0.46f);
            var useHint = MakePill(hintPanel, "UseHint", yellowButtonSprite, "Use Hint (Unlimited)", Hex("1A1A1A"), out useHintLabel);
            Pin(useHint, 0.1f, 0.20f, 0.9f, 0.34f);
            BindButton(useHint.gameObject, UseHint);
            var more = MakePill(hintPanel, "MoreHints", greenButtonSprite, "Get More Hints", Color.white, out _);
            Pin(more, 0.1f, 0.06f, 0.9f, 0.18f);
            BindButton(more.gameObject, () => messageText.text = "Shop coming soon");

            HideOverlayNow();
        }

        RectTransform MakeCard(string name)
        {
            var rect = MakeRect(name, overlay);
            var image = rect.gameObject.AddComponent<Image>();
            image.raycastTarget = true;
            if (popupCardSprite != null)
            {
                image.sprite = popupCardSprite;
                image.type = Image.Type.Sliced;
                image.color = Color.white;
            }
            else
                image.color = Hex("1A1A1A");
            return rect;
        }

        RectTransform MakePill(Transform parent, string name, Sprite background, string caption, Color textColor, out TextMeshProUGUI label)
        {
            var rect = MakeRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.raycastTarget = true;
            if (background != null)
            {
                image.sprite = background;
                image.type = Image.Type.Sliced;
                image.color = Color.white;
            }
            label = MakeText(rect, caption, 42, textColor, fontExtraBold);
            label.gameObject.name = "Caption";
            return rect;
        }

        void LayoutPill(RectTransform button, float height)
        {
            if (button == null)
                return;
            FitSlice(button.GetComponent<Image>(), 42f, height * 0.48f);
            var caption = button.Find("Caption") as RectTransform;
            if (caption == null)
                return;
            var label = caption.GetComponent<TextMeshProUGUI>();
            if (label != null)
            {
                bool winCaption = button.name == "Next" || button.name == "Replay";
                label.font = winCaption ? tmpBlack : label.font;
                label.fontSize = winCaption ? 80f : height * 0.36f;
                label.characterSpacing = winCaption ? -6f : 0f;
                label.wordSpacing = winCaption ? -10f : 0f;
                label.enableWordWrapping = false;
                label.overflowMode = TextOverflowModes.Overflow;
            }
            Stretch(caption);
        }

        void LayoutCompleteTitle()
        {
            if (winPanel == null)
                return;
            var title = winPanel.Find("Title") as RectTransform;
            if (title == null)
                return;
            title.anchorMin = title.anchorMax = new Vector2(0.5f, 1f);
            title.pivot = new Vector2(0.5f, 1f);
            title.sizeDelta = new Vector2(CompleteTitleWidth, CompleteTitleHeight);
            float topInset = Mathf.Clamp(winPanel.rect.height * 0.07f, 56f, 88f);
            title.anchoredPosition = new Vector2(0f, -topInset);
        }

        void ShowWin()
        {
            bool last = levelIndex >= allLevels.Count - 1;
            nextLabel.text = last ? "Home" : "Next Level";
            hintsUsedText.text = "Hints used: " + state.hintsUsed;
            winEarnedStars = state.hintsUsed <= 0 ? 3 : (state.hintsUsed == 1 ? 2 : 1);
            for (int i = 0; i < winStars.Length; i++)
            {
                bool earned = i < winEarnedStars;
                if (winStars[i] != null)
                    winStars[i].color = earned ? Color.white : new Color(1f, 1f, 1f, 0.28f);
                if (winStarGlows[i] != null)
                    winStarGlows[i].color = earned ? Color.white : new Color(1f, 1f, 1f, 0.12f);
            }
            LayoutWinStars();
            bool first = !celebrating;
            celebrating = true;
            if (first)
            {
                foreach (Transform child in placedLayer)
                    StartCoroutine(Motion.Punch(child, 1.04f, 0.16f));
                ShowOverlay(winPanel);
                StartCoroutine(PopStars());
            }
            else if (overlay != null && !overlay.gameObject.activeSelf)
                ShowOverlay(winPanel);
        }

        void LayoutWinStars()
        {
            if (winStarRoots[0] == null || winPanel == null)
                return;
            var row = winPanel.Find("Stars") as RectTransform;
            if (row == null)
                return;
            float w = row.rect.width;
            float h = row.rect.height;
            if (w < 8f || h < 8f)
            {
                w = Mathf.Max(w, 520f);
                h = Mathf.Max(h, 200f);
            }
            float mid = Mathf.Min(w * 0.36f, h * 0.92f);
            float side = mid * 0.72f;
            PlaceWinStar(0, new Vector2(-w * 0.28f, -h * 0.14f), side);
            PlaceWinStar(1, new Vector2(0f, h * 0.12f), mid);
            PlaceWinStar(2, new Vector2(w * 0.28f, -h * 0.14f), side);
        }

        void PlaceWinStar(int i, Vector2 pos, float size)
        {
            var root = winStarRoots[i];
            if (root == null)
                return;
            root.anchorMin = root.anchorMax = new Vector2(0.5f, 0.5f);
            root.pivot = new Vector2(0.5f, 0.5f);
            root.anchoredPosition = pos;
            root.sizeDelta = new Vector2(size, size);

            var face = root.Find("Face") as RectTransform;
            if (face != null)
                Stretch(face);

            var glow = root.Find("Glow") as RectTransform;
            if (glow == null)
                return;
            glow.SetAsFirstSibling();
            glow.anchorMin = glow.anchorMax = new Vector2(0.5f, 0.5f);
            glow.pivot = new Vector2(0.5f, 0.5f);
            glow.anchoredPosition = Vector2.zero;
            float glowSize = size * 1.05f;
            glow.sizeDelta = new Vector2(glowSize, glowSize);
        }

        void AnimateGlows()
        {
            float t = Time.unscaledTime;
            if (winPanel != null && winPanel.gameObject.activeInHierarchy)
            {
                for (int i = 0; i < winStarGlows.Length; i++)
                    PulseGlow(winStarGlows[i], t + i * 0.55f, i < winEarnedStars);
            }
            bool hintOpen = hintPanel != null && hintPanel.gameObject.activeInHierarchy;
            PulseGlow(hintPopupGlow, t, hintOpen);
            PulseGlow(headerBulbGlow, t, true);
        }

        static void PulseGlow(Image glow, float time, bool strong)
        {
            if (glow == null)
                return;
            float wave = Mathf.Sin(time * 2.4f) * 0.5f + 0.5f;
            glow.rectTransform.localScale = Vector3.one * (1f + 0.08f * wave);
            var group = glow.GetComponent<CanvasGroup>();
            if (group != null)
                group.alpha = strong ? 0.45f + 0.55f * wave : 0.12f;
        }

        Image AddGlowStack(RectTransform root, Sprite glowSprite, Sprite faceSprite)
        {
            var glow = MakeRect("Glow", root);
            var glowImage = glow.gameObject.AddComponent<Image>();
            glowImage.sprite = glowSprite;
            glowImage.preserveAspect = true;
            glowImage.raycastTarget = false;
            glow.gameObject.AddComponent<CanvasGroup>();

            var face = MakeRect("Face", root);
            Stretch(face);
            var faceImage = face.gameObject.AddComponent<Image>();
            faceImage.sprite = faceSprite;
            faceImage.preserveAspect = true;
            faceImage.raycastTarget = false;
            return glowImage;
        }

        static void LayoutStackedGlow(RectTransform root, float extra = 0.05f)
        {
            if (root == null)
                return;
            var face = root.Find("Face") as RectTransform;
            if (face != null)
                Stretch(face);
            var glow = root.Find("Glow") as RectTransform;
            if (glow == null)
                return;
            glow.SetAsFirstSibling();
            glow.anchorMin = glow.anchorMax = new Vector2(0.5f, 0.5f);
            glow.pivot = new Vector2(0.5f, 0.5f);
            glow.anchoredPosition = Vector2.zero;
            glow.sizeDelta = new Vector2(root.rect.width * (1f + extra), root.rect.height * (1f + extra));
        }

        void AddConfetti(RectTransform parent)
        {
            Color[] colors = { Hex("F03030"), Hex("1E7BFF"), Hex("FFD21F"), Hex("2EC84F") };
            float[] xs = { 0.10f, 0.22f, 0.78f, 0.90f, 0.14f, 0.86f, 0.30f, 0.70f };
            float[] ys = { 0.88f, 0.94f, 0.93f, 0.86f, 0.74f, 0.76f, 0.97f, 0.72f };
            for (int i = 0; i < xs.Length; i++)
            {
                var bit = MakeRect("Confetti", parent);
                bit.anchorMin = bit.anchorMax = new Vector2(xs[i], ys[i]);
                bit.pivot = new Vector2(0.5f, 0.5f);
                bit.sizeDelta = new Vector2(i % 2 == 0 ? 16f : 12f, i % 3 == 0 ? 22f : 12f);
                bit.localRotation = Quaternion.Euler(0f, 0f, i * 28f);
                var image = bit.gameObject.AddComponent<Image>();
                image.color = colors[i % colors.Length];
                image.raycastTarget = false;
            }
        }

        static void Pin(RectTransform rect, float xMin, float yMin, float xMax, float yMax)
        {
            rect.anchorMin = new Vector2(xMin, yMin);
            rect.anchorMax = new Vector2(xMax, yMax);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        static void SizeCentered(RectTransform rect, float w, float h)
        {
            if (rect == null)
                return;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
            rect.sizeDelta = new Vector2(w, h);
        }

        void PlaceCentered(RectTransform rect, float x, float y, float w, float h)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0f, 1f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = new Vector2(x - root.rect.xMin + w * 0.5f, y - root.rect.yMax + h * 0.5f);
            rect.sizeDelta = new Vector2(w, h);
        }

        RectTransform MakeCandyButton(Transform parent, string name, Sprite background, Sprite icon, string caption)
        {
            var rect = MakeRect(name, parent);
            var image = rect.gameObject.AddComponent<Image>();
            image.raycastTarget = true;
            if (background != null)
            {
                image.sprite = background;
                image.type = Image.Type.Sliced;
                image.color = Color.white;
            }
            else
                image.color = name == "Reset" ? Hex("F03030") : Hex("1E7BFF");
            var iconRect = MakeRect("Icon", rect);
            var iconImage = iconRect.gameObject.AddComponent<Image>();
            iconImage.sprite = icon;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            var label = MakeText(rect, caption, 48, Color.white, fontExtraBold);
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.gameObject.name = "Caption";
            return rect;
        }

        void LayoutCandyButton(RectTransform button, float height)
        {
            if (button == null)
                return;
            FitSlice(button.GetComponent<Image>(), 78f, height * 0.5f);
            var icon = button.Find("Icon") as RectTransform;
            var caption = button.Find("Caption") as RectTransform;
            if (icon == null || caption == null)
                return;
            var label = caption.GetComponent<TextMeshProUGUI>();
            float iconSize = height * 0.42f;
            float fontSize = height * 0.36f;
            if (label != null)
            {
                label.fontSize = fontSize;
                label.alignment = TextAlignmentOptions.MidlineLeft;
                label.enableWordWrapping = false;
                label.ForceMeshUpdate();
            }
            float textW = label != null ? Mathf.Max(40f, label.preferredWidth) : 80f;
            float gap = height * 0.1f;
            float total = iconSize + gap + textW;
            float start = (button.rect.width - total) * 0.5f;
            icon.anchorMin = icon.anchorMax = new Vector2(0f, 0.5f);
            icon.pivot = new Vector2(0f, 0.5f);
            icon.sizeDelta = new Vector2(iconSize, iconSize);
            icon.anchoredPosition = new Vector2(start, 0f);
            caption.anchorMin = caption.anchorMax = new Vector2(0f, 0.5f);
            caption.pivot = new Vector2(0f, 0.5f);
            caption.sizeDelta = new Vector2(textW, height);
            caption.anchoredPosition = new Vector2(start + iconSize + gap, 0f);
        }

        RectTransform MakeButton(Transform parent, string name, string color, out TextMeshProUGUI label)
        {
            var rect = MakeRect(name, parent);
            var slab = rect.gameObject.AddComponent<SlabGraphic>();
            Colors(color, out var face, out _, out _);
            slab.flat = true;
            slab.bevel = 0f;
            slab.rim = false;
            slab.corner = 40f;
            slab.SetColors(face, face, face, false);
            label = MakeText(rect, "", 48, Color.white, fontExtraBold);
            return rect;
        }

        RectTransform MakePopup(Transform parent, string title, out TextMeshProUGUI body)
        {
            var rect = MakeRect(title, parent);
            rect.gameObject.AddComponent<Image>().color = Hex("1A1A1A");
            var heading = MakeText(rect, title, 52, Color.white, fontExtraBold);
            heading.rectTransform.anchorMin = new Vector2(0f, 0.55f);
            heading.rectTransform.anchorMax = new Vector2(1f, 1f);
            body = MakeText(rect, "", 36, Color.white, fontBold);
            body.rectTransform.anchorMin = new Vector2(0f, 0.35f);
            body.rectTransform.anchorMax = new Vector2(1f, 0.6f);
            return rect;
        }

        void LayoutLevelTitle(float pillW, float pillH, float scale)
        {
            float wordSize = 60f * scale;
            float numberSize = 75f * scale;
            levelWord.fontSize = wordSize;
            levelNumber.fontSize = numberSize;
            levelWord.alignment = TextAlignmentOptions.MidlineRight;
            levelNumber.alignment = TextAlignmentOptions.MidlineLeft;
            levelWord.ForceMeshUpdate();
            levelNumber.ForceMeshUpdate();
            float space = 14f * scale;
            float wordW = Mathf.Max(40f, levelWord.preferredWidth);
            float numW = Mathf.Max(30f, levelNumber.preferredWidth);
            float total = wordW + space + numW;
            float start = (pillW - total) * 0.5f;
            var wordRect = levelWord.rectTransform;
            wordRect.sizeDelta = new Vector2(wordW, pillH);
            wordRect.anchoredPosition = new Vector2(start - pillW * 0.5f, 0f);
            var numRect = levelNumber.rectTransform;
            numRect.sizeDelta = new Vector2(numW, pillH);
            numRect.anchoredPosition = new Vector2(start - pillW * 0.5f + wordW + space, 0f);
        }

        static void PinLabel(RectTransform rect)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        void BindButton(GameObject go, UnityEngine.Events.UnityAction action)
        {
            var button = go.GetComponent<Button>();
            if (button != null)
                button.enabled = false;
            var press = go.GetComponent<TapPress>();
            if (press == null)
                press = go.AddComponent<TapPress>();
            press.click = action;
        }

        TextMeshProUGUI MakeText(Transform parent, string value, int size, Color color, Font useFont = null)
        {
            var rect = MakeRect("Label", parent);
            Stretch(rect);
            var text = rect.gameObject.AddComponent<TextMeshProUGUI>();
            text.font = TmpFont(useFont);
            text.text = value;
            text.fontSize = size;
            text.fontStyle = FontStyles.Normal;
            text.alignment = TextAlignmentOptions.Center;
            text.color = color;
            text.raycastTarget = false;
            text.enableWordWrapping = false;
            text.overflowMode = TextOverflowModes.Overflow;
            text.extraPadding = true;
            return text;
        }

        TMP_FontAsset TmpFont(Font source)
        {
            if (source == fontBlack)
                return tmpBlack;
            if (source == fontBold)
                return tmpBold;
            if (source == fontSemiBold)
                return tmpSemiBold;
            if (source == fontExtraBold)
                return tmpExtraBold;
            return tmpFont ?? tmpExtraBold ?? tmpBlack;
        }

        static TMP_FontAsset CreateTmpFont(Font source)
        {
            if (source == null)
                return null;
            var asset = TMP_FontAsset.CreateFontAsset(source);
            if (asset != null)
                asset.name = source.name;
            return asset;
        }

        static RectTransform MakeRect(string name, Transform parent)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.transform.SetParent(parent, false);
            return go.GetComponent<RectTransform>();
        }

        static void Stretch(RectTransform rect, float inset = 0f)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = new Vector2(inset, inset);
            rect.offsetMax = new Vector2(-inset, -inset);
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

    public class TapPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public UnityEngine.Events.UnityAction click;
        Coroutine anim;
        bool pressed;
        Rect pressRect;

        public void OnPointerDown(PointerEventData eventData)
        {
            pressed = true;
            pressRect = ScreenRect();
            Animate(0.96f);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            Animate(1f);
            if (!pressed)
                return;
            pressed = false;
            if (pressRect.Contains(eventData.position))
                click?.Invoke();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!pressed)
                Animate(1f);
        }

        Rect ScreenRect()
        {
            var rect = transform as RectTransform;
            Vector3[] corners = new Vector3[4];
            rect.GetWorldCorners(corners);
            Vector2 min = RectTransformUtility.WorldToScreenPoint(null, corners[0]);
            Vector2 max = RectTransformUtility.WorldToScreenPoint(null, corners[2]);
            return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
        }

        void Animate(float scale)
        {
            if (anim != null)
                StopCoroutine(anim);
            anim = StartCoroutine(ScaleTo(scale));
        }

        IEnumerator ScaleTo(float scale)
        {
            Vector3 from = transform.localScale;
            Vector3 to = Vector3.one * scale;
            float t = 0f;
            while (t < 1f)
            {
                t += Time.unscaledDeltaTime * 16f;
                float u = 1f - Mathf.Pow(1f - Mathf.Clamp01(t), 3f);
                transform.localScale = Vector3.LerpUnclamped(from, to, u);
                yield return null;
            }
            transform.localScale = to;
            anim = null;
        }
    }
}
