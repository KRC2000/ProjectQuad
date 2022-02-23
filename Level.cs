using System;
using System.IO;
using System.Collections.Generic;

using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Framework.Camera;

namespace MonogameProj1
{
    class Level
    {
        private LevelFile_xml LevelFile { get; set; }

        public Texture2D stamp_t = null;
        public SpriteFont font = null;

        public uint DrawCalls { get; private set; }

        public Level(string levelFilePath)
        {
            // Deserialise level xml file in to LevelFile
            using (StreamReader reader = new StreamReader(levelFilePath))
            {
                XmlSerializer serializer = new XmlSerializer(typeof(LevelFile_xml));
                LevelFile = (LevelFile_xml)serializer.Deserialize(reader);
            }

            // Map data can be only read as a string, so
            // converting long ass string ("..0, 0, 0, 1, 1, 0,\n..") into 2D array that Layer objects will own
            foreach (var layer in LevelFile.Layers)
            {
                List<string> rows = new List<string>();
                rows.AddRange(layer.Data_str.Split("\n"));
                rows.RemoveAt(0);
                rows.RemoveAt(rows.Count-1);
                
                layer.Data = new int[layer.Width, layer.Height];
                for (int i = 0; i < rows.Count; i++)
                {
                    List<string> row = new List<string>();
                    row.AddRange(rows[i].Split(","));
                    row.RemoveAt(row.Count-1);
                    
                    for (int j = 0; j < row.Count; j++)
                    {
                        layer.Data[j,i] = int.Parse(row[j]);
                    }
                }
            }         
        }

        public void Draw(SpriteBatch _spriteBatch, Camera camera, GraphicsDevice device, Rectangle viewarea)
        {
            _spriteBatch.Begin(SpriteSortMode.Immediate, null, SamplerState.PointClamp, null, null, null, camera.GetTransform(device.Viewport));
            uint drawCallsCounter = 0;

            for (int y = 0; y < LevelFile.Layers[0].Height; y++)
            {
                for (int x = 0; x < LevelFile.Layers[0].Width; x++)
                {
                    Vector2 pos = new Vector2(x * Game1.CELLSIZE_X, y * Game1.CELLSIZE_Y);
                    if (viewarea.Contains(pos))
                    {
                        if (LevelFile.Layers[0].Data[x, y] != 0) 
                        {
                            _spriteBatch.Draw(stamp_t, new Rectangle((int)pos.X, (int)pos.Y, (int)Game1.CELLSIZE_X, (int)Game1.CELLSIZE_Y), Color.White);
                            drawCallsCounter++;
                        }
                    }
                }
            }

            _spriteBatch.End();
            DrawCalls = drawCallsCounter;
        }
    }

    // These are deserialisation classes

    [XmlRoot("map")]
    public class LevelFile_xml
    {
        [XmlElement("tileset")]
        public Tileset_xml[] Tilesets { get; set; }
        [XmlElement("layer")]
        public Layer_xml[] Layers { get; set; }
    }
    
    public class Layer_xml
    {
        [XmlAttribute("id")]
        public int Id { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlAttribute("width")]
        public int Width { get; set; }
        [XmlAttribute("height")]
        public int Height { get; set; }
        [XmlElement("data")]
        public string Data_str { get; set; }
        [XmlIgnore]
        public int[,] Data { get; set; }
    }
    public class Tileset_xml
    {
        [XmlAttribute("firstgid")]
        public int FirstElementId { get; set; }
        [XmlAttribute("source")]
        public string Source { get; set; }
    }
}
