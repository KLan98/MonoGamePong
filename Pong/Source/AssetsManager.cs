using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class AssetsManager
{
    private static AssetsManager instance;
    private ContentManager content;
    private TextureRegion[] textureRegions;
    private Sprite[] sprites;
    private Texture2D spriteSheet;

    public AssetsManager(ContentManager content)
    {
        this.content = content;

        spriteSheet = content.Load<Texture2D>("PongAssets/spritesheet");
        InitAssets();
        
        if (instance != null && instance == this)
        {
            return;
        }

        instance = this;
    }

    //-------------------------PUBLIC METHODS-------------------------
    public Texture2D GetSpriteSheet()
    {
        return spriteSheet;
    }

    public Sprite[] GetSprites()
    {
        return sprites;
    }

    public static AssetsManager GetInstance()
    {
        return instance;
    }

    public TextureRegion GetRegion(TextureRegionType regionType)
    {
        return textureRegions[(int)regionType];
    }

    public Sprite GetSprite(TextureRegionType regionType)
    {
        // can be improved by implementing binary sort
        return sprites[(int)regionType];
    }

    //--------------------------------------PRIVATE METHODS---------------------------------
    private void InitAssets()
    {
        // Create texture regions from the sprite sheet
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

        // Create sprites from texture regions
        Sprite ballSprite = new Sprite(ball);
        Sprite ballMotionSprite = new Sprite(ballMotion);
        Sprite boardSprite = new Sprite(board);
        Sprite computerSprite = new Sprite(computer);
        Sprite playerSprite = new Sprite(player);
        Sprite scoreBarSprite = new Sprite(scoreBar);

        sprites = new Sprite[6]
        {
            ballSprite,
            ballMotionSprite,
            boardSprite,
            computerSprite,
            playerSprite,
            scoreBarSprite
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