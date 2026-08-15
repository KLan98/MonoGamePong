using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace LanMonoGameLibrary;

/// <summary>
/// Reuseable code for many projects
/// </summary>
public class Core : Game
{
    //---------------------------------------FIELDS--------------------------------
    private static Core instance;
    // The graphics pipeline in MonoGame starts with two components: the GraphicsDeviceManager and SpriteBatch.
    private SpriteBatch spriteBatch; // The SpriteBatch optimizes 2D rendering by batching similar draw calls together, improving draw performance when rendering multiple sprites.
    private GraphicsDeviceManager graphicsDeviceManager; // The GraphicsDeviceManager initializes and manages the connection to the graphics hardware. It handles tasks such as setting the screen resolution, toggling between fullscreen and windowed mode,  managing the GraphicsDevice and sprite scaling
    private GraphicsDevice graphicsDevice; // the interface between your game and the Graphics Processing Unit (GPU) the game is running on
    private ContentManager contentManager;
    private KeyboardState oldState;
    protected DebugConsole debugConsole;
    protected const float virtualHeight = 720f; // this is the resolution for mouse coordinates, sprite positions, UI layout, physics,these stay fixed internally
    protected const float virtualWidth = 1280f;

    //-------------------------------------PROPERTIES-----------------------------------------------------
    public SpriteBatch SpriteBatch
    {
        get { return spriteBatch; }
    }
    
    public Matrix SpriteScaleMatrix
    {
        get; private set;
    }

    //------------------------------CONSTRUCTOR-------------------------
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

        UpdateScaleMatrix();

        // set the window title
        Window.Title = title;

        // set the core's content manager to content manager of Game's class
        contentManager = base.Content;

        // set directory for content
        contentManager.RootDirectory = "Content";

        // set mouse visibility
        IsMouseVisible = true;
    }

    protected override void Initialize()
    {
        base.Initialize();

        // set the graphics device to a reference of Game's graphics device
        graphicsDevice = base.GraphicsDevice;

        // create new instance of sprite batch
        spriteBatch = new SpriteBatch(graphicsDevice);

        // create new instance of debug console
        debugConsole = new DebugConsole(graphicsDeviceManager);
        // inititalize debug console
        debugConsole.Initialize(this);
    }

    //-------------------------------PUBLIC METHODS-----------------------------------
    public static Core GetInstance()
    {
        return instance;
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
    /// Called whenever the resolution is updated, compute the transformMatrix through scaleX and scaleY
    /// </summary>
    public void UpdateScaleMatrix()
    {
        float scaleX = graphicsDeviceManager.PreferredBackBufferWidth / virtualWidth;
        float scaleY = graphicsDeviceManager.PreferredBackBufferHeight / virtualHeight;
         SpriteScaleMatrix = Matrix.CreateScale(scaleX, scaleY, 1.0f);
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

    /// <summary>
    /// Set the rendered resolution of the screen through back buffer
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void SetScreenResolution(int width, int height)
    {
        graphicsDeviceManager.PreferredBackBufferWidth = width;
        graphicsDeviceManager.PreferredBackBufferHeight = height;
        graphicsDeviceManager.ApplyChanges();
        UpdateScaleMatrix();
    }

    public Vector2 GetVirtualResolution()
    {
        return new Vector2(virtualWidth, virtualHeight);
    }

    public float EaseOutCubic(float x)
    {
        return 1 - (float)Math.Pow(1 - x, 3);
    }

    public float EaseInQuad(float x)
    {
        return x * x;
    }

    //-----------------------------------------------PRIVATE METHODS---------------------------------------
}