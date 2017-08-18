﻿using System;
namespace TYPoker.Src
{

    public class Card
    {
        private ESuit m_eSuit;
        private ECardValue m_eCardValue;

        public Card(ESuit newSuit, ECardValue newCardValue)
        {
            m_eSuit = newSuit;
            m_eCardValue = newCardValue;
        }

        public ESuit Suit
        {
            get { return m_eSuit; }
        }

        public ECardValue CardValue
        {
            get { return m_eCardValue; }
        }

        public int GetValueIndex()
        {
            // count      0, 1, 2 ...
            // value  (a) 2, 3, 4, 5, 6, 7, 8, 9, 10,  J, Q,  K,  A
            // return (0) 1, 2, 3, 4, 5, 6, 7, 8, 9,  10, 11, 12, 13

            // the first bit 0, is also for Ace when it is considered as 1
            return (int)m_eCardValue + 1;
        }

    }
}
