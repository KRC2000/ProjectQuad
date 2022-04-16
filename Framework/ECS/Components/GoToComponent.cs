using System;
using System.Collections.Generic;
using System.Text;

using Framework;

using Microsoft.Xna.Framework;

namespace Framework.ECS.Components
{
	/// <summary>
	/// Requires:<br></br>
	/// Framework.ECS.Components.TransformComponent;<br></br>
	/// Only for limited fps
	/// </summary>
	class GoToComponent : Base, IUpdatable
	{
		public bool isTraveling { get; private set; } = false;

		public bool isDoingQueue { get; private set; } = false;

		/// <summary>Distance traveled to the current target</summary>
		public float Traveled { get; private set; }

		/// <summary>Whole initial distance between start of travel and finish</summary>
		public float InitDistance { get; private set; }

		public float Speed { get; set; } = 1;

		///<summary>If already traveling will ingnore Go() or GoTo() when "true"</summary>
		public bool Locked { get; set; } = false;

		public Vector2 Target { get; private set; }

		public List<Vector2> Queue = new List<Vector2>();

		/// <summary>Start to finish vector</summary>
		public Vector2 v;

		/// <summary>Start to finish vector normalised</summary>
		public Vector2 v_n;

		private TransformComponent tc = null;

		/// <summary>
		/// Sets order to move transform component of the owner to the passed coordinate<br></br>
		/// Update() should be called in cycle to work
		/// </summary>
		public void GoTo(Vector2 pos)
		{
			VerifyRequiredComponents();

			if ((Locked && !isTraveling) || !Locked)
			{

				Target = pos;

				v = Target - tc.Pos;
				InitDistance = (float)Math.Sqrt(v.X * v.X + v.Y * v.Y);
				v_n = v / InitDistance;

				Traveled = 0;
				isTraveling = true;
			}
		}

		public void Update()
		{
			VerifyRequiredComponents();

			if (isTraveling)
			{
				tc.Move(v_n * Speed);
				Traveled += Speed;
				if (Traveled >= InitDistance)
				{
					tc.SetPos(Target);
					isTraveling = false;
					Traveled = 0;

					if (isDoingQueue)
					{
						if (Queue.Count > 0) Queue.RemoveAt(0);
						isDoingQueue = false;
					}
				}
			}
			else
			{
				if (Queue.Count > 0)
				{
					GoTo(Queue[0]);
					isDoingQueue = true;
				}
			}
		}

		private void VerifyRequiredComponents()
		{
			if (tc == null)
			{
				if (!Owner.TryGetComponent<TransformComponent>(out tc))
					throw new Exception($"{this.GetType()}: Entity does not has required component for system correct execution. Missing component - {typeof(TransformComponent)}");
			}
		}

		public void AddToQueue(Vector2 pos)
		{
			Queue.Add(pos);
		}
		public GoToComponent() { base.Type = typeof(GoToComponent); }
	}
}
