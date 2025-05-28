namespace HousingAPI.Common.UI;

/// <summary> Prevents NPCs from drawing in the housing UI while querying. </summary>
internal class QueryButtonDetour : ILoadable
{
	public void Load(Mod mod)
	{
		On_Main.DrawNPCHousesInUI += On_Main_DrawNPCHousesInUI;
	}

	private static void On_Main_DrawNPCHousesInUI(On_Main.orig_DrawNPCHousesInUI orig, Main self)
	{
		if (HousingQuerySystem.UIActive)
		{
			return; //Skips orig
		}

		orig(self);
	}

	public void Unload() { }
}