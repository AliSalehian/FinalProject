using System;

namespace jf_FinalProject.Logger
{
    public class DeveloperLog
    {
        public int LineNumber { get; set; }

        public string FileName { get; set; }

        public string ErrorMessage { get; set; }

        public DateTime Time { get; set; }
    }
}
