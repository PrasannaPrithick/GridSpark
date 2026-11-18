#!/usr/bin/env python3
"""Build a 250-level GridAura campaign. Tutorial 1-12 stay in campaign.json as pointers."""

from __future__ import annotations

import json
import random
from pathlib import Path

SIZE = 8
PIECES = {
    "A": (1, 4, 4),
    "B": (1, 5, 5),
    "C": (2, 2, 4),
    "D": (3, 3, 9),
    "E": (2, 3, 6),
    "F": (2, 4, 8),
    "G": (2, 5, 10),
    "H": (3, 4, 12),
}
PAIRS = (("A", "G"), ("B", "E"), ("C", "F"), ("D", "H"))
ALL_IDS = list(PIECES)
ORDER = ["H", "G", "F", "D", "E", "B", "A", "C"]
FAMILIES = [
    "courtyard",
    "corners",
    "scatter",
    "plus",
    "ring",
    "splitv",
    "splith",
    "cluster",
    "corridor",
    "frame",
    "snake",
    "center",
]
WORLDS = [
    "Color Clash",
    "Odd One Out",
    "Rescue Mission",
    "Touch & Lock",
    "New Tricks",
    "Mix It Up",
    "Mix It Up II",
    "Soft Mastery",
    "Color Rooms",
    "Spare Parts",
    "Rescue Mix",
    "Borderlands",
    "Stone & Edge",
    "Island Hop",
    "Commit Path",
    "Big First",
    "Turn Limit",
    "Harmony",
    "Deep Mix",
    "Party Pack",
    "Grand Mix",
    "Star Road",
    "High Farm",
    "Grand Finale",
]
WAVE10 = ["easy", "medium", "easy", "medium", "hard", "easy", "medium", "easy", "hard", "boss"]
WAVE8 = ["easy", "medium", "easy", "medium", "hard", "easy", "hard", "boss"]
RULE_INTROS = [
    ("color", "New rule! Same colors can't touch edge to edge."),
    ("decoy", "New rule! One piece doesn't belong. Leave it in the tray."),
    ("rescue", "New rule! Some pieces are in the wrong place. Fix them."),
    ("musttouch", "New rule! Matching colors MUST touch."),
    ("keystone", "New rule! A locked piece is already on the board."),
    ("commit", "New rule! After you drop a piece, it stays."),
    ("rotatecap", "New rule! You only get a few turns."),
    ("bigfirst", "New rule! Place the biggest leftover piece first."),
    ("border", "New rule! Every piece must touch the board edge."),
    ("stonehug", "New rule! Every piece must touch a black block."),
    ("islands", "New rule! Stay inside a room. Don't bridge the walls."),
    ("doublespare", "New rule! Two pieces don't belong. Leave both."),
    ("noundo", "New rule! Undo is off. Think before you drop."),
]
TUTORIAL = [
    {"global": i, "source": "tutorial", "id": str(i), "tag": "learn", "moveLimit": 0, "par": 0, "world": 0, "worldName": "", "posInWorld": 0, "intro": False, "coachText": ""}
    for i in range(1, 13)
]


def orients(pid):
    h, w, _ = PIECES[pid]
    out = [(h, w)]
    if h != w:
        out.append((w, h))
    return out


def area(pid):
    return PIECES[pid][2]


def first_empty(grid):
    for i, v in enumerate(grid):
        if v == 0:
            return i
    return -1


def can_put(grid, r, c, h, w):
    if r < 0 or c < 0 or r + h > SIZE or c + w > SIZE:
        return False
    for i in range(h):
        for j in range(w):
            if grid[(r + i) * SIZE + c + j]:
                return False
    return True


def put(grid, r, c, h, w, val):
    for i in range(h):
        for j in range(w):
            grid[(r + i) * SIZE + c + j] = val


def rects_touch(r1, c1, h1, w1, r2, c2, h2, w2):
    for r in range(r1, r1 + h1):
        for c in range(c1, c1 + w1):
            if c + 1 == c2 and r2 <= r < r2 + h2:
                return True
            if c - 1 == c2 + w2 - 1 and r2 <= r < r2 + h2:
                return True
            if r + 1 == r2 and c2 <= c < c2 + w2:
                return True
            if r - 1 == r2 + h2 - 1 and c2 <= c < c2 + w2:
                return True
    return False


def touches_border(r, c, h, w):
    return r == 0 or c == 0 or r + h == SIZE or c + w == SIZE


def touches_black(black, r, c, h, w):
    for br, bc in black:
        if rects_touch(r, c, h, w, br, bc, 1, 1):
            return True
    return False


def partner(pid):
    for a, b in PAIRS:
        if pid == a:
            return b
        if pid == b:
            return a
    return None


NODES = 0
NODE_LIMIT = 12000


def solve(grid, remaining, placed, need_color, need_touch, need_border, rng):
    global NODES
    NODES += 1
    if NODES > NODE_LIMIT:
        return None
    if not remaining:
        return []
    idx = first_empty(grid)
    if idx < 0:
        return None
    er, ec = divmod(idx, SIZE)
    pieces = list(remaining)
    rng.shuffle(pieces)
    pieces.sort(key=lambda p: (-area(p), rng.random()))
    for pid in pieces:
        rest = [p for p in remaining if p != pid]
        other = placed.get(partner(pid))
        for h, w in orients(pid):
            rmin = max(0, er - h + 1)
            cmin = max(0, ec - w + 1)
            rmax = min(er, SIZE - h)
            cmax = min(ec, SIZE - w)
            spots = [
                (r, c)
                for r in range(rmin, rmax + 1)
                for c in range(cmin, cmax + 1)
                if r <= er < r + h and c <= ec < c + w
            ]
            rng.shuffle(spots)
            for r, c in spots:
                if not can_put(grid, r, c, h, w):
                    continue
                if need_border and not touches_border(r, c, h, w):
                    continue
                if other is not None:
                    touch = rects_touch(r, c, h, w, other["row"], other["col"], other["h"], other["w"])
                    if need_color and touch:
                        continue
                    if need_touch and not touch:
                        continue
                put(grid, r, c, h, w, 1)
                placed[pid] = {"piece": pid, "row": r, "col": c, "h": h, "w": w}
                got = solve(grid, rest, placed, need_color, need_touch, need_border, rng)
                if got is not None:
                    return [placed[pid]] + got
                del placed[pid]
                put(grid, r, c, h, w, 0)
    return None


def add_cells(cells, n, rng):
    n = max(0, min(64, n))
    while len(cells) < n:
        cells.add((rng.randrange(SIZE), rng.randrange(SIZE)))
    extra = list(cells)
    rng.shuffle(extra)
    while len(cells) > n:
        cells.discard(extra.pop())


def layout_cells(family, n, rng):
    cells = set()
    if family == "corners":
        cells |= {(0, 0), (0, 7), (7, 0), (7, 7)}
    elif family == "center":
        for r in range(3, 5):
            for c in range(3, 5):
                cells.add((r, c))
    elif family == "plus":
        for i in range(SIZE):
            cells.add((3, i))
            cells.add((i, 3))
    elif family == "frame":
        for i in range(SIZE):
            cells.add((0, i))
            cells.add((7, i))
            cells.add((i, 0))
            cells.add((i, 7))
    elif family == "ring":
        for r in range(2, 6):
            for c in range(2, 6):
                if r in (2, 5) or c in (2, 5):
                    cells.add((r, c))
    elif family == "splitv":
        col = rng.choice([3, 4])
        for r in range(SIZE):
            cells.add((r, col))
    elif family == "splith":
        row = rng.choice([3, 4])
        for c in range(SIZE):
            cells.add((row, c))
    elif family == "courtyard":
        for i in range(1, 7):
            cells.add((1, i))
            cells.add((6, i))
    elif family == "corridor":
        row = rng.randrange(2, 6)
        for c in range(SIZE):
            cells.add((row, c))
    elif family == "snake":
        r = c = 0
        cells.add((r, c))
        for _ in range(20):
            opts = [(r + 1, c), (r, c + 1), (r - 1, c), (r, c - 1)]
            rng.shuffle(opts)
            for nr, nc in opts:
                if 0 <= nr < SIZE and 0 <= nc < SIZE:
                    r, c = nr, nc
                    cells.add((r, c))
                    break
    elif family == "cluster":
        sr, sc = rng.randrange(0, 6), rng.randrange(0, 6)
        for r in range(sr, min(SIZE, sr + 3)):
            for c in range(sc, min(SIZE, sc + 3)):
                cells.add((r, c))
    else:
        while len(cells) < n:
            cells.add((rng.randrange(SIZE), rng.randrange(SIZE)))
    add_cells(cells, n, rng)
    return cells


def merge_black(cells):
    return [{"row": r, "col": c, "h": 1, "w": 1} for r, c in sorted(cells)]


def rotation_needed(solution):
    need = []
    for step in solution:
        h0, w0, _ = PIECES[step["piece"]]
        if step["h"] != h0 or step["w"] != w0:
            need.append(step["piece"])
    return need


def rotate_level(solution, black):
    def flip(r, c, h, w):
        return c, SIZE - r - h, w, h

    new_sol = []
    for s in solution:
        r, c, h, w = flip(s["row"], s["col"], s["h"], s["w"])
        new_sol.append({"piece": s["piece"], "row": r, "col": c, "h": h, "w": w})
    new_black = set()
    for r, c in black:
        nr, nc, _, _ = flip(r, c, 1, 1)
        new_black.add((nr, nc))
    return new_sol, new_black


def fingerprint(black, solution, kind, rules):
    b = tuple(sorted(black))
    s = tuple((p["piece"], p["row"], p["col"], p["h"], p["w"]) for p in sorted(solution, key=lambda x: x["piece"]))
    return (b, s, kind, tuple(rules or []))


def pick_unused(rng, count):
    pool = list(ALL_IDS)
    rng.shuffle(pool)
    return sorted(pool[:count], key=lambda p: -area(p))


def used_ids(unused):
    return [p for p in ALL_IDS if p not in unused]


def slack(world, tag):
    if world <= 6:
        table = {"easy": 8, "medium": 6, "hard": 5, "boss": 4, "learn": 0}
    elif world <= 14:
        table = {"easy": 7, "medium": 6, "hard": 5, "boss": 4, "learn": 0}
    else:
        table = {"easy": 6, "medium": 5, "hard": 4, "boss": 3, "learn": 0}
    return table[tag]


def world_of(global_id):
    if global_id <= 12:
        return 0, "", 0, 10
    n = global_id - 12
    if n <= 230:
        world = (n - 1) // 10 + 1
        pos = (n - 1) % 10 + 1
        return world, WORLDS[world - 1], pos, 10
    pos = n - 230
    return 24, WORLDS[23], pos, 8


def tag_of(pos, length):
    wave = WAVE10 if length == 10 else WAVE8
    return wave[pos - 1]


def make_grid(black):
    grid = [0] * 64
    for r, c in black:
        grid[r * SIZE + c] = 1
    return grid


def pairs_ok(solution, need_color, need_touch):
    by = {s["piece"]: s for s in solution}
    for a, b in PAIRS:
        if a not in by or b not in by:
            continue
        touch = rects_touch(by[a]["row"], by[a]["col"], by[a]["h"], by[a]["w"], by[b]["row"], by[b]["col"], by[b]["h"], by[b]["w"])
        if need_color and touch:
            return False
        if need_touch and not touch:
            return False
    return True


def leftover_black(grid):
    return {(i // SIZE, i % SIZE) for i, filled in enumerate(grid) if filled == 0}


def try_pack(ids, rng, need_color=False, need_touch=False, need_border=False, pre_black=None):
    global NODES
    NODES = 0
    grid = [0] * 64
    if pre_black:
        for r, c in pre_black:
            grid[r * SIZE + c] = 1
    placed = {}
    sol = solve(grid, list(ids), placed, need_color, need_touch, need_border, rng)
    if sol is None:
        return None, None
    covered = set()
    for step in sol:
        for i in range(step["h"]):
            for j in range(step["w"]):
                covered.add((step["row"] + i, step["col"] + j))
    black = {(r, c) for r in range(SIZE) for c in range(SIZE) if (r, c) not in covered}
    if not pairs_ok(sol, need_color, need_touch):
        return None, None
    if need_border and any(not touches_border(s["row"], s["col"], s["h"], s["w"]) for s in sol):
        return None, None
    return sol, black


def rescue_from(solution, rng, black):
    pieces = [s["piece"] for s in solution]
    rng.shuffle(pieces)
    tray = pieces[:2]
    rest = pieces[2:]
    wrong = rest[:1]
    initial = []
    by = {s["piece"]: s for s in solution}
    occupied = {(s["row"] + i, s["col"] + j) for s in solution for i in range(s["h"]) for j in range(s["w"]) if s["piece"] not in tray}
    occupied |= set(black)
    holes = []
    for r in range(SIZE):
        for c in range(SIZE):
            if (r, c) not in occupied:
                holes.append((r, c))
    for pid in rest:
        step = dict(by[pid])
        if pid in wrong:
            h, w = step["h"], step["w"]
            rng.shuffle(holes)
            moved = False
            for r, c in holes:
                if r + h > SIZE or c + w > SIZE:
                    continue
                ok = True
                for i in range(h):
                    for j in range(w):
                        if (r + i, c + j) not in holes and (r + i, c + j) not in {(step["row"] + ii, step["col"] + jj) for ii in range(h) for jj in range(w)}:
                            # hole list is empty cells after removing tray; also allow original cells of this piece
                            pass
                cells = {(r + i, c + j) for i in range(h) for j in range(w)}
                other = occupied - {(by[pid]["row"] + i, by[pid]["col"] + j) for i in range(by[pid]["h"]) for j in range(by[pid]["w"])}
                if cells & other:
                    continue
                if r == step["row"] and c == step["col"]:
                    continue
                step["row"], step["col"] = r, c
                moved = True
                break
            if not moved:
                wrong = []
        initial.append(step)
    return tray, initial, wrong


def build_level(global_id, kind, family, rules, rng, unused=None):
    unused = unused or []
    ids = used_ids(unused)
    black_n = 64 - sum(area(p) for p in ids)
    need_color = kind == "color" or "color" in rules
    need_touch = kind == "musttouch" or "musttouch" in rules
    need_border = kind == "border" or "border" in rules
    need_hug = kind == "stonehug" or "stonehug" in rules
    sol = None
    black = set()
    for _ in range(60):
        pre = set()
        if kind == "islands":
            pre = layout_cells(rng.choice(["splitv", "splith"]), black_n, rng)
        else:
            pre.add((rng.randrange(SIZE), rng.randrange(SIZE)))
            if rng.randrange(2) == 0:
                pre.add((rng.randrange(SIZE), rng.randrange(SIZE)))
        sol, black = try_pack(ids, rng, need_color, need_touch, need_border, pre)
        if not sol:
            continue
        if need_hug and any(not touches_black(black, s["row"], s["col"], s["h"], s["w"]) for s in sol):
            sol = None
            continue
        break
    if sol is None:
        kind = "classic"
        rules = [r for r in rules if r not in ("color", "musttouch", "border", "stonehug", "islands")]
        sol, black = try_pack(ids, rng)
    if sol is None:
        return None
    to_place = list(ids)
    initial = None
    preplaced = None
    wrong = None
    if kind == "keystone" or "keystone" in rules:
        sol_sorted = sorted(sol, key=lambda s: -s["h"] * s["w"])
        lock = sol_sorted[0]
        preplaced = [lock]
        to_place = [s["piece"] for s in sol if s["piece"] != lock["piece"]]
    elif kind == "rescue":
        tray, initial, wrong = rescue_from(sol, rng, black)
        to_place = tray
        if not wrong:
            kind = "classic"
            initial = None
            to_place = list(ids)
    level = {
        "id": "P%03d" % global_id,
        "type": kind if kind not in ("commit", "rotatecap", "bigfirst", "noundo", "stonehug") else "classic",
        "rules": rules,
        "black": merge_black(black),
        "toPlace": to_place + unused,
        "solution": sol,
        "rotationNeeded": rotation_needed(sol),
    }
    if unused:
        level["unused"] = unused[0]
        if len(unused) > 1:
            level["unusedPieces"] = unused
        if kind == "doublespare":
            level["type"] = "doublespare"
        elif kind == "decoy" or len(unused) == 1:
            level["type"] = "decoy"
    if initial:
        level["initial"] = initial
        level["wrongPieces"] = wrong
        level["type"] = "rescue"
    if preplaced:
        level["preplaced"] = preplaced
        level["type"] = "keystone"
    if "rotatecap" in rules:
        need = max(2, len(level.get("rotationNeeded") or []))
        level["rotateCap"] = max(need, 4 if global_id < 80 else 3 if global_id < 160 else 2)
    return level


def choose_kind(global_id, tag, taught, intro_index):
    spotlight = (global_id - 13) % 3 == 0
    intro = False
    coach = ""
    kind = "classic"
    rules = []
    unused_n = 0
    if global_id == 13:
        return "classic", [], 0, True, "New pack! Fill every empty cell.", intro_index
    if spotlight and intro_index < len(RULE_INTROS) and tag != "boss":
        kind, coach = RULE_INTROS[intro_index]
        intro = True
        taught.add(kind)
        intro_index += 1
        if kind in ("commit", "rotatecap", "bigfirst", "noundo", "stonehug"):
            rules = [kind]
        if kind == "decoy":
            unused_n = 1
        if kind == "doublespare":
            unused_n = 2
        return kind, rules, unused_n, intro, coach, intro_index
    known = [k for k, _ in RULE_INTROS if k in taught]
    if known and tag != "easy":
        pick = known[(global_id * 3) % len(known)]
        kind = pick
        if pick in ("commit", "rotatecap", "bigfirst", "noundo", "stonehug"):
            kind = "classic"
            rules = [pick]
        if pick == "decoy":
            unused_n = 1
        if pick == "doublespare":
            unused_n = 2
        # keep it candy-crush easy: never stack two pressure rules on easy, and only one extra on hard/boss after world 8
        if tag == "boss" and "commit" in taught and pick not in ("commit", "noundo"):
            if "noundo" not in rules and global_id > 90 and pick != "rescue":
                pass
    elif known:
        kind = "classic"
        if "color" in taught and global_id % 7 == 0:
            kind = "color"
    return kind, rules, unused_n, intro, coach, intro_index


def main():
    rng = random.Random(250250)
    seen = set()
    pack = []
    playlist = list(TUTORIAL)
    taught = set()
    intro_index = 0
    for global_id in range(13, 251):
        world, world_name, pos, length = world_of(global_id)
        tag = tag_of(pos, length)
        family = FAMILIES[(global_id + pos) % len(FAMILIES)]
        kind, rules, unused_n, intro, coach, intro_index = choose_kind(global_id, tag, taught, intro_index)
        unused = pick_unused(rng, unused_n) if unused_n else []
        level = None
        for attempt in range(80):
            fam = family if attempt == 0 else FAMILIES[attempt % len(FAMILIES)]
            cand = build_level(global_id, kind, fam, list(rules), rng, unused)
            if not cand:
                continue
            black_cells = [(b["row"], b["col"]) for b in cand["black"]]
            sol = cand["solution"]
            if not cand.get("wrongPieces"):
                for _ in range(rng.randrange(4)):
                    sol, black_set = rotate_level(sol, set(black_cells))
                    black_cells = sorted(black_set)
                cand["solution"] = sol
                cand["black"] = merge_black(black_cells)
                if cand.get("preplaced"):
                    by = {s["piece"]: s for s in sol}
                    cand["preplaced"] = [by[s["piece"]] for s in cand["preplaced"] if s["piece"] in by]
            fp = fingerprint(black_cells, cand["solution"], cand["type"], cand.get("rules"))
            if fp in seen:
                continue
            seen.add(fp)
            level = cand
            break
        if level is None:
            unused = []
            level = build_level(global_id, "classic", "scatter", [], rng, [])
            fp = fingerprint([(b["row"], b["col"]) for b in level["black"]], level["solution"], level["type"], [])
            seen.add(fp)
        if intro:
            level["intro"] = True
            level["coachText"] = coach
        if "rotatecap" in (level.get("rules") or []) and not level.get("rotateCap"):
            level["rotateCap"] = 4
        used = [s["piece"] for s in level["solution"]]
        par = len(level.get("toPlace") or used)
        if level.get("wrongPieces"):
            par = len(level.get("toPlace") or []) + len(level["wrongPieces"])
        if level.get("preplaced"):
            par = len(level.get("toPlace") or [])
        par = max(3, par)
        move_limit = par + slack(world, tag)
        pack.append(level)
        playlist.append(
            {
                "global": global_id,
                "source": "pack",
                "id": level["id"],
                "tag": tag,
                "par": par,
                "moveLimit": move_limit,
                "world": world,
                "worldName": world_name,
                "posInWorld": pos,
                "intro": intro,
                "coachText": coach,
            }
        )

    root = Path(__file__).resolve().parents[1]
    data = root / "GridAura" / "Assets" / "Resources" / "Data"
    campaign = {"note": "250 levels. Tutorial 1-12 frozen. New thing every 3 campaign levels. Candy Crush wave, gentle slack.", "worldNames": WORLDS, "levels": playlist}
    (data / "campaign.json").write_text(json.dumps(campaign, indent=1) + "\n")
    (data / "campaign_levels.json").write_text(json.dumps({"levels": pack}, indent=1) + "\n")
    kinds = {}
    for lvl in pack:
        kinds[lvl["type"]] = kinds.get(lvl["type"], 0) + 1
    intros = sum(1 for e in playlist if e.get("intro"))
    print("wrote", len(playlist), "campaign rows,", len(pack), "pack levels")
    print("types", kinds, "intros", intros, "unique", len(seen))


if __name__ == "__main__":
    main()
