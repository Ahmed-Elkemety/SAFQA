using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.RecommendationAI
{
    public class RecommendationAI : IRecommendationAI
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public RecommendationAI(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;
        }
        public async Task<List<RecommendationResult>> GetRecommendationsAsync(string userId)
        {
            try
            {
                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Bypass-Tunnel-Reminder", "true");

                string fullUrl = $"https://loud-ways-sin.loca.lt/recommend/{userId}";

                var response = await _httpClient.GetAsync(fullUrl);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    return JsonConvert.DeserializeObject<List<RecommendationResult>>(content);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AI API Error: {ex.Message}");
            }

            return new List<RecommendationResult>();
        }
    }
}
