using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using jf_FinalProject;

namespace jf
{

    /// <summary>
    /// Class <c>Compiler</c> is comiler of JF122. its compile code, create symbol tables
    /// save variables, constants and arrays in a searchable format and detect compile time
    /// error of file
    /// </summary>
    class Compiler
    {

        #region Token Class
        /// <summary>
        /// Class <c>Token</c> save 3 property of token. its value, number of line that this token 
        /// exist in code and index of this token in that line
        /// </summary>
        public class Token
        {

            /// <summary>
            /// Instance variable <c>value</c> is a string and represents value of <c>Token</c> 
            /// </summary>
            public string value;

            /// <summary>
            /// Instance variable <c>lineNumber</c> is an integer and represent line number of <c>Token</c>
            /// </summary>
            public int lineNumber;

            /// <summary>
            /// Instance variable <c>indexInLine</c> is an integer and represent position of <c>Token</c> in line
            /// </summary>
            public int indexInLine;

            /// <summary>
            /// this constructor initializes new token with 
            /// (<paramref name="value"/>, <paramref name="lineNumber"/>, <paramref name="indexInLine"/>)
            /// </summary>
            /// <param name="value"> is value of token</param>
            /// <param name="lineNumber"> is number of line that this token exist in code</param>
            /// <param name="indexInLine">is number of index in line that this token exist</param>
            public Token(string value, int lineNumber, int indexInLine)
            {
                this.value = value;
                this.lineNumber = lineNumber;
                this.indexInLine = indexInLine;
            }
        }
        #endregion

        #region Attributes Of Class

        /// <summary>
        /// <c>CodeFilePath</c> is a string that contains full path of jf code file
        /// </summary>
        private string codeFilePath;

        /// <summary>
        /// <c>tokens</c> is a generic list of <c>Token</c>s that contains all tokens of code 
        /// </summary>
        private List<Token> tokens;

        /// <summary>
        /// <c>lines</c> is a generic list of strings that contains lines of code without empty lines
        /// </summary>
        private List<string> lines;

        /// <summary>
        /// <c>realLines</c> is a generic list of Tuple. each Tuple contains an integer and a string.
        /// integer is number of line and string is it value
        /// this list contains empty and filled line
        /// </summary>
        private List<Tuple<int, string>> realLines;

        /// <summary>
        /// <c>variables</c> is a generic list of Tuple that contains var that user created in code
        /// each Tuple contains a string and a double. string is name of variable and double is
        /// value of it
        /// </summary>
        private List<Tuple<string, double>> variables;

        /// <summary>
        /// <c>constants</c> is a generic list of Tuple that contains R0, i, mku and data in code
        /// each Tuple contains a sring and a generic list of double. string is name of that constant
        /// list of doubles is values of that constant. for R0, i and kmu its just a singel value (a list with one value)
        /// but for data its have several values
        /// </summary>
        private List<Tuple<string, List<double>>> constants;

        /// <summary>
        /// <c>explanationSymbolTable</c> is an instance of <c>Explanation</c> class and contains symbol table of explanation part of code
        /// all lines befor BEFORE in code is explanation part. this part of code is just configuration and details about code
        /// its has no run code lines
        /// </summary>
        private Explanation explanationSymbolTable = new Explanation();

        /// <summary>
        /// <c>performableSymboltTable</c> is an instance of <c>Performable</c> class and contains symbol table of performable part of code
        /// all lines after BEFORE in code is performable part. this part of code is runnig phase of code. its use sesnsors values and create commands for hardware
        /// </summary>
        private Performable performableSymboltTable = new Performable();

        /// <summary>
        /// <c>errors</c> is a generic list of <c>CustomError</c> objects. its contains error of code. if this list contains no error, runner can work and run the code 
        /// </summary>
        private List<CustomError> errors;

        /// <summary>
        /// <c>allErrorsText</c> is a dictionary that contains errors text. we have 20 errors. 0 means no error found in code
        /// </summary>
        private Dictionary<int, string> allErrorsText = new Dictionary<int, string>()
        {
            {0,  "no error"},
            {1, "start and end block is not equal"},
            {2, "extra end of block"},
            {3, "block started but it not ended"},
            {4, "there is other lines after 'end'"},
            {5, "there is no begin in this code"},
            {6, "MODE value should be '0' or '1'"},
            {7, "R0 value should be true number"},
            {8, "I value should be true number"},
            {9, "kmu value should be true number"},
            {10, "data values should be number"},
            {11, "BEGIN should not have value"},
            {12, "there is no END in this code"},
            {13, "END should not have value"},
            {14, "VAR has extra value"},
            {15, "VAR name should be start with letter"},
            {16, "VAR value should be number"},
            {17, "READ need a value"},
            {18, "this value is not defined"},
            {19, "RESTORE has no attribute"},
            {20, "SET value sould be 'pp' or 'pm'"},
            {21, "R0 value should be a true number" },
            {22, "DATA should contains at least one number" },
            {23, "ADD value should contains variables or numbers but there is nothing!" },
            {24, "SET should contains one value" },
            {25, "SPEED should contains one value" },
            {26, "SPEED attribute should be number or variable" },
            {27, "BRAKE should contains one value" },
            {28, "BRAKE flag should be r" },
            {29, "BRAKE work with constant moment or constant pressure so first letter of attribute should be m or p" },
            {30, "in BRAKE attribute after 'm' or 'p' character we should have '=' character but we dont have"},
            {31, "BRAKE value is not number or variable name"},
            {32, "in BRAKE attribute before condition part we need 'until' keyword"},
            {33, "in BRAKE condition there is an error"},
            {34, "SPEED has 1 or 2 attribute but we have more than 2 attribute"},
            {35, "SPEED flag is 'c'"},
            {36, "WAIT condition has an error"},
            {37, "LOOP value should be number or name of a variable"},
            {38, "time of LOOP should be a number"},
            {39, "there is an error in LEND condition"},
            {40, "there is an error in IF condition"},
            {41, "WATER attribute should be '0' or '1'"},
            {42, "FAN attribute should be number"},
            {100, "too many attribute"}
        };

        #endregion

        #region Methods Of Class
        public Compiler() { }

        /// <summary>
        /// <c>getPerformableTableRoot</c> method is getter of <c>performableSymboltTable</c> root node
        /// </summary>
        /// <returns>return a Node that is root of performable symbol table</returns>
        public Node getPerformableTableRoot() { return this.performableSymboltTable.root; }

        /// <summary>
        /// <c>getRealLine</c> method is getter of <c>realLines</c> attribute
        /// </summary>
        /// <returns>list of Tuples that contains <c>realLines</c></returns>
        public List<Tuple<int, string>> getRealLine() { return this.realLines; }

        /// <summary>
        /// <c>getConstants</c> method is getter of <c>constants</c> attribute
        /// </summary>
        /// <returns>list of Tuples that contains <c>constants</c></returns>
        public List<Tuple<string, List<double>>> getConstants() { return this.constants; }

        /// <summary>
        /// <c>getExplanationSymbolTable</c> method is getter of <c>explanationSymbolTable</c> attribute
        /// </summary>
        /// <returns>an object of <c>Explanation</c> that contains <c>explanationSymbolTable</c></returns>
        public Explanation getExplanationSymbolTable() { return this.explanationSymbolTable; }

        /// <summary>
        /// <c>getVariables</c> method is getter of <c>variables</c> attribute
        /// </summary>
        /// <returns>list of Tuples that contains <c>variables</c></returns>
        public List<Tuple<string, double>> getVariables() { return this.variables; }

        /// <summary>
        /// <c>GetErrors</c> method is getter of <c>errors</c> attribute
        /// </summary>
        /// <returns>list of <c>CustomError</c> that contains <c>errors</c></returns>
        public List<CustomError> GetErrors() { return this.errors; }

        /// <summary>
        /// <c>compile</c> compile a jf code file and create all symbol tables and lists for that code
        /// (<paramref name="absolutePath"/>, <paramref name="queue"/>)
        /// </summary>
        /// <param name="absolutePath">path of code file. this path must contains file name</param>
        /// <param name="queue">this queue is peoxy between UI and backend. compiler use it to send command to UI</param>
        public void compile(string absolutePath)
        {
            #region Initialize Object Attributes
            this.codeFilePath = absolutePath;
            Tuple<int, int> startOfPerformable;
            this.tokens = new List<Token>();
            this.lines = new List<String>();
            this.realLines = new List<Tuple<int, string>>();
            this.errors = new List<CustomError>();
            this.variables = new List<Tuple<string, double>>();
            this.constants = new List<Tuple<string, List<double>>>();
            #endregion

            #region Create Tokens And Symbol Tables
            this.Lineizer(absolutePath);
            startOfPerformable = this.FillExplanation();
            this.FillPerformable(startOfPerformable);
            #endregion
        }

        /// <summary>
        /// <c>Tokenizer</c> method get a line as string and split it to its words.
        /// this method remove extra blank spaces before and after each word
        /// (<paramref name="line"/>)
        /// </summary>
        /// <param name="line">is a string and contains all words of a line</param>
        /// <returns>array of string that contains words of input line. each index of output contains one word</returns>
        public string[] Tokenizer(string line)
        {
            string[] result = new string[2];
            string[] words = line.Split(' ');
            result[0] = words[0];
            for (int i = 1; i < words.Length; i++)
            {
                result[1] += words[i].Trim();
            }
            return result;
        }

        /// <summary>
        /// <c>Lineizer</c> method open jf code file and read it line by line.
        /// its create <c>Token</c> object for each token and add it to <c>tokens</c> array
        /// </summary>
        /// <param name="absolutePath"></param>
        public void Lineizer(string absolutePath)
        {
            int lineNumber = 0;
            int realLineNumber = 0;

            #region Open Code File And Read It Line By Line
            using (StreamReader sr = new StreamReader(absolutePath))
            {
                while (sr.Peek() >= 0)
                {

                    #region Go To Next Line If Current Line Is Empty
                    string line = sr.ReadLine();
                    if (line.Length == 0)
                    {
                        this.realLines.Add(Tuple.Create(realLineNumber, ""));
                        realLineNumber++;
                        continue;
                    }
                    #endregion

                    #region Create Tokens For Each Word
                    string[] words = line.Split(' ');
                    int wordNumber = 0;
                    foreach (var word in words)
                    {

                        #region Pass Empty Words
                        if (word.Trim().Length == 0)
                        {
                            continue;
                        }
                        #endregion

                        #region Tokenize Phrases Contains <=
                        if (word.Contains("<="))
                        {
                            string temp1 = word;
                            temp1 = word.Replace("<=", " ");
                            string[] temp2 = temp1.Split(' ');
                            Token t1 = new Token(temp2[0].Trim(), lineNumber, wordNumber);
                            wordNumber++;
                            Token t2 = new Token("<=", lineNumber, wordNumber);
                            wordNumber++;
                            Token t3 = new Token(temp2[1], lineNumber, wordNumber);
                            wordNumber++;
                            this.tokens.Add(t1);
                            this.tokens.Add(t2);
                            this.tokens.Add(t3);
                            continue;
                        }
                        #endregion

                        #region Tokenize Phrases Contains >=
                        if (word.Contains(">="))
                        {
                            string temp1 = word;
                            temp1 = word.Replace(">=", " ");
                            string[] temp2 = temp1.Split(' ');
                            Token t1 = new Token(temp2[0].Trim(), lineNumber, wordNumber);
                            wordNumber++;
                            Token t2 = new Token(">=", lineNumber, wordNumber);
                            wordNumber++;
                            Token t3 = new Token(temp2[1], lineNumber, wordNumber);
                            wordNumber++;
                            this.tokens.Add(t1);
                            this.tokens.Add(t2);
                            this.tokens.Add(t3);
                            continue;
                        }
                        #endregion

                        #region Tokenize Phrases Contains =
                        if (word.Contains("="))
                        {
                            string temp1 = word;
                            temp1 = word.Replace("=", " ");
                            string[] temp2 = temp1.Split(' ');
                            Token t1 = new Token(temp2[0].Trim(), lineNumber, wordNumber);
                            wordNumber++;
                            Token t2 = new Token("=", lineNumber, wordNumber);
                            wordNumber++;
                            Token t3 = new Token(temp2[1], lineNumber, wordNumber);
                            wordNumber++;
                            this.tokens.Add(t1);
                            this.tokens.Add(t2);
                            this.tokens.Add(t3);
                            continue;
                        }
                        #endregion

                        #region Tokenize Phrases Contains <
                        if (word.Contains("<"))
                        {
                            string temp1 = word;
                            temp1 = word.Replace("<", " ");
                            string[] temp2 = temp1.Split(' ');
                            Token t1 = new Token(temp2[0].Trim(), lineNumber, wordNumber);
                            wordNumber++;
                            Token t2 = new Token("<", lineNumber, wordNumber);
                            wordNumber++;
                            Token t3 = new Token(temp2[1], lineNumber, wordNumber);
                            wordNumber++;
                            this.tokens.Add(t1);
                            this.tokens.Add(t2);
                            this.tokens.Add(t3);
                            continue;
                        }
                        #endregion

                        #region Tokenize Phrases Contains >
                        if (word.Contains(">"))
                        {
                            string temp1 = word;
                            temp1 = word.Replace(">", " ");
                            string[] temp2 = temp1.Split(' ');
                            Token t1 = new Token(temp2[0].Trim(), lineNumber, wordNumber);
                            wordNumber++;
                            Token t2 = new Token(">", lineNumber, wordNumber);
                            wordNumber++;
                            Token t3 = new Token(temp2[1], lineNumber, wordNumber);
                            wordNumber++;
                            this.tokens.Add(t1);
                            this.tokens.Add(t2);
                            this.tokens.Add(t3);
                            continue;
                        }
                        #endregion

                        #region Tokenize Other Phrases That Contains Only One Word
                        Token t = new Token(word.Trim(), lineNumber, wordNumber);
                        this.tokens.Add(t);
                        #endregion

                        wordNumber++;
                    }
                    #endregion

                    #region Add Line To List Of Lines
                    this.lines.Add(line.Trim());
                    this.realLines.Add(Tuple.Create(realLineNumber, line.Trim()));
                    #endregion

                    lineNumber++;
                    realLineNumber++;
                }
            }
            #endregion
        }

        /// <summary>
        /// <c>checkForConditionError</c> method check conditions for errors
        /// (<paramref name="condition"/>)
        /// </summary>
        /// <param name="condition">is a string that contains operand1, operand2 and comparision operator</param>
        /// <returns>return false if we have error otherwise return true</returns>
        public bool checkForConditionError(string condition)
        {
            // each condtion has 2 operand
            string operand1;
            string operand2;

            // each condition has one operator
            string comparisionOperator = "";

            /* operand1 and operand2 are string, we should convert them to double and for this
             * purpose we need two double variables
             */
            double valueOfOperand1;
            double valueOfOperand2;

            #region check comarision operator
            // fill comparisionOperator variable
            if (condition.Contains("<="))
            {
                comparisionOperator = "<=";
            }
            else if (condition.Contains(">="))
            {
                comparisionOperator = ">=";
            }
            else if (condition.Contains("="))
            {
                comparisionOperator = "=";
            }
            else if (condition.Contains(">"))
            {
                comparisionOperator = ">";
            }
            else if (condition.Contains("<"))
            {
                comparisionOperator = "<";
            }
            else
            {
                return false;
            }
            #endregion

            // split condition string based on comparisionOperator variable
            string[] temp = condition.Split(new string[] { comparisionOperator }, StringSplitOptions.None);

            // first string gonna be our first operand
            operand1 = temp[0].Trim();

            // second string gonna be our second operand
            operand2 = temp[1].Trim();

            #region check operand 1
            /* first operand can be a double (pure number)
             * or a string. this if fill valueOfOperand1 if our first operand be a 
             * number. its just convert it to double
             */
            if (!Char.IsLetter(operand1[0]))
            {
                if (double.TryParse(operand1, out valueOfOperand1) == false)
                {
                    return false;
                }
            }
            else
            {
                bool isItInDefined = false;
                string[] sensorsNameList = { "p", "m", "n", "f", "t1", "t2", "t3", "time" };
                foreach (Tuple<string, double> t in this.variables)
                {
                    if (t.Item1 == operand1)
                    {
                        isItInDefined = true;
                        break;
                    }
                }
                foreach (string sensorName in sensorsNameList)
                {
                    if (sensorName == operand1.ToLower())
                    {
                        isItInDefined = true;
                        break;
                    }
                }
                if (isItInDefined == false)
                {
                    return false;
                }
            }
            #endregion

            #region check operand 2
            // repeat what we did for operand 1
            if (!Char.IsLetter(operand2[0]))
            {
                if (double.TryParse(operand2, out valueOfOperand2) == false)
                {
                    return false;
                }
            }
            else
            {
                bool isItInDefined = false;
                string[] sensorsNameList = { "p", "m", "n", "f", "t1", "t2", "t3", "time" };
                foreach (Tuple<string, double> t in this.variables)
                {
                    if (t.Item1 == operand2)
                    {
                        isItInDefined = true;
                        break;
                    }
                }
                foreach (string sensorName in sensorsNameList)
                {
                    if (sensorName == operand2.ToLower())
                    {
                        isItInDefined = true;
                        break;
                    }
                }
                if (isItInDefined == false)
                {
                    return false;
                }
            }
            #endregion

            return true;
        }

        /// <summary>
        /// <c>chechForError</c> method detect all compile time errors in code and add them all to <c>errors</c>
        /// (<paramref name="identifierAndAttribute"/>, <paramref name="isPerformable"/>, <paramref name="lineNumber"/>)
        /// </summary>
        /// <param name="identifierAndAttribute"> is an array of string that contains identifier and attribute. attribute part is optional</param>
        /// <param name="isPerformable"> is a boolean. if token belong to performable part of code, this flag will be true else it gonna be false</param>
        /// <param name="lineNumber"> is an integer and contains number of line of this token</param>
        private void chechForError(string[] identifierAndAttribute, bool isPerformable, int lineNumber)
        {
            #region Detect Errors in Explanation Part Of Code
            if (!isPerformable)
            {
                switch (identifierAndAttribute[0].ToLower())
                {
                    #region Mode Keyword Errors
                    case "mode":

                        // mode attribute should be 0 or 1. other numbers are not valid
                        if (identifierAndAttribute[1] != "0" && identifierAndAttribute[1] != "1")
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[6]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[6], this.codeFilePath);
                            break;
                        }

                        /* if identifier is mode keywords, its just have one attribute so length of 
                         * identifierAndAttribute should be 2. if it be more than 2, its an error 
                         */
                        if (identifierAndAttribute.Length > 2)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[100], this.codeFilePath);
                            break;
                        }

                        // mode is a constant of code and it should save in constants list
                        List<double> constValue = new List<double>();
                        constValue.Clear();
                        constValue.Add(double.Parse(identifierAndAttribute[1]));
                        this.constants.Add(Tuple.Create(identifierAndAttribute[0], constValue));
                        break;
                    #endregion

                    #region r0 Keyword Errors
                    case "r0":
                        double number;

                        /* attribute of r0 keyword should be a number, so we check it if we can 
                         *  parse it no number or not. if its not possible we find an error 
                         */
                        if (double.TryParse(identifierAndAttribute[1], out number) == false)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[7]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[7], this.codeFilePath);
                            break;
                        }

                        /* if identifier is r0 keywords, its just have one attribute so length of 
                         * identifierAndAttribute should be 2. if it be more than 2, its an error 
                         */
                        if (identifierAndAttribute.Length > 2)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[100], this.codeFilePath);
                            break;
                        }

                        // r0 is a constant of code and it should save in constants list
                        constValue = new List<double>();
                        constValue.Clear();
                        constValue.Add(double.Parse(identifierAndAttribute[1]));

                        // r0 value should be a true number and it can't be negetive number
                        if (constValue[0] < 0)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[21]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[21], this.codeFilePath);
                            break;
                        }

                        this.constants.Add(Tuple.Create(identifierAndAttribute[0], constValue));
                        break;
                    #endregion

                    #region i Keyword Errors
                    case "i":

                        /* attribute of i keyword should be a number, so we check it if we can 
                         * parse it no number or not. if its not possible we find an error 
                         */
                        if (double.TryParse(identifierAndAttribute[1], out number) == false)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[8]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[8], this.codeFilePath);
                            break;
                        }

                        /* if identifier is i keywords, its just have one attribute so length of 
                         * identifierAndAttribute should be 2. if it be more than 2, its an error 
                         */
                        if (identifierAndAttribute.Length > 2)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[100], this.codeFilePath);
                            break;
                        }

                        // i is a constant of code and it should save in constants list
                        constValue = new List<double>();
                        constValue.Clear();
                        constValue.Add(double.Parse(identifierAndAttribute[1]));
                        this.constants.Add(Tuple.Create(identifierAndAttribute[0], constValue));
                        break;
                    #endregion

                    #region kmu Keyword Errors
                    case "kmu":

                        /* attribute of kmu keyword should be a number, so we check it if we can 
                         * parse it no number or not. if its not possible we find an error 
                         */
                        if (double.TryParse(identifierAndAttribute[1], out number) == false)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[9]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[9], this.codeFilePath);
                            break;
                        }

                        /* if identifier is kmu keywords, its just have one attribute so length of 
                         * identifierAndAttribute should be 2. if it be more than 2, its an error 
                         */
                        if (identifierAndAttribute.Length > 2)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[100], this.codeFilePath);
                            break;
                        }

                        // kmu is a constant of code and it should save in constants list
                        constValue = new List<double>();
                        constValue.Clear();
                        constValue.Add(double.Parse(identifierAndAttribute[1]));
                        this.constants.Add(Tuple.Create(identifierAndAttribute[0], constValue));
                        break;
                    #endregion

                    #region data Keyword Errors
                    case "data":

                        /* data keyword has a list of number and this list is seperated
                         * from data keyword with one blank space, if there is no list of numbers
                         * its an error and we check it here
                         */
                        if (identifierAndAttribute.Length < 2 || identifierAndAttribute[1] == null)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[22]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[22], this.codeFilePath);
                            break;
                        }

                        /* attribute of data keyword is one or more numbers that are separated by ,
                         * so we should split them by , 
                         * then we should check all of them. if they can not be converted to number 
                         * we are face with an error
                         */
                        string[] temp = identifierAndAttribute[1].Split(',');
                        constValue = new List<double>();
                        constValue.Clear();
                        foreach (string s in temp)
                        {
                            if (double.TryParse(s, out number) == false)
                            {
                                if (this.allErrorsText.ContainsKey(10) == false)
                                {
                                    this.allErrorsText.Add(10, String.Format("'{0}' value is not number in data, its should be number", s));
                                }
                                else
                                {
                                    this.allErrorsText[10] = String.Format("'{0}' value is not number in data, its should be number", s);
                                }
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[10]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[10], this.codeFilePath);
                                break;
                            }
                            constValue.Add(double.Parse(s));
                        }

                        // at last, we should add data to constant list
                        this.constants.Add(Tuple.Create(identifierAndAttribute[0], constValue));
                        break;
                    #endregion

                    #region begin Keyword Errors
                    case "begin":
                        this.errors.Add(new CustomError(lineNumber, this.allErrorsText[11]));
                        jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[11], this.codeFilePath);
                        break;
                        #endregion
                }
            }
            #endregion


            #region Detect Errors in Performable Part Of Code
            else
            {
                switch (identifierAndAttribute[0].ToLower())
                {

                    #region var Keyword Errors
                    case "var":

                        /* var keyword have 2 attribute that are seperated by ,
                            * we should split them by ,
                            */
                        string[] temp = identifierAndAttribute[1].Split(',');

                        // var keyword have 2 attribute, more than 2 attribute is an error
                        if (temp.Length != 2)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[14]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[14], this.codeFilePath);
                            break;
                        }

                        /* first attribute is name of varibale and name of varibale cant start 
                            * with number. if it start with number we have an error
                            */
                        if (!Char.IsLetter(temp[0][0]))
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[15]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[15], this.codeFilePath);
                            break;
                        }

                        /* second attribute is value of variable and it should be a number
                            * if we cant parse it to number we have an error
                            */
                        double number;
                        if (double.TryParse(temp[1], out number) == false)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[16]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[16], this.codeFilePath);
                            break;
                        }

                        // at last we should add it to list of variables
                        this.variables.Add(Tuple.Create(temp[0], double.Parse(temp[1])));
                        break;
                    #endregion

                    #region read Keyword Errors
                    case "read":

                        /* read keyword just have one attribute, if length of identifierAndAttribute is
                            * one, its means that we have an error
                            */
                        if (identifierAndAttribute.Length == 1)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[17]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[17], this.codeFilePath);
                            break;
                        }

                        /* attribute of read keyword should be name of a variable
                            * we check it 
                            */
                        bool isItInDefined = false;
                        foreach (Tuple<string, double> t in this.variables)
                        {
                            if (t.Item1 == identifierAndAttribute[1])
                            {
                                isItInDefined = true;
                                break;
                            }
                        }
                        if (isItInDefined == false)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[18]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[18], this.codeFilePath);
                            break;
                        }
                        break;
                    #endregion

                    #region restore Keyword Errors
                    case "restore":

                        /* restore keyword has no attribute, so we check length of identifierAndAttribute
                         * its should be 1, if its more than 1 we have an error
                         */
                        if (identifierAndAttribute.Length > 1)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[19]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[19], this.codeFilePath);
                        }
                        break;
                    #endregion

                    #region add Keyword Errors
                    case "add":

                        /* add keyword has a list of objects (this list has 2 value, first one should 
                         * be a variable and second one can be variable or number) and this list is 
                         * seperated from add keyword with one blank space, if there is no list of 
                         * objects its an error and we check it here
                         */
                        if (identifierAndAttribute.Length != 2 || identifierAndAttribute[1] == null)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[23]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[23], this.codeFilePath);
                            break;
                        }

                        /* add keyword has 2 attribute that are seperated by ,
                         * we should split it by ,
                         */
                        temp = identifierAndAttribute[1].Split(',');

                        /* first attribute should be a varibale name, we check first letter of it
                         * and it should be an alphabet not number
                         */
                        if (!Char.IsLetter(temp[0][0]))
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[15]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[15], this.codeFilePath);
                            break;
                        }

                        // then we check that variable exist in list of variables
                        isItInDefined = false;
                        foreach (Tuple<string, double> t in this.variables)
                        {
                            if (t.Item1 == temp[0])
                            {
                                isItInDefined = true;
                                break;
                            }
                        }
                        if (isItInDefined == false)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[18]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[18], this.codeFilePath);
                            break;
                        }

                        /* second attribute can be variable or number, first we check if first letter of it
                         * not be alphabet, so its a number and we should be able to parse it to number
                         * if we cant do that we have an error
                         */
                        if (!Char.IsLetter(temp[1][0]))
                        {
                            if (double.TryParse(temp[1], out number) == false)
                            {
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[16]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[16], this.codeFilePath);
                                break;
                            }
                        }
                        else
                        {
                            /* if first letter of second attribute is not number, its a variable
                             * so we should be able to find it in our list of variables
                             * otherwise we have an error
                             */
                            isItInDefined = false;
                            foreach (Tuple<string, double> t in this.variables)
                            {
                                if (t.Item1 == temp[1])
                                {
                                    isItInDefined = true;
                                    break;
                                }
                            }
                            if (isItInDefined == false)
                            {
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[18]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[18], this.codeFilePath);
                                break;
                            }
                        }
                        break;
                    #endregion

                    #region set Keyword Errors
                    case "set":

                        /* set keyword has a  value and this value is seperated from set keyword 
                         * with one blank space, if there is no value its an error and we check it here
                         */
                        if (identifierAndAttribute.Length != 2 || identifierAndAttribute[1] == null)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[24]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[24], this.codeFilePath);
                            break;
                        }

                        /* set keyword has one attribute and it should be pp or pm
                         * otherwise we have an error
                         */
                        if (identifierAndAttribute[1].ToLower() != "pp" && identifierAndAttribute[1].ToLower() != "pm")
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[20]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[20], this.codeFilePath);
                        }
                        break;
                    #endregion

                    #region speed Keyword Errors
                    case "speed":
                        temp = identifierAndAttribute[1].Split(',');
                        /* speed attribute can be 1 or 2, if we have more attribute
                         * we have an error
                         */
                        if (temp.Length > 2)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[34]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[34], this.codeFilePath);
                            break;
                        }

                        // speed flag is 'c', other flags are illegal and we have an error
                        if (temp.Length == 2)
                        {
                            if (temp[1].ToLower() != "c")
                            {
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[35]));
                                break;
                            }
                        }
                        //identifierAndAttribute[1] = identifierAndAttribute[1].Replace(",c", "");

                        /* speed keyword has a value and this value is seperated from speed keyword 
                         * with one blank space, if there is no value its an error and we check it here
                         */
                        if (identifierAndAttribute.Length != 2 || identifierAndAttribute[1] == null)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[25]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[25], codeFilePath);
                            break;
                        }

                        /* attribute can be variable or number, first we check if first letter of it
                         * not be alphabet, so its a number and we should be able to parse it to number
                         * if we cant do that we have an error
                         */
                        if (!Char.IsLetter(temp[0][0]))
                        {
                            if (double.TryParse(temp[0], out number) == false)
                            {
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[26]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[26], this.codeFilePath);
                                break;
                            }
                        }
                        else
                        {
                            /* if first letter of second attribute is not number, its a variable
                             * so we should be able to find it in our list of variables
                             * otherwise we have an error
                             */
                            isItInDefined = false;
                            foreach (Tuple<string, double> t in this.variables)
                            {
                                if (t.Item1 == identifierAndAttribute[1])
                                {
                                    isItInDefined = true;
                                    break;
                                }
                            }
                            if (isItInDefined == false)
                            {
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[18]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[18], this.codeFilePath);
                                break;
                            }
                        }
                        break;
                    #endregion

                    #region break Keyword Errors
                    case "brake":

                        /* brake keyword has a value and this value is seperated from brake keyword 
                         * with one blank space, if there is no value its an error and we check it here
                         */
                        if (identifierAndAttribute.Length != 2 || identifierAndAttribute[1] == null)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[27]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[27], this.codeFilePath);
                            break;
                        }

                        temp = identifierAndAttribute[1].Split(',');

                        /* check if brake has flag, if flag exist it should be r charactar
                         * otherwise its an error
                         */
                        if (temp.Length == 3)
                        {
                            if (temp[2] != "r")
                            {
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[28]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[28], this.codeFilePath);
                                break;
                            }
                        }

                        /* brake work with constant moment or constant pressure
                         * so its should start with 'p' or 'm', if not we have an error
                         */
                        if (!(temp[0][0] == 'p' || temp[0][0] == 'm'))
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[29]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[29], this.codeFilePath);
                            break;
                        }

                        /* after 'p' or 'm' character we should have '=' character,
                         * if not we have an error
                         */
                        if (temp[0][1] != '=')
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[30]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[30], this.codeFilePath);
                            break;
                        }

                        /* after '=' character, we should have a number or variable,
                         * we check it and if we dont have them we have an error
                         */
                        if (temp[0][0] == 'p')
                        {
                            temp[0] = temp[0].Replace("p=", "");
                        }
                        else
                        {
                            temp[0] = temp[0].Replace("m=", "");
                        }
                        temp[0] = temp[0].Trim();
                        if (!Char.IsLetter(temp[0][0]))
                        {
                            if (double.TryParse(temp[0], out number) == false)
                            {
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[31]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[31], this.codeFilePath);
                                break;
                            }
                        }
                        else
                        {
                            /* if first letter is not number, its a variable
                             * so we should be able to find it in our list of variables
                             * otherwise we have an error
                             */
                            isItInDefined = false;
                            foreach (Tuple<string, double> t in this.variables)
                            {
                                if (t.Item1 == temp[0])
                                {
                                    isItInDefined = true;
                                    break;
                                }
                            }
                            if (isItInDefined == false)
                            {
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[31]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[31], this.codeFilePath);
                                break;
                            }
                        }

                        /* second value in attribute should be start with UNTIL or until
                         * if it doesn't start with that we have an error
                         */
                        temp[1] = temp[1].ToLower();
                        if (!temp[1].StartsWith("until"))
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[32]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[32], this.codeFilePath);
                            break;
                        }

                        /* after 'until' keyword we should have a condition
                         * if we dont have condition we have an error
                         */
                        temp[1] = temp[1].Replace("until", "");
                        temp[1] = temp[1].Trim();
                        if (!this.checkForConditionError(temp[1]))
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[33]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[33], this.codeFilePath);
                            break;
                        }
                        break;
                    #endregion

                    #region wait Keyword Errors
                    case "wait":

                        /* if identifier is wait keywords, its just have one attribute so length of 
                         * identifierAndAttribute should be 2. if it be more than 2, its an error 
                         */
                        string[] temp1 = identifierAndAttribute[1].Split(',');
                        if (temp1.Length > 1)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[100], this.codeFilePath);
                            break;
                        }

                        // check condition of wait with checkForConditionError method
                        if (!this.checkForConditionError(identifierAndAttribute[1]))
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[36]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[36], this.codeFilePath);
                            break;
                        }
                        break;
                    #endregion

                    #region stop Keyword Errors
                    case "stop":
                        // stop keyword has no attribute, if it has attribute we have an error
                        if (identifierAndAttribute.Length != 1)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[100], this.codeFilePath);
                            break;
                        }
                        break;
                    #endregion

                    #region loop Keyword Errors
                    case "loop":
                        // loop keyword has no attribute, if it has attribute we have an error
                        string[] temp2 = identifierAndAttribute[1].Split(',');
                        if (temp2.Length > 1)
                        {
                            this.errors.Add(new CustomError(lineNumber, this.allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[100], this.codeFilePath);
                            break;
                        }

                        /* attribute can be variable or number, first we check if first letter of it
                         * not be alphabet, so its a number and we should be able to parse it to number
                         * if we cant do that we have an error
                         */
                        if (!Char.IsLetter(identifierAndAttribute[1][0]))
                        {
                            if (double.TryParse(identifierAndAttribute[1], out number) == false)
                            {
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[37]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[37], this.codeFilePath);
                                break;
                            }
                        }
                        else
                        {
                            /* if first letter of attribute is not number, its a variable
                             * so we should be able to find it in our list of variables
                             * otherwise we have an error
                             */
                            isItInDefined = false;
                            foreach (Tuple<string, double> t in this.variables)
                            {
                                if (t.Item1 == identifierAndAttribute[1])
                                {
                                    isItInDefined = true;
                                    break;
                                }
                            }
                            if (isItInDefined == false)
                            {
                                this.errors.Add(new CustomError(lineNumber, this.allErrorsText[37]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[37], this.codeFilePath);
                                break;
                            }
                        }
                        break;
                    #endregion

                    #region lend Keyword Errors
                    case "lend":
                        // loop keyword has no attribute, if it has attribute we have an error
                        if (identifierAndAttribute.Length > 1)
                        {
                            if (identifierAndAttribute[1] != null)
                            {
                                string[] temp3 = identifierAndAttribute[1].Split(',');
                                if (temp3.Length > 2)
                                {
                                    this.errors.Add(new CustomError(lineNumber, this.allErrorsText[100]));
                                    jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[100], this.codeFilePath);
                                    break;
                                }
                                /* if we have 2 attribute we should check both of them,
                         * first one should be a number and second one should be a valid condition
                         */
                                if (temp3.Length == 2)
                                {
                                    if (double.TryParse(temp3[0], out number) == false)
                                    {
                                        this.errors.Add(new CustomError(lineNumber, this.allErrorsText[38]));
                                        jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[38], this.codeFilePath);
                                        break;
                                    }
                                    temp3[1] = temp3[1].ToLower().Replace("while", "");
                                    if (!checkForConditionError(temp3[1]))
                                    {
                                        this.errors.Add(new CustomError(lineNumber, this.allErrorsText[39]));
                                        jf_FinalProject.Logger.Logger.Log(this, lineNumber, this.allErrorsText[39], this.codeFilePath);
                                        break;
                                    }
                                }

                                /* if our attribute contains while keyword we have a loop with condition
                                 * and we check its condition
                                 */
                                else if (identifierAndAttribute[1].ToLower().Contains("while"))
                                {
                                    if (!this.checkForConditionError(identifierAndAttribute[1].ToLower().Replace("while", "").Trim()))
                                    {
                                        errors.Add(new CustomError(lineNumber, allErrorsText[39]));
                                        jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[39], this.codeFilePath);
                                        break;
                                    }
                                }

                                // if our attribute is a number we should check that its a valid number
                                else if (identifierAndAttribute.Length == 2)
                                {
                                    if (double.TryParse(identifierAndAttribute[1], out number) == false)
                                    {
                                        errors.Add(new CustomError(lineNumber, allErrorsText[38]));
                                        jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[38], this.codeFilePath);
                                        break;
                                    }
                                }
                            }
                        }
                        break;
                    #endregion

                    #region if Keyword Errors
                    case "if":
                        // if keyword has one attribute, if it has more attribute we have an error
                        if (identifierAndAttribute.Length != 2)
                        {
                            errors.Add(new CustomError(lineNumber, allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[100], this.codeFilePath);
                            break;
                        }
                        string[] temp4 = identifierAndAttribute[1].Split(',');
                        if (temp4.Length > 1)
                        {
                            errors.Add(new CustomError(lineNumber, allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[100], this.codeFilePath);
                            break;
                        }

                        /* attribute of if keyword is a condition so we check this condition
                         * to be a valid condition
                         */
                        if (!checkForConditionError(identifierAndAttribute[1]))
                        {
                            errors.Add(new CustomError(lineNumber, allErrorsText[40]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[40], this.codeFilePath);
                            break;
                        }
                        break;
                    #endregion

                    #region endif Keyword Errors
                    case "endif":
                        // endif keyword has no attribute, if it has attribute we have an error
                        if (identifierAndAttribute.Length != 1)
                        {
                            if (identifierAndAttribute[1] != null)
                            {
                                errors.Add(new CustomError(lineNumber, allErrorsText[100]));
                                jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[100], this.codeFilePath);
                                break;
                            }
                        }
                        break;
                    #endregion

                    #region water Keyword Errors
                    case "water":
                        // water keyword has just one attribute, if it has attribute we have an error
                        if (identifierAndAttribute.Length != 2)
                        {
                            errors.Add(new CustomError(lineNumber, allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[100], this.codeFilePath);
                            break;
                        }
                        string[] temp5 = identifierAndAttribute[1].Split(',');
                        if (temp5.Length > 1)
                        {
                            errors.Add(new CustomError(lineNumber, allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[100], this.codeFilePath);
                            break;
                        }
                        /* attribute of water should be a number and its just can be 
                         * 0 or 1, otherwise we have an error
                         */
                        if (!(identifierAndAttribute[1] == "0" || identifierAndAttribute[1] == "1"))
                        {
                            errors.Add(new CustomError(lineNumber, allErrorsText[41]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[41], this.codeFilePath);
                            break;
                        }
                        break;

                    #endregion

                    #region fan Keyword Errors
                    case "fan":
                        // fan keyword has just one attribute, if it has attribute we have an error
                        if (identifierAndAttribute.Length != 2)
                        {
                            errors.Add(new CustomError(lineNumber, allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[100], this.codeFilePath);
                            break;
                        }
                        string[] temp6 = identifierAndAttribute[1].Split(',');
                        if (temp6.Length > 1)
                        {
                            errors.Add(new CustomError(lineNumber, allErrorsText[100]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[100], this.codeFilePath);
                            break;
                        }

                        // attribute of fan is a number, we check it here
                        if (double.TryParse(identifierAndAttribute[1], out number) == false)
                        {
                            errors.Add(new CustomError(lineNumber, allErrorsText[38]));
                            jf_FinalProject.Logger.Logger.Log(this, lineNumber, allErrorsText[38], this.codeFilePath);
                            break;
                        }
                        break;
                        #endregion
                }
            }
            #endregion
        }

        /// <summary>
        /// <c>FillPerformable</c> method create symbol table of performable part of code
        /// this symbol table is a tree
        /// (<paramref name="startLineNumber"/>)
        /// </summary>
        /// <param name="startLineNumber">is a Tuple of 2 integer. first integer is number of 
        /// line of begin keyword with blank lines. second integer is same without blanck lines</param>
        public void FillPerformable(Tuple<int, int> startLineNumber)
        {

            #region Check For Error
            /* if startLineNumber is null or its values are less than 0, its means that there is no
             * begin keyword in code and we have an error
             */
            if (startLineNumber == null || startLineNumber.Item1 < 0)
            {
                errors.Add(new CustomError(startLineNumber.Item1, allErrorsText[5]));
                jf_FinalProject.Logger.Logger.Log(this, startLineNumber.Item1, allErrorsText[5], this.codeFilePath);
            }
            #endregion

            #region Initialize Variables And Create Root Of Tree
            int i;
            int realLineCounter = startLineNumber.Item1;
            Stack parentStack = new Stack();
            bool isEnded = false;
            string firstLine = realLines[startLineNumber.Item1].Item2;
            string[] result = Tokenizer(firstLine);
            performableSymboltTable.root = new Node(result[0], result[1], realLineCounter);
            parentStack.Push(performableSymboltTable.root);
            #endregion

            #region Create Symbol Table Tree
            /* iterate lines from begin keyword to end keyword. performable part of code
             * is last part of code
             */
            for (i = startLineNumber.Item1 + 1; i < realLines.Count; i++)
            {
                string line = realLines[i].Item2;

                // pass empty lines
                if (line.Length == 0)
                {
                    continue;
                }

                // tokenize line
                result = Tokenizer(line);

                /* check current line for error. if chechForError method find any error,
                 * it will add it to errors list
                 */
                this.chechForError(result, true, i);
                Node current;

                /* loop and if force us to go in depth. so when we see lend or endif keyword we 
                 * should get back from depth. all other keywords are reqular and should add to breadth
                 */
                if (result[0].ToLower() != "loop" && result[0].ToLower() != "if" && result[0].ToLower() != "lend" && result[0].ToLower() != "endif")
                {
                    current = new Node(result[0], result[1], i)
                    {
                        parent = (Node)parentStack.Peek()
                    };
                    Node temp = (Node)parentStack.Pop();
                    temp.child.Add(current);
                    parentStack.Push(temp);
                }

                /* if we see loop or if keyword we should go in depth. for this purpose we should add it
                 * to parent stack
                 */
                else if (result[0].ToLower() == "loop" || result[0].ToLower() == "if")
                {
                    current = new Node(result[0], result[1], i)
                    {
                        parent = (Node)parentStack.Peek()
                    };
                    Node temp = (Node)parentStack.Pop();
                    temp.child.Add(current);
                    parentStack.Push(temp);
                    parentStack.Push(current);
                }

                /* if we see lend or endif keyword, we should get back from depth and continue
                 * our iteration in breadth. for this purpose we should pop parent stack
                 */
                else if (result[0].ToLower() == "lend" || result[0].ToLower() == "endif")
                {
                    Node temp = (Node)parentStack.Peek();

                    /* here we check for an error, if block started with if, its should end
                     * with endif. if block started with loop, its should end with lend. 
                     * otherwise we have an error
                     */
                    if (result[0].ToLower() == "lend" && temp.identifier == "if")
                    {
                        errors.Add(new CustomError(i, allErrorsText[1]));
                        jf_FinalProject.Logger.Logger.Log(this, i, allErrorsText[1], this.codeFilePath);
                    }
                    if (result[0].ToLower() == "endif" && temp.identifier == "loop")
                    {
                        errors.Add(new CustomError(i, allErrorsText[1]));
                        jf_FinalProject.Logger.Logger.Log(this, i, allErrorsText[1], this.codeFilePath);
                    }

                    /* this part of code check for extra end of block keyword
                     * extra end of block is an error
                     */
                    if (temp == performableSymboltTable.root)
                    {
                        errors.Add(new CustomError(i, allErrorsText[2]));
                        jf_FinalProject.Logger.Logger.Log(this, i, allErrorsText[2], this.codeFilePath);
                    }
                    current = new Node(result[0], result[1], i)
                    {
                        parent = (Node)parentStack.Peek()
                    };
                    Node temp2 = (Node)parentStack.Pop();
                    temp2.child.Add(current);
                    temp2 = (Node)parentStack.Peek();

                }

                // end keyword is end of code. its end compiler but before that we check an error
                else if (result[0].ToLower() == "end")
                {
                    /* this part of code check for early end keyword. one or more blocks started 
                     * and before they end, end keyword occured in code. its an error
                     * 
                     * for example:
                     * loop 5
                     * ...
                     * ...
                     * end
                     * in this exmaple loop block started but before its end and compiler see lend
                     * keyword, end keyword accured and we faced with an error
                     */
                    if ((Node)parentStack.Peek() == performableSymboltTable.root)
                    {
                        this.errors.Add(new CustomError(i, allErrorsText[3]));
                        jf_FinalProject.Logger.Logger.Log(this, i, allErrorsText[3], this.codeFilePath);
                    }
                    break;
                }
                if (i == this.realLines.Count - 1) isEnded = true;
            }
            while (true)
            {
                if (i < realLines.Count)
                {
                    if (realLines[i].Item2.Length == 0)
                    {
                        i++;
                        continue;
                    }
                    else
                    {
                        isEnded = false;
                        break;
                    }
                }
                else
                {
                    isEnded = true;
                    break;
                }
            }
            #endregion

            // if there is another line after end keyword, its an error
            if (isEnded == false)
            {
                this.errors.Add(new CustomError(i, allErrorsText[4]));
                jf_FinalProject.Logger.Logger.Log(this, i, allErrorsText[4], this.codeFilePath);
            }
            string[] lastLine = this.Tokenizer(lines[lines.Count - 1]);
            if (lastLine[0].ToLower() != "end")
            {
                this.errors.Add(new CustomError(i, allErrorsText[12]));
                jf_FinalProject.Logger.Logger.Log(this, i, allErrorsText[12], this.codeFilePath);
            }

            if (lastLine.Length > 2)
            {
                this.errors.Add(new CustomError(i, allErrorsText[13]));
                jf_FinalProject.Logger.Logger.Log(this, i, allErrorsText[13], this.codeFilePath);
            }
        }

        /// <summary>
        /// <c>FillExplanation</c> method create symbol table of explanation part of code
        /// this symbol table is generic list
        /// </summary>
        /// <returns>its return a Tuple of 2 integers. first integer is number of line of
        /// begin keyword. its counts empty lines too. second integer is number of line of begin 
        /// keyword and its counts just non empty lines</returns>
        public Tuple<int, int> FillExplanation()
        {
            int lineCounter = 0;
            int realLineCounter = 0;
            foreach (Tuple<int, string> line in realLines)
            {

                // pass empty lines
                if (line.Item2.Length == 0)
                {
                    realLineCounter++;
                    continue;
                }

                /* begin keyword is end of explanation part of code
                 * when we see this keyword we should return real line number (counts empty lines too)
                 * and line number (without counting empty lines) as a Tuple
                 */
                if (line.Item2.ToLower().Equals("begin"))
                {
                    return Tuple.Create(realLineCounter, lineCounter);
                }

                string[] result = this.Tokenizer(line.Item2);
                this.chechForError(result, false, realLineCounter);
                this.explanationSymbolTable.st.insert(result[0], result[1], realLineCounter);
                lineCounter++;
                realLineCounter++;
            }

            /* return -1 means there is no begin keyword and in FillPerformable method 
             * we handle this error
             */
            return Tuple.Create(-1, -1);
        }

        #endregion

        #region Test Method
        public static void Print(Compiler c)
        {
            Console.WriteLine("##############################################################");
            Console.WriteLine(">>> lines created (empty lines deleted) : ");

            foreach (string line in c.lines)
            {
                Console.WriteLine(line);
            }

            Console.WriteLine("##############################################################");
            Console.WriteLine(">>> complete lines created by empty lines");

            foreach (var line in c.realLines)
            {
                Console.WriteLine("{0}  {1}", line.Item1, line.Item2);
            }

            Console.WriteLine("##############################################################");
            Console.WriteLine(">>> explamation part : ");

            foreach (var temp in c.explanationSymbolTable.st.table)
            {
                Console.WriteLine(String.Format("identifier name is ''{0}'' and its attribute is ''{1}''", temp[0], temp[1]));
            }

            Console.WriteLine("##############################################################");
            Console.WriteLine(">>> performable part : ");

            Node.printTree(c.performableSymboltTable.root);

            Console.WriteLine("##############################################################");
            Console.WriteLine(">>> list of all errors");

            if (c.errors.Count > 0)
            {
                foreach (CustomError error in c.errors)
                {
                    Console.WriteLine("in line {0} error message is : '{1}'", error.getLineNumber(), error.getMessage());
                }
            }
            else Console.WriteLine("there is no error");

            Console.WriteLine("##############################################################");

            Console.ReadLine();
        }
        #endregion

    }
}
