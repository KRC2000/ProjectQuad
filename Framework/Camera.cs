using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework
{
	class Camera
	{
		private float scale = 1;
		private float rotation = 0;

		public Vector2 Position { get; private set; }

		/// <summary>
		///Matrix o = Matrix.CreateTranslation(GetCenter(viewport).X - Position.X, GetCenter(viewport).Y - Position.Y, 0);<br></br>
		///Matrix t = Matrix.CreateTranslation(-Position.X, -Position.Y, 0);<br></br>
		///Matrix s = Matrix.CreateScale(scale);<br></br>
		///Matrix r = Matrix.CreateRotationZ(-rotation * ((float)Math.PI / 180f));<br></br>
		///return t* (Matrix.Invert(o)* r * o) * s;
		/// </summary>
		/// <param name="viewport"></param>
		/// <returns></returns>
		public Matrix GetTransform(Viewport viewport) => Matrix.CreateTranslation(-Position.X, -Position.Y, 0)
				* (Matrix.Invert(Matrix.CreateTranslation(GetCenter(viewport).X - Position.X, GetCenter(viewport).Y - Position.Y, 0))
				* Matrix.CreateRotationZ(-rotation * ((float)Math.PI / 180f)) * Matrix.CreateTranslation(GetCenter(viewport).X - Position.X, GetCenter(viewport).Y - Position.Y, 0))
				* Matrix.CreateScale(scale);

		public void SetPos(Vector2 pos)
		{
			Position = pos;
		}
		public void Move(float x, float y)
		{
			Position += new Vector2(x, y);
		}
		public void Move(Vector2 vec)
		{
			Position += vec;
		}

		public void SetZoom(float zoom)
		{
			scale = zoom;
		}
		public void AddZoom(float zoom)
		{
			scale += zoom;
		}

		public void SetRotation(float angle)
		{
			rotation = angle;
		}
		public void AddRotation(float angle)
		{
			rotation += angle;
		}

		public void SetCenter(Vector2 pos, Viewport viewport)
		{
			
			SetPos(new Vector2(pos.X - viewport.Width / scale /2, pos.Y - viewport.Height / scale /2));
		}

		public Vector2 GetCenter(Viewport viewport)
		{
			return new Vector2(Position.X + viewport.Width / scale / 2, Position.Y + viewport.Height/ scale / 2);
		}

		public Rectangle GetViewArea(Viewport viewport)
		{
			return new Rectangle((int)Position.X, (int)Position.Y, (int)(viewport.Width / scale), (int)(viewport.Height / scale));
		}
	}
}

