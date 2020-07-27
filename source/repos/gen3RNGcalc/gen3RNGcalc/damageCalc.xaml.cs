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
using System.IO;
using System.Reflection;

namespace gen3RNGcalc
{
    /// <summary>
    /// Interaction logic for damageCalc.xaml
    /// </summary>
    public partial class damageCalc : Window
    {
        /// <summary>
        /// The variable that ParseInput returns
        /// </summary>
        public static int parsedValue;
        /// <summary>
        /// Any error messages get stored in this variable
        /// </summary>
        public static string alerts;
        /// <summary>
        /// The boolean ParseInput uses to store the TryParse result
        /// </summary>
        public static bool tryIt;
        /// <summary>
        /// The current weather conditions are stored in this variable
        /// </summary>
        public static string weather;
        /// <summary>
        /// The variable that stores the index number of the defenders' ability
        /// </summary>
        public static int defAbility;
        /// <summary>
        /// Used to determine if the ItemPowerBoost loop should exit early
        /// </summary>
        public static bool leaveLoop = false;
        /// <summary>
        /// Stores either null, +, or - depending on the defense modifier on the nature of the defender
        /// </summary>
        public static string defNatureMod = "";
        /// <summary>
        /// Stores either null, +, or - depending on the special defense modifier on the nature of the defender
        /// </summary>
        public static string spDefNatureMod = "";
        /// <summary>
        /// Sets index 0 to true if you have the current HP textbox selected, false if it is not selected, and does the same for index 1 with the defenders' HP textbox
        /// </summary>
        public static bool[] editingBox = new bool[2] { false, false }; //Just combined the 2 different editingBox booleans into an array of booleans
        /// <summary>
        /// Array that contains all nature doubles, formatted as Atk, Def, SpAtk, SpDef, Speed
        /// </summary>
        public static double[] natureStuff = new double[5] { 1, 1, 1, 1, 1 }; //Combined all the nature doubles into an array of doubles
        public static string natureMod = "";
        public static string spAtkNatureMod = "";
        /// <summary>
        /// Array that contains all base stat integers, formatted as HP, Atk, Def, SpAtk, SpDef, Speed
        /// </summary>
        public static int[] baseStats = new int[6]; //Combined all the different base stat integers into one array
        /// <summary>
        /// Array that contains all EV integers, formatted as HP, Atk, Def, SpAtk, SpDef, Speed
        /// </summary>
        public static int[] allEVs = new int[6]; //Combined all the ev integers into one array
        /// <summary>
        /// Integer that stores the level of each mon temporarily during calculations
        /// </summary>
        public static int yourLevel;
        /// <summary>
        /// Array that contains all badge doubles, formatted as Atk, Def, SpAtk, SpDef, Speed
        /// </summary>
        public static double[] badges = new double[5] { 1, 1, 1, 1, 1 }; //Combined all badge doubles into one array

        /// <summary>
        /// Initialization of the window and csv file parsing
        /// </summary>
        public damageCalc()
        {
            InitializeComponent();

            //Comma separated value document parsing
            var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var file = System.IO.Path.Combine(directory, "baseStats.csv"); //Each line in baseStats.csv is formatted like so- Pokemon Name, Base HP, Base Atk, Base Def, Base SpA, Base SpD, Base Spe, Type 1, Type 2
            using (var reader = new StreamReader(file))
            {
                List<string> monsters = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    monsters.Add(values[0]);
                }
                foreach (string mons in monsters)
                {
                    monSelection1.Items.Add(mons); //Gets the names for each pokemon and adds it to the monSelection1 comboBox
                    monSelection2.Items.Add(mons); //Gets the names for each pokemon and adds it to the monSelection2 comboBox
                }
            }
            file = System.IO.Path.Combine(directory, "attacks.csv"); //Each line in attacks.csv is formatted like so- Attack Name, Attack Type, Base Power, Comment
            using (var reader = new StreamReader(file))
            {
                List<string> attacks = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    attacks.Add(values[0]);
                }
                foreach (string attack in attacks)
                {
                    attackName.Items.Add(attack); //Gets the names for each attack and adds it to the attackName comboBox
                }
            }
            file = System.IO.Path.Combine(directory, "abilities.csv"); //Each line is simply just formatted like so- Ability Name, Ability Description
            using (var reader = new StreamReader(file))
            {
                List<string> abilities = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    abilities.Add(values[0]);
                }
                foreach (string ability in abilities)
                {
                    ability1.Items.Add(ability); //Gets the names for each ability and adds it to the ability1 comboBox
                    ability2.Items.Add(ability); //Gets the names for each ability and adds it to the ability2 comboBox
                }
            }
            file = System.IO.Path.Combine(directory, "items.csv"); //Each line is formatted as in game index number, item name, description
            using (var reader = new StreamReader(file))
            {
                List<string> items = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    items.Add(values[1]);
                }
                foreach (string item in items)
                {
                    item1.Items.Add(item); //Gets the names for each item and adds it to the item1 comboBox
                    item2.Items.Add(item); //Gets the names for each item and adds it to the item2 comboBox
                }
            }
            //End csv parsing

            Calculate(); //Runs The first calculation, initializing everything in the UI
        }

        /// <summary>
        /// Sets the values for each of the types, used in a variety of places in the code
        /// </summary>
        enum TypeValues
        {
            Normal,
            Fighting,
            Flying,
            Dark,
            Psychic,
            Fire,
            Water,
            Electric,
            Grass,
            Bug,
            Ground,
            Ghost,
            Rock,
            Steel,
            Dragon,
            Ice,
            Poison,
            None
        }

        /// <summary> A quicker way to calculate stat changes on the fly </summary>
        /// <param name="findBuff">The SelectedIndex of the comboBox you want to search for, e.g. atkBuffs1.SelectedIndex</param>
        public static double Buffs(int findBuff)
        {
            if (findBuff <= 6)
            {
                return 4 - (findBuff * 0.5); //Calculates positive and neutral stat changes
            }
            else
            {
                return 1 / (1 + ((findBuff - 6) * 0.5)); //Calculates detrimental stat changes
            }
        }
        
        /// <summary>
        /// A quick way to parse the inputs of integers and return the parsed value
        /// </summary>
        /// <param name="inputNeeded">The text you want to parse, usually a TextBox.Text parameter</param>
        /// <param name="warning">The warning to display if the text cannot be parsed</param>
        /// <param name="tooHigh">The warning to display if the parsed value is too high</param>
        /// <param name="maxValue">The maximum value to be accepted</param>
        /// <returns>The parsed value of the string input if it can be parsed, 0 as a failsafe if it can't</returns>
        public static int ParseInput(string inputNeeded, string warning, string tooHigh, int maxValue)
        {
            tryIt = int.TryParse(inputNeeded, out parsedValue);
            if (!tryIt)
            {
                alerts = alerts + warning;
                return 0;
            }
            else if (parsedValue > maxValue)
            {
                alerts = alerts + tooHigh;
                tryIt = false;
                return 0;
            }
            else
            {
                return parsedValue;
            }
        }

        /// <summary>
        /// Calculates the type effectiveness of the move
        /// </summary>
        /// <param name="moveType">The type value of the move to be used, according to TypeValues</param>
        /// <param name="defendingType1">The first type of the defending pokemon, using TypeValues for the value</param>
        /// <param name="defendingType2">The second type of the defending pokemon, using TypeValues for the value</param>
        /// <returns>The product of the type matchup for the move type against the first type and the second type</returns>
        public static double effectiveness(int moveType, int defendingType1, int defendingType2)
        {
            var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var file = System.IO.Path.Combine(directory, "typeMatchups.csv");
            using (var reader = new StreamReader(file))
            {
                List<string> defType1 = new List<string>();
                List<string> defType2 = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    defType1.Add(values[Convert.ToInt32(defendingType1)]);
                    defType2.Add(values[Convert.ToInt32(defendingType2)]);
                }
                string defenderType = defType1[Convert.ToInt32(moveType)];
                string defenderType1 = defType2[Convert.ToInt32(moveType)];
                double finalDefender1 = double.Parse(defenderType);
                double finalDefender2 = double.Parse(defenderType1);
                if (defAbility == 75 && (finalDefender1 * finalDefender2) < 2) { return 0; } //Wonder guard handler
                if (defAbility == 16 && moveType == 5) { return 0; } //Flash fire handler
                if (defAbility == 28 && moveType == 10) { return 0; } //Levitate handler
                if ((defAbility == 29 || defAbility == 71) && moveType == 10) { return 0; } //Lightning rod / Volt absorb handler
                if (defAbility == 72 && moveType == 6) { return 0; } //Water absorb handler
                return finalDefender1 * finalDefender2;
            }
        }

        /// <summary>
        /// Run a quick check for all the type boosting items
        /// </summary>
        /// <param name="boostFactor">How much the base power should be boosted</param>
        /// <param name="item">The index number of the item you are looking for</param>
        /// <param name="type">The type of the move you are looking for</param>
        public int ItemPowerBoost(double boostFactor, int item, string type)
        {
            //get the type to boost and then multiply the base power by the correct amount
            TypeValues types = new TypeValues();
            types = (TypeValues)Enum.Parse(typeof(TypeValues), type);
            int typeWanted = Convert.ToInt32(types);
            if (item1.SelectedIndex == item && moveTypeSelection.SelectedIndex == typeWanted)
            {
                double calculated = double.Parse(basePower1.Text) * boostFactor;
                leaveLoop = true;
                return Convert.ToInt32(Math.Floor(calculated));
            }
            else { return int.Parse(basePower1.Text); }
        }

        /// <summary>
        /// Runs Calculate() when the Go button is pressed
        /// </summary>
        private void Go(object sender, RoutedEventArgs e)
        {
            Calculate();
        }

        /// <summary>
        /// Controls the bar that contains the visual representation of the damage done
        /// </summary>
        /// <param name="minPercentHP">Set to minPercentHP, only applicable after final calculations are complete</param>
        /// <param name="maxPercentHP">Set to maxPercentHP, only applicable after final calculations are complete</param>
        public void VisualHP(double minPercentHP, double maxPercentHP)
        {
            if (minPercentHP > 100)
            {
                minPercentHP = 100;
            }
            if (maxPercentHP > 100)
            {
                maxPercentHP = 100;
            }
            LinearGradientBrush representation = new LinearGradientBrush
            {
                StartPoint = new Point(0, 0)
            };
            GradientStop initialDamage = new GradientStop
            {
                Color = Colors.ForestGreen,
                Offset = 0.0
            };
            representation.GradientStops.Add(initialDamage);
            GradientStop minimum = new GradientStop
            {
                Color = Colors.ForestGreen,
                Offset = minPercentHP / 100
            };
            representation.GradientStops.Add(minimum);
            GradientStop valsInBetween = new GradientStop
            {
                Color = Colors.LimeGreen,
                Offset = minimum.Offset
            };
            representation.GradientStops.Add(valsInBetween);
            GradientStop maximum = new GradientStop
            {
                Color = Colors.LimeGreen,
                Offset = maxPercentHP / 100
            };
            representation.GradientStops.Add(maximum);
            GradientStop empty = new GradientStop
            {
                Color = Colors.White,
                Offset = maximum.Offset
            };
            representation.GradientStops.Add(empty);
            GradientStop emptyEnd = new GradientStop
            {
                Color = Colors.White,
                Offset = 1.0
            };
            representation.GradientStops.Add(emptyEnd);
            visualDamage.Fill = representation;
        }

        /// <summary>
        /// Used to determine nature stat changes
        /// </summary>
        /// <param name="attacker">True if calculating for the attacker, false if for the defender</param>
        /// <param name="natureBox">nature1 if attacker, nature2 if defender</param>
        public void NatureDeterm(bool attacker, ComboBox natureBox)
        {
            string nature = natureBox.Text.Substring(0, natureBox.Text.IndexOf(" "));
            if (nature == "Lonely" || nature == "Adamant" || nature == "Naughty" || nature == "Brave")
            {
                natureStuff[0] = 1.1;
                if (attacker)
                {
                    natureMod = "+";
                }
            }
            if (nature == "Bold" || nature == "Modest" || nature == "Calm" || nature == "Timid")
            {
                natureStuff[0] = 0.9;
                if (attacker)
                {
                    natureMod = "-";
                }
            }
            if (nature == "Bold" || nature == "Impish" || nature == "Lax" || nature == "Relaxed")
            {
                natureStuff[1] = 1.1;
                if (!attacker)
                {
                    defNatureMod = "+";
                }
            }
            if (nature == "Lonely" || nature == "Mild" || nature == "Gentle" || nature == "Hasty")
            {
                natureStuff[1] = 0.9;
                if (!attacker)
                {
                    defNatureMod = "-";
                }
            }
            if (nature == "Modest" || nature == "Mild" || nature == "Rash" || nature == "Quiet")
            {
                natureStuff[2] = 1.1;
                if (attacker)
                {
                    spAtkNatureMod = "+";
                }
            }
            if (nature == "Adamant" || nature == "Impish" || nature == "Careful" || nature == "Jolly")
            {
                natureStuff[2] = 0.9;
                if (attacker)
                {
                    spAtkNatureMod = "-";
                }
            }
            if (nature == "Calm" || nature == "Gentle" || nature == "Careful" || nature == "Sassy")
            {
                natureStuff[3] = 1.1;
                if (!attacker)
                {
                    spDefNatureMod = "+";
                }
            }
            if (nature == "Naughty" || nature == "Lax" || nature == "Rash" || nature == "Naive")
            {
                natureStuff[3] = 0.9;
                if (!attacker)
                {
                    spDefNatureMod = "-";
                }
            }
            if (nature == "Timid" || nature == "Hasty" || nature == "Jolly" || nature == "Naive")
            {
                natureStuff[4] = 1.1;
            }
            if (nature == "Brave" || nature == "Relaxed" || nature == "Quiet" || nature == "Sassy")
            {
                natureStuff[4] = 0.9;
            }
        }

        /// <summary>
        /// All the calculations happen here
        /// </summary>
        private void Calculate()
        {
            alerts = "";
            alert.Text = alerts;
            finalCalc.Text = "";
            finalHeader.Text = "";

            natureStuff = new double[5] { 1, 1, 1, 1, 1 }; //Rewritten to be in array form again, saving 4 manual variable assignments

            bool kanto = false;
            bool hoenn = false;

            int yourAbility = ability1.SelectedIndex;
            defAbility = ability2.SelectedIndex;

            if (noWeather.IsChecked == true) { weather = "clear"; }
            else if (raining.IsChecked == true) { weather = "rain"; }
            else if (sunny.IsChecked == true) { weather = "sun"; }
            else if (sandstorm.IsChecked == true) { weather = "sand"; }
            else { weather = "hail"; }

            if (yourAbility == 0 || defAbility == 0 || yourAbility == 6 || defAbility == 6) { weather = "neutral"; }
            if (weather != "neutral" && (yourAbility == 11 || defAbility == 11))
            {
                raining.IsChecked = true;
                weather = "rain";
            }
            if (weather != "neutral" && (yourAbility == 12 || defAbility == 12))
            {
                sunny.IsChecked = true;
                weather = "sun";
            }
            if (weather != "neutral" && (yourAbility == 49 || defAbility == 49))
            {
                sandstorm.IsChecked = true;
                weather = "sand";
            }

            bool? boulderBadge = Boulder.IsChecked;
            bool? soulBadge = Soul.IsChecked;
            bool? volcanoBadge = Volcano.IsChecked;
            bool? thunderBadge = Thunder.IsChecked;
            if (boulderBadge == true || soulBadge == true || volcanoBadge == true || thunderBadge == true)
            {
                kanto = true;
            }
            bool? stoneBadge = Stone.IsChecked;
            bool? balanceBadge = Balance.IsChecked;
            bool? mindBadge = Mind.IsChecked;
            bool? dynamoBadge = Dynamo.IsChecked;
            if (stoneBadge == true || balanceBadge == true || mindBadge == true || dynamoBadge == true)
            {
                hoenn = true;
            }
            if (kanto && hoenn)
            {
                alerts = alerts + "You can't have badges from both hoenn and kanto in the same game, dude.\n";
                alert.Text = alerts;
            }

            // Base stat input parsing
            baseStats[0] = ParseInput(baseHP1.Text, "Please enter a numerical value for the base HP.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool HPBaseParse = tryIt;
            alert.Text = alerts;
            baseStats[1] = ParseInput(baseAtk1.Text, "Please enter a numerical value for the base Attack.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool AtkBaseParse = tryIt;
            alert.Text = alerts;
            baseStats[2] = ParseInput(baseDef1.Text, "Please enter a numerical value for the base Defense.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool DefBaseParse = tryIt;
            alert.Text = alerts;
            baseStats[3] = ParseInput(baseSpAtk1.Text, "Please enter a numerical value for the base Special Attack.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpAtkBaseParse = tryIt;
            alert.Text = alerts;
            baseStats[4] = ParseInput(baseSpDef1.Text, "Please enter a numerical value for the base Special Defense.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpDefBaseParse = tryIt;
            alert.Text = alerts;
            baseStats[5] = ParseInput(baseSpeed1.Text, "Please enter a numerical value for the base Speed.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpeedBaseParse = tryIt;
            alert.Text = alerts;

            // EV input parsing
            allEVs[0] = ParseInput(HPEV1.Text, "Please enter a numerical value for the HP EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool HPEVParse = tryIt;
            alert.Text = alerts;
            allEVs[1] = ParseInput(AtkEV1.Text, "Please enter a numerical value for the Attack EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool AtkEVParse = tryIt;
            alert.Text = alerts;
            allEVs[2] = ParseInput(DefEV1.Text, "Please enter a numerical value for the Defense EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool DefEVParse = tryIt;
            alert.Text = alerts;
            allEVs[3] = ParseInput(SpAtkEV1.Text, "Please enter a numerical value for the Special Attack EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpAtkEVParse = tryIt;
            alert.Text = alerts;
            allEVs[4] = ParseInput(SpDefEV1.Text, "Please enter a numerical value for the Special Defense EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpDefEVParse = tryIt;
            alert.Text = alerts;
            allEVs[5] = ParseInput(SpeedEV1.Text, "Please enter a numerical value for the Speed EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpeedEVParse = tryIt;
            alert.Text = alerts;

            // Level input parsing
            yourLevel = ParseInput(level1.Text, "Please enter a numerical value for your level", "Pokémon have a maximum level of 100.\n", 100);
            bool levelParse = tryIt;
            alert.Text = alerts;

            // Nature determination, there might be a more efficient way of writing this, I'll look into it
            NatureDeterm(true, nature1);

            // Badge boost determination
            if (boulderBadge == true || stoneBadge == true) //attack boosting badges
            {
                badges[0] = 1.1;
            }
            if (soulBadge == true || balanceBadge == true) //defense boosting badges
            {
                badges[1] = 1.1;
            }
            if (volcanoBadge == true || mindBadge == true) //special attack and special defense boosting badges
            {
                badges[2] = 1.1;
                badges[3] = 1.1;
            }
            if (thunderBadge == true || dynamoBadge == true) //speed boosting badges
            {
                badges[4] = 1.1;
            }

            if (HPBaseParse && AtkBaseParse && DefBaseParse && SpAtkBaseParse && SpDefBaseParse && SpeedBaseParse && HPEVParse && AtkEVParse && DefEVParse && SpAtkEVParse && SpDefEVParse && SpeedEVParse && levelParse)
            {
                if ((kanto ^ hoenn) || !kanto && !hoenn)
                {
                    StatCalculation(true);

                    if (EnemyCalc()) //Makes sure that all parsing done by EnemyCalc was successful before trying to calculate anything
                    {
                        if (yourAbility != defAbility && (yourAbility == 11 || yourAbility == 12 || yourAbility == 49) && (defAbility == 11 || defAbility == 12 || defAbility == 49)) //Finds which ability weather should be on the field
                        {
                            if (int.Parse(SpeedTot1.Text) <= int.Parse(SpeedTot2.Text))
                            {
                                switch (yourAbility)
                                {
                                    case 11:
                                        raining.IsChecked = true;
                                        weather = "rain";
                                        break;
                                    case 12:
                                        sunny.IsChecked = true;
                                        weather = "sun";
                                        break;
                                    case 49:
                                        sandstorm.IsChecked = true;
                                        weather = "sand";
                                        break;
                                }
                            }
                            else
                            {
                                switch (defAbility)
                                {
                                    case 11:
                                        raining.IsChecked = true;
                                        weather = "rain";
                                        break;
                                    case 12:
                                        sunny.IsChecked = true;
                                        weather = "sun";
                                        break;
                                    case 49:
                                        sandstorm.IsChecked = true;
                                        weather = "sand";
                                        break;
                                }
                            }
                        }
                        if (attackName.SelectedIndex != 35 && attackName.SelectedIndex != 89 && attackName.SelectedIndex != 117 && attackName.SelectedIndex != 132 && !(attackName.SelectedIndex >= 162 && attackName.SelectedIndex <= 178) && attackName.SelectedIndex != 6)
                        {
                            int power = ParseInput(basePower1.Text, "Please enter a numerical value for the base power.", "A move's base power can not be higher than 255.", 255);
                            bool bpParse = tryIt;
                            alert.Text = alerts;

                            if (bpParse)
                            {
                                int[] items = new int[]
                                {
                                    ItemPowerBoost(1.1, 152, "Bug"),
                                    ItemPowerBoost(1.1, 163, "Steel"),
                                    ItemPowerBoost(1.1, 165, "Dragon"),
                                    ItemPowerBoost(1.1, 167, "Ground"),
                                    ItemPowerBoost(1.1, 168, "Rock"),
                                    ItemPowerBoost(1.1, 169, "Grass"),
                                    ItemPowerBoost(1.1, 170, "Dark"),
                                    ItemPowerBoost(1.1, 171, "Fighting"),
                                    ItemPowerBoost(1.1, 172, "Electric"),
                                    ItemPowerBoost(1.1, 173, "Water"),
                                    ItemPowerBoost(1.1, 174, "Flying"),
                                    ItemPowerBoost(1.1, 175, "Poison"),
                                    ItemPowerBoost(1.1, 176, "Ice"),
                                    ItemPowerBoost(1.1, 177, "Ghost"),
                                    ItemPowerBoost(1.1, 178, "Psychic"),
                                    ItemPowerBoost(1.1, 179, "Fire"),
                                    ItemPowerBoost(1.1, 180, "Dragon"),
                                    ItemPowerBoost(1.1, 181, "Normal"),
                                    ItemPowerBoost(1.05, 184, "Water")
                                };
                                for (int again = 0; again <= 17; again++)
                                {
                                    int originalPower = power;
                                    power = items[again];
                                    if (leaveLoop && power != originalPower) { break; } //checks to see if it should leave the loop early, and also makes sure that if it should, that the new power is different
                                }
                                leaveLoop = false;
                                finalCalc.Text = "Possible damage amounts: ";
                                double enemyCritDef = double.Parse(SpDefTot2.Text);
                                double CritAtk = double.Parse(SpAtkTot1.Text);
                                double firePower = 1;
                                double waterPower = 1;
                                double crit = 1;
                                double STAB = 1;
                                double effective = 1;
                                double minPercentHP = 0;
                                double maxPercentHP = 0;
                                double minDamage = 0;
                                double maxDamage = 0;
                                double screens = 1;
                                string criticalHit = ": ";
                                if (weather == "rain")
                                {
                                    firePower = 0.5;
                                    waterPower = 1.5;
                                }
                                if (weather == "sun")
                                {
                                    firePower = 1.5;
                                    waterPower = 0.5;
                                }
                                bool isPhysical = false;
                                int typeOfMove = moveTypeSelection.SelectedIndex;
                                if (typeOfMove == 0 || typeOfMove == 1 || typeOfMove == 2 || typeOfMove == 9 || typeOfMove == 10 || typeOfMove == 11 || typeOfMove == 12 || typeOfMove == 13 || typeOfMove == 16)
                                {
                                    isPhysical = true;
                                    enemyCritDef = double.Parse(DefTot2.Text);
                                    CritAtk = double.Parse(AtkTot1.Text);
                                }
                                if (forceCrit1.IsChecked == true && defAbility != 2 && defAbility != 54) //Checks if you want to force crits and makes sure the defender does not have battle armor or shell armor
                                {
                                    crit = 2;
                                    criticalHit = " on a critical hit: ";
                                    if (defBuffs2.SelectedIndex < 6 && isPhysical) //Ignores stat buffs to defenders defense
                                    {
                                        enemyCritDef = Math.Floor(double.Parse(DefTot2.Text) / Buffs(defBuffs2.SelectedIndex));
                                    }
                                    if (spDefBuffs2.SelectedIndex < 6 && !isPhysical) //Ignores stat buffs to defenders special defense
                                    {
                                        enemyCritDef = Math.Floor(double.Parse(SpDefTot2.Text) / Buffs(spDefBuffs2.SelectedIndex));
                                    }
                                    if (atkBuffs1.SelectedIndex > 6 && isPhysical) //Ignores stat drops to attackers attack
                                    {
                                        CritAtk = Math.Floor(double.Parse(AtkTot1.Text) / Buffs(atkBuffs1.SelectedIndex));
                                    }
                                    if (spAtkBuffs1.SelectedIndex > 6 && !isPhysical) //Ignores stat drops to attackers special attack
                                    {
                                        CritAtk = Math.Floor(double.Parse(SpAtkTot1.Text) / Buffs(spAtkBuffs1.SelectedIndex));
                                    }
                                }
                                if (forceCrit1.IsChecked == false && ((isPhysical && reflect.IsChecked == true) || (!isPhysical && lightScreen.IsChecked == true)))
                                {
                                    screens = 0.5;
                                }
                                if (moveTypeSelection.SelectedIndex == type1Selection1.SelectedIndex || moveTypeSelection.SelectedIndex == type2Selection1.SelectedIndex)
                                {
                                    STAB = 1.5;
                                }
                                effective = effectiveness(moveTypeSelection.SelectedIndex, type1Selection2.SelectedIndex, type2Selection2.SelectedIndex);
                                double modifier;
                                for (double roll = 85; roll <= 100; roll++)
                                {
                                    if (moveTypeSelection.SelectedIndex == 5) //handles the modifier for fire type attacks
                                    {
                                        modifier = roll * crit * firePower * effective * STAB * screens;
                                        if (ability1.SelectedIndex == 16 && ability1Active.IsChecked == true) { modifier *= 1.5; } //Flash fire handler
                                    }
                                    else if (moveTypeSelection.SelectedIndex == 6) //handles the modifier for water type attacks
                                    {
                                        modifier = roll * crit * waterPower * effective * STAB * screens;
                                    }
                                    else
                                    {
                                        modifier = roll * crit * effective * STAB * screens;
                                    }
                                    string comma = ", ";
                                    if (ability1.SelectedIndex == 3 || ability1.SelectedIndex == 38 || ability1.SelectedIndex == 63 || ability1.SelectedIndex == 67) //handles blaze, overgrow, swarm, and torrent
                                    {
                                        double currentHP = Convert.ToDouble(ParseInput(curHP.Text, "Please enter an integer for the current HP", "HP can be, at most, 714 with 255 base hp and max everything else", 714));
                                        bool curHPParse = tryIt;
                                        if (currentHP / double.Parse(HPTot1.Text) <= 1.0 / 3.0 && curHPParse)
                                        {
                                            switch (ability1.SelectedIndex)
                                            {
                                                case 3: //blaze
                                                    if (moveTypeSelection.SelectedIndex == 5)
                                                    {
                                                        CritAtk *= 1.5;
                                                    }
                                                    break;
                                                case 38: //overgrow
                                                    if (moveTypeSelection.SelectedIndex == 8)
                                                    {
                                                        CritAtk *= 1.5;
                                                    }
                                                    break;
                                                case 63: //swarm
                                                    if (moveTypeSelection.SelectedIndex == 9)
                                                    {
                                                        CritAtk *= 1.5;
                                                    }
                                                    break;
                                                case 67: //torrent
                                                    if (moveTypeSelection.SelectedIndex == 6)
                                                    {
                                                        CritAtk *= 1.5;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    double lvdouble = yourLevel;
                                    double damage = Math.Floor(Math.Floor(Math.Floor(2 * lvdouble / 5 + 2) * CritAtk * power / enemyCritDef) / 50); //Pulled directly from pokemon showdown's initial calculation, and adapted for C#
                                    damage += 2;
                                    damage = Math.Floor((damage * modifier) / 100); //There are a few odd cases where the calculation is off by 1, such as the max roll for 0 SpA Charizard Flamethrower vs. 0 SpD Venusaur, which results in 261, although the actual answer is 260
                                    if (roll == 85)
                                    {
                                        minPercentHP = (damage / double.Parse(HPTot2.Text)) * 100;
                                        minDamage = damage;
                                        finalCalc.Text = finalCalc.Text + " (";
                                    }
                                    if (roll == 100)
                                    {
                                        maxPercentHP = (damage / double.Parse(HPTot2.Text)) * 100;
                                        maxDamage = damage;
                                        comma = ")";
                                    }
                                    int finalDamage = Convert.ToInt32(Math.Floor(damage));
                                    finalCalc.Text = finalCalc.Text + finalDamage.ToString() + comma;
                                }

                                string firstMon = monSelection1.SelectedItem.ToString(); //monSelection1.Text;
                                string secondMon = monSelection2.SelectedItem.ToString(); //monSelection2.Text;
                                string moveName = attackName.SelectedItem.ToString(); //attackName.Text;
                                string defensiveBuffs;
                                string offensiveBuffs;
                                if (isPhysical)
                                {
                                    offensiveBuffs = atkBuffs1.Text + " ";
                                    defensiveBuffs = defBuffs2.Text + " ";
                                    if (offensiveBuffs == "-- ")
                                    {
                                        offensiveBuffs = "";
                                    }
                                    if (defensiveBuffs == "-- ")
                                    {
                                        defensiveBuffs = "";
                                    }
                                    VisualHP(minPercentHP, maxPercentHP);
                                    finalHeader.Text = offensiveBuffs + AtkEV1.Text + natureMod + " Atk " + firstMon + " " + moveName + " vs. " + defensiveBuffs + HPEV2.Text + " HP / " + DefEV2.Text + defNatureMod + " Def " + secondMon + criticalHit + minDamage.ToString() + "-" + maxDamage.ToString() + " (" + Math.Round(minPercentHP, 1).ToString() + " - " + Math.Round(maxPercentHP, 1).ToString() + "%)";
                                }
                                else
                                {
                                    offensiveBuffs = spAtkBuffs1.Text + " ";
                                    defensiveBuffs = spDefBuffs2.Text + " ";
                                    if (offensiveBuffs == "-- ")
                                    {
                                        offensiveBuffs = "";
                                    }
                                    if (defensiveBuffs == "-- ")
                                    {
                                        defensiveBuffs = "";
                                    }
                                    VisualHP(minPercentHP, maxPercentHP);
                                    finalHeader.Text = offensiveBuffs + SpAtkEV1.Text + spAtkNatureMod + " SpA " + firstMon + " " + moveName + " vs. " + defensiveBuffs + HPEV2.Text + " HP / " + SpDefEV2.Text + spDefNatureMod + " SpD " + secondMon + criticalHit + minDamage.ToString() + "-" + maxDamage.ToString() + " (" + Math.Round(minPercentHP, 1).ToString() + " - " + Math.Round(maxPercentHP, 1).ToString() + "%)";
                                }
                            }
                        }
                        else if (attackName.SelectedIndex == 6 || attackName.SelectedIndex == 166 || attackName.SelectedIndex == 169 || (attackName.SelectedIndex >= 170 && attackName.SelectedIndex <= 176)) //Calculates 2-5 hit moves
                        {
                            int power = ParseInput(basePower1.Text, "Please enter a numerical value for the base power.", "A move's base power can not be higher than 255.", 255);
                            bool bpParse = tryIt;
                            alert.Text = alerts;
                            oneHit.IsEnabled = false;

                            if (bpParse)
                            {
                                int[] items = new int[]
                                {
                                    ItemPowerBoost(1.1, 152, "Bug"),
                                    ItemPowerBoost(1.1, 163, "Steel"),
                                    ItemPowerBoost(1.1, 165, "Dragon"),
                                    ItemPowerBoost(1.1, 167, "Ground"),
                                    ItemPowerBoost(1.1, 168, "Rock"),
                                    ItemPowerBoost(1.1, 169, "Grass"),
                                    ItemPowerBoost(1.1, 170, "Dark"),
                                    ItemPowerBoost(1.1, 171, "Fighting"),
                                    ItemPowerBoost(1.1, 172, "Electric"),
                                    ItemPowerBoost(1.1, 173, "Water"),
                                    ItemPowerBoost(1.1, 174, "Flying"),
                                    ItemPowerBoost(1.1, 175, "Poison"),
                                    ItemPowerBoost(1.1, 176, "Ice"),
                                    ItemPowerBoost(1.1, 177, "Ghost"),
                                    ItemPowerBoost(1.1, 178, "Psychic"),
                                    ItemPowerBoost(1.1, 179, "Fire"),
                                    ItemPowerBoost(1.1, 180, "Dragon"),
                                    ItemPowerBoost(1.1, 181, "Normal"),
                                    ItemPowerBoost(1.05, 184, "Water")
                                };
                                for (int again = 0; again <= 17; again++)
                                {
                                    int originalPower = power;
                                    power = items[again];
                                    if (leaveLoop && power != originalPower) { break; } //checks to see if it should leave the loop early, and also makes sure that if it should, that the new power is different
                                }
                                leaveLoop = false;
                                finalCalc.Text = "Possible damage amounts per hit: ";
                                double enemyCritDef = double.Parse(SpDefTot2.Text);
                                double CritAtk = double.Parse(SpAtkTot1.Text);
                                double firePower = 1;
                                double waterPower = 1;
                                double crit = 1;
                                double STAB = 1;
                                double effective = 1;
                                double minPercentHP = 0;
                                double maxPercentHP = 0;
                                double minDamage = 0;
                                double maxDamage = 0;
                                double screens = 1;
                                string criticalHit = ": ";
                                string hits = " (" + hitsBox.Text + " hits) ";
                                if (weather == "rain")
                                {
                                    firePower = 0.5;
                                    waterPower = 1.5;
                                }
                                if (weather == "sun")
                                {
                                    firePower = 1.5;
                                    waterPower = 0.5;
                                }
                                bool isPhysical = false;
                                int typeOfMove = moveTypeSelection.SelectedIndex;
                                if (typeOfMove == 0 || typeOfMove == 1 || typeOfMove == 2 || typeOfMove == 9 || typeOfMove == 10 || typeOfMove == 11 || typeOfMove == 12 || typeOfMove == 13 || typeOfMove == 16)
                                {
                                    isPhysical = true;
                                    enemyCritDef = double.Parse(DefTot2.Text);
                                    CritAtk = double.Parse(AtkTot1.Text);
                                }
                                if (forceCrit1.IsChecked == true && defAbility != 2 && defAbility != 54) //Checks if you want to force crits and makes sure the defender does not have battle armor or shell armor
                                {
                                    crit = 2;
                                    criticalHit = " on a critical hit: ";
                                    if (defBuffs2.SelectedIndex < 6 && isPhysical) //Ignores stat buffs to defenders defense
                                    {
                                        enemyCritDef = Math.Floor(double.Parse(DefTot2.Text) / Buffs(defBuffs2.SelectedIndex));
                                    }
                                    if (spDefBuffs2.SelectedIndex < 6 && !isPhysical) //Ignores stat buffs to defenders special defense
                                    {
                                        enemyCritDef = Math.Floor(double.Parse(SpDefTot2.Text) / Buffs(spDefBuffs2.SelectedIndex));
                                    }
                                    if (atkBuffs1.SelectedIndex > 6 && isPhysical) //Ignores stat drops to attackers attack
                                    {
                                        CritAtk = Math.Floor(double.Parse(AtkTot1.Text) / Buffs(atkBuffs1.SelectedIndex));
                                    }
                                    if (spAtkBuffs1.SelectedIndex > 6 && !isPhysical) //Ignores stat drops to attackers special attack
                                    {
                                        CritAtk = Math.Floor(double.Parse(SpAtkTot1.Text) / Buffs(spAtkBuffs1.SelectedIndex));
                                    }
                                }
                                if (forceCrit1.IsChecked == false && ((isPhysical && reflect.IsChecked == true) || (!isPhysical && lightScreen.IsChecked == true)))
                                {
                                    screens = 0.5;
                                }
                                if (moveTypeSelection.SelectedIndex == type1Selection1.SelectedIndex || moveTypeSelection.SelectedIndex == type2Selection1.SelectedIndex)
                                {
                                    STAB = 1.5;
                                }
                                effective = effectiveness(moveTypeSelection.SelectedIndex, type1Selection2.SelectedIndex, type2Selection2.SelectedIndex);
                                double modifier;
                                for (double roll = 85; roll <= 100; roll++)
                                {
                                    if (moveTypeSelection.SelectedIndex == 5) //handles the modifier for fire type attacks
                                    {
                                        modifier = roll * crit * firePower * effective * STAB * screens;
                                        if (ability1.SelectedIndex == 16 && ability1Active.IsChecked == true) { modifier *= 1.5; } //Flash fire handler
                                    }
                                    else if (moveTypeSelection.SelectedIndex == 6) //handles the modifier for water type attacks
                                    {
                                        modifier = roll * crit * waterPower * effective * STAB * screens;
                                    }
                                    else
                                    {
                                        modifier = roll * crit * effective * STAB * screens;
                                    }
                                    string comma = ", ";
                                    if (ability1.SelectedIndex == 3 || ability1.SelectedIndex == 38 || ability1.SelectedIndex == 63 || ability1.SelectedIndex == 67) //handles blaze, overgrow, swarm, and torrent
                                    {
                                        double currentHP = Convert.ToDouble(ParseInput(curHP.Text, "Please enter an integer for the current HP", "HP can be, at most, 714 with 255 base hp and max everything else", 714));
                                        bool curHPParse = tryIt;
                                        if (currentHP / double.Parse(HPTot1.Text) <= 1.0 / 3.0 && curHPParse)
                                        {
                                            switch (ability1.SelectedIndex)
                                            {
                                                case 3: //blaze
                                                    if (moveTypeSelection.SelectedIndex == 5)
                                                    {
                                                        CritAtk *= 1.5;
                                                    }
                                                    break;
                                                case 38: //overgrow
                                                    if (moveTypeSelection.SelectedIndex == 8)
                                                    {
                                                        CritAtk *= 1.5;
                                                    }
                                                    break;
                                                case 63: //swarm
                                                    if (moveTypeSelection.SelectedIndex == 9)
                                                    {
                                                        CritAtk *= 1.5;
                                                    }
                                                    break;
                                                case 67: //torrent
                                                    if (moveTypeSelection.SelectedIndex == 6)
                                                    {
                                                        CritAtk *= 1.5;
                                                    }
                                                    break;
                                            }
                                        }
                                    }
                                    double lvdouble = yourLevel;
                                    double damage = Math.Floor(Math.Floor(Math.Floor(2 * lvdouble / 5 + 2) * CritAtk * power / enemyCritDef) / 50); //Pulled directly from pokemon showdown's initial calculation, and adapted for C#
                                    damage += 2;
                                    damage = Math.Floor((damage * modifier) / 100); //There are a few odd cases where the calculation is off by 1, such as the max roll for 0 SpA Charizard Flamethrower vs. 0 SpD Venusaur, which results in 261, although the actual answer is 260
                                    damage *= hitsBox.SelectedIndex + 1;
                                    if (roll == 85)
                                    {
                                        minPercentHP = (damage / double.Parse(HPTot2.Text)) * 100;
                                        minDamage = damage;
                                        finalCalc.Text = finalCalc.Text + " (";
                                    }
                                    if (roll == 100)
                                    {
                                        maxPercentHP = (damage / double.Parse(HPTot2.Text)) * 100;
                                        maxDamage = damage;
                                        comma = ")";
                                    }
                                    int finalDamage = Convert.ToInt32(Math.Floor(damage / (hitsBox.SelectedIndex + 1)));
                                    finalCalc.Text = finalCalc.Text + finalDamage.ToString() + comma;
                                }

                                string firstMon = monSelection1.Text;
                                string secondMon = monSelection2.Text;
                                string moveName = attackName.Text;
                                string defensiveBuffs;
                                string offensiveBuffs;
                                if (isPhysical)
                                {
                                    offensiveBuffs = atkBuffs1.Text + " ";
                                    defensiveBuffs = defBuffs2.Text + " ";
                                    if (offensiveBuffs == "-- ")
                                    {
                                        offensiveBuffs = "";
                                    }
                                    if (defensiveBuffs == "-- ")
                                    {
                                        defensiveBuffs = "";
                                    }
                                    VisualHP(minPercentHP, maxPercentHP);
                                    finalHeader.Text = offensiveBuffs + AtkEV1.Text + natureMod + " Atk " + firstMon + " " + moveName + hits + " vs. " + defensiveBuffs + HPEV2.Text + " HP / " + DefEV2.Text + defNatureMod + " Def " + secondMon + criticalHit + minDamage.ToString() + "-" + maxDamage.ToString() + " (" + Math.Round(minPercentHP, 1).ToString() + " - " + Math.Round(maxPercentHP, 1).ToString() + "%)";
                                }
                                else
                                {
                                    offensiveBuffs = spAtkBuffs1.Text + " ";
                                    defensiveBuffs = spDefBuffs2.Text + " ";
                                    if (offensiveBuffs == "-- ")
                                    {
                                        offensiveBuffs = "";
                                    }
                                    if (defensiveBuffs == "-- ")
                                    {
                                        defensiveBuffs = "";
                                    }
                                    VisualHP(minPercentHP, maxPercentHP);
                                    finalHeader.Text = offensiveBuffs + SpAtkEV1.Text + spAtkNatureMod + " SpA " + firstMon + " " + moveName + hits + " vs. " + defensiveBuffs + HPEV2.Text + " HP / " + SpDefEV2.Text + spDefNatureMod + " SpD " + secondMon + criticalHit + minDamage.ToString() + "-" + maxDamage.ToString() + " (" + Math.Round(minPercentHP, 1).ToString() + " - " + Math.Round(maxPercentHP, 1).ToString() + "%)";
                                }
                            }
                        }
                        else
                        {
                            finalCalc.Text = "Possible damage: ";
                            double effective = effectiveness(moveTypeSelection.SelectedIndex, type1Selection2.SelectedIndex, type2Selection2.SelectedIndex);
                            double damage;
                            string firstMon = monSelection1.Text;
                            string secondMon = monSelection2.Text;
                            string moveName = attackName.Text;
                            if (attackName.SelectedIndex == 89 || attackName.SelectedIndex == 117) //Calculates Seismic Toss and Night Shade damage
                            {
                                if (effective == 0) { damage = 0; } //Checks if the defending pokemon is immune to the attack, and sets damage to 0 if it is
                                else { damage = yourLevel; } //If the defender is not immune, then it will always take damage equal to the level of the pokemon using the attack
                            }
                            else if (attackName.SelectedIndex == 35) //Calculates Dragon Rage Damage
                            {
                                if (effective == 0) { damage = 0; } //Checks if the defending pokemon is immune to the attack, and sets damage to 0 if it is
                                else { damage = 40; } //If the defender is not immune, then it will always take 40 HP of damage
                            }
                            else if (attackName.SelectedIndex == 132) //Calculates Sonic Boom Damage
                            {
                                if (effective == 0) { damage = 0; } //Checks if the defending pokemon is immune to the attack, and sets damage to 0 if it is
                                else { damage = 20; } //If the defender is not immune, then it will always take 20 HP of damage
                            }
                            else //Calculates all other special circumstances (all the OHKO moves)
                            {
                                if (effective == 0 || defAbility == 61 || yourLevel < int.Parse(level2.Text)) { damage = 0; } //Checks if the defending pokemon is immune to the attack, and sets damage to 0 if it is
                                else { damage = double.Parse(HPTot2.Text); } //If the defender is not immune, then it will always take damage equal to its HP
                            }
                            double percentHP = (damage / double.Parse(HPTot2.Text)) * 100;
                            VisualHP(percentHP, percentHP);
                            finalHeader.Text = firstMon + " " + moveName + " vs. " + HPEV2.Text + " HP " + secondMon + ": " + damage.ToString() + " (" + Math.Round(percentHP, 1).ToString() + "%)";
                            finalCalc.Text = finalCalc.Text + damage.ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the defending pokemon's stats, and also determines if the stats could be parsed for the main calculation to continue
        /// </summary>
        /// <returns>returns true if everything could be parsed and calculated</returns>
        public bool EnemyCalc()
        {
            natureStuff = new double[5] { 1, 1, 1, 1, 1 }; //Rewritten to be in array form again, saving 4 manual variable assignments
            badges = new double[5] { 1, 1, 1, 1, 1 }; //Rewritten to be in array form again, saving 4 manual variable assignments

            int yourAbility = ability2.SelectedIndex;

            baseStats[0] = ParseInput(baseHP2.Text, "Please enter a numerical value for the base HP.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool HPBaseParse = tryIt;
            alert.Text = alerts;
            baseStats[1] = ParseInput(baseAtk2.Text, "Please enter a numerical value for the base Attack.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool AtkBaseParse = tryIt;
            alert.Text = alerts;
            baseStats[2] = ParseInput(baseDef2.Text, "Please enter a numerical value for the base Defense.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool DefBaseParse = tryIt;
            alert.Text = alerts;
            baseStats[3] = ParseInput(baseSpAtk2.Text, "Please enter a numerical value for the base Special Attack.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpAtkBaseParse = tryIt;
            alert.Text = alerts;
            baseStats[4] = ParseInput(baseSpDef2.Text, "Please enter a numerical value for the base Special Defense.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpDefBaseParse = tryIt;
            alert.Text = alerts;
            baseStats[5] = ParseInput(baseSpeed2.Text, "Please enter a numerical value for the base Speed.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpeedBaseParse = tryIt;
            alert.Text = alerts;

            // EV input parsing
            allEVs[0] = ParseInput(HPEV2.Text, "Please enter a numerical value for the HP EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool HPEVParse = tryIt;
            alert.Text = alerts;
            allEVs[1] = ParseInput(AtkEV2.Text, "Please enter a numerical value for the Attack EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool AtkEVParse = tryIt;
            alert.Text = alerts;
            allEVs[2] = ParseInput(DefEV2.Text, "Please enter a numerical value for the Defense EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool DefEVParse = tryIt;
            alert.Text = alerts;
            allEVs[3] = ParseInput(SpAtkEV2.Text, "Please enter a numerical value for the Special Attack EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpAtkEVParse = tryIt;
            alert.Text = alerts;
            allEVs[4] = ParseInput(SpDefEV2.Text, "Please enter a numerical value for the Special Defense EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpDefEVParse = tryIt;
            alert.Text = alerts;
            allEVs[5] = ParseInput(SpeedEV2.Text, "Please enter a numerical value for the Speed EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpeedEVParse = tryIt;
            alert.Text = alerts;

            // Level input parsing
            yourLevel = ParseInput(level2.Text, "Please enter a numerical value for your level", "Pokémon have a maximum level of 100.\n", 100);
            bool levelParse = tryIt;
            alert.Text = alerts;

            // Nature determination
            NatureDeterm(false, nature2);

            if (HPBaseParse && AtkBaseParse && DefBaseParse && SpAtkBaseParse && SpDefBaseParse && SpeedBaseParse && HPEVParse && AtkEVParse && DefEVParse && SpAtkEVParse && SpDefEVParse && SpeedEVParse && levelParse)
            {
                StatCalculation(false);
                return true;
            }
            else { return false; }
        }

        /// <summary>
        /// Calculates stats for attacker or defender
        /// </summary>
        /// <param name="attacker">True for attacker, false for defender</param>
        public void StatCalculation(bool attacker)
        {
            ComboBox mon = monSelection2;
            int yourAbility = ability2.SelectedIndex;
            TextBlock[] totals = new TextBlock[7] { HPTot2, AtkTot2, DefTot2, SpAtkTot2, SpDefTot2, SpeedTot2, finHPDefender };
            ComboBox[] ivs = new ComboBox[6] { HPIV2, AtkIV2, DefIV2, SpAtkIV2, SpDefIV2, SpeedIV2 };
            ComboBox itemValue = item2;
            Slider slider = HPSliderDefender;
            ComboBox status = statusConditions2;
            ComboBox[] buffBoxes = new ComboBox[5] { atkBuffs2, defBuffs2, spAtkBuffs2, spDefBuffs2, speedBuffs2 };

            if (attacker)
            {
                mon = monSelection1;
                yourAbility = ability1.SelectedIndex;
                totals = new TextBlock[7] { HPTot1, AtkTot1, DefTot1, SpAtkTot1, SpDefTot1, SpeedTot1, finHP };
                ivs = new ComboBox[6] { HPIV1, AtkIV1, DefIV1, SpAtkIV1, SpDefIV1, SpeedIV1 };
                itemValue = item1;
                slider = HPSlider;
                status = statusConditions1;
                buffBoxes = new ComboBox[5] { atkBuffs1, defBuffs1, spAtkBuffs1, spDefBuffs1, speedBuffs1 };
            }

            int HPCalc;
            if (mon.SelectedIndex == 291) { HPCalc = 1; } //If the selected mon is Shedinja, HP is always set to 1
            else
            {
                HPCalc = (((2 * baseStats[0] + ivs[0].SelectedIndex + (allEVs[0] / 4)) * yourLevel) / 100) + yourLevel + 10;
            }
            totals[0].Text = HPCalc.ToString();
            totals[6].Text = "/ " + totals[0].Text;
            if (slider.Value == 0) { slider.Value = 10; } //If your current HP is set to 0, this sets it back to the maximum HP
            else //Makes sure that the value displayed in curHP.Text is the number that should be displayed
            {
                double val = slider.Value;
                slider.Value = 0;
                slider.Value = val;
            }

            double initialAtkCalc = ((((2 * baseStats[1] + ivs[1].SelectedIndex + (allEVs[1] / 4)) * yourLevel) / 100) + 5) * natureStuff[0] * badges[0];
            initialAtkCalc = Math.Floor(initialAtkCalc) * Buffs(buffBoxes[0].SelectedIndex);
            if ((statusConditions1.SelectedIndex != 0 && yourAbility == 18) || yourAbility == 20) { initialAtkCalc = Math.Floor(initialAtkCalc * 1.5); } //checks if you are statused and have guts or have hustle
            if (itemValue.SelectedIndex == 150) { initialAtkCalc = Math.Floor(initialAtkCalc * 1.5); } //checks for choice band
            int AtkCalc = Convert.ToInt32(Math.Floor(initialAtkCalc));
            if (yourAbility == 19 || yourAbility == 44) { AtkCalc *= 2; } //checks for huge / pure power
            if (itemValue.SelectedIndex == 166 && mon.SelectedIndex == 24) { AtkCalc *= 2; } //checks if you are holding light ball and you are pikachu
            if (status.SelectedIndex == 1 && yourAbility != 18) { AtkCalc /= 2; } //checks if you are burned and don't have guts
            totals[1].Text = AtkCalc.ToString();

            double initialDefCalc = ((((2 * baseStats[2] + ivs[2].SelectedIndex + (allEVs[2] / 4)) * yourLevel) / 100) + 5) * natureStuff[1] * badges[1];
            initialDefCalc = Math.Floor(initialDefCalc) * Buffs(buffBoxes[1].SelectedIndex);
            if (status.SelectedIndex != 0 && yourAbility == 34) { initialDefCalc = Math.Floor(initialDefCalc * 1.5); } //checks if you are statused and have marvel scale
            int DefCalc = Convert.ToInt32(Math.Floor(initialDefCalc));
            totals[2].Text = DefCalc.ToString();

            double initialSpAtkCalc = ((((2 * baseStats[3] + ivs[3].SelectedIndex + (allEVs[3] / 4)) * yourLevel) / 100) + 5) * natureStuff[2] * badges[2];
            initialSpAtkCalc = Math.Floor(initialSpAtkCalc) * Buffs(buffBoxes[2].SelectedIndex);
            if (itemValue.SelectedIndex == 155 && (mon.SelectedIndex == 379 || mon.SelectedIndex == 380)) { initialSpAtkCalc = Math.Floor(initialSpAtkCalc * 1.5); } //checks for soul dew and if you are a lati twin
            int spAtkCalc = Convert.ToInt32(Math.Floor(initialSpAtkCalc));
            if (itemValue.SelectedIndex == 156 && mon.SelectedIndex == 365) { spAtkCalc *= 2; } //checks for deep sea tooth and if you are clamperl
            if (itemValue.SelectedIndex == 166 && mon.SelectedIndex == 24) { spAtkCalc *= 2; } //checks for light ball and if you are pikachu
            totals[3].Text = spAtkCalc.ToString();
            if (defAbility == 66 && (moveTypeSelection.SelectedIndex == 5 || moveTypeSelection.SelectedIndex == 15)) { spAtkCalc /= 2; } //checks if the defender has thick fat and if you are using a fire or ice move

            double initialSpDefCalc = ((((2 * baseStats[4] + ivs[4].SelectedIndex + (allEVs[4] / 4)) * yourLevel) / 100) + 5) * natureStuff[3] * badges[3];
            initialSpDefCalc = Math.Floor(initialSpDefCalc) * Buffs(buffBoxes[3].SelectedIndex);
            if (itemValue.SelectedIndex == 155 && (mon.SelectedIndex == 379 ||mon.SelectedIndex == 380)) { initialSpDefCalc = Math.Floor(initialSpDefCalc * 1.5); } //checks for soul dew and if you are a lati twin
            int spDefCalc = Convert.ToInt32(Math.Floor(initialSpDefCalc));
            if (itemValue.SelectedIndex == 157 && mon.SelectedIndex == 365) { spDefCalc *= 2; } //checks for deep sea scale and if you are clamperl
            totals[4].Text = spDefCalc.ToString();

            double initialSpeedCalc = ((((2 * baseStats[5] + ivs[5].SelectedIndex + (allEVs[5] / 4)) * yourLevel) / 100) + 5) * natureStuff[4] * badges[4];
            initialSpeedCalc = Math.Floor(initialSpeedCalc) * Buffs(buffBoxes[4].SelectedIndex);
            if (statusConditions1.SelectedIndex == 2) { initialSpeedCalc /= 4; } //checks if you are paralyzed
            int SpeedCalc = Convert.ToInt32(Math.Floor(initialSpeedCalc));
            if ((yourAbility == 4 && weather == "sun") || (yourAbility == 64 && weather == "rain")) //Checks for swift swim and chlorophyll
            {
                SpeedCalc *= 2;
            }
            totals[5].Text = SpeedCalc.ToString();
        }

        /// <summary>
        /// Updates the base stats and type of the attacker based on the mon selected
        /// </summary>
        private void newMonSelected1(object sender, SelectionChangedEventArgs e)
        {
            var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var file = System.IO.Path.Combine(directory, "baseStats.csv");
            using (var reader = new StreamReader(file))
            {
                List<string> HP = new List<string>();
                List<string> Atk = new List<string>();
                List<string> Def = new List<string>();
                List<string> SpAtk = new List<string>();
                List<string> SpDef = new List<string>();
                List<string> Speed = new List<string>();
                List<string> Type1 = new List<string>();
                List<string> Type2 = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    HP.Add(values[1]);
                    Atk.Add(values[2]);
                    Def.Add(values[3]);
                    SpAtk.Add(values[4]);
                    SpDef.Add(values[5]);
                    Speed.Add(values[6]);
                    Type1.Add(values[7]);
                    Type2.Add(values[8]);
                }
                baseHP1.Text = HP[monSelection1.SelectedIndex];
                baseAtk1.Text = Atk[monSelection1.SelectedIndex];
                baseDef1.Text = Def[monSelection1.SelectedIndex];
                baseSpAtk1.Text = SpAtk[monSelection1.SelectedIndex];
                baseSpDef1.Text = SpDef[monSelection1.SelectedIndex];
                baseSpeed1.Text = Speed[monSelection1.SelectedIndex];
                TypeValues firstType = (TypeValues)Enum.Parse(typeof(TypeValues), Type1[monSelection1.SelectedIndex]);
                TypeValues secondType = (TypeValues)Enum.Parse(typeof(TypeValues), Type2[monSelection1.SelectedIndex]);
                type1Selection1.SelectedIndex = Convert.ToInt32(firstType);
                type2Selection1.SelectedIndex = Convert.ToInt32(secondType);
            }
            if (finalHeader != null && attackName.SelectedItem != null) //Prevents Calculate() from being ran before everything is initialized
            {
                Calculate();
            }
        }

        /// <summary>
        /// Updates the base stats and type of the defender based on the mon selected
        /// </summary>
        private void newMonSelected2(object sender, SelectionChangedEventArgs e)
        {
            var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var file = System.IO.Path.Combine(directory, "baseStats.csv");
            using (var reader = new StreamReader(file))
            {
                List<string> HP = new List<string>();
                List<string> Atk = new List<string>();
                List<string> Def = new List<string>();
                List<string> SpAtk = new List<string>();
                List<string> SpDef = new List<string>();
                List<string> Speed = new List<string>();
                List<string> Type1 = new List<string>();
                List<string> Type2 = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    HP.Add(values[1]);
                    Atk.Add(values[2]);
                    Def.Add(values[3]);
                    SpAtk.Add(values[4]);
                    SpDef.Add(values[5]);
                    Speed.Add(values[6]);
                    Type1.Add(values[7]);
                    Type2.Add(values[8]);
                }
                baseHP2.Text = HP[monSelection2.SelectedIndex];
                baseAtk2.Text = Atk[monSelection2.SelectedIndex];
                baseDef2.Text = Def[monSelection2.SelectedIndex];
                baseSpAtk2.Text = SpAtk[monSelection2.SelectedIndex];
                baseSpDef2.Text = SpDef[monSelection2.SelectedIndex];
                baseSpeed2.Text = Speed[monSelection2.SelectedIndex];
                TypeValues firstType = (TypeValues)Enum.Parse(typeof(TypeValues), Type1[monSelection2.SelectedIndex]);
                TypeValues secondType = (TypeValues)Enum.Parse(typeof(TypeValues), Type2[monSelection2.SelectedIndex]);
                type1Selection2.SelectedIndex = Convert.ToInt32(firstType);
                type2Selection2.SelectedIndex = Convert.ToInt32(secondType);
            }
            if (finalHeader != null && attackName.SelectedItem != null) //Prevents Calculate() from being ran before everything is initialized
            {
                Calculate();
            }
        }

        /// <summary>
        /// Gets the type and power of the attack that is selected every time a new attack is selected
        /// </summary>
        private void AttackNameSelected(object sender, SelectionChangedEventArgs e)
        {
            var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var file = System.IO.Path.Combine(directory, "attacks.csv");
            using (var reader = new StreamReader(file))
            {
                List<string> Type = new List<string>();
                List<string> BP = new List<string>();
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    Type.Add(values[1]);
                    BP.Add(values[2]);
                }
                basePower1.Text = BP[attackName.SelectedIndex];
                TypeValues moves = (TypeValues)Enum.Parse(typeof(TypeValues), Type[attackName.SelectedIndex]);
                moveTypeSelection.SelectedIndex = Convert.ToInt32(moves);
            }
            if (attackName.SelectedIndex == 6 || attackName.SelectedIndex == 166 || attackName.SelectedIndex == 169 || (attackName.SelectedIndex >= 170 && attackName.SelectedIndex <= 176))
            {
                hitsBox.Visibility = Visibility.Visible;
                hitsNumber.Visibility = Visibility.Visible;
            }
            else
            {
                hitsBox.Visibility = Visibility.Hidden;
                hitsNumber.Visibility = Visibility.Hidden;
            }
            if (finalHeader != null && attackName.SelectedItem != null) //Prevents Calculate() from being ran before everything is initialized
            {
                Calculate();
            }
        }

        /// <summary>The main handler for abilities disallowing a status condition that can be inflicted on a Pokemon</summary>
        /// <param name="attacker">Set to true if you are editing the attackers enabled status condition choices, false if you are editing the defender</param>
        /// <param name="statusEnable">An array of booleans representing which status conditions you want enabled; formatted as burn, paralysis, poison, toxic, sleep, freeze</param>
        public void ToggleStatusChoice(bool attacker, bool[] statusEnable)
        {
            //I want to get a string that says which ones to disable, and then disable only those ones
            //Right now I have an acceptable compromise that utilizes 2 methods in order to determine which status conditions can be selected that use an array of booleans
            int init = 0; //Initializing an integer that just keeps track of how many times the loops below have ran, used to determine which boolean to read from in the statusEnable array
            if (attacker)
            {
                ComboBoxItem[] statuses = { burn1, para1, poison1, toxic1, sleep1, freeze1 }; //Declares an array that has the names of all the status ComboBoxItem items in the attackers status ComboBox
                foreach (ComboBoxItem status in statuses)
                {
                    status.IsEnabled = statusEnable[init];
                    if (status.IsSelected && statusEnable[init] == false) { statusConditions1.SelectedIndex = 0; } //Makes sure you can't have the status selected before switching abilities, setting it to none if you had a disabled status selected
                    init++;
                }
            }
            else
            {
                ComboBoxItem[] statuses = { burn2, para2, poison2, toxic2, sleep2, freeze2 }; //Declares an array that has the names of all the status ComboBoxItem items in the defenders status ComboBox
                foreach (ComboBoxItem status in statuses)
                {
                    status.IsEnabled = statusEnable[init];
                    if (status.IsSelected && statusEnable[init] == false) { statusConditions2.SelectedIndex = 0; }
                    init++;
                }
            }
        }

        /// <summary>Gets all the information needed and organized to pass off to the main ToggleStatusChoice method</summary>
        /// <param name="whichAbility">The ComboBox.SelectedIndex value of the ability method calling this method. e.g. Ability1_SelectionChanged uses ability1.SelectedIndex</param>
        /// <param name="attacker">A boolean to pass off to the main handler that is set to true if it is the attacker's ability being selected, and false if it is the defender's ability being changed</param>
        public void StatusChoiceInit(int whichAbility, bool attacker)
        {
            bool[] toggle = new bool[5];
            if (whichAbility == 23) //Checks if the user has immunity as their ability
            {
                toggle = new bool[] { true, true, false, false, true, true };
                ToggleStatusChoice(attacker, toggle);
            }
            else if (whichAbility == 25 || whichAbility == 70) //Checks if the user has insomnia or vital spirit as their ability
            {
                toggle = new bool[] { true, true, true, true, false, true };
                ToggleStatusChoice(attacker, toggle);
            }
            else if (whichAbility == 30) //Checks if the user has limber as their ability
            {
                toggle = new bool[] { true, false, true, true, true, true };
                ToggleStatusChoice(attacker, toggle);
            }
            else if (whichAbility == 32) //Checks if the user has magma armor as their ability
            {
                toggle = new bool[] { true, true, true, true, true, false };
                ToggleStatusChoice(attacker, toggle);
            }
            else if (whichAbility == 73) //Checks if the user has water veil as their ability
            {
                toggle = new bool[] { false, true, true, true, true, true };
                ToggleStatusChoice(attacker, toggle);
            }
            else //Makes sure all the options are available if the user has none of the above abilities
            {
                toggle = new bool[] { true, true, true, true, true, true };
                ToggleStatusChoice(attacker, toggle);
            }
        }

        /// <summary>
        /// Runs ability-related functions to be ran each time you select a new ability for the attacker
        /// </summary>
        private void Ability1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ability1.SelectedIndex == 16 || ability1.SelectedIndex == 26)
            {
                ability1Active.Visibility = Visibility.Visible;
            }
            else
            {
                ability1Active.Visibility = Visibility.Hidden;
            }
            StatusChoiceInit(ability1.SelectedIndex, true);
        }

        /// <summary>
        /// Runs ability-related functions to be ran each time you select a new ability for the defender
        /// </summary>
        private void Ability2_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ability2.SelectedIndex == 16 || ability2.SelectedIndex == 26)
            {
                ability2Active.Visibility = Visibility.Visible;
            }
            else
            {
                ability1Active.Visibility = Visibility.Hidden;
            }
            StatusChoiceInit(ability2.SelectedIndex, false);
        }

        /// <summary>
        /// Updates a few values in the UI based on the value of the HP slider
        /// </summary>
        private void HPSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangedHPSlider(true);
        }

        /// <summary>
        /// Updates a few values in the UI based on the value of the defenders' HP slider
        /// </summary>
        private void HPSliderDefender_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            ChangedHPSlider(false);
        }

        /// <summary>
        /// The main method that controls the data influenced by the HP sliders
        /// </summary>
        /// <param name="attackerOrDefender">Set to true if you are running for the attacker, false if running for the defender</param>
        private void ChangedHPSlider(bool attackerOrDefender)
        {
            TextBlock finalHP = finHPDefender;
            TextBox yourCurrentHP = curHPDefender;
            Slider slider = HPSliderDefender;
            bool boxEditCheck = editingBox[1];
            TextBlock percentage = HPPercentDefender;
            if (attackerOrDefender) //I massively simplified the method by making all of the UI elements variables set based on the attackerOrDefender bool, reducing the number of times it checks the bool from 3 to 1
            {
                finalHP = finHP;
                yourCurrentHP = curHP;
                slider = HPSlider;
                boxEditCheck = editingBox[0];
                percentage = HPPercent;
            }
            string maxHP = finalHP.Text.Substring(2, finalHP.Text.Length - 2);
            int HPParsed = int.Parse(maxHP);
            double currentHP = Math.Floor(HPParsed * (slider.Value / 10));
            if (!boxEditCheck) { yourCurrentHP.Text = currentHP.ToString(); }
            double val = Math.Round((currentHP / HPParsed) * 100, 1); //Math.Round(HPSlider.Value * 10, 1); I'm going with what I am so that it more accurately reflects the percentage HP it's displaying, also the value in finHP.Text is originally set to 1 instead of 0 so it doesn't try to divide by 0
            percentage.Text = val.ToString() + "%";
        }

        private void CurHP_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChangedCurHP(true);
        }

        private void CurHPDefender_TextChanged(object sender, TextChangedEventArgs e)
        {
            ChangedCurHP(false);
        }

        /// <summary>
        /// Updates a few values in the UI based on the value of the text in the CurHP textBox
        /// </summary>
        /// <param name="attackerOrDefender">Use true for attacker, false for defender</param>
        private void ChangedCurHP(bool attackerOrDefender)
        {
            if ((attackerOrDefender && editingBox[0]) || (!attackerOrDefender && editingBox[1]))
            {
                alerts = "";
                alert.Text = alerts;
                TextBlock totalHP = HPTot2;
                TextBox yourCurrentHP = curHPDefender;
                Slider slider = HPSliderDefender;
                if (attackerOrDefender) //reduced from 3 bool checks to 1
                {
                    totalHP = HPTot1;
                    yourCurrentHP = curHP;
                    slider = HPSlider;
                }
                double HPParsed = Convert.ToDouble(ParseInput(totalHP.Text, "", "", 714));
                bool maxParse = tryIt;
                if (maxParse)
                {
                    double currentHP = ParseInput(yourCurrentHP.Text, "Please enter an integer for the current HP\n", "Your current HP cannot be higher than your maximum HP\n", Convert.ToInt32(HPParsed));
                    alert.Text = alerts;
                    bool parse = tryIt;
                    if (parse)
                    {
                        double HPPercentage = currentHP / HPParsed;
                        slider.Value = HPPercentage * 10.0;
                    }
                }
            }
        }

        /// <summary>
        /// Updates editingBox[0] to true when you select the CurHP textBox, all other ways I've tested of doing this cause the item slider to be extremely laggy
        /// </summary>
        private void CurHP_GotFocus(object sender, RoutedEventArgs e)
        {
            editingBox[0] = true;
        }

        /// <summary>
        /// Updates editingBox[0] to false when you click off of the CurHP textBox, all other ways I've tested of doing this cause the item slider to be extremely laggy
        /// </summary>
        private void CurHP_LostFocus(object sender, RoutedEventArgs e)
        {
            editingBox[0] = false;
        }

        /// <summary>
        /// Updates editingBox[1] to true when you select the CurHP textBox, all other ways I've tested of doing this cause the item slider to be extremely laggy
        /// </summary>
        private void CurHPDefender_GotFocus(object sender, RoutedEventArgs e)
        {
            editingBox[1] = true;
        }

        /// <summary>
        /// Updates editingBox[1] to false when you click off of the CurHP textBox, all other ways I've tested of doing this cause the item slider to be extremely laggy
        /// </summary>
        private void CurHPDefender_LostFocus(object sender, RoutedEventArgs e)
        {
            editingBox[1] = false;
        }

        private void StatUpdated(object sender, TextChangedEventArgs e)
        {
            if (finalHeader != null && attackName.SelectedItem != null) //Prevents Calculate() from being ran before everything is initialized
            {
                Calculate();
            }
        }
    }
}
