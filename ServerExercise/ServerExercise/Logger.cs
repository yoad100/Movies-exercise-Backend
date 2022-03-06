using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ServerExercise
{
    public static class Logger
    {
        static string logsPath = Path.Combine(Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName, "ErrorsLogs.txt");
        public static void writeLog(string message)
        {
            using(StreamWriter writer = new StreamWriter(logsPath,true))
            {
                writer.WriteLine($"{DateTime.Now} {message}");
            }
        }
    }
}
