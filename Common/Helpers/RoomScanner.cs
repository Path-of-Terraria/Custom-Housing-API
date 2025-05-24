namespace HousingAPI.Common.Helpers;

public readonly struct RoomScanner(bool[] tileSet)
{
    /// <summary> How many tiles this room occupies. Shorthand for <see cref="WorldGen.numRoomTiles"/>. </summary>
    public static int NumTiles => WorldGen.numRoomTiles;

    public readonly bool[] TileSet = tileSet;

    /// <summary> Whether this room contains a tile of <paramref name="type"/>. </summary>
    public readonly bool ContainsTile(int type)
	{
		return TileSet[type];
	}

    /// <summary> Whether this room contains a tile from <see cref="TileID.Sets.RoomNeeds"/>. </summary>
    public readonly bool HasRoomNeed(int[] types)
    {
        for (int j = 0; j < types.Length; j++)
        {
            if (TileSet[types[j]])
			{
				return true;
			}
        }

        return false;
    }
}