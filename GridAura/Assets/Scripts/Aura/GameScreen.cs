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
        int tutorialCount;
        int firstRotateId = 7;
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
        CanvasScaler cheerScaler;
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
        Sprite iconSoundSprite;
        Sprite iconMusicSprite;
        Sprite iconHapticsSprite;
        Sprite iconThemeSprite;
        Sprite iconCartSprite;
        Sprite iconShieldSprite;
        Sprite iconStarSprite;
        Sprite iconBackSprite;
        Sprite iconChevronSprite;
        Sprite toggleTrackSprite;
        Sprite toggleKnobSprite;
        Sprite settingsBgSprite;
        Sprite settingsBackSprite;
        Sprite settingsRowSprite;
        Sprite settingsTitleSprite;
        Sprite settingsToggleOnSprite;
        Sprite settingsToggleKnobSprite;
        GameObject starBurstFx;
        GameObject starPoofFx;
        GameObject sparkImpactFx;
        GameObject confettiBlastFx;
        GameObject confettiShowerFx;
        TextMeshProUGUI levelWord;
        TextMeshProUGUI levelNumber;
        TextMeshProUGUI hintText;
        TextMeshProUGUI messageText;
        TextMeshProUGUI nextLabel;
        TextMeshProUGUI hintsUsedText;
        TextMeshProUGUI starsTotalText;
        TextMeshProUGUI useHintLabel;
        TextMeshProUGUI movesLabel;
        TextMeshProUGUI headerStars;
        Image headerStarIcon;
        TextMeshProUGUI bannerLabel;
        RectTransform briefing;
        CanvasGroup briefingGroup;
        TextMeshProUGUI briefingType;
        TextMeshProUGUI briefingBody;
        TextMeshProUGUI briefingMoves;
        TextMeshProUGUI briefingStars;
        bool briefingSkip;
        bool briefingDocked;
        bool hudLanded;
        const float DragSlop = 56f;
        const float BoardTapSlop = 22f;
        const string PrefLastLevel = "GridAura.LastLevel";
        const string PrefHaptics = "GridAura.Haptics";
        const string PrefSound = "GridAura.Sound";
        const string PrefMusic = "GridAura.Music";
        const string PrefHints = "GridAura.Hints";
        const string PrefUnlimited = "GridAura.UnlimitedHints";
        const string PrefNeon = "GridAura.Neon";
        const string PrefTheme = "GridAura.Theme";
        const string PrefHowTo = "GridAura.HowToSeen";
        const string PrefRelaxed = "GridAura.RelaxedTimers";
        static readonly string[] LevelTabs = { "Easy", "Medium", "Hard", "Expert" };
        static readonly Color[] LevelCandy =
        {
            Hex("2F6BFF"), Hex("E23B3B"), Hex("F5C400"), Hex("2F6BFF"), Hex("F5C400")
        };
        RectTransform menuRoot;
        RectTransform splashPanel;
        RectTransform homePanel;
        RectTransform levelsPanel;
        RectTransform settingsPanel;
        RectTransform shopPanel;
        RectTransform howToPanel;
        RectTransform themePanel;
        RectTransform privacyPanel;
        RectTransform levelsContent;
        RectTransform menuBackTo;
        TextMeshProUGUI homePlayLabel;
        TextMeshProUGUI homeStatus;
        TextMeshProUGUI homeStarLabel;
        TextMeshProUGUI shopStatus;
        TextMeshProUGUI themeStatus;
        Image soundIcon;
        RectTransform soundToggle;
        RectTransform musicToggle;
        RectTransform hapticsToggle;
        RectTransform relaxedToggle;
        RectTransform keepGoingButton;
        TextMeshProUGUI movesTitle;
        TextMeshProUGUI movesBodyLabel;
        float timeLeft;
        bool timeFailed;
        readonly List<RectTransform> levelTiles = new List<RectTransform>();
        readonly List<RectTransform> levelTabButtons = new List<RectTransform>();
        int levelTab;
        int versionTaps;
        bool soundOn = true;
        bool musicOn = true;
        bool menuOpen;
        bool settingsFromPlay;
        RectTransform movesPill;
        RectTransform banner;
        RectTransform movesPanel;
        RectTransform debugPanel;
        TMP_InputField debugLevelInput;
        TextMeshProUGUI debugStatus;
        CanvasGroup bannerGroup;
        Image movesPillImage;
        Image bannerImage;
        Coroutine bannerRoutine;
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
        RectTransform cheerRoot;
        RectTransform transitionCover;
        RectTransform transitionBar;
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
        Coroutine confettiRoutine;
        Coroutine starRoutine;
        GameObject confettiLive;
        Transform fxRoot;
        static readonly string[] PlaceCheers = { "Nice!", "Great!", "Super!", "Awesome!", "Yes!" };
        int lastTransition = -1;
        bool motionBusy;
        bool celebrating;
        bool rotating;
        bool boardIntroActive;
        CanvasGroup rootGroup;
        CanvasGroup coachGroup;
        CanvasGroup undoGroup;
        CanvasGroup hintGroup;
        RectTransform dragShadow;
        int ghostCol = -99;
        int ghostRow = -99;
        Vector2 ghostFrom;
        Vector2 ghostTo;
        float ghostBlend = 1f;
        readonly List<CanvasGroup> aidPulses = new List<CanvasGroup>();

        float pitch;
        float gap;
        Vector2 lastScreen;
        bool dragging;
        bool moved;
        bool pendingEndDrag;
        PieceControl active;
        CanvasGroup hiddenGroup;
        PlacedPiece lifted;
        int dragH;
        int dragW;
        Vector2 lastPointer;
        Vector2 pointerStart;
        int grabCol;
        int grabRow;
        bool colorClashHover;

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
            iconSoundSprite = Resources.Load<Sprite>("Art/IconSound");
            iconMusicSprite = Resources.Load<Sprite>("Art/IconMusic");
            iconHapticsSprite = Resources.Load<Sprite>("Art/IconHaptics");
            iconThemeSprite = Resources.Load<Sprite>("Art/IconTheme");
            iconCartSprite = Resources.Load<Sprite>("Art/IconCart");
            iconShieldSprite = Resources.Load<Sprite>("Art/IconShield");
            iconStarSprite = Resources.Load<Sprite>("Art/IconStar");
            iconBackSprite = Resources.Load<Sprite>("Art/IconBack");
            iconChevronSprite = Resources.Load<Sprite>("Art/IconChevron");
            settingsBgSprite = Resources.Load<Sprite>("Art/SettingsBg");
            settingsBackSprite = Resources.Load<Sprite>("Art/SettingsBack");
            settingsRowSprite = Resources.Load<Sprite>("Art/SettingsRow");
            settingsTitleSprite = Resources.Load<Sprite>("Art/SettingsTitle");
            settingsToggleOnSprite = Resources.Load<Sprite>("Art/SettingsToggleOn");
            settingsToggleKnobSprite = Resources.Load<Sprite>("Art/SettingsToggleKnob");
            toggleTrackSprite = Resources.Load<Sprite>("Art/IconToggleTrack");
            toggleKnobSprite = Resources.Load<Sprite>("Art/IconToggleKnob");
            if (toggleTrackSprite == null)
                toggleTrackSprite = settingsToggleOnSprite;
            if (toggleKnobSprite == null)
                toggleKnobSprite = settingsToggleKnobSprite;
            hintGlowSprite = Resources.Load<Sprite>("Art/HintGlow");
            starBurstFx = Resources.Load<GameObject>("Vfx/StarBurst2D");
            starPoofFx = Resources.Load<GameObject>("Vfx/StarPoof");
            sparkImpactFx = Resources.Load<GameObject>("Vfx/SparkExplosion2D");
            confettiBlastFx = Resources.Load<GameObject>("Vfx/ConfettiBlastRainbow");
            confettiShowerFx = Resources.Load<GameObject>("Vfx/ConfettiShowerRainbow");
            emptySprite = Resources.Load<Sprite>("Art/TileEmpty");
            redSprite = Resources.Load<Sprite>("Art/TileRed");
            blueSprite = Resources.Load<Sprite>("Art/TileBlue");
            yellowSprite = Resources.Load<Sprite>("Art/TileYellow");
            blackSprite = Resources.Load<Sprite>("Art/TileBlack");
            LoadCatalogs();
            Haptics.Enabled = PlayerPrefs.GetInt(PrefHaptics, 1) == 1;
            soundOn = PlayerPrefs.GetInt(PrefSound, 1) == 1;
            musicOn = PlayerPrefs.GetInt(PrefMusic, 1) == 1;
            AudioListener.volume = soundOn ? 1f : 0f;
            var camera = Camera.main;
            if (camera != null)
            {
                camera.clearFlags = CameraClearFlags.SolidColor;
                camera.backgroundColor = Color.black;
                camera.orthographic = true;
            }
            Build();
        }

        IEnumerator Start()
        {
            yield return null;
            Canvas.ForceUpdateCanvases();
            if (allLevels.Count == 0)
            {
                if (messageText != null)
                    messageText.text = "Could not load levels.";
                yield break;
            }
            yield return ShowSplashThenHome();
        }

        Camera UiCamera()
        {
            if (canvas == null || canvas.renderMode == RenderMode.ScreenSpaceOverlay)
                return null;
            return canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        }

        void Update()
        {
            float aspect = (float)Screen.width / Mathf.Max(1, Screen.height);
            scaler.matchWidthOrHeight = aspect > ReferenceWidth / ReferenceHeight ? 1f : 0f;
            if (cheerScaler != null)
                cheerScaler.matchWidthOrHeight = scaler.matchWidthOrHeight;
            var size = new Vector2(Screen.width, Screen.height);
            if (size != lastScreen)
            {
                lastScreen = size;
                if (menuOpen)
                    LayoutMenus();
                else if (boardIntroActive)
                    Layout();
                else if (state.level != null)
                    Refresh();
                else
                    Layout();
            }
            if (!motionBusy && dragging && !Input.GetMouseButton(0) && Input.touchCount == 0)
                EndDrag();
            if (dragging)
            {
                StepGhost();
                if (active != null)
                    PulseColorPartner(active.pieceId, state.IsColor);
            }
            float pulse = 0.35f + 0.4f * (Mathf.Sin(Time.unscaledTime * 3.2f) * 0.5f + 0.5f);
            for (int i = 0; i < aidPulses.Count; i++)
            {
                if (aidPulses[i] != null)
                    aidPulses[i].alpha = pulse;
            }
            AnimateGlows();
            BreatheHint();
            UpdateHintButton();
            TickSpeedClock();
        }

        void Build()
        {
            if (FindAnyObjectByType<EventSystem>() == null)
            {
                var events = new GameObject("EventSystem");
                events.AddComponent<EventSystem>();
                events.AddComponent<StandaloneInputModule>();
            }
            if (EventSystem.current != null)
                EventSystem.current.pixelDragThreshold = 1;

            var canvasGo = new GameObject("GameCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGo.GetComponent<Canvas>();
            if (Camera.main != null)
            {
                canvas.renderMode = RenderMode.ScreenSpaceCamera;
                canvas.worldCamera = Camera.main;
                canvas.planeDistance = 10f;
            }
            else
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 0;
            scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            root = MakeRect("Root", canvasGo.transform);
            Stretch(root);
            rootGroup = root.gameObject.AddComponent<CanvasGroup>();

            transitionCover = MakeRect("TransitionCover", canvasGo.transform);
            transitionCover.anchorMin = transitionCover.anchorMax = new Vector2(0.5f, 0.5f);
            transitionCover.pivot = new Vector2(0.5f, 0.5f);
            var coverImage = transitionCover.gameObject.AddComponent<Image>();
            coverImage.color = Hex("12151C");
            coverImage.raycastTarget = true;
            var coverCanvas = transitionCover.gameObject.AddComponent<Canvas>();
            coverCanvas.overrideSorting = true;
            coverCanvas.sortingOrder = 220;
            transitionBar = MakeRect("Edge", transitionCover);
            var barImage = transitionBar.gameObject.AddComponent<Image>();
            barImage.color = Hex("FFD21F");
            barImage.raycastTarget = false;
            transitionCover.gameObject.SetActive(false);

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
            hintGroup = hint.gameObject.AddComponent<CanvasGroup>();

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

            var cheerGo = new GameObject("CheerCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            var cheerCanvas = cheerGo.GetComponent<Canvas>();
            cheerCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            cheerCanvas.sortingOrder = 80;
            cheerCanvas.pixelPerfect = false;
            cheerScaler = cheerGo.GetComponent<CanvasScaler>();
            cheerScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            cheerScaler.referenceResolution = new Vector2(ReferenceWidth, ReferenceHeight);
            cheerScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            cheerRoot = MakeRect("Root", cheerGo.transform);
            Stretch(cheerRoot);

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
            var rotatePress = rotateButton.GetComponent<TapPress>();
            if (rotatePress != null)
                rotatePress.fireOnDown = true;
            rotateButton.gameObject.SetActive(false);

            messageText = MakeText(root, "", 36, Color.white, fontBold);
            BuildPopups();
            confirmPanel = MakePopup(root, "Reset this level?", out _);
            var yes = MakePill(confirmPanel, "Yes", resetButtonSprite != null ? resetButtonSprite : yellowButtonSprite, "Reset", Color.white, out _);
            BindButton(yes.gameObject, ConfirmReset);
            var no = MakePill(confirmPanel, "No", blueButtonSprite, "Cancel", Color.white, out _);
            BindButton(no.gameObject, CancelReset);
            confirmPanel.gameObject.SetActive(false);
            movesPill = MakeRect("Moves", root);
            movesPillImage = movesPill.gameObject.AddComponent<Image>();
            if (levelSprite != null)
            {
                movesPillImage.sprite = levelSprite;
                movesPillImage.type = Image.Type.Sliced;
                movesPillImage.color = Color.white;
            }
            else
                movesPillImage.color = Hex("1E222A");
            movesPillImage.raycastTarget = false;
            movesLabel = MakeText(movesPill, "Moves 0", 32, Hex("2B2B2B"), fontBlack);
            PinLabel(movesLabel.rectTransform);
            var starHold = MakeRect("StarIcon", movesPill);
            headerStarIcon = starHold.gameObject.AddComponent<Image>();
            headerStarIcon.sprite = starSprite;
            headerStarIcon.preserveAspect = true;
            headerStarIcon.raycastTarget = false;
            headerStars = MakeText(movesPill, "0", 32, Hex("1A1A1A"), fontBlack);
            PinLabel(headerStars.rectTransform);
            movesPill.gameObject.SetActive(false);

            briefing = MakeRect("Briefing", root);
            var briefingImage = briefing.gameObject.AddComponent<Image>();
            if (popupCardSprite != null)
            {
                briefingImage.sprite = popupCardSprite;
                briefingImage.type = Image.Type.Sliced;
                briefingImage.color = Color.white;
            }
            else
                briefingImage.color = Hex("1A1A1A");
            briefingImage.raycastTarget = true;
            briefingGroup = briefing.gameObject.AddComponent<CanvasGroup>();
            briefingType = MakeText(briefing, "Classic", 56, Hex("1A1A1A"), fontBlack);
            briefingType.alignment = TextAlignmentOptions.Center;
            briefingBody = MakeText(briefing, "Fill every empty cell", 32, Hex("3A3A3A"), fontBold);
            briefingBody.alignment = TextAlignmentOptions.Center;
            briefingBody.enableWordWrapping = true;
            var movesChip = MakeRect("MovesChip", briefing);
            var chipImage = movesChip.gameObject.AddComponent<Image>();
            if (yellowButtonSprite != null)
            {
                chipImage.sprite = yellowButtonSprite;
                chipImage.type = Image.Type.Sliced;
                chipImage.color = Color.white;
            }
            else
                chipImage.color = Hex("FFD21F");
            briefingMoves = MakeText(movesChip, "Moves 14", 36, Hex("1A1A1A"), fontBlack);
            briefingMoves.alignment = TextAlignmentOptions.Center;
            Stretch(briefingMoves.rectTransform, 8f);
            var briefStar = MakeRect("Star", briefing);
            var briefStarImage = briefStar.gameObject.AddComponent<Image>();
            briefStarImage.sprite = starSprite;
            briefStarImage.preserveAspect = true;
            briefStarImage.raycastTarget = false;
            briefingStars = MakeText(briefing, "0", 34, Hex("1A1A1A"), fontBlack);
            briefingStars.alignment = TextAlignmentOptions.MidlineLeft;
            BindButton(briefing.gameObject, () => briefingSkip = true);
            briefing.gameObject.SetActive(false);

            banner = MakeRect("Banner", root);
            bannerImage = banner.gameObject.AddComponent<Image>();
            bannerImage.raycastTarget = false;
            bannerLabel = MakeText(banner, "", 40, Color.white, fontExtraBold);
            bannerLabel.alignment = TextAlignmentOptions.Center;
            Stretch(bannerLabel.rectTransform, 12f);
            bannerGroup = banner.gameObject.AddComponent<CanvasGroup>();
            banner.gameObject.SetActive(false);

            movesPanel = MakePopup(overlay, "Out of moves!", out var movesBody);
            movesBody.text = "Retry this level, or go back to the start.";
            movesBodyLabel = movesBody;
            var headings = movesPanel.GetComponentsInChildren<TextMeshProUGUI>(true);
            if (headings != null && headings.Length > 0)
                movesTitle = headings[0];
            keepGoingButton = MakeButton(movesPanel, "KeepGoing", "green", out var keepText);
            keepText.text = "Keep going";
            keepText.color = Color.white;
            BindButton(keepGoingButton.gameObject, OnKeepGoing);
            keepGoingButton.anchorMin = new Vector2(0.08f, 0.34f);
            keepGoingButton.anchorMax = new Vector2(0.92f, 0.50f);
            keepGoingButton.offsetMin = keepGoingButton.offsetMax = Vector2.zero;
            keepGoingButton.gameObject.SetActive(false);
            var retry = MakeButton(movesPanel, "Retry", "yellow", out var retryText);
            retryText.text = "Retry";
            retryText.color = Hex("1A1A1A");
            BindButton(retry.gameObject, ReplayLevel);
            var home = MakeButton(movesPanel, "Home", "blue", out var homeText);
            homeText.text = "Home";
            homeText.color = Color.white;
            BindButton(home.gameObject, GoHome);
            retry.anchorMin = new Vector2(0.08f, 0.08f);
            retry.anchorMax = new Vector2(0.48f, 0.32f);
            retry.offsetMin = retry.offsetMax = Vector2.zero;
            home.anchorMin = new Vector2(0.52f, 0.08f);
            home.anchorMax = new Vector2(0.92f, 0.32f);
            home.offsetMin = home.offsetMax = Vector2.zero;
            movesPanel.gameObject.SetActive(false);

            BindButton(Find("Hint").gameObject, OnHint);
            BindButton(Find("Gear").gameObject, OnGear);
            BuildMenus();
            yes.anchorMin = new Vector2(0.08f, 0.08f);
            yes.anchorMax = new Vector2(0.48f, 0.32f);
            yes.offsetMin = yes.offsetMax = Vector2.zero;
            no.anchorMin = new Vector2(0.52f, 0.08f);
            no.anchorMax = new Vector2(0.92f, 0.32f);
            no.offsetMin = no.offsetMax = Vector2.zero;
        }

        bool HintUnlocked
        {
            get { return levelIndex >= 3; }
        }

        void OnHint()
        {
            if (boardIntroActive || !HintUnlocked || motionBusy || rotating || dragging || state.level == null || state.IsComplete())
                return;
            if (state.FreeHints || state.hintsLeft > 0)
                UseHint();
            else
                ShowHintPopup();
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
            if (motionBusy || (!state.FreeHints && state.hintsLeft <= 0))
                return;
            if (hintRoutine != null)
                StopCoroutine(hintRoutine);
            hintRoutine = StartCoroutine(PlayHintGuide());
        }

        void ReplayLevel()
        {
            if (motionBusy && (movesPanel == null || !movesPanel.gameObject.activeSelf))
                return;
            StartCoroutine(ChangeLevelRoutine(levelIndex));
        }

        void GoHome()
        {
            StopTransientFx();
            HideOverlayNow();
            celebrating = false;
            dragging = false;
            rotating = false;
            motionBusy = false;
            ShowMenu(homePanel);
        }

        IEnumerator ShowSplashThenHome()
        {
            ShowMenu(splashPanel);
            yield return Motion.Wait(1.1f);
            if (PlayerPrefs.GetInt(PrefHowTo, 0) == 0)
                ShowMenu(howToPanel, homePanel);
            else
                ShowMenu(homePanel);
        }

        void ShowMenu(RectTransform panel, RectTransform backTo = null)
        {
            menuOpen = true;
            if (backTo != null)
                menuBackTo = backTo;
            HideOverlayNow();
            if (menuRoot == null)
                return;
            menuRoot.gameObject.SetActive(true);
            menuRoot.SetAsLastSibling();
            SetMenuPanel(splashPanel, panel);
            SetMenuPanel(homePanel, panel);
            SetMenuPanel(levelsPanel, panel);
            SetMenuPanel(settingsPanel, panel);
            SetMenuPanel(shopPanel, panel);
            SetMenuPanel(howToPanel, panel);
            SetMenuPanel(themePanel, panel);
            SetMenuPanel(privacyPanel, panel);
            if (panel == homePanel)
            {
                menuBackTo = null;
                FillHome();
            }
            if (panel == levelsPanel)
                RefreshLevelSelect();
            if (panel == settingsPanel)
                FillSettings();
            if (panel == shopPanel)
                FillShop();
            if (panel == themePanel)
                FillTheme();
            LayoutMenus();
        }

        static void SetMenuPanel(RectTransform panel, RectTransform active)
        {
            if (panel != null)
                panel.gameObject.SetActive(panel == active);
        }

        void HideMenu()
        {
            menuOpen = false;
            if (menuRoot != null)
                menuRoot.gameObject.SetActive(false);
        }

        void PlayContinue()
        {
            settingsFromPlay = false;
            HideMenu();
            StartCoroutine(ChangeLevelRoutine(ContinueIndex()));
        }

        void OpenLevels()
        {
            settingsFromPlay = false;
            int next = ContinueIndex();
            levelTab = 0;
            for (int i = 3; i >= 0; i--)
            {
                if (next >= TabStart(i))
                {
                    levelTab = i;
                    break;
                }
            }
            ShowMenu(levelsPanel, homePanel);
        }

        void OpenSettingsFromHome()
        {
            settingsFromPlay = false;
            ShowMenu(settingsPanel, homePanel);
        }

        void OpenShop()
        {
            RectTransform back = homePanel;
            if (themePanel != null && themePanel.gameObject.activeSelf)
                back = themePanel;
            else if (settingsPanel != null && settingsPanel.gameObject.activeSelf)
                back = settingsPanel;
            else if (settingsFromPlay)
                back = null;
            ShowMenu(shopPanel, back);
        }

        void OpenHowTo()
        {
            settingsFromPlay = false;
            ShowMenu(howToPanel, homePanel);
        }

        void OpenTheme()
        {
            ShowMenu(themePanel, settingsPanel);
        }

        void OpenPrivacy()
        {
            ShowMenu(privacyPanel, settingsPanel);
        }

        void BackFromMenu()
        {
            var to = menuBackTo;
            menuBackTo = null;
            if (to != null && to != homePanel && to != splashPanel)
            {
                ShowMenu(to, settingsFromPlay ? null : homePanel);
                return;
            }
            if (settingsFromPlay)
            {
                HideMenu();
                settingsFromPlay = false;
                Layout();
                return;
            }
            ShowMenu(homePanel);
        }

        void DailySoon()
        {
            if (homeStatus != null)
                homeStatus.text = "Daily coming soon";
        }

        void ShopSoon()
        {
            OpenShop();
        }


        int GlobalOf(int index)
        {
            if (index < 0 || index >= allLevels.Count || allLevels[index] == null)
                return index + 1;
            return allLevels[index].global > 0 ? allLevels[index].global : index + 1;
        }

        int StarsAt(int index)
        {
            return PlayerPrefs.GetInt("GridAura.BestStars." + GlobalOf(index), 0);
        }

        bool LevelUnlocked(int index)
        {
            if (index <= 0)
                return true;
            return StarsAt(index - 1) > 0;
        }

        int ContinueIndex()
        {
            int last = PlayerPrefs.GetInt(PrefLastLevel, 0);
            last = Mathf.Clamp(last, 0, Mathf.Max(0, allLevels.Count - 1));
            if (LevelUnlocked(last) && StarsAt(last) == 0)
                return last;
            for (int i = 0; i < allLevels.Count; i++)
            {
                if (LevelUnlocked(i) && StarsAt(i) == 0)
                    return i;
            }
            return last;
        }

        void FillHome()
        {
            int next = ContinueIndex();
            int shown = GlobalOf(next);
            if (homePlayLabel != null)
                homePlayLabel.text = "Level " + shown;
            if (homeStarLabel != null)
                homeStarLabel.text = TotalStars().ToString();
            if (homeStatus != null)
                homeStatus.text = "How to Play";
            if (soundIcon != null)
                soundIcon.color = soundOn ? Color.white : new Color(1f, 1f, 1f, 0.35f);
        }

        void FillSettings()
        {
            SetToggle(soundToggle, soundOn);
            SetToggle(musicToggle, musicOn);
            SetToggle(hapticsToggle, Haptics.Enabled);
            SetToggle(relaxedToggle, RelaxedTimersOn());
        }

        bool RelaxedTimersOn()
        {
            return PlayerPrefs.GetInt(PrefRelaxed, 0) == 1;
        }

        bool SpeedClockActive()
        {
            return state != null && state.HasSpeed && !RelaxedTimersOn();
        }

        void ToggleRelaxedTimers()
        {
            PlayerPrefs.SetInt(PrefRelaxed, RelaxedTimersOn() ? 0 : 1);
            PlayerPrefs.Save();
            FillSettings();
            if (state != null && state.HasSpeed)
            {
                if (RelaxedTimersOn())
                {
                    timeLeft = 0f;
                    timeFailed = false;
                }
                else if (timeLeft <= 0f && !timeFailed)
                    timeLeft = state.level != null ? state.level.timeLimitSec : 0f;
            }
            UpdateMovesHud();
        }

        void ToggleSound()
        {
            soundOn = !soundOn;
            AudioListener.volume = soundOn ? 1f : 0f;
            PlayerPrefs.SetInt(PrefSound, soundOn ? 1 : 0);
            PlayerPrefs.Save();
            FillSettings();
            FillHome();
        }

        void ToggleMusic()
        {
            musicOn = !musicOn;
            PlayerPrefs.SetInt(PrefMusic, musicOn ? 1 : 0);
            PlayerPrefs.Save();
            FillSettings();
        }

        void ToggleHaptics()
        {
            Haptics.Enabled = !Haptics.Enabled;
            PlayerPrefs.SetInt(PrefHaptics, Haptics.Enabled ? 1 : 0);
            PlayerPrefs.Save();
            FillSettings();
            if (Haptics.Enabled)
                Haptics.Tick();
        }

        void PlayLevelFromSelect(int index)
        {
            if (!LevelUnlocked(index))
                return;
            settingsFromPlay = false;
            HideMenu();
            StartCoroutine(ChangeLevelRoutine(index));
        }

        RectTransform ActiveOverlayPanel()
        {
            if (winPanel != null && winPanel.gameObject.activeSelf)
                return winPanel;
            if (hintPanel != null && hintPanel.gameObject.activeSelf)
                return hintPanel;
            if (movesPanel != null && movesPanel.gameObject.activeSelf)
                return movesPanel;
            if (debugPanel != null && debugPanel.gameObject.activeSelf)
                return debugPanel;
            return null;
        }

        bool OverlayBusy()
        {
            if (menuOpen)
                return true;
            return overlay != null && overlay.gameObject.activeSelf;
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
            StopConfetti();
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
            if (movesPanel != null)
            {
                movesPanel.gameObject.SetActive(false);
                movesPanel.localScale = Vector3.one;
            }
            if (debugPanel != null)
            {
                debugPanel.gameObject.SetActive(false);
                debugPanel.localScale = Vector3.one;
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
            if (movesPanel != null)
                movesPanel.gameObject.SetActive(panel == movesPanel);
            if (debugPanel != null)
                debugPanel.gameObject.SetActive(panel == debugPanel);
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
            var panel = ActiveOverlayPanel();
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
                hintRoutine = null;
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
            UpdateHintButton();
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
            yield return PlayHintBulbBurst();

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

            var flyer = MakeRect("HintFly", root);
            flyer.anchorMin = flyer.anchorMax = new Vector2(0.5f, 0.5f);
            flyer.pivot = new Vector2(0.5f, 0.5f);
            flyer.sizeDelta = fromSize;
            flyer.anchoredPosition = from;
            flyer.localScale = Vector3.one;
            flyer.SetAsLastSibling();
            var flyerImage = flyer.gameObject.AddComponent<Image>();
            flyerImage.raycastTarget = false;
            SetPiece(flyerImage, ColorOf(step.piece));
            var flyerGroup = flyer.gameObject.AddComponent<CanvasGroup>();
            flyerGroup.blocksRaycasts = false;
            flyerGroup.alpha = 1f;

            int turns = 0;
            state.Oriented(step.piece, state.trayTurns.ContainsKey(step.piece) ? state.trayTurns[step.piece] : 0, out int orientH, out int orientW);
            while ((orientH != step.h || orientW != step.w) && turns < 4)
            {
                if (!state.RotateTray(step.piece))
                    break;
                state.Oriented(step.piece, state.trayTurns[step.piece], out orientH, out orientW);
                Vector2 spun = slot != null ? TraySlabSize(slot, orientH, orientW) : new Vector2(Span(orientW), Span(orientH));
                Haptics.Tick();
                yield return Motion.SpinTurn(flyer, spun, 0.2f);
                if (slot != null)
                {
                    slot.label.text = orientH + " × " + orientW;
                    FitTraySlab(slot, orientH, orientW);
                    var trayGroup = slot.slabRect.GetComponent<CanvasGroup>();
                    if (trayGroup != null)
                        trayGroup.alpha = 0.18f;
                }
                fromSize = spun;
                turns++;
            }

            yield return Motion.Lift(flyer, 42f, 1.16f, 0.4f);
            yield return Motion.Wait(0.08f);
            yield return Motion.Arc(flyer, flyer.anchoredPosition, to, flyer.sizeDelta, toSize, 64f, 0.78f);
            if (flyer != null)
            {
                flyer.anchoredPosition = to;
                flyer.sizeDelta = toSize;
                yield return Motion.Sit(flyer, 0.34f, 22f, 6f);
            }

            if (flyer != null)
                Destroy(flyer.gameObject);

            if (!state.CommitHint(step))
                messageText.text = "Almost there!";
            else
            {
                SpendHint();
                messageText.text = "";
            }
            Refresh();
            var landed = placedLayer.Find(step.piece) as RectTransform;
            if (landed != null)
            {
                PlayToonFx(sparkImpactFx, landed, ToonScale(landed, 1.35f));
            }
            if (state.IsComplete())
                ShowWin();
            motionBusy = false;
            hintRoutine = null;
            UpdateHintButton();
        }

        IEnumerator PlayHintBulbBurst()
        {
            ClearHintBurst();
            if (root == null)
                yield break;
            var header = Find("Hint");
            if (header != null)
                StartCoroutine(Motion.Punch(header, 1.14f, 0.16f));
            Haptics.Hint();

            var burst = MakeRect("HintBurst", root);
            burst.anchorMin = burst.anchorMax = new Vector2(0.5f, 0.5f);
            burst.pivot = new Vector2(0.5f, 0.5f);
            burst.sizeDelta = new Vector2(300f, 300f);
            burst.anchoredPosition = header != null ? PointInRoot(header) : Vector2.zero;
            burst.localScale = Vector3.one * 0.22f;
            burst.SetAsLastSibling();
            AddGlowStack(burst, hintGlowSprite, hintBulbSprite);
            LayoutStackedGlow(burst, 0.12f);
            var group = burst.gameObject.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            group.alpha = 0f;

            Vector2 start = burst.anchoredPosition;
            yield return Motion.Tween(0.34f, u =>
            {
                if (burst == null)
                    return;
                float k = Motion.OutBack(u);
                burst.anchoredPosition = Vector2.LerpUnclamped(start, Vector2.zero, Motion.OutCubic(u));
                burst.localScale = Vector3.one * Mathf.LerpUnclamped(0.22f, 1.12f, k);
                group.alpha = Mathf.Clamp01(u * 1.4f);
            });
            if (burst == null)
                yield break;
            burst.anchoredPosition = Vector2.zero;
            burst.localScale = Vector3.one * 1.12f;
            group.alpha = 1f;
            PlayToonFx(starPoofFx, burst, 0.7f, false, 0.9f, false, 0.45f, 0.75f);
            yield return Motion.Punch(burst, 1.22f, 0.2f);
            yield return Motion.Punch(burst, 1.16f, 0.16f);
            yield return Motion.Tween(0.22f, u =>
            {
                if (burst == null)
                    return;
                burst.localScale = Vector3.one * Mathf.Lerp(1.12f, 1.45f, u);
                group.alpha = 1f - u;
            });
            if (burst != null)
                Destroy(burst.gameObject);
        }

        void ClearHintBurst()
        {
            if (root == null)
                return;
            var localBurst = root.Find("HintBurst");
            if (localBurst != null)
                Destroy(localBurst.gameObject);
        }

        public void PointerDown(PieceControl control, PointerEventData eventData)
        {
            if (menuOpen || boardIntroActive || motionBusy || rotating || OverlayBusy())
                return;
            if (dragging)
                EndDrag();
            active = control;
            pointerStart = eventData.position;
            lastPointer = eventData.position;
            moved = false;
            dragging = false;
            if (control.onBoard && !(state.placed.ContainsKey(control.pieceId) && state.placed[control.pieceId].locked))
                BeginDrag(control, eventData);
        }

        public void PointerBeginDrag(PieceControl control, PointerEventData eventData)
        {
            if (menuOpen || boardIntroActive || motionBusy || rotating || OverlayBusy())
                return;
            if (active != control)
                PointerDown(control, eventData);
            if (dragging || rotating)
                return;
            moved = true;
            BeginDrag(control, eventData);
        }

        public void PointerDrag(PieceControl control, PointerEventData eventData)
        {
            if (motionBusy || active != control)
                return;
            if (rotating)
            {
                lastPointer = eventData.position;
                return;
            }
            float slop = control.onBoard ? 0f : DragSlop;
            if (!dragging && !moved && Vector2.Distance(eventData.position, pointerStart) > slop)
            {
                moved = true;
                BeginDrag(control, eventData);
            }
            if (dragging)
                MoveDrag(eventData);
        }

        public void PointerEndDrag(PieceControl control, PointerEventData eventData)
        {
            FinishBoardDrag(control, eventData);
        }

        public void PointerUp(PieceControl control, PointerEventData eventData)
        {
            if (FinishBoardDrag(control, eventData))
                return;
            if (active != control)
                return;
            if (motionBusy || rotating)
            {
                active = null;
                return;
            }
            if (moved)
            {
                active = null;
                return;
            }
            if (control.onBoard)
                HandleBoardTap(control.pieceId);
            else if (state.IsFrozenPiece(control.pieceId) && !state.IsPlaced(control.pieceId))
                StartCoroutine(RejectFrozen(control.pieceId, false));
            else if (state.CanRotate(control.pieceId) && !state.IsPlaced(control.pieceId))
                rotateRoutine = StartCoroutine(RotateTraySlotRoutine(control.pieceId));
            active = null;
        }

        bool FinishBoardDrag(PieceControl control, PointerEventData eventData = null)
        {
            if (active != control || !dragging)
                return false;
            Vector2 pos = eventData != null ? eventData.position : lastPointer;
            if (control.onBoard && Vector2.Distance(pos, pointerStart) < BoardTapSlop)
            {
                string id = control.pieceId;
                AbortDrag();
                HandleBoardTap(id);
                active = null;
                return true;
            }
            if (rotating)
                pendingEndDrag = true;
            else if (!motionBusy)
                EndDrag();
            active = null;
            return true;
        }

        void HandleBoardTap(string id)
        {
            if (string.IsNullOrEmpty(id) || !state.IsPlaced(id))
                return;
            if (state.placed[id].locked)
            {
                if (messageText != null)
                    messageText.text = "This piece is locked";
                Haptics.Miss();
                return;
            }
            if (state.IsFrozenPiece(id))
            {
                StartCoroutine(RejectFrozen(id, true));
                return;
            }
            if (state.CanRotate(id))
            {
                if (state.CanRotateBoard(id))
                    rotateRoutine = StartCoroutine(RotateBoardRoutine(id));
                else
                    StartCoroutine(RejectBoardRotate(id));
            }
            else
                StartCoroutine(TapReturnToTray(id));
        }

        void AbortDrag()
        {
            dragging = false;
            moved = false;
            colorClashHover = false;
            pendingEndDrag = false;
            ghost.gameObject.SetActive(false);
            if (rotateButton != null)
                rotateButton.gameObject.SetActive(false);
            if (drag != null)
                drag.gameObject.SetActive(false);
            PulseColorPartner(null, false);
            if (hiddenGroup != null)
            {
                hiddenGroup.alpha = 1f;
                hiddenGroup.blocksRaycasts = true;
                hiddenGroup = null;
            }
            if (lifted != null)
            {
                state.RestorePlaced(lifted);
                lifted = null;
            }
            Refresh();
        }

        void BeginDrag(PieceControl control, PointerEventData eventData)
        {
            if (control.onBoard && state.placed.ContainsKey(control.pieceId) && state.placed[control.pieceId].locked)
            {
                if (messageText != null)
                    messageText.text = "This piece is locked";
                Haptics.Miss();
                return;
            }
            if (string.IsNullOrEmpty(control.pieceId) || state.Find(control.pieceId) == null)
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
            pendingEndDrag = false;
            if (messageText != null && (messageText.text == "Rotate is not possible" || messageText.text == "This piece is locked"))
                messageText.text = LevelHintLine();
            int h;
            int w;
            if (control.onBoard && state.placed.ContainsKey(control.pieceId))
            {
                var placed = state.placed[control.pieceId];
                h = placed.h;
                w = placed.w;
                Vector2 local = BoardLocal(eventData.position);
                int cellCol = Mathf.FloorToInt(local.x / Mathf.Max(1f, pitch));
                int cellRow = Mathf.FloorToInt(-local.y / Mathf.Max(1f, pitch));
                grabCol = Mathf.Clamp(cellCol - placed.col, 0, w - 1);
                grabRow = Mathf.Clamp(cellRow - placed.row, 0, h - 1);
                lifted = new PlacedPiece { id = placed.id, row = placed.row, col = placed.col, h = placed.h, w = placed.w };
                state.SyncTrayTurns(placed);
                state.placed.Remove(control.pieceId);
                RevealCells(lifted.col, lifted.row, lifted.w, lifted.h);
            }
            else
            {
                if (!state.trayTurns.ContainsKey(control.pieceId))
                    state.trayTurns[control.pieceId] = 0;
                state.Oriented(control.pieceId, state.trayTurns[control.pieceId], out h, out w);
                grabCol = w / 2;
                grabRow = h / 2;
            }
            dragging = true;
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
            if (hide == null)
            {
                dragging = false;
                if (lifted != null)
                {
                    state.RestorePlaced(lifted);
                    lifted = null;
                    Refresh();
                }
                return;
            }
            var group = hide.GetComponent<CanvasGroup>();
            if (group == null)
                group = hide.gameObject.AddComponent<CanvasGroup>();
            group.alpha = slot != null ? 0.28f : 0f;
            group.blocksRaycasts = slot == null;
            hiddenGroup = group;
            ClearSlotDecorations();
            rotateButton.gameObject.SetActive(false);
            MoveDrag(eventData);
        }

        void MoveDrag(PointerEventData eventData)
        {
            lastPointer = eventData.position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, eventData.position, UiCamera(), out var pointer);
            float cellsW = Mathf.Max(1, dragW);
            float cellsH = Mathf.Max(1, dragH);
            drag.anchorMin = drag.anchorMax = new Vector2(0.5f, 0.5f);
            drag.pivot = new Vector2((grabCol + 0.5f) / cellsW, 1f - (grabRow + 0.5f) / cellsH);
            drag.anchoredPosition = pointer + new Vector2(0f, pitch * 0.8f);

            Vector2 topLeft = BoardLocal(RectTransformUtility.WorldToScreenPoint(UiCamera(), drag.TransformPoint(new Vector3(drag.rect.xMin, drag.rect.yMax, 0f))));
            int col = Mathf.RoundToInt(topLeft.x / pitch);
            int row = Mathf.RoundToInt(-topLeft.y / pitch);
            bool valid = state.CanPlace(active.pieceId, row, col, dragH, dragW, lifted == null);
            bool overBoard = row < GameState.Size && col < GameState.Size && row + dragH > 0 && col + dragW > 0;
            UpdateDragRotateButton(overBoard);
            ghost.gameObject.SetActive(overBoard);
            bool clash = overBoard && state.IsColor && state.BlockedOnlyByColor(active.pieceId, row, col, dragH, dragW);
            PulseColorPartner(active.pieceId, overBoard && state.IsColor);
            string why = overBoard ? state.DropHint(active.pieceId, row, col, dragH, dragW) : null;
            if (!string.IsNullOrEmpty(why))
            {
                if (!colorClashHover)
                {
                    colorClashHover = true;
                    if (messageText != null)
                        messageText.text = why;
                    Haptics.Miss();
                    if (clash)
                        StartCoroutine(PulseClashPartner(active.pieceId));
                }
            }
            else
                colorClashHover = false;
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

        void PulseColorPartner(string id, bool on)
        {
            string partner = on ? state.ColorPartner(id) : null;
            bool clash = colorClashHover;
            foreach (Transform child in placedLayer)
            {
                var group = child.GetComponent<CanvasGroup>();
                if (group == null || group == hiddenGroup)
                    continue;
                if (partner != null && child.name == partner)
                    group.alpha = clash
                        ? 0.45f + 0.55f * (Mathf.Sin(Time.unscaledTime * 10f) * 0.5f + 0.5f)
                        : 0.55f + 0.45f * (Mathf.Sin(Time.unscaledTime * 6f) * 0.5f + 0.5f);
                else
                    group.alpha = 1f;
            }
        }

        IEnumerator PulseClashPartner(string id)
        {
            string partner = state != null ? state.ColorPartner(id) : null;
            var cell = !string.IsNullOrEmpty(partner) ? placedLayer.Find(partner) as RectTransform : null;
            if (cell == null)
                yield break;
            var image = cell.GetComponent<Image>();
            Color rest = image != null ? image.color : Color.white;
            if (image != null)
                StartCoroutine(Motion.ColorTo(image, new Color(1f, 0.38f, 0.42f, 1f), 0.08f));
            yield return Motion.ShakeX(cell, 16f, 0.16f);
            yield return Motion.Punch(cell, 1.08f, 0.1f);
            if (image != null)
                yield return Motion.ColorTo(image, rest, 0.12f);
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
            int col;
            int row;
            if (ghostCol > -50 && ghostRow > -50)
            {
                col = ghostCol;
                row = ghostRow;
            }
            else
            {
                Vector2 topLeft = BoardLocal(RectTransformUtility.WorldToScreenPoint(UiCamera(), drag.TransformPoint(new Vector3(drag.rect.xMin, drag.rect.yMax, 0f))));
                col = Mathf.RoundToInt(topLeft.x / pitch);
                row = Mathf.RoundToInt(-topLeft.y / pitch);
            }
            dragging = false;
            colorClashHover = false;
            ghost.gameObject.SetActive(false);
            rotateButton.gameObject.SetActive(false);
            PulseColorPartner(null, false);
            if (dragRoutine != null)
                StopCoroutine(dragRoutine);
            dragRoutine = StartCoroutine(SettleDrag(piece, origin, row, col));
        }

        IEnumerator SettleDrag(PieceControl piece, PlacedPiece origin, int row, int col)
        {
            motionBusy = true;
            bool onTray = ReleasedOnTray();
            bool canPlace = piece != null && !onTray && state.CanPlace(piece.pieceId, row, col, dragH, dragW, origin == null);
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
            string clashWhy = piece != null ? state.DropHint(piece.pieceId, row, col, dragH, dragW) : null;
            if (!canPlace && !onTray)
            {
                Haptics.Miss();
                if (clashWhy == "Same colors can't touch")
                {
                    if (messageText != null)
                        messageText.text = clashWhy;
                    StartCoroutine(PulseClashPartner(piece.pieceId));
                }
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
            {
                state.Remember(origin);
                state.SyncTrayTurns(origin);
            }
            else if (!onTray && canPlace)
            {
                bool movedSpot = origin == null || row != origin.row || col != origin.col || dragH != origin.h || dragW != origin.w;
                bool placed = state.TryPlace(piece.pieceId, row, col, dragH, dragW, origin == null, origin == null && movedSpot);
                if (placed)
                {
                    if (origin != null && movedSpot)
                        state.Remember(origin, false);
                }
                else if (origin != null)
                    state.RestorePlaced(origin);
            }
            else if (origin != null)
            {
                state.RestorePlaced(origin);
                string why = piece != null ? state.DropHint(piece.pieceId, row, col, dragH, dragW) : null;
                if (!string.IsNullOrEmpty(why) && messageText != null)
                    messageText.text = why;
            }
            else if (piece != null)
            {
                string why = state.DropHint(piece.pieceId, row, col, dragH, dragW);
                if (!string.IsNullOrEmpty(why) && messageText != null)
                    messageText.text = why;
            }
            Refresh();
            if (canPlace && !onTray && piece != null)
            {
                var landed = placedLayer.Find(piece.pieceId) as RectTransform;
                bool last = state.IsComplete();
                string cheer = null;
                if (!last && state.MatchesSolution(piece.pieceId))
                    cheer = PlaceCheers[Random.Range(0, PlaceCheers.Length)];
                if (landed != null)
                    yield return PlayLandImpact(landed, cheer);
                if (last)
                    yield return Motion.Wait(0.16f);
            }
            if (state.IsComplete())
                ShowWin();
            else if (state.OutOfMoves)
                ShowOutOfMoves();
            motionBusy = false;
            dragRoutine = null;
        }

        bool ReleasedOnTray()
        {
            var tray = Find("Tray");
            return tray != null && RectTransformUtility.RectangleContainsScreenPoint(tray, lastPointer, UiCamera());
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
            image.color = EmptyTint();
            FitSlice(image, 18f);
            string spot = state.SpotColorAt(row, col);
            if (!string.IsNullOrEmpty(spot))
                AddSpotDot(cell, spot, false);
        }

        void RotateDragged()
        {
            if (motionBusy || rotating || active == null || !dragging || !state.CanRotate(active.pieceId))
            {
                if (active == null || !dragging)
                    rotateButton.gameObject.SetActive(false);
                return;
            }
            rotateRoutine = StartCoroutine(RotateDragRoutine());
        }

        IEnumerator RotateTraySlotRoutine(string id)
        {
            var slot = SlotById(id);
            if (slot == null || slot.slabRect == null || rotating)
            {
                rotateRoutine = null;
                yield break;
            }
            rotating = true;
            motionBusy = true;
            if (!state.RotateTray(id))
            {
                if (state.HasRotateCap && state.RotatesLeft <= 0 && messageText != null)
                    messageText.text = "No turns left";
                rotating = false;
                motionBusy = false;
                rotateRoutine = null;
                yield break;
            }
            MarkRotateTaught();
            state.Oriented(id, state.trayTurns[id], out int h, out int w);
            Vector2 toSize = TraySlabSize(slot, h, w);
            Haptics.Tick();
            PlayToonFx(sparkImpactFx, slot.slabRect, 0.42f, false, 0.7f, false, 0.28f, 0.55f);
            yield return Motion.SpinTurn(slot.slabRect, toSize, 0.2f);
            slot.label.text = h + " × " + w;
            FitTraySlab(slot, h, w);
            yield return Motion.Punch(slot.slabRect, 1.08f, 0.1f);
            DrawTutorialAids();
            rotating = false;
            motionBusy = false;
            rotateRoutine = null;
        }

        IEnumerator RotateBoardRoutine(string id)
        {
            var cell = placedLayer.Find(id) as RectTransform;
            if (cell == null || rotating || !state.CanRotateBoard(id))
            {
                rotateRoutine = null;
                yield break;
            }
            rotating = true;
            motionBusy = true;
            PinPivotCenter(cell);
            state.FindBoardRotation(id, out _, out _, out int nextH, out int nextW);
            Vector2 toSize = new Vector2(Span(nextW), Span(nextH));
            Haptics.Tick();
            PlayToonFx(sparkImpactFx, cell, 0.5f, false, 0.7f, false, 0.3f, 0.6f);
            yield return Motion.SpinTurn(cell, toSize, 0.2f);
            if (!state.RotateBoard(id))
            {
                Haptics.Miss();
                rotating = false;
                motionBusy = false;
                rotateRoutine = null;
                Refresh();
                yield break;
            }
            Refresh();
            rotating = false;
            motionBusy = false;
            rotateRoutine = null;
        }

        IEnumerator RotateDragRoutine()
        {
            rotating = true;
            if (state.RotateTray(active.pieceId))
                MarkRotateTaught();
            PinPivotCenter(drag);
            state.Oriented(active.pieceId, state.trayTurns[active.pieceId], out dragH, out dragW);
            Vector2 toSize = new Vector2(Span(dragW), Span(dragH));
            Haptics.Tick();
            PlayToonFx(sparkImpactFx, drag, 0.5f, false, 0.7f, false, 0.3f, 0.6f);
            if (rotateButton != null && rotateButton.gameObject.activeSelf)
                StartCoroutine(Motion.Punch(rotateButton, 1.1f, 0.12f));
            yield return Motion.SpinTurn(drag, toSize, 0.2f);
            if (active == null)
            {
                rotating = false;
                rotateRoutine = null;
                pendingEndDrag = false;
                yield break;
            }
            SetPiece(dragImage, ColorOf(active.pieceId));
            grabCol = Mathf.Clamp(grabCol, 0, dragW - 1);
            grabRow = Mathf.Clamp(grabRow, 0, dragH - 1);
            ghostCol = -99;
            var data = new PointerEventData(EventSystem.current) { position = lastPointer };
            MoveDrag(data);
            rotating = false;
            rotateRoutine = null;
            if (pendingEndDrag)
            {
                pendingEndDrag = false;
                EndDrag();
            }
        }

        void PinPivotCenter(RectTransform rect)
        {
            if (rect == null)
                return;
            Vector3 world = rect.TransformPoint(rect.rect.center);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.position = world;
        }

        void OnUndo()
        {
            if (motionBusy || OverlayBusy() || !state.CanUndo)
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
            if (motionBusy || OverlayBusy() || !state.HasMovablePlaced())
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

        IEnumerator TapReturnToTray(string id)
        {
            if (string.IsNullOrEmpty(id) || !state.IsPlaced(id) || (state.placed.ContainsKey(id) && state.placed[id].locked))
                yield break;
            motionBusy = true;
            var board = placedLayer.Find(id) as RectTransform;
            var slot = SlotById(id);
            if (board != null && slot != null)
            {
                var flyer = MakeFlyer(id, PointInRoot(board), board.sizeDelta);
                yield return Motion.Arc(flyer, PointInRoot(board), PointInRoot(slot.slabRect), board.sizeDelta, slot.slabRect.sizeDelta, 22f, 0.18f);
                Destroy(flyer.gameObject);
            }
            state.ReturnToTray(id, true);
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
            settingsFromPlay = true;
            ShowMenu(settingsPanel);
        }

        void ShowDebugPanel()
        {
            if (debugPanel == null || motionBusy)
                return;
            if (debugLevelInput != null)
            {
                int shown = state.level != null && state.level.global > 0 ? state.level.global : levelIndex + 1;
                debugLevelInput.text = shown.ToString();
            }
            if (debugStatus != null)
                debugStatus.text = "Levels 1–" + Mathf.Max(1, allLevels.Count);
            ShowOverlay(debugPanel);
        }

        void JumpDebugLevel()
        {
            if (debugLevelInput == null || allLevels.Count == 0)
                return;
            string raw = debugLevelInput.text != null ? debugLevelInput.text.Trim() : "";
            if (!int.TryParse(raw, out int global) || global < 1)
            {
                if (debugStatus != null)
                    debugStatus.text = "Enter a level number";
                return;
            }
            int index = IndexOfGlobal(global);
            if (index < 0)
            {
                if (debugStatus != null)
                    debugStatus.text = "No level " + global + " (1–" + allLevels.Count + ")";
                return;
            }
            if (debugStatus != null)
                debugStatus.text = "Jumping to " + global;
            HideMenu();
            StartCoroutine(ChangeLevelRoutine(index));
        }

        int IndexOfGlobal(int global)
        {
            for (int i = 0; i < allLevels.Count; i++)
            {
                var lvl = allLevels[i];
                int id = lvl != null && lvl.global > 0 ? lvl.global : i + 1;
                if (id == global)
                    return i;
            }
            if (global >= 1 && global <= allLevels.Count)
                return global - 1;
            return -1;
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
            if (hint == null || !hint.gameObject.activeSelf)
                return;
            if (hintGroup != null && !hintGroup.blocksRaycasts)
                return;
            var bulb = hint.Find("Bulb");
            if (bulb == null)
                return;
            float s = 1f + 0.03f * Mathf.Sin(Time.unscaledTime * 2.2f);
            bulb.localScale = Vector3.one * s;
        }

        void UpdateHintButton()
        {
            var hint = Find("Hint");
            if (hint == null)
                return;
            bool unlocked = HintUnlocked;
            hint.gameObject.SetActive(unlocked);
            if (hintGroup == null)
                hintGroup = hint.GetComponent<CanvasGroup>();
            if (hintGroup == null)
                hintGroup = hint.gameObject.AddComponent<CanvasGroup>();
            bool canTap = unlocked && !boardIntroActive && !celebrating && !OverlayBusy() && (state.level == null || !state.IsComplete());
            hintGroup.alpha = canTap ? 1f : 0.4f;
            hintGroup.blocksRaycasts = canTap;
            hintGroup.interactable = canTap;
        }

        void NextLevel()
        {
            if (motionBusy)
                return;
            if (levelIndex >= allLevels.Count - 1)
            {
                GoHome();
                return;
            }
            StartCoroutine(ChangeLevelRoutine(levelIndex + 1));
        }

        IEnumerator ChangeLevelRoutine(int index)
        {
            motionBusy = true;
            StopTransientFx();
            yield return HideOverlayRoutine();
            StopConfetti();
            if (root != null)
                root.anchoredPosition = Vector2.zero;
            int dir = NextTransition();
            SizeTransitionCover();
            PlaceTransitionBar(dir);
            Vector2 rest = Vector2.zero;
            Vector2 from = TransitionOffset(dir);
            Vector2 away = OppositeTransitionOffset(dir);
            if (transitionCover != null)
            {
                transitionCover.gameObject.SetActive(true);
                transitionCover.SetAsLastSibling();
                transitionCover.anchoredPosition = from;
                yield return Motion.Move(transitionCover, rest, 0.32f);
            }
            ShowLevel(index);
            yield return Motion.Wait(0.06f);
            if (transitionCover != null)
            {
                yield return Motion.Move(transitionCover, away, 0.34f);
                transitionCover.anchoredPosition = rest;
                transitionCover.gameObject.SetActive(false);
            }
        }

        Vector2 TransitionOffset(int dir)
        {
            float w = transitionCover != null ? transitionCover.rect.width : ReferenceWidth;
            float h = transitionCover != null ? transitionCover.rect.height : ReferenceHeight;
            if (dir == 0)
                return new Vector2(-w, 0f);
            if (dir == 1)
                return new Vector2(w, 0f);
            if (dir == 2)
                return new Vector2(0f, -h);
            return new Vector2(0f, h);
        }

        Vector2 OppositeTransitionOffset(int dir)
        {
            return -TransitionOffset(dir);
        }

        void SizeTransitionCover()
        {
            if (transitionCover == null || canvas == null)
                return;
            var canvasRect = canvas.GetComponent<RectTransform>();
            float w = canvasRect.rect.width;
            float h = canvasRect.rect.height;
            if (w < 8f)
            {
                w = ReferenceWidth;
                h = ReferenceHeight;
            }
            transitionCover.sizeDelta = new Vector2(w, h);
        }

        void PlaceTransitionBar(int dir)
        {
            if (transitionBar == null)
                return;
            float thick = 22f;
            if (dir == 0)
            {
                transitionBar.anchorMin = new Vector2(1f, 0f);
                transitionBar.anchorMax = new Vector2(1f, 1f);
                transitionBar.pivot = new Vector2(1f, 0.5f);
                transitionBar.sizeDelta = new Vector2(thick, 0f);
                transitionBar.anchoredPosition = Vector2.zero;
            }
            else if (dir == 1)
            {
                transitionBar.anchorMin = new Vector2(0f, 0f);
                transitionBar.anchorMax = new Vector2(0f, 1f);
                transitionBar.pivot = new Vector2(0f, 0.5f);
                transitionBar.sizeDelta = new Vector2(thick, 0f);
                transitionBar.anchoredPosition = Vector2.zero;
            }
            else if (dir == 2)
            {
                transitionBar.anchorMin = new Vector2(0f, 1f);
                transitionBar.anchorMax = new Vector2(1f, 1f);
                transitionBar.pivot = new Vector2(0.5f, 1f);
                transitionBar.sizeDelta = new Vector2(0f, thick);
                transitionBar.anchoredPosition = Vector2.zero;
            }
            else
            {
                transitionBar.anchorMin = new Vector2(0f, 0f);
                transitionBar.anchorMax = new Vector2(1f, 0f);
                transitionBar.pivot = new Vector2(0.5f, 0f);
                transitionBar.sizeDelta = new Vector2(0f, thick);
                transitionBar.anchoredPosition = Vector2.zero;
            }
        }

        int NextTransition()
        {
            int dir = Random.Range(0, 4);
            if (dir == lastTransition)
                dir = (dir + 1 + Random.Range(0, 3)) % 4;
            lastTransition = dir;
            return dir;
        }

        void ShowLevel(int index)
        {
            levelIndex = Mathf.Clamp(index, 0, Mathf.Max(0, allLevels.Count - 1));
            PlayerPrefs.SetInt(PrefLastLevel, levelIndex);
            PlayerPrefs.Save();
            var next = allLevels[levelIndex];
            bool tutorial = next != null && next.source == "tutorial";
            state.Begin(next, catalogPieces, tutorial, HintBank(), UnlimitedHints());
            timeFailed = false;
            timeLeft = SpeedClockActive() ? next.timeLimitSec : 0f;
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
            int shown = next != null && next.global > 0 ? next.global : levelIndex + 1;
            levelNumber.text = shown.ToString();
            levelNumber.color = TagColor(next != null ? next.tag : null);
            messageText.text = LevelHintLine();
            fingerDragStopped = false;
            fingerTapStopped = false;
            celebrating = false;
            rotating = false;
            pendingEndDrag = false;
            boardIntroActive = true;
            motionBusy = true;
            StopTransientFx();
            HideOverlayNow();
            RestoreCoach();
            hudLanded = !state.HasMoveLimit;
            briefingSkip = false;
            briefingDocked = true;
            if (briefing != null)
            {
                briefing.gameObject.SetActive(false);
                briefing.localScale = Vector3.one;
            }
            if (hudLanded)
                ShowTagBanner();
            Refresh();
            if (rotateRoutine != null)
            {
                StopCoroutine(rotateRoutine);
                rotateRoutine = null;
            }
            if (hintRoutine != null)
            {
                StopCoroutine(hintRoutine);
                hintRoutine = null;
            }
            ClearHintBurst();
            if (introRoutine != null)
                StopCoroutine(introRoutine);
            introRoutine = StartCoroutine(LevelIntro());
            UpdateHintButton();
        }

        void Refresh()
        {
            SyncTraySlots();
            Layout();
            hintText.text = "∞";
            for (int i = placedLayer.childCount - 1; i >= 0; i--)
            {
                var child = placedLayer.GetChild(i);
                child.gameObject.SetActive(false);
                Destroy(child.gameObject);
            }
            for (int i = grid.childCount - 1; i >= 0; i--)
            {
                var child = grid.GetChild(i);
                if (child == placedLayer || child == ghost)
                    continue;
                child.gameObject.SetActive(false);
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
                if (state.IsFrozenPiece(piece.id))
                    AddFrozenBadge(cell);
                AddCoveredSpotRings(cell, piece);
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
                bool keepHint = hintPanel != null && hintPanel.gameObject.activeSelf;
                bool keepMoves = movesPanel != null && movesPanel.gameObject.activeSelf;
                if (!keepHint && !keepMoves)
                    HideOverlayNow();
            }
            UpdateMovesHud();
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
            hit.offsetMin = Vector2.zero;
            hit.offsetMax = Vector2.zero;
            var image = hit.gameObject.AddComponent<Image>();
            image.color = Color.clear;
            image.raycastTarget = true;
            var control = hit.gameObject.AddComponent<PieceControl>();
            control.screen = this;
            control.pieceId = piece.id;
            control.onBoard = true;
        }

        IEnumerator RejectBoardRotate(string id)
        {
            var cell = placedLayer.Find(id) as RectTransform;
            if (cell == null)
                yield break;
            motionBusy = true;
            Haptics.Miss();
            if (messageText != null)
                messageText.text = "Rotate is not possible";
            yield return Motion.ShakeX(cell, 22f, 0.22f);
            if (cell != null)
                yield return Motion.Punch(cell, 1.05f, 0.1f);
            motionBusy = false;
        }

        IEnumerator RejectFrozen(string id, bool onBoard)
        {
            RectTransform cell = onBoard
                ? placedLayer.Find(id) as RectTransform
                : (SlotById(id) != null ? SlotById(id).slabRect : null);
            motionBusy = true;
            Haptics.Miss();
            if (messageText != null)
                messageText.text = "Frozen";
            if (cell != null)
                yield return Motion.ShakeX(cell, 18f, 0.2f);
            motionBusy = false;
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

        Vector2 TraySlabSize(TraySlot slot, int h, int w)
        {
            float maxW = Mathf.Max(8f, slot.root.rect.width * 0.84f);
            float maxH = Mathf.Max(8f, slot.root.rect.height * 0.54f);
            float unit = Mathf.Min(maxW / Mathf.Max(1, w), maxH / Mathf.Max(1, h));
            if (unit < 4f)
                unit = 4f;
            return new Vector2(Mathf.Min(maxW, unit * w), Mathf.Min(maxH, unit * h));
        }

        void FitTraySlab(TraySlot slot, int h, int w)
        {
            slot.slabRect.anchorMin = slot.slabRect.anchorMax = new Vector2(0.5f, 0.62f);
            slot.slabRect.pivot = new Vector2(0.5f, 0.5f);
            slot.slabRect.anchoredPosition = Vector2.zero;
            slot.slabRect.sizeDelta = TraySlabSize(slot, h, w);
            slot.slabRect.localRotation = Quaternion.identity;
            slot.slabRect.localScale = Vector3.one;
            SetPiece(slot.slab, ColorOf(slot.id));
            var frozenMark = slot.slabRect.Find("Frozen");
            if (state.IsFrozenPiece(slot.id))
                AddFrozenBadge(slot.slabRect);
            else if (frozenMark != null)
                Destroy(frozenMark.gameObject);
            slot.label.rectTransform.anchorMin = new Vector2(0f, 0f);
            slot.label.rectTransform.anchorMax = new Vector2(1f, 0.24f);
            slot.label.rectTransform.offsetMin = Vector2.zero;
            slot.label.rectTransform.offsetMax = Vector2.zero;
        }

        void Layout()
        {
            Canvas.ForceUpdateCanvases();
            if (root == null || root.rect.width < 10f)
                return;
            Rect safe = Screen.safeArea;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, safe.min, UiCamera(), out var min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, new Vector2(safe.xMax, safe.yMax), UiCamera(), out var max);
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
            int trayCount = TrayVisibleCount();
            bool oneRow = trayCount <= 4;
            float trayH = oneRow ? Mathf.Clamp(width * 0.28f, 240f, 320f) : Mathf.Clamp(width * 0.42f, 360f, 480f);
            bool showCoach = coach != null && coach.gameObject.activeSelf;
            float coachH = showCoach ? 110f : 0f;
            bool showInfo = state != null && (state.HasMoveLimit || (state.level != null && !state.IsTutorial));
            float infoH = showInfo ? 48f : 0f;
            float spacing = 20f;
            int gaps = 4;
            if (showCoach) gaps++;
            if (showInfo) gaps++;
            float used = topH + coachH + infoH + numberH + trayH + buttonH + spacing * gaps;
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
            if (showInfo && movesPill != null)
            {
                Place(movesPill, left + width * 0.18f, afterHeader - infoH, width * 0.64f, infoH);
                if (levelSprite != null)
                    FitSlice(movesPillImage, 68f, infoH * 0.5f);
                movesPill.gameObject.SetActive(hudLanded);
                LayoutMovesHud(width * 0.64f, infoH);
                afterHeader -= infoH + spacing;
            }
            else if (movesPill != null)
                movesPill.gameObject.SetActive(false);
            if (showCoach)
            {
                Place(coach, left, afterHeader - coachH, width, coachH);
                afterHeader -= coachH + spacing;
            }
            if (banner != null && banner.gameObject.activeSelf)
                Place(banner, left, afterHeader + 8f, width, 64f);
            float frameLeft = left + numberH;
            float boardTop = afterHeader - numberH;
            float boardBottom = boardTop - boardSize;
            Place(boardFrame, frameLeft, boardBottom, boardSize, boardSize);
            if (frameImage != null)
                frameImage.color = FrameTint();
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

            if (overlay != null)
            {
                Stretch(overlay);
                overlay.SetAsLastSibling();
            }
            if (briefing != null && briefing.gameObject.activeSelf && briefingDocked)
            {
                LayoutBriefingCard(left, width, boardBottom, boardSize);
                briefing.SetAsLastSibling();
            }
            float cardW = Mathf.Min(860f, width * 0.9f);
            float cardH = Mathf.Min(1040f, cardW * 1.22f);
            SizeCentered(winPanel, cardW, cardH);
            SizeCentered(hintPanel, cardW, cardH);
            confirmPanel.anchorMin = confirmPanel.anchorMax = new Vector2(0.5f, 0.5f);
            confirmPanel.sizeDelta = new Vector2(Mathf.Min(820f, width), 420f);
            if (movesPanel != null)
            {
                bool mercy = keepGoingButton != null && keepGoingButton.gameObject.activeSelf;
                SizeCentered(movesPanel, Mathf.Min(820f, width), mercy ? 540f : 420f);
            }
            if (debugPanel != null)
                SizeCentered(debugPanel, Mathf.Min(820f, width), 520f);
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
            if (debugPanel != null)
                LayoutPill(debugPanel.Find("Jump") as RectTransform, 110f);

            DrawNumbers(frameLeft, boardTop, rim, numberH);
            foreach (var slot in slots)
            {
                if (slot.id != null && !state.IsPlaced(slot.id))
                {
                    state.Oriented(slot.id, state.trayTurns[slot.id], out int h, out int w);
                    FitTraySlab(slot, h, w);
                }
            }
            UpdateHintButton();
            LayoutMenus();
        }

        void DrawNumbers(float frameLeft, float frameTop, float rim, float numberH)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
            {
                var child = root.GetChild(i);
                if (child.name.StartsWith("GridNum"))
                    Destroy(child.gameObject);
            }
            int boardIndex = boardFrame != null ? boardFrame.GetSiblingIndex() : 0;
            for (int i = 0; i < 8; i++)
            {
                var col = MakeText(root, (i + 1).ToString(), 28, new Color(1f, 1f, 1f, 0.7f), fontSemiBold);
                col.gameObject.name = "GridNumC" + i;
                col.raycastTarget = false;
                Place(col.rectTransform, frameLeft + rim + i * pitch, frameTop, pitch, numberH);
                col.rectTransform.SetSiblingIndex(boardIndex);
                var row = MakeText(root, (i + 1).ToString(), 28, new Color(1f, 1f, 1f, 0.7f), fontSemiBold);
                row.gameObject.name = "GridNumR" + i;
                row.raycastTarget = false;
                Place(row.rectTransform, frameLeft - numberH, frameTop - rim - (i + 1) * pitch, numberH, pitch);
                row.rectTransform.SetSiblingIndex(boardIndex);
            }
        }

        int TrayVisibleCount()
        {
            int n = 0;
            foreach (var slot in slots)
            {
                if (slot.id != null && !state.IsPlaced(slot.id))
                    n++;
            }
            return n;
        }

        void SyncTraySlots()
        {
            foreach (var slot in slots)
            {
                bool placed = slot.id != null && state.IsPlaced(slot.id);
                bool inTray = slot.id != null && !placed;
                bool spare = state.IsDecoy && slot.id != null && state.level != null && slot.id == state.level.unused && state.IsComplete();
                slot.root.gameObject.SetActive(inTray);
                slot.slab.gameObject.SetActive(inTray);
                slot.hit.gameObject.SetActive(inTray);
                var slabGroup = slot.slabRect.GetComponent<CanvasGroup>();
                if (slabGroup != null)
                {
                    slabGroup.alpha = spare ? 0.35f : 1f;
                    slabGroup.blocksRaycasts = true;
                }
                slot.control.pieceId = slot.id;
                slot.control.onBoard = false;
                slot.label.color = spare ? new Color(1f, 1f, 1f, 0.55f) : Color.white;
                if (spare)
                    slot.label.text = "spare";
                else if (inTray)
                {
                    var def = state.Find(slot.id);
                    if (def != null)
                    {
                        state.Oriented(slot.id, state.trayTurns[slot.id], out int h, out int w);
                        slot.label.text = h + " × " + w;
                    }
                }
            }
        }

        void LayoutTray(RectTransform tray)
        {
            int count = TrayVisibleCount();
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
                if (slot.id == null || state.IsPlaced(slot.id))
                    continue;
                if (!slot.root.gameObject.activeSelf)
                    slot.root.gameObject.SetActive(true);
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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(grid, screen, UiCamera(), out var local);
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

        Color EmptyTint()
        {
            return NeonOn() ? Hex("C8F4FF") : Color.white;
        }

        Color FrameTint()
        {
            return NeonOn() ? Hex("5C7CFF") : Color.white;
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
            tutorialCount = 0;
            var tutorialAsset = Resources.Load<TextAsset>("Data/tutorial_levels");
            var mainAsset = Resources.Load<TextAsset>("Data/levels");
            var specialAsset = Resources.Load<TextAsset>("Data/special_levels");
            var packAsset = Resources.Load<TextAsset>("Data/campaign_levels");
            var campaignAsset = Resources.Load<TextAsset>("Data/campaign");
            LevelCatalog tutorial = null;
            if (tutorialAsset != null)
                tutorial = JsonUtility.FromJson<LevelCatalog>(tutorialAsset.text);
            if (mainAsset != null)
                mainLevels = JsonUtility.FromJson<LevelCatalog>(mainAsset.text);
            SpecialCatalog specials = specialAsset != null ? JsonUtility.FromJson<SpecialCatalog>(specialAsset.text) : null;
            if (tutorial != null && tutorial.pieces != null)
                catalogPieces = tutorial.pieces;
            else if (mainLevels != null)
                catalogPieces = mainLevels.pieces;

            var tutorialMap = new Dictionary<int, LevelDef>();
            if (tutorial != null && tutorial.levels != null)
            {
                foreach (var lvl in tutorial.levels)
                {
                    if (lvl != null)
                        tutorialMap[lvl.id] = lvl;
                }
            }
            var mainMap = new Dictionary<int, LevelDef>();
            if (mainLevels != null && mainLevels.levels != null)
            {
                foreach (var lvl in mainLevels.levels)
                {
                    if (lvl != null)
                        mainMap[lvl.id] = lvl;
                }
            }
            var specialMap = new Dictionary<string, SpecialLevelDef>();
            if (specials != null && specials.levels != null)
            {
                foreach (var lvl in specials.levels)
                {
                    if (lvl != null && !string.IsNullOrEmpty(lvl.id))
                        specialMap[lvl.id] = lvl;
                }
            }
            SpecialCatalog pack = packAsset != null ? JsonUtility.FromJson<SpecialCatalog>(packAsset.text) : null;
            if (pack != null && pack.levels != null)
            {
                foreach (var lvl in pack.levels)
                {
                    if (lvl != null && !string.IsNullOrEmpty(lvl.id))
                        specialMap[lvl.id] = lvl;
                }
            }

            var campaign = campaignAsset != null ? JsonUtility.FromJson<CampaignFile>(campaignAsset.text) : null;
            if (campaign != null && campaign.levels != null && campaign.levels.Length > 0)
            {
                foreach (var entry in campaign.levels)
                {
                    var resolved = ResolveCampaign(entry, tutorialMap, mainMap, specialMap);
                    if (resolved == null)
                        continue;
                    allLevels.Add(resolved);
                    if (resolved.source == "tutorial")
                        tutorialCount++;
                }
            }
            if (allLevels.Count == 0)
            {
                if (tutorial != null && tutorial.levels != null)
                {
                    tutorialCount = tutorial.levels.Length;
                    allLevels.AddRange(tutorial.levels);
                }
                if (mainLevels != null && mainLevels.levels != null)
                    allLevels.AddRange(mainLevels.levels);
            }

            firstRotateId = 7;
            for (int i = 0; i < tutorialCount && i < allLevels.Count; i++)
            {
                var lvl = allLevels[i];
                if (lvl != null && lvl.tutorial != null && lvl.tutorial.rotationEnabled)
                {
                    firstRotateId = lvl.id;
                    break;
                }
            }
        }

        static LevelDef CloneLevel(LevelDef src)
        {
            return src == null ? new LevelDef() : JsonUtility.FromJson<LevelDef>(JsonUtility.ToJson(src));
        }

        static LevelDef FromSpecial(SpecialLevelDef spec)
        {
            if (spec == null)
                return new LevelDef();
            return new LevelDef
            {
                id = 0,
                key = spec.id,
                type = spec.type,
                difficulty = spec.type,
                unused = spec.unused,
                unusedPieces = spec.unusedPieces,
                rotateCap = spec.rotateCap,
                rules = spec.rules,
                wrongPieces = spec.wrongPieces,
                black = spec.black,
                toPlace = spec.toPlace,
                solution = spec.solution,
                initial = spec.initial,
                preplaced = spec.preplaced,
                rotationNeeded = spec.rotationNeeded,
                coachText = spec.coachText,
                intro = spec.intro,
                spots = spec.spots,
                frozen = spec.frozen
            };
        }

        static LevelDef ResolveCampaign(
            CampaignEntry entry,
            Dictionary<int, LevelDef> tutorialMap,
            Dictionary<int, LevelDef> mainMap,
            Dictionary<string, SpecialLevelDef> specialMap)
        {
            if (entry == null)
                return null;
            LevelDef clone = null;
            int numericId;
            if (entry.source == "tutorial" && int.TryParse(entry.id, out numericId) && tutorialMap.ContainsKey(numericId))
                clone = CloneLevel(tutorialMap[numericId]);
            else if (entry.source == "main" && int.TryParse(entry.id, out numericId) && mainMap.ContainsKey(numericId))
                clone = CloneLevel(mainMap[numericId]);
            else if (!string.IsNullOrEmpty(entry.id) && specialMap.ContainsKey(entry.id))
                clone = FromSpecial(specialMap[entry.id]);
            if (clone == null)
                return null;
            clone.source = entry.source;
            clone.key = entry.source + ":" + entry.id;
            clone.tag = entry.tag;
            clone.global = entry.global;
            clone.world = entry.world;
            clone.worldName = entry.worldName;
            clone.posInWorld = entry.posInWorld;
            clone.par = entry.par;
            clone.moveLimit = entry.moveLimit;
            clone.intro = entry.intro;
            clone.modifier = entry.modifier;
            clone.timeLimitSec = entry.timeLimitSec;
            if (!string.IsNullOrEmpty(entry.coachText))
                clone.coachText = entry.coachText;
            if (entry.source == "color" || entry.source == "decoy" || entry.source == "rescue")
                clone.type = entry.source;
            else if (string.IsNullOrEmpty(clone.type))
                clone.type = "classic";
            return clone;
        }

        void RestoreCoach()
        {
            coachHidden = false;
            if (coach == null)
                return;
            string text = CoachText();
            if (!string.IsNullOrEmpty(text))
            {
                coachLabel.text = text;
                coach.gameObject.SetActive(true);
                if (coachGroup != null)
                    coachGroup.alpha = 0f;
            }
            else
            {
                coach.gameObject.SetActive(false);
            }
        }

        string CoachText()
        {
            var lvl = state.level;
            if (lvl == null)
                return null;
            if (lvl.tutorial != null && !string.IsNullOrEmpty(lvl.tutorial.coachText))
                return lvl.tutorial.coachText;
            if (!string.IsNullOrEmpty(lvl.coachText))
                return lvl.coachText;
            if (lvl.world > 0 && lvl.posInWorld == 1)
            {
                string key = "GridAura.WorldSeen." + lvl.world;
                if (PlayerPrefs.GetInt(key, 0) == 0)
                {
                    PlayerPrefs.SetInt(key, 1);
                    PlayerPrefs.Save();
                    return string.IsNullOrEmpty(lvl.worldName) ? "World " + lvl.world : lvl.worldName;
                }
            }
            return null;
        }

        string LevelHintLine()
        {
            var parts = new System.Collections.Generic.List<string>();
            if (state.IsSpots)
                parts.Add("Match the dot colors");
            if (state.IsFrozen)
                parts.Add("Frozen pieces can't turn");
            if (SpeedClockActive())
                parts.Add("Beat the clock");
            if (state.HasRule("doublespare"))
                parts.Add("Leave two pieces in the tray");
            else if (state.IsDecoy)
                parts.Add("Use 7 of 8 pieces");
            if (state.IsColor)
                parts.Add("Same colors can't touch");
            if (state.IsRescue)
                parts.Add("Fix the misplaced pieces");
            if (state.IsMustTouch)
                parts.Add("Matching colors must touch");
            if (state.HasRule("keystone"))
                parts.Add("A locked piece is already set");
            if (state.HasRule("islands"))
                parts.Add("Stay inside a room");
            if (state.IsBorder)
                parts.Add("Every piece must touch the edge");
            if (state.IsStoneHug)
                parts.Add("Every piece must touch black");
            if (state.IsBigFirst)
                parts.Add("Biggest leftover piece first");
            if (state.HasRotateCap)
                parts.Add("Only " + state.level.rotateCap + " turns");
            if (state.IsNoUndo)
                parts.Add("Undo is off this level");
            if (parts.Count == 0)
                return "";
            return string.Join(", ", parts);
        }

        static Color TagColor(string tag)
        {
            switch (tag)
            {
                case "easy": return Hex("2E9B57");
                case "medium": return Hex("C9A227");
                case "hard": return Hex("E65100");
                case "boss": return Hex("D32F2F");
                default: return Hex("1E7BFF");
            }
        }

        void LayoutMovesHud(float w, float h)
        {
            UpdateMovesHud();
            if (headerStars != null)
            {
                headerStars.alignment = TextAlignmentOptions.MidlineRight;
                headerStars.ForceMeshUpdate();
            }
            if (movesLabel != null)
            {
                var rect = movesLabel.rectTransform;
                rect.anchorMin = rect.anchorMax = new Vector2(0f, 0.5f);
                rect.pivot = new Vector2(0f, 0.5f);
                rect.anchoredPosition = new Vector2(28f, 0f);
                rect.sizeDelta = new Vector2(w * 0.52f, h);
                movesLabel.alignment = TextAlignmentOptions.MidlineLeft;
            }
            float star = Mathf.Min(h * 0.62f, 40f);
            float starsW = headerStars != null ? Mathf.Max(36f, headerStars.preferredWidth + 6f) : 36f;
            if (headerStars != null)
            {
                var rect = headerStars.rectTransform;
                rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
                rect.pivot = new Vector2(1f, 0.5f);
                rect.anchoredPosition = new Vector2(-22f, 0f);
                rect.sizeDelta = new Vector2(starsW, h);
            }
            if (headerStarIcon != null)
            {
                var rect = headerStarIcon.rectTransform;
                rect.anchorMin = rect.anchorMax = new Vector2(1f, 0.5f);
                rect.pivot = new Vector2(1f, 0.5f);
                rect.sizeDelta = new Vector2(star, star);
                rect.anchoredPosition = new Vector2(-22f - starsW - 6f, 0f);
            }
        }

        void UpdateMovesHud()
        {
            if (headerStars != null)
                headerStars.text = TotalStars().ToString();
            if (movesLabel == null)
                return;
            if (SpeedClockActive())
            {
                int secs = Mathf.Max(0, Mathf.CeilToInt(timeLeft));
                int mm = secs / 60;
                int ss = secs % 60;
                movesLabel.text = "Time " + mm + ":" + ss.ToString("00");
                if (movesPillImage != null)
                    movesPillImage.color = Color.white;
                if (secs <= 10)
                    movesLabel.color = Hex("D32F2F");
                else if (secs <= 30)
                    movesLabel.color = Hex("E65100");
                else
                    movesLabel.color = Hex("2B2B2B");
                return;
            }
            if (!state.HasMoveLimit)
            {
                movesLabel.text = "";
                return;
            }
            movesLabel.text = "Moves " + state.movesLeft;
            if (movesPillImage != null)
                movesPillImage.color = Color.white;
            if (state.movesLeft <= 1)
                movesLabel.color = Hex("D32F2F");
            else if (state.movesLeft <= 3)
                movesLabel.color = Hex("E65100");
            else
                movesLabel.color = Hex("2B2B2B");
        }

        void TickSpeedClock()
        {
            if (!SpeedClockActive() || timeFailed || celebrating || state.IsComplete())
                return;
            if (!hudLanded || boardIntroActive || OverlayBusy() || motionBusy || hintRoutine != null)
                return;
            timeLeft -= Time.unscaledDeltaTime;
            if (timeLeft <= 0f)
            {
                timeLeft = 0f;
                ShowTimeUp();
            }
            UpdateMovesHud();
        }

        string LevelTypeTitle()
        {
            var tag = state.level != null ? state.level.tag : "";
            string kind = "Classic";
            if (state.HasRule("doublespare"))
                kind = "Double Spare";
            else if (state.IsSpots && state.IsFrozen)
                kind = "Spots & Frozen";
            else if (state.IsSpots)
                kind = "Paint Spots";
            else if (state.IsFrozen)
                kind = "Frozen Piece";
            else if (state.IsColor)
                kind = "Color Clash";
            else if (state.IsDecoy)
                kind = "Odd One Out";
            else if (state.IsRescue)
                kind = "Rescue";
            else if (state.IsMustTouch)
                kind = "Must Touch";
            else if (state.HasRule("keystone"))
                kind = "Keystone";
            else if (state.HasRule("islands"))
                kind = "Islands";
            else if (state.IsBorder)
                kind = "Border Seat";
            else if (state.IsStoneHug)
                kind = "Stone Hug";
            else if (state.IsBigFirst)
                kind = "Big First";
            else if (state.HasRotateCap)
                kind = "Turn Limit";
            else if (state.IsNoUndo)
                kind = "No Undo";
            if (SpeedClockActive() && kind == "Classic")
                kind = "Speed";
            if (tag == "boss")
                return "Boss " + kind;
            if (tag == "hard")
                return "Hard " + kind;
            return kind;
        }

        void FillBriefing()
        {
            if (briefingType != null)
                briefingType.text = LevelTypeTitle();
            if (briefingBody != null)
            {
                string body = LevelHintLine();
                briefingBody.text = string.IsNullOrEmpty(body) ? "Fill every empty cell" : body;
            }
            if (briefingMoves != null)
            {
                if (SpeedClockActive())
                {
                    int secs = Mathf.Max(0, Mathf.CeilToInt(timeLeft > 0f ? timeLeft : state.level.timeLimitSec));
                    briefingMoves.text = "Time " + (secs / 60) + ":" + (secs % 60).ToString("00");
                }
                else
                    briefingMoves.text = state.HasMoveLimit ? "Moves " + state.movesLeft : "";
            }
            if (briefingStars != null)
                briefingStars.text = TotalStars().ToString();
        }

        void LayoutBriefingCard(float left, float width, float boardBottom, float boardSize)
        {
            if (briefing == null)
                return;
            float cardW = Mathf.Min(760f, width * 0.92f);
            float cardH = 480f;
            float x = left + (width - cardW) * 0.5f;
            float y = boardBottom + (boardSize - cardH) * 0.5f;
            Place(briefing, x, y, cardW, cardH);
            var cardImage = briefing.GetComponent<Image>();
            if (cardImage != null && popupCardSprite != null)
                FitSlice(cardImage, 48f, 36f);
            briefing.SetAsLastSibling();
            if (briefingType != null)
            {
                var r = briefingType.rectTransform;
                r.anchorMin = new Vector2(0.08f, 0.68f);
                r.anchorMax = new Vector2(0.92f, 0.92f);
                r.offsetMin = r.offsetMax = Vector2.zero;
            }
            if (briefingBody != null)
            {
                var r = briefingBody.rectTransform;
                r.anchorMin = new Vector2(0.08f, 0.48f);
                r.anchorMax = new Vector2(0.92f, 0.68f);
                r.offsetMin = r.offsetMax = Vector2.zero;
            }
            var chip = briefing.Find("MovesChip") as RectTransform;
            if (chip != null)
            {
                chip.anchorMin = new Vector2(0.18f, 0.18f);
                chip.anchorMax = new Vector2(0.82f, 0.42f);
                chip.offsetMin = chip.offsetMax = Vector2.zero;
                var chipImage = chip.GetComponent<Image>();
                if (chipImage != null && yellowButtonSprite != null)
                    FitSlice(chipImage, 42f, 36f);
            }
            var star = briefing.Find("Star") as RectTransform;
            if (star != null)
            {
                star.anchorMin = star.anchorMax = new Vector2(0.5f, 0.1f);
                star.pivot = new Vector2(1f, 0.5f);
                star.sizeDelta = new Vector2(40f, 40f);
                star.anchoredPosition = new Vector2(-8f, 0f);
            }
            if (briefingStars != null)
            {
                var r = briefingStars.rectTransform;
                r.anchorMin = r.anchorMax = new Vector2(0.5f, 0.1f);
                r.pivot = new Vector2(0f, 0.5f);
                r.sizeDelta = new Vector2(80f, 40f);
                r.anchoredPosition = new Vector2(8f, 0f);
            }
        }

        IEnumerator PlayLevelBriefing()
        {
            if (briefing == null)
            {
                hudLanded = true;
                yield break;
            }
            briefingSkip = false;
            briefingDocked = true;
            hudLanded = false;
            FillBriefing();
            briefing.gameObject.SetActive(true);
            if (briefingGroup != null)
                briefingGroup.alpha = 1f;
            briefing.localScale = Vector3.one * 0.82f;
            Layout();
            briefing.SetAsLastSibling();
            yield return Motion.PopIn(briefing, 0.3f);
            float wait = 0f;
            while (wait < 2.1f && !briefingSkip)
            {
                wait += Time.unscaledDeltaTime;
                yield return null;
            }
            briefingDocked = false;
            hudLanded = true;
            Layout();
            if (movesPill != null)
            {
                Vector2 from = briefing.anchoredPosition;
                Vector2 fromSize = briefing.sizeDelta;
                Vector2 to = movesPill.anchoredPosition;
                Vector2 toSize = movesPill.sizeDelta;
                briefing.pivot = movesPill.pivot;
                briefing.anchorMin = movesPill.anchorMin;
                briefing.anchorMax = movesPill.anchorMax;
                briefing.anchoredPosition = from;
                briefing.sizeDelta = fromSize;
                float t = 0f;
                while (t < 1f)
                {
                    t += Time.unscaledDeltaTime / 0.48f;
                    float u = Motion.OutCubic(Mathf.Clamp01(t));
                    briefing.anchoredPosition = Vector2.LerpUnclamped(from, to, u);
                    briefing.sizeDelta = Vector2.LerpUnclamped(fromSize, toSize, u);
                    briefing.localScale = Vector3.LerpUnclamped(Vector3.one, Vector3.one * 0.9f, u);
                    if (briefingGroup != null)
                        briefingGroup.alpha = 1f - 0.6f * u;
                    yield return null;
                }
            }
            briefing.gameObject.SetActive(false);
            briefing.localScale = Vector3.one;
            if (briefingGroup != null)
                briefingGroup.alpha = 1f;
            briefingDocked = true;
            hudLanded = true;
            if (movesPill != null)
            {
                movesPill.gameObject.SetActive(true);
                StartCoroutine(Motion.Punch(movesPill, 1.1f, 0.18f));
            }
            Layout();
        }

        void ShowTagBanner()
        {
            if (bannerRoutine != null)
            {
                StopCoroutine(bannerRoutine);
                bannerRoutine = null;
            }
            var lvl = state.level;
            bool hard = lvl != null && lvl.tag == "hard";
            bool boss = lvl != null && lvl.tag == "boss";
            if (banner == null || (!hard && !boss))
            {
                if (banner != null)
                    banner.gameObject.SetActive(false);
                return;
            }
            bannerLabel.text = boss ? "Boss Level!" : "Hard Level!";
            bannerImage.color = boss ? Hex("C62828") : Hex("E65100");
            banner.gameObject.SetActive(true);
            if (bannerGroup != null)
                bannerGroup.alpha = 1f;
            bannerRoutine = StartCoroutine(HideBanner());
        }

        IEnumerator HideBanner()
        {
            yield return Motion.Wait(1.6f);
            if (bannerGroup != null)
                yield return Motion.Alpha(bannerGroup, 0f, 0.24f);
            if (banner != null)
                banner.gameObject.SetActive(false);
            bannerRoutine = null;
        }

        void ShowOutOfMoves()
        {
            ShowFail("Out of moves!", "Retry this level, or go back to the start.");
        }

        void ShowTimeUp()
        {
            if (timeFailed)
                return;
            timeFailed = true;
            ShowFail("Time's up!", "Retry this level, or go back to the start.");
        }

        void ShowFail(string title, string body)
        {
            if (movesPanel == null || celebrating)
                return;
            Haptics.Miss();
            if (messageText != null)
                messageText.text = title;
            if (movesTitle != null)
                movesTitle.text = title;
            if (movesBodyLabel != null)
                movesBodyLabel.text = body;
            NoteLevelFail();
            if (keepGoingButton != null)
                keepGoingButton.gameObject.SetActive(CanOfferMercy());
            ShowOverlay(movesPanel);
            Layout();
        }

        int CurrentGlobal()
        {
            return state != null && state.level != null && state.level.global > 0 ? state.level.global : levelIndex + 1;
        }

        void NoteLevelFail()
        {
            int global = CurrentGlobal();
            int n = PlayerPrefs.GetInt(GameState.FailCountKey(global), 0) + 1;
            PlayerPrefs.SetInt(GameState.FailCountKey(global), n);
            PlayerPrefs.Save();
        }

        bool CanOfferMercy()
        {
            int global = CurrentGlobal();
            return PlayerPrefs.GetInt(GameState.FailCountKey(global), 0) >= 3
                && PlayerPrefs.GetInt(GameState.MercyUsedKey(global), 0) == 0;
        }

        void ClearLevelFailStreak()
        {
            int global = CurrentGlobal();
            PlayerPrefs.SetInt(GameState.FailCountKey(global), 0);
            PlayerPrefs.SetInt(GameState.MercyUsedKey(global), 0);
            PlayerPrefs.Save();
        }

        void OnKeepGoing()
        {
            int global = CurrentGlobal();
            PlayerPrefs.SetInt(GameState.MercyUsedKey(global), 1);
            PlayerPrefs.Save();
            if (SpeedClockActive() || (state != null && state.HasSpeed))
            {
                timeLeft += 30f;
                timeFailed = false;
            }
            else
                state.GrantMercyMoves(3);
            if (keepGoingButton != null)
                keepGoingButton.gameObject.SetActive(false);
            HideOverlayNow();
            if (messageText != null)
                messageText.text = "Keep going";
            UpdateMovesHud();
        }

        int TotalStars()
        {
            int total = 0;
            for (int i = 0; i < allLevels.Count; i++)
            {
                int global = allLevels[i] != null && allLevels[i].global > 0 ? allLevels[i].global : i + 1;
                total += PlayerPrefs.GetInt("GridAura.BestStars." + global, 0);
            }
            return total;
        }

        void RecordStars(int earned)
        {
            int global = state.level != null && state.level.global > 0 ? state.level.global : levelIndex + 1;
            string key = "GridAura.BestStars." + global;
            int prev = PlayerPrefs.GetInt(key, 0);
            if (earned > prev)
                PlayerPrefs.SetInt(key, earned);
            PlayerPrefs.Save();
        }

        IEnumerator WiggleWrongPieces()
        {
            if (state.level == null || state.level.wrongPieces == null)
                yield break;
            yield return Motion.Wait(0.12f);
            foreach (var id in state.level.wrongPieces)
            {
                var cell = placedLayer.Find(id) as RectTransform;
                if (cell != null)
                    StartCoroutine(Motion.Punch(cell, 1.1f, 0.28f));
            }
        }

        static Color SpotTint(string color)
        {
            switch (color)
            {
                case "red": return Hex("E23B3B");
                case "blue": return Hex("2F6BFF");
                case "yellow": return Hex("F5C400");
                case "violet":
                case "white": return Hex("8B5CFF");
                default: return Color.white;
            }
        }

        void AddSpotDot(RectTransform cell, string color, bool ring)
        {
            var dot = MakeRect(ring ? "SpotRing" : "Spot", cell);
            dot.anchorMin = dot.anchorMax = new Vector2(0.5f, 0.5f);
            dot.pivot = new Vector2(0.5f, 0.5f);
            float size = ring ? 0.42f : 0.34f;
            dot.anchorMin = new Vector2(0.5f - size * 0.5f, 0.5f - size * 0.5f);
            dot.anchorMax = new Vector2(0.5f + size * 0.5f, 0.5f + size * 0.5f);
            dot.offsetMin = dot.offsetMax = Vector2.zero;
            var image = dot.gameObject.AddComponent<Image>();
            image.sprite = settingsToggleKnobSprite != null ? settingsToggleKnobSprite : toggleKnobSprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = ring ? Color.white : SpotTint(color);
        }

        void AddCoveredSpotRings(RectTransform cell, PlacedPiece piece)
        {
            if (!state.IsSpots || piece == null || state.level == null || state.level.spots == null)
                return;
            string mine = state.PieceColorName(piece.id);
            foreach (var spot in state.level.spots)
            {
                if (spot == null)
                    continue;
                if (spot.row < piece.row || spot.row >= piece.row + piece.h || spot.col < piece.col || spot.col >= piece.col + piece.w)
                    continue;
                if (!string.Equals(spot.color, mine, System.StringComparison.OrdinalIgnoreCase))
                    continue;
                var ring = MakeRect("SpotRing", cell);
                float x = (spot.col - piece.col + 0.5f) / piece.w;
                float y = 1f - (spot.row - piece.row + 0.5f) / piece.h;
                ring.anchorMin = ring.anchorMax = new Vector2(x, y);
                ring.pivot = new Vector2(0.5f, 0.5f);
                ring.sizeDelta = new Vector2(18f, 18f);
                var image = ring.gameObject.AddComponent<Image>();
                image.sprite = settingsToggleKnobSprite != null ? settingsToggleKnobSprite : toggleKnobSprite;
                image.preserveAspect = true;
                image.raycastTarget = false;
                image.color = new Color(1f, 1f, 1f, 0.9f);
            }
        }

        void AddFrozenBadge(RectTransform cell)
        {
            if (cell == null || cell.Find("Frozen") != null)
                return;
            var badge = MakeRect("Frozen", cell);
            badge.anchorMin = badge.anchorMax = new Vector2(0f, 1f);
            badge.pivot = new Vector2(0f, 1f);
            badge.anchoredPosition = new Vector2(8f, -8f);
            badge.sizeDelta = new Vector2(28f, 28f);
            var image = badge.gameObject.AddComponent<Image>();
            image.sprite = settingsToggleKnobSprite != null ? settingsToggleKnobSprite : toggleKnobSprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = Hex("B8E8FF");
            var flake = MakeText(badge, "*", 22, Hex("1A4A66"), fontBlack);
            flake.alignment = TextAlignmentOptions.Center;
            Stretch(flake.rectTransform);
            flake.raycastTarget = false;
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
            if (boardIntroActive || !state.IsTutorial || state.IsComplete() || dragging)
                return;
            aidLayer.SetAsLastSibling();
            bool guided = state.GuidedDrag;
            if (guided && !dragging)
                fingerDragStopped = false;
            if (guided)
            {
                DrawTargetOutlines(true);
                HighlightGuidePiece();
            }
            bool teachRotate = InRotateTeachWindow();
            if (teachRotate)
                DrawRotateArrows();
            if ((guided || state.HasAid("fingerDrag")) && !fingerDragStopped && !dragging)
                aidRoutine = StartCoroutine(FingerDragLoop());
            else if (teachRotate && state.level != null && state.level.id == firstRotateId && !fingerTapStopped)
                aidRoutine = StartCoroutine(FingerTapLoop());
        }

        bool InRotateTeachWindow()
        {
            if (!state.IsTutorial || state.level == null || dragging)
                return false;
            return state.level.id >= firstRotateId && state.level.id <= firstRotateId + 3;
        }

        void MarkRotateTaught()
        {
            fingerTapStopped = true;
        }

        void PlaceRotateNearDrag()
        {
            if (!dragging || rotateButton == null || !rotateButton.gameObject.activeSelf)
                return;
            float w = 280f;
            float h = 92f;
            if (boardFrame != null)
            {
                rotateButton.anchorMin = rotateButton.anchorMax = new Vector2(0f, 1f);
                rotateButton.pivot = new Vector2(0.5f, 1f);
                rotateButton.sizeDelta = new Vector2(w, h);
                rotateButton.anchoredPosition = new Vector2(
                    boardFrame.anchoredPosition.x + boardFrame.sizeDelta.x * 0.5f,
                    boardFrame.anchoredPosition.y - 10f);
            }
            rotateButton.SetAsLastSibling();
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

        void UpdateDragRotateButton(bool overBoard)
        {
            if (rotateButton == null)
                return;
            bool show = dragging && active != null && overBoard && state.CanRotate(active.pieceId);
            rotateButton.gameObject.SetActive(show);
            if (show)
                PlaceRotateNearDrag();
        }

        bool PointerOverTray()
        {
            var tray = Find("Tray");
            return tray != null && RectTransformUtility.RectangleContainsScreenPoint(tray, lastPointer, UiCamera());
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
                if (item.id != null && state.CanRotate(item.id) && !state.IsPlaced(item.id))
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
            RectTransformUtility.ScreenPointToLocalPointInRectangle(space, RectTransformUtility.WorldToScreenPoint(UiCamera(), world), UiCamera(), out var local);
            return local;
        }

        Vector2 GridRootPoint(int col, int row, int w, int h)
        {
            var local = new Vector2((col + w * 0.5f) * pitch, -((row + h * 0.5f) * pitch));
            Vector3 world = grid.TransformPoint(local);
            var space = aidLayer != null ? aidLayer : root;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(space, RectTransformUtility.WorldToScreenPoint(UiCamera(), world), UiCamera(), out var point);
            return point;
        }

        Vector2 PointInRoot(RectTransform rect)
        {
            if (rect == null)
                return Vector2.zero;
            Vector3 world = rect.TransformPoint(rect.rect.center);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, RectTransformUtility.WorldToScreenPoint(UiCamera(), world), UiCamera(), out var local);
            return local;
        }

        Vector2 GridPointInRoot(int col, int row, int w, int h)
        {
            var local = new Vector2((col + w * 0.5f) * pitch, -((row + h * 0.5f) * pitch));
            Vector3 world = grid.TransformPoint(local);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(root, RectTransformUtility.WorldToScreenPoint(UiCamera(), world), UiCamera(), out var point);
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
            int[] order = { 1, 0, 2 };
            for (int i = 0; i < winStarRoots.Length; i++)
            {
                if (winStarRoots[i] == null)
                    continue;
                winStarRoots[i].gameObject.SetActive(false);
                winStarRoots[i].localScale = Vector3.zero;
                winStarRoots[i].localRotation = Quaternion.identity;
                if (winStars[i] != null)
                    winStars[i].color = Color.white;
                if (winStarGlows[i] != null)
                    winStarGlows[i].color = Color.white;
            }
            yield return Motion.Wait(0.2f);
            int count = Mathf.Clamp(winEarnedStars, 0, order.Length);
            for (int n = 0; n < count; n++)
            {
                int i = order[n];
                var star = winStarRoots[i];
                if (star == null)
                    continue;
                star.gameObject.SetActive(true);
                yield return Motion.FallFromCamera(star, 0.38f, 620f, i == 1 ? 3.6f : 3.1f);
                Haptics.Impact();
                PlayToonFx(starBurstFx, star, 0.42f, true, 1.4f, false, 0.38f, 0.62f);
                PlayToonFx(starPoofFx, star, 0.32f, true, 1.2f, false, 0.28f, 0.45f);
                if (winPanel != null)
                    StartCoroutine(Motion.Punch(winPanel, 1.03f, 0.14f));
                yield return Motion.ImpactSquash(star);
                yield return Motion.Wait(0.1f);
            }
            starRoutine = null;
        }

        IEnumerator PlayLandImpact(RectTransform landed, string cheer = null)
        {
            if (landed == null)
                yield break;
            Haptics.Place();
            PlayToonFx(sparkImpactFx, landed, ToonScale(landed, 1.35f));
            if (!string.IsNullOrEmpty(cheer))
                StartCoroutine(FitTextRoutine(landed, cheer));
            if (boardFrame != null)
                StartCoroutine(Motion.Punch(boardFrame, 1.03f, 0.14f));
            yield return Motion.ImpactSquash(landed);
        }

        float ToonScale(RectTransform target, float boost)
        {
            if (target == null)
                return boost;
            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);
            float span = Mathf.Max(Vector3.Distance(corners[0], corners[3]), Vector3.Distance(corners[0], corners[1]));
            return Mathf.Clamp(span * boost, 0.7f, 2.6f);
        }

        void PlayToonFx(GameObject prefab, RectTransform target, float scale, bool useGameStar = false, float life = 2.5f, bool fromTop = false, float sizeMul = 1f, float alphaMul = 1f)
        {
            if (prefab == null || target == null)
                return;
            Vector3[] corners = new Vector3[4];
            target.GetWorldCorners(corners);
            Vector3 pos = fromTop ? (corners[1] + corners[2]) * 0.5f : (corners[0] + corners[2]) * 0.5f;
            var cam = canvas != null && canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            if (fromTop)
                pos += Vector3.up * 0.55f;
            else if (cam != null)
                pos -= cam.transform.forward * 0.25f;
            var rotation = fromTop ? Quaternion.identity : prefab.transform.rotation;
            if (fxRoot == null)
                fxRoot = new GameObject("LiveFx").transform;
            var fx = Instantiate(prefab, pos, rotation, fxRoot);
            fx.transform.localScale = Vector3.one * Mathf.Max(0.2f, scale);
            var systems = fx.GetComponentsInChildren<ParticleSystem>(true);
            Texture starTex = useGameStar && starSprite != null ? starSprite.texture : null;
            for (int i = 0; i < systems.Length; i++)
            {
                var main = systems[i].main;
                main.useUnscaledTime = true;
                if (sizeMul != 1f)
                    main.startSizeMultiplier *= sizeMul;
                if (alphaMul < 0.99f)
                {
                    var start = main.startColor;
                    var color = start.color;
                    color.a *= alphaMul;
                    start.color = color;
                    main.startColor = start;
                }
                var renderer = systems[i].GetComponent<ParticleSystemRenderer>();
                if (renderer == null)
                    continue;
                renderer.sortingOrder = fromTop ? 40 : 20;
                if (starTex == null)
                    continue;
                var mats = renderer.materials;
                for (int m = 0; m < mats.Length; m++)
                {
                    if (mats[m] == null)
                        continue;
                    mats[m].mainTexture = starTex;
                    if (mats[m].HasProperty("_BaseMap"))
                        mats[m].SetTexture("_BaseMap", starTex);
                    if (mats[m].HasProperty("_MainTex"))
                        mats[m].SetTexture("_MainTex", starTex);
                }
                renderer.materials = mats;
            }
            Destroy(fx, life);
        }

        void StopTransientFx()
        {
            Haptics.Stop();
            StopConfetti();
            if (starRoutine != null)
            {
                StopCoroutine(starRoutine);
                starRoutine = null;
            }
            if (fxRoot != null)
            {
                Destroy(fxRoot.gameObject);
                fxRoot = null;
            }
            if (cheerRoot != null)
            {
                for (int i = cheerRoot.childCount - 1; i >= 0; i--)
                    Destroy(cheerRoot.GetChild(i).gameObject);
            }
        }

        void StopConfetti()
        {
            Haptics.Stop();
            if (confettiRoutine != null)
            {
                StopCoroutine(confettiRoutine);
                confettiRoutine = null;
            }
            if (confettiLive != null)
            {
                Destroy(confettiLive);
                confettiLive = null;
            }
        }

        IEnumerator PlayScreenConfetti()
        {
            if (confettiLive != null)
            {
                Destroy(confettiLive);
                confettiLive = null;
            }
            var cam = canvas != null && canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            if (cam == null || confettiShowerFx == null)
                yield break;
            float worldH = cam.orthographicSize * 2f;
            float worldW = worldH * cam.aspect;
            Vector3 pos = new Vector3(cam.transform.position.x, cam.transform.position.y + cam.orthographicSize * 1.08f, -0.35f);
            confettiLive = new GameObject("Confetti");
            var shower = Instantiate(confettiShowerFx, pos, Quaternion.identity, confettiLive.transform);
            shower.transform.localScale = Vector3.one;
            var systems = shower.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < systems.Length; i++)
            {
                var main = systems[i].main;
                main.loop = true;
                main.useUnscaledTime = true;
                main.simulationSpace = ParticleSystemSimulationSpace.World;
                main.cullingMode = ParticleSystemCullingMode.AlwaysSimulate;
                main.scalingMode = ParticleSystemScalingMode.Hierarchy;
                main.startLifetime = new ParticleSystem.MinMaxCurve(3.4f, 5.8f);
                main.startSpeed = new ParticleSystem.MinMaxCurve(1.2f, 2.6f);
                main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.16f);
                main.gravityModifier = 1.55f;
                main.maxParticles = 1200;
                var shape = systems[i].shape;
                shape.enabled = true;
                shape.shapeType = ParticleSystemShapeType.Box;
                shape.scale = new Vector3(worldW * 1.35f, 0.35f, 0.35f);
                var emission = systems[i].emission;
                emission.rateOverTimeMultiplier *= Mathf.Max(1.2f, worldW * 0.45f);
                var renderer = systems[i].GetComponent<ParticleSystemRenderer>();
                if (renderer != null)
                    renderer.sortingOrder = 60;
                systems[i].Play(true);
            }
            float fall = 7.5f;
            float t = 0f;
            while (t < fall && confettiLive != null)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }
            if (confettiLive == null)
                yield break;
            systems = confettiLive.GetComponentsInChildren<ParticleSystem>(true);
            for (int i = 0; i < systems.Length; i++)
                systems[i].Stop(true, ParticleSystemStopBehavior.StopEmitting);
            var renderers = confettiLive.GetComponentsInChildren<ParticleSystemRenderer>(true);
            var mats = new List<Material>();
            var bases = new List<Color>();
            for (int r = 0; r < renderers.Length; r++)
            {
                var used = renderers[r].materials;
                for (int m = 0; m < used.Length; m++)
                {
                    if (used[m] == null)
                        continue;
                    mats.Add(used[m]);
                    if (used[m].HasProperty("_BaseColor"))
                        bases.Add(used[m].GetColor("_BaseColor"));
                    else if (used[m].HasProperty("_Color"))
                        bases.Add(used[m].GetColor("_Color"));
                    else
                        bases.Add(used[m].color);
                }
            }
            t = 0f;
            float fade = 2f;
            while (t < fade && confettiLive != null)
            {
                t += Time.unscaledDeltaTime;
                float a = 1f - Motion.OutCubic(t / fade);
                for (int i = 0; i < mats.Count; i++)
                {
                    if (mats[i] == null)
                        continue;
                    var c = bases[i];
                    c.a *= a;
                    if (mats[i].HasProperty("_BaseColor"))
                        mats[i].SetColor("_BaseColor", c);
                    if (mats[i].HasProperty("_Color"))
                        mats[i].SetColor("_Color", c);
                    mats[i].color = c;
                }
                yield return null;
            }
            Haptics.Stop();
            if (confettiLive != null)
            {
                Destroy(confettiLive);
                confettiLive = null;
            }
            confettiRoutine = null;
        }

        IEnumerator FitTextRoutine(RectTransform at, string phrase)
        {
            if (at == null || string.IsNullOrEmpty(phrase) || cheerRoot == null)
                yield break;
            var label = MakeText(cheerRoot, phrase, 88, Hex("FFD21F"), fontBlack);
            var rect = label.rectTransform;
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(780f, 150f);
            rect.anchoredPosition = CheerPoint(at);
            rect.localScale = Vector3.one * 0.4f;
            label.outlineWidth = 0.32f;
            label.outlineColor = new Color(0.16f, 0.07f, 0f, 1f);
            var group = label.gameObject.AddComponent<CanvasGroup>();
            group.blocksRaycasts = false;
            Vector2 from = rect.anchoredPosition;
            Vector2 to = from + new Vector2(0f, 56f);
            float t = 0f;
            while (t < 1f && rect != null)
            {
                t += Time.unscaledDeltaTime / 0.7f;
                float u = Mathf.Clamp01(t);
                float pop = u < 0.28f ? Motion.OutBack(u / 0.28f) : 1f;
                rect.localScale = Vector3.one * Mathf.LerpUnclamped(0.4f, 1f, pop);
                rect.anchoredPosition = Vector2.LerpUnclamped(from, to, Motion.OutCubic(u));
                group.alpha = u < 0.7f ? 1f : 1f - (u - 0.7f) / 0.3f;
                yield return null;
            }
            if (label != null)
                Destroy(label.gameObject);
        }

        Vector2 CheerPoint(RectTransform at)
        {
            Vector3[] corners = new Vector3[4];
            at.GetWorldCorners(corners);
            Vector3 piece = (corners[0] + corners[2]) * 0.5f;
            Vector3 anchor = piece;
            if (boardFrame != null)
            {
                boardFrame.GetWorldCorners(corners);
                Vector3 top = (corners[1] + corners[2]) * 0.5f;
                anchor = new Vector3(piece.x, top.y, piece.z);
            }
            Vector2 screen = RectTransformUtility.WorldToScreenPoint(UiCamera(), anchor);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(cheerRoot, screen, null, out var local);
            return local + new Vector2(0f, 70f);
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
            if (!state.IsTutorial)
            {
                if (state.HasMoveLimit)
                    yield return PlayLevelBriefing();
                boardIntroActive = false;
                motionBusy = false;
                introRoutine = null;
                UpdateHintButton();
                DrawTutorialAids();
                if (state.IsRescue && state.level != null && state.level.intro)
                    StartCoroutine(WiggleWrongPieces());
                yield break;
            }
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
                bool sat = false;
                for (int r = 0; r < GameState.Size; r++)
                {
                    int c = diagonal - r;
                    if (c < 0 || c >= GameState.Size)
                        continue;
                    var empty = grid.Find("Empty_" + c + "_" + r) as RectTransform;
                    if (empty != null)
                    {
                        StartCoroutine(Motion.Sit(empty, 0.32f, 16f, 5f));
                        sat = true;
                    }
                }
                if (sat)
                    Haptics.Tick();
                yield return Motion.Wait(0.028f);
            }
            yield return Motion.Wait(0.08f);
            foreach (Transform child in placedLayer)
            {
                if (child is RectTransform piece)
                {
                    StartCoroutine(Motion.Sit(piece, 0.3f, 14f, 4f));
                    Haptics.Tick();
                }
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
            UpdateHintButton();
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
            pulseRoutine = StartCoroutine(PulseGuideRoutine());
        }

        IEnumerator PulseGuideRoutine()
        {
            if (state.level != null && state.level.id == 4)
                yield return PulseOnce(Find("Undo"));
            if (HintUnlocked && state.level != null && (state.level.id == 4 || state.level.id == 5))
                yield return PulseOnce(Find("Hint"));
            pulseRoutine = null;
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
                case "green":
                    face = Hex("2E9B57"); highlight = Hex("5ED18A"); shade = Hex("1B6B3A"); break;
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
            var overlayCanvas = overlay.gameObject.AddComponent<Canvas>();
            overlayCanvas.overrideSorting = true;
            overlayCanvas.sortingOrder = 200;
            overlay.gameObject.AddComponent<GraphicRaycaster>();
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
            hintsUsedText = MakeText(winPanel, "Hints used: 0", 42, new Color(1f, 1f, 1f, 0.85f), fontBold);
            Pin(hintsUsedText.rectTransform, 0.1f, 0.37f, 0.9f, 0.44f);
            starsTotalText = MakeText(winPanel, "", 36, Hex("FFD21F"), fontBold);
            Pin(starsTotalText.rectTransform, 0.1f, 0.30f, 0.9f, 0.37f);
            starsTotalText.gameObject.SetActive(false);
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
            BindButton(more.gameObject, OpenShopFromPlay);

            BuildDebugPanel();
            HideOverlayNow();
        }

        void BuildDebugPanel()
        {
            debugPanel = MakeCard("Debug");
            var title = MakeText(debugPanel, "Debug", 52, Color.white, fontBlack);
            Pin(title.rectTransform, 0.08f, 0.82f, 0.92f, 0.96f);
            var close = MakeRect("Close", debugPanel);
            close.anchorMin = close.anchorMax = new Vector2(1f, 1f);
            close.pivot = new Vector2(1f, 1f);
            close.anchoredPosition = new Vector2(-18f, -18f);
            close.sizeDelta = new Vector2(64f, 64f);
            var closeImage = close.gameObject.AddComponent<Image>();
            closeImage.sprite = closeSprite;
            closeImage.preserveAspect = true;
            BindButton(close.gameObject, HideOverlay);

            var label = MakeText(debugPanel, "Jump to level", 36, new Color(1f, 1f, 1f, 0.9f), fontBold);
            Pin(label.rectTransform, 0.1f, 0.66f, 0.9f, 0.78f);

            var field = MakeRect("LevelField", debugPanel);
            Pin(field, 0.18f, 0.48f, 0.82f, 0.64f);
            var fieldBg = field.gameObject.AddComponent<Image>();
            fieldBg.color = Hex("2A2F38");
            fieldBg.raycastTarget = true;
            var textArea = MakeRect("Text Area", field);
            Stretch(textArea, 12f);
            textArea.gameObject.AddComponent<RectMask2D>();
            var inputText = MakeText(textArea, "", 48, Color.white, fontBlack);
            inputText.raycastTarget = true;
            inputText.alignment = TextAlignmentOptions.Center;
            var placeholder = MakeText(textArea, "e.g. 23", 40, new Color(1f, 1f, 1f, 0.35f), fontBold);
            placeholder.alignment = TextAlignmentOptions.Center;
            debugLevelInput = field.gameObject.AddComponent<TMP_InputField>();
            debugLevelInput.textViewport = textArea;
            debugLevelInput.textComponent = inputText;
            debugLevelInput.placeholder = placeholder;
            debugLevelInput.contentType = TMP_InputField.ContentType.IntegerNumber;
            debugLevelInput.characterLimit = 3;
            debugLevelInput.caretColor = Color.white;
            debugLevelInput.selectionColor = new Color(0.2f, 0.5f, 1f, 0.35f);
            debugLevelInput.lineType = TMP_InputField.LineType.SingleLine;

            debugStatus = MakeText(debugPanel, "", 30, new Color(1f, 1f, 1f, 0.7f), fontSemiBold);
            Pin(debugStatus.rectTransform, 0.1f, 0.36f, 0.9f, 0.46f);

            var jump = MakePill(debugPanel, "Jump", yellowButtonSprite, "Jump", Hex("1A1A1A"), out var jumpLabel);
            jumpLabel.font = tmpBlack;
            Pin(jump, 0.1f, 0.12f, 0.9f, 0.32f);
            BindButton(jump.gameObject, JumpDebugLevel);

            debugPanel.gameObject.SetActive(false);
        }

        void BuildMenus()
        {
            menuRoot = MakeRect("Menu", root);
            Stretch(menuRoot);
            var bg = menuRoot.gameObject.AddComponent<Image>();
            if (settingsBgSprite != null)
            {
                bg.sprite = settingsBgSprite;
                bg.type = Image.Type.Simple;
                bg.preserveAspect = false;
                bg.color = Color.white;
            }
            else
                bg.color = Hex("0E1016");
            bg.raycastTarget = true;
            var menuCanvas = menuRoot.gameObject.AddComponent<Canvas>();
            menuCanvas.overrideSorting = true;
            menuCanvas.sortingOrder = 120;
            menuRoot.gameObject.AddComponent<GraphicRaycaster>();

            splashPanel = MakeRect("Splash", menuRoot);
            Stretch(splashPanel);
            var splashTitle = MakeText(splashPanel, "GridAura", 86, Color.white, fontBlack);
            Pin(splashTitle.rectTransform, 0.08f, 0.48f, 0.92f, 0.64f);
            var splashSub = MakeText(splashPanel, "Fill every empty cell", 34, Hex("FFD21F"), fontBold);
            Pin(splashSub.rectTransform, 0.1f, 0.38f, 0.9f, 0.48f);

            homePanel = MakeRect("Home", menuRoot);
            Stretch(homePanel);
            var starChip = MakeRect("StarChip", homePanel);
            var starChipBg = starChip.gameObject.AddComponent<Image>();
            starChipBg.sprite = settingsButtonSprite != null ? settingsButtonSprite : yellowButtonSprite;
            starChipBg.type = Image.Type.Sliced;
            starChipBg.color = Hex("FFD21F");
            var starChipIcon = MakeRect("Icon", starChip);
            Pin(starChipIcon, 0.12f, 0.18f, 0.48f, 0.82f);
            var starChipImage = starChipIcon.gameObject.AddComponent<Image>();
            starChipImage.sprite = starSprite;
            starChipImage.preserveAspect = true;
            starChipImage.raycastTarget = false;
            homeStarLabel = MakeText(starChip, "0", 28, Hex("1A1A1A"), fontBlack);
            homeStarLabel.alignment = TextAlignmentOptions.Center;
            Pin(homeStarLabel.rectTransform, 0.42f, 0.08f, 0.92f, 0.92f);

            var logo = MakeRect("Logo", homePanel);
            BuildHomeLogo(logo);
            var homeTitle = MakeText(homePanel, "GridAura", 82, Color.white, fontBlack);
            homeTitle.outlineWidth = 0.28f;
            homeTitle.outlineColor = Hex("1A1A1A");
            Pin(homeTitle.rectTransform, 0.08f, 0.50f, 0.92f, 0.64f);
            var homeSub = MakeText(homePanel, "Fill the grid", 30, Hex("FFD21F"), fontExtraBold);
            Pin(homeSub.rectTransform, 0.1f, 0.45f, 0.9f, 0.51f);

            var play = MakePill(homePanel, "Play", yellowButtonSprite, "Level 1", Hex("1A1A1A"), out homePlayLabel);
            Pin(play, 0.12f, 0.30f, 0.88f, 0.43f);
            BindButton(play.gameObject, PlayContinue);
            var levels = MakePill(homePanel, "Levels", blueButtonSprite, "Levels", Color.white, out _);
            Pin(levels, 0.12f, 0.17f, 0.88f, 0.29f);
            BindButton(levels.gameObject, OpenLevels);
            homeStatus = MakeText(homePanel, "How to Play", 28, Hex("FFD21F"), fontBold);
            Pin(homeStatus.rectTransform, 0.1f, 0.12f, 0.9f, 0.17f);
            homeStatus.raycastTarget = true;
            BindButton(homeStatus.gameObject, OpenHowTo);

            MakeHomeRound(homePanel, "SettingsBtn", settingsButtonSprite, settingsGearSprite, OpenSettingsFromHome);
            MakeHomeRound(homePanel, "ShopBtn", yellowButtonSprite, iconCartSprite != null ? iconCartSprite : starSprite, OpenShop);
            var soundBtn = MakeHomeRound(homePanel, "SoundBtn", yellowButtonSprite, iconSoundSprite != null ? iconSoundSprite : hintBulbSprite, ToggleSound);
            var soundIconRect = soundBtn.Find("Icon");
            if (soundIconRect != null)
                soundIcon = soundIconRect.GetComponent<Image>();

            levelsPanel = MakeRect("Levels", menuRoot);
            Stretch(levelsPanel);
            MakeMenuHeader(levelsPanel, "Select Level");
            var tabs = MakeRect("Tabs", levelsPanel);
            Pin(tabs, 0.05f, 0.80f, 0.95f, 0.88f);
            levelTabButtons.Clear();
            for (int i = 0; i < LevelTabs.Length; i++)
            {
                int tab = i;
                var btn = MakeRect(LevelTabs[i], tabs);
                btn.anchorMin = new Vector2(i / 4f, 0f);
                btn.anchorMax = new Vector2((i + 1) / 4f, 1f);
                btn.offsetMin = new Vector2(6f, 8f);
                btn.offsetMax = new Vector2(-6f, -8f);
                PaintSliced(btn, popupCardSprite, Hex("2A303C"));
                var label = MakeText(btn, LevelTabs[i], 28, Color.white, fontBlack);
                Stretch(label.rectTransform);
                BindButton(btn.gameObject, () => SelectLevelTab(tab));
                levelTabButtons.Add(btn);
            }
            var viewport = MakeRect("Viewport", levelsPanel);
            Pin(viewport, 0.04f, 0.02f, 0.96f, 0.79f);
            viewport.gameObject.AddComponent<Image>().color = new Color(1f, 1f, 1f, 0.01f);
            viewport.gameObject.AddComponent<RectMask2D>();
            levelsContent = MakeRect("Content", viewport);
            levelsContent.anchorMin = new Vector2(0f, 1f);
            levelsContent.anchorMax = new Vector2(1f, 1f);
            levelsContent.pivot = new Vector2(0.5f, 1f);
            levelsContent.anchoredPosition = Vector2.zero;
            var scroll = viewport.gameObject.AddComponent<ScrollRect>();
            scroll.content = levelsContent;
            scroll.viewport = viewport;
            scroll.horizontal = false;
            scroll.movementType = ScrollRect.MovementType.Elastic;
            scroll.scrollSensitivity = 48f;

            settingsPanel = MakeRect("Settings", menuRoot);
            Stretch(settingsPanel);
            MakeMenuHeader(settingsPanel, "Settings");
            soundToggle = MakeSettingsToggle(settingsPanel, "Sound", 0.755f, 0.835f, iconSoundSprite, ToggleSound);
            musicToggle = MakeSettingsToggle(settingsPanel, "Music", 0.668f, 0.748f, iconMusicSprite, ToggleMusic);
            hapticsToggle = MakeSettingsToggle(settingsPanel, "Haptics", 0.581f, 0.661f, iconHapticsSprite, ToggleHaptics);
            relaxedToggle = MakeSettingsToggle(settingsPanel, "Relaxed Timers", 0.494f, 0.574f, iconThemeSprite, ToggleRelaxedTimers);
            MakeSettingsChevron(settingsPanel, "Restore", 0.367f, 0.447f, iconCartSprite, RestorePurchases);
            var restoreLabel = settingsPanel.Find("Restore") != null ? settingsPanel.Find("Restore").Find("Caption") : null;
            if (restoreLabel != null)
                restoreLabel.GetComponent<TextMeshProUGUI>().text = "Restore Purchases";
            MakeSettingsChevron(settingsPanel, "Privacy", 0.280f, 0.360f, iconShieldSprite, OpenPrivacy);
            var privacyLabel = settingsPanel.Find("Privacy") != null ? settingsPanel.Find("Privacy").Find("Caption") : null;
            if (privacyLabel != null)
                privacyLabel.GetComponent<TextMeshProUGUI>().text = "Privacy Policy";
            MakeSettingsChevron(settingsPanel, "Rate", 0.193f, 0.273f, iconStarSprite, RateUs);
            var rateLabel = settingsPanel.Find("Rate") != null ? settingsPanel.Find("Rate").Find("Caption") : null;
            if (rateLabel != null)
                rateLabel.GetComponent<TextMeshProUGUI>().text = "Rate Us";
            var version = MakeText(settingsPanel, "v1.0", 26, new Color(1f, 1f, 1f, 0.35f), fontBold);
            Pin(version.rectTransform, 0.2f, 0.04f, 0.8f, 0.10f);
            version.raycastTarget = true;
            BindButton(version.gameObject, TapVersion);

            BuildShopPanel();
            BuildHowToPanel();
            BuildThemePanel();
            BuildPrivacyPanel();
            menuRoot.gameObject.SetActive(false);
        }

        void BuildHomeLogo(RectTransform logo)
        {
            string[] cells =
            {
                "yellow", "blue", "empty", "red",
                "blue", "black", "yellow", "blue",
                "red", "empty", "black", "yellow",
                "blue", "red", "yellow", "black"
            };
            for (int i = 0; i < 16; i++)
            {
                int col = i % 4;
                int row = i / 4;
                var cell = MakeRect("C" + i, logo);
                cell.anchorMin = new Vector2(col / 4f, 1f - (row + 1) / 4f);
                cell.anchorMax = new Vector2((col + 1) / 4f, 1f - row / 4f);
                cell.offsetMin = new Vector2(6f, 6f);
                cell.offsetMax = new Vector2(-6f, -6f);
                var image = cell.gameObject.AddComponent<Image>();
                image.raycastTarget = false;
                string color = cells[i];
                if (color == "empty")
                {
                    image.sprite = emptySprite;
                    image.color = Color.white;
                }
                else if (color == "black")
                {
                    image.sprite = blackSprite;
                    image.color = Color.white;
                }
                else
                {
                    image.sprite = SpriteFor(color);
                    image.color = PieceTint(color);
                }
                image.type = Image.Type.Sliced;
                if (image.sprite != null)
                    FitSlice(image, 16f, 22f);
            }
        }

        RectTransform MakeHomeRound(Transform parent, string name, Sprite background, Sprite icon, UnityEngine.Events.UnityAction action)
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
                image.color = Hex("FFD21F");
            var iconRect = MakeRect("Icon", rect);
            Pin(iconRect, 0.22f, 0.22f, 0.78f, 0.78f);
            var iconImage = iconRect.gameObject.AddComponent<Image>();
            iconImage.sprite = icon;
            iconImage.preserveAspect = true;
            iconImage.raycastTarget = false;
            BindButton(rect.gameObject, action);
            return rect;
        }

        void LayoutHome()
        {
            if (homePanel == null)
                return;
            var starChip = homePanel.Find("StarChip") as RectTransform;
            if (starChip != null)
            {
                starChip.anchorMin = starChip.anchorMax = new Vector2(1f, 1f);
                starChip.pivot = new Vector2(1f, 1f);
                starChip.anchoredPosition = new Vector2(-36f, -36f);
                starChip.sizeDelta = new Vector2(132f, 72f);
                FitSlice(starChip.GetComponent<Image>(), 36f, 32f);
            }
            var logo = homePanel.Find("Logo") as RectTransform;
            if (logo != null)
                Pin(logo, 0.18f, 0.50f, 0.82f, 0.88f);
            float round = 108f;
            float gap = 48f;
            float total = round * 3f + gap * 2f;
            var settings = homePanel.Find("SettingsBtn") as RectTransform;
            var shop = homePanel.Find("ShopBtn") as RectTransform;
            var sound = homePanel.Find("SoundBtn") as RectTransform;
            LayoutHomeRound(settings, -total * 0.5f + round * 0.5f, round);
            LayoutHomeRound(shop, 0f, round);
            LayoutHomeRound(sound, total * 0.5f - round * 0.5f, round);
        }

        void LayoutHomeRound(RectTransform button, float x, float size)
        {
            if (button == null)
                return;
            button.anchorMin = button.anchorMax = new Vector2(0.5f, 0f);
            button.pivot = new Vector2(0.5f, 0f);
            button.anchoredPosition = new Vector2(x, 48f);
            button.sizeDelta = new Vector2(size, size);
            FitSlice(button.GetComponent<Image>(), 36f, size * 0.42f);
        }

        void SelectLevelTab(int tab)
        {
            levelTab = Mathf.Clamp(tab, 0, 3);
            RefreshLevelSelect();
        }

        static int TabStart(int tab)
        {
            if (tab <= 0)
                return 0;
            if (tab == 1)
                return 50;
            if (tab == 2)
                return 100;
            return 150;
        }

        int TabCount(int tab)
        {
            int start = TabStart(tab);
            int end = tab >= 3 ? allLevels.Count : TabStart(tab + 1);
            return Mathf.Max(0, Mathf.Min(end, allLevels.Count) - start);
        }

        void RefreshLevelSelect()
        {
            if (levelsContent == null)
                return;
            for (int t = 0; t < levelTabButtons.Count; t++)
            {
                var image = levelTabButtons[t].GetComponent<Image>();
                if (image != null)
                    image.color = t == levelTab ? Hex("1A1F28") : Hex("3A4150");
            }
            int start = TabStart(levelTab);
            int count = TabCount(levelTab);
            while (levelTiles.Count < count)
            {
                int slot = levelTiles.Count;
                var tile = MakeLevelTile(slot);
                levelTiles.Add(tile);
            }
            for (int i = 0; i < levelTiles.Count; i++)
                levelTiles[i].gameObject.SetActive(i < count);

            int cols = 5;
            float pad = 14f;
            float width = Mathf.Max(200f, levelsContent.rect.width);
            if (width < 50f)
                width = 980f;
            float tileW = (width - pad * (cols + 1)) / cols;
            float tileH = tileW * 1.18f;
            int rows = Mathf.Max(1, Mathf.CeilToInt(count / (float)cols));
            levelsContent.sizeDelta = new Vector2(0f, rows * (tileH + pad) + pad);
            int current = ContinueIndex();
            for (int i = 0; i < count; i++)
            {
                int index = start + i;
                var tile = levelTiles[i];
                int col = i % cols;
                int row = i / cols;
                tile.anchorMin = tile.anchorMax = new Vector2(0f, 1f);
                tile.pivot = new Vector2(0f, 1f);
                tile.sizeDelta = new Vector2(tileW, tileH);
                tile.anchoredPosition = new Vector2(pad + col * (tileW + pad), -(pad + row * (tileH + pad)));
                bool open = LevelUnlocked(index);
                int stars = StarsAt(index);
                bool here = open && index == current && stars == 0;
                var face = tile.Find("Face") as RectTransform;
                var glow = tile.Find("Glow") as RectTransform;
                var num = tile.Find("Num") != null ? tile.Find("Num").GetComponent<TextMeshProUGUI>() : null;
                var lockIcon = tile.Find("Lock") as RectTransform;
                if (glow != null)
                    glow.gameObject.SetActive(here);
                var faceImage = face != null ? face.GetComponent<Image>() : tile.GetComponent<Image>();
                if (faceImage != null)
                {
                    if (!open)
                        faceImage.color = Hex("2A2F38");
                    else if (here)
                        faceImage.color = Hex("1C2028");
                    else
                        faceImage.color = LevelCandy[col];
                }
                if (num != null)
                {
                    num.text = GlobalOf(index).ToString();
                    num.color = Color.white;
                    num.gameObject.SetActive(open);
                }
                if (lockIcon != null)
                    lockIcon.gameObject.SetActive(!open);
                for (int s = 0; s < 3; s++)
                {
                    var star = tile.Find("Star" + s) as RectTransform;
                    if (star == null)
                        continue;
                    star.gameObject.SetActive(open && stars > 0);
                    var starImage = star.GetComponent<Image>();
                    if (starImage != null)
                        starImage.color = s < stars ? Hex("FFD21F") : new Color(1f, 1f, 1f, 0.18f);
                }
                var press = tile.GetComponent<TapPress>();
                if (press != null)
                {
                    int playIndex = index;
                    press.click = () => PlayLevelFromSelect(playIndex);
                    press.enabled = open;
                }
            }
        }

        RectTransform MakeLevelTile(int slot)
        {
            var tile = MakeRect("L" + slot, levelsContent);
            var hit = tile.gameObject.AddComponent<Image>();
            hit.color = Color.clear;
            hit.raycastTarget = true;
            var glow = MakeRect("Glow", tile);
            Stretch(glow, -8f);
            PaintSliced(glow, popupCardSprite, Hex("FFD21F"));
            glow.GetComponent<Image>().raycastTarget = false;
            glow.gameObject.SetActive(false);
            var face = MakeRect("Face", tile);
            Stretch(face, 2f);
            PaintSliced(face, popupCardSprite, Hex("2A2F38"));
            face.GetComponent<Image>().raycastTarget = false;
            var num = MakeText(tile, "", 40, Color.white, fontBlack);
            num.gameObject.name = "Num";
            Pin(num.rectTransform, 0.08f, 0.38f, 0.92f, 0.92f);
            MakePadlock(tile);
            for (int s = 0; s < 3; s++)
            {
                var star = MakeRect("Star" + s, tile);
                star.anchorMin = star.anchorMax = new Vector2(0.28f + s * 0.22f, 0.18f);
                star.sizeDelta = new Vector2(28f, 28f);
                star.anchoredPosition = Vector2.zero;
                var image = star.gameObject.AddComponent<Image>();
                image.sprite = starSprite;
                image.preserveAspect = true;
                image.raycastTarget = false;
                image.color = Hex("FFD21F");
            }
            BindButton(tile.gameObject, () => { });
            return tile;
        }

        void LayoutMenus()
        {
            if (menuRoot == null || !menuOpen)
                return;
            Stretch(menuRoot);
            if (homePanel != null && homePanel.gameObject.activeSelf)
                LayoutHome();
            LayoutMenuHeader(levelsPanel);
            LayoutMenuHeader(settingsPanel);
            LayoutMenuHeader(shopPanel);
            LayoutMenuHeader(howToPanel);
            LayoutMenuHeader(themePanel);
            LayoutMenuHeader(privacyPanel);
            LayoutPill(homePanel != null ? homePanel.Find("Play") as RectTransform : null, 128f);
            LayoutPill(homePanel != null ? homePanel.Find("Levels") as RectTransform : null, 118f);
            LayoutPill(howToPanel != null ? howToPanel.Find("GotIt") as RectTransform : null, 110f);
            LayoutPill(shopPanel != null ? shopPanel.Find("Restore") as RectTransform : null, 88f);
            if (levelsPanel != null && levelsPanel.gameObject.activeSelf)
                RefreshLevelSelect();
        }

        void MakeMenuHeader(RectTransform panel, string title)
        {
            var back = MakeRect("Back", panel);
            Pin(back, 0.04f, 0.88f, 0.16f, 0.97f);
            PaintArt(back, settingsBackSprite != null ? settingsBackSprite : blueButtonSprite, true, false);
            var chevron = MakeRect("Arrow", back);
            Pin(chevron, 0.22f, 0.22f, 0.78f, 0.78f);
            var arrow = chevron.gameObject.AddComponent<Image>();
            arrow.sprite = iconBackSprite;
            arrow.preserveAspect = true;
            arrow.raycastTarget = false;
            arrow.color = Color.white;
            BindButton(back.gameObject, BackFromMenu);
            var bar = MakeRect("TitleBar", panel);
            Pin(bar, 0.20f, 0.88f, 0.72f, 0.97f);
            PaintArt(bar, settingsTitleSprite != null ? settingsTitleSprite : levelSprite, false, true);
            var label = MakeText(bar, title, 36, Hex("1A1A1A"), fontBlack);
            Stretch(label.rectTransform);
        }

        void LayoutMenuHeader(RectTransform panel)
        {
            if (panel == null || !panel.gameObject.activeSelf)
                return;
            float w = panel.rect.width;
            float h = panel.rect.height;
            if (w < 8f || h < 8f)
                return;
            float pad = Mathf.Max(36f, w * 0.045f);
            float back = Mathf.Clamp(h * 0.075f, 108f, 140f);
            var backRect = panel.Find("Back") as RectTransform;
            if (backRect != null)
            {
                backRect.anchorMin = backRect.anchorMax = new Vector2(0f, 1f);
                backRect.pivot = new Vector2(0f, 1f);
                backRect.anchoredPosition = new Vector2(pad, -pad);
                backRect.sizeDelta = new Vector2(back, back);
            }
            var title = panel.Find("TitleBar") as RectTransform;
            if (title != null)
            {
                float titleH = back * 0.9f;
                title.anchorMin = title.anchorMax = new Vector2(0f, 1f);
                title.pivot = new Vector2(0f, 1f);
                title.anchoredPosition = new Vector2(pad + back + 18f, -pad - (back - titleH) * 0.5f);
                title.sizeDelta = new Vector2(Mathf.Min(560f, w * 0.58f), titleH);
            }
        }

        RectTransform MakeSettingsToggle(RectTransform parent, string name, float yMin, float yMax, Sprite icon, UnityEngine.Events.UnityAction action)
        {
            var row = MakeDarkRow(parent, name, yMin, yMax);
            PlaceRowIcon(row, icon);
            var caption = MakeText(row, name, 36, Color.white, fontExtraBold);
            caption.gameObject.name = "Caption";
            caption.alignment = TextAlignmentOptions.MidlineLeft;
            Pin(caption.rectTransform, 0.18f, 0.1f, 0.66f, 0.9f);
            var toggle = MakeToggle(row);
            BindButton(row.gameObject, action);
            return toggle;
        }

        void MakeSettingsChevron(RectTransform parent, string name, float yMin, float yMax, Sprite icon, UnityEngine.Events.UnityAction action)
        {
            var row = MakeDarkRow(parent, name, yMin, yMax);
            PlaceRowIcon(row, icon);
            var caption = MakeText(row, name, 34, Color.white, fontExtraBold);
            caption.gameObject.name = "Caption";
            caption.alignment = TextAlignmentOptions.MidlineLeft;
            Pin(caption.rectTransform, 0.18f, 0.1f, 0.80f, 0.9f);
            var arrow = MakeRect("Chevron", row);
            Pin(arrow, 0.86f, 0.28f, 0.95f, 0.72f);
            var image = arrow.gameObject.AddComponent<Image>();
            image.sprite = iconChevronSprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = new Color(1f, 1f, 1f, 0.38f);
            BindButton(row.gameObject, action);
        }

        RectTransform MakeDarkRow(Transform parent, string name, float yMin, float yMax)
        {
            var row = MakeRect(name, parent);
            Pin(row, 0.055f, yMin, 0.945f, yMax);
            PaintArt(row, settingsRowSprite != null ? settingsRowSprite : popupCardSprite, true, true);
            if (settingsRowSprite == null)
            {
                var image = row.GetComponent<Image>();
                if (image != null)
                    image.color = Hex("22262F");
            }
            return row;
        }

        void PlaceRowIcon(RectTransform row, Sprite sprite)
        {
            var icon = MakeRect("Icon", row);
            Pin(icon, 0.035f, 0.22f, 0.155f, 0.78f);
            var image = icon.gameObject.AddComponent<Image>();
            image.sprite = sprite;
            image.preserveAspect = true;
            image.raycastTarget = false;
            image.color = Color.white;
        }

        RectTransform MakeToggle(RectTransform row)
        {
            var toggle = MakeRect("Toggle", row);
            toggle.anchorMin = toggle.anchorMax = new Vector2(1f, 0.5f);
            toggle.pivot = new Vector2(1f, 0.5f);
            toggle.sizeDelta = new Vector2(118f, 66f);
            toggle.anchoredPosition = new Vector2(-24f, 0f);
            var track = toggle.gameObject.AddComponent<Image>();
            track.sprite = settingsToggleOnSprite != null ? settingsToggleOnSprite : toggleTrackSprite;
            track.type = Image.Type.Sliced;
            track.preserveAspect = false;
            track.color = Color.white;
            track.raycastTarget = false;
            var knob = MakeRect("Knob", toggle);
            knob.anchorMin = knob.anchorMax = new Vector2(0.5f, 0.5f);
            knob.pivot = new Vector2(0.5f, 0.5f);
            knob.sizeDelta = new Vector2(52f, 52f);
            knob.anchoredPosition = new Vector2(26f, 0f);
            var knobImage = knob.gameObject.AddComponent<Image>();
            knobImage.sprite = settingsToggleKnobSprite != null ? settingsToggleKnobSprite : toggleKnobSprite;
            knobImage.preserveAspect = true;
            knobImage.color = Color.white;
            knobImage.raycastTarget = false;
            return toggle;
        }

        void SetToggle(RectTransform toggle, bool on)
        {
            if (toggle == null)
                return;
            var track = toggle.GetComponent<Image>();
            if (track != null)
            {
                if (on)
                {
                    track.sprite = settingsToggleOnSprite != null ? settingsToggleOnSprite : toggleTrackSprite;
                    track.color = Color.white;
                }
                else
                {
                    track.sprite = toggleTrackSprite != null ? toggleTrackSprite : settingsToggleOnSprite;
                    track.color = Hex("4A4E58");
                }
                track.type = Image.Type.Sliced;
            }
            var knob = toggle.Find("Knob") as RectTransform;
            if (knob != null)
                knob.anchoredPosition = new Vector2(on ? 26f : -26f, 0f);
        }

        void PaintArt(RectTransform rect, Sprite sprite, bool hit, bool slice)
        {
            var image = rect.gameObject.GetComponent<Image>();
            if (image == null)
                image = rect.gameObject.AddComponent<Image>();
            image.raycastTarget = hit;
            image.color = Color.white;
            image.sprite = sprite;
            if (sprite != null && slice)
                image.type = Image.Type.Sliced;
            else
            {
                image.type = Image.Type.Simple;
                image.preserveAspect = true;
            }
        }

        void BuildShopPanel()
        {
            shopPanel = MakeRect("Shop", menuRoot);
            Stretch(shopPanel);
            MakeMenuHeader(shopPanel, "Shop");
            MakeShopPack(shopPanel, "Pack5", 0.72f, 0.86f, 1, "5 Hints", "$0.99", false, () => BuyHints(5));
            MakeShopPack(shopPanel, "Pack20", 0.56f, 0.70f, 3, "20 Hints", "$2.99", true, () => BuyHints(20));
            MakeShopPack(shopPanel, "Pack50", 0.40f, 0.54f, 5, "50 Hints", "$5.99", false, () => BuyHints(50));
            var bundle = MakeDarkRow(shopPanel, "Bundle", 0.22f, 0.38f);
            var ring = MakeRect("Ring", bundle);
            Stretch(ring, -6f);
            PaintSliced(ring, popupCardSprite, Hex("7B5CFF"), false);
            var inner = MakeRect("Inner", bundle);
            Stretch(inner, 8f);
            PaintSliced(inner, popupCardSprite, Hex("1B1E28"), false);
            inner.SetAsLastSibling();
            var mini = MakeRect("Mini", bundle);
            Pin(mini, 0.04f, 0.18f, 0.22f, 0.82f);
            BuildMiniGrid(mini, new[] { "red", "blue", "yellow", "violet" }, 2, 2);
            var bundleLabel = MakeText(bundle, "Unlimited Hints\n+ Neon Theme", 30, Color.white, fontBlack);
            bundleLabel.enableWordWrapping = true;
            bundleLabel.alignment = TextAlignmentOptions.MidlineLeft;
            Pin(bundleLabel.rectTransform, 0.24f, 0.12f, 0.68f, 0.88f);
            var bundleBuy = MakeRect("Buy", bundle);
            Pin(bundleBuy, 0.70f, 0.28f, 0.96f, 0.72f);
            PaintSliced(bundleBuy, greenButtonSprite, Color.white);
            var bundlePrice = MakeText(bundleBuy, "$7.99", 30, Color.white, fontBlack);
            Stretch(bundlePrice.rectTransform);
            BindButton(bundleBuy.gameObject, BuyUnlimitedBundle);
            var restore = MakePill(shopPanel, "Restore", popupCardSprite, "Restore Purchases", Color.white, out _);
            Pin(restore, 0.12f, 0.08f, 0.88f, 0.18f);
            var restoreImage = restore.GetComponent<Image>();
            if (restoreImage != null)
                restoreImage.color = Hex("22262F");
            BindButton(restore.gameObject, RestorePurchases);
            shopStatus = MakeText(shopPanel, "", 24, Hex("FFD21F"), fontBold);
            Pin(shopStatus.rectTransform, 0.08f, 0.02f, 0.92f, 0.08f);
        }

        void MakeShopPack(RectTransform parent, string name, float yMin, float yMax, int bulbs, string title, string price, bool best, UnityEngine.Events.UnityAction buy)
        {
            var row = MakeDarkRow(parent, name, yMin, yMax);
            var icons = MakeRect("Icons", row);
            Pin(icons, 0.04f, 0.18f, 0.24f, 0.82f);
            for (int i = 0; i < Mathf.Min(3, bulbs); i++)
            {
                var bulb = MakeRect("B" + i, icons);
                float x = bulbs == 1 ? 0.25f : i * 0.28f;
                float y = i == 2 ? 0.08f : 0.22f;
                bulb.anchorMin = new Vector2(x, y);
                bulb.anchorMax = new Vector2(x + 0.5f, y + 0.7f);
                bulb.offsetMin = bulb.offsetMax = Vector2.zero;
                var image = bulb.gameObject.AddComponent<Image>();
                image.sprite = hintBulbSprite;
                image.preserveAspect = true;
                image.raycastTarget = false;
            }
            var label = MakeText(row, title, 36, Color.white, fontBlack);
            label.alignment = TextAlignmentOptions.MidlineLeft;
            Pin(label.rectTransform, 0.26f, 0.18f, 0.68f, 0.82f);
            var buyBtn = MakeRect("Buy", row);
            Pin(buyBtn, 0.70f, 0.28f, 0.96f, 0.72f);
            PaintSliced(buyBtn, greenButtonSprite, Color.white);
            var priceLabel = MakeText(buyBtn, price, 30, Color.white, fontBlack);
            Stretch(priceLabel.rectTransform);
            BindButton(buyBtn.gameObject, buy);
            if (!best)
                return;
            var badge = MakeRect("Best", row);
            badge.anchorMin = badge.anchorMax = new Vector2(1f, 1f);
            badge.pivot = new Vector2(1f, 1f);
            badge.anchoredPosition = new Vector2(-12f, -8f);
            badge.sizeDelta = new Vector2(150f, 36f);
            PaintSliced(badge, popupCardSprite, Hex("E23B3B"));
            var bestLabel = MakeText(badge, "Best Value", 18, Color.white, fontBlack);
            Stretch(bestLabel.rectTransform);
        }

        void BuildHowToPanel()
        {
            howToPanel = MakeRect("HowTo", menuRoot);
            Stretch(howToPanel);
            MakeMenuHeader(howToPanel, "How to Play");
            BuildHowToStep(howToPanel, "Step1", 0.64f, 0.88f, "1", "Drag a piece\nonto the board", true);
            BuildHowToStep(howToPanel, "Step2", 0.38f, 0.62f, "2", "Tap to rotate", false);
            BuildHowToStep(howToPanel, "Step3", 0.16f, 0.36f, "3", "Fill every cell to win", false);
            var got = MakePill(howToPanel, "GotIt", yellowButtonSprite, "Got It", Hex("1A1A1A"), out _);
            Pin(got, 0.14f, 0.04f, 0.86f, 0.14f);
            BindButton(got.gameObject, CloseHowTo);
        }

        void BuildHowToStep(RectTransform parent, string name, float yMin, float yMax, string number, string caption, bool drag)
        {
            var card = MakeDarkRow(parent, name, yMin, yMax);
            var badge = MakeRect("N", card);
            Pin(badge, 0.04f, 0.62f, 0.14f, 0.92f);
            PaintSliced(badge, blueButtonSprite, Color.white);
            var n = MakeText(badge, number, 28, Color.white, fontBlack);
            Stretch(n.rectTransform);
            var text = MakeText(card, caption, 28, Color.white, fontBold);
            text.enableWordWrapping = true;
            text.alignment = TextAlignmentOptions.TopLeft;
            Pin(text.rectTransform, 0.16f, 0.58f, 0.96f, 0.94f);
            var art = MakeRect("Art", card);
            Pin(art, 0.06f, 0.08f, 0.94f, 0.56f);
            if (drag)
            {
                var piece = MakeRect("Piece", art);
                Pin(piece, 0.02f, 0.28f, 0.38f, 0.72f);
                PaintSliced(piece, redSprite, PieceTint("red"), false);
                var arrow = MakeText(art, ">", 42, Hex("3DDC84"), fontBlack);
                Pin(arrow.rectTransform, 0.38f, 0.2f, 0.52f, 0.8f);
                var board = MakeRect("Board", art);
                Pin(board, 0.54f, 0.02f, 0.98f, 0.98f);
                BuildMiniGrid(board, new[]
                {
                    "empty", "empty", "empty",
                    "green", "green", "green",
                    "empty", "empty", "empty"
                }, 3, 3);
            }
            else if (number == "2")
            {
                var a = MakeRect("A", art);
                Pin(a, 0.04f, 0.28f, 0.40f, 0.72f);
                PaintSliced(a, yellowSprite, PieceTint("yellow"), false);
                var arrow = MakeText(art, "TURN", 26, Hex("3DDC84"), fontBlack);
                Pin(arrow.rectTransform, 0.40f, 0.2f, 0.58f, 0.8f);
                var b = MakeRect("B", art);
                Pin(b, 0.60f, 0.08f, 0.82f, 0.92f);
                PaintSliced(b, yellowSprite, PieceTint("yellow"), false);
            }
            else
            {
                var board = MakeRect("Win", art);
                Pin(board, 0.22f, 0.0f, 0.78f, 1f);
                BuildMiniGrid(board, new[]
                {
                    "red", "blue", "yellow", "empty",
                    "blue", "black", "red", "yellow",
                    "yellow", "red", "blue", "black",
                    "red", "yellow", "black", "blue"
                }, 4, 4);
            }
        }

        void BuildThemePanel()
        {
            themePanel = MakeRect("Theme", menuRoot);
            Stretch(themePanel);
            MakeMenuHeader(themePanel, "Theme");
            var candy = MakeDarkRow(themePanel, "Candy", 0.70f, 0.84f);
            var candyLabel = MakeText(candy, "Candy", 36, Color.white, fontBlack);
            candyLabel.alignment = TextAlignmentOptions.MidlineLeft;
            Pin(candyLabel.rectTransform, 0.08f, 0.1f, 0.7f, 0.9f);
            BindButton(candy.gameObject, () => EquipTheme("candy"));
            var neon = MakeDarkRow(themePanel, "Neon", 0.54f, 0.68f);
            var neonLabel = MakeText(neon, "Neon", 36, Color.white, fontBlack);
            neonLabel.alignment = TextAlignmentOptions.MidlineLeft;
            Pin(neonLabel.rectTransform, 0.08f, 0.1f, 0.7f, 0.9f);
            BindButton(neon.gameObject, () => EquipTheme("neon"));
            themeStatus = MakeText(themePanel, "", 26, Hex("FFD21F"), fontBold);
            Pin(themeStatus.rectTransform, 0.08f, 0.42f, 0.92f, 0.52f);
        }

        void BuildPrivacyPanel()
        {
            privacyPanel = MakeRect("Privacy", menuRoot);
            Stretch(privacyPanel);
            MakeMenuHeader(privacyPanel, "Privacy Policy");
            var card = MakeDarkRow(privacyPanel, "Body", 0.12f, 0.88f);
            var body = MakeText(card, "GridAura stores progress, stars, hints, and settings on this device. No account is required. Purchases restore on this device. We do not collect personal data in this version.", 28, new Color(1f, 1f, 1f, 0.86f), fontBold);
            body.enableWordWrapping = true;
            body.alignment = TextAlignmentOptions.TopLeft;
            body.overflowMode = TextOverflowModes.Overflow;
            Pin(body.rectTransform, 0.06f, 0.06f, 0.94f, 0.94f);
        }

        void BuildMiniGrid(RectTransform grid, string[] cells, int cols, int rows)
        {
            for (int i = 0; i < cells.Length; i++)
            {
                int col = i % cols;
                int row = i / cols;
                var cell = MakeRect("C" + i, grid);
                cell.anchorMin = new Vector2(col / (float)cols, 1f - (row + 1) / (float)rows);
                cell.anchorMax = new Vector2((col + 1) / (float)cols, 1f - row / (float)rows);
                cell.offsetMin = new Vector2(4f, 4f);
                cell.offsetMax = new Vector2(-4f, -4f);
                var image = cell.gameObject.AddComponent<Image>();
                image.raycastTarget = false;
                string color = cells[i];
                if (color == "empty")
                {
                    image.sprite = emptySprite;
                    image.color = Color.white;
                }
                else if (color == "black")
                {
                    image.sprite = blackSprite;
                    image.color = Color.white;
                }
                else if (color == "green")
                {
                    image.sprite = emptySprite;
                    image.color = Hex("3DDC84");
                }
                else
                {
                    image.sprite = SpriteFor(color);
                    image.color = PieceTint(color);
                }
                image.type = Image.Type.Sliced;
                if (image.sprite != null)
                    FitSlice(image, 12f, 16f);
            }
        }

        void MakePadlock(RectTransform tile)
        {
            var lockRoot = MakeRect("Lock", tile);
            Pin(lockRoot, 0.32f, 0.22f, 0.68f, 0.72f);
            var shackle = MakeRect("Shackle", lockRoot);
            Pin(shackle, 0.22f, 0.48f, 0.78f, 0.98f);
            var shackleImage = shackle.gameObject.AddComponent<Image>();
            shackleImage.color = Hex("8B93A3");
            shackleImage.raycastTarget = false;
            var body = MakeRect("Body", lockRoot);
            Pin(body, 0.08f, 0.02f, 0.92f, 0.58f);
            PaintSliced(body, popupCardSprite, Hex("9AA3B2"), false);
            lockRoot.gameObject.SetActive(false);
        }

        void PaintSliced(RectTransform rect, Sprite sprite, Color color, bool hit = true)
        {
            var image = rect.gameObject.GetComponent<Image>();
            if (image == null)
                image = rect.gameObject.AddComponent<Image>();
            image.raycastTarget = hit;
            image.color = color;
            if (sprite != null)
            {
                image.sprite = sprite;
                image.type = Image.Type.Sliced;
                FitSlice(image, 28f, 36f);
            }
        }


        int HintBank()
        {
            return Mathf.Max(0, PlayerPrefs.GetInt(PrefHints, 3));
        }

        bool UnlimitedHints()
        {
            return PlayerPrefs.GetInt(PrefUnlimited, 0) == 1;
        }

        bool NeonOwned()
        {
            return PlayerPrefs.GetInt(PrefNeon, 0) == 1 || UnlimitedHints();
        }

        bool NeonOn()
        {
            return NeonOwned() && PlayerPrefs.GetString(PrefTheme, "candy") == "neon";
        }

        void SpendHint()
        {
            if (state != null && state.FreeHints)
                return;
            if (UnlimitedHints())
                return;
            PlayerPrefs.SetInt(PrefHints, Mathf.Max(0, HintBank() - 1));
            PlayerPrefs.Save();
        }

        void BuyHints(int amount)
        {
            PlayerPrefs.SetInt(PrefHints, HintBank() + amount);
            PlayerPrefs.Save();
            if (state != null && !state.FreeHints && !UnlimitedHints())
                state.hintsLeft += amount;
            FillShop("Added " + amount + " hints");
            Haptics.Tick();
        }

        void BuyUnlimitedBundle()
        {
            PlayerPrefs.SetInt(PrefUnlimited, 1);
            PlayerPrefs.SetInt(PrefNeon, 1);
            PlayerPrefs.SetString(PrefTheme, "neon");
            PlayerPrefs.Save();
            if (state != null)
                state.hintsLeft = int.MaxValue;
            FillShop("Unlimited hints unlocked");
            FillTheme();
            Haptics.Tick();
        }

        void RestorePurchases()
        {
            FillShop(UnlimitedHints() || HintBank() > 0 ? "Purchases restored" : "Nothing to restore");
            FillTheme();
            Haptics.Tick();
        }

        void FillShop(string note = null)
        {
            if (shopStatus == null)
                return;
            if (!string.IsNullOrEmpty(note))
                shopStatus.text = note;
            else if (UnlimitedHints())
                shopStatus.text = "Unlimited hints on";
            else
                shopStatus.text = "Hints: " + HintBank();
        }

        void FillTheme()
        {
            if (themeStatus == null)
                return;
            string current = PlayerPrefs.GetString(PrefTheme, "candy");
            themeStatus.text = NeonOn() ? "Neon equipped" : (current == "neon" && !NeonOwned() ? "Buy Neon in the Shop" : "Candy equipped");
        }

        void EquipTheme(string theme)
        {
            if (theme == "neon" && !NeonOwned())
            {
                FillTheme();
                OpenShop();
                FillShop("Unlock Neon with Unlimited Hints");
                return;
            }
            PlayerPrefs.SetString(PrefTheme, theme);
            PlayerPrefs.Save();
            FillTheme();
        }

        void RateUs()
        {
            Application.OpenURL("https://apps.apple.com");
        }

        void TapVersion()
        {
            versionTaps++;
            if (versionTaps < 8)
                return;
            versionTaps = 0;
            ShowDebugPanel();
        }

        void CloseHowTo()
        {
            PlayerPrefs.SetInt(PrefHowTo, 1);
            PlayerPrefs.Save();
            settingsFromPlay = false;
            ShowMenu(homePanel);
        }

        void OpenShopFromPlay()
        {
            HideOverlayNow();
            settingsFromPlay = true;
            ShowMenu(shopPanel);
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
            RecordStars(winEarnedStars);
            if (starsTotalText != null)
                starsTotalText.text = "";
            LayoutWinStars();
            bool first = !celebrating;
            celebrating = true;
            ClearLevelFailStreak();
            if (first)
            {
                foreach (Transform child in placedLayer)
                    StartCoroutine(Motion.Punch(child, 1.04f, 0.16f));
                ShowOverlay(winPanel);
                if (confettiRoutine != null)
                    StopCoroutine(confettiRoutine);
                confettiRoutine = StartCoroutine(PlayScreenConfetti());
                starRoutine = StartCoroutine(PopStars());
            }
            else
            {
                int[] order = { 1, 0, 2 };
                int count = Mathf.Clamp(winEarnedStars, 0, order.Length);
                for (int i = 0; i < winStarRoots.Length; i++)
                {
                    if (winStarRoots[i] == null)
                        continue;
                    bool earned = false;
                    for (int n = 0; n < count; n++)
                    {
                        if (order[n] == i)
                            earned = true;
                    }
                    winStarRoots[i].gameObject.SetActive(earned);
                    winStarRoots[i].localScale = earned ? Vector3.one : Vector3.zero;
                    if (winStars[i] != null)
                        winStars[i].color = Color.white;
                    if (winStarGlows[i] != null)
                        winStarGlows[i].color = Color.white;
                }
                if (overlay != null && !overlay.gameObject.activeSelf)
                    ShowOverlay(winPanel);
            }
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
                {
                    bool on = winStarRoots[i] != null && winStarRoots[i].gameObject.activeInHierarchy;
                    PulseGlow(winStarGlows[i], t + i * 0.55f, on);
                }
            }
            bool hintOpen = hintPanel != null && hintPanel.gameObject.activeInHierarchy;
            PulseGlow(hintPopupGlow, t, hintOpen);
            bool hintLive = HintUnlocked && !boardIntroActive && hintGroup != null && hintGroup.blocksRaycasts;
            PulseGlow(headerBulbGlow, t, hintLive);
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

    public class PieceControl : MonoBehaviour, IPointerDownHandler, IInitializePotentialDragHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        public GameScreen screen;
        public string pieceId;
        public bool onBoard;

        public void OnPointerDown(PointerEventData eventData) { screen.PointerDown(this, eventData); }
        public void OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (onBoard)
                eventData.useDragThreshold = false;
        }
        public void OnBeginDrag(PointerEventData eventData) { screen.PointerBeginDrag(this, eventData); }
        public void OnDrag(PointerEventData eventData) { screen.PointerDrag(this, eventData); }
        public void OnEndDrag(PointerEventData eventData) { screen.PointerEndDrag(this, eventData); }
        public void OnPointerUp(PointerEventData eventData) { screen.PointerUp(this, eventData); }
    }

    public class TapPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public UnityEngine.Events.UnityAction click;
        public bool fireOnDown;
        Coroutine anim;
        bool pressed;
        Rect pressRect;

        public void OnPointerDown(PointerEventData eventData)
        {
            pressed = true;
            pressRect = ScreenRect();
            Animate(0.96f);
            if (!fireOnDown)
                return;
            pressed = false;
            click?.Invoke();
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
            var canvas = GetComponentInParent<Canvas>();
            Camera cam = null;
            if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
                cam = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
            Vector2 min = RectTransformUtility.WorldToScreenPoint(cam, corners[0]);
            Vector2 max = RectTransformUtility.WorldToScreenPoint(cam, corners[2]);
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
