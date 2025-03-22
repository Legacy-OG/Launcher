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
using System.IO;

namespace Legacy_Launcher
{
    internal class Program
    {
        private static string ExchangeCode = "";
        private static bool PathValid = false;

        static void Intro()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("Welcome to Legacy launcher!");
            Console.ResetColor();
            Console.WriteLine();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.access_token) && !string.IsNullOrEmpty(Properties.Settings.Default.refresh_token))
            {} else
            {
                Utils.Logger.good("You will be redirected to the Epic Games website to login. After logging in please get the \"authorizationCode\".");
                Thread.Sleep(2000);
                Process.Start(new ProcessStartInfo("https://www.epicgames.com/id/api/redirect?clientId=ec684b8c687f479fadea3cb2ad83f5c6&responseType=code") { UseShellExecute = true });
            }
        }

        static async Task Main(string[] args)
        {
            Intro();
            if (!string.IsNullOrEmpty(Properties.Settings.Default.access_token) && !string.IsNullOrEmpty(Properties.Settings.Default.refresh_token))
            {
                await RefreshToken(Properties.Settings.Default.refresh_token);
            } else
            {
                Console.Write("Please enter your authorization code: ");
                string AutorizationCode = Console.ReadLine();
                await GetAccessToken(AutorizationCode);
            }
            string GamePath = await DetermineGamePath();
            await GetExchangeCode(Properties.Settings.Default.access_token);
            LaunchService.LaunchService.InitializeLaunching(Program.ExchangeCode, GamePath, 0);
        }

        static async Task<string> DetermineGamePath()
        {
            Console.WriteLine("Do you want to update your game path?");
            Console.Write("[1]: Yes | [2]: No: ");
            string choice = Console.ReadLine();
            string GamePath = "";
            if (choice == "1")
            {
                while (!PathValid)
                {
                    Console.Write("Please enter your game path: ");
                    GamePath = Console.ReadLine();
                    if (File.Exists(Path.Combine(GamePath, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")))
                    {
                        PathValid = true;
                    }
                    else
                    {
                        Utils.Logger.warn("Path Invalid, Make sure your path has the \"Engine\" and \"FortniteGame\" Folders inside!");
                    }
                    Console.WriteLine();
                }
                Properties.Settings.Default.GamePath = GamePath;
            }
            else if (choice == "2") 
            {
                if (string.IsNullOrEmpty(Properties.Settings.Default.GamePath)) 
                {
                    Utils.Logger.warn("No existing gamepath!");
                    while (!PathValid)
                    {
                        Console.Write("Please enter your game path: ");
                        GamePath = Console.ReadLine();
                        if (File.Exists(Path.Combine(GamePath, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")))
                        {
                            PathValid = true;
                        }
                        else
                        {
                            Utils.Logger.warn("Path Invalid, Make sure your path has the \"Engine\" and \"FortniteGame\" Folders inside!");
                        }
                        Console.WriteLine();
                    }
                    Properties.Settings.Default.GamePath = GamePath;
                }
            }
            else
            {
                Utils.Logger.warn("Not a correct choice!");
                if (string.IsNullOrEmpty(Properties.Settings.Default.GamePath))
                {
                    Utils.Logger.warn("No existing gamepath!");
                    while (!PathValid)
                    {
                        Console.Write("Please enter your game path: ");
                        GamePath = Console.ReadLine();
                        if (File.Exists(Path.Combine(GamePath, "FortniteGame\\Binaries\\Win64\\", "FortniteClient-Win64-Shipping.exe")))
                        {
                            PathValid = true;
                        }
                        else
                        {
                            Utils.Logger.warn("Path Invalid, Make sure your path has the \"Engine\" and \"FortniteGame\" Folders inside!");
                        }
                        Console.WriteLine();
                    }
                    Properties.Settings.Default.GamePath = GamePath;
                }
            }

            Properties.Settings.Default.Save();
            Utils.Logger.good($"Saved Path: {Properties.Settings.Default.GamePath}");
            return Properties.Settings.Default.GamePath;
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
                    Properties.Settings.Default.access_token = json["access_token"].ToString();
                    if (json.ContainsKey("refresh_token"))
                    {
                        Properties.Settings.Default.refresh_token = json["refresh_token"].ToString();
                    }
                    else
                    {
                        Utils.Logger.error("Could not get refresh token, Launching will still proceed but automatic login might not work!");
                    }
                    Properties.Settings.Default.Save();
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

        static async Task RefreshToken(string refresh_token)
        {
            using (HttpClient client = new HttpClient())
            {
                string url = "https://account-public-service-prod.ol.epicgames.com/account/api/oauth/token";

                var requestData = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("grant_type", "refresh_token"),
                    new KeyValuePair<string, string>("refresh_token", refresh_token),
                    new KeyValuePair<string, string>("token_type", "eg1"),
                });

                client.DefaultRequestHeaders.Add("Authorization", "Basic ZWM2ODRiOGM2ODdmNDc5ZmFkZWEzY2IyYWQ4M2Y1YzY6ZTFmMzFjMjExZjI4NDEzMTg2MjYyZDM3YTEzZmM4NGQ=");

                HttpResponseMessage response = await client.PostAsync(url, requestData);
                string responseString = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseString);
                if (json.ContainsKey("access_token"))
                {
                    Properties.Settings.Default.access_token = json["access_token"].ToString();
                    Properties.Settings.Default.Save();
                }
                else
                {
                    Utils.Logger.warn("Could not get access token!");
                    Utils.Logger.good("You will be redirected to the Epic Games website to login. After logging in please get the \"authorizationCode\".");
                    Thread.Sleep(2000);
                    Process.Start(new ProcessStartInfo("https://www.epicgames.com/id/api/redirect?clientId=ec684b8c687f479fadea3cb2ad83f5c6&responseType=code") { UseShellExecute = true });
                    Console.Write("Please enter your authorization code: ");
                    string AutorizationCode = Console.ReadLine();
                    await GetAccessToken(AutorizationCode);
                    string GamePath = await DetermineGamePath();
                    await GetExchangeCode(Properties.Settings.Default.access_token);
                    LaunchService.LaunchService.InitializeLaunching(Program.ExchangeCode, GamePath, 0);
                }

                Utils.Logger.good($"Successfully logged into {json["displayName"].ToString()}");
            }
        }
    }
}
