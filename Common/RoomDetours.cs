using HousingAPI.Common.Helpers;
using HousingAPI.Common.UI;
using HousingAPI.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;

namespace HousingAPI.Common;

internal class RoomDetours : ILoadable
{
	internal static IEnumerable<ModRoomType> RoomTypes => ModContent.GetContent<ModRoomType>().Where(x => x is not VanillaRoom);

	public void Load(Mod mod)
    {
        On_WorldGen.StartRoomCheck += DoCustomCheck;
        On_WorldGen.RoomNeeds += CustomRoomNeeds;
        On_WorldGen.ScoreRoom += CustomRoomScore;
    }

    private static bool DoCustomCheck(On_WorldGen.orig_StartRoomCheck orig, int x, int y)
    {
        bool value = orig(x, y);
        RoomScanner roomScanner = new(WorldGen.houseTile);

		VanillaRoom.Instance.DoRoomCheck(new Point16(x, y), roomScanner);

        foreach (ModRoomType t in RoomTypes)
        {
            if (t.DoRoomCheck(new Point16(x, y), roomScanner) == true)
			{
				WorldGen.canSpawn = value = true;
			}
        }

        return value;
    }

    private static bool CustomRoomNeeds(On_WorldGen.orig_RoomNeeds orig, int npcType)
    {
        bool value = orig(npcType);
		RoomScanner scanner = new(WorldGen.houseTile);

		VanillaRoom.Instance.SetSuccess(value);
		WorldGen.canSpawn = value = VanillaRoom.Instance.CheckRoomNeeds(npcType, scanner);

		foreach (ModRoomType t in RoomTypes)
		{
			if (t.CheckRoomNeeds(npcType, scanner) == true)
			{
				WorldGen.canSpawn = value = true;
			}
		}

		if (MiscDetours.CurrentTask is Task.Querying)
		{
			RoomElement.Successes = [.. ModContent.GetContent<ModRoomType>().Select(x => x.Success)];
		}

		return value;
    }

    private static void CustomRoomScore(On_WorldGen.orig_ScoreRoom orig, int ignoreNPC, int npcTypeAskingToScoreRoom)
    {
        orig(ignoreNPC, npcTypeAskingToScoreRoom);

		VanillaRoom.Instance.ScoreRoom(ref WorldGen.hiScore, ignoreNPC, npcTypeAskingToScoreRoom);

        foreach (ModRoomType t in RoomTypes)
        {
            if (t.Success) //Check whether the room is still in play before allowing it to modify the score
			{
				t.ScoreRoom(ref WorldGen.hiScore, ignoreNPC, npcTypeAskingToScoreRoom);
			}
        }
    }

    public void Unload() { }
}