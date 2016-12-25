using System;
using System.Linq;
using Rehood_Naes.Entities;

namespace Rehood_Naes.Menus
{
	public class ContainerMenu : Menu
	{
		private StorageContainer container;

		public ContainerMenu(StorageContainer container, string menuID) : base(menuID)
		{
		}
	}
}

