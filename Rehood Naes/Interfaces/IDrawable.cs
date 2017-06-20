using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Rehood_Naes.Interfaces
{
	/// <summary>
	/// Providies interface for any drawable element
	/// </summary>
	public interface IDrawable
	{
		/// <summary>
		/// Draws element with specified SpriteBatch
		/// </summary>
		/// <param name="spriteBatch">SpriteBatch to draw with</param>
		void Draw(SpriteBatch spriteBatch);
		
		/// <summary>
		/// Updates element
		/// </summary>
		/// <param name="gameTime">Snapshot of timing values</param>
		void Update(GameTime gameTime);
	}
}
