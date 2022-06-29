using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace jf_FinalProject.Logger
{
    public class Logger
    {
        private readonly static string _commandSendFileName = "CommandSend.json";
        private readonly static string _developerFileName = "Developer.json";
        private readonly static string _jfErrorFileName = "JfErrors.json";
        private static StreamWriter OpenJsonFile(string fileName)
        {
            string basePath = Path.GetFullPath(Directory.GetCurrentDirectory());
            string path = basePath + @"/LogFiles" + @"/" + fileName;
            if (!Directory.Exists(basePath + @"/LogFiles"))
                Directory.CreateDirectory(basePath + @"/LogFiles");
            StreamWriter sr;
            if (!File.Exists(path))
            {
                FileStream file = File.Create(path);
                file.Close();
            }
            sr = new StreamWriter(path, append:true);
            return sr;
        }

        /// <summary>
        /// this logger log commands that our code create for hardware
        /// </summary>
        /// <param name="command">Its a string that contains command of hardware</param>
        public static void Log(string command)
        {
            StreamWriter sr = OpenJsonFile(_commandSendFileName);
            CommandSendLog commandSendLog = new CommandSendLog { Command = command, Time = DateTime.Now};
            string jsonData = JsonConvert.SerializeObject(commandSendLog, Formatting.Indented);
            sr.Write(jsonData + ",\n");
            sr.Close();
        }

        /// <summary>
        /// this logger log errors and events that happened in c# code. Its understandable just for
        /// Developers
        /// </summary>
        /// <param name="lineNumber">Its an integer that contains number of line that event happened</param>
        /// <param name="fileName">Its a string and contains name of C# file that error occured</param>
        /// <param name="errorMessage">Its a string and contains message of event or error that occured</param>
        public static void Log(int lineNumber, string fileName, string errorMessage)
        {
            StreamWriter sr = OpenJsonFile(_developerFileName);
            DeveloperLog developerLog = new DeveloperLog { LineNumber = lineNumber, ErrorMessage = errorMessage, FileName = fileName, Time = DateTime.Now };
            string jsonData = JsonConvert.SerializeObject(developerLog, Formatting.Indented);
            sr.Write(jsonData + ",\n");
            sr.Close();
        }

        /// <summary>
        /// this logger log errors that occured in jf code
        /// </summary>
        /// <param name="sender">object of sender. we use it to find caller is compiler or runner</param>
        /// <param name="lineNumber">Its an integer and contains number of line that this error occured</param>
        /// <param name="errorMessage">Its a string and contains error message</param>
        public static void Log(object sender, int lineNumber, string errorMessage)
        {
            StreamWriter sr = OpenJsonFile(_jfErrorFileName);
            ErrorType errorType;
            if (sender is jf.Compiler) errorType = ErrorType.Compile;
            else if (sender is jf.Runner) errorType = ErrorType.Run;
            else
            {
                Log(GetCurrentLine(), "Logger.cs", $"class {sender.GetType().Name} attempt to log jfCodeErrorLog");
                return;
            }
            jfCodeErrorLog jfLog = new jfCodeErrorLog { LineNumber = lineNumber,
                ErrorMessage = errorMessage,
                TypeOfError = errorType,
                Time = DateTime.Now };
            string jsonData = JsonConvert.SerializeObject(jfLog, Formatting.Indented);
            sr.Write(jsonData + ",\n");
            sr.Close();
        }

        public static int GetCurrentLine([CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }
    }
}
