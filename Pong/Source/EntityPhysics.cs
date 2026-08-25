using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

public struct EntityPhysics
{
	public Vector2 Position; // position on the screen
	public Vector2 Direction; // direction of movement
	public Vector2 Velocity;
	public float Speed;
	public Vector2 Impulse; // applied at a certain moment in time
	public Vector2 Force; // currently unused 
	public float Mass; 

	public EntityPhysics(Vector2 position, Vector2 direction, float mass)
	{
		Position = position;
		Direction = direction;
		Mass = mass;
	}
}