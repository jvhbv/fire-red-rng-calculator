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
using System.Windows.Shapes;
using System.Globalization;

namespace gen3RNGcalc
{
    /// <summary>
    /// Interaction logic for rsSeedFinder.xaml
    /// </summary>
    public partial class rsSeedFinder : Window
    {
        public rsSeedFinder()
        {
            InitializeComponent();
        }

        private void Go(object sender, RoutedEventArgs e)
        {
            error.Text = ""; //resets the error log upon running again
            bool hoursParse = double.TryParse(hour.Text, out double hours);
            if (!hoursParse) //makes sure you don't enter something that will crash the application
            {
                error.Text = error.Text + "Please enter an integer for hours.\n";
            }
            if (hours > 23) //makes sure you enter a number of hours less than or equal to the maximum in a day
            {
                error.Text = error.Text + "Please make sure the hours entered are not greater than 23.\n";
            }
            bool minutesParse = double.TryParse(minute.Text, out double minutes);
            if (!minutesParse) //makes sure you don't enter something that will crash the application
            {
                error.Text = error.Text + "Please enter an integer for minutes.\n";
            }
            if (minutes > 59) //makes sure you enter a number of minutes less than or equal to the maximum in an hour
            {
                error.Text = error.Text + "Please make sure the minutes entered are not greater than 59.";
            }
            if (minutesParse && hoursParse && minutes <= 59 && hours <= 23) //verifies that the inputs received passed all checks leading up to this point
            {
                var date = dateSelected.SelectedDate.Value.Date.AddMinutes(minutes); //creates a variable for the date received and sets the minutes for the date to the minutes input
                date = date.AddHours(hours); //sets the hours for the date to the hours input
                int result = (int)(date.Subtract(new DateTime(2000, 1, 1))).TotalMinutes; //gets the total number of minutes passed since January 1st, 2000
                int maxSeed = int.Parse("FFFF", NumberStyles.HexNumber); //just here to make the math easier to follow
                int remainder = result % maxSeed; //finalizes the number of minutes that actually matter for the seed generation
                int defaultSeed = int.Parse("5A0", NumberStyles.HexNumber); //also here to make the math easier to follow, it just declares 05A0 as the default initial seed
                int finalResult = defaultSeed + remainder; //finalizes the calculation of the initial seed
                string resultOut = finalResult.ToString("X4"); //converts the final result to the 16 bit hexadecimal number representing the initial seed
                seed.Text = resultOut; //outputs the final result as the aforementioned 16 bit hexadecimal number
                copy.Visibility = Visibility.Visible; //makes the copy button visible
            }
        }

        private void Copy_Click(object sender, RoutedEventArgs e)
        {
            MainWindow win2 = new MainWindow(); //creates a new instance of MainWindow
            win2.seedInput.Text = seed.Text; //sets the seed input field to the one generated
            win2.Show(); //shows the new instance of MainWindow
            Close(); //closes the seed finder
        }
    }
}
