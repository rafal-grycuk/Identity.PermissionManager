using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Identity.PermissionManager.Api.Tests
{
    public class ApiConnector
    {
        public enum ContentType
        {
            TextContent,
            JsonContent,
            XmlContent
        }

        public async Task<T> Request<T>(string url, HttpMethod method, object dataObject = null,
            ContentType? contentType = null, string authToken = null)
        {
            try
            {
                HttpRequestMessage message = new HttpRequestMessage(method, url);
                var httpClient = new HttpClient();
                if (authToken != null)
                    httpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + authToken);
                if (dataObject != null)
                {
                    if (contentType.HasValue && contentType.Value == ContentType.JsonContent)
                    {
                        httpClient.DefaultRequestHeaders.Accept.Add(
                            new MediaTypeWithQualityHeaderValue("application/json"));
                        string jsonObject = Newtonsoft.Json.JsonConvert.SerializeObject(dataObject);
                        message.Content = new StringContent(jsonObject, Encoding.UTF8, "application/json");
                    }
                    else if (contentType.HasValue && contentType.Value == ContentType.TextContent)
                    {
                        httpClient.DefaultRequestHeaders.Accept
                            .Add(new MediaTypeWithQualityHeaderValue("application/text"));
                        message.Content = new StringContent(dataObject.ToString(), Encoding.UTF8, "application/text");
                    }
                    else if (contentType.HasValue && contentType.Value == ContentType.XmlContent)
                    {
                        throw new NotImplementedException("xml content type was not implemented yet :)");
                    }
                }
                var response = await httpClient.SendAsync(message);
                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var responseObject = Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
                    return responseObject;
                }
                else
                    throw new Exception("Response code is not 200, therefore serialization cannot be completed.");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
