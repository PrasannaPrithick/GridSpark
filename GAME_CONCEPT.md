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

**Playable pack today:** 12 tutorial levels + 50 main levels = **62 sequential levels**. Daily levels exist as data (25) but are not in the live play loop yet.

---

## 3. Screen layout (portrait 1080×1920)

Top to bottom:

1. **Hint button** (left) with bulb + infinity badge  
2. **Level pill** (center) — “Level” + number  
3. **Settings gear** (right)  
4. **Coach banner** (tutorial only) — tap to dismiss  
5. **Board** — 8×8 inside a frame, with row/column numbers  
6. **Tray** — remaining pieces (one or two rows)  
7. **Undo** and **Reset** candy buttons  
8. **Message line** — short feedback  

During a drag, a **Rotate** button can appear if that piece must be turned to match the solution.

Popups sit on a dim overlay:

- **Hint** — “Need a Hint?” + Use Hint / Get More Hints  
- **Level Complete** — title art, 3 stars, hints used, Next Level, Replay  
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

Tap a placed (unlocked) piece with no drag to send it back to the tray.

### Rotate

Square pieces do not rotate. Non-square pieces that need a different orientation:

- Tap the tray piece (tutorial), or
- Use the Rotate button while dragging.

Each tap turns 90°. A “TURN” badge can appear on the tray slot.

### Undo

Reverts the last recorded move (place, rotate-related tray change, or return to tray).

### Reset

If any unlocked pieces are on the board, a confirm card appears. Confirming clears unlocked pieces and the undo stack. Locked / preplaced pieces stay.

### Hint

1. Tap the header hint button → hint popup scales in.
2. Tap **Use Hint**.
3. Popup fades out.
4. The next unsolved solution piece is chosen (largest area first).
5. Wrong pieces sitting on that target return to the tray if they are not locked.
6. Target cells glow yellow.
7. The piece **flies in an arc** from the tray to the cell, grows slightly in mid-air, then punches on landing.
8. That piece is **locked** (hint badge on the corner). You cannot move it again.
9. If that placement finishes the board, the win popup plays after the punch.

Tutorial levels 1–3 treat hints as unlimited and do not count against stars.

---

## 5. Stars

On Level Complete:

| Hints used | Stars |
| --- | --- |
| 0 | 3 |
| 1 | 2 |
| 2+ | 1 |

Title art is a fixed **545×196** graphic, inset from the top of the card on every resolution. Next Level / Replay use Nunito Black via TextMeshPro, size 80. Hints-used copy is size 50.

---

## 6. Tutorial (levels 1–5 especially)

| Level | What it teaches | Aids |
| --- | --- | --- |
| 1–3 | Drag pieces onto highlighted cells | Coach text, target outline, looping finger-drag demo, free hints |
| 4 | Undo | Undo button pulses once |
| 5 | Hint | Hint button pulses once |
| Later tutorial | Rotation, less hand-holding | TURN badge, rotate arrow, coach |

Guided drag (levels 1–5): only the next unplaced piece is highlighted, with a looping ghost that travels from tray to the solution cell. The demo stops as soon as the player starts a real drag.

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

- Empty cells sit in on a diagonal wave (drop from above, squash, settle).
- Locked / preplaced blocks sit in after the empty cells.
- Placement is locked and the tutorial hand stays hidden until the intro finishes.
- Tray pieces wait until the board has settled, then slide in from the left at full size (fade + side arc, not the board sit).

### Place / undo / fail drop

- Dragged piece **eases** to the snapped cell or back to origin (~0.16s, ease-out cubic).
- Successful place: impact squash, expanding shockwave ring, board punch, medium haptic. SFX later.

### Hint (single-step guide)

1. Hide popup (fade).
2. Brief pause if blockers were cleared.
3. Tray piece dims.
4. Target outline appears and pulses.
5. Piece flies tray → board on an arc (~0.42s).
6. Land impact (squash, shockwave, medium haptic). SFX later.
7. Piece locks with a hint bulb badge.
8. If the board is full, win popup starts **after** this motion.

### Level Complete

- Card pops in as above.
- Three stars fall from the camera onto the card one by one: middle, then left, then right. Center star is larger.
- Each landing: squash, card punch, heavy haptic if earned. Unearned stars stay dim. SFX later.
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

Light tap on:

- Successful place
- Hint landing
- Each earned star pop

iOS uses a native plugin. Android uses `performHapticFeedback`. Editor / desktop: no-op.

---

## 10. Technical notes

- Unity 6, URP 2D, uGUI + TextMeshPro.
- UI is built in code (`GameScreen`), not a prefab layout.
- Fonts: Nunito Black / ExtraBold / Bold / SemiBold, created as TMP assets at runtime.
- Portrait only, reference 1080×1920, CanvasScaler match width/height by aspect.
- Levels load from `Resources/Data/tutorial_levels.json` then `levels.json`.

### Main scripts

| Script | Role |
| --- | --- |
| `GameScreen.cs` | UI, input, animation, popups |
| `GameState.cs` | Board rules, undo, hint, completion |
| `LevelModels.cs` | JSON shapes |
| `Haptics.cs` | Native vibrate |
| `SlabGraphic.cs` | Fallback beveled rects |

---

## 11. Not in the live loop yet

- Daily challenge playlist (`daily_levels.json` is on disk)
- Settings gear action
- Hint shop (“Get More Hints”)
- Hint count other than unlimited (badge shows ∞)
