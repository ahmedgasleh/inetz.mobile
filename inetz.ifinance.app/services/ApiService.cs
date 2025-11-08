using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService ()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("https://10.0.2.2:7206/") };
        }

        public async Task<T?> PostAsync<T> ( string endpoint, object payload )
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<T>();
            return default;
        }
    }
}
