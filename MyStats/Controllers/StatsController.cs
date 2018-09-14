using System.Collections.Generic;
using System.Web.Http;
using MyStats.Core;
using MyStats.Core.Models;

namespace MyStats.Controllers
{
    [RoutePrefix("stats")]
    public class StatsController : ApiController
    {
        private readonly Repository _repository;

        public StatsController()
        {
            _repository = new Repository();
        }

        [HttpGet, Route("delays")]
        public IEnumerable<Delay> GetDelays()
        {
            return _repository.GetDelays();
        }

        [HttpGet, Route("{wheel}/delays")]
        public IEnumerable<Delay> GetDelays(Wheel wheel)
        {
            return _repository.GetDelays(wheel);
        }

        [HttpGet, Route("frequencies")]
        public IEnumerable<Frequence> GetFrequencies(int? takeLastDraws)
        {
            return _repository.GetFrequencies(takeLastDraws);
        }

        [HttpGet, Route("{wheel}/frequencies")]
        public IEnumerable<Frequence> GetFrequencies(Wheel wheel, int? takeLastDraws)
        {
            return _repository.GetFrequencies(wheel, takeLastDraws);
        }
    }
}
