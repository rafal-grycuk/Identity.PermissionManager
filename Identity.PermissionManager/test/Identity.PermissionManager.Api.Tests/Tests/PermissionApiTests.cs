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
            this._token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJqbm93YWtAdGVzdC50ZXN0IiwianRpIjoiZjAyZGI0ZjQtY2UzMC00NWNjLTg3MjItYThlZTBhYWI1ZDZlIiwiaWF0IjoxNDg4MDMwMTgwLCJJZGVudGl0eVVzZXIiOiJVc2VyIiwibmJmIjoxNDg4MDMwMTgwLCJleHAiOjE0OTcwMzAxODAsImlzcyI6IlN1cGVyQXdlc29tZVRva2VuU2VydmVyIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDozNDIzLyJ9.afnnibWkpJhniqMP-wjBHl232ODe4CJ4vA0ssrZU58s";
        }

        [Fact]
        public  async Task GetPermissionById()
        {
            await _apiConnector.Request<object>(this._url + "2", HttpMethod.Get, null, null, this._token);
        }
    }
}
