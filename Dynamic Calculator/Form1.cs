using System.ComponentModel.DataAnnotations;
using System.Drawing.Text;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;

namespace Dynamic_Calculator
{
    //ToDo: 1. implement variable solve mode 2. Implement Parenteses Multiplication Detection 3. Implement roots
    
    
    public partial class Form1 : Form
    {
        
        double[,] numbers = new double[2, 50];
        int[]negatives = new int[50];
        int[,] parenteses = new int[2, 20];
        int numberOfParenteses = 0;
        int numberOfNegatives = 0;
        int numberOfSigns = 0;

        string[,] signs = new string[2, 20];
        string letters = "";

        //solveMode is disabled for now
        bool solveMode = false;

        public Form1()
        {
            InitializeComponent();
        }

        private void btnProcess_Click(object sender, EventArgs e)
        {
            string noParenteses = "";
            string input = txtInput.Text.Trim();
            if(input != "")
            {
                string processedInput = InitialProcessInput(input);
                processedInput = RefreshStringData(processedInput);
                GetParenteses(processedInput);
                /*if there are parenteses, deal with them here
                 Basically, we extract the portion between the parenteses and evaluate it, then return string with the value of the expression
                 inside the parenteses replaced with the value and the parenteses removed*/
                noParenteses = numberOfParenteses > 1 ? ProcessParenteses(processedInput) : "";                
                noParenteses = numberOfParenteses == 1 ? RemoveExtraParentese(noParenteses) : noParenteses;
                processedInput = noParenteses == "" ? processedInput : noParenteses;
                string value = EvaluateString(processedInput);
                lblOutput.Text = value.ToString();
            }
            else
            {
                MessageBox.Show("Enter an expression first.", "Input Error");
            }
        }

        private string InitialProcessInput(string input)
        {

            /*
             * finds out how many letters are present. If there is one letter, the program will consider it to be a variable it must solve for.
             * If there is more than one, it will consider them to be errors and remove them all. This needs to be done first to
             * stop the program from misinterpreting an expression
             */

            for (int i = 0; i < input.Length; i++)
            {
                if (IsThisALetter(input[i]) == true)
                {
                    letters += input[i];
                }
            }
            if (letters.Length > 1)
            {
                input = RemoveLetters(input);
            }

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
            return input;
        }
        private double solveForVariable()
        {
            double result = 0;
            char letter = letters[0];
            return result;
        }

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

        //currently is not processing more than one set of parenteses
        private string ProcessParenteses(string input)
        {
            int i = 0;
            while(numberOfParenteses > 1)
            {
                if (parenteses[1, i] == 0 && parenteses[1, i + 1] == 1)
                {
                    string value = EvaluateString(input.Substring(parenteses[0, i] + 1, parenteses[0, i + 1] - parenteses[0, i] - 1));
                    input = parenteses[0, i + 1] == input.Length - 1 ? input.Substring(0, parenteses[0, i]) + value.ToString() :
                        input.Substring(0, parenteses[0, i]) + value.ToString() + input.Substring(parenteses[0, i + 1] + 1);
                    GetParenteses(input);
                    i = 0; //restart loop every time we process a set of parenteses. This is to deal with nested Parenteses.
                    MessageBox.Show(input + "\nSigns: " + numberOfSigns.ToString() + "\n" + numbers[0, 1].ToString() + "\nParenteses: " + numberOfParenteses.ToString()); //debugging
                }
                else
                {
                    i++;
                }
                
            }
            return input;

            //int sets = numberOfParenteses / 2;
            //for (int i = 0; i < sets / 2; i++) //if uneven number of parenteses, ignore the last one in the string
            //{
            //    if (parenteses[1, i] == 0 && parenteses[1, i + 1] == 1)
            //    {
            //        GetParenteses(input);
            //        string value = EvaluateString(input.Substring(parenteses[0, i] + 1, parenteses[0, i + 1] - parenteses[0, i] - 1));
            //        input = parenteses[0, i + 1] == input.Length - 1 ? input.Substring(0, parenteses[0, i]) + value.ToString() : 
            //            input.Substring(0, parenteses[0, i]) + value.ToString() + input.Substring(parenteses[0, i + 1] + 1);
            //    }else if (parenteses[1, i] == 1 && parenteses[1, i + 1] == 0)
            //    {

            //    }
            //}

        }

        //if uneven number of parenteses, this will remove the one that remains
        private string RemoveExtraParentese(string input)
        {
            for(int i = 0; i < input.Length; i++)
            {
                if (IsThisAParentese(input[i]))
                {
                    input = input.Substring(0, i) + input.Substring(i + 1);
                }
            }
            return input;
        }

        //interpret and store Numbers in memory
        private void GetNumbers(string input)
        {
            Array.Clear(numbers);
            string number = "";
            int numberIndex = 0;
            for (int i = 0; i < input.Length; i++)
            {
                bool isDigit = IsThisADigit(input[i]);
                if (isDigit == true)
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
            for(int i = 1; i < input.Length; i++)
            {
                if (IsThisAnOperator(input[i]) == true)
                {
                    if ((input[i].ToString() != "-" || (input[i].ToString() == "-" && IsThisAnOperator(input[i - 1]) == false)) &&
                        IsThisAParentese(input[i]) == false)
                    {
                        signs[0, numberOfSigns] = input[i].ToString();
                        signs[1, numberOfSigns] = i.ToString();
                        numberOfSigns++;
                    }    
                }
            }
        }

        //respect order of operations PEMDAS (P taken care of already, will reference this method to evaluate contents
        private string EvaluateString(string input)
        {
            RefreshStringData(input);            
            input = EvaluateExpression(input, "^");
            input = EvaluateExpression(input, "*");
            input = EvaluateExpression(input, "/");
            input = EvaluateExpression(input, "+");
            input = EvaluateExpression(input, "-");
            
            return input;
        }

        //where the magic happens
        //horribly inefficient atm, clean up after project works
        private string EvaluateExpression(string input, string op)
        {
            
            double result = 0;
            string value = "";
            int operations = numberOfSigns;
            int tracker = 0;
            for (int i = 0; i < operations; i++)
            {
                if (signs[0, tracker] == op && signs[1, tracker] != "0")
                {
                    double a = numbers[0, tracker];
                    double b = numbers[0, tracker + 1];
                    int aIndex = (int)numbers[1, tracker];
                    int bIndex = (int)numbers[1, tracker + 1];
                    int aL = a.ToString().Length;
                    int bL = b.ToString().Length;

                    //check if either number is negative
                    for (int j = 0; j < numberOfNegatives; j++)
                    {
                        if (negatives[j] == aIndex)
                        {
                            a = -a;
                        }
                        if (negatives[j] == bIndex)
                        {
                            b = -b;
                        }
                    }

                    result = DoMath(a, b, op);
                    if (result < 0)
                    {
                        value = "-" + result.ToString();
                    }
                    else
                    {
                        value = result.ToString();
                    }
                    

                    if (tracker > 0)
                    {
                        if (bIndex + bL >= input.Length)
                        {
                            input = input.Substring(0, aIndex) + value;
                        }
                        else
                        {
                            input = input.Substring(0, aIndex) + value + input.Substring(bIndex + bL);
                        }

                    }
                    else
                    {
                        if (bIndex + bL >= input.Length)
                        {
                            input = value;
                        }
                        else
                        {
                            input = value + input.Substring(bIndex + bL);
                        }

                    }
                    input = RefreshStringData(input);
                }
                else
                {
                    tracker++;
                }
                
            }
            return input;
        }
        //identify negative numbers
        private void FindNegativeNumbers(string input)
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
                if (IsThisADigit(input[i]) == true && input[i - 1].ToString() == "-" && IsThisAnOperator(input[i - 2]) == true)
                {
                    negatives[numberOfNegatives] = i; //store index of number that is negative
                    numberOfNegatives++;
                }
            }
        }

        //two operators back to back will cause errors. 
        //Second operator is considered a typo and removed if not a "-" , as this represents a negative number
        //if more than two operators in a row, a negative representing a negative number will implicitly always be the last operator in the sequence

        //currently is removing parenteses
        private string RemoveDuplicateOperators(string input)
        {
            int length = input.Length;
            for (int i = 0; i < length - 2; i++) //avoid checking final char
            {
                if (IsThisAnOperator(input[i]) == true && IsThisAnOperator(input[i + 1]) == true && 
                    IsThisAParentese(input[i]) == false && IsThisAParentese(input[i + 1]) == false)
                {
                    if (input[i + 1].ToString() == "-")
                    {
                        if (i == 0)
                        {
                            input = input.Substring(1);
                            i--;
                            length--;
                        }
                    }
                    else
                    {
                        if (i == 0)
                        {
                            input = input.Substring(2);
                            i--;
                            length -= 2;

                        }
                        else
                        {
                            input = input.Substring(0, i) + input.Substring(i + 1);
                            i--;
                            length--;

                        }
                    }
                }
            }
            return input;
        }

        private string RefreshStringData(string input)
        {
            input = RemoveDuplicateOperators(input);
            GetOperators(input);
            GetNumbers(input);
            FindNegativeNumbers(input);
            return input;
        }

        //decides if program is solving an expression for the right side as normal or solving for the value of a variable. Disabled for now.
        private bool DecideMode()
        {
            if (letters.Length == 1)
            {
                solveMode = true;
            }
            return solveMode;
        }

        private string RemoveLetters(string input)
        {
            int length = input.Length;
            for(int i = 0; i < length; i++)
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

        private bool IsThisADigit(char input)
        {
            bool isDigit = false;
            string digits = "0123456789.";
            foreach (char character in digits)
            {
                if (input == character)
                {
                    isDigit = true;
                }
            }
            return isDigit;
        }

        private bool IsThisALetter(char input)
        {
            bool isLetter = false;
            string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            foreach(char character in letters)
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
            foreach(char character in operators)
            {
                if (input == character)
                {
                    isOperator = true;
                }
            }
            return isOperator;
        }

        private bool IsThisAParentese(char input)
        {
            bool isP = false;
            if(input.ToString() == "(" || input.ToString() == ")")
            {
                isP = true;
            }
            return isP;
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
    }
}
