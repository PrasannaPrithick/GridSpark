using System;

namespace Aura
{
    [Serializable]
    public class LevelCatalog
    {
        public int gridSize = 8;
        public PieceDef[] pieces;
        public LevelDef[] levels;
    }

    [Serializable]
    public class PieceDef
    {
        public string id;
        public int h;
        public int w;
        public int area;
        public string color;
    }

    [Serializable]
    public class LevelDef
    {
        public int id;
        public string difficulty;
        public BlockPlacement[] black;
        public SolutionPlacement[] solution;
    }

    [Serializable]
    public class BlockPlacement
    {
        public int row;
        public int col;
        public int h;
        public int w;
    }

    [Serializable]
    public class SolutionPlacement
    {
        public string piece;
        public int row;
        public int col;
        public int h;
        public int w;
    }
}
