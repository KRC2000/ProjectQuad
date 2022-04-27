using System;
using System.IO;
using System.Collections.Generic;

using System.Xml.Serialization;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using Framework.Camera;

namespace ProjectQuad
{
	class Level
	{
		public LevelFile LevelFile { get; set; }

		public Dictionary<string, Tileset> Tilesets { get; private set; }

		public Layer ObstacleLayer { get; private set; }
		public string Name { get; private set; } = null;

		private Texture2D stamp_t = null;
		public SpriteFont font = null;

		public uint DrawCalls { get; private set; }

		public Level(string levelFilePath, string levelName)
		{
			// Deserialise level xml file in to LevelFile
			using (StreamReader reader = new StreamReader(levelFilePath))
			{
				XmlSerializer serializer = new XmlSerializer(typeof(LevelFile));
				LevelFile = (LevelFile)serializer.Deserialize(reader);

				Tilesets = new Dictionary<string, Tileset>();

				foreach (var tileset_ref in LevelFile.Tileset_refs)
				{

					using (StreamReader reader1 = new StreamReader(Path.Join("Content", tileset_ref.Source_tsx)))
					{
						XmlSerializer serializer1 = new XmlSerializer(typeof(Tileset));
						Tileset newts = (Tileset)serializer1.Deserialize(reader1);
						newts.this_ref = tileset_ref;
						Tilesets.Add(newts.Name, newts);
					}
				}
			}

			// Map data can be only read as a string, so
			// converting long ass string ("..0, 0, 0, 1, 1, 0,\n..") into 2D array that Layer objects will own.
			// Also assigning obstacle layer var
			foreach (var layer in LevelFile.Layers)
			{
				List<string> rows = new List<string>();
				rows.AddRange(layer.Data_str.Split("\n"));
				rows.RemoveAt(0);
				rows.RemoveAt(rows.Count - 1);

				layer.Data = new uint[layer.Height, layer.Width];
				for (int i = 0; i < rows.Count; i++)
				{
					List<string> row = new List<string>();
					row.AddRange(rows[i].Split(","));
					row.RemoveAt(row.Count - 1);

					for (int j = 0; j < row.Count; j++)
					{
						layer.Data[i, j] = uint.Parse(row[j]);
					}
				}

				if (layer.Name == "Obstacles") ObstacleLayer = layer;
			}
		}

		public void LoadTilesets(ContentManager content)
		{
			foreach (var tileset in Tilesets)
			{
				tileset.Value.Image.Texture = content.Load<Texture2D>(Path.GetFileNameWithoutExtension(tileset.Value.Image.Source_png));
			}
		}

		public void Draw(SpriteBatch _spriteBatch, Camera camera, GraphicsDevice device, Rectangle viewarea)
		{
			_spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, camera.GetTransform(device.Viewport));
			uint drawCallsCounter = 0;

			foreach (var layer in LevelFile.Layers)
			{
				for (int y = 0; y < layer.Width; y++)
				{
					for (int x = 0; x < layer.Height; x++)
					{
						Vector2 pos = new Vector2(y * Game1.CELLSIZE_X, x * Game1.CELLSIZE_Y);

						if (viewarea.Contains(pos))
						{
							uint tileValue = layer.Data[x, y];
							Rectangle sourceRect = new Rectangle();
							if (tileValue != 0)
							{
								foreach (var tileset in Tilesets)
								{
									// if tile value is in the tile range of current tileset -> pick and assign texture used by this tileset
									if (tileValue >= tileset.Value.this_ref.FirstElementId && tileValue < tileset.Value.this_ref.FirstElementId + tileset.Value.TileCount)
									{
										stamp_t = tileset.Value.Image.Texture;

										int r_x = (int)(tileValue - tileset.Value.this_ref.FirstElementId) % tileset.Value.Columns;
										int r_y = (int)(tileValue - tileset.Value.this_ref.FirstElementId) / tileset.Value.Columns;
										sourceRect = new Rectangle(r_x * tileset.Value.TileWidth, r_y * tileset.Value.TileHeight, tileset.Value.TileWidth, tileset.Value.TileHeight);
									}
								}

								_spriteBatch.Draw(stamp_t, new Rectangle((int)pos.X, (int)pos.Y, (int)Game1.CELLSIZE_X, (int)Game1.CELLSIZE_Y), sourceRect, Color.White, 0, new Vector2(0, 0), SpriteEffects.None, 0);
								drawCallsCounter++;
							}
						}
					}
				}
			}


			_spriteBatch.End();
			DrawCalls = drawCallsCounter;
		}

		public bool IsPassable(Point tilePos)
		{
			if (ObstacleLayer.Data[tilePos.Y, tilePos.X] == 0) return true;
			else return false;
		}
	}

	// These are deserialisation classes

	[XmlRoot("map")]
	public class LevelFile
	{
		[XmlElement("tileset")]
		public Tileset_ref[] Tileset_refs { get; set; }

		[XmlElement("layer")]
		public Layer[] Layers { get; set; }
	}

	public class Layer
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
		public uint[,] Data { get; set; }
	}
	public class Tileset_ref
	{
		[XmlAttribute("firstgid")]
		public int FirstElementId { get; set; }
		[XmlAttribute("source")]
		public string Source_tsx { get; set; }
	}

	[XmlRoot("tileset")]
	public class Tileset
	{
		[XmlAttribute("name")]
		public string Name { get; set; }

		[XmlAttribute("tilewidth")]
		public int TileWidth { get; set; }

		[XmlAttribute("tileheight")]
		public int TileHeight { get; set; }

		[XmlAttribute("tilecount")]
		public int TileCount { get; set; }

		[XmlAttribute("columns")]
		public int Columns { get; set; }

		[XmlElement("image")]
		public Image Image { get; set; }

		// Reference to this tileset from map file
		[XmlIgnore]
		public Tileset_ref this_ref { get; set; }
	}

	public class Image
	{
		[XmlAttribute("source")]
		public string Source_png { get; set; }

		[XmlAttribute("width")]
		public int Width { get; set; }

		[XmlAttribute("height")]
		public int height { get; set; }

		[XmlIgnore]
		public Texture2D Texture { get; set; }
	}
}
