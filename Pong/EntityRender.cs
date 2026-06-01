using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public struct EntityRender
{
    public Texture2D Texture;

    public EntityRender(Texture2D texture)
    {
        Texture = texture;
    }
}