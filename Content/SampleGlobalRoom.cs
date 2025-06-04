/*using HousingAPI.Common;
using HousingAPI.Common.Helpers;

namespace HousingAPI.Content;

internal class SampleGlobalRoom : GlobalRoomType
{
	public override bool? RoomNeeds(RoomScanner results)
	{
		return null; //results.ContainsTile(TileID.PiggyBank) ? false : null;
	}

	//Prevents the merchant from moving into default housing.
	public override bool? AllowNPC(int npcType)
	{
		return npcType == NPCID.Merchant ? false : null;
	}
}*/