using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using LanMonoGameLibrary;


public class InputManager
{
    private KeyboardState oldState;
    public bool ToolActive { get; private set; }

    public void UpdateInput()
    {
        KeyboardState newState = Keyboard.GetState();

        // when d is pressed
        if (newState.IsKeyDown(Keys.D))
        {
            // only process when the old state in left control and old state is not d 
            if (oldState.IsKeyDown(Keys.LeftControl) && !oldState.IsKeyDown(Keys.D))
            {
                ToolActive = !ToolActive;
                Debug.WriteLine("Debug console toggled");
            }
        }

        else if (newState.IsKeyDown(Keys.Escape))
        {
            Core.GetInstance().Exit();
        }

        oldState = newState;
    }
}
