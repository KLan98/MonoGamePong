using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using LanMonoGameLibrary;

namespace Pong;

public class MyGame : Core
{
    private AssetsManager assetsManager;
    private Texture2D spriteSheet;
    private TextureRegion ball;
    private TextureRegion board;
    private TextureRegion ballMotion;
    private TextureRegion computer;
    private TextureRegion player;
    private TextureRegion scoreBar;

    public MyGame() : base("Pong", 1280, 720, false)
    {

    }

    protected override void Initialize()
    {
        assetsManager = new AssetsManager(GetContentManager());
        ball = assetsManager.GetRegion(TextureRegionType.Ball);
        ballMotion = assetsManager.GetRegion(TextureRegionType.BallMotion);
        board = assetsManager.GetRegion(TextureRegionType.Board);
        computer = assetsManager.GetRegion(TextureRegionType.Computer);
        player = assetsManager.GetRegion(TextureRegionType.Player);
        scoreBar = assetsManager.GetRegion(TextureRegionType.ScoreBar);
        base.Initialize();
    }

    protected override void LoadContent()
    {
        spriteSheet = assetsManager.GetSpriteSheet();
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GetGraphicsDevice().Clear(Color.CornflowerBlue);

        SpriteBatch.Begin();

        //SpriteBatch.Draw(spriteSheet, Vector2.Zero, Color.White);
        ball.Draw(SpriteBatch, Vector2.Zero, Color.White);
        ballMotion.Draw(SpriteBatch, new Vector2(ballMotion.GetRectX(), 0), Color.White);
        board.Draw(SpriteBatch, new Vector2(board.GetRectX(), 0), Color.White);
        computer.Draw(SpriteBatch, new Vector2(computer.GetRectX(), 0), Color.White);
        player.Draw(SpriteBatch, new Vector2(player.GetRectX(), 0), Color.White);
        scoreBar.Draw(SpriteBatch, new Vector2(scoreBar.GetRectX(), 0), Color.White);

        SpriteBatch.End();   

        base.Draw(gameTime);
    }
}
