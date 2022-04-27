using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using Framework.ECS;
using Framework.ECS.Components;
using Framework.Pathfinder;

namespace ProjectQuad.Framework.Components
{
	/// <summary>
	/// Requires:<br></br>
	/// Framework.ECS.Components.TransformComponent;<br></br>
	/// Framework.ECS.Components.GoToComponent;
	/// </summary>
	class TravelComponent : Base
	{
		private GoToComponent gtc = null;
		private TransformComponent tc = null;
		private Pathfinder finder = null;


		public TravelComponent() { base.Type = typeof(TravelComponent); }

		public void TravelOneStep(Direction direction, Level currentLevel)
		{
			VerifyRequiredComponents();

			Point gridPos = new Point((int)((tc.Pos.X + Game1.CELLSIZE_X / 2) / Game1.CELLSIZE_X),
										(int)((tc.Pos.Y + Game1.CELLSIZE_Y / 2) / Game1.CELLSIZE_Y));

			switch (direction)
			{
				case Direction.N:
					if (currentLevel.IsPassable(new Point(gridPos.X, gridPos.Y - 1)))
					{
						gtc.GoTo(new Vector2(tc.Pos.X, tc.Pos.Y - Game1.CELLSIZE_Y));
					}
					break;
				case Direction.W:
					if (currentLevel.IsPassable(new Point(gridPos.X - 1, gridPos.Y)))
					{
						gtc.GoTo(new Vector2(tc.Pos.X - Game1.CELLSIZE_X, tc.Pos.Y));
					}
					break;
				case Direction.S:
					if (currentLevel.IsPassable(new Point(gridPos.X, gridPos.Y + 1)))
					{
						gtc.GoTo(new Vector2(tc.Pos.X, tc.Pos.Y + Game1.CELLSIZE_Y));
					}
					break;
				case Direction.E:
					if (currentLevel.IsPassable(new Point(gridPos.X + 1, gridPos.Y)))
					{
						gtc.GoTo(new Vector2(tc.Pos.X + Game1.CELLSIZE_X, tc.Pos.Y));
					}
					break;
			}
		}

		/// <summary>
		/// Depending on the passed bool queue variable, either makes entity travel to the cellPos, or queues such travel in the end of the already queued movement.
		/// Travel path accounts level and obstacles, shortest path calculated.
		/// </summary>
		/// <param name="cellPos">Travel final destination</param>
		/// <param name="currentLevel">Level that entity is traveling on</param>
		/// <param name="queue">Should travel be queued(true) or started immediatly(false)</param>
		public void TravelTo(Point cellPos, Level currentLevel, bool queue)
		{
			VerifyRequiredComponents();

			// Set up pathfinder object if it wasn't done
			if (finder == null)
			{
				finder = new Pathfinder();
				finder.Quiet = true;
				finder.Mode = PathMode.Aligned;
			}

			// Create path container, if pathfinder not initialised with this level - initialise it.
			List<Point> path = null;
			if (!finder.Initialised || finder.Tag != currentLevel.Name)
				finder.Init(currentLevel.ObstacleLayer.Data, currentLevel.Name);

			// Get entity grid position
			Point pos = new Point((int)((tc.Pos.X + Game1.CELLSIZE_X / 2) / Game1.CELLSIZE_X),
									(int)((tc.Pos.Y + Game1.CELLSIZE_Y / 2) / Game1.CELLSIZE_Y));

			// if (queue == true) then find a path starting from last position in the queue,
			// otherwise find path from the current entity position and clear the queue
			if (queue && gtc.Queue.Count > 0)
			{
				finder.GetPath(out path, new Point((int)(gtc.Queue[gtc.Queue.Count - 1].X / Game1.CELLSIZE_X),
													(int)(gtc.Queue[gtc.Queue.Count - 1].Y / Game1.CELLSIZE_Y)), cellPos);
			}
			else
			{
				if (gtc.isTraveling)
				{
					Point targetCellPos = new Point((int)(gtc.Queue[0].X / Game1.CELLSIZE_X), (int)(gtc.Queue[0].Y / Game1.CELLSIZE_Y)); 
					Vector2 targetPos = gtc.Target;
					if (finder.GetPath(out path, targetCellPos, cellPos))
					{
						gtc.Queue.Clear();
						gtc.AddToQueue(targetPos);
					}
				}
				else
				{
					if (finder.GetPath(out path, pos, cellPos))
						gtc.Queue.Clear();
				}

			}

			if (path != null)
			{
				foreach (var point in path)
				{
					gtc.AddToQueue(new Vector2(point.X * Game1.CELLSIZE_X, point.Y * Game1.CELLSIZE_Y));
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
			if (gtc == null)
			{
				if (!Owner.TryGetComponent<GoToComponent>(out gtc))
					throw new Exception($"{this.GetType()}: Entity does not has required component for system correct execution. Missing component - {typeof(GoToComponent)}");
			}
		}

		public enum Direction
		{
			N, E, S, W,
			NE, NW, SE, SW
		}
	}
}