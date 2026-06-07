using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace LanMonoGameLibrary;

public class Core : Game
{
    private static Core instance;

    // The graphics pipeline in MonoGame starts with two components: the GraphicsDeviceManager and SpriteBatch.
    private SpriteBatch spriteBatch; // The SpriteBatch optimizes 2D rendering by batching similar draw calls together, improving draw performance when rendering multiple sprites.
    private GraphicsDeviceManager graphicsDeviceManager; // The GraphicsDeviceManager initializes and manages the connection to the graphics hardware. It handles tasks such as setting the screen resolution, toggling between fullscreen and windowed mode, and managing the GraphicsDevice 

    private GraphicsDevice graphicsDevice; // the interface between your game and the Graphics Processing Unit (GPU) the game is running on
    private ContentManager contentManager;

    private KeyboardState oldState;

    //-------------------------------------PROPERTIES-----------------------------------------------------
    public SpriteBatch SpriteBatch
    {
        get { return spriteBatch; }
    }

    public Core(string title, int width, int height, bool fullScreen)
    {
        if (instance != null && instance == this)
        {
            throw new InvalidOperationException($"Only a single Core instance can be created");
        }

        instance = this;

        graphicsDeviceManager = new GraphicsDeviceManager(this);

        graphicsDeviceManager.PreferredBackBufferWidth = width;
        graphicsDeviceManager.PreferredBackBufferHeight = height;
        graphicsDeviceManager.IsFullScreen = fullScreen;

        graphicsDeviceManager.ApplyChanges();

        // set the window title
        Window.Title = title;

        // set the core's content manager to content manager of Game's class
        contentManager = base.Content;

        // set directory for content
        contentManager.RootDirectory = "Content";

        // set mouse visibility
        IsMouseVisible = true;
    }

    public static Core GetInstance()
    {
        return instance;
    }

    protected override void Initialize()
    {
        base.Initialize();

        // set the graphics device to a reference of Game's graphics device
        graphicsDevice = base.GraphicsDevice;

        // create new instance of sprite batch
        spriteBatch = new SpriteBatch(graphicsDevice);
    }

    public GraphicsDevice GetGraphicsDevice()
    {
        return graphicsDevice;
    }

    public SpriteBatch GetSpriteBatch()
    {
        return spriteBatch;
    }

    public ContentManager GetContentManager()
    {
        return contentManager;
    }

    /// <summary>
    /// Find where the current value is on the scale of max in min
    /// </summary>
    /// <param name="currentValue"></param>
    /// <param name="maxValue"></param>
    /// <param name="minValue"></param>
    /// <returns></returns>
    public float InverseLerp(float currentValue, float minValue, float maxValue)
    {
        return (currentValue - minValue) / (maxValue - minValue);
    }

    public float Remap(float value, float inMin, float inMax, float outMin, float outMax)
    {
        float t = InverseLerp(value, inMin, inMax);
        return Lerp(t, outMin, outMax);
    }

    public float Lerp(float currentValue, float minValue, float maxValue)
    {
        return minValue + currentValue * (maxValue - minValue);
    }

    /// <summary>
    /// Get the resolution of the screen
    /// </summary>
    public Vector2 GetScreenResolution()
    {
        Vector2 res = new Vector2(graphicsDeviceManager.PreferredBackBufferWidth, graphicsDeviceManager.PreferredBackBufferHeight);
        return res;
    }

    public void SetResolution1080p()
    {
        graphicsDeviceManager.PreferredBackBufferWidth = 1920;
        graphicsDeviceManager.PreferredBackBufferHeight = 1080;
        graphicsDeviceManager.ApplyChanges();
    }

    public void SetResolution600p()
    {
        graphicsDeviceManager.PreferredBackBufferWidth = 800;
        graphicsDeviceManager.PreferredBackBufferHeight = 600;
        graphicsDeviceManager.ApplyChanges();
    }

    public void SetResolution720p()
    {
        graphicsDeviceManager.PreferredBackBufferWidth = 1280;
        graphicsDeviceManager.PreferredBackBufferHeight = 720;
        graphicsDeviceManager.ApplyChanges();
    }

    public void UpdateInput()
    {
        KeyboardState newState = Keyboard.GetState();
        
        // is the a key down?
        if (newState.IsKeyDown(Keys.A))
        {
            // if not then
            if (!oldState.IsKeyDown(Keys.A))
            {
                SetResolution1080p();

                Debug.WriteLine($"Key A pressed, screen resolution {GetScreenResolution().Y}");
            }

            // if a is down then
            else
            {
                // button is being held
            }
        }

        else if (newState.IsKeyDown(Keys.B))
        {
            // if not then
            if (!oldState.IsKeyDown(Keys.B))
            {
                SetResolution600p();

                Debug.WriteLine($"Key B pressed, screen resolution {GetScreenResolution().Y}");
            }

            // if key is down then
            else
            {
                // button is being held
            }
        }

        else if (newState.IsKeyDown(Keys.C))
        {
            // if not then
            if (!oldState.IsKeyDown(Keys.C))
            {
                SetResolution720p();

                Debug.WriteLine($"Key C pressed, screen resolution {GetScreenResolution().Y}");
            }

            // if key is down then
            else
            {
                // button is being held
            }
        }

        oldState = newState;
    }
}
