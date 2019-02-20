using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Poker
{
    public class TestHands
    {
        [Fact]
        public void Test()
        {
            var mock = new Hand("KS 2D 3H 4C TD");
            Xunit.Assert.Equal(13 + 2 + 3 + 4 + 10, mock.Total());
            Xunit.Assert.Equal(Ranks.HighCard, mock.RankValue());
        }

        [Theory]
        [InlineData("AC 4H 7D KC 2S", Ranks.HighCard)]
        [InlineData("KC KH 7D 2C 5S", Ranks.Pair)]
        [InlineData("KC KH 7D 7C 5S", Ranks.TwoPairs)]
        [InlineData("KC KH KD 7C 5S", Ranks.ThreeOfAKind)]
        [InlineData("3C 4H 5D 6C 7S", Ranks.Straight)]
        [InlineData("2C 3H 4D 5C AS", Ranks.Straight)]
        [InlineData("KC QC 9C 8C 2C", Ranks.Flush)]
        [InlineData("KC KH KD 7C 7S", Ranks.FullHouse)]
        [InlineData("KC KH KD KS 5S", Ranks.FourOfAKind)]
        [InlineData("3C 4C 5C 6C 7C", Ranks.StraightFlush)]
        [InlineData("TH JH QH KH AH", Ranks.RoyalFlush)]
        public void TestRanks(string hand, Ranks rank)
        {
            var mock = new Hand(hand);
            Assert.Equal(rank,mock.RankValue());
        }
    }
}
