using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SAFQA.BLL.Dtos.AIDtos;
using SAFQA.BLL.Managers.AccountManager.Auth;
using SAFQA.DAL.Models;

namespace SAFQA.BLL.Managers.RecommendationAI
{
    public class RecommendationAI : IRecommendationAI
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public RecommendationAI(HttpClient httpClient, IConfiguration configuration , UserManager<User> userManager)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _userManager = userManager;
        }
        public async Task<List<RecommendationDto>> GetRecommendations(string userId, int n = 10)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var url = $"https://celtic-ira-consists-freelance.trycloudflare.com/recommend/{userId}?n={n}";

            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
                return new List<RecommendationDto>();

            var json = await response.Content.ReadAsStringAsync();

            var result = JsonConvert.DeserializeObject<RecommendationResponse>(json);

            return result?.Recommendations ?? new List<RecommendationDto>();
        }
    }
}
