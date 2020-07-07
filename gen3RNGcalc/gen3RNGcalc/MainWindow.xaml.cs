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
using System.ComponentModel;
using System.Threading;

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
        int minimumRepeat = 0;
        bool rollParse = false;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bool? critSearch = crits.IsChecked;
            bool? rollSearch = rolls.IsChecked;
            bool? gameSelect1 = game1.IsChecked;

            if (gameSelect1 == true)
            {
                gameVar = 5;
            }
            else { gameVar = 7; }

            rngInitSeed = seedInput.Text;
            bool initSeedParse = int.TryParse(rngInitSeed, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out InitSeed); //Attempts to parse the input, setting initSeedParse to true if it succeeds
            if (initSeedParse == false)
            {
                exception.Text = "Please enter a hexadecimal value for the initial seed.";
            }

            string repeat = repeatInput.Text;
            bool repeatInputParse = int.TryParse(repeat, out repeatTimes); //Attempts to parse the input, setting repeatInputParse to true if it succeeds
            if (repeatInputParse == false)
            {
                exception.Text = "Please enter an integer for the maximum number of times to repeat.";
            }

            string minRepeat = repeatMinimum.Text;
            bool minInputParse = int.TryParse(minRepeat, out minimumRepeat); //Attempts to parse the input, setting minInputParse to true if it succeeds
            if (minInputParse == false)
            {
                exception.Text = "Please enter an integer for the minimum frame you want displayed.";
            }

            bool minleqmax = minimumRepeat <= repeatTimes;
            if (minleqmax == false)
            {
                exception.Text = "Please make sure the minimum number to display is less than or equal to the maximum.";
            }
            
            if (rollSearch == true)
            {
                string rollSelect = rollMin.Text;
                rollParse = int.TryParse(rollSelect, out rollParsed); //Attempts to parse the input, setting minInputParse to true if it succeeds
                if (rollParse == false)
                {
                    exception.Text = "Please enter an integer for the roll you want to search for.";
                }
            }

            if (initSeedParse && repeatInputParse && minInputParse && minleqmax && rollSearch == false ^ rollSearch == true && rollParse)
            {
                Results win2 = new Results();
                win2.Show();
                int BaseNumOne = int.Parse(rngBaseNumOne, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumOne
                int BaseNumTwo = int.Parse(rngBaseNumTwo, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumTwo
                int firstCalc = BaseNumOne * InitSeed + BaseNumTwo; //Calculates the first RNG result
                int repeated = 1; //Initializes the amount of times the loop has been repeated
                string hexResult = firstCalc.ToString("X8"); //Converts firstCalc back to hex

                if (critSearch == false && rollSearch == false) //Checks if critSearch is false and if so, just runs the program with no changes
                {
                    if (minimumRepeat <= repeated)
                    {
                        win2.output.Text = "1: 0x" + hexResult;
                    }
                }
                else if (critSearch == true && hexResult[3] == '0') //Checks if critSearch is set to true and if the 4th character in hexResult is 0
                {
                    if (rollSearch == false) //Checks if rollSearch is set to false and if so, runs the normal critSearch loop
                    {
                        if (minimumRepeat <= repeated)
                        {
                            win2.output.Text = "1: 0x" + hexResult;
                        }
                    }
                    if (rollSearch == true) //Checks if rollSearch is set to true and if so, runs a subcalculation in order to check if the second value in part of a pair also meets the requirements
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
                            if (minimumRepeat <= repeated)
                            {
                                win2.output.Text = repeated + ": 0x" + hexResult;
                                finalFrame = repeated + gameVar;
                                win2.output.Text = win2.output.Text + "\n" + finalFrame + ": 0x" + subHex + "\n";
                            }
                        }
                        subLoopCount = 1; //Resets the subcalculation loop counter to 1
                    }
                }
                else if (rollSearch == true && critSearch == false && int.Parse(hexResult.Substring(3,1), NumberStyles.HexNumber) <= rollParsed)
                {
                    win2.output.Text = repeated + ": 0x" + hexResult;
                }

                while (repeated < repeatTimes) //Loop function
                {
                    firstCalc = BaseNumOne * firstCalc + BaseNumTwo; //Does the equation again
                    hexResult = firstCalc.ToString("X8");
                    repeated++;
                    if (critSearch == false && rollSearch == false) //Checks if critSearch is false and if so, just runs the program with no changes
                    {
                        if (minimumRepeat <= repeated)
                        {
                            win2.output.Text = win2.output.Text + "\n" + repeated + ": 0x" + hexResult;
                            win2.output.Height = win2.output.Height + 14;
                        }
                    }
                    else if (critSearch == true && hexResult[3] == '0') //Checks if critSearch is set to true and if the 4th character in hexResult is 0
                    {
                        if (rollSearch == false) //Checks if rollSearch is set to false and if so, runs the normal critSearch loop
                        {
                            if (minimumRepeat <= repeated)
                            {
                                win2.output.Text = win2.output.Text + "\n" + repeated + ": 0x" + hexResult;
                                win2.output.Height = win2.output.Height + 14;
                            }
                        }
                        if (rollSearch == true) //Checks if rollSearch is set to true and if so, runs a subcalculation in order to check if the second value in part of a pair also meets the requirements
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
                                if (minimumRepeat <= repeated)
                                {
                                    win2.output.Text = win2.output.Text + "\n" + repeated + ": 0x" + hexResult;
                                    finalFrame = repeated + gameVar;
                                    win2.output.Text = win2.output.Text + "\n" + finalFrame + ": 0x" + subHex + "\n";
                                    win2.output.Height = win2.output.Height + 42;
                                }
                            }
                            subLoopCount = 1; //Resets the subcalculation loop counter to 1
                        }
                    }
                    else if (rollSearch == true && critSearch == false && int.Parse(hexResult.Substring(3, 1), NumberStyles.HexNumber) <= rollParsed)
                    {
                        win2.output.Text = win2.output.Text + "\n" + repeated + ": 0x" + hexResult;
                        win2.output.Height = win2.output.Height + 14;
                    }
                }
            }
        }

        private void Help(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.Show();
        }
    }
}
