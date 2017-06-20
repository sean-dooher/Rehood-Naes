using Microsoft.Xna.Framework;
using Rehood_Naes.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Rehood_Naes.Entities
{
    /// <summary>
    /// Implements an entity that stores a certain number of items
    /// </summary>
    public class StorageContainer
    {
        #region Fields
		private List<Item> items;
        #endregion

		#region Events
		#endregion

        #region Properties
		/// <summary>
		/// Returns an unordered list of items in the container
		/// </summary>
		/// <value>The items.</value>
        public List<Item> Items
        {
			get { return new List<Item>(items); }
        }

        public int MaxCapacity
        {
            get;
            protected set;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates an storage container with initial items and a specified max capacity
        /// </summary>
        /// <param name="capacity">Max storage of container</param>
        /// <param name="items">List of items to fill container with</param>
        public StorageContainer(int capacity, List<Item> items = null)
        {
            MaxCapacity = capacity;
			this.items = new List<Item>();
			//initialize storage
			for (int i = 0; i < MaxCapacity; i++)
				this.items.Add (null);
				
            
        }
        #endregion

        #region Methods
		/// <summary>
		/// Updates elements in menu
		/// </summary>
		/// <param name="gameTime">Snapshot of timing values</param>
		public void Update(GameTime gameTime)
		{
			foreach (var item in items)
			{
				if (item is Item)
					item.Update (gameTime);
			}
		}
			
		/// <summary>
		/// Attempts to add item into a certain slot
		/// </summary>
		/// <returns><c>true</c>, if item was added, <c>false</c> otherwise.</returns>
		/// <param name="item">Item to add</param>
		/// <param name="slot">Slot to attempt to place item in</param>
        public bool AddItem(Item item, int slot)
        {
			if(slot >= 0 && slot < MaxCapacity) // slot is valid
            {
                if (items[slot] == null)
                {
                    items[slot] = item;
                    return true;
                } else if(items[slot].ItemID == item.ItemID)
                {
                   
                }
            }
            return false;
        }

        /// <summary>
        /// Adds item into first open slot. Returns true if successfuly adds all items in stack
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool AddItem(Item item)
        {
			foreach (Item i in items.Where(i => i.ItemID == item.ItemID))
			{
				if (i.Count < item.MaxStack || i.MaxStack == -1)
				{
                    int before = item.Count;
                    item.Decrease(item.Count - i.Increase(item.Count));
					if(item.Count == 0)
						return true;
				}
			}
            return AddItem(item, NextOpenSlot());
        }

		/// <summary>
		/// Removes item from container
		/// </summary>
		/// <returns><c>item</c>, if item was removed, <c>null</c> if there is no item in the slot or slot doesn't exist</returns>
		/// <param name="slot">Slot to remove item from</param>
		public Item RemoveItem(int slot)
		{
			Item i = null;
			if (slot < 0 || slot >= MaxCapacity || items [slot] == null)
				return i;
			if (items [slot].Count <= 0)
				items [slot] = null;
			else
				i = items [slot].Decrease (1);
			
			return i;
		}

		/// <summary>
		/// Finds the next open slot in the container
		/// </summary>
		/// <returns>The next open slot</returns>
        private int NextOpenSlot()
        {
			for (int i = 0; i < MaxCapacity; i++)
				if (items [i] == null)
					return i;
			return MaxCapacity;
        }
        #endregion
    }
}
