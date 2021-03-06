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
using System.Diagnostics;
using System.Reflection;

/*
    Pokémon Generation 3 RNG Calculator
    Copyright (C) 2020  Joseph Van Horn

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

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
            bool? gameSelect1 = game1.IsChecked; //The Ruby/Sapphire/Emerald button does nothing right now, pretty much only there for looks.

            if (gameSelect1 == true)
            {
                gameVar = 5; //sets gameVar to 5 if FR/LG is checked
            }
            else { gameVar = 7; } //sets gameVar to 7 if FR/LG is not checked

            rngInitSeed = seedInput.Text;
            bool initSeedParse = int.TryParse(rngInitSeed, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out InitSeed); //Attempts to parse the input, setting initSeedParse to true if it succeeds
            if (initSeedParse == false) //makes sure you don't enter something that will crash the program
            {
                exception.Text = "Please enter a hexadecimal value for the initial seed.";
            }

            string repeat = repeatInput.Text;
            bool repeatInputParse = int.TryParse(repeat, out repeatTimes); //Attempts to parse the input, setting repeatInputParse to true if it succeeds
            if (repeatInputParse == false) //makes sure you don't enter something that will crash the program
            {
                exception.Text = "Please enter an integer for the maximum number of times to repeat.";
            }

            string minRepeat = repeatMinimum.Text;
            bool minInputParse = int.TryParse(minRepeat, out minimumRepeat); //Attempts to parse the input, setting minInputParse to true if it succeeds
            if (minInputParse == false) //makes sure you don't enter something that will crash the program
            {
                exception.Text = "Please enter an integer for the minimum frame you want displayed.";
            }

            bool minleqmax = minimumRepeat <= repeatTimes;
            if (minleqmax == false) //makes sure you don't enter a reduntant minimum amount of times to repeat
            {
                exception.Text = "Please make sure the minimum number to display is less than or equal to the maximum.";
            }
            
            if (rollSearch == true)
            {
                string rollSelect = rollMin.Text;
                rollParse = int.TryParse(rollSelect, out rollParsed); //Attempts to parse the input, setting rollParse to true if it succeeds
                if (rollParse == false) //makes sure you don't enter something that will crash the program
                {
                    exception.Text = "Please enter an integer for the roll you want to search for.";
                }
            }

            if (initSeedParse && repeatInputParse && minInputParse && minleqmax && rollSearch == false ^ (rollSearch == true && rollParse)) //verifies you passed all checks up to this point for what you are searching for
            {
                Results win2 = new Results(); //creates a new instance of the results window
                win2.Show(); //shows the results window
                int BaseNumOne = int.Parse(rngBaseNumOne, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumOne
                int BaseNumTwo = int.Parse(rngBaseNumTwo, NumberStyles.HexNumber); //Sets an integer equal to the parsed value of rngBaseNumTwo
                int firstCalc = BaseNumOne * InitSeed + BaseNumTwo; //Calculates the first RNG result
                int repeated = 1; //Initializes the amount of times the loop has been repeated
                string hexResult = firstCalc.ToString("X8"); //Converts firstCalc back to hex

                if (critSearch == false && rollSearch == false) //Checks if critSearch and rollSearch is false and if so, just runs the program with no changes
                {
                    if (minimumRepeat <= repeated) //Only prints the result if it falls within the minimum frame to display
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
                        while (subLoopCount <= gameVar) //Uses gameVar to determine how many times the subcalculation should be ran before checking the calculated number
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
                else if (rollSearch == true && critSearch == false && int.Parse(hexResult.Substring(3,1), NumberStyles.HexNumber) <= rollParsed) //Checks if you want to search for rolls but not crits, and if it should print the result
                {
                    if (minimumRepeat <= repeated)
                    {
                        win2.output.Text = repeated + ": 0x" + hexResult;
                    }
                }

                while (repeated < repeatTimes) //Loop function, it might be beneficial for reading the code to change this to a for() loop in the future
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
                            while (subLoopCount <= gameVar) //This could also benefit from being a for() loop in terms of readability, but due to how short the loop already is, it is a lesser issue
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
                        if (minimumRepeat <= repeated)
                        {
                            win2.output.Text = win2.output.Text + "\n" + repeated + ": 0x" + hexResult;
                            win2.output.Height = win2.output.Height + 14;
                        }
                    }
                }
            }
        }

        private void Help(object sender, RoutedEventArgs e)
        {
            Help help = new Help();
            help.Show();
        }

        private void License(object sender, RoutedEventArgs e)
        {
            var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var file = System.IO.Path.Combine(directory, "COPYING.txt");
            Process.Start(file);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            rsSeedFinder seedFinder = new rsSeedFinder();
            seedFinder.Show();
            Close();
        }

        private void DMGCalc(object sender, RoutedEventArgs e)
        {
            damageCalc damage = new damageCalc();
            damage.Show();
        }
    }
}
