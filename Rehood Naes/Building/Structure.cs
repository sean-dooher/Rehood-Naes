using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Rehood_Naes.Interfaces;

namespace Rehood_Naes.Building
{
	/// <summary>
	/// Represents a static structure 
	/// </summary>
	public class Structure : DrawableGameComponent
	{
		#region Fields
		private List<Spritesheet> spritesheets;
		private ContentManager Content;
		private Vector2 position;
		private List<Tuple<Rectangle, Vector2, String>> elements; // < Source Rectangle, Position in drawing, SpritesheetID>
		private List<RectangleF> bounds;
		#endregion	
		
		#region Properties
		/// <summary>
		/// ID of structure
		/// </summary>
		public String StructureID
		{
			get; protected set;
		}
		
		/// <summary>
		/// Current position of structure (from top-left corner)
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
		}
		
		/// <summary>
		/// List of rectangles for collision detection
		/// </summary>
		public List<RectangleF> DrawBoxes
		{
			get { return new List<RectangleF>(bounds); }
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Loads a structure with a specified ID at a specified positon
		/// </summary>
		/// <param name="x">Horizontal position in integers</param>
		/// <param name="y">Vertical position in integers</param>
		/// <param name="structureID">ID of structure to load</param>
		/// <param name="spritesheets">Optionally loads spritesheets to reduce memory use</param>
		public Structure(int x, int y, string structureID, List<Spritesheet> spritesheets = null) : base(RPG.CurrentGame)
		{
			Content = RPG.ContentManager;
			this.spritesheets = new List<Spritesheet>();
			if(spritesheets != null)this.spritesheets.AddRange(spritesheets);
			this.position = new Vector2(x,y);
			loadStructure(this.Position,structureID);
		}
		
		/// <summary>
		/// Loads a structure with a specified ID at a specified positon
		/// </summary>
		/// <param name="position">Position of structure</param>
		/// <param name="structureID">ID of structure to load</param>
		/// <param name="spritesheets">Optionally loads spritesheets to reduce memory use</param>
		public Structure(Vector2 position, string structureID, List<Spritesheet> spritesheets = null) : base(RPG.CurrentGame)
		{
			Content = RPG.ContentManager;
			this.spritesheets = new List<Spritesheet>();
			if(spritesheets != null)this.spritesheets.AddRange(spritesheets);
			this.position = position;
			loadStructure(position, structureID);
		}
        #endregion

        #region Methods
        /// <summary>
        /// Moves the structure by a specified amount x and y
        /// </summary>
        /// <param name="x">Amount to move horizontally</param>
        /// <param name="y">Amount to move vertically</param>
        public void Move(int x, int y)
		{
			position += new Vector2(x,y);
			for(int i = 0; i < bounds.Count; i++)
				bounds[i] = new RectangleF(bounds[i].X + x, bounds[i].Y + y, bounds[i].Width, bounds[i].Height);
		}
		
		/// <summary>
		/// Draws structure with given spritebatch
		/// </summary>
		/// <param name="spriteBatch">Spritebatch to draw structure with</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			foreach(var box in elements)
			{
				Spritesheet sheet = spritesheets.Where (s => s.SpritesheetID == box.Item3).First();
				spriteBatch.Draw(sheet.Sheet, box.Item2 + Position, box.Item1, Color.White);
			}
		}
		
		/// <summary>
		/// Update structure for animation, etc
		/// </summary>
		/// <param name="gameTime">Snapshot of timing values</param>
		public override void Update(GameTime gameTime)
		{
            base.Update(gameTime);
		}
		#endregion
		
		#region Static Methods
		/// <summary>
		/// Loads a structure from an XML element
		/// </summary>
		/// <param name="structure">XML element to load structure from</param>
		/// <param name="sheets">Spritesheets to preload for lower memory use</param>
		/// <returns>New Structure from XML data</returns>
		public static Structure LoadFromXML(XElement structure, List<Spritesheet> sheets = null)
		{
			string structureID = structure.Element("StructureID").Value;
			Vector2 pos = VectorEx.FromArray(structure.Element("Position").Value.Split(','));
			return new Structure(pos, structureID, sheets);
		}
		#endregion
		
		#region Helpers
		private void loadStructure(Vector2 position, string structureID)
		{		
			elements = new List<Tuple<Rectangle, Vector2, string>>();
			bounds = new List<RectangleF>();
			string path = AppDomain.CurrentDomain.BaseDirectory + @"Content\structures\"+structureID+".xml";
			XDocument doc = XDocument.Load(path);//open xml file
			foreach(XElement element in doc.Descendants("Structure").Descendants("Element"))//Go through each element of structure
			{
				//load rectangle and position from array
				Rectangle rect = RectangleEx.FromArray(element.Element("Rectangle").Value.Split(','));
				Vector2 pos = VectorEx.FromArray(element.Element("Position").Value.Split(','));
				
				string sheetID = element.Element("spritesheet").Value;
				if(spritesheets.Count(s => s.SpritesheetID == sheetID) == 0) //if preloaded sheets don't have necessary spritesheet
					spritesheets.Add(new Spritesheet(sheetID));
				elements.Add(new Tuple<Rectangle, Vector2, string>(rect, pos, sheetID));              
			}
			foreach(XElement element in doc.Descendants("Structure").Descendants("Bounds").Elements("Box"))
			{
				RectangleF boundary = RectangleF.FromArray(element.Value.Split(','));
				boundary.Location += position;
				bounds.Add(boundary);
			}
		}
		#endregion
	}
}
