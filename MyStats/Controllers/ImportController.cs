using System.Collections.Generic;
using System.Configuration;
using System.Web.Http;
using MyStats.Core;

namespace MyStats.Controllers
{
    [RoutePrefix("import")]
    public class ImportController : ApiController
    {
        [HttpPost, Route("")]
        public IEnumerable<Draw> Import()
        {
            var importer = new Importer();
            return importer.ImportData(ConfigurationManager.AppSettings["filePath"]);
        }
    }
}