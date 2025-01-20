using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using NBomber.Contracts;
using NBomber.Contracts.Stats;
using NBomber.CSharp;
using NBomber.Http.CSharp;

namespace L_Bank.Tests.Loadtest.Cli
{
    class Program
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        static async Task Main(string[] args)
        {
            try
            {
                // Login to get JWT token
                string jwt = await Login("testuser", "testuserpass");
                Console.WriteLine("Login successful. JWT received.");

                // Get all ledgers
                var ledgers = await GetAllLedgers(jwt);
                Console.WriteLine("Ledgers received:");
                foreach (var ledger in ledgers)
                {
                    Console.WriteLine($"- {ledger.Name}");
                }
                
                var scenario = CreateLoadTestScenario(jwt);
                
                NBomberRunner
                    .RegisterScenarios(scenario)
                    .WithReportFileName("fetch_users_report")
                    .WithReportFolder("fetch_users_reports")
                    .WithReportFormats(ReportFormat.Html)
                    .Run();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
        
        static ScenarioProps CreateLoadTestScenario(string jwt)
        {
            using var httpClient = new HttpClient();

            var scenario = Scenario.Create("http_scenario", async context =>
                {
                    var request =
                        Http.CreateRequest("GET", "https://nbomber.com")
                            .WithHeader("Accept", "text/html");
                    // .WithHeader("Accept", "application/json")
                    // .WithBody(new StringContent("{ id: 1 }", Encoding.UTF8, "application/json");
                    // .WithBody(new ByteArrayContent(new [] {1,2,3}))
                        

                    var response = await Http.Send(httpClient, request);

                    return response;
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.Inject(rate: 100, 
                        interval: TimeSpan.FromSeconds(1),
                        during: TimeSpan.FromSeconds(30))
                );

            return scenario;
        }


        private static async Task<string> Login(string username, string password)
        {
            var loginUrl = "http://localhost:5000/api/v1/Login"; 

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
            var ledgersUrl = "http://localhost:5000/api/v1/Ledgers"; 

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
