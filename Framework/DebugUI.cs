using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework.DebugUI
{
	static class DebugUI
	{
		public static SpriteFont Font { get; set; }

		static public void DrawDebug<T>(SpriteBatch batch, string message, T var, uint order)
		{
			if (Font == null) throw new Exception("Error:Framework.DebugUI: No font was set");

			batch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp);
			batch.DrawString(Font, string.Concat(message, var.ToString()), new Vector2(0, order * Font.MeasureString("Q").Y), Color.White);
			batch.End();
		}
	}

}