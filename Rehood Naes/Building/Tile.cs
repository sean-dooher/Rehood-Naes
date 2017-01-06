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
		private Rectangle middle;
		private Rectangle topLeft;
		private Rectangle topRight;
		private Rectangle bottomLeft;
		private Rectangle bottomRight;
		private Rectangle drawBox;
		private bool rounded;

		#endregion

		#region Properties

		/// <summary>
		/// Size of draw area
		/// </summary>
		public Vector2 Size {
			get { return new Vector2 (drawBox.Width, drawBox.Height); }
		}

		public Rectangle InnerRectangle
		{
			get 
			{
				if (rounded)
					return new Rectangle ((int)Position.X + topLeft.Width, (int)Position.Y + topRight.Width,
						drawBox.Width - topLeft.Width - topRight.Width, drawBox.Height - topLeft.Height - bottomLeft.Height);
				else
					return new Rectangle (Position.ToPoint(), drawBox.Size);
			}
		}

		/// <summary>
		/// Position of draw area
		/// </summary>
		public Vector2 Position {
			get;
			private set;
		}

		#endregion

		#region Constructors

		/// <summary>
		/// Draws a tile in a certain draw area with a certain tileID and spritesheet(optional)
		/// </summary>
		/// <param name="drawBox"></param>
		/// <param name="tileID"></param>
		/// <param name="sheet"></param>
		public Tile (Rectangle drawBox, string tileID, bool rounded = false, Spritesheet sheet = null)
		{
			this.sheet = sheet;
			this.rounded = rounded;
			Position = new Vector2 (drawBox.X, drawBox.Y);
			loadTile (drawBox, tileID);
		}

		/// <summary>
		/// Draws a tile in a certain draw area with a certain tileID and spritesheet(optional)
		/// </summary>
		/// <param name="position">Position of draw area</param>
		/// <param name="size">Size of draw area</param>
		/// <param name="tileID">ID of tile</param>
		/// <param name="sheet">Spritesheet for caching</param>
		public Tile (Vector2 position, Vector2 size, string tileID, bool rounded = false, Spritesheet sheet = null)
		{
			this.sheet = sheet;
			this.rounded = rounded;
			Position = position;
			loadTile (new Rectangle (position.ToPoint (), size.ToPoint ()), tileID);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Draws tile with given SpriteBatch
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to draw tile with</param>
		public void Draw (SpriteBatch spriteBatch)
		{
			if (!rounded)
			{
				for (int i = 0; i < (drawBox.Width / middle.Width) + 1; i++)
					for (int j = 0; j < (drawBox.Height / middle.Height) + 1; j++)
					{
						Vector2 newPos = new Vector2 (Position.X + i * middle.Width, Position.Y + j * middle.Height);
						spriteBatch.Draw (sheet.Sheet, newPos, middle, Color.White);
					}
			}
			else
			{
				//draw middle sections, then add corners
				//middle functions as two rectangles with a significant overlap
				Rectangle horizontalBox = new Rectangle((int)Position.X, (int)Position.Y + topLeft.Height,
					drawBox.Width, drawBox.Height - topLeft.Height - bottomLeft.Height);
				Rectangle verticalBox = new Rectangle((int)Position.X + topLeft.Width, (int)Position.Y,
					drawBox.Width - topLeft.Width - topRight.Width, drawBox.Height);
				spriteBatch.Draw (sheet.Sheet, horizontalBox, middle, Color.White);
				spriteBatch.Draw (sheet.Sheet, verticalBox, middle, Color.White);
				//draw corners
				spriteBatch.Draw(sheet.Sheet, drawBox.Location.ToVector2(), topLeft, Color.White);
				spriteBatch.Draw(sheet.Sheet, 
					new Vector2(drawBox.Right - topRight.Width, Position.Y), topRight, Color.White);
				spriteBatch.Draw (sheet.Sheet,
					new Vector2 (Position.X, drawBox.Bottom - bottomLeft.Height), bottomLeft, Color.White);
				spriteBatch.Draw (sheet.Sheet,
					new Vector2 (drawBox.Right - bottomRight.Width, drawBox.Bottom - bottomRight.Height), bottomRight, Color.White);
			}
		}

		/// <summary>
		/// Updates tile
		/// </summary>
		/// <param name="gameTime">Snapshot of timing values</param>
		public void Update (GameTime gameTime)
		{
			//TODO: Add suport for animated tiles (12/27/2016)	
		}

		#endregion

		#region Static Methods

		/// <summary>
		/// Loads a new tile from an XML element
		/// </summary>
		/// <param name="tile">XML element to load</param>
		/// <param name="sheets">Spritesheets for caching</param>
		/// <returns></returns>
		public static Tile LoadFromXML (XElement tile, List<Spritesheet> sheets = null)
		{
			//TODO: add support for rounded tiles from XML
			Spritesheet tempSheet = null;
			if (sheets.Count (s => s.SpritesheetID == tile.Element ("Spritesheet").Value) > 0)//if sheets contains element
				tempSheet = sheets.First (s => s.SpritesheetID == tile.Element ("Spritesheet").Value);
			
			//generate new rectangle from position and size
			Rectangle tileRect = RectangleEx.FromArray (
				                     (tile.Element ("Position").Value + "," + tile.Element ("Size").Value).Split (','));
	
			return new Tile (tileRect, tile.Element ("TileID").Value, false, tempSheet);
		}

		#endregion

		#region Helpers

		private void loadTile (Rectangle drawBox, string ID)
		{
			this.drawBox = drawBox;
			string path = AppDomain.CurrentDomain.BaseDirectory + @"Content\tiles\" + ID + ".xml";
			XDocument doc = XDocument.Load (path);
			string sheetID = doc.Descendants ("Spritesheet").First().Value;
			if (sheet == null || sheet.SpritesheetID != sheetID) //if loaded spritesheet doesn't match
				sheet = new Spritesheet (sheetID);
			//add sourceboxes, middle is the sourcebox if it is not rounded
			middle = RectangleEx.FromArray(doc.Descendants("Middle").Elements("Box").First().Value.Split(','));
			if (rounded)
			{
				topLeft = RectangleEx.FromArray (doc.Descendants ("TopLeft").Elements ("Box").First ().Value.Split (','));
				topRight = RectangleEx.FromArray (doc.Descendants ("TopRight").Elements ("Box").First ().Value.Split (','));
				bottomLeft = RectangleEx.FromArray (doc.Descendants ("BottomLeft").Elements ("Box").First ().Value.Split (','));
				bottomRight = RectangleEx.FromArray (doc.Descendants ("BottomRight").Elements ("Box").First ().Value.Split (','));
			}

		}

		#endregion
	}
}
