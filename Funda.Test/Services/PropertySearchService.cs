using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using EnumsNET;
using Funda.Test.Exceptions;
using Funda.Test.Models;
using Funda.Test.Services.Interfaces;
using Funda.Test.Utilities;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Funda.Test.Services
{
    public class PropertySearchService : IPropertySearchService
    {
        private readonly PropertySearchOptions _options;
        private readonly IMapper _mapper;

        public PropertySearchService(IOptionsFactory<PropertySearchOptions> optionsFactory, IMapper mapper)
        {
            _options = optionsFactory.Create(Options.DefaultName);
            _mapper = mapper;
        }

        public async Task<IEnumerable<PropertyListing>> FindProperties(SearchCriteria searchCriteria, CancellationToken cancellationToken)
        {
            var searchParameters = new List<string>(new[] {string.IsNullOrWhiteSpace(searchCriteria.Place) ? _options.DefaultPlace : searchCriteria.Place});
            if (searchCriteria.Garden) searchParameters.Add("tuin");

            var pathSeparator = Path.AltDirectorySeparatorChar;
            var listings = new List<PropertyListing>();
            var page = 1;

            using var client = new HttpClient {BaseAddress = new Uri(_options.BaseUrl)};
            using var throttler = new Throttler(_options.ThrottleLimit, TimeSpan.FromMinutes(1));

            while (!cancellationToken.IsCancellationRequested)
            {
                throttler.Throttle(cancellationToken);

                using var response = await client.GetAsync(QueryHelpers.AddQueryString(_options.ApiKey,
                    new (string key, string value)[]
                    {
                        ("type", searchCriteria.SearchType.AsString(EnumFormat.Description)),
                        ("zo", $"{pathSeparator}{string.Join(pathSeparator, searchParameters)}{pathSeparator}"),
                        ("page", page++.ToString()),
                        ("pageSize", _options.PageSize.ToString())
                    }.ToDictionary(p => p.key, p => p.value)), cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    var searchResult = JsonConvert.DeserializeObject<SearchResult>(await response.Content.ReadAsStringAsync());

                    if (!searchResult.Objects.Any()) break;
                    listings.AddRange(_mapper.Map<IEnumerable<PropertyListing>>(searchResult.Objects));
                }
                else
                {
                    throw new HttpClientException($"Property search request unsuccessful: {response.ReasonPhrase}") { Response = response };
                }
            }

            return listings;
        }
    }
}
