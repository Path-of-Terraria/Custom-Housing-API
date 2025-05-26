using HousingAPI.Common;
using HousingAPI.Common.Helpers;
using Humanizer;
using System.Collections.Generic;
using Terraria.Localization;

namespace HousingAPI.Content;

/// <summary> Represents the default NPC room. </summary>
public sealed class VanillaRoom : ModRoomType
{
	public static VanillaRoom Instance { get; private set; }

	internal void SetSuccess(bool value, string log = null)
	{
		Success = value;

		if (log != null)
		{
			ErrorLog = log;
		}
	}

	public override void Load()
	{
		Instance = this;
	}

	protected override bool RoomCheck(int x, int y, RoomScanner results)
	{
		return Success;
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

		if (!value)
		{
			LogNeeds(results);
		}

		return value;
	}

	private void LogNeeds(RoomScanner results)
	{
		List<string> elements = [];

		if (!results.HasRoomNeed(TileID.Sets.RoomNeeds.CountsAsChair))
		{
			elements.Add(Language.GetTextValue("Game.HouseChair"));
		}

		if (!results.HasRoomNeed(TileID.Sets.RoomNeeds.CountsAsTable))
		{
			elements.Add(Language.GetTextValue("Game.HouseTable"));
		}

		if (!results.HasRoomNeed(TileID.Sets.RoomNeeds.CountsAsDoor))
		{
			elements.Add(Language.GetTextValue("Game.HouseDoor"));
		}

		if (!results.HasRoomNeed(TileID.Sets.RoomNeeds.CountsAsTorch))
		{
			elements.Add(Language.GetTextValue("Game.HouseLightSource"));
		}

		ErrorLog = Language.GetTextValue("Game.HouseMissing_" + elements.Count).FormatWith([.. elements]);
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