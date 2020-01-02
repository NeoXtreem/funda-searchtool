using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Funda.Test.Models;

namespace Funda.Test.Services.Interfaces
{
    public interface IPropertySearchService
    {
        Task<IEnumerable<PropertyListing>> FindProperties(SearchCriteria searchCriteria, CancellationToken cancellationToken);
    }
}
