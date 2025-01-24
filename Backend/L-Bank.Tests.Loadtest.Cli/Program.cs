using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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
                string jwt = await Login("admin", "adminpass");
                Console.WriteLine("Login successful. JWT received.");

                double startingMoney = await GetTotalMoney(jwt);
                Console.WriteLine($"Starting money: {startingMoney}");

                var scenario = CreateLoadTestScenario(jwt);
                var bookingScenario = CreateBookingLoadTestScenario(jwt);
                NBomberRunner
                    .RegisterScenarios(scenario, bookingScenario)
                    .WithReportFileName("fetch_users_report")
                    .WithReportFolder("fetch_users_reports")
                    .WithReportFormats(ReportFormat.Html)
                    .Run();

                double endingMoney = await GetTotalMoney(jwt);
                Console.WriteLine($"Ending money: {endingMoney}");

                double difference = endingMoney - startingMoney;
                Console.WriteLine($"Difference: {difference:F4}");
                if (difference == 0)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("STARTING AND ENDING MONEY OK!");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("MONEY HAS CHANGED!");
                }

                Console.ResetColor();
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

        private static async Task<double> GetTotalMoney(string jwt)
        {
            var ledgersUrl = "http://localhost:5000/api/v1/Ledgers";

            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            var response = await HttpClient.GetAsync(ledgersUrl);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to retrieve ledgers.");
            }

            var ledgers = await response.Content.ReadFromJsonAsync<List<Ledger>>();
            if (ledgers == null) throw new Exception("Failed to deserialize ledgers.");

            return ledgers.Sum(ledger => ledger.Amount);
        }

        static ScenarioProps CreateLoadTestScenario(string jwt)
        {
            var scenario = Scenario.Create("http_scenario", async context =>
                {
                    var request = Http.CreateRequest("GET", "http://localhost:5000/api/v1/BankInfo")
                        .WithHeader("Authorization", $"Bearer {jwt}")
                        .WithHeader("Accept", "application/json");

                    var response = await Http.Send(HttpClient, request);

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

        static ScenarioProps CreateBookingLoadTestScenario(string jwt)
        {
            var scenario = Scenario.Create("booking_scenario", async context =>
                {
                    var bookingData = new
                    {
                        sourceId = 1,
                        destinationId = 2,
                        amount = 10,
                        source = new
                        {
                            id = 1,
                            name = "string",
                            balance = 10000
                        },
                        destination = new
                        {
                            id = 2,
                            name = "string",
                            balance = 10000
                        }
                    };

                    var jsonContent = JsonSerializer.Serialize(bookingData);

                    var request = Http.CreateRequest("POST", "http://localhost:5000/api/v1/Bookings")
                        .WithHeader("Accept", "application/json")
                        .WithHeader("Authorization", $"Bearer {jwt}")
                        .WithBody(new StringContent(jsonContent, Encoding.UTF8, "application/json"));

                    var response = await Http.Send(HttpClient, request);

                    return response;
                })
                .WithoutWarmUp()
                .WithLoadSimulations(
                    Simulation.Inject(rate: 50,
                        interval: TimeSpan.FromSeconds(1),
                        during: TimeSpan.FromSeconds(30))
                );

            return scenario;
        }

        public class LoginResponse
        {
            public string Token { get; set; }
        }

        public class Ledger
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public double Amount { get; set; }
        }
    }
}