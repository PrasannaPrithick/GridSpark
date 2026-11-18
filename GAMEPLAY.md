# GridAura — Full Gameplay Document

This is the complete picture of the game as it exists now: what it is, how the grid works, every rule, the campaign, the UI, and how those pieces were added from the first packing puzzle through the live 250-level loop.

Project folder: **GridSpark**. Unity project: **GridAura**. Player-facing name: **GridAura**.

Portrait only. Reference resolution **1080×1920**. Unity 6, URP 2D, uGUI + TextMeshPro. The play screen is built in code (`GameScreen.cs`), not a prefab layout.

---

## 1. What the game is

GridAura is an 8×8 rectangle-packing puzzle.

You are given a framed board. Some cells are **black obstacles**. The rest are **empty cream cells**. You drag colored rectangles from a **tray** onto the board. You win when **every empty cell is covered** — no overlaps, no leftover gaps.

There are always the same eight piece shapes (A–H). What changes per level is:

- where the black blocks sit
- which pieces start in the tray vs already on the board
- whether a piece must be rotated
- extra rules (color, decoy, rescue, must-touch, keystone, commit, turn cap, big-first, border, stone hug, islands, double spare, no undo)
- a move limit (from level 13)

It is not a match-3, not a sliding puzzle, and not Tetris. Pieces do not fall, lock by gravity, or clear lines. You place them by hand until the picture is full.

---

## 2. The grid

### Size and origin

- The board is always **8 rows × 8 columns = 64 cells**.
- Coordinates are **0-indexed**.
- **Row 0 is the top.** Row 7 is the bottom.
- **Col 0 is the left.** Col 7 is the right.
- Every rectangle (black block or colored piece) is stored as **top-left cell + height + width**.
  - `h` = how many **rows** it occupies (vertical size)
  - `w` = how many **columns** it occupies (horizontal size)

Example: a piece at `row 1, col 2, h 2, w 3` covers:

```
     0 1 2 3 4 5 6 7
  0  . . . . . . . .
  1  . . X X X . . .
  2  . . X X X . . .
  3  . . . . . . . .
```

The on-screen numbers around the frame are **1–8** for the player. Internally everything is 0–7.

### Three kinds of cell

| Cell | What it is | Can cover? | Can move? |
| --- | --- | --- | --- |
| Empty (cream) | Must be filled to win | Yes — that is the goal | n/a |
| Black | Locked obstacle | No | No |
| Colored piece | A placed rectangle | Occupies empty cells | Yes, unless locked |

A cell has at most one owner. `CellOwner` returns `"black"`, a piece id (`"A"`…`"H"`), or empty.

### Why 8×8 and 8 pieces

The eight pieces have a combined area of **58**. A full classic pack therefore needs **6 black cells** (64 − 58 = 6) so the board can be filled exactly. Tutorial and rescue levels hide some pieces as preplaced/locked, so the tray is smaller. Decoy levels add extra black so **only 7 of 8 pieces fit**.

### Visual board

- The board sits in a **frame** with a rim.
- Empty cells use a sliced cream tile sprite.
- Black cells use a dark block sprite.
- Colored pieces use 9-slice candy slabs (red / blue / yellow / violet).
- Row numbers sit on the left; column numbers sit on top.
- During a drag, a **ghost** snaps to the nearest legal (or illegal) cell under the piece.

---

## 3. The eight pieces

The catalog is shared by tutorial, main, special, and daily JSON. Default orientation is the unrotated `h × w` below. A 90° turn **swaps h and w**. Squares do not change.

| Id | Default | Area | Color | Rotates? | Partner (color rule) |
| --- | --- | --- | --- | --- | --- |
| A | 1×4 | 4 | Red | Yes → 4×1 | G |
| B | 1×5 | 5 | Blue | Yes → 5×1 | E |
| C | 2×2 | 4 | Yellow | No | F |
| D | 3×3 | 9 | Violet | No | H |
| E | 2×3 | 6 | Blue | Yes → 3×2 | B |
| F | 2×4 | 8 | Yellow | Yes → 4×2 | C |
| G | 2×5 | 10 | Red | Yes → 5×2 | A |
| H | 3×4 | 12 | Violet | Yes → 4×3 | D |

Color history: H and D used to be **white**, which disappeared against empty cells. They are **violet** now.

Tray labels show the **current** orientation, e.g. `2 × 3` or `3 × 2`.

There is no piece I, no irregular polyomino, no L/T/S shapes. Every piece is an axis-aligned rectangle.

---

## 4. How a level is built

A level is a JSON object. Campaign entries in `campaign.json` pick a source file and an id, then stamp world/tag/moves on top.

### Fields

| Field | Meaning |
| --- | --- |
| `black` | Obstacle rectangles (`row, col, h, w`) |
| `preplaced` | Pieces already on the board **locked** (tutorial). Cannot drag, undo, or hint-move. |
| `toPlace` | Piece ids that start in the tray |
| `solution` | Intended packing. Used by hints and tutorial guides. Not shown to the player. |
| `rotationNeeded` | Tray pieces whose solution orientation is the rotated form |
| `tutorial` | Coach text, aids, whether rotation is on |
| `initial` | Rescue: pieces already on the board **unlocked** |
| `wrongPieces` | Rescue: which of those initial pieces are in the wrong place |
| `unused` | Decoy: the spare piece that does not fit |
| `type` | `classic` / `color` / `decoy` / `rescue` |
| `tag` | `learn` / `easy` / `medium` / `hard` / `boss` |
| `moveLimit` | 0 = unlimited (tutorial). Otherwise the countdown |
| `par` | Target placements; move limit is par + slack |
| `intro` | First time this rule appears; extra coach text |
| `coachText` | Banner copy for new rules / world first visit |

### Data files

| File | What it holds |
| --- | --- |
| `Resources/Data/campaign.json` | Live 250-level playlist |
| `Resources/Data/campaign_levels.json` | Unique packs for levels 13–250 |
| `Resources/Data/tutorial_levels.json` | 12 teaching levels |
| `Resources/Data/levels.json` | 50 classic main packs |
| `Resources/Data/special_levels.json` | 10 color + 10 decoy + 10 rescue |
| `Resources/Data/daily_levels.json` | 25 dailies — **on disk, not in the live loop** |

`campaign.json` ids are strings so Unity `JsonUtility` can load mixed tutorial/main/special ids (`"12"`, `"C1"`, `"R10"`).

### Win check

Walk all 64 cells. If any cell has no owner (not black, not a piece), the level is not complete. Decoy wins with one piece still in the tray. Color wins only if every empty cell is filled **and** no same-color pair shares an edge (illegal drops are already blocked).

---

## 5. Player actions

### Drag and place

1. Touch a tray piece, or a movable piece already on the board.
2. Drag. The piece follows the finger, lifted slightly so the hand does not cover it.
3. If the piece started on the board, it is **lifted off** the grid for the drag (its cells become empty again) so you can rotate and re-drop.
4. A ghost shows the snapped top-left cell.
   - Valid: same color, translucent.
   - Invalid: coral/red translucent.
5. Release.
   - Valid: piece eases into the cell, spark VFX, squash, board punch, haptic. This **spends one move** if the level has a limit.
   - Invalid from tray: piece eases back to the tray. Miss haptic. No move spent.
   - Invalid from board: piece eases back to its old cell. No move spent.
   - Dropped on the tray: piece returns to the tray (undoable). No move spent.

Drag starts after the finger moves about **56 pixels**, so a slightly sloppy tap is still a tap.

On **color** levels, a drop that fits geometrically but touches the same-color partner is illegal. Message: **Same colors can't touch**.

### Rotate

Squares (C 2×2, D 3×3) never rotate.

Non-square pieces turn **90°** (h and w swap):

| Where | How |
| --- | --- |
| Tray | Tap the piece |
| Board | Tap the piece. No Rotate chip on top of it. |
| Mid-drag | A large **Rotate** button sits **under the board**. A second finger can tap it (fires on touch down) |

If the turned rectangle still fits — same top-left, or a nearby origin that still **overlaps** the old footprint — it spins and stays. If it cannot fit, the piece **shakes**, a miss haptic plays, it stays where it was, and the toast says **Rotate is not possible**.

Rotate does **not** spend a move.

Tutorial levels 1–6 have rotation **off**. Levels 7–12 and the whole campaign have it **on**.

TURN badges (levels 7–10) sit on tray pieces that still need a turn to match the solution. The looping tap-hand plays on **level 7 only**. TURN hides while you drag.

### Return to tray

Drag a piece onto the tray, or tap a **square** placed piece. Non-square taps try to rotate instead of returning. Undoable. No move spent.

### Undo

Reverts the last recorded step: a place, a tray/board rotate that was stored, or a return to tray. If that step spent a move, the move is **refunded**. Locked / preplaced / hint-locked pieces cannot be undone away.

### Reset

If any unlocked pieces are on the board, a confirm card asks. Confirming:

- removes unlocked pieces
- clears the undo stack
- restores `movesLeft` to `moveLimit`
- on **rescue**, puts the `initial` arrangement back
- locked / preplaced / hint-locked pieces stay

### Hint

Hidden on levels **1–3** (the hand and glowing gaps teach placement). From **level 4**, the bulb appears after the board intro.

1. Tap the header bulb. If hints remain (live build: unlimited, badge `∞`), the hint runs immediately — no popup.
2. If hints were exhausted, a **Need a Hint?** card would offer more (shop not live).
3. A large bulb flies from the header to screen center, pulses, fades.
4. The next unsolved **solution** piece is chosen, **largest area first**.
5. Unlocked pieces sitting on that target return to the tray.
6. If the piece needs turning, it **spins to the solution orientation first**, then flies. No yellow target highlight.
7. The piece lifts from the tray, arcs to the cell, sits, and **locks**. Hint badge on the corner. You cannot move it again.
8. `hintsUsed` goes up (except tutorial 1–3, where hints are free and the button is hidden anyway).
9. If that placement fills the board, the win popup waits until the sit finishes.

---

## 6. What is locked vs free

| Piece state | Drag | Rotate | Undo away | Hint can move it |
| --- | --- | --- | --- | --- |
| In the tray | Yes | Yes (if non-square) | — | Yes (places it) |
| Placed by the player | Yes | Yes (if it still fits) | Yes | Yes, if it blocks the hinted cell |
| Rescue `initial` (unlocked) | Yes | Yes | Yes | Yes |
| Preplaced (tutorial) | No | No | No | No |
| Placed by hint | No | No | No | No (already correct) |
| Black block | No | No | No | No |

---

## 7. Tutorial (levels 1–12)

World 0. Tag `learn`. **No move limit.** Board intro: empty cells sit in on a diagonal wave, then locked blocks, then tray pieces slide in from the left. You cannot place until that intro finishes.

Each tutorial level is a **full 8×8 pack** with 3 black regions, but most pieces are already **preplaced and locked**. The tray only holds the lesson pieces. Preplaced count goes **7 → 1** as the pack opens up.

| Level | Tray | Preplaced | Rotation | Aids | Coach |
| --- | --- | --- | --- | --- | --- |
| 1 | D | 7 | Off | finger-drag, target outline | Drag the piece into the glowing gap. |
| 2 | C, H | 6 | Off | target outline | Two pieces this time. Fill both gaps. |
| 3 | B, D, H | 5 | Off | region glow | Match each piece to the gap with its shape. |
| 4 | B, C, F, G | 4 | Off | region glow | Fill every gap. Made a mistake? Tap Undo. |
| 5 | A, B, C, D, F | 3 | Off | region glow | Stuck? Tap the lightbulb for a hint. |
| 6 | A, C, E, F, G, H | 2 | Off | — | Fill all the gaps to finish. |
| 7 | C, G (G must turn) | 6 | On | finger-tap, TURN, outline | This piece needs to turn. Tap it to rotate, then drag it into the gap. |
| 8 | B, E, F (B must turn) | 5 | On | region glow | Tap a piece to rotate it before placing. |
| 9 | A, B, E, F (B, E turn) | 4 | On | region glow | Two pieces need turning here. |
| 10 | A, B, D, E, F (A, E turn) | 3 | On | — | Only rotate a piece when it needs it. |
| 11 | A, B, D, E, F, H (E, F, H turn) | 2 | On | — | Almost ready for full puzzles. |
| 12 | A–G (B, E, F turn) | 1 | On | — | Last practice! Next: puzzles with all 8 pieces. |

Guided drag on 1–3: only the next unplaced piece is highlighted. A looping ghost travels tray → solution cell. The demo stops as soon as the player starts a real drag.

Hint button: hidden 1–3, shown from 4, disabled during intro. Level 4 pulses **Undo**, then the hint bulb. Level 5 pulses the bulb again.

Coach banner can be tapped away. It comes back when the level is shown again.

---

## 8. Campaign (levels 13–250)

Tutorial **1–12 stays frozen**. After that, `campaign.json` plays **238 unique packs** (23 worlds of 10, then an 8-level finale). Wave inside each 10-level world:

**easy, medium, easy, medium, hard, easy, medium, easy, hard, boss**  
(short: **EMEMHEMEHB**)

The level after a boss is always easy. Hard and boss tint the level number and can flash a banner. Campaign levels skip the tutorial sit-in; instead a **briefing card** (see UI) names the type and move count.

**Every 3 levels** from 13 (13, 16, 19, …) shows a new rule, layout, or challenge. New-rule intros (coach trial):

| First time | Global | Type |
| --- | --- | --- |
| Full pack | 13 | Classic |
| Color Clash | 16 | Color |
| Odd One Out | 19 | Decoy |
| Rescue | 25 | Rescue |
| Must Touch | 28 | Partners must share an edge |
| Keystone | 31 | One locked piece already on the board |
| Commit | 34 | Dropped pieces stay |
| Turn limit | 37 | Only a few rotates |
| Big First | 40 | Largest leftover piece next |
| Border Seat | 43 | Every piece touches the frame |
| Stone Hug | 46 | Every piece touches black |
| Islands | 49 | Stay inside a room |
| Double Spare | 55 | Leave two pieces |
| No Undo | 58 | Undo is off |

### Worlds

23 worlds of 10 (13–242) then Grand Finale 243–250. Names include Color Clash, Odd One Out, Rescue Mission, Touch & Lock, New Tricks, Mix It Up, Borderlands, Island Hop, Commit Path, Harmony, Star Road, High Farm, Grand Finale.

Counts: **12 tutorial + 238 unique packs = 250**.

### Move limit

From level 13, each drop onto the board costs **1 move**.

`moveLimit = par + slack` — Candy Crush slack, not tight:

| Tag | Early slack | Late slack |
| --- | --- | --- |
| easy | +8 | +6 |
| medium | +6 | +5 |
| hard | +5 | +4 |
| boss | +4 | +3 |

Decoy par is 7 (seven placements). Rescue par is lower (4–6) because several pieces already sit on the board.

**Does not spend a move:** rotate, return to tray, hint, undo (undo **refunds** if the undone step was a drop), reset (restores the full count).

At 0 moves with a gap still open: **Out of moves** popup — Retry / Home.

### Full playlist

Levels 1–12 stay in `tutorial_levels.json`. Levels 13–250 are unique packs in `campaign_levels.json`, queued by `campaign.json`. Do not reuse the old 92-level main/special ids for the live loop.

| # | World | Tag | Type | Src | Id | Moves |
| --- | ---: | --- | --- | --- | --- | ---: |
| 1–12 | Tutorial | learn | classic | tutorial | 1–12 | none |
| 13–250 | Worlds 1–24 | Candy Crush wave | mixed unique | pack | P013–P250 | par + slack |
---

## 9. Game types

Campaign levels mix the types below. The briefing card names the type at the start of levels 13+.

### Classic

Fill every empty cell. All eight pieces (or the tutorial subset) belong. Same-color pieces **may** touch. This is the original game.

### Color Clash (`type: color`)

Same-color **partners may not share an edge**. Corners are allowed.

| Pair | Color |
| --- | --- |
| A and G | Red |
| B and E | Blue |
| C and F | Yellow |
| D and H | Violet |

Illegal drop: ghost turns coral, toast **Same colors can't touch**. First seen at **level 16**.

### Odd One Out (`type: decoy`)

All **8 pieces** are in the tray. Only **7 fit**. Leave the spare. First seen at **level 19**.

### Double Spare (`type: doublespare`)

Two pieces do not fit. Leave both. First seen at **level 55**.

### Rescue (`type: rescue`)

Some pieces start **on the board, unlocked**. A listed subset is in the **wrong place**. Reset restores that start. First seen at **level 25**. Wrong pieces wiggle on the intro.

### Must Touch (`type: musttouch`)

Matching colors **must** share an edge. First seen at **level 28**.

### Keystone, Commit, Turn limit, Big First, Border, Stone Hug, Islands, No Undo

See the intro table in section 8. These are live. Timed levels are still not in the loop.

---

## 10. Stars and save data

On **Level Complete**, stars come only from **hints used** (on that attempt). Moves left do not change stars.

| Hints used | Stars |
| ---: | ---: |
| 0 | 3 |
| 1 | 2 |
| 2+ | 1 |

Best stars per **global** level id are stored in PlayerPrefs (`GridAura.BestStars.{n}`). There are **no theme chests**. One look for the whole game: the current candy tiles and frame.

Other prefs: `GridAura.WorldSeen.{world}`, `GridAura.LastLevel`, `GridAura.Haptics`.

---

## 11. Screen layout

Portrait, top to bottom:

1. **Hint** (left) — bulb + infinity badge. Hidden 1–3.
2. **Level pill** (center) — “Level” + number. Number tint: green easy, yellow medium, orange hard, red boss.
3. **Gear** (right) — debug Jump panel in the current build (not a player settings screen).
4. **Moves HUD** (from 13) — LevelPill sprite, text `Moves N`, StarClear **sprite** + a number. No ★ character (it did not render). Text goes orange at 3 moves left, red at 1. The pill stays the cream LevelPill art.
5. **Coach banner** — tutorial copy, new-rule intros, first visit to a world.
6. **Board** — 8×8 in a frame, 1–8 numbers.
7. **Tray** — remaining pieces, one row if ≤4, two rows otherwise. Each slot is a colored slab plus `h × w`.
8. **Undo** and **Reset** candy buttons.
9. **Message line** — short feedback (color toast, out of moves, encourage lines).

### Level briefing (campaign 13+)

Players were missing the tiny moves bar at the top. On campaign levels a **card pops in the middle of the board**:

- Game type: Classic / Color Clash / Odd One Out / Rescue (Hard/Boss prefix when tagged)
- One-line rule: *Fill every empty cell* / *Same colors can't touch* / *Use 7 of 8 pieces* / *Fix the misplaced pieces*
- Yellow **Moves N** chip (YellowButton sprite)
- StarClear icon + lifetime star count

Tap to skip, or wait ~2 seconds. The card then **flies up and shrinks** into the header moves pill, which punches in.

### Popups (dim overlay)

- **Hint** — only when hints would be exhausted (“Need a Hint?” + Get More Hints). Shop not live.
- **Level Complete** — title art 545×196, earned stars only (no faded ghosts), hints used, Next Level, Replay. Rainbow confetti on open.
- **Out of moves** — Retry / Home.
- **Reset confirm** — Reset / Cancel.

Win stars fall one by one: middle, left, right. Center star is larger. Each landing: toon burst using the game star art, squash, card punch, haptic.

Mid-level correct place (not the last piece, and on-solution) can pop **Nice! / Great! / Super! / Awesome! / Yes!**.

Between levels a colored bar wipes across (random direction, not the same twice in a row).

---

## 12. Animation and feel

Nothing should snap.

| Moment | Motion |
| --- | --- |
| Buttons | Scale 0.96 on press, back on release |
| Overlay cards | Fade + scale 0.84→1 with overshoot |
| Tutorial start | Diagonal sit-in of empty cells, then blocks, then tray from the left |
| Campaign start | Briefing pop, then fly to header |
| Rotate | −90° spin ~0.2s, scale pulse, spark, tick haptic |
| Valid drop | Ease to cell, spark explosion, squash, board punch |
| Invalid drop | Ease back, miss haptic |
| Hint | Bulb flight, optional spin, slow lift, arc, sit, lock |
| Win | Card pop, confetti, stars fall, board pieces punch |
| Reset confirm | Same overshoot pop as other cards |

Always on: hint bulb glow, win-star glow, tutorial target/tray pulse.

### Haptics

Editor / desktop: no-op.

| Event | Feel |
| --- | --- |
| Place | Click |
| Intro chain | Light ticks |
| Miss | Error buzz |
| Hint | Double pulse |
| Stars | One pulse per landing |

iOS: Core Haptics + UIKit generators (`GridAuraHaptics.mm`). Android: Vibrator / VibratorManager (`VIBRATE` permission, `GridAuraHaptics.androidlib`). Next Level clears leftover star particles and stops rumble.

### VFX

Epic Toon FX subset in `Resources/Vfx`: spark on drop, star burst / poof on win stars, rainbow confetti blast + shower on the win card.

---

## 13. Technical map

| Script | Role |
| --- | --- |
| `GameScreen.cs` | UI, input, animation, popups, campaign load |
| `GameState.cs` | Board rules, undo, hint, rotate, completion, moves |
| `LevelModels.cs` | JSON shapes |
| `Motion.cs` | Unscaled tweens (safe if a target is destroyed) |
| `Haptics.cs` | Native vibrate |
| `SlabGraphic.cs` | Fallback beveled rects when a sprite is missing |

Fonts: Nunito Black / ExtraBold / Bold / SemiBold, built as TMP assets at runtime.

Art used live: LevelPill, YellowButton, PopupCard, StarClear, StarGlow, empty/black/piece slices, candy Undo/Reset, hint bulb.

Debug: gear → Jump to a global level number. Do not ship as the settings screen.

---

## 14. From scratch to now

How the game grew. This is the product history, not a git log.

### 1. Core packing

One 8×8 board, eight rectangles, black obstacles, tray, drag to place. Win = every empty cell covered. That rule has never changed.

### 2. Tutorial pack

Twelve levels that open the same puzzle slowly: one piece, then two, then undo, then hint, then rotation. Preplaced locked pieces keep the board looking like a real pack while the tray stays small.

### 3. Rotation

90° swap of h/w. Tray tap first (levels 7–10 with TURN). Then a Rotate button while dragging over the board. Then **rotate on the grid**: chip on placed pieces, tap-to-rotate if it still fits, second-finger rotate while dragging. Squares never turn.

### 4. Feel

Unscaled tweens, no snaps, haptics, Epic Toon FX sparks/stars/confetti, level-complete stars, cheer lines, wipe transitions.

### 5. Campaign instead of “tutorial then dump 50 classics”

`campaign.json` became the playlist. Candy Crush wave (EMEMHEMEHB). Named worlds. Hard/boss banners. Move limits from 13. Out-of-moves fail state. Stars from hints. One default skin.

### 6. Three extra types, introduced early

Color at 15, decoy at 23, rescue at 33 — not buried at level 80. Mix worlds 4–6, then Deep Pack and Grand Finale return to pure classic.

### 7. HUD players can actually see

Moves + stars used to live only as a thin bar at the top (and a ★ glyph that did not draw). Now: briefing card in the **middle** (type + moves), flies to a LevelPill HUD with the **StarClear sprite**.

### 8. App shell now, polish later

Splash → Home (Play/Continue, Levels, Daily soon, Settings). Sequential level select. Settings: haptics, jump, shop stub. One default skin. Out-of-moves Home and last-level Next return to Home.

Still later: daily playlist, hint shop, world-map art, full theme packs, extra types.

---

## 15. One-screen rules (player facing)

Cover every cream cell. Black is stone. Drag pieces from the tray. Tap to turn a long piece. Same colors must not touch on Color Clash levels. One extra piece is a fake on Odd One Out — leave it. Rescue levels start messy; fix them. From level 13 you have a move budget; dropping onto the board spends one. Hints lock a piece in the right place and cost stars. Three stars = no hints.
