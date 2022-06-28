using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jf_FinalProject.Logger
{

    public enum ErrorType
    {
        Compile = 0,
        Run = 1
    }
    public class jfCodeErrorLog
    {
        public int LineNumber { get; set; }

        public string ErrorMessage { get; set; }

        public ErrorType TypeOfError { get; set; }

        public DateTime Time { get; set; }
    }
}
