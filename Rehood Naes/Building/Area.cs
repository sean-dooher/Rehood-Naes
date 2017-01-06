using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using Rehood_Naes.Building;
using Rehood_Naes.Interfaces;
using Rehood_Naes.Entities;
using Rehood_Naes.Events;
using Rehood_Naes.Menus;
using IDrawable = Rehood_Naes.Interfaces.IDrawable;

namespace Rehood_Naes.Building
{
	/// <summary>
	/// An area that contains structures, their locations and logic for drawing and updating
	/// </summary>
	public class Area : IDrawable
	{
		#region Fields
		private Menu menu;
		private bool paused = false;
		private List<IDrawable> drawElements;
		private List<Entity> entities;
		private List<EventBox> boxes;
		private List<Spritesheet> sheets;
		private ContentManager Content;
		private GraphicsDevice graphics;
		private Song background;
        #endregion

        #region Properties
        public static Dictionary<string, Area> VisitedAreas = new Dictionary<string, Area>();

        /// <summary>
        /// Last state of keyboard
        /// </summary>
        public KeyboardState LastKeyboard;
		
		/// <summary>
		/// Last state of Mouse
		/// </summary>
		public MouseState LastMouse;
		
		/// <summary>
		/// User used in Area
		/// </summary>
		public Player User
		{
			get { return RPG.player; }
		}
		
		/// <summary>
		/// List of drawable elements
		/// </summary>
		public List<IDrawable> Elements
		{
			get { return drawElements; }
		}
		
		/// <summary>
		/// List of entities, not including player, used in area
		/// </summary>
		public List<Entity> Entities
		{
			get { return entities; }
		}
		
		/// <summary>
		/// List of EventBoxes used in area
		/// </summary>
		public List<EventBox> EventBoxes
		{
			get { return boxes; }
		}
		
		/// <summary>
		/// List of spritesheets used in Area
		/// </summary>
		public List<Spritesheet> Spritesheets
		{
			get{ return sheets; }
		}

		public Menu AreaMenu
		{
			get { return menu; }
		}

		public bool Paused {
			get { return paused; }
			set { paused = value; }
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Loads an area from a specified ID
		/// </summary>
		/// <param name="areaID"></param>
		public Area(string areaID)
		{
			graphics = RPG.GraphicsManager.GraphicsDevice;
			this.Content = RPG.ContentManager;
			loadArea(areaID);
            Area.VisitedAreas[areaID] = this;
		}
		#endregion
		
		#region Methods
		/// <summary>
		/// Add static elemant to draw
		/// </summary>
		/// <param name="element">Element to add</param>
		public void AddElement(IDrawable element)
		{
			drawElements.Add(element);
		}
		
		/// <summary>
		/// Updates everything in area based on user input
		/// </summary>
		public void Update(GameTime gameTime)
		{			
			if(!Paused) //don't update if menu is shown
			{	
				drawElements.ForEach(element => element.Update(gameTime));//update elements
				entityUpdates(gameTime);
			}
			
			User.Update(gameTime);	
			menu.Update(gameTime);
			LastKeyboard = Keyboard.GetState ();
			LastMouse = Mouse.GetState ();
		}
		
		/// <summary>
		/// Draws the area
		/// </summary>
		/// <param name="spriteBatch">Spritebatch to draw area with</param>
		public void Draw(SpriteBatch spriteBatch)
		{
				drawElements.ForEach(element => element.Draw(spriteBatch));
				foreach(Enemy enemy in entities.Where(entity => entity is Enemy))
					enemy.Draw(spriteBatch);
				
				if(RPG.DebugMode)
                    boxes.ForEach(box => box.DrawDebug(spriteBatch));
				
				User.Draw(spriteBatch);
            	menu.Draw(spriteBatch);
		}
		
		/// <summary>
		/// Checks for collision between an entity and structures
		/// </summary>
		/// <param name="entity">Entity used to test for collision</param>
		/// <returns>True if entity doesn't collide with anything in area, false otherwise</returns>
		public bool CheckForCollisions(Entity entity)
		{
			return CheckForCollisions(entity, new Vector2(0,0));
		}
		
		/// <summary>
		/// Checks for collision between an entity offset by a specified amount and structures
		/// </summary>
		/// <param name="entity">Entity to test for collision with</param>
		/// <param name="offset">Offset of entity coords</param>
		/// <returns>True if offset entity doesn't collide with anything in area, false otherwise</returns>
		public bool CheckForCollisions(Entity entity, Vector2 offset)
		{
			RectangleF bounds = entity.Bounds;
			bounds.Offset(offset);
			
			if(!graphics.Viewport.Bounds.Contains(bounds)) //if player is not in window
				return false;
			
			foreach(Structure struc in drawElements.Where(element => element is Structure))
			{
				
				if(struc.DrawBoxes.Count(box => box.Intersects(bounds)) != 0)
					return false;
			}
			
			if(entities.Count(ent => ent != entity && ent.Bounds.Intersects(bounds)) > 0)
				return false;
			if(!(entity is Player) && bounds.Intersects(User.Bounds))
				return false;
			
			return true;
		}
		#endregion
		
		#region Helpers
		private void entityUpdates(GameTime gameTime)
		{
			foreach (Enemy enemy in entities.Where(entity => entity is Enemy))
			{
				enemy.Update(gameTime);
			}
		}
		
        public static Area LoadArea(string areaID)
        {
            if (Area.VisitedAreas.Keys.Contains(areaID))
            {
                var area = Area.VisitedAreas[areaID];
                foreach (EventBox box in area.EventBoxes)
                    box.LoadBox(true);
                return area;
            }
            else
                return new Area(areaID);
        }

		/// <summary>
		/// Advances user to next area
		/// </summary>
		/// <param name="sender">Object command sent from</param>
		/// <param name="e">EventArgs that contain position and areaID</param>
		private void OnNewAreaEnter(Entity sender, BoxArgs e)
		{
			//if everything is either dead or in its original position
			if(entities.Count(entity => entity.State == SpriteState.Die) +
			   entities.Count(entity => (entity.InitialPosition - entity.Position).Length() <= 1) == entities.Count)
			{
                foreach (EventBox box in EventBoxes)
                    box.UnloadBox(true);
				User.Position = (e as NewAreaEventArgs).Position;
                User.CurrentArea = LoadArea((e as NewAreaEventArgs).AreaID);
			}
		}
		
		/// <summary>
		/// Spawns an entity
		/// </summary>
		/// <param name="sender">Object command is sent from</param>
		/// <param name="e">EventArgs that contain position, name, entityID and max number</param>
		private void Spawn(Entity sender, BoxArgs e)
		{
			EntitySpawnEventArgs args = e as EntitySpawnEventArgs;
			//get current count of entities of this type
			int currentNum = entities.Count(entity => entity.EntityName.Contains(args.Name)
			                                && entity.EntityID == args.EntityID);
			
			Entity potential = new Enemy(this, args.Position, args.Name + currentNum, args.EntityID, args.Direction, args.Bounds);
			if(args.Type == EntityType.Enemy && CheckForCollisions(potential))
				entities.Add(potential);
			//else 
				//entities.Add(new NPC(this, args.Position, args.Name + currentNum, args.EntityID));
		}
		
		/// <summary>
		/// Creates list of all strucutures in area by reading from specified area id
		/// </summary>
		/// <param name="areaID">ID of area to load</param>
		/// <returns>List of drawable elements in array</returns>
		private void loadArea(string areaID)
		{
			XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"Content\areas\" + areaID + ".xml");
			
			//create new lists for area
			drawElements = new List<IDrawable>();
			entities = new List<Entity>();
			sheets = new List<Spritesheet>();
			//create new eventboxes, replacing old ones if necessary
			if(boxes != null)
				boxes.ForEach(box => box.UnloadBox());
			boxes = new List<EventBox>();
				
			//loads main menu
			menu = new Menu(doc.Descendants("Area").Elements("Menu").First().Value);
			foreach(XElement element in doc.Descendants("Area").Descendants("Sheets").Descendants("SheetID"))
			{
				sheets.Add(new Spritesheet(element.Value));
			}
			
			if(doc.Descendants("Area").Elements("BGM").Count() > 0
                && doc.Descendants("Area").Elements("BGM").First().Element("MusicID").Value != "None") //load bgm if is contained in xml
			{
				background = Content.Load<Song>("sound/music/" + doc.Descendants("Area").Elements("BGM").First().Element("MusicID").Value);
				if(MediaPlayer.State != MediaState.Playing || MediaPlayer.Queue.ActiveSong.Name != background.Name)
				{
					MediaPlayer.Play(background);
					MediaPlayer.Volume = .3f;
					MediaPlayer.IsRepeating = bool.Parse(doc.Descendants("Area").Elements("BGM").First().Element("Loop").Value);
				}
			}
			
			foreach(XElement element in doc.Descendants("Area").Descendants("Tile"))//load tiles
				drawElements.Add(Tile.LoadFromXML(element, sheets));
			
			foreach(XElement element in doc.Descendants("Area").Descendants("Structure"))//load structures
				drawElements.Add(Structure.LoadFromXML(element, sheets));
			
			foreach(XElement element in doc.Descendants("Area").Elements("Entity"))
			{	
				EntityType type = (EntityType)Enum.Parse(typeof(EntityType), element.Element("Type").Value);
				SpriteDirection direction = (SpriteDirection)Enum.Parse(typeof(SpriteDirection), element.Element("Direction").Value);
				
				string entityID = element.Element("EntityID").Value;
				string name = element.Element("Name").Value;
				
				RectangleF bounds = RectangleF.FromArray(element.Element("Bounds").Value.Split(','));
				
				string[] pos = element.Element("Position").Value.Split(',');
				Vector2 position = new Vector2(float.Parse(pos[0]), float.Parse(pos[1]));
				
				EntitySpawnEventArgs args = new EntitySpawnEventArgs(entityID, name, type, 1, direction, position, bounds);
				Spawn(User, args);
			}
			
			foreach(XElement element in doc.Descendants("Area").Descendants("Player").Elements("EventBox"))//load player event boxes
			{
				RectangleF rect = RectangleF.FromArray(element.Element("Box").Value.Split(','));
				EventBox.Condition condition = null;
				if(element.Elements("Condition").Count() > 0)
				{
					string methodString = element.Element("Condition").Value;
					condition = (EventBox.Condition)Delegate.CreateDelegate(typeof(EventBox.Condition), this, methodString);
				}
				if(element.Element("Method").Value == "OnNewAreaEnter")
				{
					Vector2 position = VectorEx.FromArray(element.Element("Position").Value.Split(','));
					NewAreaEventArgs args = new NewAreaEventArgs(element.Element("AreaID").Value, position);
					boxes.Add(new EventBox(User, rect, OnNewAreaEnter, args, condition));
				}
				else if(element.Element("Method").Value == "Spawn")
				{
					int maxNum = int.Parse(element.Element("MaxNum").Value);
					Vector2 position = VectorEx.FromArray(element.Element("Position").Value.Split(','));
					EntityType type = (EntityType)Enum.Parse(typeof(EntityType), element.Element("Type").Value);
					SpriteDirection direction = (SpriteDirection)Enum.Parse(typeof(SpriteDirection), element.Element("Direction").Value);
					
					EntitySpawnEventArgs args = 
						new EntitySpawnEventArgs(element.Element("EntityID").Value,
						                         element.Element("Name").Value, type, maxNum, direction,
						                         position, RectangleF.FromArray(element.Element("Bounds").Value.Split(',')));
					boxes.Add(new EventBox(User, rect, Spawn, args, null));
				}
			}
		}
		#endregion
	}
}
