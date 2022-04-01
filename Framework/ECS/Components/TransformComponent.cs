using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;

namespace Framework.ECS.Components
{
    internal class TransformComponent: Base
    {
        public Vector2 Pos { get; private set; }

        public TransformComponent() { base.Type = typeof(TransformComponent); }

        public void Move(Vector2 vec){
            Pos += vec;
        }

        public void SetPos(Vector2 vec){
            Pos = vec;
        }
	}
}
