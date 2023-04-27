using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text;
using System.Text.Json;

namespace SAP_Transfer.Controllers.api.v1
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class SapLoginController : ControllerBase
    {
        public class LoginModel
        {
            public string? CompanyDB { get; set; }
            public string? UserName { get; set; }
            public string? Password { get; set; }
            public string? SAPUsername { get; set; }
            public string? SAPPassword { get; set; }
        }

        // POST api/v1/<SapLoginController>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LoginModel value)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => true
            };
            var client = new HttpClient(handler);
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(new
            {
                CompanyDB = "Homart_TEST8",
                UserName = "m2",
                Password = "Hope&"
            }), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://192.168.0.44:50000/b1s/v1/Login", content);
            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseContent);
            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.IsSuccessStatusCode);
                var jsonDoc = JsonDocument.Parse(responseContent);
                if (jsonDoc.RootElement.TryGetProperty("SessionId", out var sessionId))
                {
                    client.DefaultRequestHeaders.Add("Cookie", $"B1SESSION={sessionId.GetString()}");
                    var getResponse = await client.GetAsync($"https://192.168.0.44:50000/b1s/v1/view.svc/Homart_CheckUserAccount_B1SLQuery()?$filter=U_UserCode eq '{value.SAPUsername}' and U_UserPW eq '{value.SAPPassword}'");
                    var getResponseContent = await getResponse.Content.ReadAsStringAsync();
                    var getJsonDoc = JsonDocument.Parse(getResponseContent);
                    using (var stream = new MemoryStream())
                    {
                        using (var writer = new Utf8JsonWriter(stream))
                        {
                            writer.WriteStartObject();
                            writer.WriteString("SessionId", sessionId.GetString());
                            foreach (var element in getJsonDoc.RootElement.EnumerateObject())
                            {
                                element.WriteTo(writer);
                            }
                            writer.WriteEndObject();
                        }
                        return Ok(Encoding.UTF8.GetString(stream.ToArray()));
                    }
                }
                else
                {
                    return BadRequest("SessionId not found in response body.");
                }
            }
            else
            {
                var logoutResponse = await client.PostAsync("https://192.168.0.44:50000/b1s/v1/Logout", null);
                if (logoutResponse.StatusCode == HttpStatusCode.NoContent)
                {
                    return StatusCode((int)response.StatusCode, responseContent);
                }
                else
                {
                    return StatusCode((int)logoutResponse.StatusCode, await logoutResponse.Content.ReadAsStringAsync());
                }
            }
        }
    }
}