using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pong;
using System;
using System.Collections.Generic;
using System.Diagnostics;

public class AssetsManager : IObserver, ISubject
{
    private static AssetsManager instance;
    private ContentManager content;
    private TextureRegion[] textureRegions;
    private Sprite[] sprites;
    private Sprite[] movingSprites;
    private Sprite[] ballSprites;
    private Texture2D spriteSheet;
    private Vector2[] sizeVector;
    private TextureRegion ballTextureRegion;

    public Dictionary<EventType, List<IObserver>> ObserversDict { get; set; }

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

        ObserversDict = new Dictionary<EventType, List<IObserver>>();
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

    public Sprite[] GetBallSprites()
    {
        return ballSprites;
    }

    public Vector2[] GetSizeVector()
    {
        return sizeVector;
    }

    //--------------------------------------PRIVATE METHODS---------------------------------
    private void InitAssets()
    {
        // Create texture regions from the sprite sheet
        ballTextureRegion = new TextureRegion(spriteSheet, 0, 0, 30, 30);
        TextureRegion ballMotionTextureRegion = new TextureRegion(spriteSheet, ballTextureRegion.GetRectWidth(), 0, 46, 46);
        TextureRegion boardTextureRegion = new TextureRegion(spriteSheet, ballMotionTextureRegion.GetRectWidth() + ballTextureRegion.GetRectWidth(), 0, 802, 455);
        TextureRegion computerTextureRegion = new TextureRegion(spriteSheet, boardTextureRegion.GetRectX() + boardTextureRegion.GetRectWidth(), 0, 17, 120);
        TextureRegion playerTextureRegion = new TextureRegion(spriteSheet, computerTextureRegion.GetRectX() + computerTextureRegion.GetRectWidth(), 0, 17, 120);
        TextureRegion scoreBarTextureRegion = new TextureRegion(spriteSheet, playerTextureRegion.GetRectX() + playerTextureRegion.GetRectWidth(), 0, 341, 47);

        textureRegions = new TextureRegion[6] {
            ballTextureRegion,
            ballMotionTextureRegion,
            boardTextureRegion,
            computerTextureRegion,
            playerTextureRegion,
            scoreBarTextureRegion,
        };

        // Create sprites from texture regions
        Sprite ballSprite = new Sprite(ballTextureRegion);
        ballSprite.LayerDepth = 1.0f;
        //ballSprite.CenterOrigin(); needed for circle collider, currently using rect collider

        Sprite ballMotionSprite = new Sprite(ballMotionTextureRegion);
        ballMotionSprite.LayerDepth = 1.0f;

        Sprite boardSprite = new Sprite(boardTextureRegion);
        boardSprite.LayerDepth = 0.0f;

        Sprite computerSprite = new Sprite(computerTextureRegion);
        computerSprite.LayerDepth = 1.0f;

        Sprite playerSprite = new Sprite(playerTextureRegion);
        playerSprite.LayerDepth = 1.0f;

        Sprite playerScoreBarSprite = new Sprite(scoreBarTextureRegion);
        playerScoreBarSprite.LayerDepth = 0.1f;

        Sprite comScoreBarSprite = new Sprite(scoreBarTextureRegion);
        comScoreBarSprite.LayerDepth = 0.1f;
        comScoreBarSprite.SpriteEffects = SpriteEffects.FlipHorizontally;

        sprites = new Sprite[3]
        {
            boardSprite,
            playerScoreBarSprite,
            comScoreBarSprite
        };

        movingSprites = new Sprite[2]
        {
            playerSprite,
            computerSprite,
        };

        ballSprites = new Sprite[1]
        {
            ballSprite
        };
    }

    public void OnNotify(object eventData)
    {
        int numberOfBalls = (int)eventData;
        Debug.WriteLine("event 1 fired");

        // Re-init ballSprites array
        ballSprites = new Sprite[numberOfBalls];

        for (int i = 0; i < numberOfBalls; i++)
        {
            Sprite ballSprite = new Sprite(ballTextureRegion);
            ballSprite.LayerDepth = 1.0f;
            ballSprites[i] = ballSprite;
        }

        Notify(EventType.ASSET_MAMAGER_BALL_ASSETS_UPDATED, numberOfBalls);
    }

    public void Notify(EventType eventType, object eventData)
    {
        if (ObserversDict.TryGetValue(eventType, out List<IObserver> observers))
        {
            foreach (IObserver observer in observers)
            {
                observer.OnNotify(eventData);
            }
        }
    }

    public void AddObserver(EventType eventType, IObserver observer)
    {
        // if the key already exists then add the observer to its associated list
        if (ObserversDict.TryGetValue(eventType, out List<IObserver> observers))
        {
            observers.Add(observer);
        }

        // if not then create a new key - value entry in the dictionary
        else
        {
            ObserversDict.Add(eventType, new List<IObserver> { observer });
        }
    }

    public void RemoveObserver(EventType eventType, IObserver observer)
    {
        if (ObserversDict.TryGetValue(eventType, out List<IObserver> observers))
        {
            observers.Remove(observer);
        }
    }
}