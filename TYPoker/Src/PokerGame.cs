using System;



namespace TYPoker.Src
{
    public class PokerGame
    {

		private EPokerStage m_eStage;
		public EPokerStage Stage
		{
			get { return m_eStage; }
		}


		public PokerGame()
        {
            m_eStage = EPokerStage.Start;
        }
    }
}
