using System.Collections.Generic;
using System.Linq;
using Funda.Test.Models;
using Funda.Test.Services.Interfaces;

namespace Funda.Test.Services
{
    public class RankingService : IRankingService
    {
        public IEnumerable<(string, int)> GetTopTenMakelaars(IEnumerable<PropertyListing> listings)
        {
            return listings
                .GroupBy(l => l.MakelaarNaam)
                .OrderByDescending(g => g.Count())
                .Take(10)
                .Select(g => (g.Key, g.Count()));
        }
    }
}
