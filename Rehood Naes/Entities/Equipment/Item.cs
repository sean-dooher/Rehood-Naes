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
    public class Item : IDrawable, IUpdateable
    {
        #region Fields
        #endregion

        #region Properties	
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

        public int DrawOrder => throw new NotImplementedException();

        public bool Visible => throw new NotImplementedException();

        public bool Enabled => throw new NotImplementedException();

        public int UpdateOrder => throw new NotImplementedException();
        #endregion

        #region Constructors
        public Item(int itemID, int count = 1)
		{
			ItemID = itemID;
			loadItem (ItemID);
			Count = count;
		}

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        #endregion
        #region Methods
        public void Update(GameTime gameTime)
		{
			//TODO: Animation and stuff
		}

		public void Draw(SpriteBatch spriteBatch)
        {
			
        }

		/// <summary>
		/// Attempts to increase count
		/// </summary>
		/// <param name="count">Amount to increase by</param>
		/// <returns><c>0</c> if successful, <c>num failed to add</c> if Count + count > MaxStack</returns>
		public int Increase(int count)
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
		/// Removes count items from the stack and returns removed stack
		/// </summary>
		/// <param name="count">Count.</param>
		public Item Decrease(int count)
		{
			int originalCount = Count;
			Count = Math.Max (Count - count, 0);
			return new Item (this.ItemID, originalCount - Count + count);
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

        public void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
