using System;
using System.Collections.Generic;
using System.Web.Http;
using MyStats.Core;

namespace MyStats.Controllers
{
    [RoutePrefix("draws")]
    public class DrawsController : ApiController
    {
        private readonly Repository _repository;

        public DrawsController()
        {
                _repository = new Repository();
        }

        [HttpGet, Route("")]
        public IEnumerable<Draw> Get(int? takeLastDraws)
        {
            return _repository.Get(takeLastDraws);
        }

        [HttpGet, Route("")]
        public Draw Get(DateTime date)
        {
            return _repository.Get(date);
        }
    }
}
