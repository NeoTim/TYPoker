using System;

using TYPoker.Src;

namespace TYPoker
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");


			// Testing...
			/*
            Card hole1 = new Card(ESuit.Hearts,     ECardValue.Ace);
            Card hole2 = new Card(ESuit.Hearts,     ECardValue.Five);

            Card flop1 = new Card(ESuit.Hearts,     ECardValue.Two);
            Card flop2 = new Card(ESuit.Hearts,     ECardValue.Three);
            Card flop3 = new Card(ESuit.Hearts,     ECardValue.Four);
            */

			Card hole1 = new Card(ESuit.Clubs, ECardValue.Ace);
			Card hole2 = new Card(ESuit.Diamonds, ECardValue.Ace);

			Card flop1 = new Card(ESuit.Spades, ECardValue.Ace);
			Card flop2 = new Card(ESuit.Hearts, ECardValue.Ace);
			Card flop3 = new Card(ESuit.Hearts, ECardValue.Four);

            Hand aHand = new Hand();

            aHand.SetHole(hole1, hole2);

            aHand.SetFlop(flop1, flop2, flop3);


            PokerLogic.CheckStraightFlush(aHand);
            PokerLogic.CheckFourOfAKind(aHand);


            if(aHand.m_bStraightFlush)
            {
                Console.WriteLine("Has straight flush");
            }
            else
            {
                Console.WriteLine("No straight flush");
            }


            if(aHand.m_bFourOfAKind)
            {
                Console.WriteLine("Has four of a kind " + aHand.m_uiHighCard);
            }
            else
            {
                Console.WriteLine("No four of a kind");
            }

            Console.WriteLine(PokerLogic.CalLowBit(1));


            Console.WriteLine(PokerLogic.CountBits(1023));

        }
    }
}
