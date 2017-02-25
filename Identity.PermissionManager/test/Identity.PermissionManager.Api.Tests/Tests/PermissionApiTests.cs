using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Identity.PermissionManager.ViewModels.VMs;
using Xunit;

namespace Identity.PermissionManager.Api.Tests.Tests
{
    public class PermissionApiTests
    {
        private readonly ApiConnector _apiConnector;
        private string _token;
        private string _url;
        public PermissionApiTests()
        {
            this._apiConnector = new ApiConnector();
            this._url = "http://localhost:3423/api/Permission/";
            this._token = "";
        }

        [Fact]
        public void GetPermissionById()
        {
            var response = _apiConnector.Request<object>(this._url+"2", HttpMethod.Get, null, null, _token).Result;
        }
    }
}
