using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class TextureRegion
{
	private Texture2D sourceTexture;
	private Rectangle rectangle; // drawing region of the source texture

    /// <summary>
    /// Creates a new texture region using the specified source texture.
    /// </summary>
    /// <param name="texture">The texture to use as the source texture for this texture region</param>
    /// <param name="x">The x coordinate of the top-left corner of the created Rectangle.</param>
    /// <param name="y">The y coordinate of the top-left corner of the created Rectangle.</param>
    /// <param name="width">The width of the created Rectangle</param>
    /// <param name="height">The height of the created Rectangle</param>
    public TextureRegion(Texture2D texture, int x, int y, int width, int height)
	{
		this.sourceTexture = texture;
		this.rectangle = new Rectangle(x, y, width, height);
	}

    /// <summary>
    /// Get width of the drawing region
    /// </summary>
    /// <returns></returns>
    public int GetRectWidth()
    {
        return rectangle.Width;
    }

    /// <summary>
    /// Get height of the drawing region
    /// </summary>
    /// <returns></returns>
    public int GetRectHeight()
    {
        return rectangle.Height;
    }

    /// <summary>
    /// Return the default x coordinate of the top-left corner of the created Rectangle
    /// </summary>
    /// <returns></returns>
    public int GetRectX()
    {
        return rectangle.X;
    }

    /// <summary>
    /// Return the default y coordinate of the top-left corner of the created Rectangle
    /// </summary>
    /// <returns></returns>
    public int GetRectY()
    {
        return rectangle.Y;
    }

    /// <summary>
    /// Draw sprite batch with position and color as parameters
    /// </summary>
    /// <param name="spriteBatch">The spritebatch instance used for batching draw calls.</param>
    /// <param name="position">The xy-coordinate location to draw this texture region on the screen.</param>
    /// <param name="color">The color mask to apply when drawing this texture region on screen.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color)
    {
        Draw(spriteBatch, position, color, 0.0f, Vector2.Zero, Vector2.One, SpriteEffects.None, 0.0f);
    }

    /// <summary>
    /// Draw sprite batch with the option for setting the origin, rotation and single float value to be applied to both axes for scaling
    /// </summary>
    /// <param name="spriteBatch"></param>
    /// <param name="position"></param>
    /// <param name="color"></param>
    /// <param name="rotation">mount of rotation, in radians, to apply when drawing this texture region on screen</param>
    /// <param name="origin">The center of rotation, scaling, and position when drawing this texture region on screen</param>
    /// <param name="scale">single float value to be applied to both axes for scaling</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, float scale, SpriteEffects effects, float layerDepth)
    {
        Draw(spriteBatch, position, color, rotation, origin, new Vector2(scale, scale), effects, layerDepth);
    }

    /// <summary>
    /// Original draw method that the other two depend on, most flexible can change effects, layerDepth, independy x/y scaling, rotation
    /// </summary>
    /// <param name="spriteBatch">The spritebatch instance used for batching draw calls.</param>
    /// <param name="position">The xy-coordinate location to draw this texture region on the screen.</param>
    /// <param name="color">The color mask to apply when drawing this texture region on screen.</param>
    /// <param name="rotation">The amount of rotation, in radians, to apply when drawing this texture region on screen.</param>
    /// <param name="origin">The center of rotation, scaling, and position when drawing this texture region on screen.</param>
    /// <param name="scale">The amount of scaling to apply to the x- and y-axes when drawing this texture region on screen.</param>
    /// <param name="effects">Specifies if this texture region should be flipped horizontally, vertically, or both when drawing on screen.</param>
    /// <param name="layerDepth">The depth of the layer to use when drawing this texture region on screen.</param>
    public void Draw(SpriteBatch spriteBatch, Vector2 position, Color color, float rotation, Vector2 origin, Vector2 scale, SpriteEffects effects, float layerDepth)
    {
        spriteBatch.Draw(sourceTexture, position, rectangle, color, rotation, origin, scale, effects, layerDepth);
    }
}