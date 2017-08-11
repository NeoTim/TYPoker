using System;

//using System.Runtime.CompilerServices;
//[assembly: InternalsVisibleTo("PokerLogic")]

namespace TYPoker.Src
{
    
    public class Hand
    {

        internal Int32 m_iFlushBit;
        internal Int32[] m_iStraightBit = new Int32[PokerLogic.kSuitSize];

        internal Int32 m_iHandValue;
        internal UInt32 m_uiHighCard;

        private Card[] m_CardHole = new Card[2];

        // Copy from Game
        private Card[] m_CardFlop = new Card[3];
        private Card m_CardTurn;
        private Card m_CardRiver;


        internal bool m_bStraightFlush;
        internal bool m_bFullHouse;
        internal bool m_bFourOfAKind;
        internal bool m_bThreeOfAKind;

        public PokerGame game; // Access game stage


        public Hand()
        {
        }

        public void SetHole(Card c1, Card c2, bool debug = false)
        {
            m_iFlushBit += 1 << PokerLogic.s_aSuitShift[(int)c1.Suit];
            m_iFlushBit += 1 << PokerLogic.s_aSuitShift[(int)c2.Suit];

            m_iStraightBit[(int)c1.Suit] |= 1 << c1.GetValueIndex();
            m_iStraightBit[(int)c2.Suit] |= 1 << c2.GetValueIndex();

            m_CardHole[0] = c1;
            m_CardHole[1] = c2;

            m_iHandValue |= 1 << c1.GetValueIndex();
            m_iHandValue |= 1 << c2.GetValueIndex();



            // check Game Stage

            if(debug) DebugPrint();
        }

        public void SetFlop(Card c1, Card c2, Card c3, bool debug = false)
        {
			m_iFlushBit += 1 << PokerLogic.s_aSuitShift[(int)c1.Suit];
			m_iFlushBit += 1 << PokerLogic.s_aSuitShift[(int)c2.Suit];
            m_iFlushBit += 1 << PokerLogic.s_aSuitShift[(int)c3.Suit];

			m_iStraightBit[(int)c1.Suit] |= 1 << c1.GetValueIndex();
			m_iStraightBit[(int)c2.Suit] |= 1 << c2.GetValueIndex();
			m_iStraightBit[(int)c3.Suit] |= 1 << c3.GetValueIndex();

            m_CardFlop[0] = c1;
            m_CardFlop[1] = c2;
            m_CardFlop[2] = c3;

            m_iHandValue |= 1 << c1.GetValueIndex();
            m_iHandValue |= 1 << c2.GetValueIndex();
            m_iHandValue |= 1 << c3.GetValueIndex();

            // check game stage

            if (debug) DebugPrint();
        }

        public void SetTurn(Card c1, bool debug = false)
        {
            m_iFlushBit += 1 << PokerLogic.s_aSuitShift[(int)c1.Suit];
            m_iStraightBit[(int)c1.Suit] |= 1 << c1.GetValueIndex();
            m_CardTurn = c1;
			m_iHandValue |= 1 << c1.GetValueIndex();

			if (debug) DebugPrint();
        }

		public void SetRiver(Card c1, bool debug = false)
		{
			m_iFlushBit += 1 << PokerLogic.s_aSuitShift[(int)c1.Suit];
			m_iStraightBit[(int)c1.Suit] |= 1 << c1.GetValueIndex();
            m_CardRiver = c1;
			m_iHandValue |= 1 << c1.GetValueIndex();

			if (debug) DebugPrint();
		}

        public void DebugPrint()
        {

			Util.BinaryPrint(m_iHandValue);
			Util.BinaryPrint(m_iFlushBit, 12);
			Util.BinaryPrint(m_iStraightBit[0]);
			Util.BinaryPrint(m_iStraightBit[1]);
			Util.BinaryPrint(m_iStraightBit[2]);
			Util.BinaryPrint(m_iStraightBit[3]);


            Console.WriteLine("----------------\n");
        }


    }
}
