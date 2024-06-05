using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using Amazon.DynamoDBv2.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using WebApplication1.Modles;

namespace WebApplication1.Controllers
{   
    [ApiController]
    [Route("[controller]")]
    public class HotelController : ControllerBase
    {
        readonly string _key = "1";
        private readonly IAmazonDynamoDB _dynamoDbClient;
        private readonly DynamoDBContext _context;

        public HotelController()
        {
            var config = new AmazonDynamoDBConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.EUNorth1
            };

            _dynamoDbClient = new AmazonDynamoDBClient("2", " 2", config);
            _context = new DynamoDBContext(_dynamoDbClient);
        }

        [HttpGet("sea" +
            "rch")]
        public async Task<IActionResult> GetSearchHotel([FromQuery] string keyword)
        {
            using (var client = new HttpClient())
            {
                string url = $"https://api.content.tripadvisor.com/api/v1/location/search?key={_key}&searchQuery={keyword}&language=en";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();
                var hotelData = JsonConvert.DeserializeObject<Root>(json);

                string jsonResponse = JsonConvert.SerializeObject(hotelData);
                var content = new ContentResult
                {
                    Content = jsonResponse,
                    ContentType = "application/json",
                    StatusCode = 200
                };

                return content;
            }
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetLocationDetails([FromQuery] string locationId)
        {
            using (var client = new HttpClient())
            {
                string url = $"https://api.content.tripadvisor.com/api/v1/location/{locationId}/details?key={_key}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                return Content(json, "application/json");
            }
        }

        [HttpGet("photos")]
        public async Task<IActionResult> GetLocationPhotos([FromQuery] string locationId)
        {
            using (var client = new HttpClient())
            {
                string url = $"https://api.content.tripadvisor.com/api/v1/location/{locationId}/photos?key={_key}";
                var response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                string json = await response.Content.ReadAsStringAsync();

                return Content(json, "application/json");
            }
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddHotel([FromBody] Hotel2 hotel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            await _context.SaveAsync(hotel);
            return Ok(new { Message = "Hotel added successfully" });
        }

        [HttpGet("get")]
        public async Task<IActionResult> GetHotel([FromQuery] string hotelName)
        {
            var hotel = await _context.LoadAsync<Hotel2>(hotelName);
            if (hotel == null)
            {
                return NotFound();
            }
            return Ok(hotel);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> DeleteHotel([FromQuery] string hotelName)
        {
            var hotel = await _context.LoadAsync<Hotel2>(hotelName);
            if (hotel == null)
            {
                return NotFound();
            }

            await _context.DeleteAsync(hotel);
            return Ok(new { Message = "Hotel deleted successfully" });
        }
    }

    [DynamoDBTable("Travelgo")]
    public class Hotel2
    {
        [DynamoDBHashKey]
        [Required(ErrorMessage = "HotelName is required")]
        public string Hotel { get; set; }

        [DynamoDBProperty]
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; }

        [DynamoDBProperty]
        [Required(ErrorMessage = "LocationId is required")]
        public string LocationId { get; set; }
    }
}












