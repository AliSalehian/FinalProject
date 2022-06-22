using System.Collections.Generic;

namespace jf
{
    /// <summary>
    /// class <c>SymbolTable</c> is a class that we use it to save symbol table
    /// of explanation part of code
    /// </summary>
    class SymbolTable
    {
        public List<string[]> table = new List<string[]>();
        public int lineNumber;

        public void insert(string idefName, string attribute, int lineNumber)
        {
            string[] temp = { idefName, attribute };
            this.table.Add(temp);
            this.lineNumber = lineNumber;
        }
    }
}
