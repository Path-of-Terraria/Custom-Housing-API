using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria.UI;

namespace HousingAPI.Common.UI;

internal class RoomElement : UIElement
{
	public static readonly Asset<Texture2D> Back = ModContent.Request<Texture2D>("HousingAPI/Assets/RoomElement");
	public static readonly Asset<Texture2D> Score = ModContent.Request<Texture2D>("HousingAPI/Assets/Score");

	public readonly int Type;
	public readonly string Text;

	internal static bool[] Successes;
	private float _fadeIn = 0f;

	public RoomElement(ModRoomType type, int indexInList)
	{
		Type = type.Type;
		Text = type.DisplayName.Value;
		_fadeIn -= indexInList;

		Successes = null;

		Width.Pixels = 224;
		Height.Pixels = 24;
	}

	public override void Update(GameTime gameTime)
	{
		_fadeIn = Math.Min(_fadeIn + 0.25f, 1);
		base.Update(gameTime);
	}

	public override void Draw(SpriteBatch spriteBatch)
	{
		base.Draw(spriteBatch);

		var area = GetDimensions().ToRectangle();
		Vector2 center = area.Center() - new Vector2(20 * (1f - _fadeIn), 0);

		spriteBatch.Draw(Back.Value, center - Back.Size() / 2, Color.White * 0.85f * _fadeIn);

		//Texture2D icon = TextureAssets.Item[ItemID.WoodenTable].Value;
		//spriteBatch.Draw(icon, center + new Vector2(area.Width / 2 - 17, -1) - icon.Size() / 2, Color.White * _fadeIn);

		Utils.DrawBorderString(spriteBatch, Text, center, Main.MouseTextColorReal * _fadeIn, 0.9f, 0.5f, 0.4f);

		if (Successes is not null)
		{
			Rectangle frame = Score.Frame(1, 2, 0, Successes[Type] ? 0 : 1, sizeOffsetY: -2);
			spriteBatch.Draw(Score.Value, center + new Vector2(-area.Width / 2 + 14, 0), frame, Color.White * 0.85f * _fadeIn, 0, frame.Size() / 2, 0.8f, default, 0);
		}
	}
}