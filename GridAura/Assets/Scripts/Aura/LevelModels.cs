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
        public string key;
        public string source;
        public string type;
        public string tag;
        public string difficulty;
        public int global;
        public int world;
        public string worldName;
        public int posInWorld;
        public int par;
        public int moveLimit;
        public bool intro;
        public string coachText;
        public string unused;
        public string[] unusedPieces;
        public int rotateCap;
        public string[] rules;
        public string[] wrongPieces;
        public BlockPlacement[] black;
        public SolutionPlacement[] solution;
        public SolutionPlacement[] preplaced;
        public SolutionPlacement[] initial;
        public string[] toPlace;
        public string[] rotationNeeded;
        public TutorialInfo tutorial;
        public SpotCell[] spots;
        public string[] frozen;
        public string modifier;
        public int timeLimitSec;
    }

    [Serializable]
    public class SpotCell
    {
        public int row;
        public int col;
        public string color;
    }

    [Serializable]
    public class TutorialInfo
    {
        public string phase;
        public bool rotationEnabled;
        public string[] aids;
        public string coachText;
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

    [Serializable]
    public class CampaignFile
    {
        public string[] worldNames;
        public CampaignEntry[] levels;
    }

    [Serializable]
    public class CampaignEntry
    {
        public int global;
        public string source;
        public string id;
        public string tag;
        public int par;
        public int moveLimit;
        public int world;
        public string worldName;
        public int posInWorld;
        public bool intro;
        public string coachText;
        public string modifier;
        public int timeLimitSec;
    }

    [Serializable]
    public class SpecialCatalog
    {
        public SpecialLevelDef[] levels;
    }

    [Serializable]
    public class SpecialLevelDef
    {
        public string id;
        public string type;
        public string[] rules;
        public int rotateCap;
        public BlockPlacement[] black;
        public string[] toPlace;
        public SolutionPlacement[] solution;
        public SolutionPlacement[] initial;
        public SolutionPlacement[] preplaced;
        public string unused;
        public string[] unusedPieces;
        public string[] wrongPieces;
        public string[] rotationNeeded;
        public string coachText;
        public bool intro;
        public SpotCell[] spots;
        public string[] frozen;
    }
}
