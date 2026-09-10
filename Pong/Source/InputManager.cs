using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using LanMonoGameLibrary;
using Pong;

public class InputManager
{
    private KeyboardState oldState;
    public bool ToolActive { get; private set; }
    private Vector2 down = new Vector2(0, 1);
    private Vector2 up = new Vector2(0, -1);
    private PhysicsManager physicsManager;

    public InputManager()
    {
        physicsManager = PhysicsManager.GetInstance();
    }

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
            Vector2 direction = physicsManager.GetDirection(0); // player index = 0
            if (direction != up)
            {
                physicsManager.SetDirection(0, up);
            }
        }

        else if (newState.IsKeyUp(Keys.Up))
        {
            if (oldState.IsKeyDown(Keys.Up))
            {
                physicsManager.MovingStop(0);
            }
        }

        if (newState.IsKeyDown(Keys.Down))
        {
            Vector2 direction = physicsManager.GetDirection(0); // player index = 0
            if (direction != down)
            {
                physicsManager.SetDirection(0, down);
            }
        }
        else if (newState.IsKeyUp(Keys.Down))
        {
            if (oldState.IsKeyDown(Keys.Down))
            {
                physicsManager.MovingStop(0);
            }
        }

        oldState = newState;
    }
}
