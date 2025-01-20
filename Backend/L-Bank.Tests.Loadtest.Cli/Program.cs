using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LBank.Tests.Loadtest.Cli
{
    class Program
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            try
            {
                // Login to get JWT token
                string jwt = await Login("admin", "adminpass");
                Console.WriteLine("Login successful. JWT received.");

                // Get all ledgers
                var ledgers = await GetAllLedgers(jwt);
                Console.WriteLine("Ledgers received:");
                foreach (var ledger in ledgers)
                {
                    Console.WriteLine($"- {ledger.Name}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        private static async Task<string> Login(string username, string password)
        {
            var loginUrl = "https://localhost:5000/api/v1/Login"; // Replace with your API URL

            var loginPayload = new
            {
                Username = username,
                Password = password
            };

            var response = await HttpClient.PostAsJsonAsync(loginUrl, loginPayload);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Login failed. Please check your credentials.");
            }

            var responseData = await response.Content.ReadFromJsonAsync<LoginResponse>();
            return responseData?.Token ?? throw new Exception("JWT token not received.");
        }

        private static async Task<List<Ledger>> GetAllLedgers(string jwt)
        {
            var ledgersUrl = "https://localhost:5000/api/v1/Ledgers"; // Replace with your API URL

            HttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", jwt);

            var response = await HttpClient.GetAsync(ledgersUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve ledgers.");
            }

            var ledgers = await response.Content.ReadFromJsonAsync<List<Ledger>>();
            return ledgers ?? new List<Ledger>();
        }

        // Classes for API response deserialization
        public class LoginResponse
        {
            public string Token { get; set; }
        }

        public class Ledger
        {
            public int Id { get; set; }
            public string Name { get; set; }
        }
    }
}
