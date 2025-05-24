using HousingAPI.Common;
using HousingAPI.Common.Helpers;

namespace HousingAPI.Content;

/// <summary> Represents the default NPC room. </summary>
internal class VanillaRoom : ModRoomType
{
	internal static VanillaRoom Instance { get; private set; }
	public void SetSuccess(bool value)
	{
		Success = value;
	}

	public override void SetStaticDefaults()
	{
		Instance = ModContent.GetInstance<VanillaRoom>();
	}

	protected override bool RoomNeeds(RoomScanner results)
	{
		bool value = Success;

		foreach (GlobalRoomType t in ModContent.GetContent<GlobalRoomType>())
		{
			bool? allowRoom = t.RoomNeeds(results);
			if (allowRoom == null)
			{
				continue;
			}

			WorldGen.canSpawn = value &= allowRoom.Value;
		}

		return value;
	}

	protected override bool AllowNPC(int npcType)
	{
		bool value = Success;

		foreach (GlobalRoomType t in ModContent.GetContent<GlobalRoomType>())
		{
			bool? allowRoom = t.AllowNPC(npcType);
			if (allowRoom == null)
			{
				continue;
			}

			WorldGen.canSpawn = value &= allowRoom.Value;
		}

		return value;
	}

	public override void ScoreRoom(ref int score, int ignoreType, int npcType)
	{
		foreach (GlobalRoomType t in ModContent.GetContent<GlobalRoomType>())
		{
			t.ScoreRoom(ref score, ignoreType, npcType);
		}
	}
}