using System;
using System.Collections.Generic;
using UnityEngine;

namespace Aura
{
    public class PlacedPiece
    {
        public string id;
        public int row;
        public int col;
        public int h;
        public int w;
        public bool locked;
    }

    public class UndoStep
    {
        public string id;
        public bool wasPlaced;
        public int row;
        public int col;
        public int h;
        public int w;
        public int trayTurns;
        public bool spentMove;
    }

    public class GameState
    {
        public const int Size = 8;
        static readonly string[] ColorA = { "A", "G" };
        static readonly string[] ColorB = { "B", "E" };
        static readonly string[] ColorC = { "C", "F" };
        static readonly string[] ColorD = { "D", "H" };

        public PieceDef[] pieces;
        public LevelDef level;
        public readonly Dictionary<string, PlacedPiece> placed = new Dictionary<string, PlacedPiece>();
        public readonly Dictionary<string, int> trayTurns = new Dictionary<string, int>();
        public readonly HashSet<string> preplacedIds = new HashSet<string>();
        public readonly List<BlockPlacement> black = new List<BlockPlacement>();
        public readonly Stack<UndoStep> undo = new Stack<UndoStep>();
        public int hintsLeft = 3;
        public int hintsUsed;
        public int moveLimit;
        public int movesLeft;
        bool tutorialPack;

        public bool IsTutorial
        {
            get { return tutorialPack; }
        }

        public bool IsColor
        {
            get { return level != null && level.type == "color"; }
        }

        public bool IsDecoy
        {
            get { return level != null && level.type == "decoy"; }
        }

        public bool IsRescue
        {
            get { return level != null && level.type == "rescue"; }
        }

        public bool HasMoveLimit
        {
            get { return moveLimit > 0; }
        }

        public bool OutOfMoves
        {
            get { return HasMoveLimit && movesLeft <= 0 && !IsComplete(); }
        }

        public bool RotationEnabled
        {
            get { return !IsTutorial || level == null || level.tutorial == null || level.tutorial.rotationEnabled; }
        }

        public bool FreeHints
        {
            get { return IsTutorial && level.id <= 3; }
        }

        public bool GuidedDrag
        {
            get { return IsTutorial && level.id <= 3; }
        }

        public string NextGuideId()
        {
            foreach (var id in TrayIds())
            {
                if (!IsPlaced(id))
                    return id;
            }
            return null;
        }

        public void Begin(LevelDef next, PieceDef[] catalog, bool tutorial = false)
        {
            pieces = catalog;
            level = next;
            tutorialPack = tutorial;
            placed.Clear();
            trayTurns.Clear();
            preplacedIds.Clear();
            black.Clear();
            undo.Clear();
            hintsLeft = int.MaxValue;
            hintsUsed = 0;
            moveLimit = next != null ? Math.Max(0, next.moveLimit) : 0;
            movesLeft = moveLimit;
            if (pieces != null)
            {
                foreach (var piece in pieces)
                    trayTurns[piece.id] = 0;
            }
            if (next != null && next.black != null)
                black.AddRange(next.black);
            if (next != null && next.preplaced != null)
            {
                foreach (var step in next.preplaced)
                    PlaceLocked(step);
            }
            PlaceInitial();
        }

        void PlaceLocked(SolutionPlacement step)
        {
            if (step == null || string.IsNullOrEmpty(step.piece))
                return;
            if (!TryPlace(step.piece, step.row, step.col, step.h, step.w, false))
                return;
            placed[step.piece].locked = true;
            preplacedIds.Add(step.piece);
        }

        void PlaceInitial()
        {
            if (level == null || level.initial == null)
                return;
            foreach (var step in level.initial)
            {
                if (step == null || string.IsNullOrEmpty(step.piece))
                    continue;
                TryPlace(step.piece, step.row, step.col, step.h, step.w, false);
            }
        }

        public PieceDef Find(string id)
        {
            if (pieces == null)
                return null;
            foreach (var piece in pieces)
            {
                if (piece.id == id)
                    return piece;
            }
            return null;
        }

        public IList<string> TrayIds()
        {
            var ids = new List<string>();
            var seen = new HashSet<string>();
            AddIds(ids, seen, level != null ? level.toPlace : null);
            if (level != null && level.initial != null)
            {
                foreach (var step in level.initial)
                {
                    if (step != null)
                        AddId(ids, seen, step.piece);
                }
            }
            if (level != null)
                AddId(ids, seen, level.unused);
            if (ids.Count == 0 && pieces != null)
            {
                foreach (var piece in pieces)
                    AddId(ids, seen, piece.id);
            }
            return ids;
        }

        static void AddIds(List<string> ids, HashSet<string> seen, string[] more)
        {
            if (more == null)
                return;
            foreach (var id in more)
                AddId(ids, seen, id);
        }

        static void AddId(List<string> ids, HashSet<string> seen, string id)
        {
            if (string.IsNullOrEmpty(id) || !seen.Add(id))
                return;
            ids.Add(id);
        }

        public bool HasAid(string name)
        {
            if (level == null || level.tutorial == null || level.tutorial.aids == null)
                return false;
            foreach (var aid in level.tutorial.aids)
            {
                if (aid == name)
                    return true;
            }
            return false;
        }

        public bool NeedsRotation(string id)
        {
            if (level == null || level.rotationNeeded == null)
                return false;
            foreach (var piece in level.rotationNeeded)
            {
                if (piece == id)
                    return true;
            }
            return false;
        }

        public bool CanRotate(string id)
        {
            if (!RotationEnabled || string.IsNullOrEmpty(id))
                return false;
            if (placed.ContainsKey(id) && placed[id].locked)
                return false;
            var def = Find(id);
            return def != null && def.h != def.w;
        }

        public bool CanRotateBoard(string id)
        {
            int row;
            int col;
            int h;
            int w;
            return FindBoardRotation(id, out row, out col, out h, out w);
        }

        public bool RotateBoard(string id)
        {
            int row;
            int col;
            int h;
            int w;
            if (!FindBoardRotation(id, out row, out col, out h, out w))
                return false;
            var piece = placed[id];
            PushUndo(id);
            placed[id] = new PlacedPiece
            {
                id = piece.id,
                row = row,
                col = col,
                h = h,
                w = w,
                locked = piece.locked
            };
            SyncTrayTurns(placed[id]);
            return true;
        }

        public bool FindBoardRotation(string id, out int row, out int col, out int h, out int w)
        {
            row = 0;
            col = 0;
            h = 0;
            w = 0;
            if (!IsPlaced(id) || placed[id].locked)
                return false;
            var def = Find(id);
            if (def == null || def.h == def.w)
                return false;
            var piece = placed[id];
            h = piece.w;
            w = piece.h;
            if (FitsBoard(id, piece.row, piece.col, h, w))
            {
                row = piece.row;
                col = piece.col;
                return true;
            }
            int best = 0;
            bool found = false;
            int rowMin = Mathf.Max(0, piece.row - h + 1);
            int rowMax = Mathf.Min(Size - h, piece.row + piece.h - 1);
            int colMin = Mathf.Max(0, piece.col - w + 1);
            int colMax = Mathf.Min(Size - w, piece.col + piece.w - 1);
            for (int r = rowMin; r <= rowMax; r++)
            {
                for (int c = colMin; c <= colMax; c++)
                {
                    if (!FitsBoard(id, r, c, h, w))
                        continue;
                    int overlapH = Mathf.Min(r + h, piece.row + piece.h) - Mathf.Max(r, piece.row);
                    int overlapW = Mathf.Min(c + w, piece.col + piece.w) - Mathf.Max(c, piece.col);
                    int overlap = Mathf.Max(0, overlapH) * Mathf.Max(0, overlapW);
                    if (overlap > best)
                    {
                        best = overlap;
                        row = r;
                        col = c;
                        found = true;
                    }
                }
            }
            return found;
        }

        public bool MustRotate(string id)
        {
            if (!CanRotate(id))
                return false;
            var solution = SolutionOf(id);
            if (solution == null)
                return false;
            int turns = trayTurns.ContainsKey(id) ? trayTurns[id] : 0;
            Oriented(id, turns, out int h, out int w);
            return h != solution.h || w != solution.w;
        }

        public SolutionPlacement SolutionOf(string id)
        {
            if (level == null || level.solution == null)
                return null;
            foreach (var step in level.solution)
            {
                if (step.piece == id)
                    return step;
            }
            return null;
        }

        public void Oriented(string id, int turns, out int h, out int w)
        {
            var piece = Find(id);
            if (piece == null)
            {
                h = 1;
                w = 1;
                return;
            }
            bool swap = (turns & 1) == 1;
            h = swap ? piece.w : piece.h;
            w = swap ? piece.h : piece.w;
        }

        public void SyncTrayTurns(PlacedPiece piece)
        {
            if (piece == null || string.IsNullOrEmpty(piece.id))
                return;
            var def = Find(piece.id);
            if (def == null)
                return;
            if (piece.h == def.h && piece.w == def.w)
                trayTurns[piece.id] = 0;
            else if (piece.h == def.w && piece.w == def.h)
                trayTurns[piece.id] = 1;
        }

        public bool MatchesSolution(string id)
        {
            if (string.IsNullOrEmpty(id) || !placed.ContainsKey(id))
                return false;
            var sol = SolutionOf(id);
            return sol != null && Matches(placed[id], sol);
        }

        public int PlayerPlacedCount()
        {
            int n = 0;
            foreach (var pair in placed)
            {
                if (!pair.Value.locked)
                    n++;
            }
            return n;
        }

        public bool IsPlaced(string id)
        {
            return placed.ContainsKey(id);
        }

        public bool IsPreplaced(string id)
        {
            return preplacedIds.Contains(id);
        }

        public bool HasMovablePlaced()
        {
            foreach (var piece in placed.Values)
            {
                if (!piece.locked)
                    return true;
            }
            return false;
        }

        public bool CanUndo
        {
            get { return undo.Count > 0; }
        }

        public UndoStep PeekUndo()
        {
            return undo.Count == 0 ? null : undo.Peek();
        }

        public List<string> UnlockedIds()
        {
            var ids = new List<string>();
            foreach (var pair in placed)
            {
                if (!pair.Value.locked)
                    ids.Add(pair.Key);
            }
            return ids;
        }

        public bool CanPlace(string id, int row, int col, int h, int w, bool requireMoves = true)
        {
            if (requireMoves && OutOfMoves)
                return false;
            if (!FitsBoard(id, row, col, h, w))
                return false;
            if (TouchesColorPartner(id, row, col, h, w))
                return false;
            return true;
        }

        public bool FitsBoard(string id, int row, int col, int h, int w)
        {
            if (row < 0 || col < 0 || row + h > Size || col + w > Size)
                return false;
            for (int r = row; r < row + h; r++)
            {
                for (int c = col; c < col + w; c++)
                {
                    if (CellOwner(r, c, id) != null)
                        return false;
                }
            }
            return true;
        }

        public bool BlockedOnlyByColor(string id, int row, int col, int h, int w)
        {
            return FitsBoard(id, row, col, h, w) && TouchesColorPartner(id, row, col, h, w);
        }

        public string ColorPartner(string id)
        {
            if (id == ColorA[0]) return ColorA[1];
            if (id == ColorA[1]) return ColorA[0];
            if (id == ColorB[0]) return ColorB[1];
            if (id == ColorB[1]) return ColorB[0];
            if (id == ColorC[0]) return ColorC[1];
            if (id == ColorC[1]) return ColorC[0];
            if (id == ColorD[0]) return ColorD[1];
            if (id == ColorD[1]) return ColorD[0];
            return null;
        }

        public bool TouchesColorPartner(string id, int row, int col, int h, int w)
        {
            if (!IsColor)
                return false;
            string partner = ColorPartner(id);
            if (string.IsNullOrEmpty(partner) || !placed.ContainsKey(partner))
                return false;
            var other = placed[partner];
            for (int r = row; r < row + h; r++)
            {
                for (int c = col; c < col + w; c++)
                {
                    if (AdjacentToRect(r, c, other.row, other.col, other.h, other.w))
                        return true;
                }
            }
            return false;
        }

        static bool AdjacentToRect(int r, int c, int row, int col, int h, int w)
        {
            bool left = c + 1 == col && r >= row && r < row + h;
            bool right = c - 1 == col + w - 1 && r >= row && r < row + h;
            bool up = r + 1 == row && c >= col && c < col + w;
            bool down = r - 1 == row + h - 1 && c >= col && c < col + w;
            return left || right || up || down;
        }

        public string CellOwner(int row, int col, string ignoreId)
        {
            foreach (var block in black)
            {
                if (row >= block.row && row < block.row + block.h && col >= block.col && col < block.col + block.w)
                    return "black";
            }
            foreach (var pair in placed)
            {
                if (pair.Key == ignoreId)
                    continue;
                var piece = pair.Value;
                if (row >= piece.row && row < piece.row + piece.h && col >= piece.col && col < piece.col + piece.w)
                    return pair.Key;
            }
            return null;
        }

        public bool TryPlace(string id, int row, int col, int h, int w, bool recordUndo, bool countMove = false)
        {
            if (!CanPlace(id, row, col, h, w, countMove))
                return false;
            if (recordUndo)
                PushUndo(id, countMove);
            placed[id] = new PlacedPiece { id = id, row = row, col = col, h = h, w = w };
            if (countMove)
                SpendMove();
            return true;
        }

        public void RestorePlaced(PlacedPiece piece)
        {
            if (piece == null || string.IsNullOrEmpty(piece.id))
                return;
            placed[piece.id] = new PlacedPiece
            {
                id = piece.id,
                row = piece.row,
                col = piece.col,
                h = piece.h,
                w = piece.w,
                locked = preplacedIds.Contains(piece.id)
            };
        }

        public void Remember(PlacedPiece piece, bool countMove = false)
        {
            if (piece == null)
                return;
            undo.Push(new UndoStep
            {
                id = piece.id,
                wasPlaced = true,
                row = piece.row,
                col = piece.col,
                h = piece.h,
                w = piece.w,
                trayTurns = trayTurns.ContainsKey(piece.id) ? trayTurns[piece.id] : 0,
                spentMove = countMove
            });
            if (countMove)
                SpendMove();
        }

        public void ReturnToTray(string id, bool recordUndo)
        {
            if (!placed.ContainsKey(id) || placed[id].locked)
                return;
            if (recordUndo)
                PushUndo(id);
            placed.Remove(id);
        }

        public bool RotateTray(string id)
        {
            if (placed.ContainsKey(id) || !CanRotate(id))
                return false;
            if (!trayTurns.ContainsKey(id))
                trayTurns[id] = 0;
            trayTurns[id] = (trayTurns[id] + 1) & 3;
            return true;
        }

        public void Undo()
        {
            if (undo.Count == 0)
                return;
            var step = undo.Pop();
            trayTurns[step.id] = step.trayTurns;
            if (step.spentMove && HasMoveLimit && movesLeft < moveLimit)
                movesLeft++;
            if (step.wasPlaced)
            {
                placed[step.id] = new PlacedPiece
                {
                    id = step.id,
                    row = step.row,
                    col = step.col,
                    h = step.h,
                    w = step.w,
                    locked = preplacedIds.Contains(step.id)
                };
            }
            else
            {
                placed.Remove(step.id);
            }
        }

        public void ResetUnlocked()
        {
            var ids = new List<string>(placed.Keys);
            foreach (var id in ids)
            {
                if (!placed[id].locked)
                    placed.Remove(id);
            }
            undo.Clear();
            movesLeft = moveLimit;
            PlaceInitial();
            if (pieces == null)
                return;
            foreach (var piece in pieces)
            {
                if (!placed.ContainsKey(piece.id))
                    trayTurns[piece.id] = 0;
            }
        }

        public bool IsComplete()
        {
            for (int r = 0; r < Size; r++)
            {
                for (int c = 0; c < Size; c++)
                {
                    if (CellOwner(r, c, null) == null)
                        return false;
                }
            }
            return true;
        }

        public SolutionPlacement PeekHint()
        {
            if (level == null || level.solution == null)
                return null;
            SolutionPlacement next = null;
            int bestArea = -1;
            string bestId = null;
            foreach (var step in level.solution)
            {
                if (placed.ContainsKey(step.piece) && Matches(placed[step.piece], step))
                    continue;
                var def = Find(step.piece);
                int area = def != null ? def.area : step.h * step.w;
                if (next == null || area > bestArea || (area == bestArea && string.CompareOrdinal(step.piece, bestId) < 0))
                {
                    next = step;
                    bestArea = area;
                    bestId = step.piece;
                }
            }
            if (next == null)
                return null;
            for (int r = next.row; r < next.row + next.h; r++)
            {
                for (int c = next.col; c < next.col + next.w; c++)
                {
                    var owner = CellOwner(r, c, next.piece);
                    if (owner != null && owner != "black" && placed.ContainsKey(owner) && placed[owner].locked)
                        return null;
                }
            }
            return next;
        }

        public bool PrepareHint(SolutionPlacement next)
        {
            if (next == null)
                return false;
            bool changed = false;
            var blockers = new HashSet<string>();
            for (int r = next.row; r < next.row + next.h; r++)
            {
                for (int c = next.col; c < next.col + next.w; c++)
                {
                    var owner = CellOwner(r, c, next.piece);
                    if (owner != null && owner != "black")
                        blockers.Add(owner);
                }
            }
            foreach (var blocker in blockers)
            {
                if (placed.ContainsKey(blocker) && placed[blocker].locked)
                    return false;
                ReturnToTray(blocker, true);
                changed = true;
            }
            if (placed.ContainsKey(next.piece))
            {
                ReturnToTray(next.piece, true);
                changed = true;
            }
            return changed;
        }

        public bool CommitHint(SolutionPlacement next)
        {
            if (next == null)
                return false;
            if (!TryPlace(next.piece, next.row, next.col, next.h, next.w, true))
                return false;
            placed[next.piece].locked = true;
            if (!FreeHints)
                hintsUsed++;
            return true;
        }

        public bool TryHint()
        {
            var next = PeekHint();
            if (next == null)
                return false;
            PrepareHint(next);
            return CommitHint(next);
        }

        void SpendMove()
        {
            if (HasMoveLimit && movesLeft > 0)
                movesLeft--;
        }

        static bool Matches(PlacedPiece piece, SolutionPlacement step)
        {
            return piece.row == step.row && piece.col == step.col && piece.h == step.h && piece.w == step.w;
        }

        void PushUndo(string id, bool spentMove = false)
        {
            var step = new UndoStep
            {
                id = id,
                trayTurns = trayTurns.ContainsKey(id) ? trayTurns[id] : 0,
                spentMove = spentMove
            };
            if (placed.ContainsKey(id))
            {
                var current = placed[id];
                step.wasPlaced = true;
                step.row = current.row;
                step.col = current.col;
                step.h = current.h;
                step.w = current.w;
            }
            undo.Push(step);
        }
    }
}
