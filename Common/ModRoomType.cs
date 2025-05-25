using HousingAPI.Common.Helpers;
using HousingAPI.Content;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.Localization;

namespace HousingAPI.Common;

public class RoomTypeDatabase : ModSystem
{
	public static readonly Dictionary<int, ModRoomType> RoomByType = [];
	public static int TypeOf<T>() where T : ModRoomType
	{
		return ModContent.GetInstance<T>().Type;
	}

	/// <summary> All registered <see cref="ModRoomType"/>s except for <see cref="VanillaRoom"/>. </summary>
	public static IEnumerable<ModRoomType> GetContent => ModContent.GetContent<ModRoomType>().Where(x => x is not VanillaRoom);
}

/// <summary> Allows you to define custom NPC room behaviour as a singleton. <br/>
/// Multiple room types can apply at once, if conditions allow it. <para/>
/// The provided methods are often called in the order they appear. For example: <list type="bullet">
/// <item> RoomCheck </item>
/// <item> RoomNeeds </item>
/// <item> RoomScore </item>
/// </list></summary>
public abstract class ModRoomType : ModType
{
	/// <summary> Whether this room completely overrides the vanilla room type. </summary>
	public virtual bool Priority => false;
	/// <summary> The name of this room as displayed in-game. Defaults to "Mods.(Mod Name).Rooms.(ModType Name).DisplayName". </summary>
	public virtual LocalizedText DisplayName => Language.GetText($"Mods.{Mod.Name}.Rooms.{Name}.DisplayName");
	/// <summary> The description of this room. Defaults to "Mods.(Mod Name).Rooms.(ModType Name).Description". </summary>
	public virtual LocalizedText Description => Language.GetText($"Mods.{Mod.Name}.Rooms.{Name}.Description");

	/// <summary> Tracks consecutive operations for this room and whether it is valid. </summary>
	public bool Success { get; protected set; }
	public int Type { get; private set; }

	protected sealed override void Register()
	{
		Type = RoomTypeDatabase.RoomByType.Count;
		RoomTypeDatabase.RoomByType.Add(Type, this);

		ModTypeLookup<ModRoomType>.Register(this);
	}

	public sealed override void SetupContent()
	{
		SetStaticDefaults();
	}

    /// <summary> Calls <see cref="RoomCheck"/> and updates <see cref="RoomCheckSucceeded"/> accordingly. </summary>
    public bool DoStructureCheck(Point16 origin, RoomScanner results)
    {
		return Success = RoomCheck(origin.X, origin.Y, results);
	}

    /// <summary> Calls <see cref="RoomNeeds"/> according to <see cref="RoomCheckSucceeded"/>. </summary>
    public bool DoBasicCheck(int npcType, RoomScanner results, out bool needsMet)
    {
		needsMet = false;
		if (!Success)
		{
			return false;
		}

		needsMet = RoomNeeds(results);
		return Success && (Success = needsMet && (MiscDetours.CurrentTask is Task.Querying || AllowNPC(npcType)));
	}

    /// <summary> Used to check room structure. Defaults to null, which uses the vanilla response. <br/>
    /// This is more advanced than <see cref="RoomNeeds"/> and doesn't normally need to be used. Consult vanilla code (<see cref="WorldGen.StartRoomCheck"/>) before using this method. </summary>
    /// <param name="x"> The x coordinate to start scanning at. </param>
    /// <param name="y"> The y coordinate to start scanning at. </param>
    /// <returns> Whether this room is the correct shape, size, etc. for this <see cref="ModRoomType"/>. </returns>
    protected virtual bool RoomCheck(int x, int y, RoomScanner results)
	{
		return WorldGen.canSpawn;
	}

    /// <summary> Used to check room furniture requirements. Defaults to false, which uses the vanilla response. </summary>
    /// <param name="results"> A helper to easily get the contents of a room. Tile data normally corresponds to <see cref="WorldGen.houseTile"/>. </param>
    /// <returns> Whether this room has all required furniture for this type and <paramref name="npcType"/>. </returns>
    protected virtual bool RoomNeeds(RoomScanner results)
	{
		return false;
	}

	/// <summary> Whether <paramref name="npcType"/> is allowed in this room. Returns true by default. </summary>
	/// <param name="npcType"> The NPC type trying to move in. Corresponds to <see cref="WorldGen.prioritizedTownNPCType"/>. </param>
	protected virtual bool AllowNPC(int npcType)
	{
		return true;
	}

	/// <summary> Used to modify how much an NPC favours a room with <paramref name="score"/>. </summary>
	/// <param name="score"> The room score to modify. </param>
	/// <param name="ignoreType"></param>
	/// <param name="npcType"> The type of NPC scoring the room. </param>
	public virtual void ScoreRoom(ref int score, int ignoreType, int npcType) { }
}