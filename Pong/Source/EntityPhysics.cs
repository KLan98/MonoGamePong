using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

public class EntityPhysics
{
	public Vector2 Position;
	public Vector2 Direction;
	public Vector2 Velocity;

	public EntityPhysics(Vector2 position, Vector2 direction, Vector2 velocity)
	{
		Position = position;
		Direction = direction;
		Velocity = velocity;
	}
}