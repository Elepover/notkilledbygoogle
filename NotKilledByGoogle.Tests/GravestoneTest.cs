using System;
using System.Collections.Generic;
using NotKilledByGoogle.Bot.Grave;
using Xunit;

namespace NotKilledByGoogle.Tests
{
    public class GravestoneTest
    {
        private static IEnumerable<Gravestone?[]> EqualGravestones()
        {
            yield return new Gravestone[]
            {
                new() { Name = "Google Scuba Diver", Description = "You should have written it right.", DeceasedType = DeceasedType.App },
                new() { Name = "Google Scuba Diver", Description = "Yes I have now.", DeceasedType = DeceasedType.Service }
            };
            yield return new Gravestone[]
            {
                new() { Name = "Google Nexus", Description = "Awesome smartphone product line killed for more revenue.", DateOpen = new (2000, 1, 1, 1, 1, 1, TimeSpan.Zero) },
                new() { Name = "Google Nexus", Description = "Awesome smartphone product line killed for better user experience.", DateOpen = new (2001, 2, 2, 2, 2, 2, TimeSpan.Zero) }
            };
            yield return new Gravestone[]
            {
                new() { Name = "Project Soli", Description = "They will come back one day.", DateClose = new (2020, 8, 6, 0, 0, 0, TimeSpan.Zero) },
                new() { Name = "Project Soli", Description = "Just not that soon.", DateClose = new (2020, 10, 15, 0, 0, 0, TimeSpan.Zero) }
            };
            yield return new Gravestone[]
            {
                new() { Name = "URL changed", ReferenceLink = "https://www.google.com" },
                new() { Name = "URL changed", ReferenceLink = "https://www.google.com/search?q=killedbygoogle.com"}
            };
            yield return new Gravestone?[]
            {
                null,
                null
            };
        }

        private static IEnumerable<Gravestone?[]> UnequalGravestones()
        {
            yield return new Gravestone[]
            {
                new() { Name = "Awesome product 1", Description = "IT IS AWESOME" },
                new() { Name = "Awesome product 2", Description = "IT IS AWESOME" }
            };
            yield return new Gravestone[]
            {
                new() { Name = "Luke", Description = "Imbecile" },
                new() { Name = "Material Design 2", Description = "Imbecile" }
            };
            yield return new Gravestone?[]
            {
                new() { Name = "Luke", Description = "Imbecile" },
                null
            };
            yield return new Gravestone?[]
            {
                null,
                new() { Name = "Google+", Description = "Google, plus, something equals to..." }
            };
        }
        
        [Theory]
        [MemberData(nameof(EqualGravestones))]
        public void TestEquality(Gravestone? stone1, Gravestone? stone2)
        {
            Assert.True(new GravestoneEqualityComparer().Equals(stone1, stone2));
        }

        [Theory]
        [MemberData(nameof(UnequalGravestones))]
        public void TestInequality(Gravestone? stone1, Gravestone? stone2)
        {
            Assert.False(new GravestoneEqualityComparer().Equals(stone1, stone2));
        }
    }
}
