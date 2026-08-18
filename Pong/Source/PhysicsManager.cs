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
    private Core core;
    private Rectangle[] rectColliders;
    private float[] easingTimeElapsed = new float[3]; // easing time elapsed for every moving entities 
    private float easeDuration = 2; // time it takes to reach max speed
    private Sprite[] movingSprites = new Sprite[3];
    private Sprite[] sprites = new Sprite[3];
    private Vector2[] initPos = new Vector2[3];
    private int screenVWidth, screenVHeight;
    private Rectangle topCollider, botCollider, leftCollider, rightCollider;
    private Vector2 normal; // normal used for calculating reflection vector

    public PhysicsManager()
    {
        AssetsManager assetsManager = AssetsManager.GetInstance();
        movingSprites = assetsManager.GetMovingSprites();
        sprites = assetsManager.GetSprites();

        core = Core.GetInstance();
        Vector2 screenVRes = core.GetVirtualResolution();

        // init physics objects 
        Vector2 playerInitPos = new Vector2 { X = 0, Y = screenVRes.Y / 2 - movingSprites[0].Size.Y / 2 };
        EntityPhysics playerPhysics = new EntityPhysics(playerInitPos, Vector2.Zero);

        Vector2 comIntPos = new Vector2 { X = screenVRes.X - movingSprites[1].Size.X, Y = playerInitPos.Y };
        EntityPhysics comPhysics = new EntityPhysics(comIntPos, Vector2.Zero);

        Vector2 ballInitPos = new Vector2 { X = screenVRes.X / 2, Y = screenVRes.Y / 2 };
        EntityPhysics ballPhysics = new EntityPhysics(ballInitPos, Vector2.Zero);

        // Init positions of non-moving objects
        Vector2 boardInitPos = Vector2.Zero;
        Vector2 playerScoreBarInitPos = Vector2.Zero;
        Vector2 comScoreBarInitPos = new Vector2 { X = screenVRes.X - sprites[2].Size.X, Y = 0 };

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

        //ballPhysics.Direction = new Vector2(-1, 0);
        ballPhysics.Direction = new Vector2(0.1f, 0.3f);

        screenVWidth = (int)core.GetVirtualResolution().X;
        screenVHeight = (int)core.GetVirtualResolution().Y;

        // Updated based on screen resolution
        topCollider = new Rectangle(0, 0, screenVWidth, 1);
        botCollider = new Rectangle(0, screenVHeight, screenVWidth, 1);
        leftCollider = new Rectangle(0, 0, 1, screenVHeight);
        rightCollider = new Rectangle(screenVWidth, 0, 0, screenVHeight);

        normal = Vector2.Zero;
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

        //rectColliders = new Rectangle[movingPhysics.Length];

        for (int i = 0; i < movingPhysics.Length; i++)
        {
            //rectColliders[i] = new Rectangle((int)movingPhysics[i].Position.X, (int)movingPhysics[i].Position.Y, (int)movingSprites[i].Size.X, (int)movingSprites[i].Size.Y);
            Vector2 currentPosition = movingPhysics[i].Position; // store the current position
            Vector2 direction = movingPhysics[i].Direction;
            float currentSpeed = movingPhysics[i].Speed;

            // increase easing time only when moving
            if (movingPhysics[i].Direction != Vector2.Zero)
            {
                easingTimeElapsed[i] += deltaTime;
            }

            // easing time so far / total ease duration, clamp it so that it won't exceed 1.0f
            float normalizedElapsed = MathF.Min(1.0f, easingTimeElapsed[i] / easeDuration);

            // Easing function use normalized time as input 
            float easedTime = core.EaseInQuad(normalizedElapsed);

            // lerp between current speed and max speed, eased time as input, clamped between current speed and max speed
            float easedSpeed = MathHelper.Lerp(currentSpeed, maxSpeed, easedTime);

            float displacement = easedSpeed * deltaTime;

            Vector2 movedDistance = displacement * direction;
            Vector2 velocity = easedSpeed * direction; // speed * direction
            Vector2 newPosition = currentPosition + movedDistance; 

            // if player or com collides with either top or bottom bounds
            if (i < 2)
            {
                // construct a predicted rect based on the new position, this is the next position the player will move to, but currently in currentPosition
                Rectangle predictedRect = new Rectangle(
                    (int)newPosition.X, (int)newPosition.Y,
                    (int)movingSprites[i].Size.X, (int)movingSprites[i].Size.Y);

                // if entity intersects top collider and is moving upward (negative movedDistance indicates moving upward)
                if (predictedRect.Intersects(topCollider) && movedDistance.Y < 0)
                {
                    newPosition.Y = currentPosition.Y; // block only upward movement
                }

                // if entity intersects top collider and is moving downward (positive movedDistance indicates moving downward)
                else if (predictedRect.Intersects(botCollider) && movedDistance.Y > 0)
                {
                    newPosition.Y = currentPosition.Y; // block only downward movement
                }
            }

            movingPhysics[i].Position = newPosition;
            movingPhysics[i].Velocity = velocity;
            movingPhysics[i].Speed = easedSpeed;
            //else if (i == 2)
            //{
            //    if (rectColliders[i].Intersects(rectColliders[1]))
            //    {
            //        normal = Vector2.UnitX;
            //    }

            //    else if (rectColliders[i].Intersects(rectColliders[0]))
            //    {
            //        normal = -Vector2.UnitX;
            //    }

            //    else if (rectColliders[i].Intersects(topCollider))
            //    {
            //        normal = Vector2.UnitY;
            //    }

            //    else if (rectColliders[i].Intersects(botCollider))
            //    {
            //        normal = -Vector2.UnitY;
            //    }

            //    else if (rectColliders[i].Intersects(leftCollider))
            //    {
            //        Debug.WriteLine("Enemy scored");
            //        // increment point
            //    }

            //    else if (rectColliders[i].Intersects(rightCollider))
            //    {
            //        Debug.WriteLine("Player scored");
            //        // increment point
            //    }

            //    else
            //    {
            //        movingPhysics[i].Position += movedDistance;
            //        movingPhysics[i].Velocity = velocity;
            //        movingPhysics[i].Speed = easedSpeed;
            //    }

            //    // if ball collides with top or bottom colliders update direction of ball
            //    movingPhysics[i].Direction = Vector2.Reflect(movingPhysics[i].Direction, normal);
            //}
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

    public Rectangle GetColliderInfo(int index)
    {
        if (rectColliders == null)
        {
            return new Rectangle(0, 0, 0, 0);
        }

        return rectColliders[index];
    }

    // set an entity to its init position
    public void ResetPosition(int index)
    {
        // get the current screen res
        Vector2 screenVRes = core.GetVirtualResolution();
        Debug.WriteLine($"{screenVRes}");
        MovingStop(index);

        // set init pos
        switch (index)
        {
            case (int)MovingEntities.Player:
                movingPhysics[index].Position = new Vector2 { X = 0, Y = screenVRes.Y / 2 - movingSprites[0].Size.Y / 2 };
                break;

            case (int)MovingEntities.Com:
                movingPhysics[index].Position = new Vector2 { X = screenVRes.X - movingSprites[1].Size.X, Y = screenVRes.Y / 2 - movingSprites[1].Size.Y / 2 };
                break;

            case (int)MovingEntities.Ball:
                movingPhysics[index].Position = new Vector2 { X = screenVRes.X / 2, Y = screenVRes.Y / 2 };
                break;
        }
    }

    public enum MovingEntities
    {
        Player,
        Com,
        Ball
    }

    public enum StaticEntities
    {
        Board,
        PlayerScoreBar,
        ComScoreBar
    }
}