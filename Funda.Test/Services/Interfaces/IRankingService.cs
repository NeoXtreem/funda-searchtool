using System.Collections.Generic;
using Funda.Test.Models;

namespace Funda.Test.Services.Interfaces
{
    public interface IRankingService
    {
        IEnumerable<(string name, int count)> GetTopTenMakelaars(IEnumerable<PropertyListing> listings);
    }
}
