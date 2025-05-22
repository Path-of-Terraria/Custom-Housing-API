using Terraria.DataStructures;

namespace HousingAPI.Common;

internal class RoomDetours : ILoadable
{
    public void Load(Mod mod)
    {
        On_WorldGen.StartRoomCheck += DoCustomCheck;
        On_WorldGen.RoomNeeds += CustomRoomNeeds;
        On_WorldGen.ScoreRoom += CustomRoomScore;
    }

    private static bool DoCustomCheck(On_WorldGen.orig_StartRoomCheck orig, int x, int y)
    {
        bool value = orig(x, y);

        foreach (var t in RoomTypeDatabase.LoadedTypes)
        {
            if (t.DoRoomCheck(new Point16(x, y)) is bool valueMod)
                WorldGen.canSpawn = value = valueMod;
        }

        return value;
    }

    private static bool CustomRoomNeeds(On_WorldGen.orig_RoomNeeds orig, int npcType)
    {
        bool value = orig(npcType);

        RoomScanner roomScanner = new(WorldGen.houseTile);
        foreach (var t in RoomTypeDatabase.LoadedTypes)
        {
            if (t.RoomNeeds(npcType, roomScanner) is bool valueMod)
                WorldGen.canSpawn = value = valueMod;
        }

        return value;
    }

    private static void CustomRoomScore(On_WorldGen.orig_ScoreRoom orig, int ignoreNPC, int npcTypeAskingToScoreRoom)
    {
        orig(ignoreNPC, npcTypeAskingToScoreRoom);

        foreach (var t in RoomTypeDatabase.LoadedTypes)
            t.ScoreRoom(ref WorldGen.hiScore, ignoreNPC, npcTypeAskingToScoreRoom);
    }

    public void Unload() { }
}