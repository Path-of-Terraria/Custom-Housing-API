using HousingAPI.Common;
using HousingAPI.Common.Helpers;

namespace HousingAPI.Content;

internal class SampleRoom : ModRoomType
{
	public override bool Priority => true;

    //Allows the merchant to move into a room with a piggy bank.
    //Because the merchant was removed from default housing in SampleGlobalRoom, this is the only valid room type for him.
    protected override bool RoomNeeds(RoomScanner results)
	{
		return results.ContainsTile(TileID.PiggyBank);
	}

	protected override bool AllowNPC(int npcType)
	{
		return npcType == NPCID.Merchant;
	}
}