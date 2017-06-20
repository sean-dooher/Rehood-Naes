using System;
using System.Windows.Forms;
using System.Linq;
using System.Xml.Linq;
using System.Reflection;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using Rehood_Naes.Interfaces;

namespace Rehood_Naes.Menu
{
	/// <summary>
	/// Menus overlay specific elements and buttons for the user to interact with
	/// </summary>
	public class Menu : DrawableGameComponent
	{
		#region Fields
		protected List<IMenuElement> elements;
		protected List<Spritesheet> sheets;
		#endregion
		
		#region Properties
		/// <summary>
		/// Whether or not the menu is showing
		/// </summary>
		public bool isShowing
		{
			get; set;
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Loads a new menu from an id
		/// </summary>
		/// <param name="menuID">ID to load from</param>
		public Menu(string menuID) : base(RPG.CurrentGame)
		{
			loadMenu(menuID);
		}
		#endregion
		
		#region Public Methods
		/// <summary>
		/// Updates elements in menu
		/// </summary>
		/// <param name="gameTime">Snapshot of timing values</param>
		public void Update(GameTime gameTime)
		{
			if(isShowing)
				elements.ForEach(element => element.Update(gameTime));
		}
		
		/// <summary>
		/// Draws menu elements with given SpriteBatch
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch used</param>
		public void Draw(GameTime gameTime)
        { 
			if(isShowing)
				elements.ForEach(element => element.Draw(gameTime));
		}
		#endregion
		
		#region Helpers
		private void exit(object sender, MouseState mouse)
		{
			isShowing = false;
		}
				
		private void openOptions(object sender, MouseState mouse)
		{
			MessageBox.Show("NOT IMPLEMENTED");
		}
				
		private void saveGame(object sender, MouseState mouse)
		{
			MessageBox.Show("NOT IMPLEMENTED");
		}
		
		private void loadGame(object sender, MouseState mouse)
		{
			MessageBox.Show("NOT IMPLEMENTED");
		}
		
		private void exitGame(object sender, MouseState mouse)
		{
			MessageBox.Show ("NOT IMPLEMENTED");
		}
		
		protected void loadMenu(string menuID)
		{
			sheets = new List<Spritesheet>();
			elements = new List<IMenuElement>();
			XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"Content\menus\"+menuID+".xml");
			doc.Descendants("Menu").Elements("Spritesheet").ToList().ForEach(element => sheets.Add(new Spritesheet(element.Value)));
			foreach(XElement element in doc.Descendants("Button"))
			{
				string buttonID = element.Elements("ButtonID").First().Value;
				Vector2 position = VectorEx.FromArray(element.Elements("Position").First().Value.Split(','));
				string text = element.Elements("Text").First().Value;
				Button temp = new Button(position, text, buttonID, sheets);
				
				foreach(string methodString in element.Elements("Method"))
				{
					temp.On_Click += (Button.ButtonClick)Delegate.CreateDelegate(typeof(Button.ButtonClick), this, methodString);
				}
				elements.Add(temp);
			}
			foreach(XElement element in doc.Descendants("Tiles"))
			{
				
			}
		}
		#endregion
	}
}
