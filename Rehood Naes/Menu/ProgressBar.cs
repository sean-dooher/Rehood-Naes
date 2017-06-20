using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rehood_Naes.Menu
{
	/// <summary>
	/// Two tone progressbar to represent changing values
	/// </summary>
	public class ProgressBar : IMenuElement
	{
		#region Fields
		private Texture2D frontTexture;
		private Texture2D backTexture;
		private Rectangle frontRectangle;
		private int maxValue;
		private int currentValue;

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        #endregion

        #region Properties
        /// <summary>
        /// Position of progress bar; can be set publically
        /// </summary>
        public Vector2 Position
		{
			get; set;
		}
		
		/// <summary>
		/// Size of progress bar
		/// </summary>
		public Vector2 Size
		{
			get; private set;
		}

        public int DrawOrder => throw new NotImplementedException();

        public bool Visible => throw new NotImplementedException();

        public bool Enabled => throw new NotImplementedException();

        public int UpdateOrder => throw new NotImplementedException();
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new progress bar with certain colors, poisiton, size, and initial and max amounts
        /// </summary>
        /// <param name="position">Position of bar on screen</param>
        /// <param name="size">Size of bar</param>
        /// <param name="maxValue">Max value of bar</param>
        /// <param name="startValue">Starting value of bar</param>
        /// <param name="front">Front color</param>
        /// <param name="back">Back color</param>
        public ProgressBar(Vector2 position, Vector2 size, int maxValue, int startValue, Color front, Color back)
		{
			Position = position;	
			Size = size;
			
			//set value parameters
			this.maxValue = maxValue;
			currentValue = startValue;
			
			//create front texture
			Color[] frontColor = new Color[(int)(size.X * size.Y)];
			for(int i = 0; i < frontColor.Length; i++)
				frontColor[i] = front;
			frontTexture = new Texture2D(RPG.GraphicsManager.GraphicsDevice, (int)size.X, (int)size.Y, false, SurfaceFormat.Color);
			frontTexture.SetData(frontColor);
			
			//create back texture
			Color[] backColor = new Color[(int)(size.X * size.Y)];
			for(int i = 0; i < backColor.Length; i++)
				backColor[i] = back;
			backTexture = new Texture2D(RPG.GraphicsManager.GraphicsDevice, (int)size.X, (int)size.Y, false, SurfaceFormat.Color);
			backTexture.SetData(backColor);
			
			frontRectangle = new Rectangle(0, 0, (int)(Size.X * currentValue/maxValue), (int)Size.Y);
		}
		#endregion
		
		#region Methods
		/// <summary>
		/// Event subscriber to change bar position
		/// </summary>
		/// <param name="sender">Object command sent with</param>
		/// <param name="amount">Amount to move</param>
		public void Move(object sender, Vector2 amount)
		{
			Position = Position + amount;
		}
		
		/// <summary>
		/// Move bar position by a certain x and y amount
		/// </summary>
		/// <param name="x">Amount x to move</param>
		/// <param name="y">Amount y to move</param>
		public void Move(float x, float y)
		{
			Move(this, new Vector2(x, y));
		}
		
		/// <summary>
		/// Event subscriber to change bar value
		/// </summary>
		/// <param name="sender">Object command was sent from</param>
		/// <param name="newAmount">New current value of bar</param>
		public void ChangeProgress(object sender, int newAmount)
		{
			currentValue = newAmount;
		}
		
		/// <summary>
		/// Updates the size of the front bar every frame
		/// </summary>
		/// <param name="gameTime">Provides snapshotting of times</param>
		public void Update(GameTime gameTime)
		{
			frontRectangle.Width = (int)(Size.X * currentValue/maxValue);
		}
		
		/// <summary>
		/// Draws the progressbar with a given SpriteBatch
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to draw progress bar with</param>
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(backTexture, Position, Color.White);
			spriteBatch.Draw(frontTexture, Position, frontRectangle, Color.White);
		}

        public void Draw(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
