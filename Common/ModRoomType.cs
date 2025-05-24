using Terraria.DataStructures;
using Terraria.Localization;

namespace HousingAPI.Common;

/// <summary> Allows you to define custom NPC room behaviour as a singleton. <br/>
/// Multiple room types can apply at once, if conditions allow it. <para/>
/// The provided methods are often called in the order they appear. For example: <list type="bullet">
/// <item> RoomCheck </item>
/// <item> RoomNeeds </item>
/// <item> RoomScore </item>
/// </list></summary>
public abstract class ModRoomType : ModType
{
    /// <summary> The name of this room as displayed in-game. Defaults to "Mods.(Mod Name).Rooms.(ModType Name)". </summary>
    public virtual LocalizedText DisplayName => Language.GetText($"Mods.{Mod.Name}.Rooms.{Name}");

    public static int TypeOf<T>() where T : ModRoomType
	{
		return ModContent.GetInstance<T>().Type;
	}
    /// <summary> Used to track the order content was registered in for <see cref="Type"/>. </summary>
    private static int RegistryCount;

    /// <summary> Tracks consecutive operations for this room and whether it is valid. </summary>
    public bool Success { get; private set; }
    public int Type { get; private set; }

    protected sealed override void Register()
    {
        Type = RegistryCount;
        RegistryCount++;

        ModTypeLookup<ModRoomType>.Register(this);
    }

    /// <summary> Calls <see cref="RoomCheck"/> and updates <see cref="RoomCheckSucceeded"/> accordingly. </summary>
    public bool DoRoomCheck(Point16 origin, RoomScanner results)
    {
        bool? value = RoomCheck(origin.X, origin.Y, results);
        return Success = value ?? true;
    }

    /// <summary> Calls <see cref="RoomNeeds"/> according to <see cref="RoomCheckSucceeded"/>. </summary>
    public bool CheckRoomNeeds(int npcType, RoomScanner results)
    {
        return Success && (Success = RoomNeeds(npcType, results));
    }

    /// <summary> Used to check room structure. Defaults to null, which uses the vanilla response. <br/>
    /// This is more advanced than <see cref="RoomNeeds"/> and doesn't normally need to be used. Consult vanilla code (<see cref="WorldGen.StartRoomCheck"/>) before using this method. </summary>
    /// <param name="x"> The x coordinate to start scanning at. </param>
    /// <param name="y"> The y coordinate to start scanning at. </param>
    /// <returns> Whether this room is the correct shape, size, etc. for this <see cref="ModRoomType"/>. </returns>
    protected virtual bool? RoomCheck(int x, int y, RoomScanner results)
	{
		return null;
	}

    /// <summary> Used to check room furniture requirements. Defaults to false, which uses the vanilla response. </summary>
    /// <param name="results"> A helper to easily get the contents of a room. Tile data normally corresponds to <see cref="WorldGen.houseTile"/>. </param>
    /// <param name="npcType"> The NPC type trying to move in. Corresponds to <see cref="WorldGen.prioritizedTownNPCType"/>. </param>
    /// <returns> Whether this room has all required furniture for this type and <paramref name="npcType"/>. </returns>
    protected virtual bool RoomNeeds(int npcType, RoomScanner results)
	{
		return false;
	}

    /// <summary> Used to modify how much an NPC favours a room with <paramref name="score"/>. </summary>
    /// <param name="score"> The room score to modify. </param>
    /// <param name="ignoreType"></param>
    /// <param name="npcType"> The type of NPC scoring the room. </param>
    public virtual void ScoreRoom(ref int score, int ignoreType, int npcType) { }
}