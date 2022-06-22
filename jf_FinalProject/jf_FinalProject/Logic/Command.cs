using System.Drawing;

namespace jf
{
    /// <summary>
    /// <c>Command</c> is an UI command that create by Compiler and Runner class
    /// they add an Object of this class to queue and UI use them and create user friendly 
    /// output for user
    /// </summary>
    internal class Command
    {

        #region Attributes Of Class

        /// <summary>
        /// <c>type</c> attribute is a string that save type of command. we have bellow types:
        /// <list type="number">
        /// 
        /// <item>
        /// <description>create ritchbox</description>
        /// </item>
        /// 
        /// <item>
        /// <description>delete ritchbox</description>
        /// </item>
        /// 
        /// <item>
        /// <description>highlight</description>
        /// </item>
        /// 
        /// <item>
        /// <description>error</description>
        /// </item>
        /// 
        /// <item>
        /// <description>set</description>
        /// </item>
        /// 
        /// <item>
        /// <description>test</description>
        /// </item>
        /// 
        /// </list>
        /// </summary>
        private string type;

        /// <summary>
        /// <c>lineNumber</c> attribute is an integer. if command should be applied to 
        /// a specific line, this attribute should set. command types that this attribute should 
        /// set for them are: 
        /// <list type="bullet">
        /// 
        /// <item>
        /// <description>highlight</description>
        /// </item>
        /// 
        /// <item>
        /// <description>error</description>
        /// </item>
        /// 
        /// </list>
        /// </summary>
        private int lineNumber;

        /// <summary>
        /// <c>bgColor</c> attribute is an object of Color class. its should set for command with type:
        /// 
        /// <list type="bullet">
        /// 
        /// <item>
        /// <description>highlight</description>
        /// </item>
        /// 
        /// <item>
        /// <description>error</description>
        /// </item>
        /// 
        /// </list>
        /// </summary>
        //private Color bgColor;

        /// <summary>
        /// <c>newLine</c> attribute is a string and contains new line that we wanna insert or edit
        /// </summary>
        private string newLine;
        #endregion

        #region Constructors Of Class

        /// <summary>
        /// this constructor create a Command object just with type and line number
        /// (<paramref name="type"/>, <paramref name="lineNumber"/>)
        /// </summary>
        /// <param name="type">type of command</param>
        /// <param name="lineNumber">number of line</param>
        public Command(string type, int lineNumber)
        {
            this.type = type;
            this.lineNumber = lineNumber;
        }

        /// <summary>
        /// this constructor create a Command object just with type and new line
        /// (<paramref name="type"/>, <paramref name="newLine"/>)
        /// </summary>
        /// <param name="type">type of command</param>
        /// <param name="newLine">new line</param>
        public Command(string type, string newLine)
        {
            this.type = type;
            this.newLine = newLine;
        }

        /// <summary>
        /// this constructor create a Command object just with type, line number and color
        /// (<paramref name="type"/>, <paramref name="lineNumber"/>, <paramref name="bgColor"/>)
        /// </summary>
        /// <param name="type">type of command</param>
        /// <param name="lineNumber">number of line</param>
        /// <param name="bgColor">color that is an object of <c>Color</c> class</param>
        //public Command(string type, int lineNumber, Color bgColor*/)
        //{
        //    this.type = type;
        //    this.lineNumber = lineNumber;
        //    this.bgColor = bgColor;
        //}

        /// <summary>
        /// this constructor create a Command object just with type
        /// (<paramref name="type"/>)
        /// </summary>
        /// <param name="type">type of command</param>
        public Command(string type)
        {
            this.type=type;
        }
        #endregion

        #region Methods Of Class

        /// <summary>
        /// <c>getType</c> method is getter of <c>type</c>
        /// </summary>
        /// <returns>a string that is type of command</returns>
        public string getType(){ return type; }

        /// <summary>
        /// <c>setType</c> method is setter of <c>type</c>
        /// (<paramref name="type"/>)
        /// </summary>
        /// <param name="type">is a string that should be a valid type</param>
        public void setType(string type){ this.type = type; }

        /// <summary>
        /// <c>getLineNumber</c> method is getter of <c>lineNumber</c>
        /// </summary>
        /// <returns>an integer that is <c>lineNumber</c> of command</returns>
        public int getLineNumber(){ return lineNumber; }

        /// <summary>
        /// <c>setLineNumber</c> method is setter of <c>lineNumber</c>
        /// (<paramref name="lineNumber"/>)
        /// </summary>
        /// <param name="lineNumber">is an integer that is new line number of command</param>
        public void setLineNumber(int lineNumber){ this.lineNumber = lineNumber; }

        /// <summary>
        /// <c>getBgColor</c> method is getter of <c>bgColor</c>
        /// </summary>
        /// <returns> is an object of <c>Color</c> class that is color of command</returns>
        //public Color getBgColor(){ return bgColor; }

        /// <summary>
        /// <c>getNewLine</c> method is getter of <c>newLine</c>
        /// </summary>
        /// <returns>a string that is new line of command</returns>
        public string getNewLine(){ return newLine; }
        #endregion

    }
}
