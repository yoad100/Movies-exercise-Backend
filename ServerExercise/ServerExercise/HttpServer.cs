// Filename:  HttpServer.cs        
// Author:    Benjamin N. Summerton <define-private-public>        
// License:   Unlicense (http://unlicense.org/)

using System;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ServerExercise
{
    class HttpServer
    {
        public static HttpListener listener;
        public static string url = "http://localhost:8000/";
        private static DBHandler db = DBHandler.createDbInstance("localhost", "MoviesDB","root","5322453");

        public static async Task HandleIncomingConnections()
        {
            bool runServer = true;
            // While a user hasn't visited the `shutdown` url, keep on handling requests
            while (runServer)
            {
                // Will wait here until we hear from a connection
                HttpListenerContext ctx = await listener.GetContextAsync();
               
                // Peel out the requests and response objects
                HttpListenerRequest req = ctx.Request;
                HttpListenerResponse resp = ctx.Response;

                resp.AddHeader("Access-Control-Allow-Origin", "*");
                resp.AddHeader("Access-Control-Allow-Credentials", "true");
                resp.AddHeader("Access-Control-Allow-Methods", "GET,HEAD,OPTIONS,POST,PUT");
                resp.AddHeader("Access-Control-Allow-Headers", "Access-Control-Allow-Headers, Origin,Accept, X-Requested-With, Content-Type, Access-Control-Request-Method, Access-Control-Request-Headers");
                if (req.HttpMethod == "GET" && (req.Url.AbsolutePath == "/getMovies"))
                {
                    await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(db.getTableRowsFromDB("movie")));
                    resp.Close();
                }

                else if (req.HttpMethod == "POST" && (req.Url.AbsolutePath == "/addMovie"))
                {
                    
                    string respData = GetRequestPostData(req);
                    string resMessage = "";
                    try
                    {
                        JObject json = JObject.Parse(respData);
                        string movieName = json.GetValue("movieName").ToString();
                        string movieCategory = json.GetValue("movieCategory").ToString();
                        int movieRating = Int32.Parse(json.GetValue("movieRating").ToString());
                        string movieImageUrl = json.GetValue("movieImageUrl").ToString();
                        JToken deletedMovie = json.GetValue("deletedMovie");
                        if(deletedMovie != null)
                        {
                            db.deleteMovie(deletedMovie.ToString());
                        }
                        resMessage = db.insertMovieToDB
                            (
                                movieName
                                ,movieCategory 
                                ,movieRating 
                                ,movieImageUrl 
                            );
                    }
                    catch (Exception ex)
                    {
                        Logger.writeLog(ex.Message);
                        resMessage = ex.Message;
                    }
                    finally
                    {
                        await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes(resMessage));
                        resp.Close();
                    }



                }
                else
                {
                    await resp.OutputStream.WriteAsync(Encoding.UTF8.GetBytes("Page not found"));
                    resp.Close();
                }


            }
        }

        public static string GetRequestPostData(HttpListenerRequest request)
        {
            if (!request.HasEntityBody)
            {
                return null;
            }
            using (System.IO.Stream body = request.InputStream) // here we have data
            {
                using (var reader = new System.IO.StreamReader(body, request.ContentEncoding))
                {
                    return reader.ReadToEnd();
                }
            }
        }


        public static void Main(string[] args)
        {
            // Create a Http server and start listening for incoming connections
            listener = new HttpListener();
            listener.Prefixes.Add(url);
            listener.Start();
            Console.WriteLine("Listening for connections on {0}", url);

            // Handle requests
            Task listenTask = HandleIncomingConnections();
            listenTask.GetAwaiter().GetResult();

            // Close the listener
            listener.Close();
        }
    }
}