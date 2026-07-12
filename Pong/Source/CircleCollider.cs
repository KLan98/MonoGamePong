using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public struct CircleCollider
{
    // X position of center
    public float X
    {
        get;
    }


    // Y position of center
    public float Y
    {
        get;
    }

    public float Radius;

    public Vector2 Position
    {
        get
        {
            return new Vector2(X, Y);
        }
    }

    // The highest point of the circle 
    public Vector2 Top
    {
        get
        {
            return new Vector2(X, Y - Radius);
        }
    }

    // The lowest point of the circle
    public Vector2 Bottom
    {
        get
        {
            return new Vector2(X, Y + Radius);
        }
    }

    // The right most point of the circle
    public Vector2 Right
    {
        get
        {
            return new Vector2(X + Radius, Y);
        }
    }

    // The left most point of the circle
    public Vector2 Left
    {
        get
        {
            return new Vector2(X - Radius, Y);
        }
    }

    public CircleCollider(float radius, float x, float y)
    {
        this.Radius = radius;
        this.X = x;
        this.Y = y;
    }

    // Check if 2 circles intersect
    public bool Intersect(CircleCollider other)
    {
        if (true)
        {
            return true;
        }

        else
        {
            // this will be default case
            return false;
        }
    }
}
