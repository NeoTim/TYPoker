using System;
namespace TYPoker.Src
{
    public class TieBreakerLogic
    {
        public TieBreakerLogic()
        {
        }

        public const Int64 kTwoTo48Minus1 = 281474976710655;
        public const Int64 kTwoTo32Minus1 = 4294967295;
        public const Int64 kTwoTo16Minus1 = 65535;

        // --------------- Straight Flush Tie Breaker ------------------
        // If two players have straight flush, the cards must be in the same suit
        // for 2 + 2 + 3 + 1 + 1 < 5 x 2
        //
        // Tie: straight flush cards are community cards
        // Utility Values are designed as below
        // 10, J, Q, K, A --> 0xe // 14
        // 9, 10, J, Q, K --> 0xd // 13
        // ....
        // 2, 3, 4, 5, 6 --> 0x6
        // A, 2, 3, 4, 5 --> 0x5
        // (Value is the highest card in the straight)
        // AddVal = [0x5, 0xe]
        public static Int64 CalStraightFlushTB(Int64 bits)
        {
            if (bits == 0) return 0;

            Int64 addVal = 0x00;

            //Console.WriteLine("bits = " + bits);
            if(bits > kTwoTo48Minus1)
            {
                bits >>= 48;
            }
            else if(bits > kTwoTo32Minus1)
            {
                bits >>= 32;
            }
            else if(bits > kTwoTo16Minus1)
            {
                bits >>= 16;
            }

            addVal = BitOperations.CalHighBit((Int32)bits) + 1;

            Console.WriteLine("Straight flush addVal: " + addVal);
            return addVal;
        }

        // --------------- Four of a kind Tie Breaker ------------------
        // Scenario 1: AAAAJ, 6666A, bigger four of a kind wins
        // Scenario 2: four of kind cards are community cards, check high card
        //              if high card is still shared, tie
        //              
        // To generalize, use 2 hexadecimal digits for Four Of A Kind value
        // 1st digit is the four of a kind card
        // 2nd digit is the single card that has the highest value
        // e.g. AAAAJ  --> 0xeb
        //      6666A  --> 0x6e
        //
        // addVal = [0x23, 0xed] ~ [35, 237]
        public static Int64 CalFourOfAKindTB(Int32 tb1, Int64 tb_bits)
        {
            if (tb1 == 0 && tb_bits == 0) return 0;

            Int64 addVal = 0x00;

            addVal += tb1 << 4;
            Console.WriteLine("tb1 = " + tb1 + ", addVal = " + addVal);

            UInt32 highBit = BitOperations.CalHighBit((Int32)tb_bits) + 1;
            addVal += (int)highBit;
            Console.WriteLine("high bit = " + highBit + ", addVal = " + addVal);

            return addVal;
        }


        // --------------- Fullhouse Tie Breaker ------------------
        // 1st digit: three of a kind
        // 2nd digit: high pair
        //
        // addVal = [0x23, 0xed] ~ [35, 237]
        public static Int64 CalFullHouseTB(Int32 threeOfAkindValue, Int64 pairValue)
        {
            if (pairValue == 0) return 0;
            Int64 addVal = 0x00;

            addVal += threeOfAkindValue << 4;
            Console.WriteLine("tb1 = " + threeOfAkindValue + ", addVal = " + addVal);

            addVal += pairValue;
            Console.WriteLine("pair = " + pairValue + ", addVal = " + addVal);

            return addVal;
        }


        // --------------- Flush Tie Breaker ------------------
        // A flush is any hand with five cards of the same suit. If two or more players hold a flush,
        // the flush with the highest card wins. If more than one player has the same strength high card,
        // then the strength of the second highest card held wins. This continues through the five highest
        // cards in the player's hands.
        //
        // A Q 9 4 2
        // A J 10 4 2
        // 
        // 14 digits are used in encoding a straight
        // 2^14 -1 = 16383 (<-- theoretical highest util value, include straight flush)
        // A, K, Q, J, 9 --> 15616
        // 7, 5, 4, 3, 2 --> 94
        // Only care about the highest 5 cards
        //
        // addVal = [94, 15616] < 0x4000
        public static Int64 CalFlushTieBreaker(Int64 bits)
        {
            Int64 addVal = 0x00;

            // Eliminate 7, 6 cards, compare highest 5 cards
            while(BitOperations.CountBits((uint)bits) > 5)
            {
                Int32 lowBit = (Int32)BitOperations.CalLowBit((Int32)bits) + 1;
                bits = (bits >> lowBit) << lowBit;
            }

            Util.BinaryPrint((Int32)bits, "High 5 flush cards: ");
            addVal += bits;
            Console.WriteLine("flush = " + bits + ", addVal = " + addVal);

            return addVal;
        }

        // --------------- Straight Tie Breaker ------------------
        // Pick highest straight
        // addVal = [0x5, 0xe] 
        public static Int64 CalStraightTB(Int64 bits)
        {
            if (bits == 0) return 0;
            Int64 addVal = 0x00;

            Util.BinaryPrint((Int32)bits, "Straight cards: ");
            uint highBit = BitOperations.CalHighBit((Int32)bits) + 1;

            Console.WriteLine("add Straight util val = " + highBit);

            addVal = highBit;
            return addVal;
        }


        // --------------- Three of a kind Tie Breaker ------------------
        // High threeOfAkind + 2 single cards
        //           0x[2-e]0000 + 0xffff (int converted from straight bits)
        //                         2^12 + 2^13 = 12288 (K,A)
        //                         2^1  + 2^2  = 6     (2,3)
        // addVal < 1048575 (0x100000)
        // << 16 three of a kind value + high 2 bits
        // (14 x 65536 + 16384 = 933,888 < 0x100000 = 1,048,576)
        public static Int64 CalThreeOfAKindTB(Int32 tb1, Int64 bits)
        {
            Console.WriteLine("TB: three of a kind: " + tb1);

            //Util.BinaryPrint((int)bits, "single cards bit: ");

            if (tb1 == 0 && bits == 0) return 0;

            Int64 addVal = tb1 << 16; // make sure it's bigger than the single cards
            Console.WriteLine("TB: three of a kind card addVal = " + addVal);


            // Compare highest 2 cards
            while (BitOperations.CountBits((uint)bits) > 2)
            {
                Int32 lowBit = (Int32)BitOperations.CalLowBit((Int32)bits) + 1;
                bits = (bits >> lowBit) << lowBit;
            }

            Util.BinaryPrint((Int32)bits, "High 2 single cards: ");
            addVal += bits;
            Console.WriteLine("TB: Three of a kind, addVal = " + addVal + " (singles = " + bits + ")");

            return addVal;
        }


        // << 4 pair bits + high card value
        public static Int64 CalTwoPairTB(Int32 tb1, Int64 bits)
        {
            if (tb1 == 0 && bits == 0) return 0;
            Int64 addVal = 0x00;

            Util.BinaryPrint((Int32)bits, "TB: TwoPair bits: ");

            addVal += bits << 4;    // bits that contain two pairs
            addVal += tb1;          // highest single card

            Console.WriteLine("TB: TwoPair " + addVal + " (high card = " + tb1 + ")");
            return addVal;
        }

        // << 16 pair value + high 3 bits
        public static Int64 CalOnePairTB(Int32 tb1, Int64 bits)
        {
            if (tb1 == 0 && bits == 0) return 0;
            Int64 addVal = 0x00;

            Console.WriteLine("TB: OnePair, has a pair of " + tb1);
            addVal += tb1 << 16; // the pair

            // Compare highest 3 single cards
            while (BitOperations.CountBits((uint)bits) > 3)
            {
                Int32 lowBit = (Int32)BitOperations.CalLowBit((Int32)bits) + 1;
                bits = (bits >> lowBit) << lowBit;
            }

            addVal += bits;      // the rest single bits, no need to select high 3

            Util.BinaryPrint((Int32)bits, "TB: OnePair Single card bits: ");
            Console.WriteLine("TB: OnePair " + addVal);
            return addVal;
        }

        public static Int64 CalHighCardTB(Int64 bits)
        {
            if (bits == 0) return 0;
            Int64 addVal = 0x00;

            // Compare highest 5 cards
            while (BitOperations.CountBits((uint)bits) > 5)
            {
                Int32 lowBit = (Int32)BitOperations.CalLowBit((Int32)bits) + 1;
                bits = (bits >> lowBit) << lowBit;
            }

            addVal += bits;

            Util.BinaryPrint((Int32)bits, "TB: High Card bits: ");
            Console.WriteLine("TB: High Card " + addVal);
            return addVal;
        }
    }
}
