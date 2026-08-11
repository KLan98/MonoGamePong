using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;

namespace Pong;

public class MyGame : Core
{
    //--------------------------------FIELDS-------------------------------------------
    private AssetsManager assetsManager;
    private InputManager inputManager;
    //private Texture2D spriteSheet;
    private Sprite[] sprites;
    private Sprite[] movingSprites;
    private Sprite boardSprite;
    private PhysicsManager physicsManager;

    public MyGame() : base("Pong", (int)virtualWidth, (int)virtualHeight, false)
    {
    }

    // stuffs that are exclusive for pong should be initialize here
    protected override void Initialize()
    {
        assetsManager = new AssetsManager(GetContentManager());
        physicsManager = new PhysicsManager();
        inputManager = new InputManager(); // for now this input manager is exclusive

        // scale the board sprite to fit screen
        boardSprite = assetsManager.GetSprite(0);

        // called once scale the board to fit screen whenever application starts
        ScaleBoardToFitScreen(GetScreenResolution());

        base.Initialize();
    }

    protected override void LoadContent()
    {
        //spriteSheet = assetsManager.GetSpriteSheet();

        sprites = assetsManager.GetSprites();

        movingSprites = assetsManager.GetMovingSprites();
    }

    // Game loop logic
    protected override void Update(GameTime gameTime)
    {
        inputManager.UpdateInput();
        physicsManager.UpdatePhysics(gameTime);
        //Debug.WriteLine(physicsManager.TestElapsedGameTime(gameTime));
        base.Update(gameTime);
    }

    // Game loop rendering
    protected override void Draw(GameTime gameTime)
    {
        GetGraphicsDevice().Clear(Color.CornflowerBlue);

        // SpriteSortMode.FrontToBack, if layer depth = 1 then object is rendered on top of objects with layer depth = 0
        // SpriteSortMode.BackToFront, if layer depth = 0 then object is rendered on t
        // op of objects with layer depth = 1
        SpriteBatch.Begin(SpriteSortMode.FrontToBack, transformMatrix: SpriteScaleMatrix);
        //SpriteBatch.Draw(spriteSheet, Vector2.Zero, Color.White);

        // draw sprites of moving objects
        for (int i = 0; i < movingSprites.Length; i++)
        {
            movingSprites[i].Draw(SpriteBatch, physicsManager.GetPosition(i));
        }
        
        // draw sprites of non-moving objects
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].Draw(SpriteBatch, physicsManager.GetStaticPosition(i));
        }

        SpriteBatch.End();
        
        // Draw debug UI
        debugConsole.ImGuiRenderer.BeginLayout(gameTime);
        debugConsole.UpdateDraw(inputManager.ToolActive);
        debugConsole.ImGuiRenderer.EndLayout();

        base.Draw(gameTime);
    }

    //----------------------------------PRIVATE METHODS----------------------------------------------
    private void ScaleBoardToFitScreen(Vector2 targetRes)
    {
        Vector2 currentRes = GetScreenResolution();

        float spriteWidth = boardSprite.Width;
        float spriteHeight = boardSprite.Height;

        float scaledX = Remap(spriteWidth, 0, boardSprite.TextureRegion.GetRectWidth(), 0, targetRes.X) / spriteWidth;
        float scaledY = Remap(spriteHeight, 0, boardSprite.TextureRegion.GetRectHeight(), 0, targetRes.Y) / spriteHeight;

        // set the scale factor for board sprite once!
        boardSprite.Scale = new Vector2(scaledX, scaledY);
    }
}