using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
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

        public User(string APIKey, string UserName)
        {
            this.APIKey = APIKey;
            this.UserName = UserName;
        }
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
                json = json.Trim('\"');
                return (json);
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                return (content);
            }
        }

        static async Task<string> PostAsJsonAsync(string path, StringContent input)
        {
            HttpResponseMessage response = await client.PostAsync(path, input);
            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                return (json);
            }
            else
            {
                string content = await response.Content.ReadAsStringAsync();
                return (content);
            }
        }

        static void Main(string[] args)
        {
            client.DefaultRequestHeaders.Accept.Clear(); //clear base headers
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json")); //specify json response
            client.BaseAddress = new Uri("http://localhost:5001/");
            User currentUser = new User(null, null);

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
                        RunAsync(input, currentUser).GetAwaiter().GetResult();
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

        static async Task RunAsync(string input, User currentUser)
        {
            string path = null;
            #region Input Handling
            if (!string.IsNullOrEmpty(input))
            {
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
                                            path = client.BaseAddress + "api/talkback/sort?"; //fix output format

                                            string integers = inputArr[2].Trim('[', ']');                                            
                                            var integersArr = integers.Split(',');
                                            for (int i = 0; i < integersArr.Length; i++)
                                            {
                                                path += "integers=" + integersArr[i] + "&";
                                            }
                                            path = path.TrimEnd('&');
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
                                            path = client.BaseAddress + "api/user/new?username=" + inputArr[2];
                                            var response = await GetStringAsync(path);
                                            Console.WriteLine("\n" + response + "\n"); //format
                                            break;
                                            }
                                        case "post":
                                            {
                                            path = client.BaseAddress + "api/user/new";
                                            var UserName = JsonConvert.SerializeObject(inputArr[2]);
                                            UserName = UserName.Trim('\"');
                                            var body = new StringContent(UserName, UnicodeEncoding.UTF8, "application/json");
                                            var response = await client.PostAsync(path, body);
                                            if (response.IsSuccessStatusCode)
                                            {
                                                var json = await response.Content.ReadAsStringAsync();
                                                json = json.Trim('\"');
                                                currentUser.APIKey = json;
                                                currentUser.UserName = UserName;
                                                Console.WriteLine(json);
                                            }
                                            else
                                            {
                                                string content = await response.Content.ReadAsStringAsync();
                                                Console.WriteLine(content);
                                            }
                                            break;
                                            }
                                        case "delete":
                                            {
                                            if (currentUser.APIKey != null)
                                            {
                                                path = client.BaseAddress + "api/user/removeuser?username=" + currentUser.UserName;
                                                client.DefaultRequestHeaders.Add("ApiKey", currentUser.APIKey); //Adds APIKey to headers

                                                var response = await client.DeleteAsync(path);
                                                var json = await response.Content.ReadAsStringAsync();
                                                if (json == "\"User not found\"")
                                                { Console.WriteLine("False"); }
                                                else
                                                { Console.WriteLine("True"); }
                                                client.DefaultRequestHeaders.Remove("ApiKey");
                                            }
                                            else
                                            {
                                                Console.WriteLine("You need to do a User Post or User Set first");
                                            }
                                            break;
                                            }
                                        case "role": //add store functionality if ok
                                            {
                                            if (currentUser.APIKey != null)
                                            {
                                                path = client.BaseAddress + "api/user/changerole";
                                                client.DefaultRequestHeaders.Add("ApiKey", currentUser.APIKey); //Adds APIKey to headers
                                                string tempBody = "{\n\"username\" : \"" + inputArr[2] + "\",\n\"role\" : \"" + inputArr[3] + "\"\n}"; //formatting for json object

                                                var body = new StringContent(tempBody, UnicodeEncoding.UTF8, "application/json"); //body formatting
                                                var response = await client.PostAsync(path, body); //request

                                                var json = await response.Content.ReadAsStringAsync(); //response parse
                                                Console.WriteLine("\n" + json + "\n");

                                                if (response.IsSuccessStatusCode) //if sucessful, store user details
                                                {
                                                    currentUser.APIKey = inputArr[3];
                                                    currentUser.UserName = inputArr[2];
                                                    Console.WriteLine("Stored\n");
                                                }
                                                client.DefaultRequestHeaders.Remove("ApiKey");
                                            }
                                            else
                                            { Console.WriteLine("You need to do a User Post or User Set first"); }
                                            break;
                                        }
                                        case "set": //client side only 'Store givenAPI Key and username as variables'
                                            {
                                            currentUser.APIKey = inputArr[3];
                                            currentUser.UserName = inputArr[2];
                                            Console.WriteLine("Stored\n");

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
