using System;
using System.Collections.Generic;
using System.Net;
using System.Web.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace truepele.ApiClient.Tests.WebApiMock
{
    [Microsoft.AspNetCore.Mvc.Route("api/values")]
    public class ValuesController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private static readonly int[] Values = {1, 3, 4, 7, 11, 15};
        private static int _counter = 0;

        public ValuesController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
        }
        
        // GET
        public ActionResult<IEnumerable<int>> Index()
        {
            if (_appSettings.ApiFailRatio > 0 && _counter++ % _appSettings.ApiFailRatio == 0)
            {
                throw new HttpResponseException(HttpStatusCode.InternalServerError);
            }

            return Values;
        }
    }
}