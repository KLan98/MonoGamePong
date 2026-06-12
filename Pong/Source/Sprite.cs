using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

// control the properties of a sprite for example:
// texture region
// rotation
// scale
public class Sprite
{
	public TextureRegion TextureRegion { get; set; }
	public float Rotation { get; set; } = 0.0f;
	public Color Color { get; set; } = Color.White;

    /// <summary>
    /// Gets or Sets the xy-coordinate origin point, relative to the top-left corner, of this sprite.
    /// </summary>
    /// <remarks>
    /// Default value is Vector2.Zero
    /// </remarks>
    public Vector2 Origin { get; set; } = Vector2.Zero;

	/// <summary>
	/// How many times scaled sprite is in comparision to the og
	/// </summary>
	public Vector2 Scale { get; set; } = Vector2.One;
	public float LayerDepth { get; set; } = 0.0f;
	public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
	
	/// <summary>
	/// Width of sprite = width of texture region * scale
	/// </summary>
	public float Width
	{ 
		get
		{
			return TextureRegion.GetRectWidth() * Scale.X;
		}
	}

    /// Height of sprite = height of texture region * scale
    /// </summary>
    public float Height
	{
		get
		{
			return TextureRegion.GetRectHeight() * Scale.Y;
		}
	}

	public Sprite()
	{

	}

	public Sprite(TextureRegion textureRegion)
	{
		TextureRegion = textureRegion;
	}

	/// <summary>
	/// Set the origin of this sprite to its center
	/// </summary>
	public void CenterOrigin()
	{
		Origin = new Vector2(TextureRegion.GetRectWidth() * 0.5f, TextureRegion.GetRectHeight() * 0.5f);
	}

	public void Draw(SpriteBatch spriteBatch, Vector2 drawPos)
	{
		TextureRegion.Draw(spriteBatch, drawPos, Color, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
	}

    /// <summary>
    /// get the texture region of this type and return it
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public Sprite CreateSprite(TextureRegionType type)
	{
		TextureRegion textureRegion = AssetsManager.GetInstance().GetRegion(type);
		return new Sprite(textureRegion);
	}
}
   