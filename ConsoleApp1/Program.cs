using System;
using System.Globalization;
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


            /*
             The way the
             if (var.IndexOf(string, offset, length) == offset){}
             function works is that it looks at a specific variable (var in the example), then it checks if a specific set of characters (string in the example) in an index location of the
             variable (offset in the example) for a specific amount of characters (length in the example) is actually at the index location (offset in the example).

             Because of how it's set up, all it cares about is the contents of string within the bounds of length when performing the check, if string is not at said index location (i.e. a 
             different set of characters is at the index location of the variable in question) then it will return false, as the string at that offset for that length is not actually at the 
             offset in question.
            */

            //------------------------- Initialization ------------------------- 
            string repeat = ""; //Number of times to repeat
            string rngInitSeed = ""; //Initial seed
            string rngBaseNumOne = "41C64E6D"; //First number in RNG equation
            string rngBaseNumTwo = "6073"; //Second fixed number in RNG equation
            bool critSearch = false; //Initializes a boolean for the question of whether to search for crit frames
            string critAsk; //Initializes the string that will be read from to set critSearch
            bool rollSearch = false; //Initializes a boolean for the question of whether to search for rolls after crits
            string rollAsk; //Initializes the string that will be read from to set rollSearch
            int subCalc; //Initializes an integer that's used in the rollSearch loops
            int subLoopCount = 1; //Initializes another integer that's used in the rollSearch loops
            string subHex = "FFFFFFFF"; //Initializes a string that's used in the rollSearch loops
            int finalFrame; //Initializes an integer that's used if a match is found on the rollSearch loops
            int parseAttempt; //Initializes an integer that's mandatory for TryParse
            CultureInfo provider = CultureInfo.InvariantCulture; //Creates an IFormatProvider variable so TryParse can work with the hex values on initial seed input
            bool initSeedParse = false; //Initializes a boolean for the TryParse while loop for the initial seed
            bool repeatInputParse = false; //Initializes a boolean for the TryParse while loop for the number of times to repeat
            //------------------------- End of initialization ------------------------- 



            //------------------------- Questions ------------------------- 
            while (initSeedParse == false) //Makes sure that as long as you enter characters that are not hex numbers you will be asked this question again
            {
                Console.WriteLine("Enter the initial seed hex value: "); //Self-Explanatory
                rngInitSeed = Console.ReadLine(); //Sets initial seed to the value that was input
                initSeedParse = int.TryParse(rngInitSeed, NumberStyles.HexNumber, provider, out parseAttempt); //Attempts to parse the input, setting initSeedParse to true if it succeeds
                if (initSeedParse == false)
                {
                    Console.WriteLine("Please enter a hex value.");
                    Console.WriteLine();
                }
            }

            while (repeatInputParse == false) //Makes sure that as long as you enter characters that are not decimals you will be asked this question again
            {
                Console.WriteLine("Enter number of times to repeat: "); //Self-Explanatory
                repeat = Console.ReadLine(); //Gets number of times to repeat
                repeatInputParse = int.TryParse(repeat, out parseAttempt); //Attempts to parse the input, setting repeatInputParse to true if it succeeds
                if (repeatInputParse == false)
                {
                    Console.WriteLine("Please enter an integer.");
                    Console.WriteLine();
                }
            }

            Console.WriteLine("Would you like to search for only crit frames?"); //Self-Explanatory
            critAsk = Console.ReadLine(); //Gets answer for crit frame question
            if (critAsk.IndexOf("y", 0, 1) == 0 || critAsk.IndexOf("Y", 0, 1) == 0) //Checks if the first character of critAsk is "y" or "Y" and sets critSearch to true if it is
            {
                critSearch = true;
            }

            if (critSearch == true) //Only asks this question if you answered yes to the critAsk question
            {
                Console.WriteLine("Would you like to search for only max roll crit frame pairs?"); //Self-Explanatory
                rollAsk = Console.ReadLine(); //Gets answer for roll frame question
                if (rollAsk.IndexOf("y", 0, 1) == 0 || rollAsk.IndexOf("Y", 0, 1) == 0) //Checks if the first character of critAsk is "y" or "Y" and sets rollSearch to true if it is
                {
                    rollSearch = true;
                }
            }
            //------------------------- End of questions ------------------------- 



            //------------------------- Calculation and display ------------------------- 
            int repeatTimes = Int32.Parse(repeat); //Sets an integer equal to the parsed value of repeat
            int InitSeed = Int32.Parse(rngInitSeed, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngInitSeed
            int BaseNumOne = Int32.Parse(rngBaseNumOne, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumOne
            int BaseNumTwo = Int32.Parse(rngBaseNumTwo, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumTwo
            int firstCalc = BaseNumOne * InitSeed + BaseNumTwo; //Calculates the first RNG result
            int repeated = 1; //Initializes the amount of times the loop has been repeated
            string hexResult = firstCalc.ToString("X8"); //Converts firstCalc back to hex
            if (critSearch == false) //Checks if critSearch is false and if so, just runs the program with no changes
            {
                Console.WriteLine("1: 0x" + hexResult); //Prints the hex value of firstCalc
            }
            if (critSearch == true && hexResult.IndexOf("0", 3, 1) == 3) //Checks if critSearch is set to true and checks if the 4th character in hexResult is "0"
            {
                if (rollSearch == false) //Checks if rollSearch is set to false and if so, runs the normal critSearch loop
                {
                    Console.WriteLine("1: 0x" + hexResult);
                }
                if (rollSearch == true) //Checks if rollSearch is set to true and if so, runs a subcalculation in order to check if the second value in part of a pair also meets the requirements
                {
                    subCalc = BaseNumOne * firstCalc + BaseNumTwo;
                    subLoopCount++;
                    while (subLoopCount <= 5)
                    {
                        subCalc = BaseNumOne * subCalc + BaseNumTwo;
                        subHex = subCalc.ToString("X8");
                        subLoopCount++;
                    }
                    if (subHex.IndexOf("0", 3, 1) == 3) //Only prints if both the initial and final frame in a pair fit the requirements
                    {
                        Console.WriteLine(repeated + ": 0x" + hexResult);
                        finalFrame = repeated + 5;
                        Console.WriteLine(finalFrame + ": 0x" + subHex);
                        Console.WriteLine();
                    }
                    subLoopCount = 1; //Resets the subcalculation loop counter to 1
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
                    if (rollSearch == false) //Checks if rollSearch is set to false and if so, runs the normal critSearch loop
                    {
                        Console.WriteLine(repeated + ": 0x" + hexResult);
                    }
                    if (rollSearch == true) //Checks if rollSearch is set to true and if so, runs a subcalculation in order to check if the second value in part of a pair also meets the requirements
                    {
                        subCalc = BaseNumOne * firstCalc + BaseNumTwo;
                        subLoopCount++;
                        while (subLoopCount <= 5)
                        {
                            subCalc = BaseNumOne * subCalc + BaseNumTwo;
                            subHex = subCalc.ToString("X8");
                            subLoopCount++;
                        }
                        if (subHex.IndexOf("0", 3, 1) == 3) //Only prints if both the initial and final frame in a pair fit the requirements
                        {
                            Console.WriteLine(repeated + ": 0x" + hexResult);
                            finalFrame = repeated + 5;
                            Console.WriteLine(finalFrame + ": 0x" + subHex);
                            Console.WriteLine();
                        }
                        subLoopCount = 1; //Resets the subcalculation loop counter to 1
                    }
                }
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
            //------------------------- End of calculation and display ------------------------- 
        }
    }
}
