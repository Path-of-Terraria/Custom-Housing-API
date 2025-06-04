using HousingAPI.Common.Helpers;
using HousingAPI.Common.UI;
using HousingAPI.Content;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ObjectData;

namespace HousingAPI.Common;

internal class RoomDetours : ILoadable
{
	internal static readonly Dictionary<ushort, int> TileCounts = [];
	internal static readonly HashSet<Point16> Scanned = [];

	public void Load(Mod mod)
    {
		On_WorldGen.CheckRoom += HandleTileCounts;
        On_WorldGen.StartRoomCheck += DoCustomCheck;
        On_WorldGen.RoomNeeds += CustomRoomNeeds;
        On_WorldGen.ScoreRoom += CustomRoomScore;
    }

	private static void HandleTileCounts(On_WorldGen.orig_CheckRoom orig, int x, int y)
	{
		orig(x, y);

		if (WorldGen.canSpawn && !Scanned.Contains(new Point16(x, y)))
		{
			ushort tileType = Main.tile[x, y].TileType; //Some platforms cause issues with IsTopLeft
			if ((tileType == TileID.Platforms || TileObjectData.IsTopLeft(x, y)) && !TileCounts.TryAdd(tileType, 1))
			{
				TileCounts[tileType]++;
			}

			Scanned.Add(new Point16(x, y));
		}
	}

	private static bool DoCustomCheck(On_WorldGen.orig_StartRoomCheck orig, int x, int y)
    {
		Scanned.Clear();
		TileCounts.Clear(); //TileCounts and Scanned are populated in the following orig

        bool value = orig(x, y);
        RoomScanner roomScanner = new();

		VanillaRoom.Instance.SetSuccess(value);
		value = VanillaRoom.Instance.DoStructureCheck(new Point16(x, y), roomScanner);

        foreach (ModRoomType t in RoomTypeDatabase.GetContent)
        {
			value |= t.DoStructureCheck(new Point16(x, y), roomScanner);
		}

        return WorldGen.canSpawn = value;
    }

    private static bool CustomRoomNeeds(On_WorldGen.orig_RoomNeeds orig, int npcType)
    {
        bool vanillaValue = orig(npcType);
		bool modValue = false; //Add a value for non-vanilla types so we can adjust logic using Priority correctly
		RoomScanner scanner = new();

		VanillaRoom.Instance.SetSuccess(vanillaValue);
		vanillaValue = VanillaRoom.Instance.DoBasicCheck(npcType, scanner, out _);

		foreach (ModRoomType t in RoomTypeDatabase.GetContent)
		{
			modValue |= t.DoBasicCheck(npcType, scanner, out bool needsMet);

			if (needsMet && t.Priority)
			{
				VanillaRoom.Instance.SetSuccess(vanillaValue = false, Language.GetTextValue($"Mods.{nameof(HousingAPI)}.Rooms.Common.Override"));
			}
		}

		if (MiscDetours.CurrentTask is Task.Querying) //Take a snapshot of current successes for UI indicators
		{
			RoomElement.UpdateIndicators();
		}

		return WorldGen.canSpawn = vanillaValue || modValue;
    }

    private static void CustomRoomScore(On_WorldGen.orig_ScoreRoom orig, int ignoreNPC, int npcTypeAskingToScoreRoom)
    {
        orig(ignoreNPC, npcTypeAskingToScoreRoom);

		if (VanillaRoom.Instance.Success)
		{
			VanillaRoom.Instance.ScoreRoom(ref WorldGen.hiScore, ignoreNPC, npcTypeAskingToScoreRoom);
		}

        foreach (ModRoomType t in RoomTypeDatabase.GetContent)
        {
            if (t.Success) //Check whether the room is still in play before allowing it to modify the score
			{
				t.ScoreRoom(ref WorldGen.hiScore, ignoreNPC, npcTypeAskingToScoreRoom);
			}
        }
    }

    public void Unload() { }
}