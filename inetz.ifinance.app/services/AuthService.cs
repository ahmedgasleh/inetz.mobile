using inetz.ifinance.app.models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.services
{
    public class AuthService
    {
        private readonly ApiService _apiService;

        public AuthService ( ApiService apiService )
        {
            _apiService = apiService;
        }

        public async Task<AuthResponse?> RegisterAsync ( UserRegistration model )
        {
            var result = await _apiService.PostAsync<AuthResponse>("register", model);
            if (result?.Success == true)
            {
                await SecureStorage.SetAsync("PhoneNumber", model.PhoneNumber);
                await SecureStorage.SetAsync("BinNumber", result.Bin);
            }
            return result;
        }

        public async Task<AuthResponse?> LoginAsync ( LoginRequest request )
        {
            var result = await _apiService.PostAsync<AuthResponse>("login", request);
            if (result?.Success == true)
            {
                await SecureStorage.SetAsync("AuthToken", result.Token);
            }
            return result;
        }

        public async Task<bool> IsUserRegisteredAsync ()
        {
            var phone = await SecureStorage.GetAsync("PhoneNumber");
            return !string.IsNullOrEmpty(phone);
        }

        public async Task<bool> IsUserLoggedInAsync ()
        {
            var token = await SecureStorage.GetAsync("AuthToken");
            return !string.IsNullOrEmpty(token);
        }

        public async Task LogoutAsync ()
        {
            SecureStorage.Remove("AuthToken");
            // optionally clear BinNumber or user info
        }
    }
}
