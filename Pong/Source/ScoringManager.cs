using LanMonoGameLibrary;
using System;
using System.Diagnostics;

namespace Pong
{
    public class ScoringManager
    {
        private int[] scores; // scores of player and com, index 0 for player, index 1 for com
        private TextRenderManager textRenderManager;

        public ScoringManager(TextRenderManager textRenderManager)
        {
            scores = new int[2]
            {
                0,
                0
            };

            this.textRenderManager = textRenderManager;
        }

        public void OnUpdateScore(object eventData)
        {
            if (!Core.TryGet(eventData, out int scoringIndex))
            {
                return;
            }

            scores[scoringIndex]++;

            int score = scores[scoringIndex];

            // update the scoringIndex
            textRenderManager.UpdateScoreBoard(score, scoringIndex);
        }
    }
}
