using HousingAPI.Common;

namespace HousingAPI.Content;

internal class SampleGlobalRoom : GlobalRoomType
{
    //Prevents the merchant from moving into default housing.
    public override bool? AllowRoom(int npcType, RoomScanner results)
	{
		return npcType == NPCID.Merchant ? false : null;
	}
}