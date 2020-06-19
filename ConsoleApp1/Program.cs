using System;
using System.Globalization;
using System.IO;

namespace ConsoleApp1
{
    public class calc
    {
        private static bool YesOrNo(string question)
        {
            Console.WriteLine(question);
            string answer = Console.ReadLine();
            return char.ToLower(answer[0]) == 'y';
        }

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
            string rngInitSeed = ""; //Initial seed
            string rngBaseNumOne = "41C64E6D"; //First number in RNG equation
            string rngBaseNumTwo = "6073"; //Second fixed number in RNG equation
            bool critSearch = false; //Initializes a boolean for the question of whether to search for crit frames
            bool rollSearch = false; //Initializes a boolean for the question of whether to search for rolls after crits
            int subCalc; //Initializes an integer that's used in the rollSearch loops
            int subLoopCount = 1; //Initializes another integer that's used in the rollSearch loops
            string subHex = "FFFFFFFF"; //Initializes a string that's used in the rollSearch loops
            int finalFrame; //Initializes an integer that's used if a match is found on the rollSearch loops
            bool save = false; //Initializes a boolean for the question of whether to save the results
            bool encounterSearch = false;
            bool noEncounters = false;
            int tileFrame = 0;
            int looped = 0;
            int encounterCalc; //Initializes integer used to store the result of the calculation that determines if an encounter occurs
            int moveType = 5; //Initializes integer used to determine what type of movement you are using for the noEncounters loop
            int InitSeed = 0;
            int repeatTimes = 0;
            int rollParsed = 0;
            int rollCalculation;
            int gameVar = 0; //Initializes an integer used to determine how many frames should be passed in the subcalculation loops before returning the result
            //------------------------- End of initialization ------------------------- 



            //------------------------- Questions ------------------------- 
            bool initSeedParse = false; //Initializes a boolean for the TryParse while loop for the initial seed
            while (initSeedParse == false) //Makes sure that as long as you enter characters that are not hex numbers you will be asked this question again
            {
                Console.WriteLine("Enter the initial seed hex value: "); //Self-Explanatory
                rngInitSeed = Console.ReadLine(); //Sets initial seed to the value that was input
                initSeedParse = int.TryParse(rngInitSeed, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out InitSeed); //Attempts to parse the input, setting initSeedParse to true if it succeeds
                if (initSeedParse) { break; }
                Console.WriteLine("Please enter a hex value.");
                Console.WriteLine();
            }

            if (YesOrNo("Would you like to search for only frames that produce an encounter? (Ruby/Sapphire only)"))
            {
                encounterSearch = true;

                if (YesOrNo("Do you want to calculate how many tiles you can move based on the RNG frame you entered the first tile?")) //Only asks this if you answered yes to the previous question
                {
                    noEncounters = true;

                    Console.WriteLine("Are you (w)alking, (r)unning, (s)urfing, on the (a)cro bike, or on the (m)ach bike? (acro not implemented yet, if you do not input a valid movement type, mach bike is chosen)");
                    string moveAsk = Console.ReadLine();
                    if (char.ToLower(moveAsk[0]) == 'w')
                    {
                        moveType = 17;
                    }
                    if (char.ToLower(moveAsk[0]) == 'r' || char.ToLower(moveAsk[0]) == 's')
                    {
                        moveType = 9;
                    }
                    if (char.ToLower(moveAsk[0]) == 'a')
                    {
                        moveType = 17;
                    }
                    if (char.ToLower(moveAsk[0]) == 'm')
                    {
                        moveType = 5;
                    }

                    bool tileFrameParse = false;
                    while (tileFrameParse == false)
                    {
                        Console.WriteLine("Enter the first RNG frame that you walked into a tile.");
                        string firstTileFrame = Console.ReadLine();
                        tileFrameParse = int.TryParse(firstTileFrame, out tileFrame);
                        if (tileFrameParse) { break; }
                        Console.WriteLine("Please enter an integer.");
                        Console.WriteLine();
                    }
                }
            }

            if (noEncounters == false)
            {
                bool repeatInputParse = false; //Initializes a boolean for the TryParse while loop for the number of times to repeat
                while (repeatInputParse == false) //Makes sure that as long as you enter characters that are not decimals you will be asked this question again
                {
                    Console.WriteLine("Enter number of times to repeat: "); //Self-Explanatory
                    string repeat = Console.ReadLine(); //Gets number of times to repeat
                    repeatInputParse = int.TryParse(repeat, out repeatTimes); //Attempts to parse the input, setting repeatInputParse to true if it succeeds
                    if (repeatInputParse) { break; }
                    Console.WriteLine("Please enter an integer.");
                    Console.WriteLine();
                }

                if (encounterSearch == false)
                {
                    if (YesOrNo("Would you like to search for only crit frames?"))
                    {
                        critSearch = true;
                    }
                    if (critSearch) //Only asks this question if you answered yes to the critAsk question
                    {
                        if (YesOrNo("Would you like to search for only max roll crit frame pairs?"))
                        {
                            rollSearch = true;

                            bool gameInput = false;
                            while (gameInput == false)
                            {
                                Console.WriteLine("Are you playing (f)ire red, (l)eaf green, (r)uby, (s)apphire, or (e)merald?");
                                string gameAsk = Console.ReadLine();
                                if (char.ToLower(gameAsk[0]) == 'f' || char.ToLower(gameAsk[0]) == 'l')
                                {
                                    gameVar = 5;
                                    gameInput = true;
                                    break;
                                }
                                if (char.ToLower(gameAsk[0]) == 'r' || char.ToLower(gameAsk[0]) == 's' || char.ToLower(gameAsk[0]) == 'e')
                                {
                                    gameVar = 7;
                                    gameInput = true;
                                    break;
                                }
                                Console.WriteLine("Please enter a valid game code.");
                                Console.WriteLine();
                            }

                            bool minRollParse = false;
                            while (minRollParse == false) //Makes sure that as long as you enter characters that are not decimals you will be asked this question again
                            {
                                Console.WriteLine("What is the minimum roll you will accept? (0-16, 0 being max damage, 16 being minimum damage)");
                                string minRoll = Console.ReadLine(); //Gets the minimum roll you will accept
                                minRollParse = int.TryParse(minRoll, out rollParsed); //Attempts to parse the input, setting minRollParse to true if it succeeds
                                if (minRollParse) { break; }
                                Console.WriteLine("Please enter an integer.");
                                Console.WriteLine();
                            }
                        }
                    }
                }

                if (YesOrNo("Would you like to save the results to a txt file? (significantly slower at the moment)"))
                {
                    save = true;
                }
            }
            //------------------------- End of questions ------------------------- 



            //------------------------- Calculation and display ------------------------- 
            int BaseNumOne = int.Parse(rngBaseNumOne, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumOne
            int BaseNumTwo = int.Parse(rngBaseNumTwo, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumTwo
            int firstCalc = BaseNumOne * InitSeed + BaseNumTwo; //Calculates the first RNG result
            int repeated = 1; //Initializes the amount of times the loop has been repeated
            string hexResult = firstCalc.ToString("X8"); //Converts firstCalc back to hex

            if (noEncounters)
            {
                //tileFrame = Int32.Parse(firstTileFrame);
                while (repeated < tileFrame) //Finds RNG value on RNG frame given
                {
                    firstCalc = BaseNumOne * firstCalc + BaseNumTwo;
                    hexResult = firstCalc.ToString("X8");
                    repeated++;
                }

                subCalc = BaseNumOne * firstCalc + BaseNumTwo;
                encounterCalc = 321;
                while (encounterCalc >= 320)
                {
                    subLoopCount++;
                    while (subLoopCount <= moveType)
                    {
                        subCalc = BaseNumOne * subCalc + BaseNumTwo;
                        subHex = subCalc.ToString("X8");
                        subLoopCount++;
                    }
                    subLoopCount = 0;

                    string hexDigits = subHex.Substring(0, 4); //Makes a string that is the first 4 characters of subHex
                    int encounterVar = int.Parse(hexDigits, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of hexDigits
                    encounterCalc = encounterVar % 2880; //Sets an integer equal to encounterVar mod 2880
                    if (encounterCalc >= 320)
                    {
                        looped++;
                    }
                }
                Console.WriteLine("You can move a total of " + looped + " tiles before you hit an encounter, assuming the RNG function is not called extra times between tiles.");
            }

            if (noEncounters == false)
            {
                if (critSearch == false) //Checks if critSearch is false and if so, just runs the program with no changes
                {
                    if (encounterSearch == false)
                    {
                        Console.WriteLine("1: 0x" + hexResult); //Prints the hex value of firstCalc
                        if (save)
                        {
                            File.AppendAllText(rngInitSeed + "data.txt", "1: 0x" + hexResult + "\n");
                        }
                    }
                    if (encounterSearch)
                    {
                        string hexDigits = hexResult.Substring(0, 4); //Makes a string that is the first 4 characters of hexResult
                        int encounterVar = Int32.Parse(hexDigits, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of hexDigits
                        encounterCalc = encounterVar % 2880; //Sets an integer equal to encounterVar mod 2880
                        if (encounterCalc < 320) //Checks if encounterCalc is less than 320 and prints result if so
                        {
                            Console.WriteLine("1: 0x" + hexResult); //Prints the hex value of firstCalc
                            if (save)
                            {
                                File.AppendAllText(rngInitSeed + "data-encounters.txt", "1: 0x" + hexResult + "\n");
                            }
                        }
                    }
                }
                if (critSearch && hexResult[3] == '0') //Checks if critSearch is set to true and if the 4th character in hexResult is 0
                {
                    if (rollSearch == false) //Checks if rollSearch is set to false and if so, runs the normal critSearch loop
                    {
                        Console.WriteLine("1: 0x" + hexResult);
                        if (save)
                        {
                            File.AppendAllText(rngInitSeed + "data-critsonly.txt", "1: 0x" + hexResult + "\n");
                        }
                    }
                    if (rollSearch) //Checks if rollSearch is set to true and if so, runs a subcalculation in order to check if the second value in part of a pair also meets the requirements
                    {
                        subCalc = BaseNumOne * firstCalc + BaseNumTwo;
                        subLoopCount++;
                        while (subLoopCount <= gameVar)
                        {
                            subCalc = BaseNumOne * subCalc + BaseNumTwo;
                            subHex = subCalc.ToString("X8");
                            subLoopCount++;
                        }
                        rollCalculation = int.Parse(subHex.Substring(3, 1), NumberStyles.HexNumber);
                        if (rollCalculation <= rollParsed)
                        {
                            Console.WriteLine(repeated + ": 0x" + hexResult);
                            finalFrame = repeated + gameVar;
                            Console.WriteLine(finalFrame + ": 0x" + subHex);
                            Console.WriteLine();
                            if (save)
                            {
                                File.AppendAllText(rngInitSeed + "data-critrollpairs.txt", repeated + ": 0x" + hexResult + "\n");
                                File.AppendAllText(rngInitSeed + "data-critrollpairs.txt", finalFrame + ": 0x" + subHex + "\n\n");
                            }
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
                        if (encounterSearch == false)
                        {
                            Console.WriteLine(repeated + ": 0x" + hexResult);
                            if (save)
                            {
                                File.AppendAllText(rngInitSeed + "data.txt", repeated + ": 0x" + hexResult + "\n");
                            }
                        }
                        if (encounterSearch)
                        {
                            string hexDigits = hexResult.Substring(0, 4);
                            int encounterVar = int.Parse(hexDigits, NumberStyles.HexNumber);
                            encounterCalc = encounterVar % 2880;
                            if (encounterCalc < 320)
                            {
                                Console.WriteLine(repeated + ": 0x" + hexResult); //Prints the hex value of firstCalc
                                if (save)
                                {
                                    File.AppendAllText(rngInitSeed + "data-encounters.txt", repeated + ": 0x" + hexResult + "\n");
                                }
                            }
                        }
                    }
                    if (critSearch && hexResult[3] == '0')
                    {
                        if (rollSearch == false) //Checks if rollSearch is set to false and if so, runs the normal critSearch loop
                        {
                            Console.WriteLine(repeated + ": 0x" + hexResult);
                            if (save)
                            {
                                File.AppendAllText(rngInitSeed + "data-critsonly.txt", repeated + ": 0x" + hexResult + "\n");
                            }
                        }
                        if (rollSearch) //Checks if rollSearch is set to true and if so, runs a subcalculation in order to check if the second value in part of a pair also meets the requirements
                        {
                            subCalc = BaseNumOne * firstCalc + BaseNumTwo;
                            subLoopCount++;
                            while (subLoopCount <= gameVar)
                            {
                                subCalc = BaseNumOne * subCalc + BaseNumTwo;
                                subHex = subCalc.ToString("X8");
                                subLoopCount++;
                            }
                            rollCalculation = int.Parse(subHex.Substring(3, 1), NumberStyles.HexNumber);
                            if (rollCalculation <= rollParsed)
                            {
                                Console.WriteLine(repeated + ": 0x" + hexResult);
                                finalFrame = repeated + gameVar;
                                Console.WriteLine(finalFrame + ": 0x" + subHex);
                                Console.WriteLine();
                                if (save)
                                {
                                    File.AppendAllText(rngInitSeed + "data-critrollpairs.txt", repeated + ": 0x" + hexResult + "\n");
                                    File.AppendAllText(rngInitSeed + "data-critrollpairs.txt", finalFrame + ": 0x" + subHex + "\n\n");
                                }
                            }
                            subLoopCount = 1; //Resets the subcalculation loop counter to 1
                        }
                    }
                }
            }
            Console.WriteLine("Press enter to continue.");
            Console.ReadLine();
            //------------------------- End of calculation and display ------------------------- 
        }
    }
}
