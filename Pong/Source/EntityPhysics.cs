using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class EntityPhysics
{
	public Vector2 Position; // position on the screen
	public Vector2 Direction; // direction of movement
	public float Velocity; // average velocity

	public EntityPhysics(Vector2 position, Vector2 direction, float velocity)
	{
		Position = position;
		Direction = direction;
		Velocity = velocity;
	}
}