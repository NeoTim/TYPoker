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


            Card hole1 = new Card(ESuit.Hearts,     ECardValue.Four);
            Card hole2 = new Card(ESuit.Hearts,     ECardValue.Six);
            Card flop1 = new Card(ESuit.Spades,     ECardValue.Two);
            Card flop2 = new Card(ESuit.Clubs,     ECardValue.Five);
            Card flop3 = new Card(ESuit.Diamonds,     ECardValue.Ten);



			/*
		   Card hole1 = new Card(ESuit.Clubs, ECardValue.King);
		   Card hole2 = new Card(ESuit.Diamonds, ECardValue.King);

		   Card flop1 = new Card(ESuit.Spades, ECardValue.King);
		   Card flop2 = new Card(ESuit.Hearts, ECardValue.King);
		   Card flop3 = new Card(ESuit.Hearts, ECardValue.Ace); */

			Card turn = new Card(ESuit.Clubs, ECardValue.Ace);
            Card river = new Card(ESuit.Diamonds, ECardValue.Queen);


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
			PokerLogic.CheckFullHouse(aHand); 
            PokerLogic.CheckFlush(aHand);
            PokerLogic.CheckStraight(aHand);
            // PokerLogic.CheckThreeOfAKind(aHand); // Checked in Full house
            // two pair, one pair, high card
            PokerLogic.CheckPair(aHand);

            Console.WriteLine("High card is " + aHand.m_uiHighCard);

            // Console.WriteLine(PokerLogic.CalLowBit(1));


			//Console.WriteLine(PokerLogic.CountBits(1023));

		}
    }
}
