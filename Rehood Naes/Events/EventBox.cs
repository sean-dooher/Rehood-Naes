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

namespace Rehood_Naes.Events
{
	/// <summary>
	/// Provides 
	/// </summary>
	public class EventBox
	{
		private Action<Entity, BoxArgs> executeOnEntry;
		private RectangleF enterRect;
		private BoxArgs eventParams;
		private List<Entity> checkList;
        private Dictionary<Entity, bool> enabled;
        private Dictionary<Entity, bool> saved;
		private List<Entity> entered;
		private Condition checkCondition;
		
		#region Events
		/// <summary>
		/// Checks if something is true before executing
		/// </summary>
		public delegate bool Condition(object sender, Vector2 moveAmount);
        #endregion

        /// <summary>
        /// Creates an event box that executes an action when any entity in a list enters a certain area
        /// </summary>
        /// <param name="entities">Entities that can trigger event</param>
        /// <param name="onEnter">Rectangle that entry into triggers event</param>
        /// <param name="execute">Action to execute</param>
        /// <param name="eventParams">BoxID</param>
        /// <param name="condition">Condition that must be true before event with execute</param>
        public EventBox(List<Entity> entities, RectangleF onEnter, Action<Entity, BoxArgs> execute, BoxArgs eventParams, Condition condition = null)
        {
            //add list of entities
            checkList = entities;
            enabled = new Dictionary<Entity, bool>();
            foreach (Entity entity in checkList)
            {
                entity.On_Move += CheckMove;
                enabled[entity] = true;
            }
            //add entry rectangle
            enterRect = new RectangleF(onEnter.X, onEnter.Y, onEnter.Width, onEnter.Height);
			
			entered = new List<Entity>();
			//add events
			this.eventParams = eventParams;
			executeOnEntry = execute;
			
			if(condition == null)
				this.checkCondition += (sender, moveAmount) => true;
			else
				this.checkCondition = condition;
		}
		
		/// <summary>
		/// Creates an event box that executes an action when an entity enters a certain area
		/// </summary>
		/// <param name="entity">Entity that can trigger event</param>
		/// <param name="onEnter">Rectangle that entry into triggers event</param>
		/// <param name="execute">Action to execute</param>
		/// <param name="eventParams">BoxID</param>
		/// <param name="condition">Condition that must be true before event with execute</param>
		public EventBox(Entity entity, RectangleF onEnter, Action<Entity, BoxArgs> execute, BoxArgs eventParams, Condition condition = null) 
			: this(new List<Entity>(){entity}, onEnter, execute, eventParams, condition) { }

        /// <summary>
        /// Unloads all entities from EventBox
        /// </summary>
        public void UnloadBox(bool saveState = false)
        {
            if(saveState)
            {
                saved = new Dictionary<Entity, bool>(enabled);
            }
            foreach(Entity entity in checkList)
            { 
                entity.On_Move -= CheckMove;
                enabled[entity] = false;
            }
        }

        /// <summary>
        /// Unload a specific entity from an event box
        /// </summary>
        /// <param name="removeEntity">Removes certain entity from event box</param>
        public void UnloadBox(Entity removeEntity)
        {
            if (checkList.Contains(removeEntity))
            {
                removeEntity.On_Move -= CheckMove;
                enabled[removeEntity] = false;
            }
        }

        /// <summary>
        /// Loads all entities into EventBox
        /// </summary>
        public void LoadBox(bool loadState = false)
        {
            if (loadState)
                enabled = saved;
            foreach (Entity entity in checkList)
            {
                if(!loadState)
                    enabled[entity] = true;
                entity.On_Move += CheckMove;
            }
        }

        /// <summary>
        /// Loads a specific entity into an event box
        /// </summary>
        /// <param name="addEntity">Loads a certain entity into event box</param>
        public void LoadBox(Entity addEntity)
        {
            if (!checkList.Contains(addEntity))
                checkList.Add(addEntity);
            addEntity.On_Move += CheckMove;
            enabled[addEntity] = true;
        }


        /// <summary>
        /// Draws black box in the enter rectangle for debugging
        /// </summary>
        /// <param name="spriteBatch">Spritebatch to use to draw</param>
        public void DrawDebug(SpriteBatch spriteBatch)
		{
			Tile back = new Tile((Rectangle)this.enterRect, "black1");
			back.Draw(spriteBatch);
		}
		
		private void CheckMove(object sender, Vector2 moveCoords)
		{
			if(checkList.Contains(sender as Entity) && enabled[sender as Entity])
			{
				Entity entity = sender as Entity;
				RectangleF bounds = entity.Bounds;
				bounds.Offset(moveCoords.X, moveCoords.Y);
				if(enterRect.Intersects(bounds) && !entered.Contains(entity) && checkCondition(this, moveCoords))
				{
					entered.Add(entity);
					executeOnEntry.Invoke(entity, eventParams);
				}
				else if(!enterRect.Intersects(bounds) && entered.Contains(entity))
					entered.Remove(entity);
			}
		}
	}
}
