using System.Collections.Generic;
using Terraria.DataStructures;

namespace HousingAPI.Common;

public static class RoomTypeDatabase
{
    public static readonly HashSet<RoomType> LoadedTypes = [];
}

/// <summary> Allows you to define custom NPC room behaviour as a singleton stored in <see cref="RoomTypeDatabase.LoadedTypes"/>.<br/>
/// Multiple room types can apply at once, if conditions allow it. </summary>
public abstract class RoomType : ModType
{
    public bool RoomCheckFailed { get; private set; }

    protected sealed override void Register()
    {
        RoomTypeDatabase.LoadedTypes.Add(this);
        Load();
    }

    public bool? DoRoomCheck(Point16 origin)
    {
        bool? value = CheckRoom(origin.X, origin.Y);
        RoomCheckFailed = !(value ?? true);

        return value;
    }

    /// <summary> Used to check room structure. Returns null by default, which uses the vanilla response. </summary>
    /// <param name="x"> The x coordinate to start scanning at. </param>
    /// <param name="y"> The y coordinate to start scanning at. </param>
    /// <returns> Whether this room is the correct shape, size, etc. for this <see cref="RoomType"/>. </returns>
    protected virtual bool? CheckRoom(int x, int y) => null;

    /// <summary> Used to check room furniture requirements. Returns null by default, or false if <see cref="RoomCheckFailed"/>. <br/>
    /// Null uses the vanilla response. </summary>
    /// <param name="npcType"> Corresponds to <see cref="WorldGen.prioritizedTownNPCType"/>. </param>
    /// <returns> Whether this room has all required furniture for this <see cref="RoomType"/> and <paramref name="npcType"/>. </returns>
    public virtual bool? RoomNeeds(int npcType) => RoomCheckFailed ? false : null;

    /// <summary> Used to modify how much an NPC favours a room with <paramref name="score"/>. </summary>
    /// <param name="score"> The room score to modify. </param>
    /// <param name="ignoreType"></param>
    /// <param name="npcType"> The type of NPC scoring the room. </param>
    public virtual void ScoreRoom(ref int score, int ignoreType, int npcType) { }
}