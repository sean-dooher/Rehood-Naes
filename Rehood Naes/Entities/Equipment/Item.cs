using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rehood_Naes.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Rehood_Naes.Interfaces;

namespace Rehood_Naes.Entities
{
    public class Item : Entity
    {
        #region Fields
        #endregion

        #region Properties
        public enum ItemState
        {
            None, Bag, Dropped
        }

        public ItemState ItemMode
        {
            get;
            protected set;
        }
			
        public int ItemID
        {
            get; protected set;
        }

        public string ItemName
        {
            get; protected set;
        }

		public int EquipSlot 
		{
			get;
			protected set;
		}

		public int Count 
		{
			get;
			protected set;
		}

		public int MaxStack {
			get;
			protected set;
		}
        #endregion

        #region Constructors
		public Item(Area currentArea, Vector2 position, int itemID, ItemState mode, int count = 1) :
		base(currentArea, position, "item" + itemID.ToString(), itemID.ToString())
		{
			ItemMode = mode;
			ItemID = itemID;
			loadItem (ItemID);
			Count = count;
			LoadEntity (position, AppDomain.CurrentDomain.BaseDirectory + @"Content\items\item_entity.xml");
		}
        #endregion
        #region Methods
		new public void Update(GameTime gameTime)
		{
			//TODO: Animation and stuff
		}

		new public void Draw(SpriteBatch spriteBatch)
        {
			base.Draw (spriteBatch);
        }

		/// <summary>
		/// Attempts to increase count
		/// </summary>
		/// <param name="count">Amount to increase by</param>
		/// <returns><c>0</c> if successful, <c>num failed to add</c> if Count + count > MaxStack</returns>
		public int Add(int count)
		{
			if (count > 0)
			{
				int originalCount = Count;
				Count = Math.Min (Count + count, MaxStack);
				return originalCount + count - Count;
			}
			return 0;
		}

		/// <summary>
		/// Removes count items from the stack
		/// </summary>
		/// <param name="count">Count.</param>
		public Item Reduce(int count)
		{
			int originalCount = Count;
			Count = Math.Max (Count - count, 0);
			return new Item (this.CurrentArea, position, this.ItemID, ItemMode, originalCount - Count + count);
		}
        #endregion

        #region Helpers
        public void loadItem(int itemID)
        {
			XDocument doc = XDocument.Load (AppDomain.CurrentDomain.BaseDirectory + @"Content\items\items.xml");
			var itemXML = doc.Descendants ("Item").Where (elem => elem.Element ("ID").Value == itemID.ToString()).First ();
			MaxStack = int.Parse(itemXML.Element ("MaxStack").Value);
			EquipSlot = int.Parse (itemXML.Element ("EquipSlot").Value);
			ItemName = itemXML.Element ("Name").Value;
        }
        #endregion
    }
}
