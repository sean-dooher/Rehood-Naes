using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Rehood_Naes.Entities;

namespace Rehood_Naes.Interfaces
{
	/// <summary>
	/// Rectangle that uses float values instead of ints
	/// </summary>
	public class RectangleF
	{
		#region Properties
		/// <summary>
		/// X coordinate of top left corner
		/// </summary>
		public float X;	
		
		/// <summary>
		/// Y coordinate of top left corner
		/// </summary>
		public float Y;	
		
		/// <summary>
		/// Width of rectangle
		/// </summary>
		public float Width;
		
		/// <summary>
		/// Height of rectangle
		/// </summary>
		public float Height;
		
		/// <summary>
		/// Position of top left coordinate of rectangle
		/// </summary>
		public Vector2 Location
		{
			get { return new Vector2(X, Y); }
			set { X = value.X; Y = value.Y; }
		}
		
		/// <summary>
		/// Size of rectangle
		/// </summary>
		public Vector2 Size
		{
			get { return new Vector2(Width, Height); }
			set { Width = value.X; Height = value.Y; }
		}
		
		/// <summary>
		/// Coordinate for center of rectangle
		/// </summary>
		public Vector2 Center
		{
			get { return new Vector2(X + Width/2, Y + Height/2); }
		}
		
		/// <summary>
		/// Determines whether rectangle is empty or not
		/// </summary>
		public bool IsEmpty
		{
			get
			{
				if(Size.X == 0f && Size.Y == 0f)
					return true;
				return false;
			}
		}
		
		/// <summary>
		/// X coordinate of left side
		/// </summary>
		public float Left
		{
			get { return X; }
		}
		
		/// <summary>
		/// X coordinate of right side
		/// </summary>
		public float Right
		{
			get { return X + Width; }
		}
		
		/// <summary>
		/// Y coordinate of top side
		/// </summary>
		public float Top
		{
			get { return Y; }
		}
		
		/// <summary>
		/// Y coordinate of bottom side
		/// </summary>
		public float Bottom
		{
			get { return Y + Height; }
		}
		#endregion
		
		/// <summary>
		/// Creates new rectangle with at a certain (x,y) with a certain size
		/// </summary>
		/// <param name="x">X coordinate to use</param>
		/// <param name="y">Y coordinate to use</param>
		/// <param name="width">Width of rectangle</param>
		/// <param name="height">Height of rectangle</param>
		public RectangleF(float x, float y, float width, float height)
		{
			X = x;
			Y = y;
			Width = width;
			Height = height;
		}
		
		/// <summary>
		/// Creates new rectangle at certain point with a certain size
		/// </summary>
		/// <param name="position">Location of top left corner</param>
		/// <param name="size">Size of rectangle</param>
		public RectangleF(Vector2 position, Vector2 size)
		{
			X = position.X;
			Y = position.Y;
			Width = size.X;
			Height = size.Y;
		}
		
		#region Methods
		/// <summary>
		/// Checks if two rectangles intersect
		/// </summary>
		/// <param name="rect">Other rectangle to use</param>
		/// <returns>True if rectangles intersect, false otherwise</returns>
		public bool Intersects(RectangleF rect)
		{
			if(IsEmpty || rect.IsEmpty)
				return false;
			if(Left < rect.Right && Right > rect.Left
			   && Top < rect.Bottom && Bottom > rect.Top)
				return true;
			return false;
		}
		
		/// <summary>
		/// Checks if two rectangles intersect
		/// </summary>
		/// <param name="rect">Other rectangle to use</param>
		/// <returns>True if rectangles intersect, false otherwise</returns>
		public bool Intersects(Rectangle rect)
		{
			return Intersects((RectangleF) rect);
		}
		
		/// <summary>
		/// Checks if this rectangle contains another
		/// </summary>
		/// <param name="rect">Other rectangle to use</param>
		/// <returns>True if rectangle is contained, false otherwise</returns>
		public bool Contains(RectangleF rect)
		{
			if(IsEmpty || rect.IsEmpty)
				return false;
			if(Left < rect.Left && Right > rect.Right
			   && Top < rect.Top && Bottom > rect.Bottom)
				return true;
			else 
				return false;
		}
		
		/// <summary>
		/// Checks if this rectangle contains another
		/// </summary>
		/// <param name="rect">Other rectangle to use</param>
		/// <returns>True if rectangle is contained, false otherwise</returns>
		public bool Contains(Rectangle rect)
		{
			if(IsEmpty || rect.IsEmpty)
				return false;
			if(Left < rect.Left && Right > rect.Right
			   && Top < rect.Top && Bottom > rect.Bottom)
				return true;
			else 
				return false;
		}
		
		/// <summary>
		/// Moves the rectangle by a certain amount
		/// </summary>
		/// <param name="offset">Amount to move by</param>
		public void Offset(Vector2 offset)
		{
			Offset(offset.X, offset.Y);
		}
		
		/// <summary>
		/// Moves the rectangle by a certain amount
		/// </summary>
		/// <param name="x">Amount to move on x axis</param>
		/// <param name="y">Amount to move on y axis</param>
		public void Offset(float x, float y)
		{
			X += x;
			Y += y;
		}
		
		/// <summary>
		/// Increases size of rectangle by certain amount
		/// </summary>
		/// <param name="amount">Amount to increase size by</param>
		public void Inflate(Vector2 amount)
		{
			Inflate(amount.X, amount.Y);
		}
		
		/// <summary>
		/// Increases size of rectangle by certain amount
		/// </summary>
		/// <param name="x">Amount to increase width</param>
		/// <param name="y">Amount to increase height</param>
		public void Inflate(float x, float y)
		{
			Width += x;
			Height+= y;
		}
		#endregion
		
		#region Operators/Static Methods
		/// <summary>
		/// Creates new RectangleF from array of strings
		/// </summary>
		/// <param name="values">Array to use</param>
		/// <returns>New rectangle made from values in array</returns>
		public static RectangleF FromArray(string[] values)
		{
			return new RectangleF(float.Parse(values[0]), float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
		}
		
		/// <summary>
		/// Creates new RectangleF from array of floats
		/// </summary>
		/// <param name="values">Array to use</param>
		/// <returns>New rectangle made from values in array</returns>
		public static RectangleF FromArray(float[] values)
		{
			return new RectangleF(values[0], values[1], values[2], values[3]);
		}
		
		/// <summary>
		/// Turns RectangleF into Rectangle
		/// </summary>
		/// <param name="rectf">RectangleF to convert</param>
		/// <returns>Rectangle with truncated values of RectangleF</returns>
		public static explicit operator Rectangle(RectangleF rectf)
		{
			return new Rectangle((int)rectf.X, (int)rectf.Y, (int)rectf.Width, (int)rectf.Height);
		}
		
		/// <summary>
		/// Turns Rectangle into RectangleF
		/// </summary>
		/// <param name="rect">Rectangle to convert</param>
		/// <returns>RectangleF with values of Rectangle</returns>
		public static explicit operator RectangleF(Rectangle rect)
		{
			return new RectangleF(rect.X, rect.Y, rect.Height, rect.Width);
		}
		#endregion
	}
	
	/// <summary>
	/// Provides useful extensions for compatibility with RectangleFs and other classes
	/// </summary>
	public static class Extensisons
	{
		/// <summary>
		/// Provides direction from this point to another
		/// </summary>
		/// <param name="vector">This point</param>
		/// <param name="end">End point</param>
		/// <returns></returns>
		public static SpriteDirection DirectionToPoint(this Vector2 vector, Vector2 end)
		{
			Vector2 direction = end - vector;
			if(Math.Abs(direction.Y) > Math.Abs(direction.X))//If change is more vertical than horizontal
			{
				return direction.Y > 0 ? SpriteDirection.South : SpriteDirection.North;
			}
			else  //If movement is more horizontal than vertical
			{
				return direction.X > 0 ? SpriteDirection.East : SpriteDirection.West;
			}
		}
		
		/// <summary>
		/// Checks if this rectangle intersects another
		/// </summary>
		/// <param name="rect">Current rectangle</param>
		/// <param name="rectf">Other rectangle to use</param>
		/// <returns>True if rectangles interesect, false otherwise</returns>
		public static bool Intersects(this Rectangle rect, RectangleF rectf)
		{
			if(rect.IsEmpty || rectf.IsEmpty)
				return false;
			if(rect.Left < rectf.Right && rect.Right > rectf.Left
			   && rect.Top < rectf.Bottom && rect.Bottom > rectf.Top)
				return true;
			return false;
		}
		
		/// <summary>
		/// Checks if this rectangle contains another
		/// </summary>
		/// <param name="rect">Current rectangle</param>
		/// <param name="rectf">Other rectangle to use</param>
		/// <returns>True if rectangle is contained, false otherwise</returns>
		public static bool Contains(this Rectangle rect, RectangleF rectf)
		{
			if(rectf.IsEmpty)
				return false;
			if(rect.Left < rectf.Left && rect.Right > rectf.Right
			   && rect.Top < rectf.Top && rect.Bottom > rectf.Bottom)
				return true;
			else 
				return false;
		}
		
		/// <summary>
		/// Allows RectangleFs to be used in drawing by spriteBatch
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to draw with</param>
		/// <param name="texture">Texture to draw</param>
		/// <param name="destinationRectangle">Rectangle to draw it in</param>
		/// <param name="sourceRectangle">Rectangle of sub texture to draw in main texture</param>
		/// <param name="color">A color mask</param>
		public static void Draw(this SpriteBatch spriteBatch, Texture2D texture, RectangleF destinationRectangle, Rectangle sourceRectangle, Color color)
		{
			spriteBatch.Draw(texture, destinationRectangle.Location, sourceRectangle, color);
		}
	}
}
