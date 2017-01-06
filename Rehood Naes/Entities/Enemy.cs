using System;
using System.Linq;
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
	/// Represents an on screen enemy type entity; Attacks player, etc
	/// </summary>
	public class Enemy : Entity
	{
		#region Fields
		private const int cooldown = 90; //sets default cooldown
		private string path;
		private int currentCD;
		private SpriteDirection startDirection;
		private RectangleF walkLimit;
		#endregion
		
		#region Constructors
		/// <summary>
		/// Creates new enemy in a certain area at a certain positon with a certain name, ID, direction and bounds
		/// </summary>
		/// <param name="currentArea">Area enemy is in</param>
		/// <param name="position">Position of enemy</param>
		/// <param name="name">Unique name of entity</param>
		/// <param name="enemyID">ID of enemy</param>
		/// <param name="direction">Default direction enemy faces</param>
		/// <param name="bounds">Area that enemy can move in</param>
		public Enemy(Area currentArea, Vector2 position, string name, string enemyID, SpriteDirection direction, RectangleF bounds) : base(currentArea, position, name, enemyID)
		{
			path = AppDomain.CurrentDomain.BaseDirectory + @"Content\enemies\" + enemyID + ".xml";
			
			walkLimit = new RectangleF(bounds.Location, bounds.Size);
			
			currentCD = 0;
			
			startDirection = direction;
			base.LoadEntity(position, path);
		}
		#endregion
		
		#region Methods
		/// <summary>
		/// Updates enemy logic
		/// </summary>
		/// <param name="gameTime"></param>
		public new void Update(GameTime gameTime)
		{
			MoveUpdate(gameTime);
			base.Update(gameTime);
		}
		
//		public new void Draw(SpriteBatch spriteBatch)
//		{
//			SpriteFont font = RPG.ContentManager.Load<SpriteFont>("fonts/Arial");
//			Vector2 size = font.MeasureString(this.Position.ToString());
//			Vector2 pos = new Vector2(Bounds.Center.X - size.X/2, Position.Y - size.Y);
//			spriteBatch.DrawString(font, this.Position.ToString(), pos, Color.White);
//			base.Draw(spriteBatch);
//		}
		
		private void MoveUpdate(GameTime gameTime)
		{
			if(sprite.State != SpriteState.Die)
			{
				//set bounds
				RectangleF tempBounds = Bounds;
				RectangleF tempPlayer = CurrentArea.User.Bounds;
				
				tempBounds.Inflate(2f, 2f);
				tempBounds.Offset(-1f, -1f);
				
				Vector2 moveAmount = CurrentArea.User.Position - this.Position;
				moveAmount.Normalize();
				moveAmount = Vector2.Multiply(moveAmount, .8f);
				
				bool turn = true;
				
				//if player is in bounds, isn't too close to enemy and there are no collisions
				//move towards the player
				if(!(tempPlayer.Intersects(tempBounds)) 
				   && CurrentArea.CheckForCollisions(this, moveAmount) 
				   && walkLimit.Contains(tempBounds) && walkLimit.Contains(tempPlayer))
				{
					Move(moveAmount);
				}
				//if player is out of bounds return to start
				else if(!walkLimit.Contains(tempPlayer))
				{
					moveAmount = InitialPosition - Position;
					moveAmount.Normalize();
					tempBounds.Offset(moveAmount);
					if(walkLimit.Contains(tempBounds) && CurrentArea.CheckForCollisions(this, moveAmount) && (Position - InitialPosition).Length() > 1 )
						Move(moveAmount);
					if((Position - InitialPosition).Length() <= 1)
					{
						turn = false;
						Turn(startDirection);
					}
		        }
				else if(tempPlayer.Intersects(tempBounds) && sprite.State != SpriteState.Attack)
				{
					if(currentCD == 0)
					{
						Attack();
						currentCD++;
					}
				}
				
				if(turn)
					Turn(moveAmount);
				if(currentCD != 0 && currentCD != cooldown)
					currentCD++;
				else if(currentCD == cooldown)
					currentCD = 0;			
			}
		}
		
		private void Attack()
		{
			if(sprite.State != SpriteState.Attack && sprite.State != SpriteState.Die)
			{
				TryAttack(CurrentArea.User);
				sprite.State = SpriteState.Attack;
			}
		}
		#endregion
	}
}
