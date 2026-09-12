using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

public struct Circle : IEquatable<Circle>
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

    public Circle(float radius, float x, float y)
    {
        this.Radius = radius;
        this.X = x;
        this.Y = y;
    }

    // Check if 2 circles intersect
    public bool Intersect(Circle other)
    {
        // calculate the distance between 2 centers 
        // if sum of radii > distance then they are intersect
        
        float distanceSquared = Vector2.DistanceSquared(this.Position, other.Position);

        if ((this.Radius + other.Radius) * (this.Radius + other.Radius) > distanceSquared)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// If 2 circles have the same radius and they are at the same position, then they are equal
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public bool Equals(Circle other)
    {
        if (this.Radius == other.Radius && this.Position == other.Position)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Check if an object is equal to this circle
    /// </summary>
    /// <param name="obj"></param>
    /// <returns></returns>
    public override bool Equals(object obj)
    {
        if (obj is Circle && Equals(obj))
        {
            return true;
        }

        return false;
    }

    public static bool operator ==(Circle lhs, Circle rhs)
    {
        return lhs.Equals(rhs);
    }

    public static bool operator !=(Circle lhs, Circle rhs)
    {
        return !lhs.Equals(rhs);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
