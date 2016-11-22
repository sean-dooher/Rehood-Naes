using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rehood_Naes.Building;
using Rehood_Naes.Interfaces;
using Rehood_Naes.Entities;
using Rehood_Naes.Events;
using Rehood_Naes.Menus;
using IDrawable = Rehood_Naes.Interfaces.IDrawable;

namespace Rehood_Naes.Building
{
	/// <summary>
	/// A tile draws a certain image repeating over a certain area
	/// </summary>
	public class Tile : IDrawable
	{
		#region Fields
		private Spritesheet sheet;
		private Rectangle sourceBox;
		private Rectangle drawBox;
		#endregion
		
		#region Properties
		/// <summary>
		/// Size of draw area
		/// </summary>
		public Vector2 Size
		{
			get { return new Vector2(drawBox.Width, drawBox.Height); }
		}
		
		/// <summary>
		/// Position of draw area
		/// </summary>
		public Vector2 Position
		{
			get; private set;
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Draws a tile in a certain draw area with a certain tileID and spritesheet(optional)
		/// </summary>
		/// <param name="drawBox"></param>
		/// <param name="tileID"></param>
		/// <param name="sheet"></param>
		public Tile(Rectangle drawBox, string tileID, Spritesheet sheet = null)
		{
			this.sheet = sheet;
			Position = new Vector2(drawBox.X, drawBox.Y);
			loadTile(drawBox, tileID);
		}
		
		/// <summary>
		/// Draws a tile in a certain draw area with a certain tileID and spritesheet(optional)
		/// </summary>
		/// <param name="position">Position of draw area</param>
		/// <param name="Size">Size of draw area</param>
		/// <param name="tileID">ID of tile</param>
		/// <param name="sheet">Spritesheet for caching</param>
		public Tile(Vector2 position, Vector2 Size, string tileID, Spritesheet sheet = null)
		{
			this.sheet = sheet;
			Position = position;
			loadTile(new Rectangle((int)position.X, (int)position.Y, (int)Size.X, (int) Size.Y), tileID);
		}
		#endregion
		
		#region Methods
		/// <summary>
		/// Draws tile with given SpriteBatch
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to draw tile with</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			for(int i = 0; i < (drawBox.Width / sourceBox.Width) + 1; i++)
			{
				for(int j = 0; j < (drawBox.Height / sourceBox.Height) + 1; j++)
				{
					Vector2 newPos = new Vector2(Position.X + i * sourceBox.Width, Position.Y + j * sourceBox.Height);
					spriteBatch.Draw(sheet.Sheet, newPos, sourceBox, Color.White);
				}
			}
		}
		
		/// <summary>
		/// Updates tile
		/// </summary>
		/// <param name="gameTime">Snapshot of timing values</param>
		public void Update(GameTime gameTime)
		{
			
		}
		#endregion
		
		#region Static Methods
		/// <summary>
		/// Loads a new tile from an XML element
		/// </summary>
		/// <param name="tile">XML element to load</param>
		/// <param name="sheets">Spritesheets for caching</param>
		/// <returns></returns>
		public static Tile LoadFromXML(XElement tile, List<Spritesheet> sheets = null)
		{
			Spritesheet tempSheet = null;
			if(sheets.Count(s => s.SpritesheetID == tile.Element("Spritesheet").Value) > 0)//if sheets contains element
				tempSheet = sheets.First(s => s.SpritesheetID == tile.Element("Spritesheet").Value);
			
			//generate new rectangle from position and size
			Rectangle tileRect = RectangleEx.FromArray(
				(tile.Element("Position").Value + "," + tile.Element("Size").Value).Split(','));
	
			return new Tile(tileRect, tile.Element("TileID").Value, tempSheet);
		}
		#endregion
		#region Helpers
		private void loadTile(Rectangle drawBox, string ID)
		{
			this.drawBox = drawBox;
			string path = AppDomain.CurrentDomain.BaseDirectory + @"Content\tiles\" + ID + ".xml";
			XDocument doc = XDocument.Load(path);
			string sheetID = doc.Descendants("Tile").Descendants("Spritesheet").ToArray()[0].Value;
			if(sheet == null || sheet.SpritesheetID != sheetID) //if loaded spritesheet doesn't match
				sheet = new Spritesheet(sheetID);
			sourceBox = RectangleEx.FromArray(doc.Descendants("Tile").Descendants("Box").First().Value.Split(','));
		}
		#endregion
	}
}
