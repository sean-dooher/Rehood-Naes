using System;
using Microsoft.Xna.Framework;

namespace Rehood_Naes.Interfaces
{
	/// <summary>
	/// Providies some extensions/useful methods for Rectangles
	/// </summary>
	public static class RectangleEx
	{
		/// <summary>
		/// Creates a rectangle from a certain array of ints
		/// </summary>
		/// <param name="values">Array of ints to use</param>
		/// <returns>New Rectangle with values from array of ints</returns>
		public static Rectangle FromArray(int[] values)
		{
			return new Rectangle(values[0], values[1], values[2], values[3]);
		}
		
		/// <summary>
		/// Creates a rectangle from a certain array of strings
		/// </summary>
		/// <param name="values">Array of strings to use</param>
		/// <returns>New Rectangle with values from array of strings</returns>
		public static Rectangle FromArray(string[] values)
		{
			return new Rectangle(int.Parse(values[0]), int.Parse(values[1]), int.Parse(values[2]), int.Parse(values[3]));
		}
	}
	
	/// <summary>
	/// Provides useful extensions for Vector2s
	/// </summary>
	public static class VectorEx
	{	
		/// <summary>
		/// Creates a Vector2 from array of strings
		/// </summary>
		/// <param name="values">Strings to use</param>
		/// <returns>New Vector2 with values provided from string</returns>
		public static Vector2 FromArray(string[] values)
		{
			return new Vector2(float.Parse(values[0]), float.Parse(values[1]));
		}
	}
}
