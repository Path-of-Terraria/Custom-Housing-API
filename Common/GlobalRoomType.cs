using HousingAPI.Common.Helpers;

namespace HousingAPI.Common;

/// <summary> Allows you to modify default NPC housing rules. </summary>
public abstract class GlobalRoomType : ModType
{
    protected sealed override void Register()
	{
		ModTypeLookup<GlobalRoomType>.Register(this);
	}

	public sealed override void SetupContent()
	{
		SetStaticDefaults();
	}

	/// <summary> Allows you to modify whether this room is valid. Returns null by default, which uses the vanilla response. </summary>
	public virtual bool? RoomNeeds(RoomScanner results)
	{
		return null;
	}

	public virtual bool? AllowNPC(int npcType)
	{
		return null;
	}

    /// <summary> Allows you to modify how this room is scored by <paramref name="npcType"/>. </summary>
    public virtual void ScoreRoom(ref int score, int ignoreType, int npcType) { }
}