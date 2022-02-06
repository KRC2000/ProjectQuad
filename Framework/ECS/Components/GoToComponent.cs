﻿using System;
using System.Collections.Generic;
using System.Text;

using Framework;

namespace Framework.ECS.Components
{
	/// <summary>
	/// Only for limited fps
	/// </summary>
    internal class GoToComponent: Base, IUpdatable
    {
		public float targetX;
		public float targetY;

		public bool traveling = false;
		public float traveled;
		public float initDistance;

		public float speed = 1;
		public float vx, vy;
		public float vx_n, vy_n;
		
		TransformComponent tc = null;

		public void GoTo(float x, float y)
		{
			if (Owner.TryGetComponent<TransformComponent>(out tc))
			{
				targetX = x;
				targetY = y;

				vx = targetX - tc.X; vy = targetY - tc.Y;
				initDistance = (float)Math.Sqrt(vx * vx + vy * vy);
				vx_n = vx / initDistance;
				vy_n = vy / initDistance;

				traveling = true;
			}
			else throw new Exception($"Entity does not has required component for system correct execution. Missing component - {typeof(TransformComponent)}");
		}

        public void Update()
        {
			if (traveling){
				tc.Move(vx_n * speed, vy_n * speed);
				traveled += 1 * speed;
				if (traveled >= initDistance) traveling = false;
            }
        }

        public GoToComponent() { base.Type = typeof(GoToComponent); }
	}
}
