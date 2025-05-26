using HousingAPI.Common.Helpers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using System.Collections.Generic;
using Terraria.Localization;
using Terraria.UI;

namespace HousingAPI.Common.UI;

/// <summary> Used to contain info about a query in UI. </summary>
public struct QueryInfo(bool Success, string Error = null)
{
	public bool Success = Success;
	public string Error = Error;
}

internal class RoomElement : UIElement
{
	public static readonly Asset<Texture2D> Back = ModContent.Request<Texture2D>("HousingAPI/Assets/RoomElement");
	public static readonly Asset<Texture2D> Back_Highlight = ModContent.Request<Texture2D>("HousingAPI/Assets/RoomElement_Highlight");
	public static readonly Asset<Texture2D> Score = ModContent.Request<Texture2D>("HousingAPI/Assets/Score");

	public readonly int Type;
	public readonly string Text;

	private static readonly Dictionary<int, QueryInfo> InfoByType = [];
	private float _fadeIn = 0f;

	public RoomElement(ModRoomType room, int indexInList)
	{
		Type = room.Type;
		Text = room.DisplayName.Value;
		_fadeIn -= indexInList;

		InfoByType.Clear();

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

		QueryInfo info;
		spriteBatch.Draw(Back.Value, center - Back.Size() / 2, Color.White * 0.95f * _fadeIn);
		Utils.DrawBorderString(spriteBatch, Text, center, Main.MouseTextColorReal * _fadeIn, 0.9f, 0.5f, 0.4f);

		if (IsMouseHovering)
		{
			spriteBatch.Draw(Back_Highlight.Value, center - Back_Highlight.Size() / 2, Color.White * 0.95f * _fadeIn);

			string tooltip = RoomTypeDatabase.RoomByType[Type].Description.Value;
			if (InfoByType.TryGetValue(Type, out info))
			{
				if (info.Success)
				{
					tooltip = Language.GetTextValue($"Mods.{nameof(HousingAPI)}.Rooms.Common.Suitable");
				}
				else if (info.Error != null)
				{
					tooltip = info.Error;
				}
			}

			HoverTooltip.QueueTooltip(tooltip);
		}

		if (InfoByType.TryGetValue(Type, out info))
		{
			const float iconScale = 0.8f;
			Rectangle frame = Score.Frame(1, 2, 0, info.Success ? 0 : 1, sizeOffsetY: -2);

			spriteBatch.Draw(Score.Value, center + new Vector2(-area.Width / 2 + 14, 2), frame, Color.Black * 0.3f * _fadeIn, 0, frame.Size() / 2, iconScale, default, 0);
			spriteBatch.Draw(Score.Value, center + new Vector2(-area.Width / 2 + 14, 0), frame, Color.White * 0.85f * _fadeIn, 0, frame.Size() / 2, iconScale, default, 0);
		}
	}

	public static void UpdateIndicators()
	{
		InfoByType.Clear();

		foreach (int type in RoomTypeDatabase.RoomByType.Keys)
		{
			ModRoomType room = RoomTypeDatabase.RoomByType[type];
			InfoByType.Add(type, new(room.Success, room.ErrorLog));
		}
	}
}