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
        private readonly static string _sensorCheckFileName = "SensorCheck.json";

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

        #region Command Send Log
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
        #endregion

        #region Developer Log
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
        #endregion

        #region JF Code Error Log
        /// <summary>
        /// this logger log errors that occured in jf code
        /// </summary>
        /// <param name="sender">object of sender. we use it to find caller is compiler or runner</param>
        /// <param name="lineNumber">Its an integer and contains number of line that this error occured</param>
        /// <param name="errorMessage">Its a string and contains error message</param>
        /// <param name="jfFileName">Its a string and contains jf file name</param>
        public static void Log(object sender, int lineNumber, string errorMessage, string jfFileName)
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
            jfCodeErrorLog jfLog = new jfCodeErrorLog { 
                FileName = jfFileName,
                LineNumber = lineNumber,
                ErrorMessage = errorMessage,
                TypeOfError = errorType,
                Time = DateTime.Now };
            string jsonData = JsonConvert.SerializeObject(jfLog, Formatting.Indented);
            sr.Write(jsonData + ",\n");
            sr.Close();
        }
        #endregion

        #region Sensor Check Log
        /// <summary>
        /// this logger log when code check value of a sensor and save sensor name, its value and exact
        /// time that we check it in log file.
        /// </summary>
        /// <param name="sensorName">Its a string that contains name of sensor that we check</param>
        /// <param name="sensorValue">Its a double that is value of sensor</param>
        public static void Log(string sensorName, double sensorValue)
        {
            StreamWriter sr = OpenJsonFile(_sensorCheckFileName);
            SensorCheckLog sensorCheckLog = new SensorCheckLog { SensorName = sensorName, SensorValue = sensorValue, Time = DateTime.Now };
            string jsonData = JsonConvert.SerializeObject(sensorCheckLog, Formatting.Indented);
            sr.Write(jsonData + ",\n");
            sr.Close();
        }
        #endregion
        public static int GetCurrentLine([CallerLineNumber] int lineNumber = 0)
        {
            return lineNumber;
        }
    }
}
