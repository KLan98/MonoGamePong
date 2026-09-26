using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using static Pong.GameConstants;

namespace Pong
{
    public class TextRenderManager
    {
        private AssetsManager assetsManager;
        private SpriteFont[] fonts;
        private GameText[] gameTexts;

        public TextRenderManager()
        {
            assetsManager = AssetsManager.GetInstance();
            fonts = assetsManager.GetFonts();
            gameTexts = assetsManager.GetGameTexts();

            gameTexts[0].Position = Vector2.Zero; // player's score
            gameTexts[1].Position = new Vector2 { X = VIRTUAL_WIDTH - gameTexts[1].SpriteFont.MeasureString(gameTexts[1].Text).X, Y = 0}; // com's score
            gameTexts[2].Position = new Vector2 { X = VIRTUAL_WIDTH / 2, Y = VIRTUAL_HEIGHT / 2 };
        }

        public void OnDisplayScoringMessage(object eventData)
        {
            string scoringMessage = eventData as string;

            if (scoringMessage != null)
            {
                gameTexts[2].Text = scoringMessage;
                gameTexts[2].CenterOrigin();
            }
        }

        public void UpdateScoreBoard(int score, int index)
        {
            gameTexts[index].Text = score.ToString();
        }

        public void OnServeCountDown(object eventData)
        {
            string countDownMessage;

            if (!Core.TryGet(eventData, out float stateTime))
            {
                return;
            }

            if (stateTime <= 1f)
            {
                countDownMessage = "3";
            }

            else if (1f < stateTime && stateTime <= 2f)
            {
                countDownMessage = "2";
            }

            else if (2f < stateTime && stateTime <= 2.9f)
            {
                countDownMessage = "1";
            }

            else
            {
                countDownMessage = "";
            }

            gameTexts[2].Text = countDownMessage;
            gameTexts[2].CenterOrigin();
        }

        public void UpdateDraw(SpriteBatch spriteBatch)
        {
            if (fonts.Length == 0)
            {
                return;
            }

            for (int i = 0; i < gameTexts.Length; i++)
            {
                gameTexts[i].Draw(spriteBatch);
            }
        }
    }
}
