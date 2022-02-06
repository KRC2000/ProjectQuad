using System;
using System.Collections.Generic;
using System.Text;

namespace Framework.ECS.Components
{
    internal class TransformComponent: Base
    {
        public float X { get; set; }
        public float Y { get; set; }
        public TransformComponent() { base.Type = typeof(TransformComponent); }

		public void Move(float x, float y)
		{
            X += x;
            Y += y;
        }
	}
}
