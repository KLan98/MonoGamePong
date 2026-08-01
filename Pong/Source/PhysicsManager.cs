using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

public class PhysicsManager
{
    private float maxVelocity = 180f;
    private EntityPhysics[] movingPhysics;
    private Vector2[] staticPositions;
    private static PhysicsManager instance;
    private Vector2[] sizeVector;
    private Core core;

    private float[] easingTimeElapsed = new float[3]; // easing time elapsed for every moving entities 
    private float easeDuration = 3; // time it takes to reach max speed

    public PhysicsManager()
    {
        AssetsManager assetsManager = AssetsManager.GetInstance();
        sizeVector = assetsManager.GetSizeVector();

        core = Core.GetInstance();
        Vector2 screenRes = core.GetScreenResolution();

        // init physical fields of all entities
        Vector2 playerInitPos = new Vector2 { X = 0, Y = screenRes.Y / 2 - sizeVector[3].Y / 2 };
        EntityPhysics playerPhysics = new EntityPhysics(playerInitPos, Vector2.Zero, 0f);

        Vector2 comIntPos = new Vector2 { X = screenRes.X - sizeVector[4].X, Y = playerInitPos.Y };
        EntityPhysics comPhysics = new EntityPhysics(comIntPos, Vector2.Zero, 0f);

        Vector2 ballInitPos = new Vector2 { X = screenRes.X / 2, Y = screenRes.Y / 2 };
        EntityPhysics ballPhysics = new EntityPhysics(ballInitPos, Vector2.Zero, 0f);

        // Init positions of non-moving objects
        Vector2 boardInitPos = Vector2.Zero;
        Vector2 playerScoreBarInitPos = Vector2.Zero;
        Vector2 comScoreBarInitPos = new Vector2 { X = screenRes.X - sizeVector[2].X, Y = 0 };

        // should be synced with sprites array in asset manager
        staticPositions = new Vector2[3]
        {
            boardInitPos,
            playerScoreBarInitPos,
            comScoreBarInitPos
        };

        // should be synced with movingSprites array in asset manager
        movingPhysics = new EntityPhysics[3]
        {
            playerPhysics,
            comPhysics,
            ballPhysics,
        };

        if (instance != null)
        {
            return;
        }
        instance = this;
    }

    public static PhysicsManager GetInstance()
    {
        return instance;
    }

    public EntityPhysics GetEntityPhysics(int index)
    {
        return movingPhysics[index];
    }

    public Vector2 GetDirection(int index)
    {
        return movingPhysics[index].Direction;
    }

    public void SetDirection(int index, Vector2 direction)
    {
        movingPhysics[index].Direction = direction;
    }

    public Vector2 GetPosition(int index)
    {
        return movingPhysics[index].Position;
    }

    // called in game-logic-update
    public void UpdatePhysics(GameTime gameTime)
    {
        for (int i = 0; i < movingPhysics.Length; i++)
        {
            float currentVelocity = movingPhysics[i].Velocity;
            Vector2 direction = movingPhysics[i].Direction;

            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // increase easing time
            easingTimeElapsed[i] += deltaTime;

            // easing time so far / total ease duration, clamp it so that it won't exceed 1.0f
            float normalizedElapsed = MathF.Min(1.0f, easingTimeElapsed[i] / easeDuration);

            // Easing function use normalized time as input 
            float easedTime = core.EaseInQuad(normalizedElapsed);

            // lerp between current velocity and max velocity, eased time as input, clamped between current velocity and max velocity
            float easedVelocity = MathHelper.Lerp(currentVelocity, maxVelocity, easedTime);

            float displacement = easedVelocity * deltaTime;

            Vector2 movedDistance = displacement * direction;

            // update position and velocity
            movingPhysics[i].Position += movedDistance;
            movingPhysics[i].Velocity = easedVelocity;
        }

        // create a new circle collision every physics update
        Circle ballCollider = new Circle(sizeVector[5].X, movingPhysics[2].Position.X, movingPhysics[2].Position.Y);
    }

    public void MovingStop(int index)
    {
        movingPhysics[index].Velocity = 0;
        movingPhysics[index].Direction = Vector2.Zero;
        easingTimeElapsed[index] = 0;
    }

    //public string TestElapsedGameTime(GameTime gameTime)
    //{
    //    return $"delta gameTime since the last frame {gameTime.ElapsedGameTime.Milliseconds / 1000f}";
    //}

    public Vector2 GetStaticPosition(int index)
    {
        return staticPositions[index];
    }

    public void SetStaticPosition(int index, Vector2 newPos)
    {
        // only handle the assigning of data, no rendering included 
        staticPositions[index] = newPos;
    }
}