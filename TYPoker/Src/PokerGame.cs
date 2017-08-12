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
            Card holeA1 = new Card(ESuit.Clubs, ECardValue.Jack);
            Card holeA2 = new Card(ESuit.Clubs, ECardValue.King);
            m_listPlayers[0].m_hand.SetHole(holeA1, holeA2);

			// Player B
            Card holeB1 = new Card(ESuit.Diamonds, ECardValue.Ace);
			Card holeB2 = new Card(ESuit.Hearts, ECardValue.Six);
            m_listPlayers[1].m_hand.SetHole(holeB1, holeB2);
        }

        public void DealFlopCards()
        {
            GoToNextStage();

			Card flop1 = new Card(ESuit.Spades, ECardValue.Two);
			Card flop2 = new Card(ESuit.Clubs, ECardValue.Five);
			Card flop3 = new Card(ESuit.Diamonds, ECardValue.Ten);

            m_listPlayers[0].m_hand.SetFlop(flop1, flop2, flop3);
            m_listPlayers[1].m_hand.SetFlop(flop1, flop2, flop3);
        }

        public void DealTurnCard()
        {
            GoToNextStage();
            Card turn = new Card(ESuit.Clubs, ECardValue.Ace);

            m_listPlayers[0].m_hand.SetTurn(turn);
            m_listPlayers[1].m_hand.SetTurn(turn);
        }

        public void DealRiverCard()
        {
            GoToNextStage();
            Card river = new Card(ESuit.Diamonds, ECardValue.Jack);

            m_listPlayers[0].m_hand.SetRiver(river);
            m_listPlayers[1].m_hand.SetRiver(river);
		}

        public void DetermineWins()
        {
            int val1, val2;

            val1 = CheckPlayerHand(m_listPlayers[0].m_hand);

            Console.WriteLine("\n\n");
            val2 = CheckPlayerHand(m_listPlayers[1].m_hand);

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

        private int CheckPlayerHand(Hand aHand)
        {
            aHand.DebugPrint();

            // Utility value
            if(PokerLogic.CheckStraightFlush(aHand)) // Royal Flush included
            {
                return 0x10000000;
            }
            else if (PokerLogic.CheckFourOfAKind(aHand))
            {
                return 0x01000000;
            }
            else if (PokerLogic.CheckFullHouse(aHand)) // NOTE: will check three of a kind here
            {
                return 0x00100000;
            }
            else if (PokerLogic.CheckFlush(aHand))
            {
                return 0x00010000;
            }
            else if(PokerLogic.CheckStraight(aHand))
            {
                return 0x00001000;
            }
            else if(PokerLogic.CheckThreeOfAKind(aHand))
            {
                return 0x00000100;
            }
            else if (PokerLogic.CheckPair(aHand) > 0)
            {
                return 0x00000010;
            }
            else
            {
                PokerLogic.CheckHighCard(aHand);
                // check high card
                return 0x00000001;
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
