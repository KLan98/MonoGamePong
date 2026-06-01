using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace LanMonoGameLibrary;

public class Core : Game
{
    private static Core instance;

    // The graphics pipeline in MonoGame starts with two components: the GraphicsDeviceManager and SpriteBatch.
    private SpriteBatch spriteBatch; // The SpriteBatch optimizes 2D rendering by batching similar draw calls together, improving draw performance when rendering multiple sprites.
    private GraphicsDeviceManager graphicsDeviceManager; // The GraphicsDeviceManager initializes and manages the connection to the graphics hardware. It handles tasks such as setting the screen resolution, toggling between fullscreen and windowed mode, and managing the GraphicsDevice 

    private GraphicsDevice graphicsDevice; // the interface between your game and the Graphics Processing Unit (GPU) the game is running on
    private ContentManager contentManager;

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
}
