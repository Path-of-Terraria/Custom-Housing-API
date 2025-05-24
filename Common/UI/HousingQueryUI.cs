using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System.Collections.Generic;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.UI;

namespace HousingAPI.Common.UI;

internal class HousingQueryUI : UIState
{
	internal static bool Querying => Main.instance.mouseNPCIndex == -1 && Main.instance.mouseNPCType == 0;

	public static readonly Asset<Texture2D> QueryBack = ModContent.Request<Texture2D>("HousingAPI/Assets/QueryElement");
	private UIList _elements = [];

	public override void OnInitialize()
	{
		_elements = [];
		_elements.Width.Set(250, 0);
		_elements.Height.Set(0, 0.5f);
		_elements.Left.Set(-20, 0);
		_elements.Height = StyleDimension.Fill;
		_elements.HAlign = 1;

		Append(_elements);

		On_Main.DrawInterface_38_MouseCarriedObject += ActivateDisplay;
	}

	public override void OnActivate()
	{
		_elements.Clear();

		int counter = 0;
		foreach (ModRoomType i in ModContent.GetContent<ModRoomType>())
		{
			_elements.Add(new RoomElement(i, counter));
			counter++;
		}
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
		_elements.Top.Set(GetVerticalOffset() + 230, 0);

		if (!Querying)
		{
			HousingQuerySystem.DeactivateUI();
		}
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);

		var area = _elements.GetDimensions().ToRectangle();
		spriteBatch.Draw(QueryBack.Value, area.TopLeft() + new Vector2(0, -56), Color.White * 0.85f);
	}

	private static void ActivateDisplay(On_Main.orig_DrawInterface_38_MouseCarriedObject orig, Main self)
	{
		if (Querying)
		{
			HousingQuerySystem.ActivateUI();
		}

		orig(self);
	}

	private static int GetVerticalOffset()
	{
		int value = 0;
		if (Main.mapEnabled)
		{
			if (!Main.mapFullscreen && Main.mapStyle == 1)
			{
				value = 256;
			}

			PlayerInput.SetZoom_UI();
			int pushUp = Main.instance.RecommendedEquipmentAreaPushUp;

			if (value + pushUp > Main.screenHeight)
			{
				value = Main.screenHeight - pushUp;
			}
		}

		return value;
	}
}

[Autoload(Side = ModSide.Client)]
internal class HousingQuerySystem : ModSystem
{
	public static bool UIActive => _displayInterface.CurrentState == DisplayState;

	private static UserInterface _displayInterface;
	internal static HousingQueryUI DisplayState;

	public static void ActivateUI()
	{
		if (_displayInterface is null)
		{
			return;
		}

		UIState oldState = _displayInterface.CurrentState;
		_displayInterface.SetState(DisplayState);
		
		if (oldState != _displayInterface.CurrentState)
		{
			DisplayState.OnActivate();
		}
	}

	public static void DeactivateUI()
	{
		_displayInterface?.SetState(null);
	}

	public override void PostSetupContent()
	{
		_displayInterface = new UserInterface();
		DisplayState = new HousingQueryUI();

		DisplayState.Activate();
	}

	public override void UpdateUI(GameTime gameTime)
	{
		if (_displayInterface?.CurrentState != null)
		{
			_displayInterface.Update(gameTime);
		}
	}

	public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
	{
		int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
		if (mouseTextIndex != -1)
		{
			layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(Mod.Name + ':' + nameof(HousingQueryUI), Action, InterfaceScaleType.UI));
		}

		static bool Action()
		{
			if (_displayInterface?.CurrentState != null)
			{
				_displayInterface.Draw(Main.spriteBatch, new GameTime());
			}

			return true;
		}
	}
}