using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using LanMonoGameLibrary;

public class InputManager
{
    private KeyboardState oldState;
    public bool ToolActive { get; private set; }
    private Vector2 down = new Vector2(0, 1);
    private Vector2 up = new Vector2(0, -1);

    public void UpdateInput()
    {
        KeyboardState newState = Keyboard.GetState();
        
        // toggle debug console
        if (newState.IsKeyDown(Keys.D))
        {
            // only process when the old state in left control and old state is not d 
            if (oldState.IsKeyDown(Keys.LeftControl) && !oldState.IsKeyDown(Keys.D))
            {
                ToolActive = !ToolActive;
                Debug.WriteLine("Debug console toggled");
            }
        }

        // exit application
        if (newState.IsKeyDown(Keys.Escape))
        {
            Core.GetInstance().Exit();
        }

        //------------------------PLAYER CONTROL-------------------
        if (newState.IsKeyDown(Keys.Up))
        {
            Vector2 direction = PhysicsManager.GetInstance().GetDirection(0); // player index = 0
            if (direction != up)
            {
                PhysicsManager.GetInstance().SetDirection(0, up);
            }

            PhysicsManager.GetInstance().UpdateVelocity(0);
            Debug.WriteLine($"Velocity of player {PhysicsManager.GetInstance().GetEntityPhysics(0).Velocity}");
        }
        else if (newState.IsKeyUp(Keys.Up))
        {
            if (oldState.IsKeyDown(Keys.Up))
            {
                PhysicsManager.GetInstance().ResetVelocity(0);
                Debug.WriteLine($"Velocity of player {PhysicsManager.GetInstance().GetEntityPhysics(0).Velocity}");
            }
        }

        if (newState.IsKeyDown(Keys.Down))
        {
            Vector2 direction = PhysicsManager.GetInstance().GetDirection(0); // player index = 0
            if (direction != down)
            {
                PhysicsManager.GetInstance().SetDirection(0, down);
            }

            PhysicsManager.GetInstance().UpdateVelocity(0);
            Debug.WriteLine($"Velocity of player {PhysicsManager.GetInstance().GetEntityPhysics(0).Velocity}");
        }
        else if (newState.IsKeyUp(Keys.Down))
        {
            if (oldState.IsKeyDown(Keys.Down))
            {
                PhysicsManager.GetInstance().ResetVelocity(0);
                Debug.WriteLine($"Velocity of player {PhysicsManager.GetInstance().GetEntityPhysics(0).Velocity}");
            }
        }

        oldState = newState;
    }
}
