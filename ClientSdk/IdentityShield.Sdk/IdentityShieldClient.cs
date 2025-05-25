using IdentityShield.Contracts.AccountLinking;
using IdentityShield.Contracts.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IdentityShield.Sdk
{
    public sealed class IdentityShieldClient
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;
        private string _accessToken;

        public IdentityShieldClient(string baseUrl)
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = new Uri(baseUrl);

            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        // Set auth token for subsequent requests
        public void SetToken(string accessToken)
        {
            _accessToken = accessToken;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
        }

        // Clear the auth token
        public void ClearToken()
        {
            _accessToken = null;
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }

        #region Authentication APIs

        // Login with email
        public async Task<JwtResponse> LoginByEmailAsync(string email, string password)
        {
            return await PostAsync<JwtResponse>("/Shield/Authentication/Login/Email", new
            {
                email,
                password
            });
        }

        // Login with phone number
        public async Task<JwtResponse> LoginByPhoneNumberAsync(string phoneNumber, string password)
        {
            return await PostAsync<JwtResponse>("/Shield/Authentication/Login/PhoneNumber", new
            {
                phoneNumber,
                password
            });
        }

        // Login with provider
        public async Task<JwtResponse> LoginByProviderAsync(string providerName, string providerValue)
        {
            return await PostAsync<JwtResponse>("/Shield/Authentication/Login/Provider", new
            {
                providerName,
                providerValue
            });
        }

        // Login as managed account
        public async Task<JwtResponse> LoginAsManagedAccountAsync(string managedAccountId)
        {
            return await PostAsync<JwtResponse>("/Shield/Authentication/Login/ManagedAccount", new
            {
                managedAccountId
            });
        }

        // Register with email
        public async Task<object> RegisterByEmailAsync(string email, string password)
        {
            return await PostAsync<object>("/Shield/Authentication/Register/Email", new
            {
                email,
                password
            });
        }

        // Confirm email registration
        public async Task<JwtResponse> ConfirmEmailRegistrationAsync(string email, string token)
        {
            return await PostAsync<JwtResponse>("/Shield/Authentication/Register/Confirm/Email", new
            {
                email,
                token
            });
        }

        // Register with phone number
        public async Task<object> RegisterByPhoneNumberAsync(string phoneNumber, string password)
        {
            return await PostAsync<object>("/Shield/Authentication/Register/PhoneNumber", new
            {
                phoneNumber,
                password
            });
        }

        // Confirm phone number registration
        public async Task<JwtResponse> ConfirmPhoneNumberRegistrationAsync(string phoneNumber, string token)
        {
            return await PostAsync<JwtResponse>("/Shield/Authentication/Register/Confirm/PhoneNumber", new
            {
                phoneNumber,
                token
            });
        }

        // Register with provider
        public async Task<JwtResponse> RegisterByProviderAsync(string providerName, string providerValue)
        {
            return await PostAsync<JwtResponse>("/Shield/Authentication/Register/Provider", new
            {
                providerName,
                providerValue
            });
        }

        // Logout
        public async Task<bool> LogoutAsync()
        {
            return await PostAsync<bool>("/Shield/Authentication/Logout", new { });
        }

        // Request enforce logout by email
        public async Task<bool> RequestEnforceLogoutByEmailAsync(string email)
        {
            return await PostAsync<bool>("/Shield/Authentication/Logout/Request/Email", new
            {
                email
            });
        }

        // Request enforce logout by phone number
        public async Task<bool> RequestEnforceLogoutByPhoneNumberAsync(string phoneNumber)
        {
            return await PostAsync<bool>("/Shield/Authentication/Logout/Request/PhoneNumber", new
            {
                phoneNumber
            });
        }

        // Confirm enforce logout
        public async Task<bool> ConfirmEnforceLogoutAsync(string token)
        {
            return await PostAsync<bool>("/Shield/Authentication/Logout/Confirm", new
            {
                token
            });
        }

        #endregion

        #region Account Linking APIs

        // Create managed account
        public async Task<CreateManagedAccount.Response> CreateManagedAccountAsync(CreateManagedAccount.Request request, CancellationToken cancellationToken)
        {
            return await PostAsync<CreateManagedAccount.Response>("/Shield/AccountLinking/Create", request, cancellationToken);
        }

        // Link managed account
        public async Task<LinkManagedAccount.Response> LinkManagedAccountAsync(LinkManagedAccount.Request request, CancellationToken cancellationToken)
        {
            return await PostAsync<LinkManagedAccount.Response>("/Shield/AccountLinking/Link", request, cancellationToken);
        }

        // Unlink managed account
        public async Task<UnlinkManagedAccount.Response> UnlinkManagedAccountAsync(UnlinkManagedAccount.Request request, CancellationToken cancellationToken)
        {
            return await DeleteAsync<UnlinkManagedAccount.Response>("/Shield/AccountLinking/Unlink", request, cancellationToken);
        }

        // Get all managed accounts
        public async Task<GetManagedByAccounts.Response> GetManagerAccountsAsync(GetManagerAccounts.Request request, CancellationToken cancellationToken)
        {
            return await GetAsync<GetManagedByAccounts.Response>($"/Shield/AccountLinking/Manager/{request.ManagerId}", cancellationToken);
        }

        // Get all accounts managing this account
        public async Task<GetManagedByAccounts.Response> GetManagedByAccountsAsync(GetManagedByAccounts.Request request, CancellationToken cancellationToken)
        {
            return await GetAsync<GetManagedByAccounts.Response>($"/Shield/AccountLinking/Managed/{request.ManagedId}", cancellationToken);
        }

        #endregion

        #region Token Management APIs

        // Refresh token
        public async Task<JwtResponse> RefreshTokenAsync(string refreshToken)
        {
            return await PostAsync<JwtResponse>("/Shield/Token/Refresh", new
            {
                refreshToken
            });
        }

        #endregion

        #region Email Management APIs

        // Request email change
        public async Task<bool> RequestEmailChangeAsync(string newEmail)
        {
            return await PostAsync<bool>("/Shield/Email/Change/Request", new
            {
                newEmail
            });
        }

        // Confirm email change
        public async Task<bool> ConfirmEmailChangeAsync(string token)
        {
            return await PostAsync<bool>("/Shield/Email/Change/Confirm", new
            {
                token
            });
        }

        #endregion

        #region Phone Number Management APIs

        // Request phone number change
        public async Task<bool> RequestPhoneNumberChangeAsync(string newPhoneNumber)
        {
            return await PostAsync<bool>("/Shield/PhoneNumber/Change/Request", new
            {
                newPhoneNumber
            });
        }

        // Confirm phone number change
        public async Task<bool> ConfirmPhoneNumberChangeAsync(string token)
        {
            return await PostAsync<bool>("/Shield/PhoneNumber/Change/Confirm", new
            {
                token
            });
        }

        #endregion

        #region Password Management APIs

        // Forgot password by email
        public async Task<bool> ForgotPasswordByEmailAsync(string email)
        {
            return await PostAsync<bool>("/Shield/Password/Forgot/Email", new
            {
                email
            });
        }

        // Forgot password by phone number
        public async Task<bool> ForgotPasswordByPhoneNumberAsync(string phoneNumber)
        {
            return await PostAsync<bool>("/Shield/Password/Forgot/PhoneNumber", new
            {
                phoneNumber
            });
        }

        // Reset password by email
        public async Task<bool> ResetPasswordByEmailAsync(string email, string token, string newPassword)
        {
            return await PostAsync<bool>("/Shield/Password/Reset/Email", new
            {
                email,
                token,
                newPassword
            });
        }

        // Reset password by phone number
        public async Task<bool> ResetPasswordByPhoneNumberAsync(string phoneNumber, string token, string newPassword)
        {
            return await PostAsync<bool>("/Shield/Password/Reset/PhoneNumber", new
            {
                phoneNumber,
                token,
                newPassword
            });
        }

        // Change password
        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            return await PostAsync<bool>("/Shield/Password/Change", new
            {
                currentPassword,
                newPassword
            });
        }

        #endregion

        #region Provider Management APIs

        // Link provider
        public async Task<bool> LinkProviderAsync(string providerName, string providerValue)
        {
            return await PostAsync<bool>("/Shield/Provider/Link", new
            {
                providerName,
                providerValue
            });
        }

        // Unlink provider
        public async Task<bool> UnlinkProviderAsync(string providerName)
        {
            return await PostAsync<bool>("/Shield/Provider/Unlink", new
            {
                providerName
            });
        }

        #endregion

        #region HTTP Helper Methods

        private async Task<T> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);

            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }

        private async Task<T> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
        {
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content, cancellationToken);

            string responseContent = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }

        private async Task<T> DeleteAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default)
        {
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            var request = new HttpRequestMessage(HttpMethod.Delete, endpoint)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            HttpResponseMessage response = await _httpClient.SendAsync(request);

            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }


        #endregion
    }

}
