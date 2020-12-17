using System;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.GraveKeeper;
using Xunit;

namespace NotKilledByGoogle.Tests
{
    public class DeserializationTest
    {
        [Theory]
        [InlineData("{\"dateClose\":\"2020-06-24\",\"dateOpen\":\"2018-09-01\",\"description\":\"An app born to be killed by Google.\",\"link\":\"https://killedbygoogle.com\",\"name\":\"Awesome App\",\"type\":\"app\"}")]
        [InlineData("{\"dataClose\":\"2020-10-22\",\"dateOpen\":\"2011-11-16\",\"description\":\"A service born to be killed by Google.\",\"link\":\"https://killedbygoogle.com\",\"name\":\"Awesome Service\",\"type\":\"service\"}")]
        [InlineData("{\"dataClose\":\"2020-12-14\",\"dateOpen\":\"2017-12-11\",\"description\":\"A hardware born to be killed by Google.\",\"link\":\"https://killedbygoogle.com\",\"name\":\"Awesome Hardware\",\"type\":\"hardware\"}")]
        public Task CorrectGravestone(string raw) => Gravestone.ParseAsync(raw);

        [Theory]
        [InlineData("{\"dataClose\":\"2020-13-14\",\"dateOpen\":\"2017-12-11\",\"description\":\"\",\"link\":\"https://killedbygoogle.com\",\"name\":\"\",\"type\":\"app\"}")]
        [InlineData("{\"dataClose\":\"2020-12-14\",\"dateOpen\":\"2017-12-32\",\"description\":\"\",\"link\":\"https://killedbygoogle.com\",\"name\":\"\",\"type\":\"app\"}")]
        [InlineData("{\"dataClose\":\"2020-12-14\",\"dateOpen\":\"2017/12/31\",\"description\":\"\",\"link\":\"https://killedbygoogle.com\",\"name\":\"\",\"type\":\"app\"}")]
        [InlineData("{\"dataClose\":\"2020-12-14\",\"dateOpen\":\"2017 12 31\",\"description\":\"\",\"link\":\"https://killedbygoogle.com\",\"name\":\"\",\"type\":\"app\"}")]
        public void CorruptDate(string raw) => Assert.ThrowsAnyAsync<Exception>(() => Gravestone.ParseAsync(raw));

        [Theory]
        [InlineData("{\"dataClose\":\"2020-11-10\",\"dateOpen\":\"2018-10-05\",\"description\":\"We're not sure this time but Google surely killed something.\",\"link\":\"https://killedbygoogle.com\",\"name\":\"Mysterious Object\",\"type\":\"magic\"}")]
        [InlineData("{\"dataClose\":\"2020-11-10\",\"dateOpen\":\"2018-10-05\",\"description\":\"A very useful search engine.\",\"link\":\"https://killedbygoogle.com\",\"name\":\"Google\",\"type\":\"cash-source\"}")]
        public async Task UnknownDeceasedType(string raw)
        {
            var gravestone = await Gravestone.ParseAsync(raw);
            Assert.Equal(DeceasedType.Other, gravestone.DeceasedType);
        }
    }
}
