namespace HousingAPI.Common;

public readonly struct RoomScanner(bool[] tileSet)
{
    public readonly bool[] tileSet = tileSet;

    /// <summary> Whether this room contains a tile of <paramref name="type"/>. </summary>
    public readonly bool ContainsTile(int type) => tileSet[type];

    /// <summary> Whether this room contains a tile from <see cref="TileID.Sets.RoomNeeds"/>. </summary>
    public readonly bool HadRoomNeed(int[] types)
    {
        for (int j = 0; j < types.Length; j++)
        {
            if (tileSet[types[j]])
                return true;
        }

        return false;
    }
}