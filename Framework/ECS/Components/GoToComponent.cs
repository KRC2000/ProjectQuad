using System;
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
		public bool isTraveling { get; private set; } = false;

		/// <summary>Distance traveled to the current target</summary>
		public float Traveled { get; private set; }

		/// <summary>Whole initial distance between start of travel and finish</summary>
		public float InitDistance { get; private set; }

		public float Speed { get; set; } = 1;

		///<summary>If already traveling will ingnore Go() or GoTo() when "true"</summary>
		public bool Locked { get; set; } = false;

		public TransformComponent transform_comp {get => tc; private set => tc = value;}

		// Private var's:

		private float targetX;
		private float targetY;

		/// <summary>Start to finish vector</summary>
		private float vx, vy;

		/// <summary>Start to finish vector normalised</summary>
		private float vx_n, vy_n;
		
		private TransformComponent tc = null; 
		
		/// <summary>
		/// Moves transform component of the owner to the passed coordinate<br></br>
		/// Update() should be called in cycle to work
		/// </summary>
		public void GoTo(float x, float y)
		{
			if ((Locked && !isTraveling) || !Locked)
			{
                if (tc != null ||
                    Owner.TryGetComponent<TransformComponent>(out tc))
                {
                    targetX = x;
                    targetY = y;

                    vx = targetX - tc.X; vy = targetY - tc.Y;
                    InitDistance = (float)Math.Sqrt(vx * vx + vy * vy);
                    vx_n = vx / InitDistance;
                    vy_n = vy / InitDistance;

                    isTraveling = true;
                }
                else throw new Exception($"Entity does not has required component for system correct execution. Missing component - {typeof(TransformComponent)}");
			}
        }

        public void Go(float x, float y)
        {
			if ((Locked && !isTraveling) || !Locked)
			{
                if (tc != null ||
                    Owner.TryGetComponent<TransformComponent>(out tc))
                {
                    targetX = tc.X + x;
                    targetY = tc.Y + y;

                    vx = targetX - tc.X; vy = targetY - tc.Y;
                    InitDistance = (float)Math.Sqrt(vx * vx + vy * vy);
                    vx_n = vx / InitDistance;
                    vy_n = vy / InitDistance;

                    isTraveling = true;
                }
                else throw new Exception($"Entity does not has required component for system correct execution. Missing component - {typeof(TransformComponent)}");
			}
        }

        public void Update()
        {
			if (isTraveling){
				tc.Move(vx_n * Speed, vy_n * Speed);
				Traveled += 1 * Speed;
				if (Traveled >= InitDistance) { isTraveling = false; Traveled = 0;}
            }
        }

        public GoToComponent() { base.Type = typeof(GoToComponent); }
	}
}
