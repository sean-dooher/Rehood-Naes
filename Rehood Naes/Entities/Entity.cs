using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Rehood_Naes.Building;
using Rehood_Naes.Interfaces;
using Rehood_Naes.Entities;
using Rehood_Naes.Events;
using Rehood_Naes.Menus;
using IDrawable = Rehood_Naes.Interfaces.IDrawable;

namespace Rehood_Naes.Entities
{
	/// <summary>
	/// Base class for all entity objects; represents an entity on screen with specific logic
	/// </summary>
	public abstract class Entity : IDrawable
	{
		#region Fields
		private double currentHealth;
		private int maxHealth;
		private double regenRate;
		private int strength;
		private Vector2 initalPosition;
		Vector2 healthOffset;
		private Vector2 size;
		private Vector2 offset;
		private Area currentArea;
		private ProgressBar healthBar;
        private string entityID;
		//protected StorageContainer inventory;
		//protected StorageContainer equipment;
		
		/// <summary>
		/// Last position of entity
		/// </summary>
		protected Vector2 lastPos;
		
		/// <summary>
		/// Sprite used to draw entity
		/// </summary>
		protected CharacterSprite sprite;
		
		/// <summary>
		/// Content manager used to load new content
		/// </summary>
		protected ContentManager Content;
		#endregion

		#region Events
		/// <summary>
		/// Represents acceptable methods to handle move events
		/// </summary>
		public delegate void MoveEvent(object sender, Vector2 moveVector);
		
		/// <summary>
		/// Event to invoke when entity moves
		/// </summary>
		public event MoveEvent On_Move;
		
		/// <summary>
		/// Represents acceptable methods to handle health change
		/// </summary>
		public delegate void AmountChange(object sender, int currentHealth);
		
		/// <summary>
		/// Event to invoke when health changes
		/// </summary>
		public event AmountChange On_HealthChanged;
		#endregion
		
		#region Properties
		/// <summary>
		/// Provides current position of entity on screen
		/// </summary>
		public Vector2 Position
		{
			get 
			{ return sprite.Position + offset; }
			set 
			{
				sprite.Position = value - offset;
				if (healthBar != null)
					healthBar.Position = new Vector2 ((Bounds.Center + healthOffset).X, (Position + healthOffset).Y);
			}
		}

		/// <summary>
		/// Gets initial position of entity
		/// </summary>
		public Vector2 InitialPosition
		{
			get { return initalPosition; }
		}
		
		/// <summary>
		/// Provides a boundary box to be used for collision detection
		/// </summary>
		public RectangleF Bounds
		{
			get { return new RectangleF(Position, size); }
		}

		/// <summary>
		/// Current direction sprite is facing
		/// </summary>
		public SpriteDirection Direction
		{
			get { return sprite.Direction; }
		}
		
		/// <summary>
		/// Current state of sprite
		/// </summary>
		public SpriteState State
		{
			get { return sprite.State; }
		}
				
		/// <summary>
		/// Current Area sprite is in
		/// </summary>
		public Area CurrentArea
		{
			get { return currentArea; }
			set {
                currentArea = value;
            }
		}
			
		/// <summary>
		/// Unique identifier for Entity
		/// </summary>
		public string EntityName
		{
			get; protected set;
		}
		
		/// <summary>
		/// Identifier unique for entity type
		/// </summary>
		public string EntityID
		{
			get { return entityID; }
            private set { entityID = value; }
        }
		
		/// <summary>
		/// Max health of entity
		/// </summary>
		public int MaxHealth
		{
			get { return maxHealth; }
		}

		/// <summary>
		/// Current health of entity
		/// </summary>
		public double CurrentHealth
		{
			get { return currentHealth; }
			protected set 
			{
				if(On_HealthChanged != null) On_HealthChanged.Invoke(this, (int)value);
				currentHealth = value;
			}
		}
		
		/// <summary>
		/// Amount of damage one hit does
		/// </summary>
		public int Strength
		{
			get { return strength; }
		}
		#endregion

		#region Constructors
		/// <summary>
		/// Creates new Entity at a position in an area with a specified name
		/// </summary>
		/// <param name="startingArea">Area to load entity in</param>
		/// <param name="position">Position of entity</param>
		/// <param name="name">Name of entity</param>
		protected Entity(Area startingArea, Vector2 position, string name, string entityID)
		{
			EntityName = name;
            EntityID = entityID;
			Content = RPG.ContentManager;
			initalPosition = position;
			lastPos = new Vector2(position.X,position.Y);
			currentArea = startingArea;
		}
		#endregion

		#region Methods
		/// <summary>
		/// Turn character in certain direction
		/// </summary>
		/// <param name="direction">Direction to turn</param>
		public void Turn(SpriteDirection direction)
		{
			sprite.Direction = direction;
		}
		
		/// <summary>
		/// Turns sprite in a certain direction based on the move vector
		/// </summary>
		/// <param name="direction">Direction to turn</param>
		public void Turn(Vector2 direction)
		{
			if(Math.Abs(direction.Y) > Math.Abs(direction.X))//If movement is more vertical than horizontal
			{
				Turn(direction.Y > 0 ? SpriteDirection.South : SpriteDirection.North);
			}
			else  //If movement is more horizontal than vertical
			{
				Turn(direction.X > 0 ? SpriteDirection.East : SpriteDirection.West);
			}
		}
		
		/// <summary>
		/// Move character amount pixels in current direction
		/// </summary>
		/// <param name="amount">Amount of pixels to move</param>
		public void Move(float amount)
		{
			if(sprite.State != SpriteState.Die){Vector2 moveAmount = Vector2.Zero;
				if(sprite.Direction == SpriteDirection.North) moveAmount.Y--;
				else if(sprite.Direction == SpriteDirection.South) moveAmount.Y++;
				else if(sprite.Direction == SpriteDirection.East) moveAmount.X++;
				else if(sprite.Direction == SpriteDirection.West) moveAmount.X--;
				moveAmount *= amount;
				Move(moveAmount);
			}
		}
		
		/// <summary>
		/// Moves the entity by a specified amount of pixels
		/// </summary>
		/// <param name="x">Pixels in x direction to move</param>
		/// <param name="y">Pixels in y direction to move</param>
		public void Move(float x, float y)
		{
			if(sprite.State != SpriteState.Die && sprite.Move(x,y) && On_Move != null)
				On_Move(this, new Vector2(x, y));
		}
		
		/// <summary>
		/// Moves the entity by a specified offset
		/// </summary>
		/// <param name="offset">Vector offset to move entity by</param>
		public void Move(Vector2 offset)
		{
			Move(offset.X, offset.Y);
		}
		
		/// <summary>
		/// Try to attack entities in list
		/// </summary>
		/// <param name="entities">Entities to try to attack</param>
		public void TryAttack(List<Entity> entities)
		{
			foreach (Entity entity in entities) 
			{
				TryAttack (entity);
			}
		}
		
		/// <summary>
		/// Tries to attack a specific entity
		/// </summary>
		/// <param name="entity">Entity to attack</param>
		public void TryAttack(Entity entity)
		{
			if(sprite.State != SpriteState.Die)
			{
				RectangleF tempBounds = this.Bounds;
				tempBounds.Inflate(2f,2f);
				tempBounds.Offset(-1f, -1f);
				if(tempBounds.Intersects(entity.Bounds))
					entity.Damage(strength);
			}
		}
		
		/// <summary>
		/// Damages the entity by a certain amount
		/// </summary>
		/// <param name="amount">Amount to damage by</param>
		public void Damage(int amount)
		{
			if(sprite.State != SpriteState.Die)
			{
				currentHealth -= amount;
				if(On_HealthChanged != null)
					On_HealthChanged(this, (int)currentHealth);
				if(currentHealth <= 0)
					Die();
			}
		}
		
		/// <summary>
		/// Controls death logic, can be overridden
		/// On default 
		/// </summary>
		protected void Die()
		{
			if(healthBar != null)
			{
				On_Move -= healthBar.Move;
				On_HealthChanged -= healthBar.ChangeProgress;
			}
			foreach(EventBox box in currentArea.EventBoxes)
				box.UnloadBox(this);
			sprite.State = SpriteState.Die;
			size = Vector2.Zero;
		}
		
		/// <summary>
		/// Draws the entity with provided SpriteBatch
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to draw entity with</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			sprite.Draw(spriteBatch);
			if(healthBar != null && sprite.State != SpriteState.Die)
				healthBar.Draw(spriteBatch);
			if(RPG.DebugMode)
			{
				(new Tile(new Rectangle((int)Bounds.Left, (int)Bounds.Top, (int)Bounds.Width, 1), "black1")).Draw(spriteBatch);
				(new Tile(new Rectangle((int)Bounds.Left, (int)Bounds.Top, 1, (int)Bounds.Height), "black1")).Draw(spriteBatch);
				(new Tile(new Rectangle((int)Bounds.Left, (int)Bounds.Bottom, (int)Bounds.Width, 1), "black1")).Draw(spriteBatch);
				(new Tile (new Rectangle ((int)Bounds.Right, (int)Bounds.Top, 1, (int)Bounds.Height), "black1")).Draw(spriteBatch);
			}
		}

		/// <summary>
		/// Updates the entity
		/// </summary>
		public void Update(GameTime gameTime)
		{
			if(Position.Equals(lastPos) && sprite.State != SpriteState.Attack && sprite.State != SpriteState.Die)
				sprite.State = SpriteState.Idle;
			lastPos = new Vector2(Position.X,Position.Y);
			sprite.Update(gameTime);
			if(healthBar != null)
				healthBar.Update(gameTime);
			if(CurrentHealth < MaxHealth)
				CurrentHealth += regenRate;
			else if(CurrentHealth > MaxHealth)
				CurrentHealth = MaxHealth;
		}
		#endregion
		
		#region Helpers
		/// <summary>
		/// Load entity at position from loadpath
		/// </summary>
		/// <param name="position">Position to create entity at</param>
		/// <param name="loadPath">Path to load entity from</param>
		protected void LoadEntity(Vector2 position, string loadPath)
		{
			XDocument entityDoc = XDocument.Load (loadPath);
			var entityXML = entityDoc.Element ("Entity");
			string characterID = entityXML.Elements ("Base").First ().Value;
			maxHealth = int.Parse (entityXML.Elements ("MaxHealth").First ().Value);
			currentHealth = int.Parse (entityXML.Elements ("CurrentHealth").First ().Value);
			strength = int.Parse (entityXML.Elements ("Strength").First ().Value);
			regenRate = double.Parse (entityXML.Elements ("Regen").First ().Value) / 60;//divide by 60 to make it per second
			//load inventory

			//if (entityXML.Elements ("InventorySize").Count () != 0)
			//	inventory = new StorageContainer (int.Parse (entityXML.Element ("InventorySize").Value));
			//else
			//	inventory = new StorageContainer (0);

			
			List<Spritesheet> defaultOverlay = new List<Spritesheet> (); //TODO: Implement gear system
			
			//load offset and size from xml
			offset = VectorEx.FromArray (entityXML.Elements ("Offset").First ().Value.Split (','));
			size = VectorEx.FromArray (entityXML.Elements ("Size").First ().Value.Split (','));
			
			//add spritesheets and load spritesheet lists
			foreach (XElement element in entityXML.Elements("Spritesheet"))
				defaultOverlay.Add (new Spritesheet (element.Value));
			foreach (XElement element in entityXML.Elements("SpriteList"))
				defaultOverlay.AddRange (Spritesheet.LoadList (element.Value));
			
			//create sprite
			sprite = new CharacterSprite (position - offset, characterID, defaultOverlay);
			
			//load healthbar if it has one
			if (entityXML.Elements ("HealthBar").Count () > 0)
			{
				XElement healthBar = entityXML.Elements ("HealthBar").First ();
				
				//set colors front document
				string[] frontString = healthBar.Element ("FrontColor").Value.Split (',');
				string[] backString = healthBar.Element ("BackColor").Value.Split (',');
				Color front = new Color (int.Parse (frontString [0]), int.Parse (frontString [1]), int.Parse (frontString [2]));
				Color back = new Color (int.Parse (backString [0]), int.Parse (backString [1]), int.Parse (backString [2]));
				
				//get size and set position
				int buffer = int.Parse (healthBar.Element ("Buffer").Value);
				Vector2 barSize = VectorEx.FromArray (healthBar.Element ("Size").Value.Split (','));
				Vector2 barPosition = new Vector2 (Bounds.Center.X - barSize.X / 2, Position.Y - barSize.Y - buffer);
				this.healthBar = new ProgressBar (barPosition, barSize, maxHealth, (int)currentHealth, front, back);
				healthOffset = new Vector2 ((this.healthBar.Position - Bounds.Center).X, (this.healthBar.Position - Position).Y);
				
				On_Move += this.healthBar.Move;
				On_HealthChanged += this.healthBar.ChangeProgress;
			}
			
			//replace frames if necessary [MOVE TO CHARACTER SPRITE]
			foreach (XElement element in entityXML.Elements("Frame"))
			{
				Enum frameEnum;
				if (element.Element ("Enum").Attribute ("type").Value == "SpriteState")
					frameEnum = (SpriteState)Enum.Parse (typeof(SpriteState), element.Element ("Enum").Value);
				else
					frameEnum = (SpriteDirection)Enum.Parse (typeof(SpriteDirection), element.Element ("Enum").Value);
				List<int> frameIndexes = new List<int> ();
				foreach (string index in element.Element("Value").Value.Split(','))
				{
					frameIndexes.Add (int.Parse (index));
				}
				sprite.UpdateFrame (frameEnum, frameIndexes.ToArray ());
			}
		}
		#endregion
	}
}
