using System;
using System.IO;

namespace ConsoleApp1
{
    public class calc
    {

        public static void Main(string[] args)
        {
            /*
            Possible future feature implementations-

            Finding only pairs of frames that give both a crit and max damage roll - Added
            Adding a way to print only frames after a given RNG frame
            */

            //------------------------- Initialization ------------------------- 
            string repeat; //Number of times to repeat
            string rngInitSeed; //Initial seed
            string rngBaseNumOne = "41C64E6D"; //First number in RNG equation
            string rngBaseNumTwo = "6073"; //Second fixed number in RNG equation
            bool critSearch = false; //Initializes a bool for the question of whether to search for crit frames
            string critAsk; //Initializes the string that will be read from to set critSearch
            bool rollSearch = false; //Initializes a bool for the question of whether to search for rolls after crits
            string rollAsk; //Initializes the string that will be read from to set rollSearch
            int subCalc; //Initializes an integer that's used in the rollSearch loops
            int subLoopCount = 1; //Initializes another integer that's used in the rollSearch loops
            string subHex = "FFFFFFFF"; //Initializes a string that's used in the rollSearch loops
            int finalFrame; //Initializes an integer that's used if a match is found on the rollSearch loops
            //------------------------- End of initialization ------------------------- 



            //------------------------- Questions ------------------------- 
            Console.WriteLine("Enter the initial seed hex value: "); //Self-Explanatory
            rngInitSeed = Console.ReadLine(); //Sets initial seed to the value that was input

            Console.WriteLine("Enter number of times to repeat: "); //Self-Explanatory
            repeat = Console.ReadLine(); //Gets number of times to repeat

            Console.WriteLine("Would you like to search for only crit frames?"); //Self-Explanatory
            critAsk = Console.ReadLine(); //Gets answer for crit frame question
            if (critAsk.IndexOf("y", 0, 1) == 0) //Checks if the first character of critAsk is "y" and sets critSearch to true if it is
            {
                critSearch = true;
            }
            if (critAsk.IndexOf("Y", 0, 1) == 0) //Checks if the first character of critAsk is "Y" and sets critSearch to true if it is
            {
                critSearch = true;
            }

            if (critSearch == true)
            {
                Console.WriteLine("Would you like to search for only max roll crit frame pairs?"); //Self-Explanatory
                rollAsk = Console.ReadLine(); //Gets answer for roll frame question
                if (rollAsk.IndexOf("y", 0, 1) == 0) //Checks if the first character of critAsk is "y" and sets rollSearch to true if it is
                {
                    rollSearch = true;
                }
                if (rollAsk.IndexOf("Y", 0, 1) == 0) //Checks if the first character of critAsk is "Y" and sets rollSearch to true if it is
                {
                    rollSearch = true;
                }
            }
            //------------------------- End of questions ------------------------- 



            //------------------------- Calculation and display ------------------------- 
            int repeatTimes = Int32.Parse(repeat); //Sets an integer equal to the parsed value of repeat
            int InitSeed = Int32.Parse(rngInitSeed, System.Globalization.NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngInitSeed
            int BaseNumOne = Int32.Parse(rngBaseNumOne, System.Globalization.NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumOne
            int BaseNumTwo = Int32.Parse(rngBaseNumTwo, System.Globalization.NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumTwo
            int firstCalc = BaseNumOne * InitSeed + BaseNumTwo; //Calculates the first RNG result
            int repeated = 1; //Initializes the amount of times the loop has been repeated
            string hexResult = firstCalc.ToString("X8"); //Converts firstCalc back to hex
            if (critSearch == false)
            {
                Console.WriteLine("1: 0x" + hexResult); //Prints the hex value of firstCalc
            }
            if (critSearch == true && hexResult.IndexOf("0", 3, 1) == 3) //Checks if critSearch is set to true and checks if the 4th character in hexResult is "0"
            {
                if (rollSearch == false)
                {
                    Console.WriteLine("1: 0x" + hexResult);
                }
                if (rollSearch == true)
                {
                    subCalc = BaseNumOne * firstCalc + BaseNumTwo;
                    subLoopCount++;
                    while (subLoopCount <= 5)
                    {
                        subCalc = BaseNumOne * subCalc + BaseNumTwo;
                        subHex = subCalc.ToString("X8");
                        subLoopCount++;
                    }
                    if (subHex.IndexOf("0", 3, 1) == 3)
                    {
                        Console.WriteLine(repeated + ": 0x" + hexResult);
                        finalFrame = repeated + 5;
                        Console.WriteLine(finalFrame + ": 0x" + subHex);
                        Console.WriteLine();
                    }
                    subLoopCount = 1;
                }
            }

            while (repeated < repeatTimes) //Loop function
            {
                firstCalc = BaseNumOne * firstCalc + BaseNumTwo; //Does the equation again
                hexResult = firstCalc.ToString("X8");
                repeated++; //Adds 1 to repeated, allowing the program to terminate after running the correct amount of times
                if (critSearch == false)
                {
                    Console.WriteLine(repeated + ": 0x" + hexResult);
                }
                if (critSearch == true && hexResult.IndexOf("0", 3, 1) == 3)
                {
                    if (rollSearch == false)
                    {
                        Console.WriteLine(repeated + ": 0x" + hexResult);
                    }
                    if (rollSearch == true)
                    {
                        subCalc = BaseNumOne * firstCalc + BaseNumTwo;
                        subLoopCount++;
                        while (subLoopCount <= 5)
                        {
                            subCalc = BaseNumOne * subCalc + BaseNumTwo;
                            subHex = subCalc.ToString("X8");
                            subLoopCount++;
                        }
                        if (subHex.IndexOf("0", 3, 1) == 3)
                        {
                            Console.WriteLine(repeated + ": 0x" + hexResult);
                            finalFrame = repeated + 5;
                            Console.WriteLine(finalFrame + ": 0x" + subHex);
                            Console.WriteLine();
                        }
                        subLoopCount = 1;
                    }
                }
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
            //------------------------- End of calculation and display ------------------------- 
        }
    }
}
