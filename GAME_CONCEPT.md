# GridAura — Game Concept, Systems, and Animation Map

Portrait puzzle game. Fill an 8×8 board with colored rectangles so every empty cell is covered. No overlaps, no gaps.

---

## 1. Core idea

The board is a framed 8×8 grid.

- **Empty cells** are cream slots you must cover.
- **Black blocks** are locked obstacles. You cannot cover or move them.
- **Colored pieces** are rectangles (1×4, 2×2, 2×3, 3×3, and so on). You drag them from the tray onto the board.

You win a level when every non-black cell is covered.

### Piece colors

| Color | Used for |
| --- | --- |
| Red | Pieces A, G |
| Blue | Pieces B, E |
| Yellow | Pieces C, F |
| Violet | Pieces D, H (formerly white — too close to empty cells) |
| Black | Fixed blockers |

---

## 2. How a level is built

Each level JSON defines:

- `black` — obstacle rectangles (row, col, h, w)
- `preplaced` — pieces already on the board, locked
- `toPlace` — pieces that start in the tray
- `solution` — the intended packing (used by hints and tutorial guides)
- `rotationNeeded` — tray pieces whose solution orientation is rotated
- `tutorial` — optional coach text and on-screen aids

**Playable pack today:** **250** campaign levels from `campaign.json` (12 tutorial + 238 unique packs in `campaign_levels.json`). Daily levels exist as data (25) but are not in the live play loop yet.

Tutorial **1–12 is frozen**. No new rules, no move limit, no briefing card.

From 13, worlds of 10 follow a Candy Crush wave: **easy, medium, easy, medium, hard, easy, medium, easy, hard, boss**. After a boss the next level is easy. Each world is only a little harder than the one before — slack stays generous (easy +8 down to +6 late; boss +4 down to +3). Hard and boss flash a banner. The 8-level finale (243–250) uses easy/medium/easy/medium/hard/easy/hard/boss.

**Every 3 campaign levels** (13, 16, 19, …) shows something new: a new rule intro, or a new layout family, or a light extra challenge. New-rule intros get a coach trial. Never stack two harsh modifiers. No timers.

| Band | Levels | Worlds |
| --- | --- | --- |
| Tutorial | 1–12 | Learn |
| Open | 13–42 | Color Clash, Odd One Out, Rescue Mission |
| New tricks | 43–122 | Touch & Lock through Borderlands |
| Mix | 123–242 | Stone, islands, commit, big-first, turn limit, harmony |
| Finale | 243–250 | Grand Finale |

### Special types

- **Color** — same-color partners may not share an edge (A–G, B–E, C–F, D–H). Corners are fine.
- **Decoy** — 8 in the tray, only 7 fit. **Double Spare** — leave two.
- **Rescue** — some pieces start on the board unlocked, a few in the wrong place.
- **Must Touch** — matching colors **must** share an edge.
- **Keystone** — one piece is already locked on the board.
- **Commit** — after you drop a piece, it stays (undo still works unless No Undo).
- **Turn limit** — only N rotates.
- **Big First** — place the biggest leftover piece next.
- **Border Seat** — every piece must touch the frame.
- **Stone Hug** — every piece must touch a black block.
- **Islands** — rooms split by stone; stay inside a room.
- **No Undo** — undo is off for that level. Reset still works.

### Moves

From level 13, each drop onto the board costs **1 move**. `moveLimit = par + slack` with Candy Crush slack (not tight). Rotate, return to tray, hint, and undo-refund cost nothing. Tutorial levels have no limit.

---

## 3. Screen layout (portrait 1080×1920)

Top to bottom:

1. **Hint button** (left) with bulb + infinity badge — hidden on levels 1–3, enabled from level 4 after the board intro  
2. **Level pill** (center) — “Level” + number, tinted by wave tag (green easy, yellow medium, orange hard, red boss)  
3. **Settings gear** (right)  
4. **Moves / stars row** (from level 13) — LevelPill sprite, “Moves N”, and the StarClear icon plus a number (no ★ character). On campaign levels this row first pops in the **middle of the board** as a briefing card (game type + move count), then flies up into the header.  
5. **Coach banner** — tutorial, new-rule intros, and the first visit to a world  
6. **Board** — 8×8 inside a frame, with row/column numbers  
7. **Tray** — remaining pieces (one or two rows)  
8. **Undo** and **Reset** candy buttons  
9. **Message line** — short feedback 

No Rotate chip on placed pieces. Tap a piece on the board to turn it if the new rectangle still fits; if not, it shakes, a miss haptic plays, and the toast says “Rotate is not possible”. While dragging a rotatable piece over the board, a Rotate button sits under the board so a second finger can tap it.

Popups sit on a dim overlay:

- **Hint** — only when you have no hints left (“Need a Hint?” + Get More Hints)  
- **Level Complete** — title art, 3 stars, hints used, Next Level, Replay  
- **Out of moves** — Retry / Home (from level 13)  
- **Reset confirm** — Reset / Cancel  

---

## 4. Player actions

### Drag and place

1. Touch a tray piece (or a movable piece already on the board).
2. Drag. The piece follows the finger, slightly above the touch so it is not covered by the hand.
3. A **ghost** shows the snapped cell on the board.
   - Valid drop: same piece color, translucent.
   - Invalid drop: coral/red translucent.
4. Release.
   - Valid: piece eases into the cell, then a small landing punch, plus a light haptic.
   - Invalid from tray: piece eases back to the tray.
   - Invalid from board: piece eases back to its old cell.
   - Dropped on the tray: piece returns to the tray (undoable).

Tap a placed non-square piece to rotate it in place. If the turn does not fit, the piece shakes back, vibrates, and shows “Rotate is not possible”. Tap a square (or a piece that cannot rotate) to send it back to the tray. Drag a piece onto the tray to return it.

### Rotate

Square pieces do not rotate. Non-square pieces can turn 90°:

- Tap the tray piece, or
- Tap the piece on the board, or
- While dragging over the board, tap the Rotate button under the board (works with a second finger).

Each tap turns 90° with a short spin if the new rectangle still overlaps its old cells and fits. If it cannot fit, the piece shakes, stays where it was, and toasts “Rotate is not possible”. Rotate, return to tray, and undo do not spend a move. TURN badges show on levels 7–10 only on pieces that still need a turn. The tap-hand plays on that first lesson only. While dragging, TURN stays hidden.

### Undo

Reverts the last recorded move (place, rotate-related tray change, or return to tray).

### Reset

If any unlocked pieces are on the board, a confirm card appears. Confirming clears unlocked pieces and the undo stack, and restores the move count. Rescue levels put the starting arrangement back. Locked / preplaced pieces stay.

### Hint

The hint button is hidden on levels 1–3 (hand + glowing gaps teach placement). It appears from **level 4**, but stays disabled while the board intro is playing.

1. Tap the header hint button. If you still have hints (including unlimited), the hint runs immediately — no popup.
2. If you have **no** hints left, the “Need a Hint?” card opens so you can get more.
3. A large bulb flies from the header to the center of the screen, pulses, then fades.
4. The next unsolved solution piece is chosen (largest area first).
5. Wrong pieces sitting on that target return to the tray if they are not locked.
6. If the piece needs turning, it **spins to the solution orientation first**, then flies. No yellow target highlight.
7. The piece **lifts slowly from the tray**, arcs to the cell, then **sits** onto the spot.
8. That piece is **locked** (hint badge on the corner). You cannot move it again.
9. If that placement finishes the board, the win popup plays after the sit.

Tutorial levels 1–3 treat hints as unlimited and do not count against stars, but the button is not shown yet.

---

## 5. Stars

On Level Complete:

| Hints used | Stars |
| --- | --- |
| 0 | 3 |
| 1 | 2 |
| 2+ | 1 |

Moves left do not change stars. Best stars per level are saved. There are no theme chests or extra skins — the live candy look is the only look.

Title art is a fixed **545×196** graphic, inset from the top of the card on every resolution. Next Level / Replay use Nunito Black via TextMeshPro, size 80. Hints-used copy is size 50.

---

## 6. Tutorial (levels 1–5 especially)

| Level | What it teaches | Aids |
| --- | --- | --- |
| 1–3 | Drag pieces onto highlighted cells | Coach text, target outline, looping finger-drag demo. Hint button hidden. |
| 4 | Undo, hint button appears | Undo pulses, then hint button pulses |
| 5 | Hint reminder | Hint button pulses; coach: tap the lightbulb |
| 7–10 | Rotation | TURN only on pieces that still need a turn; tap-hand on level 7 only |

Guided drag (levels 1–3): only the next unplaced piece is highlighted, with a looping ghost that travels from tray to the solution cell. The demo stops as soon as the player starts a real drag.

Coach banner can be tapped away. It comes back when the level is shown again.

---

## 7. Animation map (nothing should snap)

### Always on

- Hint bulb glow (header + popup) pulses scale and alpha.
- Win stars: sharp star on top, blur star behind at 105% size, glow pulses.
- Tutorial target outlines and the next tray piece pulse alpha.
- Buttons scale down on press (ease-out) and back on release.

### Overlay popups (Hint, Level Complete)

- **In:** overlay fades 0→1, card scales 0.84→1 with a slight overshoot (ease-out back).
- **Out:** overlay fades 1→0, card eases slightly down in scale, then hides.
- Next / Replay fade the win card out before loading the next (or same) level.

### Level start

- Tutorial levels 1–12: empty cells sit in on a diagonal wave, then locked blocks, then tray pieces slide in from the left. Placement is locked until that intro finishes.
- Main levels (13+): no boarding intro — the board is ready immediately.

### Rotate

- Tray tap or drag Rotate button: piece spins −90° in ~0.2s with a scale pulse, small spark, light haptic, then settles at the new size. No instant snap.

- Dragged piece **eases** to the snapped cell or back to origin (~0.16s, ease-out cubic).
- Successful place: Epic Toon FX spark explosion, squash, board punch, medium haptic. SFX later.

### Hint (single-step guide)

1. Header tap with hints left starts the guide immediately (no confirm card). Out of hints: shop popup only. Button is off during intro and hidden on levels 1–3.
2. Hint bulb flies from the header to screen center, pulses, and fades.
3. Brief pause if blockers were cleared.
4. Tray piece dims. If it needs a turn, it spins first. No target-cell highlight.
5. Piece lifts slowly from the tray (~0.4s), arcs to the cell (~0.78s), then sits onto the spot.
6. Spark on the locked piece, medium haptic. SFX later.
7. If the board is full, win popup starts **after** this motion.

### Level Complete

- Card pops in as above.
- Only earned stars appear. They fall from the camera one by one: middle, then left, then right. Center star is larger. Unearned slots stay empty — no faded ghosts.
- Each landing: toon burst using the game star art, squash, card punch, haptic. SFX later.
- Static confetti art is gone; rainbow confetti particles play when the card opens.

### Fit cheers

- A correct mid-level place can pop a random line (Nice!, Great!, Super!, Awesome!, Yes!).
- The last piece that fills the board does **not** show encourage text.
- Off-solution places: no text.
- Star glow keeps pulsing while the card is open.

### Reset confirm

- Confirm card pops in with the same overshoot scale.

---

## 8. What is locked vs free

| Piece state | Can drag | Can undo away | Hint can move it |
| --- | --- | --- | --- |
| Tray | Yes | — | Yes (places it) |
| Placed by player | Yes | Yes | Yes, if it blocks the hinted cell |
| Preplaced | No | No | No |
| Placed by hint | No | No | No (already correct) |

---

## 9. Haptics

Distinct pulses (editor / desktop: no-op):

- **Place** — click when a piece lands on the board
- **Intro chain** — light ticks as empty cells and locked blocks sit in
- **Miss** — error buzz when an invalid drop returns to the tray or old cell
- **Hint** — double-pulse when the hint bulb plays
- **Stars** — a pulse each time an earned star lands
- **Confetti** — no extra rumble; haptics follow the stars

iOS uses Core Haptics + UIKit generators. Android uses the Vibrator / VibratorManager APIs (`VIBRATE` permission). Tapping Next clears leftover star particles and stops rumble.

---

## 10. Technical notes

- Unity 6, URP 2D, uGUI + TextMeshPro.
- UI is built in code (`GameScreen`), not a prefab layout.
- Fonts: Nunito Black / ExtraBold / Bold / SemiBold, created as TMP assets at runtime.
- Portrait only, reference 1080×1920, CanvasScaler match width/height by aspect.
- Levels load from `Resources/Data/campaign.json`, which points at `tutorial_levels.json`, `levels.json`, and `special_levels.json`.

### Main scripts

| Script | Role |
| --- | --- |
| `GameScreen.cs` | UI, input, animation, popups |
| `GameState.cs` | Board rules, undo, hint, completion |
| `LevelModels.cs` | JSON shapes |
| `Haptics.cs` | Native vibrate |
| `SlabGraphic.cs` | Fallback beveled rects |

---

## 11. App shell (current candy UI, polish later)

- **Splash** then **Home**: Play / Continue, Levels, Daily (soon), Settings
- **Levels**: scroll of campaign tiles, sequential unlock, star counts
- **Settings**: haptics on/off, jump debug, hint shop stub
- **Look**: one default candy skin. No extra themes or chests.
- **Home** from out-of-moves / last-level Next returns to Home, not level 1
- Gear on the play screen opens Settings (Back resumes the level)

Still later: daily playlist, hint shop, world map art.
