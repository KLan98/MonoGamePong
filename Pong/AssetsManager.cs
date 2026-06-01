using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class AssetsManager
{
    private ContentManager content;
    private TextureRegion[] textureRegions;
    private Texture2D spriteSheet;

    public AssetsManager(ContentManager content)
    {
        this.content = content;

        spriteSheet = content.Load<Texture2D>("PongAssets/spritesheet");
        CreateTextureRegions();
    }

    public Texture2D GetSpriteSheet()
    {
        return spriteSheet;
    }

    public TextureRegion[] GetTextureRegions()
    {
        return textureRegions;
    }

    private void AddRegion(TextureRegionType type, int x, int y, int width, int height)
    {
        TextureRegion textureRegion = new TextureRegion(spriteSheet, x, y, width, height);

    }

    public TextureRegion GetRegion(TextureRegionType regionType)
    {
        return textureRegions[(int)regionType];
    }

    public void CreateTextureRegions()
    {
        TextureRegion ball = new TextureRegion(spriteSheet, 0, 0, 30, 30);
        TextureRegion ballMotion = new TextureRegion(spriteSheet, ball.GetRectWidth(), 0, 46, 46);
        TextureRegion board = new TextureRegion(spriteSheet, ballMotion.GetRectWidth() + ball.GetRectWidth(), 0, 802, 455);
        TextureRegion computer = new TextureRegion(spriteSheet, board.GetRectX() + board.GetRectWidth(), 0, 17, 120);
        TextureRegion player = new TextureRegion(spriteSheet, computer.GetRectX() + computer.GetRectWidth(), 0, 17, 120);
        TextureRegion scoreBar = new TextureRegion(spriteSheet, player.GetRectX() + player.GetRectWidth(), 0, 341, 47);

        textureRegions = new TextureRegion[6] {
            ball,
            ballMotion,
            board,
            computer,
            player,
            scoreBar,
        };
    }
}

public enum TextureRegionType
{
    Ball,
    BallMotion,
    Board,
    Computer,
    Player,
    ScoreBar
}