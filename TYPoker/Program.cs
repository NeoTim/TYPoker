using System;

using TYPoker.Src;

namespace TYPoker
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            PokerGame aPokerGame = new PokerGame();
            // Testing...


            Card hole1 = new Card(ESuit.Hearts,     ECardValue.Jack);
            Card hole2 = new Card(ESuit.Hearts,     ECardValue.Five);
            Card flop1 = new Card(ESuit.Hearts,     ECardValue.Two);
            Card flop2 = new Card(ESuit.Hearts,     ECardValue.Three);
            Card flop3 = new Card(ESuit.Hearts,     ECardValue.Four);



			/*
		   Card hole1 = new Card(ESuit.Clubs, ECardValue.King);
		   Card hole2 = new Card(ESuit.Diamonds, ECardValue.King);

		   Card flop1 = new Card(ESuit.Spades, ECardValue.King);
		   Card flop2 = new Card(ESuit.Hearts, ECardValue.King);
		   Card flop3 = new Card(ESuit.Hearts, ECardValue.Ace); */

			Card turn = new Card(ESuit.Clubs, ECardValue.Ace);
            Card river = new Card(ESuit.Diamonds, ECardValue.Ace);


            Hand aHand = new Hand();
            aHand.game = aPokerGame;

            aHand.SetHole(hole1, hole2);
            aPokerGame.GoToNextStage();

            aHand.SetFlop(flop1, flop2, flop3);
            aPokerGame.GoToNextStage();

            aHand.SetTurn(turn);
            aPokerGame.GoToNextStage();

            aHand.SetRiver(river, true);
            aPokerGame.GoToNextStage();


            PokerLogic.CheckStraightFlush(aHand); // Royal Flush included
            PokerLogic.CheckFourOfAKind(aHand);
			PokerLogic.CheckFullHouse(aHand); // 
            // PokerLogic.CheckThreeOfAKind(aHand); // Checked in Full house
            PokerLogic.CheckFlush(aHand);

           // Console.WriteLine(PokerLogic.CalLowBit(1));


            //Console.WriteLine(PokerLogic.CountBits(1023));

        }
    }
}
