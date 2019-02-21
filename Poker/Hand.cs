using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poker
{
    public class Hand
    {
        private static Dictionary<char, int> ValueToNumber = new Dictionary<char, int>();
        private static Dictionary<char, Suits> CharToSuit = new Dictionary<char, Suits>();

        private List<Card> cards = new List<Card>();
        private Ranks rank = Ranks.NotSet;
        private List<int> values = new List<int>();

        static Hand()
        {
            ValueToNumber.Add('2', 2);
            ValueToNumber.Add('3', 3);
            ValueToNumber.Add('4', 4);
            ValueToNumber.Add('5', 5);
            ValueToNumber.Add('6', 6);
            ValueToNumber.Add('7', 7);
            ValueToNumber.Add('8', 8);
            ValueToNumber.Add('9', 9);
            ValueToNumber.Add('T', 10);
            ValueToNumber.Add('J', 11);
            ValueToNumber.Add('Q', 12);
            ValueToNumber.Add('K', 13);
            ValueToNumber.Add('A', 14);

            CharToSuit.Add('H', Suits.Hearts);
            CharToSuit.Add('C', Suits.Clubs);
            CharToSuit.Add('D', Suits.Diamonds);
            CharToSuit.Add('S', Suits.Spades);
        }

        public Hand(string hand)
        {
            FillCards(hand);

            EvaluateHand();
        }

        private void EvaluateHand()
        {
            var temp = new Dictionary<int, int>();
            var sameSuit = true;
            var compareSuit = cards[0].Suit;
            foreach (var c in cards)
            {
                if (!temp.ContainsKey(c.Value)) temp.Add(c.Value, 0);
                temp[c.Value]++;
                if (c.Suit != compareSuit) sameSuit = false;
            }

            GetRank(temp, sameSuit);
            FillValues(temp);
        }

        private void FillCards(string hand)
        {
            foreach (var val in hand.Split(' '))
            {
                var parts = val.ToCharArray();
                cards.Add(new Card(ValueToNumber[parts[0]], CharToSuit[parts[1]]));
            }
        }

        private void GetRank(Dictionary<int, int> temp, bool sameSuit)
        {
            var straight = FindStraight(temp);
            var profile = temp.Values.ToList();
            profile.Sort();
            profile.Reverse();
            var result = string.Join("", profile.Select(p => p.ToString()).ToArray());
            var keys = temp.Keys.ToList();
            keys.Sort();
            switch (result)
            {
                case "11111":
                    rank = Ranks.HighCard;
                    if (sameSuit) rank = Ranks.Flush;
                    if (straight && rank != Ranks.Flush) rank = Ranks.Straight;
                    if (straight && sameSuit)
                    {
                        rank = keys[0] == 10 ? Ranks.RoyalFlush : Ranks.StraightFlush;
                    }
                    break;
                case "2111":
                    rank = Ranks.Pair;
                    break;
                case "221":
                    rank = Ranks.TwoPairs;
                    break;
                case "311":
                    rank = Ranks.ThreeOfAKind;
                    break;
                case "32":
                    rank = Ranks.FullHouse;
                    break;
                case "41":
                    rank = Ranks.FourOfAKind;
                    break;
                default:
                    rank = Ranks.Error;
                    break;
            }
        }

        private void FillValues(Dictionary<int, int> temp)
        {
            var vals = temp.Keys.ToList();
            vals.Sort();
            vals.Reverse();
            switch (rank)
            {
                case Ranks.HighCard:
                case Ranks.Flush:
                    values.AddRange(vals);
                    break;
                case Ranks.Pair:
                case Ranks.ThreeOfAKind:
                case Ranks.FourOfAKind:
                    foreach (var val in vals) if (temp[val] != 1) values.Add(val);
                    foreach (var val in vals) if (temp[val] == 1) values.Add(val);
                    break;
                case Ranks.Straight:
                case Ranks.StraightFlush:
                case Ranks.RoyalFlush:
                    values.Add(vals[0]);
                    break;
                case Ranks.FullHouse:
                    foreach (var val in vals) if (temp[val] == 3) values.Add(val);
                    foreach (var val in vals) if (temp[val] == 2) values.Add(val);
                    break;
                case Ranks.TwoPairs:
                    foreach (var val in vals) if (temp[val] == 2) values.Add(val);
                    values.Sort();
                    values.Reverse();
                    foreach (var val in vals) if (temp[val] == 1) values.Add(val);
                    break;
            }
        }

        private static bool FindStraight(Dictionary<int, int> temp)
        {
            var keys = temp.Keys.ToList();
            if (keys.Contains(2) && keys.Contains(14))
            {
                keys.Remove(14);
                keys.Add(1);
            }
            keys.Sort();
            var first = keys[0];
            for (int ix = 1; ix < 5; ix++)
            {
                if (keys[ix] != first + 1)
                {
                    return false;
                }
                first = keys[ix];
            }
            return true;
        }

        public Ranks RankValue() => rank;

        public Results GetResult(Hand other)
        {
            if (this.rank > other.rank) return Results.Win;
            if (this.rank < other.rank) return Results.Loss;
            var ix = 0;
            while(ix < values.Count)
            {
                if (this.values[ix] > other.values[ix]) return Results.Win;
                else if (this.values[ix] < other.values[ix]) return Results.Loss;
                ix++;
            }
            return Results.Draw;
        }
    }
    public class Card
    {
        public int Value { get; set; }
        public Suits Suit { get; set; }

        public Card(int value, Suits suit)
        {
            Value = value;
            Suit = suit;
        }
    }
    public enum Suits { Hearts, Clubs, Diamonds, Spades }
    public enum Ranks { Error = -1,
                        NotSet = 0 ,
                        HighCard = 1,
                        Pair = 2,
                        TwoPairs = 3,
                        ThreeOfAKind = 4,
                        Straight = 5,
                        Flush = 6,
                        FullHouse = 7,
                        FourOfAKind = 8,
                        StraightFlush = 9,
                        RoyalFlush = 10 }
    public enum Results { Win, Loss, Draw }
}
