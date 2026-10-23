using System;
using System.Collections.Generic;

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
    }

    public class GameState
    {
        public const int Size = 8;
        public PieceDef[] pieces;
        public LevelDef level;
        public readonly Dictionary<string, PlacedPiece> placed = new Dictionary<string, PlacedPiece>();
        public readonly Dictionary<string, int> trayTurns = new Dictionary<string, int>();
        public readonly HashSet<string> preplacedIds = new HashSet<string>();
        public readonly List<BlockPlacement> black = new List<BlockPlacement>();
        public readonly Stack<UndoStep> undo = new Stack<UndoStep>();
        public int hintsLeft = 3;
        public int hintsUsed;

        public bool IsTutorial
        {
            get { return level != null && level.tutorial != null; }
        }

        public bool RotationEnabled
        {
            get { return level == null || level.tutorial == null || level.tutorial.rotationEnabled; }
        }

        public bool FreeHints
        {
            get { return IsTutorial && level.id <= 3; }
        }

        public bool GuidedDrag
        {
            get { return IsTutorial && level.id <= 5; }
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

        public void Begin(LevelDef next, PieceDef[] catalog)
        {
            pieces = catalog;
            level = next;
            placed.Clear();
            trayTurns.Clear();
            preplacedIds.Clear();
            black.Clear();
            undo.Clear();
            hintsLeft = int.MaxValue;
            hintsUsed = 0;
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
                {
                    if (step == null || string.IsNullOrEmpty(step.piece))
                        continue;
                    if (!TryPlace(step.piece, step.row, step.col, step.h, step.w, false))
                        continue;
                    placed[step.piece].locked = true;
                    preplacedIds.Add(step.piece);
                }
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
            if (level != null && level.toPlace != null && level.toPlace.Length > 0)
            {
                ids.AddRange(level.toPlace);
                return ids;
            }
            if (pieces != null)
            {
                foreach (var piece in pieces)
                    ids.Add(piece.id);
            }
            return ids;
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

        public bool MustRotate(string id)
        {
            if (!RotationEnabled || string.IsNullOrEmpty(id) || placed.ContainsKey(id))
                return false;
            var def = Find(id);
            if (def == null || def.h == def.w)
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
            bool swap = (turns & 1) == 1;
            h = swap ? piece.w : piece.h;
            w = swap ? piece.h : piece.w;
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

        public bool CanPlace(string id, int row, int col, int h, int w)
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

        public bool TryPlace(string id, int row, int col, int h, int w, bool recordUndo)
        {
            if (!CanPlace(id, row, col, h, w))
                return false;
            if (recordUndo)
                PushUndo(id);
            placed[id] = new PlacedPiece { id = id, row = row, col = col, h = h, w = w };
            return true;
        }

        public void Remember(PlacedPiece piece)
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
                trayTurns = trayTurns.ContainsKey(piece.id) ? trayTurns[piece.id] : 0
            });
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
            if (!RotationEnabled || placed.ContainsKey(id))
                return false;
            trayTurns[id] = (trayTurns[id] + 1) & 3;
            return true;
        }

        public void Undo()
        {
            if (undo.Count == 0)
                return;
            var step = undo.Pop();
            trayTurns[step.id] = step.trayTurns;
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

        static bool Matches(PlacedPiece piece, SolutionPlacement step)
        {
            return piece.row == step.row && piece.col == step.col && piece.h == step.h && piece.w == step.w;
        }

        void PushUndo(string id)
        {
            var step = new UndoStep { id = id, trayTurns = trayTurns.ContainsKey(id) ? trayTurns[id] : 0 };
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
