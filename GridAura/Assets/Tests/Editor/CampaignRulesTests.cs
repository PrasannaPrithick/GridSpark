using System.Collections.Generic;
using Aura;
using NUnit.Framework;
using UnityEngine;

public class CampaignRulesTests
{
    static PieceDef[] Catalog()
    {
        return new[]
        {
            P("A", 1, 4, 4, "red"),
            P("B", 1, 5, 5, "blue"),
            P("C", 2, 2, 4, "yellow"),
            P("D", 3, 3, 9, "violet"),
            P("E", 2, 3, 6, "blue"),
            P("F", 2, 4, 8, "yellow"),
            P("G", 2, 5, 10, "red"),
            P("H", 3, 4, 12, "violet")
        };
    }

    static PieceDef P(string id, int h, int w, int area, string color)
    {
        return new PieceDef { id = id, h = h, w = w, area = area, color = color };
    }

    static CampaignFile LoadCampaign()
    {
        var asset = Resources.Load<TextAsset>("Data/campaign");
        Assert.IsNotNull(asset, "campaign.json missing");
        return JsonUtility.FromJson<CampaignFile>(asset.text);
    }

    static SpecialCatalog LoadPacks()
    {
        var asset = Resources.Load<TextAsset>("Data/campaign_levels");
        Assert.IsNotNull(asset, "campaign_levels.json missing");
        return JsonUtility.FromJson<SpecialCatalog>(asset.text);
    }

    static Dictionary<string, SpecialLevelDef> PackMap()
    {
        var map = new Dictionary<string, SpecialLevelDef>();
        var packs = LoadPacks();
        Assert.IsNotNull(packs.levels);
        foreach (var pack in packs.levels)
        {
            if (pack != null && !string.IsNullOrEmpty(pack.id))
                map[pack.id] = pack;
        }
        return map;
    }

    [Test]
    public void CampaignHas250ConsecutiveLevels()
    {
        var campaign = LoadCampaign();
        Assert.AreEqual(250, campaign.levels.Length);
        for (int i = 0; i < campaign.levels.Length; i++)
            Assert.AreEqual(i + 1, campaign.levels[i].global);
    }

    [Test]
    public void EveryCampaignPackExistsOnce()
    {
        var campaign = LoadCampaign();
        var packs = PackMap();
        var used = new HashSet<string>();
        foreach (var entry in campaign.levels)
        {
            if (entry.source != "pack")
                continue;
            Assert.IsTrue(packs.ContainsKey(entry.id), "missing pack " + entry.id);
            Assert.IsTrue(used.Add(entry.id), "duplicate pack " + entry.id);
        }
        Assert.AreEqual(packs.Count, used.Count);
    }

    [Test]
    public void SpeedOnlyOnEasyClassicWithClock()
    {
        var campaign = LoadCampaign();
        var packs = PackMap();
        int speed = 0;
        foreach (var entry in campaign.levels)
        {
            if (entry.modifier != "speed" && entry.timeLimitSec <= 0)
                continue;
            speed++;
            Assert.AreEqual("speed", entry.modifier);
            Assert.AreEqual(0, entry.moveLimit);
            Assert.Greater(entry.timeLimitSec, 0);
            Assert.AreEqual("easy", entry.tag);
            var pack = packs[entry.id];
            Assert.AreEqual("classic", pack.type);
            Assert.IsTrue(pack.rules == null || pack.rules.Length == 0);
        }
        Assert.AreEqual(18, speed);
    }

    [Test]
    public void ComboTypeAndRulesBothApply()
    {
        var state = new GameState();
        var level = new LevelDef
        {
            type = "color",
            rules = new[] { "spots" },
            spots = new[] { new SpotCell { row = 0, col = 0, color = "red" } },
            toPlace = new[] { "A", "B", "C", "D", "E", "F", "G", "H" }
        };
        state.Begin(level, Catalog());
        Assert.IsTrue(state.IsColor);
        Assert.IsTrue(state.IsSpots);
        Assert.IsTrue(state.HasRule("color"));
        Assert.IsTrue(state.HasRule("spots"));
    }

    [Test]
    public void SpotsRejectWrongColorAndAllowMatch()
    {
        var state = new GameState();
        var level = new LevelDef
        {
            type = "spots",
            spots = new[] { new SpotCell { row = 0, col = 0, color = "red" } },
            toPlace = new[] { "A", "B" }
        };
        state.Begin(level, Catalog());
        Assert.IsNull(state.PlaceReject("A", 0, 0, 1, 4, false));
        Assert.AreEqual("This spot needs a red piece", state.PlaceReject("B", 0, 0, 1, 5, false));
        Assert.IsFalse(state.BlockedOnlyBySpot("A", 0, 0, 1, 4));
        Assert.IsTrue(state.BlockedOnlyBySpot("B", 0, 0, 1, 5));
    }

    [Test]
    public void FrozenBlocksRotateAndWrongOrient()
    {
        var state = new GameState();
        var level = new LevelDef
        {
            type = "frozen",
            frozen = new[] { "A" },
            toPlace = new[] { "A", "B" }
        };
        state.Begin(level, Catalog());
        Assert.IsTrue(state.IsFrozenPiece("A"));
        Assert.IsFalse(state.IsFrozenPiece("B"));
        Assert.IsFalse(state.CanRotate("A"));
        Assert.IsTrue(state.CanRotate("B"));
        Assert.IsNull(state.PlaceReject("A", 0, 0, 1, 4, false));
        Assert.AreEqual("Frozen", state.PlaceReject("A", 0, 0, 4, 1, false));
        Assert.IsTrue(state.RotateTray("B"));
        Assert.IsFalse(state.RotateTray("A"));
    }

    [Test]
    public void MustTouchChecksPartnerOnDrop()
    {
        var state = new GameState();
        var level = new LevelDef
        {
            type = "musttouch",
            toPlace = new[] { "A", "G" }
        };
        state.Begin(level, Catalog());
        Assert.IsTrue(state.TryPlace("A", 0, 0, 1, 4, false));
        Assert.AreEqual("Matching colors must touch", state.PlaceReject("G", 2, 0, 2, 5, false));
        Assert.IsNull(state.PlaceReject("G", 1, 0, 2, 5, false));
    }

    [Test]
    public void KeystoneUsesPreplaced()
    {
        var state = new GameState();
        var level = new LevelDef
        {
            type = "keystone",
            rules = new[] { "spots" },
            spots = new[] { new SpotCell { row = 0, col = 0, color = "red" } },
            preplaced = new[]
            {
                new SolutionPlacement { piece = "A", row = 0, col = 0, h = 1, w = 4 }
            },
            toPlace = new[] { "B" }
        };
        state.Begin(level, Catalog());
        Assert.IsTrue(state.HasRule("keystone"));
        Assert.IsTrue(state.IsSpots);
        Assert.IsTrue(state.IsPlaced("A"));
        Assert.IsTrue(state.IsPreplaced("A"));
        Assert.IsTrue(state.placed["A"].locked);
    }

    [Test]
    public void NewPacksHaveValidSpotsAndFrozen()
    {
        var packs = PackMap();
        var colors = new Dictionary<string, string>
        {
            { "A", "red" }, { "G", "red" }, { "B", "blue" }, { "E", "blue" },
            { "C", "yellow" }, { "F", "yellow" }, { "D", "violet" }, { "H", "violet" }
        };
        var catalog = new Dictionary<string, PieceDef>();
        foreach (var piece in Catalog())
            catalog[piece.id] = piece;
        foreach (var pack in packs.Values)
        {
            var cover = new Dictionary<string, string>();
            if (pack.solution != null)
            {
                foreach (var step in pack.solution)
                {
                    for (int r = step.row; r < step.row + step.h; r++)
                    {
                        for (int c = step.col; c < step.col + step.w; c++)
                            cover[r + "," + c] = step.piece;
                    }
                }
            }
            if (pack.spots != null)
            {
                foreach (var spot in pack.spots)
                {
                    string owner;
                    Assert.IsTrue(cover.TryGetValue(spot.row + "," + spot.col, out owner), pack.id + " uncovered spot");
                    Assert.AreEqual(colors[owner], spot.color, pack.id + " spot color");
                }
            }
            if (pack.frozen != null)
            {
                foreach (var id in pack.frozen)
                {
                    foreach (var step in pack.solution)
                    {
                        if (step.piece != id)
                            continue;
                        var def = catalog[id];
                        Assert.AreEqual(def.h, step.h, pack.id + " frozen " + id);
                        Assert.AreEqual(def.w, step.w, pack.id + " frozen " + id);
                    }
                }
            }
        }
    }

    [Test]
    public void MercyKeysArePerLevel()
    {
        Assert.AreEqual("GridAura.FailCount.34", GameState.FailCountKey(34));
        Assert.AreEqual("GridAura.MercyUsed.34", GameState.MercyUsedKey(34));
    }
}
