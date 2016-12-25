using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Rehood_Naes.Interfaces
{
	/// <summary>
	/// Represents a texture with an ID for ease of loading and caching
	/// </summary>
	public class Spritesheet
	{
		#region Field
		private string sheetID;
		Texture2D sheet;
		private static Dictionary<string, Spritesheet> sheets;
		#endregion		
		
		#region Properties
		/// <summary>
		/// ID of spritesheet
		/// </summary>
		public string SpritesheetID
		{
			get { return sheetID; }
		}	
		
		/// <summary>
		/// Texture of sheet
		/// </summary>
		public Texture2D Sheet
		{
			get { return sheet; }
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Creates new spritesheet with given ID and texture
		/// </summary>
		/// <param name="sheet">Texture to use</param>
		/// <param name="spriteID">ID to use</param>
		public Spritesheet(Texture2D sheet, string spriteID)
		{
			sheetID = spriteID;
			this.sheet = sheet;
		}
		
		/// <summary>
		/// Loads new spritesheet with given ID
		/// </summary>
		/// <param name="spriteID">ID to load</param>
		public Spritesheet(string spriteID)
		{
			try
			{
				sheets.Count();
			}
			catch
			{
				sheets = new Dictionary<string, Spritesheet> ();
			}
			XDocument listDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"Content/spritesheets/sheets.xml");
			string path = listDoc.Descendants("List").Elements("Spritesheet").First(element => element.Element("ID").Value == spriteID).Element("Path").Value;			
			sheet = sheets.ContainsKey(spriteID) ? sheets[spriteID].Sheet : RPG.ContentManager.Load<Texture2D>(path);
			this.sheetID = spriteID;
		}
		#endregion
		
		#region Static Methods
		/// <summary>
		/// Loads a list of related spritesheets from a given list id
		/// </summary>
		/// <param name="listID">ID of list to load</param>
		/// <returns>List of related spritesheets</returns>
		public static List<Spritesheet> LoadList(string listID)
		{
			List<Spritesheet> sheet = new List<Spritesheet>();
			XDocument listDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"Content/spritesheets/sheets.xml");
			foreach(XElement element in listDoc.Descendants("List").Elements("Spritesheet")
			        .Where(ele => ele.Elements("SpriteList").Count() > 0 && ele.Element("SpriteList").Value == listID))
			{
				sheet.Add(new Spritesheet(element.Element("ID").Value));
			}
			return sheet;
		}
		#endregion
	}
}
