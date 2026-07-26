using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

public class PhysicsManager
{
    private float maxVelocity = 10f;
    private EntityPhysics[] movingPhysics;
    private Vector2[] staticPositions;
    private static PhysicsManager instance;
    private Vector2[] sizeVector;

    public PhysicsManager()
    {
        AssetsManager assetsManager = AssetsManager.GetInstance();
        sizeVector = assetsManager.GetSizeVector();
        
        Core core = Core.GetInstance();
        Vector2 screenRes = core.GetScreenResolution();

        // init physical fields of all entities
        Vector2 playerInitPos = new Vector2 { X = 0, Y = screenRes.Y / 2 - sizeVector[3].Y / 2 };
        EntityPhysics playerPhysics = new EntityPhysics(playerInitPos, Vector2.Zero, 0f);

        Vector2 comIntPos = new Vector2 { X = screenRes.X - sizeVector[4].X, Y = playerInitPos.Y };
        EntityPhysics comPhysics = new EntityPhysics(comIntPos, Vector2.Zero, 0f);

        Vector2 ballInitPos = new Vector2 { X = screenRes.X / 2, Y = screenRes.Y/ 2 };
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
            float displacement = movingPhysics[i].Velocity * gameTime.ElapsedGameTime.Milliseconds / 1000f;
            movingPhysics[i].Position += displacement * movingPhysics[i].Direction; // update position
            //Debug.WriteLine($"Position player = {movingPhysics[0].Position}");

            // update collision
            
        }

        Circle ballCollider = new Circle(sizeVector[5].X, movingPhysics[2].Position.X, movingPhysics[2].Position.Y);

        Debug.WriteLine($"ball collider {ballCollider.Radius}, {ballCollider.Position}");

        // create a new circle collision every physics update
        //CircleCollider ballCollider = new CircleCollider(ball, movingPhysics[i].Position.X, movingPhysics[i].Position.Y);

    }

    /// <summary>
    /// Update velocity for entity with index 
    /// </summary>
    /// <param name="index"></param>
    public void UpdateVelocity(int index)
    {
        if (movingPhysics[index].Velocity < maxVelocity)
        {
            // LAN_TODO: check if there is a way to use ease in for this or should it be used for position?
            movingPhysics[index].Velocity = Core.GetInstance().EaseOutCubic(movingPhysics[index].Velocity + 1);
        }
    }

    public void ResetVelocity(int index)
    {
        movingPhysics[index].Velocity = 0;
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