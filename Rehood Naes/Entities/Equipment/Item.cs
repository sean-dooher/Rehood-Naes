using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rehood_Naes.Building;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Rehood_Naes.Entities
{
    public abstract class Item : Entity
    {
        #region Fields
        //private CharacterSprite bagIcon;
        //private CharacterSprite droppedIcon;
        #endregion

        #region Properties
        public enum DrawState
        {
            None, Bag, Dropped
        }

        public DrawState DrawMode
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
        #endregion

        #region Constructors
        public Item(Area currentArea, Vector2 position, string name, int itemID, DrawState draw) :
            base(currentArea, position, name, itemID.ToString())
        {
            DrawMode = draw;
            ItemID = itemID;
            ItemName = name;
        }
        #endregion
        #region Methods
        public new void Draw(SpriteBatch spriteBatch)
        {
            if (DrawMode != DrawState.None)
                base.Draw(spriteBatch);
        }
        #endregion

        #region Helpers
        public void loadItem(int itemID)
        {
            
        }
        #endregion
    }
}
