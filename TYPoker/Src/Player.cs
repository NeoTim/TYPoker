using System;
namespace TYPoker.Src
{
    public class Player
    {

        private string m_strPlayerId;
        private string m_strPlayerName;

		// This is when you choose to leave a table for a few hands. 
        // If a Player sits out for more than fifteen minutes, or has missed
        // two rounds of blinds, they are removed from the table. 
		private bool m_bSittingOut;
		
        private bool m_bBusted;

        private bool m_bSittingIn;

        private bool m_bFouled;


        // player has multiple compositions
        public Hand m_hand;

        // accounting 

        // account balance

        public Player(string id, string name)
        {
            m_strPlayerId = id;
            m_strPlayerName = name;

            m_hand = new Hand();
        }
    }
}
