using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Framework;

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

        public static bool PrintDrawCalls { get; set; }

        public void foo(){
            
        }

        public Level(string levelFilePath)
        {

            //FillMapRandomly();

            LevelFile = JsonSerializer.Deserialize<LevelFile>(File.ReadAllText(levelFilePath));
        }

        //private void FillMapRandomly()
        //{
        //    Random random = new Random();
        //    for (uint y = 0; y < LevelFile.SizeY; y++)
        //    {
        //        for (uint x = 0; x < LevelFile.SizeX; x++)
        //        {
        //            LevelFile.Map
        //            Map[x, y] = Convert.ToBoolean(random.Next(0, 2));
        //        }
        //    }
        //}

        public void Draw(SpriteBatch _spriteBatch, Camera camera, GraphicsDevice device, Rectangle viewarea)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, camera.GetTransform(device.Viewport));
            uint drawCalls = 0;
            for (uint y = 0; y < LevelFile.SizeY; y++)
            {
                for (uint x = 0; x < LevelFile.SizeX; x++)
                {
                    Vector2 pos = new Vector2(x * Game1.CELLSIZE_X, y * Game1.CELLSIZE_Y);
                    if (viewarea.Contains(pos))
                    {
                        //if (LevelFile.Map[y * LevelFile.SizeX + x] == 1) _spriteBatch.Draw(stamp_t, pos, Color.White);
                        if (LevelFile.Map[y * LevelFile.SizeX + x] == 1) _spriteBatch.Draw(stamp_t, new Rectangle((int)pos.X, (int)pos.Y, (int)Game1.CELLSIZE_X, (int)Game1.CELLSIZE_Y), Color.White);
                        drawCalls++;
                    }
                }
            }
            _spriteBatch.End();
            
            if (PrintDrawCalls)
            {
                _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, null);
                _spriteBatch.DrawString(font, $"Level draw calls: {drawCalls}", new Vector2(), Color.White);
                _spriteBatch.End();
            }
        }
    }


}
