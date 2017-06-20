using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Rehood_Naes.Interfaces;
using IDrawable = Rehood_Naes.Interfaces.IDrawable;

namespace Rehood_Naes.Menus
{
	/// <summary>
	/// Represents a button to use in a menu
	/// </summary>
	public class Button : IDrawable
	{
		#region Fields
		private MouseState lastState;
		private List<Spritesheet> spriteSheets;
		private string normalID;
		private string hoverID;
		private string buttonID;
		private Rectangle buttonBounds;
		private SpriteFont textFont;
		private int state; //0 == normal, 1 == hover
		#endregion
		
		#region Events
		/// <summary>
		/// Represents methods that can be used to handle ButtonClicks
		/// </summary>
		public delegate void ButtonClick(object sender, MouseState CurrentState);
		
		/// <summary>
		/// Event to use whenever button is clicked
		/// </summary>
		public event ButtonClick On_Click;
		#endregion
		
		#region Properties
		/// <summary>
		/// Position of button on screen
		/// </summary>
		public Vector2 Position
		{
			get { return new Vector2(buttonBounds.X, buttonBounds.Y); }
		}

		public Vector2 Size
		{
			get { return new Vector2 (buttonBounds.Width, buttonBounds.Height); }
		}

		public Rectangle Bounds
		{
			get { return buttonBounds; }
		}
		/// <summary>
		/// ID of button
		/// </summary>
		public string ButtonID
		{
			get { return buttonID; }
		}
		
		/// <summary>
		/// Text on button
		/// </summary>
		public string ButtonText
		{
			get; set;
		}
			
		#endregion
		
		#region Constructors
		/// <summary>
		/// Creates new button at a certain position with certain id, text, spritesheets to use for caching, and font
		/// </summary>
		/// <param name="position">Position of button</param>
		/// <param name="buttonText">Text on button</param>
		/// <param name="buttonID">ID of button</param>
		/// <param name="sheets">Location of button</param>
		/// <param name="font">Font to use to write on button</param>
		public Button(Vector2 position, string buttonText, string buttonID, List<Spritesheet> sheets = null, SpriteFont font = null)
		{
			XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory +
			                               @"Content\menus\buttons\" + buttonID + ".xml");
			this.buttonID = buttonID;
			ButtonText = buttonText;
			textFont = font;
			
			if(sheets == null)
				spriteSheets = new List<Spritesheet>();
			else 
				spriteSheets = new List<Spritesheet>(sheets);
			
			Vector2 size = VectorEx.FromArray(doc.Descendants("Button").Elements("Size").First().Value.Split(','));
			buttonBounds = new Rectangle(position.ToPoint(), size.ToPoint());
			
			//get base image
			normalID = doc.Descendants("Button").Elements("Image").First().Value;
			if(spriteSheets.Count(sheet =>sheet.SpritesheetID == normalID) == 0)
				spriteSheets.Add(new Spritesheet(normalID));
			
			//get hover image
			hoverID = doc.Descendants("Button").Elements("Hover").First().Value;
			if(spriteSheets.Count(sheet => sheet.SpritesheetID == hoverID) == 0)
				spriteSheets.Add(new Spritesheet(hoverID));
			
			//if there is no font loaded, use the default
			if(textFont == null) 
				textFont = RPG.ContentManager.Load<SpriteFont>(@"fonts\InterfaceFont");
			
			lastState = Mouse.GetState();
		}
		#endregion
		
		#region Methods
		/// <summary>
		/// Updates button, checks for click and hover
		/// </summary>
		/// <param name="gameTime">Snapshot of timing values</param>
		public void Update(GameTime gameTime)
		{
			if(buttonBounds.Contains(Mouse.GetState().Position))
			{
				state = 1;
				if(On_Click != null && 
				   lastState.LeftButton == ButtonState.Pressed && Mouse.GetState().LeftButton == ButtonState.Released)
					On_Click(this, Mouse.GetState());
			}
			else state = 0;
			lastState = Mouse.GetState();
		}
		
		/// <summary>
		/// Draws button with certain SpriteBatch
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to use</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			if(state == 0)
				spriteBatch.Draw(spriteSheets.First(sheet => sheet.SpritesheetID == normalID).Sheet,
			                                buttonBounds, Color.White);
			else
				spriteBatch.Draw(spriteSheets.First(sheet => sheet.SpritesheetID == hoverID).Sheet,
			                                buttonBounds, Color.White);	
			spriteBatch.DrawString(textFont, ButtonText, 
			                       new Vector2(buttonBounds.X + buttonBounds.Width/2 - textFont.MeasureString(ButtonText).X/2,
				                              buttonBounds.Y + buttonBounds.Height/2 -  textFont.MeasureString(ButtonText).Y/2), Color.White);
		}
		#endregion
	}
}
