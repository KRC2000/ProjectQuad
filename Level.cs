using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Framework.Camera;

namespace MonogameProj1
{
    class LevelFile
    {
        public string Name { get; set; }
        public int SizeX { get; set; }
        public int SizeY { get; set; }
        public int[] Map { get; set; }
    }

    class Level
    {
        LevelFile LevelFile { get; set; }

        public Texture2D stamp_t = null;
        public SpriteFont font = null;

        public uint DrawCalls { get; private set; }

        public void foo(){
            
        }

        public Level(string levelFilePath)
        {
            LevelFile = JsonSerializer.Deserialize<LevelFile>(File.ReadAllText(levelFilePath));
        }

        public void Draw(SpriteBatch _spriteBatch, Camera camera, GraphicsDevice device, Rectangle viewarea)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, camera.GetTransform(device.Viewport));
            uint drawCallsCounter = 0;
            for (uint y = 0; y < LevelFile.SizeY; y++)
            {
                for (uint x = 0; x < LevelFile.SizeX; x++)
                {
                    Vector2 pos = new Vector2(x * Game1.CELLSIZE_X, y * Game1.CELLSIZE_Y);
                    if (viewarea.Contains(pos))
                    {
                        if (LevelFile.Map[y * LevelFile.SizeX + x] == 1) _spriteBatch.Draw(stamp_t, new Rectangle((int)pos.X, (int)pos.Y, (int)Game1.CELLSIZE_X, (int)Game1.CELLSIZE_Y), Color.White);
                        drawCallsCounter++;
                    }
                }
            }
            _spriteBatch.End();
            DrawCalls = drawCallsCounter;
        }
    }


}
