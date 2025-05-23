using Terraria.DataStructures;

namespace HousingAPI.Common;

internal class RoomDetours : ILoadable
{
    internal static Mod Mod { get; private set; }

    public void Load(Mod mod)
    {
        On_WorldGen.StartRoomCheck += DoCustomCheck;
        On_WorldGen.RoomNeeds += CustomRoomNeeds;
        On_WorldGen.ScoreRoom += CustomRoomScore;

        Mod = mod;
    }

    private static bool DoCustomCheck(On_WorldGen.orig_StartRoomCheck orig, int x, int y)
    {
        bool value = orig(x, y);
        RoomScanner roomScanner = new(WorldGen.houseTile);

        foreach (ModRoomType t in ModContent.GetContent<ModRoomType>())
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
        RoomScanner roomScanner = new(WorldGen.houseTile);

        foreach (GlobalRoomType t in Mod.GetContent<GlobalRoomType>())
        {
            bool? allowRoom = t.AllowRoom(npcType, roomScanner);
            if (allowRoom == null)
			{
				continue;
			}

            WorldGen.canSpawn = value &= allowRoom.Value;
        }

        foreach (ModRoomType t in Mod.GetContent<ModRoomType>())
        {
            if (t.CheckRoomNeeds(npcType, roomScanner) == true)
            {
                roomScanner.ActiveRoomTypes.Add(t.Type);
                WorldGen.canSpawn = value = true;
            }
        }

        return value;
    }

    private static void CustomRoomScore(On_WorldGen.orig_ScoreRoom orig, int ignoreNPC, int npcTypeAskingToScoreRoom)
    {
        orig(ignoreNPC, npcTypeAskingToScoreRoom);

        foreach (GlobalRoomType t in Mod.GetContent<GlobalRoomType>())
		{
			t.ScoreRoom(ref WorldGen.hiScore, ignoreNPC, npcTypeAskingToScoreRoom);
		}

        foreach (ModRoomType t in Mod.GetContent<ModRoomType>())
        {
            if (t.Success) //Check whether the room is still in play before allowing it to modify the score
			{
				t.ScoreRoom(ref WorldGen.hiScore, ignoreNPC, npcTypeAskingToScoreRoom);
			}
        }
    }

    public void Unload() { }
}