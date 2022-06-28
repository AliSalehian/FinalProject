using Newtonsoft.Json;
using System;
using System.IO;

namespace jf_FinalProject.Logger
{
    public class Logger
    {
        private readonly static string _commandSendFileName = "CommandSend.json";
        private readonly static string _developerFileName = "Developer.json";
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

        public static void Log(string command)
        {
            StreamWriter sr = OpenJsonFile(_commandSendFileName);
            CommandSendLog commandSendLog = new CommandSendLog { Command = command, Time = DateTime.Now};
            string jsonData = JsonConvert.SerializeObject(commandSendLog, Formatting.Indented);
            sr.Write(jsonData + "\n");
            sr.Close();
        }

        public static void Log(int lineNumber, string fileName, string errorMessage)
        {
            StreamWriter sr = OpenJsonFile(_developerFileName);
            DeveloperLog developerLog = new DeveloperLog { LineNumber = lineNumber, ErrorMessage = errorMessage, FileName = fileName, Time = DateTime.Now };
            string jsonData = JsonConvert.SerializeObject(developerLog, Formatting.Indented);
            sr.Write(jsonData + "\n");
            sr.Close();
        }
    }
}
