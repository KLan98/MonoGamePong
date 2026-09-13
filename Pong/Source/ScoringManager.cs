using LanMonoGameLibrary;
using System;
using System.Diagnostics;

namespace Pong
{
    public class ScoringManager : IObserver
    {
        private int[] scoreBoards; // scores of player and com, index 0 for player, index 1 for com

        public ScoringManager()
        {
            scoreBoards = new int[2]
            {
                0,
                0
            };
        }

        public void OnNotify(object eventData)
        {
            int scoreBoardID = (int)eventData;
            //Debug.WriteLine($"Increment score on scoreboard {scoreBoardID}");
            scoreBoards[scoreBoardID]++;    
        }
    }
}
