# GridAura — Full Gameplay Document

This is the complete picture of the game as it exists now: what it is, how the grid works, every rule, the campaign, the UI, and how those pieces were added from the first packing puzzle through the live 92-level loop.

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
- extra rules (color clash, decoy, rescue)
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
| `Resources/Data/campaign.json` | Live 92-level playlist |
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

## 8. Campaign (levels 13–92)

After the tutorial, `campaign.json` plays **80 more levels** in 8 worlds of 10. Wave inside each world:

**easy, medium, easy, medium, hard, easy, medium, easy, hard, boss**  
(short: **EMEMHEMEHB**)

The level after a boss is always easy. Hard and boss tint the level number and can flash a banner. Campaign levels skip the tutorial sit-in; instead a **briefing card** (see UI) names the type and move count.

New rules start **early**, not at the end of the pack:

| First time | Global | Type |
| --- | --- | --- |
| Color Clash rule | 15 | Color (`C1`) |
| Odd One Out rule | 23 | Decoy (`D1`) |
| Rescue rule | 33 | Rescue (`R1`) |

### Worlds

| World | Levels | Name | Mix |
| --- | --- | --- | --- |
| 0 | 1–12 | Tutorial | Learn to place, undo, hint, rotate |
| 1 | 13–22 | Color Clash | 13–14 classic, then color from 15 |
| 2 | 23–32 | Odd One Out | Decoy + classic |
| 3 | 33–42 | Rescue Mission | Rescue + classic |
| 4 | 43–52 | Mix It Up | Color, decoy, rescue, classic |
| 5 | 53–62 | Mix It Up II | Mixed |
| 6 | 63–72 | Mastery | Mixed |
| 7 | 73–82 | Deep Pack | Classic leftovers |
| 8 | 83–92 | Grand Finale | Hardest classic pack |

Counts in the live playlist: **12 tutorial + 50 classic + 10 color + 10 decoy + 10 rescue = 92**.

### Move limit

From level 13, each drop onto the board costs **1 move**.

`moveLimit = par + slack`

| Tag | Typical par | Slack | Typical limit |
| --- | --- | --- | --- |
| easy | 8 | +6 | 14 |
| medium | 8 | +4 | 12 |
| hard | 8 | +3 | 11 |
| boss | 8 | +2 | 10 |

Decoy par is 7 (seven placements). Rescue par is lower (4–6) because several pieces already sit on the board.

**Does not spend a move:** rotate, return to tray, hint, undo (undo **refunds** if the undone step was a drop), reset (restores the full count).

At 0 moves with a gap still open: **Out of moves** popup — Retry / Home.

### Full playlist

`src` is the JSON pool. `id` is the level inside that pool.

| # | World | Tag | Type | Src | Id | Moves |
| --- | ---: | --- | --- | --- | --- | ---: |
| 1–12 | Tutorial | learn | classic | tutorial | 1–12 | none |
| 13 | Color Clash | easy | classic | main | 1 | 14 |
| 14 | Color Clash | medium | classic | main | 7 | 12 |
| 15 | Color Clash | easy | **color** intro | color | C1 | 14 |
| 16 | Color Clash | medium | classic | main | 11 | 12 |
| 17 | Color Clash | hard | classic | main | 25 | 11 |
| 18 | Color Clash | easy | color | color | C2 | 14 |
| 19 | Color Clash | medium | classic | main | 12 | 12 |
| 20 | Color Clash | easy | color | color | C3 | 14 |
| 21 | Color Clash | hard | color | color | C7 | 11 |
| 22 | Color Clash | boss | classic | main | 37 | 10 |
| 23 | Odd One Out | easy | **decoy** intro | decoy | D1 | 13 |
| 24 | Odd One Out | medium | classic | main | 15 | 12 |
| 25 | Odd One Out | easy | decoy | decoy | D2 | 13 |
| 26 | Odd One Out | medium | classic | main | 16 | 12 |
| 27 | Odd One Out | hard | classic | main | 32 | 11 |
| 28 | Odd One Out | easy | decoy | decoy | D3 | 13 |
| 29 | Odd One Out | medium | classic | main | 17 | 12 |
| 30 | Odd One Out | easy | classic | main | 6 | 14 |
| 31 | Odd One Out | hard | decoy | decoy | D7 | 10 |
| 32 | Odd One Out | boss | classic | main | 41 | 10 |
| 33 | Rescue Mission | easy | **rescue** intro | rescue | R1 | 10 |
| 34 | Rescue Mission | medium | classic | main | 21 | 12 |
| 35 | Rescue Mission | easy | rescue | rescue | R2 | 10 |
| 36 | Rescue Mission | medium | classic | main | 22 | 12 |
| 37 | Rescue Mission | hard | classic | main | 36 | 11 |
| 38 | Rescue Mission | easy | rescue | rescue | R3 | 10 |
| 39 | Rescue Mission | medium | classic | main | 23 | 12 |
| 40 | Rescue Mission | easy | classic | main | 10 | 14 |
| 41 | Rescue Mission | hard | rescue | rescue | R7 | 8 |
| 42 | Rescue Mission | boss | classic | main | 46 | 10 |
| 43 | Mix It Up | easy | color | color | C4 | 14 |
| 44 | Mix It Up | medium | classic | main | 26 | 12 |
| 45 | Mix It Up | easy | decoy | decoy | D4 | 13 |
| 46 | Mix It Up | medium | classic | main | 27 | 12 |
| 47 | Mix It Up | hard | rescue | rescue | R9 | 8 |
| 48 | Mix It Up | easy | classic | main | 14 | 14 |
| 49 | Mix It Up | medium | color | color | C8 | 12 |
| 50 | Mix It Up | easy | decoy | decoy | D5 | 13 |
| 51 | Mix It Up | hard | classic | main | 40 | 11 |
| 52 | Mix It Up | boss | rescue | rescue | R10 | 8 |
| 53 | Mix It Up II | easy | decoy | decoy | D6 | 13 |
| 54 | Mix It Up II | medium | classic | main | 34 | 12 |
| 55 | Mix It Up II | easy | rescue | rescue | R4 | 10 |
| 56 | Mix It Up II | medium | classic | main | 35 | 12 |
| 57 | Mix It Up II | hard | color | color | C9 | 11 |
| 58 | Mix It Up II | easy | classic | main | 18 | 14 |
| 59 | Mix It Up II | medium | decoy | decoy | D8 | 11 |
| 60 | Mix It Up II | easy | rescue | rescue | R5 | 10 |
| 61 | Mix It Up II | hard | classic | main | 45 | 11 |
| 62 | Mix It Up II | boss | color | color | C10 | 10 |
| 63 | Mastery | easy | rescue | rescue | R6 | 11 |
| 64 | Mastery | medium | classic | main | 38 | 12 |
| 65 | Mastery | easy | color | color | C5 | 14 |
| 66 | Mastery | medium | classic | main | 39 | 12 |
| 67 | Mastery | hard | decoy | decoy | D9 | 10 |
| 68 | Mastery | easy | classic | main | 24 | 14 |
| 69 | Mastery | medium | rescue | rescue | R8 | 9 |
| 70 | Mastery | easy | color | color | C6 | 14 |
| 71 | Mastery | hard | classic | main | 47 | 11 |
| 72 | Mastery | boss | decoy | decoy | D10 | 9 |
| 73 | Deep Pack | easy | classic | main | 2 | 14 |
| 74 | Deep Pack | medium | classic | main | 8 | 12 |
| 75 | Deep Pack | easy | classic | main | 3 | 14 |
| 76 | Deep Pack | medium | classic | main | 9 | 12 |
| 77 | Deep Pack | hard | classic | main | 19 | 11 |
| 78 | Deep Pack | easy | classic | main | 4 | 14 |
| 79 | Deep Pack | medium | classic | main | 13 | 12 |
| 80 | Deep Pack | easy | classic | main | 5 | 14 |
| 81 | Deep Pack | hard | classic | main | 20 | 11 |
| 82 | Deep Pack | boss | classic | main | 33 | 10 |
| 83 | Grand Finale | easy | classic | main | 28 | 14 |
| 84 | Grand Finale | medium | classic | main | 42 | 12 |
| 85 | Grand Finale | easy | classic | main | 29 | 14 |
| 86 | Grand Finale | medium | classic | main | 43 | 12 |
| 87 | Grand Finale | hard | classic | main | 48 | 11 |
| 88 | Grand Finale | easy | classic | main | 30 | 14 |
| 89 | Grand Finale | medium | classic | main | 44 | 12 |
| 90 | Grand Finale | easy | classic | main | 31 | 14 |
| 91 | Grand Finale | hard | classic | main | 49 | 11 |
| 92 | Grand Finale | boss | classic | main | 50 | 10 |

---

## 9. Game types

Every level is one of four types. The briefing card names it at the start of campaign levels.

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

Illegal drop: ghost turns coral, toast **Same colors can't touch**. First seen at **level 15 (C1)** with coach: *New rule! Pieces of the same color can't touch edge to edge.*

All 10 color levels (`C1`–`C10`) put A–H in the tray with 3 black regions, same as a classic pack, but the unique solution depends on the touch rule.

### Odd One Out (`type: decoy`)

All **8 pieces** are in the tray. Only **7 fit**. Extra black (typically 4 regions) so area does not allow the eighth. Leave the spare. The board is complete with one piece left in the tray.

Label: **Use 7 of 8 pieces.** First seen at **level 23 (D1)** with coach: *One piece doesn't belong. Leave it in the tray.*

Spares by level: D1 H, D2 H, D3 G, D4 D, D5 F, D6 D, D7 A, D8 C, D9 G, D10 C.

### Rescue (`type: rescue`)

Some pieces start **on the board, unlocked**. A listed subset (`wrongPieces`) is in the **wrong place**. The rest of `initial` is already correct but still movable. A few pieces start in the tray (`toPlace`). Reset restores that starting arrangement, not an empty board.

First seen at **level 33 (R1)** with coach: *These pieces are in the wrong place. Move them to finish.* Wrong pieces wiggle once on that intro.

| Id | Wrong | Already on board | In tray |
| --- | --- | --- | --- |
| R1 | B | 5 | A, E, F |
| R2 | B | 5 | A, F, H |
| R3 | A | 5 | E, F, H |
| R4 | C, E | 6 | D, F |
| R5 | E, H | 6 | D, G |
| R6 | B, C | 5 | A, E, H |
| R7 | E, F | 5 | D, G, H |
| R8 | A, C, E | 6 | D, H |
| R9 | B, E, H | 6 | C, D |
| R10 | A, B, D | 5 | C, E, H |

### Types discussed, not built

Must-touch, big-first, cornered, frozen pieces, rotate budget, timed levels. Not in the live loop.

---

## 10. Stars, chests, save data

On **Level Complete**, stars come only from **hints used** (on that attempt). Moves left do not change stars.

| Hints used | Stars |
| ---: | ---: |
| 0 | 3 |
| 1 | 2 |
| 2+ | 1 |

Best stars per **global** level id are stored in PlayerPrefs (`GridAura.BestStars.{n}`). The win card shows the lifetime total and how many stars remain until the next chest.

| Stars | Chest name |
| ---: | --- |
| 25 | Sunset Board |
| 50 | Ocean Pieces |
| 75 | Chime Tiles |
| 100 | Forest Board |
| 150 | Candy Pieces |
| 200 | Pop Tiles |
| 250 | Midnight Board |

Home → **Themes** equips an unlocked chest. That tints the board, empty tiles, or pieces (Sunset warm, Ocean cooler pieces, Forest green frame, Midnight dark tiles, and so on). Full art packs can replace these tints later.

Other prefs: `GridAura.ChestsClaimed`, `GridAura.WorldSeen.{world}`, `GridAura.LastLevel`, `GridAura.Theme`, `GridAura.Haptics`.

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
- **Level Complete** — title art 545×196, earned stars only (no faded ghosts), hints used, star total / next chest, Next Level, Replay. Rainbow confetti on open.
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

`campaign.json` became the playlist. Candy Crush wave (EMEMHEMEHB). Eight named worlds. Hard/boss banners. Move limits from 13. Out-of-moves fail state. Stars from hints. Star chests (names only).

### 6. Three extra types, introduced early

Color at 15, decoy at 23, rescue at 33 — not buried at level 80. Mix worlds 4–6, then Deep Pack and Grand Finale return to pure classic.

### 7. HUD players can actually see

Moves + stars used to live only as a thin bar at the top (and a ★ glyph that did not draw). Now: briefing card in the **middle** (type + moves), flies to a LevelPill HUD with the **StarClear sprite**.

### 8. App shell now, polish later

Splash → Home (Play/Continue, Levels, Daily soon, Themes, Settings). Sequential level select. Settings: haptics, jump, shop stub. Themes apply chest tints. Out-of-moves Home and last-level Next return to Home.

Still later: daily playlist, hint shop, world-map art, full theme packs, extra types.

---

## 15. One-screen rules (player facing)

Cover every cream cell. Black is stone. Drag pieces from the tray. Tap to turn a long piece. Same colors must not touch on Color Clash levels. One extra piece is a fake on Odd One Out — leave it. Rescue levels start messy; fix them. From level 13 you have a move budget; dropping onto the board spends one. Hints lock a piece in the right place and cost stars. Three stars = no hints.
