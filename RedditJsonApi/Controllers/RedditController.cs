using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace RedditJsonApi.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    public class RedditController : ControllerBase
    {

        private static List<HistoryEntry>? History 
        {
            // ta w³aœciwoœæ to "baza danych" do historii - uzna³em, ¿e rozwi¹zanie z prawdziw¹ baz¹ danych zamiast pliku to overkill
            get
            {
                return HistoryProvider.GetHistory();
            }
        }

        private readonly Random _rand = new Random();
        private readonly IConfiguration _configuration;
        private readonly ILogger<RedditController> _logger;
        private readonly HttpClient _client;

        public RedditController(IConfiguration configuration, ILogger<RedditController> logger)
        {
            _configuration = configuration;
            _logger = logger;

            // dodatkowo inicjalizacja klienta HTTP do pobierania obrazków z Reddita
            _client = new HttpClient();
            _client.DefaultRequestHeaders.Accept.Clear();
            _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        /// <summary>
        /// Searches the subreddit for images and returns a random one.
        /// </summary>
        /// <returns>A random image url.</returns>
        [Produces("application/json")]
        [HttpGet("random")]
        public async Task<IActionResult> GetRandomImage()
        {
            string result;
            List<string> images = new List<string>();

            string path = $"https://www.reddit.com/r/{_configuration["Subreddit"]}.json";

            // uzna³em, ¿e lepiej robiæ zapytanie do Reddita za ka¿dym u¿yciem tego endpointa, bo zawartoœæ subreddita mo¿e siê ca³kiem czêsto zmieniaæ
            using (HttpResponseMessage response = await _client.GetAsync(path))
            {
                // ³uskanie url obrazków z tego co zwróci³o nam zapytanie do reddita
                string data = await response.Content.ReadAsStringAsync();
                var resultObj = JObject.Parse(data);
                var str = Convert.ToString(resultObj?["data"]?["children"]);
                JArray obj = JArray.Parse(str);
                foreach (var item in obj)
                {
                    var url = item?["data"]?["url_overridden_by_dest"];
                    if (url != null) images.Add(url.ToString());
                }
            }
            // jeœli nie znaleŸliœmy ¿adnego obrazka:
            if(images.Count == 0) return StatusCode((int)System.Net.HttpStatusCode.NotFound, "It seems there are no pictures.");

            // wybieranie losowego i dodawanie do historii
            result = images[_rand.Next(images.Count)];
            History?.Add(new HistoryEntry
            {
                Date = DateTime.Now,
                Image = result,
            });

            HistoryProvider.Save();
            return Ok(result);
        }

        /// <summary>
        /// Endpoint for history of getting random image.
        /// </summary>
        /// <returns>History of using /random endpoint.</returns>
        [Produces("application/json")]
        [HttpGet("history")]
        public IActionResult GetHistory()
        {
            if(History == null)
            {
                return StatusCode((int)System.Net.HttpStatusCode.InternalServerError, "History file is missing");
            }
            return Ok(History);
        }
    }
}