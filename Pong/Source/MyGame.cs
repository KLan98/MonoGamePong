using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong;

public class MyGame : Core
{
    //--------------------------------FIELDS-------------------------------------------
    private AssetsManager assetsManager;
    private Texture2D spriteSheet;
    private Sprite[] sprites;
    private Sprite boardSprite;
    private DebugConsole debugConsole;
    private Vector2 screenRes;
    private KeyboardState oldState;
    private bool toolActive;
    private Matrix spriteScaleMatrix;

    public MyGame() : base("Pong", 1280, 720, false)
    {
        screenRes = GetScreenResolution();
    }

    protected override void Initialize()
    {
        assetsManager = new AssetsManager(GetContentManager());
        debugConsole = new DebugConsole();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteSheet = assetsManager.GetSpriteSheet();

        sprites = assetsManager.GetSprites();

        // scale the board sprite to screen size
        boardSprite = assetsManager.GetSprite(TextureRegionType.Board);

        float spriteWidth = boardSprite.Width;
        float spriteHeight = boardSprite.Height;

        boardSprite.Scale = new Vector2(Remap(spriteWidth, 0, boardSprite.TextureRegion.GetRectWidth(), 0, screenRes.X) / spriteWidth, Remap(spriteHeight, 0, boardSprite.TextureRegion.GetRectHeight(), 0, screenRes.Y) / spriteHeight);
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        UpdateInput();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GetGraphicsDevice().Clear(Color.CornflowerBlue);

        SpriteBatch.Begin(transformMatrix: spriteScaleMatrix);

        //SpriteBatch.Draw(spriteSheet, Vector2.Zero, Color.White);
        foreach (var sprite in sprites)
        {
            sprite.Draw(SpriteBatch, Vector2.Zero);
        }

        SpriteBatch.End();   

        // Draw debug UI
        ImGuiRenderer.BeginLayout(gameTime);
        debugConsole.Update(toolActive);
        ImGuiRenderer.EndLayout();

        base.Draw(gameTime);
    }
}