using IdentityShield.Contracts.AccountLinking;
using IdentityShield.Contracts.Authentication.Login;
using IdentityShield.Contracts.Authentication.Logout;
using IdentityShield.Contracts.Authentication.Register;
using IdentityShield.Contracts.EmailManagement;
using IdentityShield.Contracts.Models;
using IdentityShield.Contracts.PasswordManagement;
using IdentityShield.Contracts.PhoneNumberManagement;
using IdentityShield.Contracts.ProviderManagement;
using IdentityShield.Contracts.TokenManagement;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace IdentityShield.Client
{
    public class IdentityShieldClient
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
        public async Task<LoginResponse> LoginByEmailAsync(string email, string password)
        {
            var request = new LoginByEmailRequest
            {
                Email = email,
                Password = password
            };
            
            return await PostAsync<LoginResponse>("/Shield/Authentication/Login/Email", request);
        }

        // Login with phone number
        public async Task<LoginResponse> LoginByPhoneNumberAsync(string phoneNumber, string password)
        {
            var request = new LoginByPhoneNumberRequest
            {
                PhoneNumber = phoneNumber,
                Password = password
            };
            
            return await PostAsync<LoginResponse>("/Shield/Authentication/Login/PhoneNumber", request);
        }

        // Login with provider
        public async Task<LoginResponse> LoginByProviderAsync(string providerName, string providerValue)
        {
            var request = new LoginByProviderRequest
            {
                ProviderName = providerName,
                ProviderValue = providerValue
            };
            
            return await PostAsync<LoginResponse>("/Shield/Authentication/Login/Provider", request);
        }

        // Login as managed account
        public async Task<LoginResponse> LoginAsManagedAccountAsync(string managedAccountId)
        {
            var request = new LoginByManagedAccountRequest
            {
                ManagedAccountId = managedAccountId
            };
            
            return await PostAsync<LoginResponse>("/Shield/Authentication/Login/ManagedAccount", request);
        }

        // Register with email
        public async Task<RegisterResponse> RegisterByEmailAsync(string email, string password)
        {
            var request = new RegisterByEmailRequest
            {
                Email = email,
                Password = password
            };
            
            return await PostAsync<RegisterResponse>("/Shield/Authentication/Register/Email", request);
        }

        // Confirm email registration
        public async Task<LoginResponse> ConfirmEmailRegistrationAsync(string email, string token)
        {
            var request = new ConfirmEmailRegistrationRequest
            {
                Email = email,
                Token = token
            };
            
            return await PostAsync<LoginResponse>("/Shield/Authentication/Register/Confirm/Email", request);
        }

        // Register with phone number
        public async Task<RegisterResponse> RegisterByPhoneNumberAsync(string phoneNumber, string password)
        {
            var request = new RegisterByPhoneNumberRequest
            {
                PhoneNumber = phoneNumber,
                Password = password
            };
            
            return await PostAsync<RegisterResponse>("/Shield/Authentication/Register/PhoneNumber", request);
        }

        // Confirm phone number registration
        public async Task<LoginResponse> ConfirmPhoneNumberRegistrationAsync(string phoneNumber, string token)
        {
            var request = new ConfirmPhoneNumberRegistrationRequest
            {
                PhoneNumber = phoneNumber,
                Token = token
            };
            
            return await PostAsync<LoginResponse>("/Shield/Authentication/Register/Confirm/PhoneNumber", request);
        }

        // Register with provider
        public async Task<LoginResponse> RegisterByProviderAsync(string providerName, string providerValue, string roleName = "User")
        {
            var request = new RegisterByProviderRequest
            {
                ProviderName = providerName,
                ProviderValue = providerValue,
                RoleName = roleName
            };
            
            return await PostAsync<LoginResponse>("/Shield/Authentication/Register/Provider", request);
        }

        // Logout
        public async Task<LogoutResponse> LogoutAsync()
        {
            var request = new LogoutRequest();
            
            return await PostAsync<LogoutResponse>("/Shield/Authentication/Logout", request);
        }

        // Request enforce logout by email
        public async Task<LogoutResponse> RequestEnforceLogoutByEmailAsync(string email)
        {
            var request = new EnforceLogoutByEmailRequest
            {
                Email = email
            };
            
            return await PostAsync<LogoutResponse>("/Shield/Authentication/Logout/Request/Email", request);
        }

        // Request enforce logout by phone number
        public async Task<LogoutResponse> RequestEnforceLogoutByPhoneNumberAsync(string phoneNumber)
        {
            var request = new EnforceLogoutByPhoneNumberRequest
            {
                PhoneNumber = phoneNumber
            };
            
            return await PostAsync<LogoutResponse>("/Shield/Authentication/Logout/Request/PhoneNumber", request);
        }

        // Confirm enforce logout
        public async Task<LogoutResponse> ConfirmEnforceLogoutAsync(string token)
        {
            var request = new ConfirmEnforceLogoutRequest
            {
                Token = token
            };
            
            return await PostAsync<LogoutResponse>("/Shield/Authentication/Logout/Confirm", request);
        }

        #endregion

        #region Account Linking APIs

        // Create managed account
        public async Task<AccountLinkingResponse> CreateManagedAccountAsync(string email, string password, string linkType)
        {
            var request = new CreateManagedAccountRequest
            {
                Email = email,
                Password = password,
                LinkType = linkType
            };
            
            return await PostAsync<AccountLinkingResponse>("/Shield/AccountLinking/Create", request);
        }

        // Link managed account
        public async Task<AccountLinkingResponse> LinkManagedAccountAsync(string managedAccountId, string linkType)
        {
            var request = new LinkManagedAccountRequest
            {
                ManagedAccountId = managedAccountId,
                LinkType = linkType
            };
            
            return await PostAsync<AccountLinkingResponse>("/Shield/AccountLinking/Link", request);
        }

        // Unlink managed account
        public async Task<AccountLinkingResponse> UnlinkManagedAccountAsync(string managedAccountId)
        {
            var request = new UnlinkManagedAccountRequest
            {
                ManagedAccountId = managedAccountId
            };
            
            return await DeleteAsync<AccountLinkingResponse>("/Shield/AccountLinking/Unlink", request);
        }

        // Get all managed accounts
        public async Task<IEnumerable<ShieldUser>> GetManagedAccountsAsync()
        {
            var response = await GetAsync<GetManagedAccountsResponse>("/Shield/AccountLinking/Managed/All");
            return response.Accounts;
        }

        // Get all accounts managing this account
        public async Task<IEnumerable<ShieldUser>> GetManagedByAccountsAsync()
        {
            var response = await GetAsync<GetManagedByAccountsResponse>("/Shield/AccountLinking/ManagedBy/All");
            return response.Accounts;
        }

        #endregion

        #region Token Management APIs

        // Refresh token
        public async Task<RefreshTokenResponse> RefreshTokenAsync(string refreshToken)
        {
            var request = new RefreshTokenRequest
            {
                RefreshToken = refreshToken
            };
            
            return await PostAsync<RefreshTokenResponse>("/Shield/Token/Refresh", request);
        }

        #endregion

        #region Email Management APIs

        // Request email change
        public async Task<EmailChangeResponse> RequestEmailChangeAsync(string newEmail)
        {
            var request = new RequestEmailChangeRequest
            {
                NewEmail = newEmail
            };
            
            return await PostAsync<EmailChangeResponse>("/Shield/Email/Change/Request", request);
        }

        // Confirm email change
        public async Task<EmailChangeResponse> ConfirmEmailChangeAsync(string token)
        {
            var request = new ConfirmEmailChangeRequest
            {
                Token = token
            };
            
            return await PostAsync<EmailChangeResponse>("/Shield/Email/Change/Confirm", request);
        }

        #endregion

        #region Phone Number Management APIs

        // Request phone number change
        public async Task<PhoneNumberChangeResponse> RequestPhoneNumberChangeAsync(string newPhoneNumber)
        {
            var request = new RequestPhoneNumberChangeRequest
            {
                NewPhoneNumber = newPhoneNumber
            };
            
            return await PostAsync<PhoneNumberChangeResponse>("/Shield/PhoneNumber/Change/Request", request);
        }

        // Confirm phone number change
        public async Task<PhoneNumberChangeResponse> ConfirmPhoneNumberChangeAsync(string token)
        {
            var request = new ConfirmPhoneNumberChangeRequest
            {
                Token = token
            };
            
            return await PostAsync<PhoneNumberChangeResponse>("/Shield/PhoneNumber/Change/Confirm", request);
        }

        #endregion

        #region Password Management APIs

        // Forgot password by email
        public async Task<PasswordResponse> ForgotPasswordByEmailAsync(string email)
        {
            var request = new ForgotPasswordByEmailRequest
            {
                Email = email
            };
            
            return await PostAsync<PasswordResponse>("/Shield/Password/Forgot/Email", request);
        }

        // Forgot password by phone number
        public async Task<PasswordResponse> ForgotPasswordByPhoneNumberAsync(string phoneNumber)
        {
            var request = new ForgotPasswordByPhoneNumberRequest
            {
                PhoneNumber = phoneNumber
            };
            
            return await PostAsync<PasswordResponse>("/Shield/Password/Forgot/PhoneNumber", request);
        }

        // Reset password by email
        public async Task<PasswordResponse> ResetPasswordByEmailAsync(string email, string token, string newPassword)
        {
            var request = new ResetPasswordByEmailRequest
            {
                Email = email,
                Token = token,
                NewPassword = newPassword
            };
            
            return await PostAsync<PasswordResponse>("/Shield/Password/Reset/Email", request);
        }

        // Reset password by phone number
        public async Task<PasswordResponse> ResetPasswordByPhoneNumberAsync(string phoneNumber, string token, string newPassword)
        {
            var request = new ResetPasswordByPhoneNumberRequest
            {
                PhoneNumber = phoneNumber,
                Token = token,
                NewPassword = newPassword
            };
            
            return await PostAsync<PasswordResponse>("/Shield/Password/Reset/PhoneNumber", request);
        }

        // Change password
        public async Task<PasswordResponse> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var request = new ChangePasswordRequest
            {
                CurrentPassword = currentPassword,
                NewPassword = newPassword
            };
            
            return await PostAsync<PasswordResponse>("/Shield/Password/Change", request);
        }

        #endregion

        #region Provider Management APIs

        // Link provider
        public async Task<ProviderResponse> LinkProviderAsync(string providerName, string providerValue)
        {
            var request = new LinkProviderRequest
            {
                ProviderName = providerName,
                ProviderValue = providerValue
            };
            
            return await PostAsync<ProviderResponse>("/Shield/Provider/Link", request);
        }

        // Unlink provider
        public async Task<ProviderResponse> UnlinkProviderAsync(string providerName)
        {
            var request = new UnlinkProviderRequest
            {
                ProviderName = providerName
            };
            
            return await PostAsync<ProviderResponse>("/Shield/Provider/Unlink", request);
        }

        #endregion

        #region HTTP Helper Methods

        private async Task<T> GetAsync<T>(string endpoint)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(endpoint);
            
            string content = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(content, _jsonOptions);
        }

        private async Task<T> PostAsync<T>(string endpoint, object data)
        {
            string json = JsonSerializer.Serialize(data, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(endpoint, content);
            
            string responseContent = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<T>(responseContent, _jsonOptions);
        }

        private async Task<T> DeleteAsync<T>(string endpoint, object data)
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

        private async Task EnsureSuccessStatusCodeAsync(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                string errorContent = await response.Content.ReadAsStringAsync();
                throw new IdentityShieldException(
                    (int)response.StatusCode,
                    $"API request failed with status {response.StatusCode}: {errorContent}");
            }
        }

        #endregion
    }

    public class IdentityShieldException : Exception
    {
        public int StatusCode { get; }

        public IdentityShieldException(int statusCode, string message) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
