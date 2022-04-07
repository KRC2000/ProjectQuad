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
                        if (currentLevel.IsPassable(new Point(gridPos.X, gridPos.Y - 1))) {
                            gtc.GoTo(new Vector2(tc.Pos.X, tc.Pos.Y - Game1.CELLSIZE_Y));
                        }
                    break;
                    case Direction.W:
                        if (currentLevel.IsPassable(new Point(gridPos.X - 1, gridPos.Y))) {
                            gtc.GoTo(new Vector2(tc.Pos.X - Game1.CELLSIZE_X, tc.Pos.Y));
                        }
                    break;
                    case Direction.S:
                        if (currentLevel.IsPassable(new Point(gridPos.X, gridPos.Y + 1))) {
                            gtc.GoTo(new Vector2(tc.Pos.X, tc.Pos.Y + Game1.CELLSIZE_Y));
                        }
                    break;
                    case Direction.E:
                        if (currentLevel.IsPassable(new Point(gridPos.X + 1, gridPos.Y))) {
                            gtc.GoTo(new Vector2(tc.Pos.X + Game1.CELLSIZE_X, tc.Pos.Y));
                        }
                    break;
                }
        }
        
        public void TravelTo(Point cellPos, Level currentLevel, bool queue)
        {
            VerifyRequiredComponents();

            if (finder == null) {
                finder = new Pathfinder();
                finder.Mode = PathMode.Aligned;
            }

            List<Point> path = null;
            if (!finder.Initialised || finder.Tag != "map_default")
            {
                path = new List<Point>();
                finder.Init(currentLevel.LevelFile.Layers[0].Data, "map_default");
            }

            
            Point pos = new Point((int)((tc.Pos.X + Game1.CELLSIZE_X / 2) / Game1.CELLSIZE_X),
                                    (int)((tc.Pos.Y + Game1.CELLSIZE_Y / 2) / Game1.CELLSIZE_Y));

            if (queue && gtc.Queue.Count > 0)
            {
                finder.GetPath(out path, new Point((int)(gtc.Queue[gtc.Queue.Count-1].X / Game1.CELLSIZE_X), 
                                                    (int)(gtc.Queue[gtc.Queue.Count-1].Y / Game1.CELLSIZE_Y)), cellPos);
            }
            else
            {
                finder.GetPath(out path, pos, cellPos);
                gtc.Queue.Clear();
            }
            

            foreach (var point in path)
            {
                gtc.AddToQueue(new Vector2(point.X * Game1.CELLSIZE_X, point.Y * Game1.CELLSIZE_Y));
            }
        }

        private void VerifyRequiredComponents()
        {
            if (tc == null){
				if (!Owner.TryGetComponent<TransformComponent>(out tc))
                	throw new Exception($"{this.GetType()}: Entity does not has required component for system correct execution. Missing component - {typeof(TransformComponent)}");
			}
            if (gtc == null){
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