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

        public void GoToNextStage()
        {
            switch(m_eStage)
            {
                case EPokerStage.Start: 
                    m_eStage = EPokerStage.Hole;
                    break;
                case EPokerStage.Hole:  
                    m_eStage = EPokerStage.Flop;
                    break;
                case EPokerStage.Flop:
                    m_eStage = EPokerStage.Turn;
                    break;
                case EPokerStage.Turn:
                    m_eStage = EPokerStage.River;
                    break;
                case EPokerStage.River:
                    m_eStage = EPokerStage.Start;
                    break;
				default:
                    Console.WriteLine("Error: unknown game stage??? Added new stuff?");
                    break;
            }
        }
    }
}
