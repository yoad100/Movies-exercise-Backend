using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using MySql.Data.MySqlClient;
using System.Text.Json;

namespace ServerExercise
{
    class DBHandler
    {
        private static DBHandler  db;
        private MySqlConnection connection;

        private DBHandler(string server,string databaseName,string username,string password) {
            try
            {
               connection = new MySqlConnection(
              "SERVER=" + server + ";" + "DATABASE=" + databaseName + ";"
              + "UID=" + username + ";" + "PASSWORD=" + password + ";"
              );

            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                Console.WriteLine(ex.Message);
            }
          
        }

        public string deleteMovie(string movieName)
        {
            try
            {
                string query =
             "DELETE FROM MOVIE WHERE movieName=" + movieName;
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return "Data deleted";
                }
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                return ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return "Data deletion failed";
        }

        public string insertMovieToDB(string movieName,string movieCategory,int movieRating,string movieImageUrl)
        {
            try
            {
                string query =
              "INSERT INTO movie(movieName,movieCategory,movieRating,movieImageUrl) Values('"
              + movieName + "','" + movieCategory + "','" + movieRating + "','" + movieImageUrl + "')";
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                if (cmd.ExecuteNonQuery() == 1)
                {
                    return "Data inserted";
                }
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                return ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return "Data insertion failed";
        }
        public string getTableRowsFromDB(string tableName)
        {
            
            try
            {
                string query = "SELECT * FROM " + tableName + " ORDER BY movieRating DESC LIMIT 10;";
                connection.Open();
                MySqlCommand cmd = new MySqlCommand(query, connection);
                MySqlDataReader reader = cmd.ExecuteReader();
                List<Dictionary<String, String>> arr = new List<Dictionary<String, String>>();
                
                while (reader.Read())
                {
                    Dictionary<String, String> dict = new Dictionary<String, String>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        dict.Add(reader.GetName(i), reader.GetValue(i).ToString());
                    }
                    arr.Add(dict);
                }
                reader.Close();
                var json = JsonSerializer.Serialize(arr);
                return json;
            }
            catch (Exception ex)
            {
                Logger.writeLog(ex.Message);
                return ex.Message;
            }
            finally
            {
                connection.Close();
            }
            return "Fail";

        } 

        [MethodImpl(MethodImplOptions.Synchronized)]
        public static DBHandler createDbInstance(string server, string databaseName, string username, string password)
        {
            if(db == null)
            {
                db = new DBHandler(server,databaseName,username,password);
            }

            return db;
        }
    }
}
