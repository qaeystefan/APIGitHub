using System.Net;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;

namespace ApiTests
{
    public class GitHubApiTests
    {
        private HttpClient _client;
        private string _baseUrl;
        private string _postEndpoint;
        private string _getEndpoint;
        private string _updateEndpoint;
        private string _deleteEndpoint;
        private string _token;
        private string _userAgent;

        [SetUp]
        public void Setup()
        {
            var config = JObject.Parse(File.ReadAllText("config.json"));
            _baseUrl = config["BaseUrl"].ToString();
            _postEndpoint = config["PostEndpoint"].ToString();
            _getEndpoint = config["GetEndpoint"].ToString();
            _updateEndpoint = config["UpdateEndpoint"].ToString();
            _deleteEndpoint = config["DeleteEndpoint"].ToString();
            _token = config["Token"].ToString();
            _userAgent = config["UserAgent"].ToString();

            _client = new HttpClient
            {
                BaseAddress = new Uri(_baseUrl)
            };
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            _client.DefaultRequestHeaders.UserAgent.ParseAdd(_userAgent);
        }

        [Test]
        public async Task TestPost()
        {
            var postBody = File.ReadAllText("Json/postBody.json");
            var content = new StringContent(postBody, System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PostAsync(_postEndpoint, content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(response.StatusCode == HttpStatusCode.Created);
        }

        [Test]
        public async Task TestGet()
        {
            var response = await _client.GetAsync(_getEndpoint);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task TestUpdate()
        {
            var updateBody = File.ReadAllText("Json/updateBody.json");
            var content = new StringContent(updateBody, System.Text.Encoding.UTF8, "application/json");
            var response = await _client.PatchAsync(_updateEndpoint, content);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(response.StatusCode == HttpStatusCode.OK);
        }

        [Test]
        public async Task TestDelete()
        {
            var response = await _client.DeleteAsync(_deleteEndpoint);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(response.StatusCode == HttpStatusCode.NoContent);
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
        }
    }
}
