using System;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
	/// Represents the user character
	/// </summary>
	public class Player : Entity
	{
		#region Fields
		private int deathCooldown;
		private ContainerMenu inventoryMenu;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Creates a new player in a certain area with a certain position, name and playerID
		/// </summary>
		/// <param name="currentArea">Area to load in</param>
		/// <param name="position">Initial position</param>
		/// <param name="name">Name of player</param>
		/// <param name="playerID">ID of player</param>
		public Player(Area currentArea, Vector2 position, string name, string playerID) : base(currentArea, position, name, playerID)
		{
			LoadEntity(position, AppDomain.CurrentDomain.BaseDirectory + @"Content/player/" + playerID + ".xml");
			inventoryMenu = new ContainerMenu (inventory, "inventory");
		}
		#endregion
		
		#region Methods	
		/// <summary>
		/// Tries to attack everything in area; plays animation
		/// </summary>
		public void Attack()
		{
			if(sprite.State != SpriteState.Attack)
			{
				TryAttack(CurrentArea.Entities);
				sprite.State = SpriteState.Attack;
			}
		}
		
		/// <summary>
		/// Controls death logic
		/// </summary>
		public new void Die()
		{
			deathCooldown = 0;
			base.Die();
		}
		
		/// <summary>
		/// Updates player and frame moving logic
		/// </summary>
		/// <param name="gameTime">Snapshot of times</param>
		public new void Update(GameTime gameTime)
		{
			MouseState mouse = Mouse.GetState();
			KeyboardState keyboard = Keyboard.GetState();
			KeyboardState lastKeyboard = CurrentArea.LastKeyboard;

			if (keyboard.IsKeyUp (Keys.Escape) && lastKeyboard.IsKeyDown (Keys.Escape))//toggle main menu with escape key
			{
				CurrentArea.AreaMenu.isShowing = !CurrentArea.AreaMenu.isShowing;
				CurrentArea.Paused = CurrentArea.AreaMenu.isShowing;
				inventoryMenu.isShowing = false;
			}
			if (lastKeyboard.IsKeyDown (Keys.E) && keyboard.IsKeyUp (Keys.E) && !CurrentArea.AreaMenu.isShowing)
			{
				inventoryMenu.isShowing = !inventoryMenu.isShowing;
				CurrentArea.Paused = inventoryMenu.isShowing || CurrentArea.AreaMenu.isShowing;
			}

			RPG.MouseVisible = CurrentArea.AreaMenu.isShowing || inventoryMenu.isShowing; //enable mouse if menu is shown
			if(sprite.State != SpriteState.Die && !CurrentArea.Paused)
			{				
				Vector2 moveDirection = Vector2.Zero;
				if(mouse.LeftButton == ButtonState.Pressed)
					Attack();
				
				if(keyboard.IsKeyDown(Keys.F1) && !lastKeyboard.IsKeyDown(Keys.F1))
		    		RPG.DebugMode = !RPG.DebugMode;
				
				//movement vectors
				if(keyboard.IsKeyDown(Keys.W)) 
				{
					Turn(SpriteDirection.North);
					moveDirection.Y--;
				}
		    	if(keyboard.IsKeyDown(Keys.A)) 
		    	{
		    		Turn(SpriteDirection.West);
		    		moveDirection.X--;
		    	}
		    	if(keyboard.IsKeyDown(Keys.S)) 
		    	{
		    		Turn(SpriteDirection.South);
		    		moveDirection.Y++;
		    	}
		    	if(keyboard.IsKeyDown(Keys.D)) 
		    	{
		    		Turn(SpriteDirection.East);
		    		moveDirection.X++;
		    	}	
		    	//keep speed constant
				moveDirection.Normalize();
				moveDirection *= 2;
		    	
				if (keyboard.IsKeyDown (Keys.F) && lastKeyboard.IsKeyUp (Keys.F))
					inventory.AddItem (new Item (CurrentArea, Vector2.Zero, 1, Item.ItemState.Bag));

		   		if(keyboard.IsKeyDown(Keys.LeftShift)) 
		    		moveDirection *= 3; //sprint or speed key
		   		
		    	if(moveDirection.Length() > 0 && CurrentArea.CheckForCollisions(this, moveDirection))
		    		Move(moveDirection);
		    	
				else if(moveDirection.Length() > 0 && keyboard.IsKeyDown(Keys.Space) && RPG.DebugMode)
		    		Move(moveDirection);//debug override key
				this.Position = new Vector2((int)Position.X, (int)Position.Y);
			}
	
			if(sprite.State == SpriteState.Die && sprite.CurrentFrame == sprite.MaxFrame && deathCooldown == 120)
        		RPG.LoadNewGame();
			else if(sprite.State == SpriteState.Die 
			        && sprite.CurrentFrame == sprite.MaxFrame && deathCooldown < 120)
				deathCooldown++;

        	base.Update(gameTime);
			inventoryMenu.Update (gameTime);
		}
		
		/// <summary>
		/// Draws player with a given SpriteBatch
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to draw with</param>
		public new void Draw(SpriteBatch spriteBatch)
		{
			base.Draw(spriteBatch);
			inventoryMenu.Draw (spriteBatch);
		}
		#endregion
	}
}

