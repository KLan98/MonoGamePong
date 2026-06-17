using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong;

public class MyGame : Core
{
    //--------------------------------FIELDS-------------------------------------------
    private AssetsManager assetsManager;
    private InputManager inputManager;
    private Texture2D spriteSheet;
    private Sprite[] sprites;
    private Sprite boardSprite;
    private Vector2 screenRes;

    public MyGame() : base("Pong", 1280, 720, false)
    {
        screenRes = GetScreenResolution();
    }

    // stuffs that are exclusive for pong should be initialize here
    protected override void Initialize()
    {
        assetsManager = new AssetsManager(GetContentManager());
        inputManager = new InputManager(); // for now this input manager is exclusive

        // scale the board sprite to fit screen
        boardSprite = AssetsManager.GetInstance().GetSprite(TextureRegionType.Board);
        ScaleToFitScreen(screenRes);

        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteSheet = assetsManager.GetSpriteSheet();

        sprites = assetsManager.GetSprites();
    }

    // Game loop logic
    protected override void Update(GameTime gameTime)
    {
        inputManager.UpdateInput();

        base.Update(gameTime);
    }

    // Game loop rendering
    protected override void Draw(GameTime gameTime)
    {
        GetGraphicsDevice().Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(transformMatrix: SpriteScaleMatrix);
        //SpriteBatch.Draw(spriteSheet, Vector2.Zero, Color.White);
        foreach (var sprite in sprites)
        {
            sprite.Draw(SpriteBatch, Vector2.Zero);
        }
        SpriteBatch.End();
        
        // Draw debug UI
        debugConsole.ImGuiRenderer.BeginLayout(gameTime);
        debugConsole.UpdateDraw(inputManager.ToolActive);
        debugConsole.ImGuiRenderer.EndLayout();

        base.Draw(gameTime);
    }

    //----------------------------------PRIVATE METHODS----------------------------------------------
    private void ScaleToFitScreen(Vector2 targetRes)
    {
        Vector2 currentRes = Core.GetInstance().GetScreenResolution();

        float spriteWidth = boardSprite.Width;
        float spriteHeight = boardSprite.Height;

        float scaledX = Remap(spriteWidth, 0, boardSprite.TextureRegion.GetRectWidth(), 0, targetRes.X) / spriteWidth;
        float scaledY = Remap(spriteHeight, 0, boardSprite.TextureRegion.GetRectHeight(), 0, targetRes.Y) / spriteHeight;

        // set the scale factor for board sprite once!
        boardSprite.Scale = new Vector2(scaledX, scaledY);
    }
}