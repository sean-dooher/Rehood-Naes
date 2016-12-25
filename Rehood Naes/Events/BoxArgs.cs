using System;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using Rehood_Naes.Entities;
using Rehood_Naes.Interfaces;

namespace Rehood_Naes.Events
{
	#region BaseArg
	/// <summary>
	/// Base class for event box arguments
	/// </summary>
	public abstract class BoxArgs : EventArgs
	{
		#region Fields
		private int timesTriggered;
		#endregion
		
		#region Properties	
		/// <summary>
		/// Number of times the event has been triggered
		/// </summary>
		public int TimesTriggered
		{
			get { return timesTriggered; }
			set { timesTriggered = value; }
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Creates new BoxArgs
		/// </summary>
		public BoxArgs()
		{
			timesTriggered = 0;
		}
		#endregion
	}
	#endregion
	
	#region BoxEventArgs
	/// <summary>
	/// EventArgs for loading new area
	/// </summary>
	public class NewAreaEventArgs : BoxArgs
	{
		#region Fields
		private string areaID;
		private Vector2 position;
		#endregion
		
		#region Properties
		/// <summary>
		/// AreaID to load
		/// </summary>
		public string AreaID
		{
			get { return areaID; }
		}
		
		/// <summary>
		/// Position to place player at
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Creates new EventArgs for a area loading event at position 0,0
		/// </summary>
		/// <param name="areaID">ID of area to load</param>
		public NewAreaEventArgs(string areaID) : this(areaID, Vector2.Zero) { }
		
		/// <summary>
		/// Creates new EventArgs for a area loading event
		/// </summary>
		/// <param name="areaID">ID of area to load</param>
		/// <param name="position">Position to place player at</param>
		public NewAreaEventArgs(string areaID, Vector2 position) : base()
		{
			this.areaID = areaID;
			this.position = position;
		}
		#endregion
	}
	
	/// <summary>
	/// Type of Entity
	/// </summary>
	public enum EntityType
	{
		/// <summary>
		/// Represents a Player
		/// </summary>
		Player,
		/// <summary>
		/// Represents an NPC
		/// </summary>
		NPC,
		/// <summary>
		/// Represents an Enemy
		/// </summary>
		Enemy
	}
	
	/// <summary>
	/// Spawn arguments for an entity
	/// </summary>
	public class EntitySpawnEventArgs : BoxArgs
	{
		#region Fields
		private int maxNum;
		private string name;
		private string entityID;
		private Vector2 position;
		private SpriteDirection startDirection;
		private RectangleF moveLimit;
		private EntityType spawnType;
		#endregion
		
		#region Properties
		/// <summary>
		/// EntityID to load
		/// </summary>
		public string EntityID
		{
			get { return entityID; }
		}
		
		/// <summary>
		/// Unique name prefix
		/// </summary>
		public string Name
		{
			get { return name; }
		}
		
		/// <summary>
		/// Type of entity
		/// </summary>
		public EntityType Type
		{
			get { return spawnType; }
		}
		/// <summary>
		/// Maximum number of this entity to spawn at a given time
		/// </summary>
		public int MaxNum
		{
			get { return maxNum; }
		}
		
		/// <summary>
		/// Position to place entity at
		/// </summary>
		public Vector2 Position
		{
			get { return position; }
		}
		
		/// <summary>
		/// Boundaries for entity to walk in
		/// </summary>
		public RectangleF Bounds
		{
			get { return moveLimit; }
		}
		
		/// <summary>
		/// Initial direction of entity
		/// </summary>
		public SpriteDirection Direction
		{
			get { return startDirection; }
		}
		#endregion
		
		#region Constructors
		/// <summary>
		/// Creates new argument for spawning entity
		/// </summary>
		/// <param name="entityID">ID of entity</param>
		/// <param name="name">Name of entity</param>
		/// <param name="type">Type of entity</param>
		/// <param name="maxNum">Maximum number of entities that can be spawned</param>
		/// <param name="direction">Initial direction</param>
		/// <param name="position">Current position</param>
		/// <param name="bounds">Bounds for entity to walk in</param>
		public EntitySpawnEventArgs(string entityID, string name, EntityType type, int maxNum, SpriteDirection direction, Vector2 position, RectangleF bounds) 
		{ 
			this.entityID = entityID;
			this.name = name;
			this.spawnType = type;
			this.maxNum = maxNum;
			this.position = position;
			this.moveLimit = bounds;
			this.startDirection = direction;
		}
		#endregion
	}
	#endregion
}
