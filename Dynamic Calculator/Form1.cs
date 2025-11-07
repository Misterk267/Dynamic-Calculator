using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using static System.Collections.Specialized.BitVector32;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Dynamic_Calculator
{
    //ToDo: 1.implement variable solve mode


    public partial class Form1 : Form
    {

        //class variables for evaluation mode
        double[,] numbers = new double[2, 50];

        int[] negatives = new int[50];
        int[,] parenteses = new int[2, 20];
        
        int numberOfParenteses = 0;
        int numberOfNegatives = 0;
        int numberOfSigns = 0;
        int numberOfExp = 0;
        int numberOfMultDiv = 0;
        int numberOfAddSubtract = 0;
        int numberOfVars = 0;

        string[,] signs = new string[2, 30];
        string[,] multDiv = new string[2, 30];
        string[,] addSubtract = new string[2, 30];
        string[,] exp = new string[2, 30];

        //class variables for solve mode
        bool solveMode = false;
        bool containsEquals = false;
        string[,] terms = new string[2, 20];
        int numberOfTerms = 0;

        public Form1()
        {
            InitializeComponent();
        }

        /*
         Program works as follows:
         1. Get and cleanup input string - remove letters, invalid charcaters, etc
         2. Identify portions inside parenteses () and evaluate their contents, modifying original string to reflect the changes
         3. When there are no more parenteses, the string is ready for final processing. Systemically perform math operations and
            amend string as necessary to reflect state for next operation accuracy. Continue until only the answer remains.
         4. Output answer to user
        */
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Size = new Size(473, 466);
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            lstHistory.Items.Clear();
            solveMode = false;
            string noParenteses = "";
            string input = txtInput.Text.Trim();
            if (input != "")
            {
                string processedInput = InitialProcessInput(input); //string cleanup
                if (solveMode == true)
                {
                    //do stuff here
                }
                else
                {
                    processedInput = RefreshStringData(processedInput); //find out where parenteses are, if any
                    noParenteses = numberOfParenteses > 1 ? ProcessParenteses(processedInput) : ""; //skip if no parenteses               
                    noParenteses = numberOfParenteses == 1 ? RemoveExtraParentese(noParenteses) : noParenteses; //remove extra if neccessary
                    processedInput = noParenteses == "" ? processedInput : noParenteses; //update output string with post-parenteses value, if necessary
                    string value = EvaluateExpression(processedInput, false); //evaluate final string
                    txtOutput.Text = value.ToString(); //output to user
                }


            }
            else
            {
                MessageBox.Show("Enter an expression first.", "Input Error");
            }
        }

        /******************************************************************************************
         *                             Methods that clean up strings
         ******************************************************************************************/
        private string InitialProcessInput(string input)
        {
            //remove spaces and unwanted chars such as punctuation, etc
            int length = input.Length;
            for (int i = 0; i < length; i++)
            {
                if (IsThisADigit(input[i]) == false && IsThisAnOperator(input[i]) == false && IsThisALetter(input[i]) == false)
                {
                    input = input.Remove(i, 1);
                    i--;
                    length--;
                }
            }
            /**************************************************************************************************************************************
             * finds out how many letters are present. If there is one letter, the program will consider it to be a variable it must solve for.
             * If there is more than one, it will consider them to be errors and remove them all.
             **************************************************************************************************************************************/

            string letters = "";
            for (int i = 0; i < input.Length; i++)
            {
                if (IsThisALetter(input[i]))
                {
                    letters += input[i];
                }
            }
            if (letters != "")
            {
                input = AreAllLettersTheSame(letters) == false ? RemoveLetters(input) : input;
                solveMode = true;
            }
            else
            {
                solveMode = false;
            }
            lstHistory.Items.Add("Initial Input: ".PadRight(15) + "|  " + input);
            return input;
        }

        //two operators back to back will cause errors. 
        //Second operator is considered a typo and removed if not a "-" , as this represents a negative number
        //if more than two operators in a row, a negative representing a negative number will implicitly always be the last operator in the sequence
        private string RemoveDuplicateOperators(string input)
        {
            int length = input.Length;
            for (int i = 0; i < length - 2; i++) //avoid checking final char
            {
                if (IsThisAnOperator(input[i]) == true && IsThisAnOperator(input[i + 1]) == true && //dont count parenteses
                    IsThisAParentese(input[i]) == false && IsThisAParentese(input[i + 1]) == false)
                {
                    if (input[i + 1].ToString() == "-" && input[i].ToString() != "-")
                    {
                        if (i == 0)
                        {
                            input = input.Substring(1);
                            i--;
                            length--;
                        }
                    }
                    else if (input[i + 1].ToString() == "-" && input[i].ToString() == "-")
                    {
                        if (i == 0)
                        {
                            input = input.Substring(2);
                            length -= 2;
                            i--;
                        }
                        else
                        {
                            input = input.Substring(0, i) + "+" + input.Substring(i + 2);
                        }
                    }
                    else
                    {
                        input = i == 0 ? input.Substring(2) : input.Substring(0, i) + input.Substring(i + 1);
                        length = i == 0 ? length -= 2 : length--;
                    }
                }
            }
            return input;
        }

        private string RemoveLetters(string input)
        {
            int length = input.Length;
            for (int i = 0; i < length; i++)
            {
                if (IsThisALetter(input[i]) == true)
                {
                    input = input.Remove(i, 1);
                    i--;
                    length--;
                }
            }
            return input;
        }

        /******************************************************************************************
         *                             Methods that deal with parenteses
         ******************************************************************************************/

        private void GetParenteses(string input)
        {
            //store Parenteses Indexes in memory, as well as what type they are
            Array.Clear(parenteses);
            numberOfParenteses = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i].ToString() == "(")
                {
                    parenteses[0, numberOfParenteses] = i;
                    parenteses[1, numberOfParenteses] = 0; //0 = open, 1 = close - keeps track of which kind each is
                    numberOfParenteses++;
                }
                if (input[i].ToString() == ")")
                {
                    parenteses[0, numberOfParenteses] = i;
                    parenteses[1, numberOfParenteses] = 1; //0 = open, 1 = close - keeps track of which kind each is
                    numberOfParenteses++;
                }
            }
        }

        //removes content between innermost or first set of parenteses and replaces that portion of string with its value
        private string ProcessParenteses(string input)
        {
            int i = 0;
            while (numberOfParenteses > 1)
            {
                if (parenteses[1, i] == 0 && parenteses[1, i + 1] == 1)
                {
                    string value = EvaluateExpression(input.Substring(parenteses[0, i] + 1, parenteses[0, i + 1] - parenteses[0, i] - 1), true);
                    input = RefreshStringData(input);
                    if (parenteses[0, i] != 0) //dont check previous char if open parentese is first char in string
                    {
                        value = IsThisADigit(input[parenteses[0, i] - 1]) ? "*" + value : value; //if parenteses value is multiplied by previous number w/o sign, insert multiplication
                    }
                    input = parenteses[0, i + 1] == input.Length - 1 ? input.Substring(0, parenteses[0, i]) + value.ToString() :
                        input.Substring(0, parenteses[0, i]) + value.ToString() + input.Substring(parenteses[0, i + 1] + 1);
                    input = RefreshStringData(input);
                    lstHistory.Items.Add("Expression: ".PadRight(15) + "|  " + input);
                    i = 0; //restart loop every time we process a set of parenteses. This is to deal with nested Parenteses.
                }
                else
                {
                    i++; //check next set
                }
            }
            return input;
        }

        //if uneven number of parenteses, this will remove the one that remains
        private string RemoveExtraParentese(string input)
        {
            for (int i = 0; i < input.Length; i++)
            {
                input = IsThisAParentese(input[i]) ? input.Substring(0, i) + input.Substring(i + 1) : input;
            }
            return input;
        }

        /******************************************************************************************
         *                             Methods that store things in memory
         ******************************************************************************************/

        //identify and store negative numbers
        private void GetNegativeTerms(string input)
        {
            Array.Clear(negatives);
            numberOfNegatives = 0;
            if (IsThisAnOperator(input[0]) == true)
            {

                if (input[0].ToString() == "-")
                {
                    negatives[0] = 1; //if first char is -, first number must be negative
                    numberOfNegatives++;
                }
            }
            for (int i = 2; i <= input.Length - 1; i++)
            {
                if ((IsThisADigit(input[i]) || IsThisALetter(input[i])) && input[i - 1].ToString() == "-" && IsThisAnOperator(input[i - 2]) == true)
                {
                    negatives[numberOfNegatives] = i; //store index of number that is negative
                    numberOfNegatives++;
                }
            }
        }

        //interpret and store Numbers in memory
        private void GetNumbers(string input)
        {
            Array.Clear(numbers);
            string number = "";
            int numberIndex = 0;
            for (int i = 0; i < input.Length; i++)
            {
                bool isNumber = IsThisADigit(input[i]);
                if (isNumber == true)
                {
                    number += input[i];

                    if (i == input.Length - 1)
                    {
                        numbers[0, numberIndex] = double.Parse(number);
                        numbers[1, numberIndex] = i - (number.Length - 1);
                        numberIndex++;
                        number = "";
                    }
                }
                else
                {
                    if (i != 0 && number != "")
                    {
                        numbers[0, numberIndex] = double.Parse(number);
                        numbers[1, numberIndex] = i - (number.Length);
                        numberIndex++;
                        number = "";
                    }
                }
            }
        }

        //finds and stores all operators
        private void GetOperators(string input)
        {
            numberOfSigns = 0;
            Array.Clear(signs);
            for (int i = 1; i < input.Length; i++)
            {
                if (IsThisAnOperator(input[i]))
                {
                    if ((input[i].ToString() != "-" && input[i].ToString() != "=" && IsThisAParentese(input[i]) == false) ||
                        (input[i].ToString() == "-" && IsThisAnOperator(input[i - 1]) == false)); //dont add - to operator list if it represnts a negative number
                    {
                        signs[0, numberOfSigns] = input[i].ToString();
                        signs[1, numberOfSigns] = i.ToString();
                        numberOfSigns++;
                    }
                }
            }
            SortOperators();
        }

        private void GetOperators(string input, string opToIgnore)
        {
            numberOfSigns = 0;
            Array.Clear(signs);
            for (int i = 1; i < input.Length; i++)
            {
                if (IsThisAnOperator(input[i]) && input[i].ToString() != opToIgnore)
                {
                    if ((input[i].ToString() != "-" && input[i].ToString() != "=" && IsThisAParentese(input[i]) == false) ||
                        (input[i].ToString() == "-" && IsThisAnOperator(input[i - 1]) == false)) ; //dont add - to operator list if it represnts a negative number
                    {
                        signs[0, numberOfSigns] = input[i].ToString();
                        signs[1, numberOfSigns] = i.ToString();
                        numberOfSigns++;
                    }
                }
            }
            SortOperators();
        }

        //update all string data at once
        private string RefreshStringData(string input)
        {
            input = RemoveDuplicateOperators(input);

            Array.Clear(numbers);
            Array.Clear(signs);
            Array.Clear(parenteses);
            Array.Clear(negatives);

            numberOfNegatives = 0;
            numberOfSigns = 0;
            numberOfParenteses = 0;

            string number = "";
            int numberIndex = 0;

            if (input[0].ToString() == "-")
            {
                negatives[0] = 1; //if first char is -, first number must be negative
                numberOfNegatives++;
            }

            for (int i = 0; i < input.Length; i++)
            {
                //store operators in order, skipping = and any - that represent negative numbers
                if (IsThisAnOperator(input[i]) && i > 0)
                {
                    if ((input[i].ToString() != "-" && input[i].ToString() != "=" && IsThisAParentese(input[i]) == false) ||
                        (input[i].ToString() == "-" && IsThisAnOperator(input[i - 1]) == false)) ; //dont add - to operator list if it represents a negative number
                    {
                        signs[0, numberOfSigns] = input[i].ToString();
                        signs[1, numberOfSigns] = i.ToString();
                        numberOfSigns++;
                    }
                }

                if (input[i].ToString() == "(")
                {
                    parenteses[0, numberOfParenteses] = i;
                    parenteses[1, numberOfParenteses] = 0; //0 = open, 1 = close - keeps track of which kind each is
                    numberOfParenteses++;
                }
                if (input[i].ToString() == ")")
                {
                    parenteses[0, numberOfParenteses] = i;
                    parenteses[1, numberOfParenteses] = 1; //0 = open, 1 = close - keeps track of which kind each is
                    numberOfParenteses++;
                }

                if (IsThisADigit(input[i]))
                {
                    number += input[i];

                    if (i == input.Length - 1)
                    {
                        numbers[0, numberIndex] = double.Parse(number);
                        numbers[1, numberIndex] = i - (number.Length - 1);
                        numberIndex++;
                        number = "";
                    }
                }
                else
                {
                    if (i != 0 && number != "")
                    {
                        numbers[0, numberIndex] = double.Parse(number);
                        numbers[1, numberIndex] = i - (number.Length);
                        numberIndex++;
                        number = "";
                    }
                }

                if (i > 1 && i < input.Length)
                {
                    if ((IsThisADigit(input[i]) || IsThisALetter(input[i])) && input[i - 1].ToString() == "-" && IsThisAnOperator(input[i - 2]) == true)
                    {
                        negatives[numberOfNegatives] = i; //store index of negative number
                        numberOfNegatives++;
                    }
                }
            }
            SortOperators();
            return input;
        }

        private void SortOperators()
        {
            numberOfExp = 0;
            numberOfMultDiv = 0;
            numberOfAddSubtract = 0;
            for(int i = 0; i < numberOfSigns; i++)
            {
                switch (signs[0, i])
                {
                    case "^":
                        exp[0, numberOfExp] = "^";
                        exp[1, numberOfExp] = i.ToString(); ;
                        numberOfExp++;
                        break;
                    case "*":
                    case "/":
                        multDiv[0, numberOfMultDiv] = signs[0, i];
                        multDiv[1, numberOfMultDiv] = i.ToString(); ;
                        numberOfMultDiv++;
                        break;
                    case "+":
                    case "-":
                        addSubtract[0, numberOfAddSubtract] = signs[0, i];
                        addSubtract[1, numberOfAddSubtract] = i.ToString();
                        numberOfAddSubtract++;
                        break;
                }
            }
        }

        private string OperatorToText(string op)
        {
            string opText = "";
            switch (op)
            {
                case "^":
                    opText = "Exponent: ";
                    break;
                case "*":
                    opText = "Multiply: ";
                    break;
                case "/":
                    opText = "Divide: ";
                    break;
                case "+":
                    opText = "Add: ";
                    break;
                case "-":
                    opText = "Subtract: ";
                    break;
            }
            return opText;
        }

        /******************************************************************************************
         *                             Methods that process string portions
         ******************************************************************************************/

        private string EvaluateExpression(string input, bool parentesesPortion)
        {
            input = RefreshStringData(input);
            int loopLimit = 0;
            string previous = input;
            string Parenteses = parentesesPortion ? $"({input})" : "";
            string[,] operators = new string[2, 50];
            for (int k = 0; k < 3; k++)
            {
                switch (k)
                {
                    case 0:
                        loopLimit = numberOfExp;
                        operators = exp;
                        break;
                    case 1:
                        loopLimit = numberOfMultDiv;
                        operators = multDiv;
                        break;
                    case 2:
                        loopLimit = numberOfAddSubtract;
                        operators = addSubtract;
                        break;
                }
                for (int i = 0; i < loopLimit; i++)
                {
                    string result;
                    string op = operators[0, i];

                    double a = numbers[0, int.Parse(operators[1, i])];
                    double b = numbers[0, int.Parse(operators[1, i]) + 1];
                    int aIndex = (int)numbers[1, int.Parse(operators[1, i])];
                    int bIndex = (int)numbers[1, int.Parse(operators[1, i]) + 1];
                    int aL = a.ToString().Length;
                    int bL = b.ToString().Length;

                    //check if either number is negative
                    for (int j = 0; j < numberOfNegatives; j++)
                    {
                        a = negatives[j] == aIndex ? -a : a;
                        b = negatives[j] == bIndex ? -b : b;
                    }

                    result = DoMath(a, b, op).ToString();
                    if (int.Parse(operators[1, i]) == 0) //avoid including anything before first number/operater pair as rollover
                    {
                        input = bIndex + bL >= input.Length ? result : result + input.Substring(bIndex + bL);
                    }
                    else
                    {
                        input = bIndex + bL >= input.Length ? input.Substring(0, aIndex) + result :
                        input.Substring(0, aIndex) + result + input.Substring(bIndex + bL);
                    }
                    input = RefreshStringData(input); //refresh for next loop iteration

                    //prepare output labels
                    string mem = parentesesPortion ? "Parenteses: ".PadRight(15) + "|  " + Parenteses + " = " + input : OperatorToText(op).PadRight(15) + "|  " + input;

                    if (input != previous && numberOfParenteses < 2)
                    {
                        lstHistory.Items.Add(mem);
                    }
                    i--;
                    loopLimit--;
                }
            }
            return input;
        }

        //if exponent calculation, b should be the exponent
        private double DoMath(double a, double b, string op)
        {
            double total;
            switch (op)
            {
                case "^":
                    total = Math.Pow(a, b);
                    break;
                case "+":
                    total = a + b;
                    break;
                case "-":
                    total = a - b;
                    break;
                case "*":
                    total = a * b;
                    break;
                case "/":
                    total = a / b;
                    break;
                default:
                    total = a + b;
                    break;
            }
            return total;
        }

        private string ReverseString(string input)
        {
            string newString = "";
            for (int i = input.Length - 1; i >= 0; i--)
            {
                newString += input[i];
            }
            return newString;
        }

        /******************************************************************************************
         *                             Methods that identify characters
         ******************************************************************************************/
        private bool AreAllLettersTheSame(string letters)
        {
            bool areSame = false;
            for (int i = 0; i < letters.Length - 1; i++)
            {
                if (IsThisALetter(letters[i]) && IsThisALetter(letters[i + 1]))
                {
                    areSame = letters[i].ToString().ToUpper() == letters[i + 1].ToString().ToUpper() ? true : areSame;
                }
            }
            return areSame;
        }
        private bool IsThisADigit(char input)
        {
            bool isDigit = false;
            string digits = "0123456789.";
            foreach (char character in digits)
            {
                isDigit = input == character ? true : isDigit;
            }
            return isDigit;
        }

        private bool IsThisALetter(char input)
        {
            bool isLetter = false;
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            foreach (char character in letters)
            {
                if (input.ToString().ToUpper() == character.ToString())
                {
                    isLetter = true;
                    letters += input;
                }
            }
            return isLetter;
        }

        private bool IsThisAnOperator(char input)
        {
            bool isOperator = false;
            string operators = "()^*/+-=";
            foreach (char character in operators)
            {
                isOperator = input == character ? true : isOperator;
            }
            return isOperator;
        }

        private bool IsThisAParentese(char input)
        {
            bool isP = false;
            if (input.ToString() == "(" || input.ToString() == ")")
            {
                isP = true;
            }
            return isP;
        }

        private bool IsThisAnEquals(char input)
        {
            bool isEquals = input.ToString() == "=" ? true : false;
            return isEquals;
        }

        private bool IsThisAnExponent(char input)
        {
            bool isExp = input.ToString() == "^" ? true : false;
            return isExp;
        }

        /******************************************************************************************
         *                             Methods that deal with variable solving
         ******************************************************************************************/
        private void FindVariables(string input)
        {
            string mem = "";
            int varCounter = 0;
            Array.Clear(terms);
            for (int i = 0; i < input.Length; i++)
            {
                if (IsThisALetter(input[i]))
                {
                    mem += input[i];
                    for (int j = i - 1; j >= 0; j--)
                    {
                        if (IsThisALetter(input[j]) || IsThisADigit(input[j]))
                        {
                            mem += input[j];
                            if (j == 0 && mem != "")
                            {
                                mem = ReverseString(mem);
                                terms[0, varCounter] = mem;
                                terms[1, varCounter] = j.ToString();
                                mem = "";
                                varCounter++;
                                j = -1; //exit inner loop
                            }
                        }
                        else
                        {
                            mem = ReverseString(mem);
                            terms[0, varCounter] = mem;
                            terms[1, varCounter] = j.ToString();
                            mem = "";
                            varCounter++;
                            j = -1; //exit inner loop
                        }
                    }
                }
            }
        }

        //test this next
        private string EquationSorter(string input)
        {
            input = InitialProcessEquation(input);
            string[] sep = input.Split("=");
            string[] combined = SortEquationPortion(sep[0], sep[1], false);
            combined = SortEquationPortion(combined[0], combined[1], true);
            combined[0] = SortLikeTerms(combined[0]);
            combined[0] = SimplifyExpression(combined[0]);
            combined[1] = EvaluateExpression(combined[1], false);
            string sorted = combined[0] + "=" + combined[1];
            return sorted;
        }

        public string[] SortEquationPortion(string part1, string part2, bool secondRun)
        {
            string mem = "";
            if (secondRun)
            {
                mem = part2;
                part2 = part1;
                part1 = mem;
            }
            GetOperators(part1, "^"); //populate number of signs var once before loop start

            //put all variable terms on left side
            for (int i = 0; i < numberOfSigns; i++)
            {
                GetTerms(part1);
                GetOperators(part1, "^");
                GetNegativeTerms(part1);

                string term = terms[0, i];
                string firstop = negatives[0] == int.Parse(terms[1, i]) ? "-" : "+";
                string op = i == 0 ? firstop : signs[0, i - 1];
                if (ContainsVariable(term) == secondRun)
                {
                    op = ReverseOp(op);
                    string newTerm = op + term;
                    part2 += newTerm;
                    if (i == 0)
                    {
                        part1 = part1.Substring(int.Parse(terms[1, i + 1]));
                    }
                    else if (i == numberOfSigns - 1)
                    {
                        part1 = part1.Substring(0, int.Parse(signs[1, i]));
                    }
                    else
                    {
                        part1 = part1.Substring(0, int.Parse(signs[1, i])) + part1.Substring(int.Parse(terms[0, i]) + term.Length);
                    }
                }
            }
            string[] output = { part1, part2 };
            return output;
        }

        private string ReverseOp(string op)
        {
            switch (op.Trim())
            {
                case "+":
                    op = "-";
                    break;
                case "-":
                    op = "+";
                    break;
                case "^":
                    op = "^";
                    break;
                case "*":
                    op = "/";
                    break;
                case "/":
                    op = "*";
                    break;
            }
            return op;
        }

        private bool ContainsVariable(string term)
        {
            bool isVar = false;
            for (int i = 0; i < term.Length; i++)
            {
                if (IsThisALetter(term[i]))
                {
                    isVar = true;
                }
            }
            return isVar;
        }
        private void GetTerms(string input)
        {
            Array.Clear(terms);
            string term = "";
            numberOfTerms = 0;
            for (int i = 0; i < input.Length; i++)
            {
                bool isNumber = IsThisADigit(input[i]) || IsThisALetter(input[i]) || IsThisAnExponent(input[i]);
                if (isNumber == true)
                {
                    term += input[i];

                    if (i == input.Length - 1)
                    {
                        terms[0, numberOfTerms] = term;
                        terms[1, numberOfTerms] = (i - (term.Length - 1)).ToString();
                        numberOfTerms++;
                        term = "";
                    }
                }
                else
                {
                    if (i != 0 && term != "")
                    {
                        terms[0, numberOfTerms] = term;
                        terms[1, numberOfTerms] = (i - (term.Length)).ToString();
                        numberOfTerms++;
                        term = "";
                    }
                }
            }
        }

        //split equation into two strings based on position of Equals
        private string InitialProcessEquation(string input)
        {
            //ensure there is only one equal sign before split. If none, add one at end.
            int equals = HowManyEquals(input);

            if (equals > 1)
            {
                input = RemoveMultipleEquals(input);
                equals = 1;
            }

            input = equals == 1 ? input + "=0" : input;
            input = RefreshEquationData(input);

            //remove or simplify all parenteses
            GetParenteses(input);
            if (numberOfParenteses > 1)
            {
                input = EquationParenteses(input);
            }
            if (numberOfParenteses == 1)
            {
                input = RemoveExtraParentese(input);
            }

            return input;
        }

        //test this when done :) 
        private string EquationParenteses(string input)
        {
            int i = 0;
            while (numberOfParenteses > 1)
            {
                if (parenteses[1, i] == 0 && parenteses[1, i + 1] == 1)
                {
                    int newStart = 0;
                    string mem = "";
                    string expression = input.Substring(parenteses[0, i] + 1, parenteses[0, i + 1] - parenteses[0, i] - 1);
                    expression = SimplifyExpression(expression);
                    if (IsThisADigit(input[parenteses[0, 1] - 1]) || IsThisALetter(input[parenteses[0, 1] - 1]))
                    {
                        for (int j = parenteses[0, 1] - 1; j >= 0; j--)
                        {
                            if (IsThisAnOperator(input[j]) == false)
                            {
                                mem += input[j];
                            }
                            else
                            {
                                newStart = j + 1;
                                j = -1; //break loop when encounter end of term
                            }
                        }
                        if (mem != "")
                        {
                            mem = ReverseString(mem);
                            expression = DistributeFactor(mem, expression);
                        }
                        input = parenteses[0, i + 1] == input.Length - 1 ? input.Substring(0, newStart) + expression :
                            input.Substring(0, newStart) + expression + input.Substring(parenteses[0, i + 1] + 1);
                        GetParenteses(input);
                        i = 0;
                    }
                    else
                    {
                        input = parenteses[0, i + 1] == input.Length - 1 ? input.Substring(0, parenteses[0, i]) + expression :
                            input.Substring(0, parenteses[0, i]) + expression.ToString() + input.Substring(parenteses[0, i + 1] + 1);
                        GetParenteses(input);
                        i = 0; //restart loop every time we process a set of parenteses. This is to deal with nested Parenteses.
                    }
                }
                else
                {
                    i++; //check next set
                }
            }
            return input;
        }

        private void DetectParentesesMultiplication(string input)
        {


        }

        //fix this
        private string SimplifyExpression(string input)
        {
            GetTerms(input);
            GetOperators(input, "^");
            GetNegativeTerms(input);
            string result = "";
            string prev = "";
            for(int j = 0; j < numberOfMultDiv; j++)
            {
                if(prev == input)
                {
                    j++;
                    if(j >= numberOfMultDiv)
                    {
                        break;
                    }
                }
                prev = input;
                switch (multDiv[0, j])
                {
                    case "*":
                        result = MultiplyTerms(terms[0, j], terms[0, j + 1]);
                        break;
                    case "/":
                        result = DivideTerms(terms[0, j], terms[0, j + 1]);
                        break;
                }
                if(numberOfTerms > 2)
                {
                    if (int.Parse(terms[1, j + 1]) + terms[1, j + 1].Length > input.Length)
                    {
                        input = input.Substring(0, int.Parse(terms[1, j])) + result;
                    }
                    else
                    {
                        input = input.Substring(0, int.Parse(terms[1, j])) + result + input.Substring(int.Parse(terms[1, j + 1]) + terms[0, j + 1].Length);
                    }
                }
                else
                {
                    input = result;
                }
                GetTerms(input);
                GetOperators(input, "^");
                GetNegativeTerms(input);
                j--;
            }
            GetTerms(input);
            GetOperators(input, "^");
            GetNegativeTerms(input);
            int operations = numberOfAddSubtract;
            for (int i = 0; i < operations; i++)
            {
                if (prev == input)
                {
                    i++; //skip inoperable terms (Different bases or exponents, etc.)
                    if (i >= numberOfMultDiv)
                    {
                        break;
                    }
                }
                prev = input;
                switch (addSubtract[0, i])
                {
                    case "+":
                        result = AddSubTerms(terms[0, i], terms[0, i + 1], true);
                        break;
                    case "-":
                        result = AddSubTerms(terms[0, i], terms[0, i + 1], false);
                        break;
                }
                if(numberOfTerms > 2)
                {
                    if (int.Parse(terms[1, i + 1]) + terms[1, i + 1].Length > input.Length)
                    {
                        input = input.Substring(0, int.Parse(terms[1, i])) + result;
                    }
                    else
                    {
                        input = input.Substring(0, int.Parse(terms[1, i])) + result + input.Substring(int.Parse(terms[1, i + 1]) + terms[0, i + 1].Length);
                    }                
                }
                else
                {
                    input = result;
                }
                GetTerms(input);
                GetOperators(input, "^");
                GetNegativeTerms(input);
                i--;
            }
            return input;
        }

        private string SeparateTerm(string term)
        {
            string number = "";
            string exp = "";
            string letter = "";
            bool firstNumber = false;
            bool letterPresent = false;
            bool exponent = false;
            foreach (char character in term)
            {
                if(IsThisADigit(character))
                {
                    if (!exponent)
                    {
                        number += character;
                        firstNumber = true;
                    }
                    else
                    {
                        exp += character;
                    }                        
                }else if (IsThisALetter(character))
                {
                    letter += character;
                    letterPresent = true;
                }else if (IsThisAnExponent(character))
                {
                    exponent = true;
                }      
            }
            if(firstNumber && letterPresent && exponent)
            {
                term = number + "," + letter + "," + exp;
            }else if(!firstNumber && letterPresent && exponent)
            {
                term = "1," + letter + "," + exp;
            }else if(firstNumber && !letterPresent && exponent)
            {
                term = DoMath(double.Parse(number), double.Parse(exp), "^").ToString() + ", ,1";
            }else if(firstNumber && letterPresent && !exponent)
            {
                term = number + "," + letter + ",1";
            }else if(firstNumber && !letterPresent && !exponent)
            {
                term = number + ", ," + "1";
            }else if(!firstNumber && letterPresent && !exponent)
            {
                term = "1," + letter + ",1";
            }
            return term;
        }

        private string RemoveMultipleEquals(string input)
        {
            int Equals = 0;
            int length = input.Length;
            for (int i = length - 1; i >= 0; i++)
            {
                if (IsThisAnEquals(input[i]))
                {
                    Equals++;
                }
                if (Equals > 1)
                {
                    input = input.Remove(i, 1);
                    Equals--;
                }
            }
            return input;
        }

        private int HowManyEquals(string input)
        {
            int a = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (IsThisAnEquals(input[i]))
                {
                    a++;
                }
            }
            return a;
        }


        private string RefreshEquationData(string input)
        {

            return input;
        }

        private string DistributeFactor(string mult1, string mult2)
        {
            //memory allocation for result terms
            string[] newTerms = new string[20];

            //populate storage for first string
            GetTerms(mult1);
            string[,] termStorage = terms;
            int termNumber = numberOfTerms;
            GetOperators(mult1, "^");
            string[,] opStorage = signs;
            int opNumber = numberOfSigns;

            //populate class arrays for second string
            GetTerms(mult2);
            GetOperators(mult2, "^");

            //storage for term processing
            string firstTerm;
            string result = "";

            for (int j = 0; j < termNumber; j++)
            {
                firstTerm = termStorage[0, j];

                for (int i = 0; i < numberOfTerms; i++)
                {
                    string secondTerm = terms[0, i];
                    result = i != numberOfTerms - 1 ? result += MultiplyTerms(firstTerm, secondTerm) + signs[0, i] :
                        result += MultiplyTerms(firstTerm, secondTerm);
                }

                result = j < termStorage.Length - 1 ? result += opStorage[0, j] : result; //avoid trying to add operator that doesn't exist
            }

            MessageBox.Show(result);
            return result;
        }

        private string MultiplyTerms(string factor1, string factor2)
        {
            //create memory for new term
            string newNumber = "";
            string newLetter = "";
            string newExp = "";
            string newTerm = "";

            //get details of first term
            string[] first = SeparateTerm(factor1).Split(",");
            string firstNumber = first[0];
            string firstLetter = first[1];
            string firstExp = first[2];

            //get details of second term
            string[] second = SeparateTerm(factor2).Split(",");
            string secondNumber = second[0];
            string secondLetter = second[1];  
            string secondExp = second[2];

            //account for exponents of 0 changing details of operation
            if (double.Parse(firstExp) == 0)
            {
                firstExp = "1";
                firstLetter = "";
                firstNumber = "1";
            }
            if (double.Parse(secondExp) == 0)
            {
                secondExp = "1";
                secondLetter = "";
                secondNumber = "1";
            }

            //construct new term
            newLetter = firstLetter == secondLetter ? firstLetter : firstLetter + secondLetter;
            newNumber = DoMath(double.Parse(firstNumber), double.Parse(secondNumber), "*").ToString();

            //deal with speical cases of number value
            if (double.Parse(newNumber) == 0)
            {
                newTerm = "0";
            }
            else if (double.Parse(newNumber) == 1)
            {
                newTerm = newLetter;
            }
            else
            {
                newTerm = newNumber + newLetter;
            }

            //deal with exponents and get new exponent value and add to term
            if (firstExp != "" && secondExp != "")
            {
                newExp = "^" + DoMath(double.Parse(firstExp), double.Parse(secondExp), "+").ToString();
            }
            else if (firstExp != "" && secondExp == "")
            {
                newExp = "^" + firstExp.ToString();
            }
            else if (firstExp == "" && secondExp != "")
            {
                newExp = "^" + secondExp.ToString();
            }
            else
            {
                newExp = "";
            }
            newTerm = newTerm != "0" ? newTerm + newExp : newTerm;
            MessageBox.Show(newTerm);
            return newTerm;
        }

        private string DivideTerms(string term1, string term2)
        {
            string result = "";
            //stuff goes here
            return result;
        }

        public string AddSubTerms(string term1, string term2, bool areAdding)
        {
            //getting information
            string result = "";
            string[] first = SeparateTerm(term1).Split(",");
            string[] second = SeparateTerm(term2).Split(",");
            string exp = "";

            if (areAdding)
            {
                result = first[1] == second[1] && first[2] == second[2] ? (double.Parse(first[0]) + double.Parse(second[0])).ToString() : result;               
                exp = first[2] == second[2] ? first[2] : exp;
                if (IsThisALetter(first[1][0]))
                {
                    result += first[1];
                }
                if(result != "" && exp != "")
                {
                    result = first[2] != "1" ? result += exp : result;
                }
                else
                {
                    result = term1 + "+" + term2;
                }
            }
            else
            {
                result = first[1] == second[1] ? (double.Parse(first[0]) - double.Parse(second[0])).ToString() : result;
                exp = first[2] == second[2] ? first[2] : exp;
                if (IsThisALetter(first[1][0]))
                {
                    result += first[1];
                }
                if (result != "" && exp != "")
                {
                    result = first[2] != "1" ? result += exp : result;
                }
                else
                {
                    result = term1 + "-" + term2;
                }
            }
            return result;
        }

        //this should relocate addable/subbable terms to be adjacent if they have the same base
        //needs testing
        private string SortLikeTerms(string input)
        {
            if (!IsThisAnOperator(input[0]) || input[0].ToString() != "-")
            {
                input = "+" + input;
            }

            string numLetExpPattern = @"[+\-]?[0-9]+[a-zA-Z]\^[0-9]+";
            string letExpPattern = @"[+\-]?[^0-9][a-zA-Z]\^[0-9]+";
            string letterPattern = @"[+\-]?[^0-9][a-zA-Z][^\^][0-9]+";
            string numLetPattern = @"[+\-]?[0-9]+[a-zA-Z][^\^][0-9]+";
            string numPattern = @"[0-9]+";
            
            Regex letter = new Regex(letterPattern);
            Regex numLet = new Regex(numLetPattern);
            Regex numLetExp = new Regex(numLetExpPattern);
            Regex letExp = new Regex(letExpPattern);
            Regex num = new Regex(numPattern);

            GetTerms(input);

            string mem = "";
            List<string> termStorage = new List<string>();

            //this is inefficient, will be fixed after migration of program logic to use lists instead of arrays
            for(int j = 0; j < numberOfTerms; j++)
            {
                termStorage.Add(terms[0, j]);
            }

            int loops = numberOfTerms;
            for(int i = 0; i < loops; i++)
            {
                Match match1 = numLetExp.Match(terms[0, i]);
                if (match1.Success)
                {
                    mem += match1.ToString();
                    termStorage.Remove(match1.ToString());
                    i--;
                    loops--;
                    continue;
                }
                Match match2 = letExp.Match(terms[0, i]);
                if (match2.Success)
                {
                    mem += match2.ToString();
                    termStorage.Remove(match2.ToString());
                    i--;
                    loops--;
                    continue;
                }
                Match match3 = numLet.Match(terms[0, i]);
                if (match3.Success)
                {
                    mem += match3.ToString();
                    termStorage.Remove(match3.ToString());
                    i--;
                    loops--;
                    continue;
                }
                Match match4 = letter.Match(terms[0, i]);
                if (match4.Success)
                {
                    mem += match4.ToString();
                    termStorage.Remove(match4.ToString());
                    i--;
                    loops--;
                    continue;
                }
                Match match5 = num.Match(terms[0, i]);
                if (match5.Success)
                {
                    mem += match5.ToString();
                    termStorage.Remove(match5.ToString());
                    i--;
                    loops--;
                    continue;
                }
            }
            if (IsThisAnOperator(mem[0]) && mem[0].ToString() != "-")
            {
                mem = mem.Substring(1);
            }
            return mem;
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            string test = SortLikeTerms("3x*7x-2x-1+2x^2");
            //string test = AddSubTerms("3x", "7x", false);
            MessageBox.Show(test);
           
        }
    }
}
