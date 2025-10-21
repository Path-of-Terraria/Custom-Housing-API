using HousingAPI.Content;
using HousingAPI.Utilities;
using MonoMod.Cil;

#nullable enable

namespace HousingAPI.Common;

internal class SpawningDetours : ILoadable
{
	public void Load(Mod mod)
    {
		IL_WorldGen.UpdateWorld_Inner += FixSpecialHomelessNpcsPreventingTownNpcSpawns;
    }

    public void Unload() { }

	private static void FixSpecialHomelessNpcsPreventingTownNpcSpawns(ILContext ctx)
	{
		var il = new ILCursor(ctx);

		// Match a part of the '!(Main.npc[i].ModNPC?.TownNPCStayingHomeless)) ?? true)' short-circuit.
		ILLabel? continueLabel = null;
		il.GotoNext(
			MoveType.After,
			i => i.MatchCall(typeof(ModNPC), $"get_{nameof(ModNPC.TownNPCStayingHomeless)}"),
			i => i.MatchNewobj(typeof(bool?))
		);

		// Match 'prioritizedTownNPCType = Main.npc[i].type;'.
		int iIndex = -1;
		il.GotoNext(
			MoveType.Before,
			i => i.MatchLdsfld(typeof(Main), nameof(Main.npc)),
			i => i.MatchLdloc(out iIndex),
			i => i.MatchLdelemRef(),
			i => i.MatchLdfld(typeof(NPC), nameof(NPC.type)),
			i => i.MatchStsfld(typeof(WorldGen), nameof(WorldGen.prioritizedTownNPCType))
		);
		// Go back and grab the label of the most recent branching operation. Varies between optimization modes.
		il.FindPrev(out _, i => i.MatchBrtrue(out continueLabel) || i.MatchBr(out continueLabel));

		// This location is likely branched into.
		ILUtils.HijackIncomingLabels(il);

		il.EmitLdloc(iIndex);
		il.EmitDelegate(AllowPrioritizingHomelessNpc);
		il.EmitBrfalse(continueLabel!);
	}

	private static bool AllowPrioritizingHomelessNpc(int npcIndex)
	{
		// Do not allow NPCs with special requirements to hog normal rooms, preventing town NPC spawns in the process.
		if (Main.npc[npcIndex] is NPC npc && !VanillaRoom.AllowsNPC(npc.type))
		{
			return false;
		}

		return true;
	}
}