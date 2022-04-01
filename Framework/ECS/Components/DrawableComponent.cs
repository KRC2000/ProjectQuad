using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Framework.ECS.Components
{
    internal class DrawableComponent: Base
    {
		public Texture2D texture;
		public TransformComponent tc = null;
		public DrawableComponent() { base.Type = typeof(DrawableComponent); }

		public void Draw(SpriteBatch _batch)
		{
			if (Owner.TryGetComponent<TransformComponent>(out tc))
				_batch.Draw(texture, tc.Pos, Color.White);
			else
				_batch.Draw(texture, new Vector2(0, 0), Color.White);
		}

		public void Draw(SpriteBatch _batch, Vector2 size)
		{
			if (Owner.TryGetComponent<TransformComponent>(out tc))
				_batch.Draw(texture, new Rectangle((int)tc.Pos.X, (int)tc.Pos.Y, (int)size.X, (int)size.Y), Color.White);
			else
				_batch.Draw(texture, new Rectangle(0, 0, (int)size.X, (int)size.Y) , Color.White);
		}
	}
}
