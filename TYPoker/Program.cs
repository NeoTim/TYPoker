using System;

using TYPoker.Src;

namespace TYPoker
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            PokerGame aPokerGame = new PokerGame();
           
            aPokerGame.JoinPlayer("001", "Amy");
            aPokerGame.JoinPlayer("002", "Bob");

            aPokerGame.StartNewRound();
            aPokerGame.DealHandCards();
            aPokerGame.DealFlopCards();
            aPokerGame.DealTurnCard();
            aPokerGame.DealRiverCard();


            aPokerGame.DetermineWins();


        }
    }
}
