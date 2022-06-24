using System.Windows.Media;

namespace jf
{
    /// <summary>
    /// <c>Command</c> is an UI command that create by Compiler and Runner class
    /// they add an Object of this class to queue and UI use them and create user friendly 
    /// output for user
    /// </summary>
    internal class CommandEventArgs
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
        public string Type { get; set; }

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
        public int LineNumber { get; set; }

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
        public SolidColorBrush Color { get; set; }

        /// <summary>
        /// <c>newLine</c> attribute is a string and contains new line that we wanna insert or edit
        /// </summary>
        public string newLine;
        #endregion
    }
}
