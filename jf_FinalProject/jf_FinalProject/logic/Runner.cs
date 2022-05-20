using System;
using System.Collections.Generic;
using System.IO;
using System.Collections;
using System.Windows.Media;

namespace jf_FinalProject.logic
{

    /// <summary>
    /// class <c>Runner</c> is runner of JF122 code. after that <c>Compiler</c> class
    /// compile our code and found no error, runner run code line by line and create 
    /// hardware commands. runner is last part of logic of this project
    /// </summary>
    class Runner
    {

        #region Attributes Of Class

        /// <summary>
        /// <c>compiler</c> attribute is an object of <c>jf.Compiler</c> class
        /// we dont create a compiler object in <c>Runner</c> class. its created in 
        /// <c>Program</c> class and pass from <c>Program</c> to here.
        /// </summary>
        Compiler compiler;

        /// <summary>
        /// <c>commands</c> attribute is a queue that we put commands in it and UI will use it
        /// its like <c>compiler</c>attribute, a global attribute so we will not create it in this class
        /// </summary>
        Queue<Command> commands;

        /// <summary>
        /// <c>sensorHandler</c> attribute is an object of <c>SensorHandler</c> class. this object
        /// is our interface with sensors. we just read value of sensors with this object and 
        /// do nothing about all complexity about it, <c>SensorHandler</c> will handle it for us
        /// </summary>
        SensorHandler sensorHandler;

        /// <summary>
        /// <c>GREEN</c> attribute is an object of <c>Color</c> and represent green color.
        /// we use it for highlighting line of codes in UI. its a readonly attribute and cant change.
        /// </summary>
        public readonly Color GREEN = Color.FromRgb(0x4c, 0xe6, 0x00); // #4ce600 

        /// <summary>
        /// <c>RED</c> attribute is an object of <c>Color</c> and represent red color.
        /// we use it for highlighting line of codes in UI. its a readonly attribute and cant change.
        /// </summary>
        public readonly Color RED = Color.FromRgb(0xff, 0x5c, 0x33); // #ff5c33

        /// <summary>
        /// <c>ORANGE</c> attribute is an object of <c>Color</c> and represent orange color.
        /// we use it for highlighting line of codes in UI. its a readonly attribute and cant change.
        /// </summary>
        public readonly Color ORANGE = Color.FromRgb(0xff, 0xcc, 0x00);
        #endregion

        #region Constructor Of Class

        /// <summary>
        /// this constuctor get all global attributes that used by other backend classes and UI
        /// part of project
        /// (<paramref name="compiler"/>, <paramref name="commands"/>, <paramref name="sensorHandler"/>)
        /// </summary>
        /// <param name="compiler"> is an object of <c>jf.Compiler</c> class that compile code</param>
        /// <param name="commands">is a queue that shared with UI part of project</param>
        /// <param name="sensorHandler">is an object of <c>SensorHandler</c> class</param>
        public Runner(Compiler compiler, Queue<Command> commands, SensorHandler sensorHandler)
        {
            this.compiler = compiler;
            this.commands = commands;
            this.sensorHandler = sensorHandler;
        }
        #endregion

        #region Methods Of Class

        /// <summary>
        /// <c>writeCommandToFile</c> method write commands that created for hardware by runner
        /// to a text file. its a temporary file and in future its should replace with a port writer
        /// (<paramref name="text"/>)
        /// </summary>
        /// <param name="text">is a string that is a single command that we want write it in file</param>
        public void writeCommandToFile(string text)
        {
            const string commandFilePath = "../../board/commands.txt";
            using (StreamWriter sw = new StreamWriter(commandFilePath, append: true))
            {
                sw.WriteLine(text);
            }
        }

        /// <summary>
        /// <c>findData</c> method find data (array in JF122 syntax) and return it
        /// this method use list of constatns that <c>jf.Compiler</c> class created.
        /// (<paramref name="constants"/>)
        /// </summary>
        /// <param name="constants">is a generic list that created by <c>jf.Compiler</c> class and contain all constants of code.</param>
        /// <returns> a generic list of doubles that contains values of data, if there is no data in
        /// code it will return null.
        /// </returns>
        public List<double> findData(List<Tuple<string, List<double>>> constants)
        {
            for (int i = 0; i < constants.Count; i++)
            {
                if (constants[i].Item1.ToLower() == "data")
                {
                    return constants[i].Item2;
                }
            }
            return null;
        }

        /// <summary>
        /// <c>findVariable</c> method get name of a variable and find it in list of variables
        /// that comlier create it and return its value
        /// (<paramref name="variables"/>, <paramref name="valueName"/>)
        /// </summary>
        /// <param name="variables">is a generic list that created by <c>jf.Compiler</c> class and contain all variables of code.</param>
        /// <param name="valueName">is a string. its name of variable that we wanna find it.</param>
        /// <returns>a double that is value of variable that we wanna find it, if variable didnt created
        /// in code this method will return 0.
        /// </returns>
        public double findVariable(List<Tuple<string, double>> variables, string valueName)
        {
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables[i].Item1.ToLower() == valueName)
                {
                    return variables[i].Item2;
                }
            }
            return 0;
        }

        /// <summary>
        /// <c>findVariableAndChangeValue</c> method is like <c>findVariable</c> method
        /// but this method change the value of that variable with input new value.
        /// (<paramref name="variables"/>, <paramref name="variableName"/>, <paramref name="newValue"/>)
        /// </summary>
        /// <param name="variables">is a generic list of Tuple that <c>jf.Compiler</c> generate it</param>
        /// <param name="variableName">is a string that is name of variable that we are looking for it</param>
        /// <param name="newValue">is a double. its new value for variable</param>
        public void findVariableAndChangeValue(List<Tuple<string, double>> variables, string variableName, double newValue)
        {
            for (int i = 0; i < variables.Count; i++)
            {
                if (variables[i].Item1.ToLower() == variableName)
                {
                    variables[i] = Tuple.Create(variables[i].Item1, newValue);
                }
            }
        }

        /// <summary>
        /// <c>checkCondition</c> method get a condition in JF122 syntax and all variables and return
        /// a boolean.
        /// (<paramref name="condition"/>, <paramref name="variables"/>)
        /// </summary>
        /// <param name="condition">is a string that contains condition</param>
        /// <param name="variables">is a generic list of Tuple that <c>jf.Compiler</c> create it</param>
        /// <returns>a boolean. if condition is true return true else return false</returns>
        public bool checkCondition(string condition, List<Tuple<string, double>> variables)
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

            // split condition string based on comparisionOperator variable
            string[] temp = condition.Split(new string[] { comparisionOperator }, StringSplitOptions.None);

            // first string gonna be our first operand
            operand1 = temp[0].Trim();

            // second string gonna be our second operand
            operand2 = temp[1].Trim();

            /* first operand can be a double (pure number)
             * or a string. this if fill valueOfOperand1 if our first operand be a 
             * number. its just convert it to double
             */
            if (double.TryParse(operand1, out valueOfOperand1) == true)
            {
                valueOfOperand1 = double.Parse(operand1);
            }

            // if our first operand is not a double, it can be name of a sensor or a variable
            else
            {
                // fill valueOfOperand1 if our first operand is name of a sensor
                if (this.sensorHandler.checkSensorExist(operand1))
                {
                    valueOfOperand1 = this.sensorHandler.getSensor(operand1);
                }

                // fill valueOfOperand1 if our fisrt operand is name of a variable
                else
                {
                    valueOfOperand1 = this.findVariable(variables, operand1);
                }
            }

            // repeat what we did for operand 1
            if (double.TryParse(operand2, out valueOfOperand2) == true)
            {
                valueOfOperand2 = double.Parse(operand2);
            }
            else
            {
                if (this.sensorHandler.checkSensorExist(operand2))
                {
                    valueOfOperand2 = this.sensorHandler.getSensor(operand2);
                }
                else
                {
                    valueOfOperand2 = this.findVariable(variables, operand2);
                }
            }

            /* now that we have values of operand and operator,
             * calculate output is just a switch case
             */
            switch (comparisionOperator)
            {
                case "<":
                    return valueOfOperand1 < valueOfOperand2;
                case ">":
                    return valueOfOperand1 > valueOfOperand2;
                case "=":
                    return valueOfOperand1 == valueOfOperand2;
                case "<=":
                    return valueOfOperand1 <= valueOfOperand2;
                case ">=":
                    return valueOfOperand2 >= valueOfOperand1;
            }
            return false;
        }

        /// <summary>
        /// <c>run</c> method is main method of <c>jf.Runner</c> class.
        /// its run code line by line, create command for UI and hardware.
        /// </summary>
        /// <returns></returns>
        public int run()
        {
            #region Initialize Variables
            bool errorDetected = false;
            int lineCounter = 0;
            int realLineCounter = 0;
            bool isPerformable = false;
            List<CustomError> compilerErrors = compiler.GetErrors();
            Explanation explanation = compiler.getExplanationSymbolTable();
            List<Tuple<int, string>> realLines = compiler.getRealLine();
            Stack<int> loopCount = new Stack<int>();
            Stack<bool> loopStarted = new Stack<bool>();
            Stack<int> loopIndex = new Stack<int>();
            Stack<int> loopEndIndex = new Stack<int>();
            bool loopSeen = false;
            #endregion

            /* if compiler find an error in code, errorDetected value change to true
             * and main loop will not run
             */
            if (compilerErrors.Count > 0)
            {
                errorDetected = true;
            }

            /* for each error highlight that line with red color.
             * we do it with create command for UI.
             */
            foreach (CustomError error in compilerErrors)
            {
                this.commands.Enqueue(new Command("highlight", error.getLineNumber(), this.RED));
            }

            if (errorDetected)
            {
                return 0;
            }

            #region Run Explanation Part Of Code
            /* iterate explanation part of code and run it line by line
             * if compiler find any error in code, this loop will not run
             */
            while (!errorDetected)
            {
                /* begin keyword is last line in explanation part.
                 * so when this keyword come up, our iteration over explanation part
                 * should stop. before that we make isPerformable variable true.
                 * isPerformable variable tell our code to start running performable part of code
                 */
                if (realLines[realLineCounter].Item2.ToLower() == "begin")
                {
                    this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                    isPerformable = true;
                    realLineCounter++;
                    break;
                }

                // pass empty lines
                if (realLines[realLineCounter].Item2 == "")
                {
                    this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                    realLineCounter++;
                    continue;
                }

                // check if we are still in explanation part of code and there is no UI command in queue
                if (this.commands.Count == 0 && !isPerformable)
                {
                    switch (explanation.st.table[lineCounter][0].ToLower())
                    {
                        // jf122 do nothing but it should exist in top of code
                        case "jf122":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            lineCounter++;
                            realLineCounter++;
                            break;

                        /* code keyword convert a line to comment. 
                         * our compiler just pass lines that started with code keyword
                         */
                        case "code":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            lineCounter++;
                            realLineCounter++;
                            break;

                        // mode keyword create a hardware command
                        case "mode":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            this.writeCommandToFile("mode " + explanation.st.table[lineCounter][1]);
                            lineCounter++;
                            realLineCounter++;
                            break;

                        // r0, i and kmu are configs of hardware and create hardware commands
                        case "r0":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            this.commands.Enqueue(new Command("set", explanation.st.table[lineCounter][1]));
                            this.writeCommandToFile("r0 " + explanation.st.table[lineCounter][1]);
                            lineCounter++;
                            realLineCounter++;
                            break;
                        case "i":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            this.writeCommandToFile("i " + explanation.st.table[lineCounter][1]);
                            lineCounter++;
                            realLineCounter++;
                            break;
                        case "kmu":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            this.writeCommandToFile("kmu " + explanation.st.table[lineCounter][1]);
                            lineCounter++;
                            realLineCounter++;
                            break;

                        /* data is array in JF122 syntax but we dont need to do anything about it in
                         * runner. compiler handle it completly
                         */
                        case "data":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            lineCounter++;
                            realLineCounter++;
                            break;
                    }
                }
            }
            #endregion

            #region Run Performable Part Of Code

            // this part, is the most complex part of code
            if (isPerformable)
            {
                #region Initialize Variables
                List<Tuple<string, List<double>>> constants = compiler.getConstants();
                List<Tuple<string, double>> variables = compiler.getVariables();
                List<double> data = null;
                int indexOfData = 0;
                Node root = this.compiler.getPerformableTableRoot();
                Stack parentStack = new Stack();
                Stack parentIndexStack = new Stack();
                Node current = null;
                int index = 0;
                parentIndexStack.Push(0);
                parentStack.Push(root);
                DateTime start = DateTime.Now;
                DateTime end = DateTime.Now;
                Stack<bool> ifConditionSatisfied = new Stack<bool>();
                ifConditionSatisfied.Push(true);
                #endregion

                // performable part will not run if an error detected
                while (!errorDetected)
                {
                    if (loopSeen && loopStarted.Peek() == true)
                    {
                        index = loopIndex.Peek();
                        loopSeen = false;
                    }
                    else
                    {
                        index = (int)parentIndexStack.Peek();
                    }

                    Node temp = (Node)parentStack.Peek();
                    if (index > temp.child.Count - 1)
                    {
                        break;
                    }

                    current = temp.child[index];

                    switch (current.identifier.ToLower())
                    {
                        case "begin":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            break;
                        case "var":
                            if (ifConditionSatisfied.Peek())
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                realLineCounter++;
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "read":
                            if (ifConditionSatisfied.Peek())
                            {
                                if (data == null)
                                {
                                    data = this.findData(constants);
                                }
                                if (data == null)
                                {
                                    // TODO: add cross mark in UI
                                    this.commands.Enqueue(new Command("highlight error", realLineCounter, this.RED));
                                    errorDetected = true;
                                    break;
                                }
                                if (indexOfData > data.Count) indexOfData--;
                                this.findVariableAndChangeValue(variables, current.attribute, data[indexOfData]);
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                indexOfData++;
                                realLineCounter++;
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "restore":
                            if (ifConditionSatisfied.Peek())
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                indexOfData = 0;
                                realLineCounter++;
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "add":
                            if (ifConditionSatisfied.Peek())
                            {
                                double test1 = 0;
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                realLineCounter++;
                                string[] temp1 = current.attribute.Split(',');
                                double newValue = 0;
                                if (double.TryParse(temp1[1], out newValue))
                                {
                                    test1 = newValue + this.findVariable(variables, temp1[0]);
                                    this.findVariableAndChangeValue(variables, temp1[0], test1);
                                }
                                else
                                {
                                    test1 = this.findVariable(variables, temp1[1]) + this.findVariable(variables, temp1[0]);
                                    this.findVariableAndChangeValue(variables, temp1[0], test1);
                                }
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "set":
                            if (ifConditionSatisfied.Peek())
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                this.writeCommandToFile("set " + current.attribute);
                                // TODO: we should complete it later
                                realLineCounter++;
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "turn":
                            if (ifConditionSatisfied.Peek())
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                this.writeCommandToFile("turn " + current.attribute);
                                realLineCounter++;
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "fan":
                            if (ifConditionSatisfied.Peek())
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                this.writeCommandToFile("fan " + current.attribute);
                                realLineCounter++;
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "speed":
                            if (ifConditionSatisfied.Peek())
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                this.writeCommandToFile("speed " + current.attribute);
                                // TODO: command increase speed and reed speed sensor till condition satisfied. if c is exist speed should keep on condition 
                                // else turn off motor
                                realLineCounter++;
                                if (loopEndIndex.Count != 0)
                                {
                                    realLineCounter--;
                                }
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "wait":
                            if (ifConditionSatisfied.Peek())
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                if (this.checkCondition(current.attribute, variables))
                                {
                                    this.writeCommandToFile("wait condiotion satisfied");
                                }
                                else
                                {
                                    this.writeCommandToFile("wait condiotion didnt satisfied and runner waited");
                                    while (true)
                                    {
                                        if (this.checkCondition(current.attribute, variables))
                                        {
                                            break;
                                        }
                                    }
                                }
                                realLineCounter++;
                                if (loopEndIndex.Count != 0)
                                {
                                    realLineCounter--;
                                }
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "brake":
                            if (ifConditionSatisfied.Peek())
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                // TODO: command should log if r flag seted
                                string condition = current.attribute.Split(',')[1].Replace("UNTIL", "");
                                string presure = current.attribute.Split(',')[0];
                                while (true)
                                {
                                    if (this.checkCondition(condition, variables))
                                    {
                                        break;
                                    }
                                    this.writeCommandToFile("brake presure: " + presure);
                                }
                                realLineCounter++;
                                if (loopEndIndex.Count != 0)
                                {
                                    realLineCounter--;
                                }
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "loop":
                            if (ifConditionSatisfied.Peek())
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                // TODO: check error handler in compiler for LOOP. LOOP should have attribute
                                loopCount.Push(Int32.Parse(current.attribute));
                                loopStarted.Push(true);
                                loopIndex.Push(0);
                                loopSeen = true;
                                start = DateTime.Now;
                                realLineCounter++;
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "lend":
                            if (ifConditionSatisfied.Peek())
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                                bool conditionStisfied = true;
                                string LoopCondition = "";
                                int validDuration = 0;
                                if (current.attribute.Contains(","))
                                {
                                    LoopCondition = current.attribute.Split(',')[1].ToLower().Replace("while", "");
                                    conditionStisfied = this.checkCondition(LoopCondition, variables);
                                    if (Int32.TryParse(current.attribute.Split(',')[0], out validDuration))
                                    {
                                        validDuration = Int32.Parse(current.attribute.Split(',')[0]);
                                    }
                                    while (true)
                                    {
                                        end = DateTime.Now;
                                        TimeSpan loopDuration = end - start;
                                        if (validDuration + 1 <= loopDuration.TotalSeconds)
                                        {
                                            this.writeCommandToFile("WARNING > valid duration is: " + validDuration + " but loop duration is: " + loopDuration.TotalSeconds + " s");
                                            break;
                                        }
                                        else if (validDuration <= loopDuration.TotalSeconds)
                                        {
                                            break;
                                        }
                                    }
                                }
                                else if (current.attribute.ToLower().Contains("while"))
                                {
                                    LoopCondition = current.attribute.ToLower().Replace("while", "");
                                    conditionStisfied = this.checkCondition(LoopCondition, variables);
                                }
                                else if (current.attribute.Length > 0)
                                {
                                    if (Int32.TryParse(current.attribute, out validDuration))
                                    {
                                        validDuration = Int32.Parse(current.attribute);
                                    }
                                    while (true)
                                    {
                                        end = DateTime.Now;
                                        TimeSpan loopDuration = end - start;
                                        if (validDuration + 1 <= loopDuration.TotalSeconds)
                                        {
                                            this.writeCommandToFile("WARNING > valid duration is: " + validDuration + " but loop duration is: " + loopDuration.TotalSeconds + " s");
                                            break;
                                        }
                                        else if (validDuration <= loopDuration.TotalSeconds)
                                        {
                                            break;
                                        }
                                    }
                                }
                                int currentLoopRepeat = loopCount.Pop();
                                if (loopEndIndex.Count == 0 && conditionStisfied)
                                {
                                    loopEndIndex.Push(index);
                                }
                                else if (loopEndIndex.Count != 0 && conditionStisfied)
                                {
                                    if (loopEndIndex.Peek() != index)
                                    {
                                        loopEndIndex.Push(index);
                                    }
                                }
                                currentLoopRepeat--;
                                if (currentLoopRepeat <= 0 || !conditionStisfied)
                                {
                                    if (!conditionStisfied)
                                    {
                                        this.writeCommandToFile("while condition didnt satisfied!!!");
                                    }
                                    loopStarted.Pop();
                                    loopStarted.Push(false);
                                    loopSeen = false;
                                    parentStack.Pop();
                                    parentIndexStack.Pop();
                                    index = (int)parentIndexStack.Peek();
                                    temp = (Node)parentStack.Peek();
                                    if (loopEndIndex.Count != 0)
                                        loopEndIndex.Pop();
                                    realLineCounter++;
                                    break;
                                }
                                else
                                {
                                    current = (Node)parentStack.Peek();
                                    current = current.child[0];
                                    loopCount.Push(currentLoopRepeat);
                                    loopSeen = true;
                                }
                            }
                            else
                            {
                                this.commands.Enqueue(new Command("highlight", realLineCounter, this.ORANGE));
                                realLineCounter++;
                            }
                            break;
                        case "end":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            this.writeCommandToFile("end!!!");
                            realLineCounter++;
                            break;
                        case "if":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            ifConditionSatisfied.Push(this.checkCondition(current.attribute, variables));
                            realLineCounter++;
                            break;
                        case "endif":
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            ifConditionSatisfied.Pop();
                            index++;
                            realLineCounter++;
                            break;
                    }
                    while (true)
                    {
                        if (realLineCounter >= realLines.Count)
                        {
                            errorDetected = true;
                            break;
                        }
                        if (realLines[realLineCounter].Item2 == "")
                        {
                            this.commands.Enqueue(new Command("highlight", realLineCounter, this.GREEN));
                            realLineCounter++;
                        }
                        else break;
                    }
                    if (current.child.Count > 0)
                    {
                        parentStack.Push(current);
                        parentIndexStack.Push(0);
                        continue;
                    }
                    if (loopStarted.Count > 0)
                    {
                        if (loopStarted.Peek() == true)
                        {
                            if (loopEndIndex.Count <= 0)
                            {
                                index++;
                                parentIndexStack.Pop();
                                parentIndexStack.Push(index);
                                continue;
                            }
                            if (index <= loopEndIndex.Peek())
                            {
                                index++;
                                parentIndexStack.Pop();
                                parentIndexStack.Push(index);
                                continue;
                            }
                        }
                    }
                    if (index < temp.child.Count)
                    {
                        index++;
                        parentIndexStack.Pop();
                        parentIndexStack.Push(index);
                        continue;
                    }
                    else
                    {
                        if (temp == root)
                        {
                            break;
                        }
                        parentIndexStack.Pop();
                        int tempIndex = (int)parentIndexStack.Pop();
                        tempIndex++;
                        parentStack.Pop();
                        parentIndexStack.Push(tempIndex);
                    }
                }
            }
            #endregion

            return 1;
        }
        #endregion
    }
}
