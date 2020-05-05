using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DistSysACWClient
{
    #region Task 10 and beyond
    public enum Role { User, Admin };
    public class User
    {
        public string APIKey { get; set; }
        public string UserName { get; set; }
        public Role Role { get; set; }
    }

    class Client
    {
        static HttpClient client = new HttpClient();
        static void PrintUser(User user)
        {
            Console.WriteLine($"Name: {user.UserName}\tRole: " +
                $"{user.Role}\tAPIKey: {user.APIKey}");
        }

        static async Task<User> GetUserAsync(string path)
        {
            User user = null;
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                user = JsonConvert.DeserializeObject<User>(await response.Content.ReadAsStringAsync());
            }
            return user;
        }

        static async Task<string> GetStringAsync(string path)
        {
            HttpResponseMessage response = await client.GetAsync(path);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return(json);
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                return($"Content: {content}");
            }            
        }

        static void Main(string[] args)
        {
            string welcomeMessage = "Hello. What would you like to do?";
            while (true)
            {
                var client = new HttpClient();
                Console.WriteLine(welcomeMessage);
                string input = Console.ReadLine();
                welcomeMessage = "What would you like to do next?";
                if (input.ToLower() != "exit")
                {
                    try
                    {
                        RunAsync(input).GetAwaiter().GetResult();
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine("Error Output: " + e);
                    }
                }
                else
                {
                    Environment.Exit(0);
                }
            }
        }

        static async Task RunAsync(string input)
        {
            #region Input Handling
            if (!string.IsNullOrEmpty(input))
            {
                    client.DefaultRequestHeaders.Accept.Clear(); //clear base headers
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //specify json response
                    client.BaseAddress = new Uri("http://localhost:5001/");
                    string path = null;

                    try
                    {
                        string[] inputArr = input.Split(" "); //splits input string by space character
                        switch (inputArr[0].ToLower()) //switch statements for input to determine api request
                        {
                            #region Talkback Controller
                            case "talkback":
                                {
                                    switch (inputArr[1].ToLower())
                                    {
                                        case "hello":
                                            {
                                                path = client.BaseAddress + "api/talkback/hello";
                                                var response = await GetStringAsync(path);
                                                Console.WriteLine("\n" + response + "\n");
                                                break;
                                            }
                                        case "sort":
                                            {
                                                path = client.BaseAddress + "api/talkback/sort"; //fix output format
                                                var response = await GetStringAsync(path);
                                                Console.WriteLine("\n" + response + "\n");

                                                break;
                                            }
                                    }
                                    break;
                                }
                            #endregion

                            #region User Controller
                            case "user":
                                {
                                    switch (inputArr[1].ToLower())
                                    {
                                        case "get":
                                            {
                                            path = client.BaseAddress + "api/user/new";
                                            var response = await GetStringAsync(path);
                                            Console.WriteLine("\n" + response + "\n"); //format
                                            break;
                                            }
                                        case "post":
                                            {
                                            path = client.BaseAddress + "api/user/new";
                                            var response = await GetStringAsync(path);
                                            Console.WriteLine("\n" + response + "\n"); //format
                                            break;
                                            }
                                        case "delete":
                                            {
                                            path = client.BaseAddress + "api/user/removeuser";
                                            var response = await GetStringAsync(path);
                                            Console.WriteLine("\n" + response + "\n"); //format
                                            break;
                                        }
                                        case "role": //add store functionality if ok
                                            {
                                            path = client.BaseAddress + "api/user/changerole";
                                            var response = await GetStringAsync(path);
                                            Console.WriteLine("\n" + response + "\n"); //format

                                            if (response == "OK") //fix
                                            {
                                                Console.WriteLine("Stored");
                                            }

                                            //store details

                                            break;
                                        }
                                        case "set": //client side only 'Store givenAPI Key and username as variables'
                                            {
                                            //call store method

                                            break;
                                            }
                                }
                                    break;
                                }
                            #endregion

                            #region Protected Controller

                            case "protected":
                                {
                                    switch (inputArr[1].ToLower())
                                    {
                                        case "hello":
                                            {
                                            path = client.BaseAddress + "api/protected/hello";
                                            var response = await GetStringAsync(path);
                                            Console.WriteLine("\n" + response + "\n");
                                            break;
                                            }
                                        case "sha1":
                                            {
                                            path = client.BaseAddress + "api/protected/sha1";
                                            var response = await GetStringAsync(path);
                                            Console.WriteLine("\n" + response + "\n");
                                            break;
                                        }
                                        case "sha256":
                                            {
                                            path = client.BaseAddress + "api/protected/sha256";
                                            var response = await GetStringAsync(path);
                                            Console.WriteLine("\n" + response + "\n");
                                            break;
                                        }
                                    }
                                    break;
                                }
                            #endregion

                            default:
                                {
                                    { Console.WriteLine("\nInvalid Input \n"); }
                                    break;
                                }
                        }
                    }
                    catch (Exception e)
                    {

                        Console.WriteLine("Error Output: " + e);
                    }
                }
            else
            { Console.WriteLine("Invalid Input /n"); }
            #endregion
        }

    }
    #endregion
}
