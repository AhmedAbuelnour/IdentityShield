using IdentityShield.Client;
using IdentityShield.Domain.Models;
using System.Text.Json;

namespace IdentityShield.ClientDemo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            try
            {
                // Create the client with the base URL of your API
                var client = new IdentityShieldClient("https://localhost:7264"); // Use the URL from your launchSettings.json
                
                Console.WriteLine("IdentityShield API Client Demo");
                Console.WriteLine("===============================");

                // Example: Login with email
                Console.WriteLine("\nLogging in with email...");
                JwtResponse loginResponse = await client.LoginByEmailAsync("user@example.com", "Password123!");
                
                Console.WriteLine($"Login successful! Access Token: {loginResponse.AccessToken.Substring(0, 20)}...");
                
                // Set the token for subsequent authenticated requests
                client.SetToken(loginResponse.AccessToken);
                
                // Example: Get managed accounts
                Console.WriteLine("\nFetching managed accounts...");
                var managedAccounts = await client.GetManagedAccountsAsync();
                
                Console.WriteLine($"Found {managedAccounts.Count()} managed accounts:");
                foreach (var account in managedAccounts)
                {
                    Console.WriteLine($"- {account.UserName} ({account.Id})");
                }
                
                // Example: Request password reset
                Console.WriteLine("\nRequesting password reset by email...");
                bool resetRequestSuccess = await client.ForgotPasswordByEmailAsync("user@example.com");
                
                Console.WriteLine($"Password reset request {(resetRequestSuccess ? "successful" : "failed")}");
                
                // Example: Refresh token
                Console.WriteLine("\nRefreshing token...");
                JwtResponse refreshResponse = await client.RefreshTokenAsync(loginResponse.RefreshToken);
                
                Console.WriteLine($"Token refreshed! New Access Token: {refreshResponse.AccessToken.Substring(0, 20)}...");
                
                // Update token with the new one
                client.SetToken(refreshResponse.AccessToken);
                
                // Example: Logout
                Console.WriteLine("\nLogging out...");
                bool logoutSuccess = await client.LogoutAsync();
                
                Console.WriteLine($"Logout {(logoutSuccess ? "successful" : "failed")}");
                
                // Clear the token after logout
                client.ClearToken();
                
                Console.WriteLine("\nDemo completed successfully!");
            }
            catch (IdentityShieldException ex)
            {
                Console.WriteLine($"API Error (Status {ex.StatusCode}): {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
