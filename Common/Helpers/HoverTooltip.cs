using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.GameContent;

namespace HousingAPI.Common.Helpers;

internal class HoverTooltip : ILoadable
{
	public static Asset<Texture2D> Border = Main.Assets.Request<Texture2D>("Images/UI/PanelBorder");
	public static Asset<Texture2D> Background = Main.Assets.Request<Texture2D>("Images/UI/PanelBackground");

	private static string Tooltip;

	public static void QueueTooltip(string tooltip)
	{
		Tooltip = tooltip;
	}

	public void Load(Mod mod)
	{
		On_Main.DrawInterface_28_InfoAccs += DrawTooltip;
	}

	private static void DrawTooltip(On_Main.orig_DrawInterface_28_InfoAccs orig, Main self)
	{
		const int padding = 4;

		orig(self);

		if (Tooltip != null)
		{
			Vector2 size = FontAssets.MouseText.Value.MeasureString(Tooltip);
			var pos = Vector2.Min(Main.MouseScreen + new Vector2(40), new Vector2(Main.screenWidth - size.X, Main.screenHeight - size.Y));

			DrawPanel(Main.spriteBatch, new Rectangle((int)pos.X - padding, (int)pos.Y - padding, (int)size.X + padding * 2, (int)size.Y + padding), Color.Black * 0.5f, Color.Black * 0.25f);
			Utils.DrawBorderString(Main.spriteBatch, Tooltip, pos, Main.MouseTextColorReal);

			Tooltip = null;
		}
	}

	public void Unload() { }

	/// <summary> Draws a background panel based on vanilla code. </summary>
	internal static void DrawPanel(SpriteBatch spriteBatch, Rectangle area, Color color, Color borderColor = default, int cornerSize = 12)
	{
		const int bar = 4;
		int corner = cornerSize;

		var point = new Point(area.X, area.Y);
		var point2 = new Point(point.X + area.Width - corner, point.Y + area.Height - corner);
		int width = point2.X - point.X - corner;
		int height = point2.Y - point.Y - corner;

		for (int i = 0; i < 2; i++)
		{
			Texture2D texture = ((i == 0) ? Background : Border).Value;
			Color c = (i == 0) ? color : borderColor;

			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y, corner, corner), new Rectangle(0, 0, corner, corner), c);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y, corner, corner), new Rectangle(corner + bar, 0, corner, corner), c);
			spriteBatch.Draw(texture, new Rectangle(point.X, point2.Y, corner, corner), new Rectangle(0, corner + bar, corner, corner), c);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point2.Y, corner, corner), new Rectangle(corner + bar, corner + bar, corner, corner), c);
			spriteBatch.Draw(texture, new Rectangle(point.X + corner, point.Y, width, corner), new Rectangle(corner, 0, bar, corner), c);
			spriteBatch.Draw(texture, new Rectangle(point.X + corner, point2.Y, width, corner), new Rectangle(corner, corner + bar, bar, corner), c);
			spriteBatch.Draw(texture, new Rectangle(point.X, point.Y + corner, corner, height), new Rectangle(0, corner, corner, bar), c);
			spriteBatch.Draw(texture, new Rectangle(point2.X, point.Y + corner, corner, height), new Rectangle(corner + bar, corner, corner, bar), c);
			spriteBatch.Draw(texture, new Rectangle(point.X + corner, point.Y + corner, width, height), new Rectangle(corner, corner, bar, bar), c);
		}
	}
}