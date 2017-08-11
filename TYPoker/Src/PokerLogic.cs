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
        public static void CheckStraightFlush(Hand hand)
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


            // Debug
			if (hand.m_bStraightFlush)
			{
				Console.WriteLine("Has straight flush");
			}
			else
			{
				Console.WriteLine("No straight flush");
			}
        }


        public static void CheckFourOfAKind(Hand hand)
        {
            Int32 bitsFourOfAKind = (hand.m_iStraightBit[0] & hand.m_iStraightBit[1] &
                                      hand.m_iStraightBit[2] & hand.m_iStraightBit[3]);

            hand.m_bFourOfAKind =  bitsFourOfAKind!= 0;


            if(hand.m_bFourOfAKind)
            {
                // This value: A-->14, K-->13
                hand.m_uiHighCard = CalLowBit(bitsFourOfAKind) + 1; // AKQ...32(a)
            }



            // Debug
            if(hand.m_bFourOfAKind)
            {
                Console.WriteLine("Has four of a kind " + hand.m_uiHighCard);
            }
            else
            {
                Console.WriteLine("No four of a kind");
            }
        }


        public static void CheckFullHouse(Hand hand)
        {
           
            Int32 bitThreeOfAKind = CheckThreeOfAKind(hand);

            if(hand.m_bThreeOfAKind)
            {
                // 2 x threeOfAKind
                if(CountBits((UInt32)bitThreeOfAKind) > 1)
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

            if (hand.m_bFullHouse)
            {
                Console.WriteLine("Has fullhouse");
            }
            else
            {
                Console.WriteLine("No fullhouse");
            }

        }

        public static bool CheckFlush(Hand hand)
        {
            if (hand.m_bStraightFlush)
            {
                Console.WriteLine("Has Straight flush. Return now.");
                return true;
            }

            if (hand.m_bFourOfAKind)
            {
                Console.WriteLine("Has four of a kind. Return now.");
                return false;
            }

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
                Console.WriteLine("Has flush");
            }
            else
            {
                Console.WriteLine("No flush");
            }

            return isFlush;
        }

        public static bool CheckStright(Hand hand)
        {
			if (hand.m_bStraightFlush) return true;

			if (hand.m_bFourOfAKind) return false;


            // TODO
            return true;
        }


        // Could have two three of a kind
        // E.g. Hole C2, C3, D2, D3, S3, H2, HA
        public static Int32 CheckThreeOfAKind(Hand hand)
        {
            Int32 a = hand.m_iStraightBit[0] & hand.m_iStraightBit[1] & hand.m_iStraightBit[2];
			Int32 b = hand.m_iStraightBit[0] & hand.m_iStraightBit[1] & hand.m_iStraightBit[3];
			Int32 c = hand.m_iStraightBit[0] & hand.m_iStraightBit[2] & hand.m_iStraightBit[3];
			Int32 d = hand.m_iStraightBit[1] & hand.m_iStraightBit[2] & hand.m_iStraightBit[3];

            Int32 bitThreeOfAKind = a | b | c | d;

            hand.m_bThreeOfAKind = bitThreeOfAKind != 0;

            // if a & b & c & d != 0, it's four of a kind

			if(hand.m_bThreeOfAKind)
            {
				Console.WriteLine("Has three of a kind " + hand.m_uiHighCard);


				/// Using lowBit + shift for calculation
				/// You can only have at most 2 three-of-a-kind (7 cards)
				///hand.m_uiHighCard = CalHighBit(bitThreeOfAKind) + 1; // AKQ...32(a)
				UInt32 aLowThree = CalLowBit(bitThreeOfAKind) + 1;

				Int32 tmp = (bitThreeOfAKind >> ((Int32)aLowThree));
				if (tmp > 0) // has another higher three?
				{
					hand.m_uiHighCard = aLowThree + (CalLowBit(tmp) + 1);

					Console.WriteLine("Has 2 three of a kind " + aLowThree + ", " + hand.m_uiHighCard);
				}
				else
				{
					Console.WriteLine("Has 1 three of a kind ");
					hand.m_uiHighCard = aLowThree;
				}
			}
			else
			{
				Console.WriteLine("No three of a kind");
			}

            return bitThreeOfAKind;
		}



        // TODO move to math class
        // 0 based
        // 0, 1     --> 0
        // 2 (10)   --> 1
        public static UInt32 CalLowBit(Int32 b)
        {
            // C++
            // float f = (float)(b & -b);
            // return (*(uint32_t*)&f >> 23) - 0x7f;


            if (b == 0) return 0; // unused

            UInt32 count = 0;
            while((b & 1) == 0)
            {
                b >>= 1;
                count++;
            }

            return count;
		}
/*
        public static UInt32 CalHighBit(Int32 b)
        {
            if (b == 0) return 0; // unused

            UInt32 count = 0;
            UInt32 v = 1;

            while(b > v)
            {
                v <<= 1;
                count++;
            }
            // 0 based counting
            return count;
        }
*/

		// Bit Hacks:  at most 14-bit values in b, perfect for single suit
		public static UInt32 CountBits(UInt32 b)
        {
            return (UInt32)((b * 0x200040008001ul & 0x111111111111111ul)% 0xf);
        }
    }

}
