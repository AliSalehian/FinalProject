namespace jf
{
    /// <summary>
    /// class <c>CustomError</c> used for save an error that occured in code
    /// </summary>
    class CustomError
    {
        #region Attribute Of Class

        /// <summary>
        /// <c>lineNumber</c> attribute is an integer that contain number of line that 
        /// error occured
        /// </summary>
        private int lineNumber;

        /// <summary>
        /// <c>message</c> attribute is text of error
        /// </summary>
        private string message;
        #endregion

        #region Constructor Of Class

        /// <summary>
        /// this constructor get <c>lineNumber</c> and <c>message</c> and create a new error
        /// (<paramref name="lineNumber"/>, <paramref name="message"/>)
        /// </summary>
        /// <param name="lineNumber">number of line that error occured</param>
        /// <param name="message">text of error</param>
        public CustomError(int lineNumber, string message){
            this.lineNumber = lineNumber;
            this.message = message;
        }
        #endregion

        #region Methods Of Class

        /// <summary>
        /// <c>getLineNumber</c> method is getter of <c>lineNumber</c>
        /// </summary>
        /// <returns>an integer that is <c>lineNumber</c> attribute</returns>
        public int getLineNumber(){ return lineNumber; }

        /// <summary>
        /// <c>getMessage</c> method is getter of <c>message</c>
        /// </summary>
        /// <returns>a string that is <c>message</c> attribute</returns>
        public string getMessage(){ return message; }
        #endregion
    }
}
