using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.Net.Sockets;
using System.Text;
using Microsoft.CodeAnalysis;


namespace WebServer
{

    class Program
    {
        const string api_key = "5fee2d373de0ac00a9db64cff3cad54d";
        static TcpListener listener = new TcpListener(IPAddress.Any, 8083);
        static HttpClient client = new HttpClient();
        static readonly Dictionary<string, string> cache = new Dictionary<string, string>();

        public static void Main(String[] args)
        {
            listener.Start();
            Console.WriteLine("Listening...");

            Thread nit = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        TcpClient client = listener.AcceptTcpClient();
                        ThreadPool.QueueUserWorkItem(ProcessRequest!, client);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);

                    }
                }
            });
            nit.Start();
            Console.WriteLine("Pokrenuta Nit");

            Console.ReadKey();
            listener.Stop();

        }

        static void ProcessRequest(object state)
        {
            TcpClient client = (TcpClient)state;
            NetworkStream stream = client.GetStream();
            StreamReader reader = new StreamReader(stream);
            StreamWriter writer = new StreamWriter(stream)
            {
                AutoFlush = true
            };

            try
            {
                string requests = reader.ReadLine();

                if (requests != null)
                {

                    string[] parts = Regex.Split(requests, @"\s+");

                    if (parts.Length == 3 && parts[0].Equals("GET"))
                    {

                        string query = Regex.Replace(parts[1], "%20", "+");
                        query = query.Remove(0, 1);

                        if (!cache.ContainsKey(query))
                        {
                            Console.WriteLine("Koristim API :");
                            JObject data = SearchMovieByName(query);
                            Console.WriteLine("" + data);

                            if (data["total_results"].ToString() != "0")
                            {
                                string forma = "<div>";
                                foreach (var item in data["results"])
                                {
                                    forma += $"<h1>Original Title : {item["original_title"]}</h1>";
                                    forma += $"<p>ID: {item["id"]}</p";
                                    forma += $"<p>Overview : {item["overview"]}</p>";
                                    forma += $"<img src=\"https://image.tmdb.org/t/p/w500{item["poster_path"]}\" ></img>";
                                }
                                forma += "</div>";

                                string htmlContent = "<!DOCTYPE html>" +
                                             "<html lang=\"en\">" +
                                             "<head><meta charset=\"UTF-8\"><title>Test Page</title></head>" +
                                             "<body><h1>Welcome to MOVIE OVERVIEW</h1><p>podaci se salju uri-om</p></body>" +
                                             forma +
                                             "</html>";
                                // HTTP response header

                                string response = "HTTP/1.1 200 OK\r\n" +
                                          "Content-Length: " + Encoding.UTF8.GetByteCount(htmlContent) + "\r\n" +
                                          "Content-Type: text/html; charset=UTF-8\r\n" +
                                          "\r\n" +
                                          htmlContent;

                                cache.Add(query, response);

                                writer.Write(response);
                            }
                            else
                            {

                                writer.Write("HTTP/1.1 404 Not Found\r\n"); // Changed to 404 Not Found
                                writer.Write("Content-Type: text/html; charset=UTF-8\r\n");
                                writer.Write("\r\n");

                                //vrati Lepo da prikaze da nije tu film
                                writer.Write("<!DOCTYPE html>" +
                                             "<html lang=\"en\">" +
                                             "<head><meta charset=\"UTF-8\"><title>Not Found</title></head>" +
                                             "<body>" +
                                             "<h1>Welcome to MOVIE OVERVIEW</h1>" +
                                             "<p>Podaci se šalju URI-om</p>" +
                                             "<div><h1>Film Nije Pronađen</h1></div>" +
                                             "</body>" +
                                             "</html>");

                            }
                        }
                        else
                        {
                            //samo citam tako da nema potrebe da koristim lock
                            Console.WriteLine("Koristim Cache");
                            Console.WriteLine(cache[query]);
                            writer.WriteLine(cache[query]);
                        }
                        Console.WriteLine("Response je poslat");
                    }
                }
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        static JObject SearchMovieByName(string query)
        {
            string url = $"https://api.themoviedb.org/3/search/movie?query={query}&api_key={api_key}";

            try
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                Console.WriteLine(response);
                if (response.IsSuccessStatusCode)
                {
                    string res_body = response.Content.ReadAsStringAsync().Result; //ovo je sinhrona operacija
                    JObject data = JObject.Parse(res_body);
                    return data;
                }
                return null;
            }
            catch (System.Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }
    }
}
