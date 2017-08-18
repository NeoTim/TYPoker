using System;
namespace TYPoker.Src
{
	public enum ESuit
	{
		Clubs,
		Diamonds,
		Hearts,
		Spades,
	}

	public enum ECardValue
	{
		Two, Three, Four, Five, Six, Seven, Eight, Nine,
		Ten, Jack, Queen, King, Ace
	}
	public enum EPokerStage
	{
		Start, Hole, Flop, Turn, River,
	}

    public class PokerLogic
    {

        public static readonly int[] s_aSuitShift = { 0, 3, 6, 9 };

		public const UInt16 kSuitSize = 4;
		public const UInt16 kCardValueSize = 13;

        // 2^31 - 1 = 2147483647 -- 10 digits
        public const Int64 kBaseStraightFlush   = 0x800000; // + [0xf], 7 digits in decimal
        public const Int64 kBaseFourOfAkind     = 0x700000;
        public const Int64 kBaseFullHouse       = 0x600000;
        public const Int64 kBaseFlush           = 0x500000;
        public const Int64 kBaseStraight        = 0x400000;
        public const Int64 kBaseThreeOfAKind    = 0x300000; // + [0xeffff]

		// TODO: figure out the proper range, this will have an overlap
		public const Int64 kBaseTwoPair         = 0x200000; // TODO: + [0xfff]
        public const Int64 kBaseOnePair         = 0x100000; // TODO: + [0xffff]

        public const Int64 kBaseHighCard        = 0x000000; // + [2, 2^14] [0, 0xfffff]



        public PokerLogic()
        {
        }

        public static Int64 GetHandRank(Hand hand)
		{
			hand.DebugPrint();

            Int64 aRank = 0;
			// Utility value
			if (CheckStraightFlush(hand)) // Royal Flush included
			{
                aRank = kBaseStraightFlush;
                aRank += TieBreakerLogic.CalStraightFlushTB(hand.m_iTieBreakerBits);

                Console.WriteLine("Straight Flush: util value = " + aRank);
                return aRank;
			}
			else if (CheckFourOfAKind(hand))
			{
                aRank = kBaseFourOfAkind;
                aRank += TieBreakerLogic.CalFourOfAKindTB((Int32)hand.m_uiHighCard, hand.m_iTieBreakerBits);

                Console.WriteLine("Four of a kind: util value = " + aRank);
                return aRank;
			}
			else if (CheckFullHouse(hand)) // NOTE: will check three of a kind here
			{
                aRank = kBaseFullHouse;
                aRank += TieBreakerLogic.CalFullHouseTB((Int32)hand.m_uiHighCard, hand.m_iTieBreakerBits);

				Console.WriteLine("Fullhouse: util value = " + aRank);
				return aRank;
			}
			else if (CheckFlush(hand))
			{
                aRank = kBaseFlush;

                aRank += TieBreakerLogic.CalFlushTieBreaker(hand.m_iTieBreakerBits);

                Console.WriteLine("Flush: util value = " + aRank);
				return aRank;
			}
			else if (CheckStraight(hand))
			{
                aRank = kBaseStraight;
                aRank += TieBreakerLogic.CalStraightTB(hand.m_iTieBreakerBits);

                Console.WriteLine("Straight: util value = " + aRank);
				return aRank;
			}
			else if (CheckThreeOfAKind(hand)) // Note: only retrieve results
			{
                aRank = kBaseThreeOfAKind;

                aRank += TieBreakerLogic.CalThreeOfAKindTB((Int32)hand.m_uiHighCard, hand.m_iTieBreakerBits);

                Console.Write("Three of a kind: util value = " + aRank);
                return aRank;
			}
			else if (CheckPair(hand) > 1)
			{
                // TODO

				return 0x000000100;
			}
			else if (CheckPair(hand) > 0)
			{
				return 0x000000010;
			}
			else
			{
				CheckHighCard(hand);
				return 0x00000001;
			}
		}


        // Includes Royal Flush
        public static bool CheckStraightFlush(Hand hand)
        {
            // 14 x 4 = 56 < 64, first bit will always be 0, thus Int64/UInt64 don't matter
            // NOTE: cast to int64 before shifting

            Int64 allBits = (Int64)hand.m_iStraightBit[0] | (Int64)hand.m_iStraightBit[1] << 16 |
                                (Int64)hand.m_iStraightBit[2] << 32 | (Int64)hand.m_iStraightBit[3] << 48;

			//Util.BinaryPrint64(allBits);

            // A -> 1, cuz A, 2, 3, 4, 5 is also a Straight called wheel, whcih is the smallest
            allBits |= ((Int64)hand.m_iStraightBit[0] >> 13) | ((Int64)hand.m_iStraightBit[1] >> 13) << 16 |
                       ((Int64)hand.m_iStraightBit[2] >> 13) << 32 | ((Int64)hand.m_iStraightBit[3] >> 13) << 48;

            //Util.BinaryPrint64(allBits);

            allBits = (allBits & (allBits << 1) & (allBits << 2) & (allBits << 3) & (allBits << 4));
           
            //Util.BinaryPrint64(allBits);
            hand.m_iTieBreakerBits = allBits; // new, used for tie breaking, will be overwrite 

            hand.m_bStraightFlush = allBits!= 0;

            Util.DebugAssert(hand.m_bStraightFlush, "Straight Flush");
			
            return hand.m_bStraightFlush;
        }


        public static bool CheckFourOfAKind(Hand hand)
        {
            Int32 bitsFourOfAKind = (hand.m_iStraightBit[0] & hand.m_iStraightBit[1] &
                                      hand.m_iStraightBit[2] & hand.m_iStraightBit[3]);

            hand.m_bFourOfAKind =  bitsFourOfAKind!= 0;

            if(hand.m_bFourOfAKind)
            {
                // This value: A-->14, K-->13, and you can have only 1 four of a kind
                hand.m_uiHighCard = BitOperations.CalLowBit(bitsFourOfAKind) + 1; // AKQ...32(a)
            }

            // New tiebreaker bits: exclude the fourOfAKind bit
            hand.m_iTieBreakerBits = (hand.m_iStraightBit[0] | hand.m_iStraightBit[1] | hand.m_iStraightBit[2]
                                      | hand.m_iStraightBit[3]) & (~bitsFourOfAKind);

            Util.DebugAssert(hand.m_bFourOfAKind, "Four Of A Kind");

            return hand.m_bFourOfAKind;
        }

        public static bool CheckFullHouse(Hand hand)
        {
           
            Int32 bitThreeOfAKind = _CheckThreeOfAKind(hand);

            // After checking three of a kind, highCard is set as the high three of a kind value
            if(hand.m_bThreeOfAKind)
            {
                // 2 x threeOfAKind
                if(BitOperations.CountBits((UInt32)bitThreeOfAKind) > 1)
                {
                    hand.m_bFullHouse = true;

                    // Tie Breaker: Use the lower three of a kind as a pair
                    hand.m_iTieBreakerBits = BitOperations.CalLowBit(bitThreeOfAKind) + 1;
                }
                else
                {
                    Int32 pairBits = ~bitThreeOfAKind & (hand.m_iStraightBit[0] & hand.m_iStraightBit[1]) |
			                         ~bitThreeOfAKind & (hand.m_iStraightBit[0] & hand.m_iStraightBit[2]) |
			                         ~bitThreeOfAKind & (hand.m_iStraightBit[0] & hand.m_iStraightBit[3]) |
			                         ~bitThreeOfAKind & (hand.m_iStraightBit[1] & hand.m_iStraightBit[2]) |
			                         ~bitThreeOfAKind & (hand.m_iStraightBit[1] & hand.m_iStraightBit[3]) |
                                     ~bitThreeOfAKind & (hand.m_iStraightBit[2] & hand.m_iStraightBit[3]);

                    if( pairBits != 0 )
                    {
                        // Tie Breaker, use high pair
                        hand.m_iTieBreakerBits = BitOperations.CalHighBit(pairBits) + 1;
                        hand.m_bFullHouse = true;
                    }
                    else
                    {
                        // Three of a kind + two single cards
                        hand.m_iTieBreakerBits = ~bitThreeOfAKind & (hand.m_iStraightBit[0] | hand.m_iStraightBit[1]
                                                                     | hand.m_iStraightBit[2] | hand.m_iStraightBit[3]);

                        // Console.WriteLine("---!!!!-- " + hand.m_iTieBreakerBits);

                        hand.m_bFullHouse = false;
                    }

                }
            }

            Util.DebugAssert(hand.m_bFullHouse, "Fullhouse");

            return hand.m_bFullHouse;
        }

        public static bool CheckFlush(Hand hand)
        {
            bool isFlush = false;
            int bits = 0;

            switch(hand.game.Stage)
            {
                // Player will have 5 cards at most in Stage Flop
                // so only need to match if the lowest digit
                case EPokerStage.Flop:
                    bits = hand.m_iFlushBit & 0b100100100100;
                    isFlush = (hand.m_iFlushBit & (bits >> 2)) != 0;
                    break;

                // Player could have 5, 6, 7 cards in the same suit
                // (Only one flush coud present)
                case EPokerStage.Turn:
                case EPokerStage.River:
					bits = hand.m_iFlushBit & 0b100100100100;
                    int tmp = (hand.m_iFlushBit & (bits >> 2)) | (hand.m_iFlushBit & (bits >> 1));
                    isFlush = tmp!= 0;
                    break;
            }

            if(isFlush)
            {
                // New Tie Breaker: if two players got flush, need to further compare the values
                if(BitOperations.CountBits((UInt32)hand.m_iStraightBit[0]) >= 5)
                {
                    hand.m_iTieBreakerBits = hand.m_iStraightBit[0];
                }
				else if (BitOperations.CountBits((UInt32)hand.m_iStraightBit[1]) >= 5)
				{
					hand.m_iTieBreakerBits = hand.m_iStraightBit[1];
				}
				else if (BitOperations.CountBits((UInt32)hand.m_iStraightBit[2]) >= 5)
				{
					hand.m_iTieBreakerBits = hand.m_iStraightBit[2];
				}
				else if (BitOperations.CountBits((UInt32)hand.m_iStraightBit[3]) >= 5)
				{
					hand.m_iTieBreakerBits = hand.m_iStraightBit[3];
				}
            }

            Util.DebugAssert(isFlush, "Flush");

            return isFlush;
        }

        public static bool CheckStraight(Hand hand)
        {
            bool isStraight;

            int allBits = hand.m_iStraightBit[0] | hand.m_iStraightBit[1] |
                          hand.m_iStraightBit[2] | hand.m_iStraightBit[3];


            // A -> a
			allBits |= hand.m_iStraightBit[0] >> 13 | hand.m_iStraightBit[1] >> 13 |
					   hand.m_iStraightBit[2] >> 13 | hand.m_iStraightBit[3] >> 13;


            Int32 tmpBits;
            tmpBits = allBits & (allBits << 1) & (allBits << 2) & (allBits << 3) & (allBits << 4);

            isStraight = tmpBits != 0;

            if(isStraight)
            {
				// New, tie breaker
				hand.m_iTieBreakerBits = tmpBits;
            }

            Util.DebugAssert(isStraight, "Straight");

            return isStraight;
        }


        // Could have two three of a kind
        // E.g. Hole C2, C3, D2, D3, S3, H2, HA
        private static Int32 _CheckThreeOfAKind(Hand hand)
        {
            Int32 a = hand.m_iStraightBit[0] & hand.m_iStraightBit[1] & hand.m_iStraightBit[2];
			Int32 b = hand.m_iStraightBit[0] & hand.m_iStraightBit[1] & hand.m_iStraightBit[3];
			Int32 c = hand.m_iStraightBit[0] & hand.m_iStraightBit[2] & hand.m_iStraightBit[3];
			Int32 d = hand.m_iStraightBit[1] & hand.m_iStraightBit[2] & hand.m_iStraightBit[3];

            Int32 bitThreeOfAKind = a | b | c | d;

            hand.m_bThreeOfAKind = bitThreeOfAKind != 0;

			if(hand.m_bThreeOfAKind)
            {
				/// Using lowBit + shift for calculation
				/// Because a player only have at most 2 three-of-a-kind (7 cards)
				///hand.m_uiHighCard = CalHighBit(bitThreeOfAKind) + 1; // AKQ...32(a)
				UInt32 aLowThree = BitOperations.CalLowBit(bitThreeOfAKind) + 1;

				Int32 tmp = (bitThreeOfAKind >> ((Int32)aLowThree));
				if (tmp > 0) // has another higher three?
				{
					UInt32 aHighThree = aLowThree + (BitOperations.CalLowBit(tmp) + 1);
					// Console.WriteLine("FullhouseCheck: Has 2 three of a kind " + aLowThree + ", " + aHighThree);
                    hand.m_uiHighCard = aHighThree;
				}
				else
				{
					// Console.WriteLine("FullhouseCheck: Has 1 three of a kind ");
					hand.m_uiHighCard = aLowThree;
				}
			}

            return bitThreeOfAKind;
		}

        // NOTE: this must be called after FullHouse check
        public static bool CheckThreeOfAKind(Hand hand)
        {
            Util.DebugAssert(hand.m_bThreeOfAKind, "Three Of A Kind");

            return hand.m_bThreeOfAKind;
        }

        public static UInt32 CheckPair(Hand hand)
        {
            // if (hand.m_bFourOfAKind || hand.m_bFullHouse || hand.m_bThreeOfAKind) return true;

            Int32 pairBits = (hand.m_iStraightBit[0] & hand.m_iStraightBit[1]) |
                             (hand.m_iStraightBit[0] & hand.m_iStraightBit[2]) |
                             (hand.m_iStraightBit[0] & hand.m_iStraightBit[3]) |
                             (hand.m_iStraightBit[1] & hand.m_iStraightBit[2]) |
                             (hand.m_iStraightBit[1] & hand.m_iStraightBit[3]) |
                             (hand.m_iStraightBit[2] & hand.m_iStraightBit[3]);


            UInt32 count = BitOperations.CountBits((UInt32)pairBits);

            // TODO: eliminate 3 paris
            // two pair bits + pick single high card
            Console.WriteLine("Number of pairs: " + count);

            return count;
        }

        public static UInt32 CheckHighCard(Hand hand)
        {
            // TODO

            return 0;
        }

    }

}
