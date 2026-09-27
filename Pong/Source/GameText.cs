using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GameText
{
    /// <summary>
    /// Which font to use 
    /// </summary>
    public SpriteFont SpriteFont { get; set; }
    public float LayerDepth { get; set; }
    public string Text { get; set; }
    public Vector2 Position { get; set; }
    public float Rotation { get; set; } = 0.0f;
    public Color Color { get; set; } = Color.White;
    public Vector2 Origin { get; set; } = Vector2.Zero;
    public SpriteEffects SpriteEffects { get; set; } = SpriteEffects.None;
    public float Scale { get; set; } = 1.0f;

    public GameText()
    {
    }

    public GameText(SpriteFont spriteFont)
    {
        SpriteFont = spriteFont;
    }

    public void CenterOrigin()
    {
        Vector2 size = SpriteFont.MeasureString(Text) * Scale;
        Origin = size / 2;
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.DrawString(SpriteFont, Text, Position, Color, Rotation, Origin, Scale, SpriteEffects, LayerDepth);
    }
}
