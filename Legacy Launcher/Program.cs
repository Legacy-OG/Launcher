using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Text;
using System.Threading.Tasks;
using Legacy_Launcher.LaunchService;
using Legacy_Launcher.Utils;
using System.Net.Http;
using System.Net;
using Newtonsoft.Json.Linq;

namespace Legacy_Launcher
{
    internal class Program
    {
        private static string ExchangeCode = "";

        static void Intro()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to Legacy launcher!");
            Console.ResetColor();
            Console.WriteLine();
            Utils.Logger.good("You will be redirected to the Epic Games website to login. After logging in please get the \"authorizationCode\".");
            Thread.Sleep(2000);
            Process.Start(new ProcessStartInfo("https://www.epicgames.com/id/api/redirect?clientId=ec684b8c687f479fadea3cb2ad83f5c6&responseType=code") { UseShellExecute = true });
        }

        static async Task Main(string[] args)
        {
            Intro();
            Console.Write("Please enter your authorization code: "); // We should check if this is valid
            string AutorizationCode = Console.ReadLine();
            await GetAccessToken(AutorizationCode);
            Console.Write("Please enter your game path: "); // we should also check if this is valid
            string GamePath = Console.ReadLine();
            Console.WriteLine();
            LaunchService.LaunchService.InitializeLaunching(Program.ExchangeCode, GamePath, 0);
        }

        static async Task GetAccessToken(string AuthorizationCode)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token";

                var requestData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "authorization_code"),
                    new KeyValuePair<string, string>("code", AuthorizationCode),
                    new KeyValuePair<string, string>("token_type", "eg1"),
                });

                client.DefaultRequestHeaders.Add("Authorization", "Basic ZWM2ODRiOGM2ODdmNDc5ZmFkZWEzY2IyYWQ4M2Y1YzY6ZTFmMzFjMjExZjI4NDEzMTg2MjYyZDM3YTEzZmM4NGQ=");

                HttpResponseMessage response = await client.PostAsync(url, requestData);
                string responseString = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseString);
                if (json.ContainsKey("access_token"))
                {
                    await GetExchangeCode(json["access_token"].ToString());
                } else
                {
                    Utils.Logger.error("Could not get access token!");
                    Console.ReadKey();
                    Environment.Exit(1);
                }

                Utils.Logger.good($"Successfully logged into {json["displayName"].ToString()}");
            }
        }

        static async Task GetExchangeCode(string AccessToken)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://account-public-service-prod03.ol.epicgames.com/account/api/oauth/exchange";

                client.DefaultRequestHeaders.Add("Authorization", $"bearer {AccessToken}");

                HttpResponseMessage response = await client.GetAsync(url);
                string responseString = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseString);
                if (json.ContainsKey("code"))
                {
                    ExchangeCode = json["code"].ToString();
                } else
                {
                    Utils.Logger.error("Failed to get exchange code!");
                    Console.ReadKey();
                    Environment.Exit(1);
                }
            }
        }
    }
}
