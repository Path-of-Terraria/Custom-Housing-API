using HousingAPI.Common;

namespace HousingAPI.Content;

internal class SampleRoom : ModRoomType
{
    //Allows the merchant to move into a room with a piggy bank.
    //Because the merchant was removed from default housing in SampleGlobalRoom, this is the only valid room type for him.
    protected override bool RoomNeeds(int npcType, RoomScanner results)
	{
		return npcType == NPCID.Merchant && results.ContainsTile(TileID.PiggyBank);
	}
}