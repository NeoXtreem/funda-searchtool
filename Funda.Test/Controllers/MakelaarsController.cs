using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Funda.Test.Exceptions;
using Funda.Test.Models;
using Funda.Test.Services.Interfaces;
using Funda.Test.Types;
using Funda.Test.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Funda.Test.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MakelaarsController : ControllerBase
    {
        private readonly ILogger<MakelaarsController> _logger;
        private readonly IPropertySearchService _propertySearchService;
        private readonly IRankingService _rankingService;
        private CancellationTokenSource _cts;

        public MakelaarsController(ILogger<MakelaarsController> logger, IPropertySearchService propertySearchService, IRankingService rankingService)
        {
            _logger = logger;
            _propertySearchService = propertySearchService;
            _rankingService = rankingService;
        }

        [HttpGet("[action]/{place}/{garden}")]
        public async Task<ActionResult<IEnumerable<MakelaarViewModel>>> GetTopTen(string place, bool garden)
        {
            _cts?.Cancel();
            _cts = new CancellationTokenSource();

            try
            {
                var listings = await _propertySearchService.FindProperties(new SearchCriteria
                {
                    SearchType = SearchType.Buy,
                    Place = place,
                    Garden = garden
                }, _cts.Token);

                return Ok(_rankingService.GetTopTenMakelaars(listings).Select(x => new MakelaarViewModel {Name = x.name, Count = x.count}));
            }
            catch (HttpClientException e)
            {
                _logger.LogError(e, "Error occurred in the property search service during response processing while finding properties.");
                return BadRequest(e);
            }
            catch (HttpRequestException e)
            {
                _logger.LogError(e, "Error occurred in the property search service during an HTTP request to find properties.");
                return BadRequest(e);
            }
        }
    }
}
