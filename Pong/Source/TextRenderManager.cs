using System;
using LanMonoGameLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pong
{
    public class TextRenderManager : IObserver
    {
        private AssetsManager assetsManager;
        private SpriteFont[] fonts;

        public TextRenderManager()
        {
            assetsManager = AssetsManager.GetInstance();
            fonts = assetsManager.GetFonts();
        }

        // Will be observer of PongFSM 
        public void OnNotify(object eventData)
        {
            throw new NotImplementedException();
        }

        public void UpdateDraw(SpriteBatch spriteBatch)
        {
            if (fonts.Length == 0)
            {
                return;
            }

            //for (int i = 0; i < fonts.Length; i++)
            //{
            //    spriteBatch.DrawString(fonts[i], "hello world", Vector2.Zero, Color.White);
            //}
        }
    }
}
