using System.Collections.Generic;

namespace HousingAPI.Common;

public readonly struct RoomScanner(bool[] tileSet)
{
    /// <summary> How many tiles this room occupies. Shorthand for <see cref="WorldGen.numRoomTiles"/>. </summary>
    public static int NumTiles => WorldGen.numRoomTiles;

    public readonly bool[] TileSet = tileSet;
    internal readonly List<int> ActiveRoomTypes = [];

    /// <summary> Whether this room counts as the given type. </summary>
    public readonly bool CountsAsType(int type)
	{
		return ActiveRoomTypes.Contains(type);
	}
    /// <inheritdoc cref="CountsAsType(int)"/>
    public readonly bool CountsAsType<T>() where T : ModRoomType
	{
		return ActiveRoomTypes.Contains(ModRoomType.TypeOf<T>());
	}

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