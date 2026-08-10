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
    private Sprite[] movingSprites;
    private Texture2D spriteSheet;
    private Vector2[] sizeVector;

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

    public Sprite[] GetMovingSprites()
    {
        return movingSprites;
    }

    public static AssetsManager GetInstance()
    {
        return instance;
    }

    public Sprite GetSprite(int index)
    {
        // can be improved by implementing binary sort
        return sprites[(int)index];
    }

    public Sprite GetMovingSprite(int index)
    {
        return movingSprites[index];
    }

    public Vector2[] GetSizeVector()
    {
        return sizeVector;
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
        ballSprite.LayerDepth = 1.0f;
        //ballSprite.CenterOrigin(); needed for circle collider, currently using rect collider

        Sprite ballMotionSprite = new Sprite(ballMotion);
        ballMotionSprite.LayerDepth = 1.0f;

        Sprite boardSprite = new Sprite(board);
        boardSprite.LayerDepth = 0.0f;

        Sprite computerSprite = new Sprite(computer);
        computerSprite.LayerDepth = 1.0f;
        
        Sprite playerSprite = new Sprite(player);
        playerSprite.LayerDepth = 1.0f;

        Sprite playerScoreBarSprite = new Sprite(scoreBar);
        playerScoreBarSprite.LayerDepth = 0.1f;

        Sprite comScoreBarSprite = new Sprite(scoreBar);
        comScoreBarSprite.LayerDepth = 0.1f;
        comScoreBarSprite.SpriteEffects = SpriteEffects.FlipHorizontally;

        sprites = new Sprite[3]
        {
            boardSprite,
            playerScoreBarSprite,
            comScoreBarSprite
        };

        movingSprites = new Sprite[3]
        {
            playerSprite,
            computerSprite,
            ballSprite
        };
    }
}