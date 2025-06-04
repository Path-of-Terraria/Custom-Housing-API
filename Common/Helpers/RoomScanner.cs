using System.Collections.Generic;
using Terraria.DataStructures;

namespace HousingAPI.Common.Helpers;

/// <summary> Contains helpers for checking room requirements. </summary>
public readonly struct RoomScanner()
{
    /// <summary> How many tiles this room occupies. </summary>
    public int NumTiles
	{
		get
		{
			int count = 0;
			foreach (Point16 coord in Scanned)
			{
				if (!WorldGen.SolidOrSlopedTile(coord.X, coord.Y))
				{
					count++;
				}
			}

			return count;
		}
	}

	private readonly bool[] TileSet = WorldGen.houseTile;
	private readonly Dictionary<ushort, int> TileCounts = RoomDetours.TileCounts;

	/// <summary> A hash of all tile coordinates scanned by this room. Can be used for more complex checks than the methods provided. </summary>
	public readonly HashSet<Point16> Scanned = RoomDetours.Scanned;

	/// <summary> Whether this room contains a tile of <paramref name="type"/>. </summary>
	public readonly bool ContainsTile(int type)
	{
		return TileSet[type];
	}

	/// <summary> Counts all tiles in the room of <paramref name="type"/>. If <paramref name="type"/> is a multitile, only the top leftmost tile is counted. </summary>
	public readonly int TileCount(int type)
	{
		if (TileCounts.TryGetValue((ushort)type, out int value))
		{
			return value;
		}

		return 0;
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