using System;
using System.Collections.Generic;


namespace TYPoker.Src
{
    public class PokerGame
    {

		private EPokerStage m_eStage;
		public EPokerStage Stage
		{
			get { return m_eStage; }
		}

        public List<Player> m_listPlayers;


		public PokerGame()
        {
            m_eStage = EPokerStage.Start;

            m_listPlayers = new List<Player>();
        }

        public Player JoinPlayer(string id, string name)
        {
            Player aPlayer = new Player(id, name);

            aPlayer.m_hand.game = this; // TODO

            m_listPlayers.Add(aPlayer);

            return aPlayer;
        }

        public void StartNewRound()
        {
            m_eStage = EPokerStage.Start;

            Console.WriteLine("Starting a new round");
        }

        public void DealHandCards()
        {
            GoToNextStage();

            // Player A
            Card holeA1 = new Card(ESuit.Clubs, ECardValue.Eight);
            Card holeA2 = new Card(ESuit.Diamonds, ECardValue.Eight);
            m_listPlayers[0].m_hand.SetHole(holeA1, holeA2);

			// Player B
            Card holeB1 = new Card(ESuit.Spades, ECardValue.Seven);
            Card holeB2 = new Card(ESuit.Hearts, ECardValue.Seven);
            m_listPlayers[1].m_hand.SetHole(holeB1, holeB2);
        }

        public void DealFlopCards()
        {
            GoToNextStage();

            Card flop1 = new Card(ESuit.Diamonds, ECardValue.Five);
            Card flop2 = new Card(ESuit.Spades, ECardValue.Five);
            Card flop3 = new Card(ESuit.Clubs, ECardValue.Four);

            m_listPlayers[0].m_hand.SetFlop(flop1, flop2, flop3);
            m_listPlayers[1].m_hand.SetFlop(flop1, flop2, flop3);
        }

        public void DealTurnCard()
        {
            GoToNextStage();
            Card turn = new Card(ESuit.Diamonds, ECardValue.Four);

            m_listPlayers[0].m_hand.SetTurn(turn);
            m_listPlayers[1].m_hand.SetTurn(turn);
        }

        public void DealRiverCard()
        {
            GoToNextStage();
            Card river = new Card(ESuit.Diamonds, ECardValue.Three);

            m_listPlayers[0].m_hand.SetRiver(river);
            m_listPlayers[1].m_hand.SetRiver(river);
		}

        public void DetermineWins()
        {
            Int64 val1, val2;

            val1 = PokerLogic.GetHandRank(m_listPlayers[0].m_hand);

            Console.WriteLine("\n\n");
            val2 = PokerLogic.GetHandRank(m_listPlayers[1].m_hand);

			Console.WriteLine("\n\n");
            if(val1 < val2)
            {
                Console.WriteLine("Player1 lost; Player2 won");
            }
            else if(val1 == val2)
            {
                Console.WriteLine("Player1 Player2 tied: TODO check tier breaker");
            }
            else if(val1 > val2)
            {
                Console.WriteLine("Player1 won; Player2 lost");
            }
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
