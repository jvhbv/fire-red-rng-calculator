using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Globalization;
using System.IO;

namespace gen3RNGcalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        string rngInitSeed;
        string rngBaseNumOne = "41C64E6D"; //First number in RNG equation
        string rngBaseNumTwo = "6073"; //Second fixed number in RNG equation
        int InitSeed;
        int repeatTimes = 0;
        int subCalc; //Initializes an integer that's used in the rollSearch loops
        int subLoopCount = 1; //Initializes another integer that's used in the rollSearch loops
        int rollCalculation;
        string subHex = "FFFFFFFF";
        int rollParsed = 0;
        int gameVar = 0;
        int finalFrame;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool? critSearch = crits.IsChecked;
            bool? rollSearch = rolls.IsChecked;
            bool? gameSelect1 = game1.IsChecked;

            Results win2 = new Results();
            win2.Show();

            if (gameSelect1 == true)
            {
                gameVar = 5;
            }
            else { gameVar = 7; }

            bool initSeedParse = false;
            while (initSeedParse == false) //Makes sure that as long as you enter characters that are not hex numbers you will be asked this question again
            {
                rngInitSeed = seedInput.Text;
                initSeedParse = int.TryParse(rngInitSeed, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out InitSeed); //Attempts to parse the input, setting initSeedParse to true if it succeeds
                if (initSeedParse) { break; }
                win2.output.Text = "Please enter a hex value.";
            }
            bool repeatInputParse = false; //Initializes a boolean for the TryParse while loop for the number of times to repeat
            while (repeatInputParse == false) //Makes sure that as long as you enter characters that are not decimals you will be asked this question again
            {
                string repeat = repeatInput.Text;
                repeatInputParse = int.TryParse(repeat, out repeatTimes); //Attempts to parse the input, setting repeatInputParse to true if it succeeds
                if (repeatInputParse) { break; }
                win2.output.Text = "Please enter an integer.";
            }


            int BaseNumOne = int.Parse(rngBaseNumOne, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumOne
            int BaseNumTwo = int.Parse(rngBaseNumTwo, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumTwo
            int firstCalc = BaseNumOne * InitSeed + BaseNumTwo; //Calculates the first RNG result
            int repeated = 1; //Initializes the amount of times the loop has been repeated
            string hexResult = firstCalc.ToString("X8"); //Converts firstCalc back to hex

            if (critSearch == false) //Checks if critSearch is false and if so, just runs the program with no changes
            {
                win2.output.Text = "1: 0x" + hexResult;
            }
            if (critSearch == true && hexResult[3] == '0') //Checks if critSearch is set to true and if the 4th character in hexResult is 0
            {
                if (rollSearch == false) //Checks if rollSearch is set to false and if so, runs the normal critSearch loop
                {
                    Console.WriteLine("1: 0x" + hexResult);
                }
                if (rollSearch == true) //Checks if rollSearch is set to true and if so, runs a subcalculation in order to check if the second value in part of a pair also meets the requirements
                {
                    bool minRollParse = false;
                    while (minRollParse == false) //Makes sure that as long as you enter characters that are not decimals you will be asked this question again
                    {
                        string minRoll = rollMin.Text; //Gets the minimum roll you will accept
                        minRollParse = int.TryParse(minRoll, out rollParsed); //Attempts to parse the input, setting minRollParse to true if it succeeds
                        if (minRollParse) { break; }
                    }
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
                    }
                    subLoopCount = 1; //Resets the subcalculation loop counter to 1
                }
            }

            while (repeated < repeatTimes) //Loop function
            {
                firstCalc = BaseNumOne * firstCalc + BaseNumTwo; //Does the equation again
                hexResult = firstCalc.ToString("X8");
                repeated++;
                if (critSearch == false) //Checks if critSearch is false and if so, just runs the program with no changes
                {
                    win2.output.Text = win2.output.Text + "\n" + repeated + ": 0x" + hexResult;
                    win2.output.Height = win2.output.Height + 14;
                }
                if (critSearch == true && hexResult[3] == '0') //Checks if critSearch is set to true and if the 4th character in hexResult is 0
                {
                    if (rollSearch == false) //Checks if rollSearch is set to false and if so, runs the normal critSearch loop
                    {
                        win2.output.Text = win2.output.Text + "\n" + repeated + ": 0x" + hexResult;
                        win2.output.Height = win2.output.Height + 14;
                    }
                    if (rollSearch == true) //Checks if rollSearch is set to true and if so, runs a subcalculation in order to check if the second value in part of a pair also meets the requirements
                    {
                        bool minRollParse = false;
                        while (minRollParse == false) //Makes sure that as long as you enter characters that are not decimals you will be asked this question again
                        {
                            string minRoll = rollMin.Text;
                            minRollParse = int.TryParse(minRoll, out rollParsed); //Attempts to parse the input, setting minRollParse to true if it succeeds
                            if (minRollParse) { break; }
                        }
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
                            win2.output.Text = win2.output.Text + "\n" + repeated + ": 0x" + hexResult;
                            finalFrame = repeated + gameVar;
                            win2.output.Text = win2.output.Text + "\n" + finalFrame + ": 0x" + subHex + "\n";
                            win2.output.Height = win2.output.Height + 42;
                        }
                        subLoopCount = 1; //Resets the subcalculation loop counter to 1
                    }
                }
            }
        }
    }
}
