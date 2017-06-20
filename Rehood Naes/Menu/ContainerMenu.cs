using System;
using System.Linq;
using Rehood_Naes.Entities;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Xml.Linq;
using Rehood_Naes.Interfaces;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using Rehood_Naes.Building;

namespace Rehood_Naes.Menu
{
	public class ContainerMenu : Menu
	{
		public enum ContainerFormat
		{
			Rectangle, Armor
		}

		private StorageContainer container;
		private List<Button> slots;
		private Vector2 position;
		private int columns;
		private Tile background;
		private ContainerFormat format;

		public StorageContainer Container
		{
			get { return container; }
		}

		public ContainerMenu(StorageContainer container, string menuID) : base(menuID)
		{
			this.container = container;
			loadContainer (menuID);
		}

		/// <summary>
		/// Updates elements in menu
		/// </summary>
		/// <param name="gameTime">Snapshot of timing values</param>
		new public void Update(GameTime gameTime)
		{
			if (isShowing)
			{
				container.Update (gameTime);
				background.Update (gameTime);
				slots.ForEach (slot => slot.Update(gameTime));
			}
			base.Update (gameTime);
		}

		/// <summary>
		/// Draws menu elements with given SpriteBatch
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch used</param>
		new public void Draw(SpriteBatch spriteBatch)
		{
			if (isShowing)
			{
				background.Draw (spriteBatch);
				slots.ForEach (slot => slot.Draw (spriteBatch));
				for (int i = 0; i < container.MaxCapacity; i++)
				{
					Item item = container.Items [i];
					Button slot = slots [i];
					if (item != null)
					{
						item.Position = slot.Bounds.Center.ToVector2();
						item.Draw (spriteBatch);
					}
				}
			}
			base.Draw (spriteBatch);
		}

		protected void loadContainer(string menuID)
		{
			slots = new List<Button> ();
			XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + @"Content\menus\"+menuID+".xml");
			var containerXML = doc.Element("Menu").Element("Container");
			this.position = VectorEx.FromArray (containerXML.Element ("Position").Value.Split (','));
			this.format = (ContainerFormat)Enum.Parse (typeof(ContainerFormat), containerXML.Elements ("Format").First ().Value);
			string slotID = containerXML.Element ("Slot").Value;
			string style = containerXML.Element ("Style").Value;
			bool rounded = bool.Parse (containerXML.Element ("Rounded").Value);
			Vector2 borderSize = VectorEx.FromArray(containerXML.Element ("BorderSize").Value.Split (','));
			this.columns = int.Parse (containerXML.Element ("Columns").Value);
			this.loadMenu (menuID);
			Vector2 baseSlot = new Button (Vector2.Zero, "", slotID).Size;
			if (format == ContainerFormat.Rectangle)
			{
				for (int i = 0; i < container.MaxCapacity; i++)
				{
					int x = i % columns;
					int y = i / columns;
					Vector2 pos = position + borderSize + new Vector2 (x * baseSlot.X, y * baseSlot.Y);
					Button slot = new Button (pos, "", slotID);
					int currentSlot = i;
					slot.On_Click += (object sender, MouseState CurrentState) => SlotClick (currentSlot, CurrentState);
					slots.Add (slot);
				}
				background = new Tile (
					new Rectangle (position.ToPoint (),
						(borderSize * 2 + new Vector2 (baseSlot.X * columns, baseSlot.Y * (float)Math.Ceiling ((float)container.MaxCapacity / columns))).ToPoint ()),
					style, rounded);
			}
			else if (format == ContainerFormat.Armor)
			{
				
			}
		}

		private void SlotClick(int slot, MouseState mouse)
		{
			string message = "Slot: " + slot.ToString () + "\n" + "Item name: ";
			if (container.Items [slot] == null)
				message += "None";
			else
				message += container.Items [slot].ItemName;
			System.Windows.Forms.MessageBox.Show (message);
		}

	}
}

