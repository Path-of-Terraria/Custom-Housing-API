using HousingAPI.Common.UI;

namespace HousingAPI.Common.Helpers;

public enum Task : byte
{
	Spawning,
	Querying,
	Moving
}

public class MiscDetours : ILoadable
{
	public static Task CurrentTask { get; private set; }

	public void Load(Mod mod)
	{
		On_Main.DrawInterface_38_MouseCarriedObject += On_Main_DrawInterface_38_MouseCarriedObject;
		On_WorldGen.MoveTownNPC += CorrectMoveNPCType;
	}

	private static void On_Main_DrawInterface_38_MouseCarriedObject(On_Main.orig_DrawInterface_38_MouseCarriedObject orig, Main self)
	{
		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			CurrentTask = HousingQueryUI.Querying ? Task.Querying : Task.Moving;
			orig(self);
			CurrentTask = Task.Spawning;
		}
		else
		{
			orig(self);
		}
	}

	private static bool CorrectMoveNPCType(On_WorldGen.orig_MoveTownNPC orig, int x, int y, int n)
	{
		if (CurrentTask is Task.Moving) //Temporarily change prioritizedTownNPCType to the NPC type being moved
		{
			int oldType = WorldGen.prioritizedTownNPCType;
			WorldGen.prioritizedTownNPCType = Main.npc[n].type;

			bool value = orig(x, y, n);

			WorldGen.prioritizedTownNPCType = oldType;
			return value;
		}
		else
		{
			return orig(x, y, n);
		}
	}

	public void Unload() { }
}