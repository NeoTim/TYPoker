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

        public PokerLogic()
        {
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

            hand.m_bStraightFlush = (allBits & (allBits << 1) & (allBits << 2) & (allBits << 3) & (allBits << 4)) != 0;

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

            Util.DebugAssert(hand.m_bFourOfAKind, "Four Of A Kind");

            return hand.m_bFourOfAKind;
        }

        public static bool CheckFullHouse(Hand hand)
        {
           
            Int32 bitThreeOfAKind = _CheckThreeOfAKind(hand);

            if(hand.m_bThreeOfAKind)
            {
                // 2 x threeOfAKind
                if(BitOperations.CountBits((UInt32)bitThreeOfAKind) > 1)
                {
                    hand.m_bFullHouse = true;
                }
                else
                {
                    if( (~bitThreeOfAKind & (hand.m_iStraightBit[0] & hand.m_iStraightBit[1]) |
                         ~bitThreeOfAKind & (hand.m_iStraightBit[0] & hand.m_iStraightBit[2]) |
                         ~bitThreeOfAKind & (hand.m_iStraightBit[0] & hand.m_iStraightBit[3]) |
                         ~bitThreeOfAKind & (hand.m_iStraightBit[1] & hand.m_iStraightBit[2]) | 
                         ~bitThreeOfAKind & (hand.m_iStraightBit[1] & hand.m_iStraightBit[3]) | 
                         ~bitThreeOfAKind & (hand.m_iStraightBit[2] & hand.m_iStraightBit[3])) != 0 )
                    {
                        // has a pair, TODO: tie breaker value
                        hand.m_bFullHouse = true;
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


            isStraight = (allBits & (allBits << 1) & (allBits << 2) & (allBits << 3) & (allBits << 4)) != 0;

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
					Console.WriteLine("FullhouseCheck: Has 2 three of a kind " + aLowThree + ", " + aHighThree);
                    hand.m_uiHighCard = aHighThree;
				}
				else
				{
					Console.WriteLine("FullhouseCheck: Has 1 three of a kind ");
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
