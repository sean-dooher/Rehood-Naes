using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rehood_Naes.Interfaces;

namespace Rehood_Naes.Entities
{
	#region Enums
	/// <summary>
	/// State of the sprite animation
	/// </summary>
	public enum SpriteState
	{
		///<summary>Represent when sprite is idle</summary>
		Idle,
		///<summary>Represents when sprite is walking</summary>
		Walk,
		///<summary>Represents when sprite is attacking</summary>
		Attack,
		///<summary>Represents when sprite is dying or dead</summary>
		Die
	}
		
	/// <summary>
	/// Direction of the sprite.
	/// Numbers represent row on spritesheet
	/// </summary>
	public enum SpriteDirection
	{
		///<summary>Represents facing Y axis</summary>
		North,
		///<summary>Represents facing away from X axis</summary>
		West,
		///<summary>Represents facing away from Y axis</summary>
		South,
		///<summary>Represents facing X axis</summary>
		East
	}
	#endregion
		
	/// <summary>
	/// Handles drawing logic for entities
	/// Frames are stored in following format:
	/// SpriteStates: [Length, Row]
	/// SpriteDirection: [Offset]
	/// </summary>
	public class CharacterSprite : IDrawable, IUpdateable
	{	
		#region Enums
		/// <summary>
		/// Represents info on a current frame
		/// </summary>
		public enum FrameInfo
		{
			/// <summary>
			/// Represents row frame is on
			/// </summary>
			Row,
			/// <summary>
			/// Represents number of frames in animation
			/// </summary>
			Number
		}
		#endregion

		#region Fields
		private double baseSpeed;
		private double speed; //speed of animation (Default .25x)
		private int currentRow;
		private double currentColumn;
		private int totalFrames; //total frames in animation
		private Vector2 position;
		private Vector2 size;
		private SpriteState currentState; 
		private SpriteDirection currentDirection;
		private Dictionary<Enum, int[]> frames;//dictionary of frames for current figure
		private List<Spritesheet> baseSheets;
		private List<Spritesheet> secondarySheets; //secondary sheets to draw on

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        #endregion

        #region Properties		
        /// <summary>
        /// Returns current frame of animation
        /// </summary>
        public int CurrentFrame
		{
			get { return (int)currentColumn; }
		}
		
		/// <summary>
		/// Returns amount of frames in animation
		/// </summary>
		public int MaxFrame
		{
			get { return totalFrames; }
		}
		/// <summary>
		/// Position of sprite in window
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}
		
		/// <summary>
		/// Represents total size of sprite
		/// </summary>
		public Vector2 Size
		{
			get { return size; }
		}
		
		/// <summary>
		/// Current state of sprite
		/// </summary>
		public SpriteState State
		{
			get { return currentState; }
			set 
			{	
				totalFrames = GetFrameInfo(FrameInfo.Number, value, Direction);
				currentRow = GetFrameInfo(FrameInfo.Row, value, Direction);
				currentState = value;
			}
		}
		
		/// <summary>
		/// Current direction of sprite
		/// </summary>
		public SpriteDirection Direction
		{
			get { return currentDirection; }
			set
			{
				currentDirection = value;
				currentRow = GetFrameInfo(FrameInfo.Row, State, value);
			}
		}

        public int DrawOrder => throw new NotImplementedException();

        public bool Visible => throw new NotImplementedException();

        public bool Enabled => throw new NotImplementedException();

        public int UpdateOrder => throw new NotImplementedException();
        #endregion

        #region Constructor
        /// <summary>
        /// Creates a character sprite to handle drawing logic
        /// </summary>
        /// <param name="position">Initial position on screen</param>
        /// <param name="characterID">ID of character to load</param>
        /// <param name="spriteSheets">List of non-base(overlay) spritesheets to add(armour, etc)</param>
        public CharacterSprite(Vector2 position, string characterID, List<Spritesheet> baseSheets, List<Spritesheet> secondarySheets = null)
		{
			this.baseSheets = baseSheets;
			this.secondarySheets = secondarySheets == null ? new List<Spritesheet>() : secondarySheets;
			loadCharacter(position, characterID);
		}
		#endregion
		
		#region Methods
		/// <summary>
		/// Replace all the spritesheets used to draw character except for the base ones
		/// </summary>
		/// <param name="sheets">New spritesheets used to draw</param>
		public void UpdateSpriteSheets(List<Spritesheet> sheets)
		{
			//resets secondary sheets and adds from input
			secondarySheets = new List<Spritesheet>();
			secondarySheets.AddRange (sheets);
		}
		
		/// <summary>
		/// Attempts to updates or adds animation frames to frame dictionary
		/// </summary>
		/// <param name="key">Key to add </param>
		/// <param name="frames">Frame indexes to add in format [length,toprow]</param>
		/// <returns>True if update was sucessful</returns>
		public bool UpdateFrame(Enum key, int[] frames)
		{
			if(key.GetType() == typeof(SpriteState) || key.GetType() == typeof(SpriteDirection))
			{
				this.frames[key] = frames;
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// Moves the character by specified pixels and animates a walk if player is not attacking
		/// </summary>
		/// <param name="x">Pixels to move horizontally</param>
		/// <param name="y">Pixels to move vertically</param>
		public bool Move(float x, float y)
		{
			if(State != SpriteState.Attack)
			{
				speed = baseSpeed * Math.Sqrt(x*x + y*y);
				position.X += x;
				position.Y += y;
				//Set direction
				if(Math.Abs(y) > Math.Abs(x))//If movement is more vertical than horizontal
				{
					Direction = y > 0 ? SpriteDirection.South : SpriteDirection.North;
				}
				else  //If movement is more horizontal than vertical
				{
					Direction = x > 0 ? SpriteDirection.East : SpriteDirection.West;	
				}
				if(Math.Abs(x) > 0 || Math.Abs(y) > 0) State = SpriteState.Walk;//if moving, set state to walk
				else State = SpriteState.Idle;
				return true;
			}
			return false;
		}	
		
		/// <summary>
		/// Updates the sprites animation
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if(State == SpriteState.Idle || State == SpriteState.Attack) 
				speed = baseSpeed;
			if(currentColumn < totalFrames && speed > 0)
			{
				currentColumn += speed;
			}
			else if(currentState == SpriteState.Die) { }
			else
			{
				if(State == SpriteState.Attack) State = SpriteState.Idle;//End attack after animation is done
				currentColumn = 0;
			}
		}
		
		/// <summary>
		/// Draw sprite, base textures first
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to draw with</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			Rectangle sourceRec = new Rectangle((int)currentColumn * (int)size.X, (int)currentRow * (int)size.Y, (int)size.X, (int)size.Y);
					
			//draw base textures first
			foreach(Spritesheet sheet in baseSheets)
			{
				spriteBatch.Draw(sheet.Sheet, position, sourceRec, Color.White);
			}
			
			//then draw non-base textures
			foreach(Spritesheet sheet in secondarySheets)
			{
				spriteBatch.Draw(sheet.Sheet, position, sourceRec, Color.White);
			}
		}
		#endregion

		#region Helpers
		private void loadCharacter(Vector2 position, string characterID)
		{
			frames = new Dictionary<Enum, int[]>();
			XDocument charDoc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"Content/characters/" + characterID + ".xml");
			
			//set speed values from xml file
			baseSpeed = double.Parse(charDoc.Descendants("Character").Elements("Speed").First().Value);
			speed = baseSpeed;
			
			//set draw rectangle from xml file
			size = VectorEx.FromArray(charDoc.Descendants("Character").Elements("Size").First().Value.Split(','));
			this.position = position;
				
			//set current direction from xml file
			currentDirection = (SpriteDirection)Enum.Parse(typeof(SpriteDirection), charDoc.Descendants("Character").Elements("Direction").First().Value);
			
			//set frame list from xml file
			foreach(XElement element in charDoc.Descendants("Character").Elements("Frame"))
			{
				Enum frameEnum;
				if(element.Element("Enum").Attribute("type").Value == "SpriteState")
					frameEnum = (SpriteState)Enum.Parse(typeof(SpriteState), element.Element("Enum").Value);
				else 
					frameEnum = (SpriteDirection)Enum.Parse(typeof(SpriteDirection), element.Element("Enum").Value);
				List<int> frameIndexes = new List<int>();
				foreach(string index in element.Element("Value").Value.Split(','))
				{
					frameIndexes.Add(int.Parse(index));
				}
				frames.Add(frameEnum, frameIndexes.ToArray());
			}
		}
		
		private int GetFrameInfo(FrameInfo type, SpriteState state, SpriteDirection direction)
		{
			if(type == FrameInfo.Row)
			{
				return frames[state][1] + (state == SpriteState.Die ? 0 : frames[direction][0]);
			}
			else if(type == FrameInfo.Number)
			{
				return frames[state][0];
			}
			else
				return -1;
		}

        public void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
