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


        public static void CheckStraightFlush(Hand hand)
        {
            // 14 x 4 = 56 < 64, first bit will always be 0, thus Int64/UInt64 don't matter

            Int64 allBits = hand.m_iStraightBit[0] | hand.m_iStraightBit[1] << 16 |
                            hand.m_iStraightBit[2] << 32 | hand.m_iStraightBit[3] << 48;

            // A -> 1
            allBits |= (hand.m_iStraightBit[0] >> 13) | (hand.m_iStraightBit[1] >> 13) << 16 |
                       (hand.m_iStraightBit[2] >> 13) << 32 | (hand.m_iStraightBit[3] >> 13) << 16;


            hand.m_bStraightFlush = (allBits & (allBits << 1) & (allBits << 2) & (allBits << 3) & (allBits << 4)) != 0;

        }


        public static void CheckFourOfAKind(Hand hand)
        {
            Int32 bitsFourOfAKind = (hand.m_iStraightBit[0] & hand.m_iStraightBit[1] &
                                      hand.m_iStraightBit[2] & hand.m_iStraightBit[3]);

            hand.m_bFourOfAKind =  bitsFourOfAKind!= 0;


            if(hand.m_bFourOfAKind)
            {
                // This value: A-->14, K-->13
                hand.m_uiHighCard = CalLowBit(bitsFourOfAKind) + 1;
            }
        }

        // Could have two three of a kind
        // E.g. Hole C2, C3, D2, D3, S3, H2, HA
        public static void CheckThreeOfAKind(Hand hand)
        {


        }



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

		// Bit Hacks:  at most 14-bit values in b, perfect for single suit
		public static UInt32 CountBits(UInt32 b)
        {
            return (UInt32)((b * 0x200040008001ul & 0x111111111111111ul)% 0xf);
        }
    }

}
