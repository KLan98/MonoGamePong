using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

public class PhysicsManager
{
    private float maxSpeed = 300f;
    private EntityPhysics[] movingPhysics;
    private Vector2[] staticPositions;
    private static PhysicsManager instance;
    private Vector2[] sizeVector;
    private Core core;
    private Rectangle[] rectColliders;
    private float[] easingTimeElapsed = new float[3]; // easing time elapsed for every moving entities 
    private float easeDuration = 2; // time it takes to reach max speed

    public PhysicsManager()
    {
        AssetsManager assetsManager = AssetsManager.GetInstance();
        sizeVector = assetsManager.GetSizeVector();

        core = Core.GetInstance();
        Vector2 screenRes = core.GetScreenResolution();

        // init physics objects 
        Vector2 playerInitPos = new Vector2 { X = 0, Y = screenRes.Y / 2 - sizeVector[3].Y / 2 };
        EntityPhysics playerPhysics = new EntityPhysics(playerInitPos, Vector2.Zero);

        Vector2 comIntPos = new Vector2 { X = screenRes.X - sizeVector[4].X, Y = playerInitPos.Y };
        EntityPhysics comPhysics = new EntityPhysics(comIntPos, Vector2.Zero);

        Vector2 ballInitPos = new Vector2 { X = screenRes.X / 2, Y = screenRes.Y / 2 };
        EntityPhysics ballPhysics = new EntityPhysics(ballInitPos, Vector2.Zero);

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

        ballPhysics.Direction = new Vector2(1, 0);
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
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        int screenWidth = (int)core.GetScreenResolution().X;
        int screenHeight = (int)core.GetScreenResolution().Y;

        for (int i = 0; i < movingPhysics.Length; i++)
        {
            float speed = movingPhysics[i].Speed;
            Vector2 direction = movingPhysics[i].Direction;

            // increase easing time
            easingTimeElapsed[i] += deltaTime;

            // easing time so far / total ease duration, clamp it so that it won't exceed 1.0f
            float normalizedElapsed = MathF.Min(1.0f, easingTimeElapsed[i] / easeDuration);

            // Easing function use normalized time as input 
            float easedTime = core.EaseInQuad(normalizedElapsed);

            // lerp between current speed and max speed, eased time as input, clamped between current speed and max speed
            float easedSpeed = MathHelper.Lerp(speed, maxSpeed, easedTime);

            float displacement = easedSpeed * deltaTime;

            Vector2 movedDistance = displacement * direction;

            // update position and speed
            movingPhysics[i].Position += movedDistance;
            movingPhysics[i].Speed = easedSpeed;
        }

        // create a new circle collision every physics update
        // LAN_TODO remove magic index problem 
        Rectangle ballCollider = new Rectangle((int)movingPhysics[2].Position.X, (int)movingPhysics[2].Position.Y, (int)sizeVector[5].X, (int)sizeVector[5].Y);
        //Debug.WriteLine($"ball position {movingPhysics[2].Position}"); 
        //Debug.WriteLine($"{ballCollider} created at {ballCollider.Location}");

        Rectangle topCollider = new Rectangle(0, 0, screenWidth, 1);
        Rectangle botCollider = new Rectangle(0, screenHeight, screenWidth, 1);
        Rectangle leftCollider = new Rectangle(0, 0, 1, screenHeight);
        Rectangle rightCollider = new Rectangle(screenWidth, 0, 0, screenHeight);

        // LAN_TODO: implement collision responses 
        // ballCollider collision responses
        if (ballCollider.Intersects(topCollider) || ballCollider.Intersects(botCollider))
        {
            // blocking and bounce collision response
        }

        else if (ballCollider.Intersects(rightCollider))
        {
            // increase player point
            Debug.WriteLine($"------------Player gained point");
        }

        else if (ballCollider.Intersects(leftCollider))
        {
            // increase com point
            Debug.WriteLine($"------------Com gained point");
        }

        // for player and com
        for (int i = 0; i < 2; i++)
        {
            Rectangle rectCollider = new Rectangle((int)movingPhysics[i].Position.X, (int)movingPhysics[i].Position.Y, (int)sizeVector[3].X, (int)sizeVector[3].Y);

            // if ball collides with player or com
            if (rectCollider.Intersects(ballCollider))
            {
                //Debug.WriteLine($"-------------------------{rectCollider}, info at intersect {rectCollider.Left}, {rectCollider.Right}, {rectCollider.Top}, {rectCollider.Bottom} collides with {ballCollider}, info at intersect {ballCollider.Left}, {ballCollider.Right}, {ballCollider.Top}, {ballCollider.Bottom}");
                // blocking and bounce collision response
            }

            // if player or com collides with either top or bottom bounds
            if (rectCollider.Intersects(topCollider) || rectCollider.Intersects(botCollider))
            {
                //Debug.WriteLine($"-------------------------{rectCollider}, info at intersect {rectCollider.Left}, {rectCollider.Right}, {rectCollider.Top}, {rectCollider.Bottom} collides with {topCollider}, info at intersection {topCollider.Left}, {topCollider.Right}, {topCollider.Top}, {topCollider.Bottom}");
                // blocking collision response
            }
        }
    }

    public void MovingStop(int index)
    {
        movingPhysics[index].Speed = 0;
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