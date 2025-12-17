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
            _httpClient = new HttpClient { BaseAddress = new Uri("https://fb41pxk9-7206.use.devtunnels.ms/") };
        }

        public async Task<ApiResult<T>> PostAsync<T> ( string endpoint, object payload )
        {
            var response = await _httpClient.PostAsJsonAsync(endpoint, payload);
            var status = response.StatusCode;

            if (response.IsSuccessStatusCode)
            {
                var data = await response.Content.ReadFromJsonAsync<T>();
                return ApiResult<T>.Success(data!, status);
            }

            var error = await response.Content.ReadAsStringAsync();
            if (string.IsNullOrWhiteSpace(error))  error = response.StatusCode.ToString();
            return ApiResult<T>.Failure(status, error);
        }
    }
}
