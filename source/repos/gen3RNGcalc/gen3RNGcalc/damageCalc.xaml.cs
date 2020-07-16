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
        public static int parsedValue;
        public static string alerts;
        public static bool tryIt;
        public static string weather;
        public static int defAbility;

        public damageCalc()
        {
            InitializeComponent();
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
                    ability1.Items.Add(ability);
                    ability2.Items.Add(ability);
                }
            }
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
        /// <param name="findBuff">The SelectedIndex of the comboBox you want to search for</param>
        public static double Buffs(int findBuff) //findBuff is set to the SelectedIndex of the buff dropdown you are searching for, e.g. atkBuffs1.SelectedIndex
        {
            if (findBuff <= 6)
            {
                return 4 - (findBuff * 0.5);
            }
            else
            {
                return 1 / (1 + ((findBuff - 6) * 0.5));
            }
        }
        
        /// <summary>
        /// A quick way to parse the inputs of integers and return the parsed value
        /// </summary>
        /// <param name="inputNeeded">The text you want to parse, usually a TextBox.Text parameter</param>
        /// <param name="warning">The warning to display if the text cannot be parsed</param>
        /// <param name="tooHigh">The warning to display if the parsed value is too high</param>
        /// <param name="maxValue">The maximum value to be accepted</param>
        /// <returns></returns>
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
        /// <returns></returns>
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
                if (defAbility == 75)
                {
                    if ((finalDefender1 * finalDefender2) < 2) { return 0; }
                }
                return finalDefender1 * finalDefender2;
            }
        }

        /// <summary>
        /// Start the calculation on press of the "Go" button
        /// </summary>
        private void Go(object sender, RoutedEventArgs e)
        {
            alerts = "";
            alert.Text = alerts;
            finalCalc.Text = "";
            finalHeader.Text = "";

            double atkNature = 1;
            double defNature = 1;
            double spAtkNature = 1;
            double spDefNature = 1;
            double speedNature = 1;
            double atkBadge = 1;
            double defBadge = 1;
            double spAtkBadge = 1;
            double spDefBadge = 1;
            double speedBadge = 1;

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
            int yourBaseHP = ParseInput(baseHP1.Text, "Please enter a numerical value for the base HP.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool HPBaseParse = tryIt;
            alert.Text = alerts;
            int yourBaseAtk = ParseInput(baseAtk1.Text, "Please enter a numerical value for the base Attack.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool AtkBaseParse = tryIt;
            alert.Text = alerts;
            int yourBaseDef = ParseInput(baseDef1.Text, "Please enter a numerical value for the base Defense.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool DefBaseParse = tryIt;
            alert.Text = alerts;
            int yourBaseSpAtk = ParseInput(baseSpAtk1.Text, "Please enter a numerical value for the base Special Attack.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpAtkBaseParse = tryIt;
            alert.Text = alerts;
            int yourBaseSpDef = ParseInput(baseSpDef1.Text, "Please enter a numerical value for the base Special Defense.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpDefBaseParse = tryIt;
            alert.Text = alerts;
            int yourBaseSpeed = ParseInput(baseSpeed1.Text, "Please enter a numerical value for the base Speed.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpeedBaseParse = tryIt;
            alert.Text = alerts;

            // EV input parsing
            int yourHPEVs = ParseInput(HPEV1.Text, "Please enter a numerical value for the HP EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool HPEVParse = tryIt;
            alert.Text = alerts;
            int yourAtkEVs = ParseInput(AtkEV1.Text, "Please enter a numerical value for the Attack EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool AtkEVParse = tryIt;
            alert.Text = alerts;
            int yourDefEVs = ParseInput(DefEV1.Text, "Please enter a numerical value for the Defense EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool DefEVParse = tryIt;
            alert.Text = alerts;
            int yourSpAtkEVs = ParseInput(SpAtkEV1.Text, "Please enter a numerical value for the Special Attack EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpAtkEVParse = tryIt;
            alert.Text = alerts;
            int yourSpDefEVs = ParseInput(SpDefEV1.Text, "Please enter a numerical value for the Special Defense EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpDefEVParse = tryIt;
            alert.Text = alerts;
            int yourSpeedEVs = ParseInput(SpeedEV1.Text, "Please enter a numerical value for the Speed EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpeedEVParse = tryIt;
            alert.Text = alerts;

            // Level input parsing
            int yourLevel = ParseInput(level1.Text, "Please enter a numerical value for your level", "Pokémon have a maximum level of 100.\n", 100);
            bool levelParse = tryIt;
            alert.Text = alerts;

            // Nature determination, there might be a more efficient way of writing this, I'll look into it
            string nature = nature1.Text.Substring(0, nature1.Text.IndexOf(" "));
            if (nature == "Lonely" || nature == "Adamant" || nature == "Naughty" || nature == "Brave")
            {
                atkNature = 1.1;
            }
            if (nature == "Bold" || nature == "Modest" || nature == "Calm" || nature == "Timid")
            {
                atkNature = 0.9;
            }
            if (nature == "Bold" || nature == "Impish" || nature == "Lax" || nature == "Relaxed")
            {
                defNature = 1.1;
            }
            if (nature == "Lonely" || nature == "Mild" || nature == "Gentle" || nature == "Hasty")
            {
                defNature = 0.9;
            }
            if (nature == "Modest" || nature == "Mild" || nature == "Rash" || nature == "Quiet")
            {
                spAtkNature = 1.1;
            }
            if (nature == "Adamant" || nature == "Impish" || nature == "Careful" || nature == "Jolly")
            {
                spAtkNature = 0.9;
            }
            if (nature == "Calm" || nature == "Gentle" || nature == "Careful" || nature == "Sassy")
            {
                spDefNature = 1.1;
            }
            if (nature == "Naughty" || nature == "Lax" || nature == "Rash" || nature == "Naive")
            {
                spDefNature = 0.9;
            }
            if (nature == "Timid" || nature == "Hasty" || nature == "Jolly" || nature == "Naive")
            {
                speedNature = 1.1;
            }
            if (nature == "Brave" || nature == "Relaxed" || nature == "Quiet" || nature == "Sassy")
            {
                speedNature = 0.9;
            }

            // Badge boost determination
            if (boulderBadge == true || stoneBadge == true)
            {
                atkBadge = 1.1;
            }
            if (soulBadge == true || balanceBadge == true)
            {
                defBadge = 1.1;
            }
            if (volcanoBadge == true || mindBadge == true)
            {
                spAtkBadge = 1.1;
                spDefBadge = 1.1;
            }
            if (thunderBadge == true || dynamoBadge == true)
            {
                speedBadge = 1.1;
            }

            if (HPBaseParse && AtkBaseParse && DefBaseParse && SpAtkBaseParse && SpDefBaseParse && SpeedBaseParse && HPEVParse && AtkEVParse && DefEVParse && SpAtkEVParse && SpDefEVParse && SpeedEVParse && levelParse)
            {
                if ((kanto ^ hoenn) || !kanto && !hoenn)
                {
                    int HPCalc;
                    if (monSelection1.SelectedIndex == 291) { HPCalc = 1; } //If the selected mon is Shedinja, HP is always set to 1
                    else
                    {
                        HPCalc = (((2 * yourBaseHP + HPIV1.SelectedIndex + (yourHPEVs / 4)) * yourLevel) / 100) + yourLevel + 10;
                    }
                    HPTot1.Text = HPCalc.ToString();

                    double initialAtkCalc = ((((2 * yourBaseAtk + AtkIV1.SelectedIndex + (yourAtkEVs / 4)) * yourLevel) / 100) + 5) * atkNature * atkBadge;
                    initialAtkCalc = Math.Floor(initialAtkCalc) * Buffs(atkBuffs1.SelectedIndex);
                    if ((statusConditions1.SelectedIndex != 0 && yourAbility == 18) || yourAbility == 20) { initialAtkCalc = Math.Floor(initialAtkCalc * 1.5); } //checks if you are statused and have guts or have hustle
                    int AtkCalc = Convert.ToInt32(Math.Floor(initialAtkCalc));
                    if (yourAbility == 19 || yourAbility == 44) { AtkCalc *= 2; } //checks for huge / pure power
                    if (statusConditions1.SelectedIndex == 1 && yourAbility != 18) { AtkCalc /= 2; } //checks if you are burned and don't have guts
                    AtkTot1.Text = AtkCalc.ToString();

                    double initialDefCalc = ((((2 * yourBaseDef + DefIV1.SelectedIndex + (yourDefEVs / 4)) * yourLevel) / 100) + 5) * defNature * defBadge;
                    initialDefCalc = Math.Floor(initialDefCalc) * Buffs(defBuffs1.SelectedIndex);
                    if (statusConditions1.SelectedIndex != 0 && yourAbility == 34) { initialDefCalc = Math.Floor(initialDefCalc * 1.5); } //checks if you are statused and have marvel scale
                    int DefCalc = Convert.ToInt32(Math.Floor(initialDefCalc));
                    DefTot1.Text = DefCalc.ToString();

                    double initialSpAtkCalc = ((((2 * yourBaseSpAtk + SpAtkIV1.SelectedIndex + (yourSpAtkEVs / 4)) * yourLevel) / 100) + 5) * spAtkNature * spAtkBadge;
                    initialSpAtkCalc = Math.Floor(initialSpAtkCalc) * Buffs(spAtkBuffs1.SelectedIndex);
                    int spAtkCalc = Convert.ToInt32(Math.Floor(initialSpAtkCalc));
                    SpAtkTot1.Text = spAtkCalc.ToString();

                    double initialSpDefCalc = ((((2 * yourBaseSpDef + SpDefIV1.SelectedIndex + (yourSpDefEVs / 4)) * yourLevel) / 100) + 5) * spDefNature * spDefBadge;
                    initialSpDefCalc = Math.Floor(initialSpDefCalc) * Buffs(spDefBuffs1.SelectedIndex);
                    int spDefCalc = Convert.ToInt32(Math.Floor(initialSpDefCalc));
                    SpDefTot1.Text = spDefCalc.ToString();

                    double initialSpeedCalc = ((((2 * yourBaseSpeed + SpeedIV1.SelectedIndex + (yourSpeedEVs / 4)) * yourLevel) / 100) + 5) * speedNature * speedBadge;
                    initialSpeedCalc = Math.Floor(initialSpeedCalc) * Buffs(speedBuffs1.SelectedIndex);
                    int SpeedCalc = Convert.ToInt32(Math.Floor(initialSpeedCalc));
                    if ((yourAbility == 4 && weather == "sun") || (yourAbility == 64 && weather == "rain")) //Checks for swift swim and chlorophyll
                    {
                        SpeedCalc *= 2;
                    }
                    SpeedTot1.Text = SpeedCalc.ToString();

                    if (EnemyCalc()) //Makes sure that all parsing done by EnemyCalc was successful before trying to calculate anything
                    {
                        if (yourAbility != defAbility && (yourAbility == 11 || yourAbility == 12 || yourAbility == 49) && (defAbility == 11 || defAbility == 12 || defAbility == 49))
                        {
                            if (SpeedCalc <= int.Parse(SpeedTot2.Text)) //Is there a more efficent way of doing this, rather than just checking for each ability value individually?
                            {
                                if (yourAbility == 11)
                                {
                                    raining.IsChecked = true;
                                    weather = "rain";
                                }
                                else if (yourAbility == 12)
                                {
                                    sunny.IsChecked = true;
                                    weather = "sun";
                                }
                                else
                                {
                                    sandstorm.IsChecked = true;
                                    weather = "sand";
                                }
                            }
                            else
                            {
                                if (defAbility == 11)
                                {
                                    raining.IsChecked = true;
                                    weather = "rain";
                                }
                                else if (defAbility == 12)
                                {
                                    sunny.IsChecked = true;
                                    weather = "sun";
                                }
                                else
                                {
                                    sandstorm.IsChecked = true;
                                    weather = "sand";
                                }
                            }
                        }
                        if (attackName.SelectedIndex != 35 && attackName.SelectedIndex != 89 && attackName.SelectedIndex != 117 && attackName.SelectedIndex != 132 && !(attackName.SelectedIndex >= 162 && attackName.SelectedIndex <= 165))
                        {
                            int power = ParseInput(basePower1.Text, "Please enter a numerical value for the base power.", "A move's base power can not be higher than 255.", 255);
                            bool bpParse = tryIt;
                            alert.Text = alerts;

                            if (bpParse)
                            {
                                finalCalc.Text = "Possible damage amounts: ";
                                double firePower = 1;
                                double waterPower = 1;
                                double crit = 1;
                                double STAB = 1;
                                double effective = 1;
                                double minPercentHP = 0;
                                double maxPercentHP = 0;
                                double minDamage = 0;
                                double maxDamage = 0;
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
                                if (forceCrit1.IsChecked == true && (defAbility != 2 || defAbility != 54)) //Checks if you want to force crits and makes sure the defender does not have battle armor or shell armor
                                {
                                    crit = 2;
                                }
                                if (moveTypeSelection.SelectedIndex == type1Selection1.SelectedIndex || moveTypeSelection.SelectedIndex == type2Selection1.SelectedIndex)
                                {
                                    STAB = 1.5;
                                }
                                effective = effectiveness(moveTypeSelection.SelectedIndex, type1Selection2.SelectedIndex, type2Selection2.SelectedIndex);
                                double modifier;
                                bool isPhysical = false;
                                int typeOfMove = moveTypeSelection.SelectedIndex;
                                if (typeOfMove == 0 || typeOfMove == 1 || typeOfMove == 2 || typeOfMove == 9 || typeOfMove == 10 || typeOfMove == 11 || typeOfMove == 12 || typeOfMove == 13 || typeOfMove == 16)
                                {
                                    isPhysical = true;
                                }
                                for (double roll = 85; roll <= 100; roll++)
                                {
                                    if (moveTypeSelection.SelectedIndex == 5)
                                    {
                                        modifier = roll * crit * firePower * effective * STAB;
                                    }
                                    else if (moveTypeSelection.SelectedIndex == 6)
                                    {
                                        modifier = roll * crit * waterPower * effective * STAB;
                                    }
                                    else
                                    {
                                        modifier = roll * crit * effective * STAB;
                                    }
                                    double atkdouble = 0;
                                    string defOfDefender;
                                    if (isPhysical)
                                    {
                                        atkdouble = AtkCalc;
                                        defOfDefender = DefTot2.Text;
                                    }
                                    else
                                    {
                                        atkdouble = spAtkCalc;
                                        defOfDefender = SpDefTot2.Text;
                                    }
                                    double lvdouble = yourLevel;
                                    double damage = Math.Floor(Math.Floor(Math.Floor(2 * lvdouble / 5 + 2) * atkdouble * power / double.Parse(defOfDefender)) / 50); //Pulled directly from pokemon showdown's initial calculation, and adapted for C#
                                    damage += 2;
                                    damage = Math.Floor((damage * modifier) / 100); //There are a few odd cases where the calculation is off by 1, such as the max roll for 0 SpA Charizard Flamethrower vs. 0 SpD Venusaur, which results in 261, although the actual answer is 260
                                    if (roll == 85)
                                    {
                                        minPercentHP = (damage / double.Parse(HPTot2.Text)) * 100;
                                        minDamage = damage;
                                    }
                                    if (roll == 100)
                                    {
                                        maxPercentHP = (damage / double.Parse(HPTot2.Text)) * 100;
                                        maxDamage = damage;
                                    }
                                    int finalDamage = Convert.ToInt32(Math.Floor(damage));
                                    finalCalc.Text = finalCalc.Text + finalDamage.ToString() + ", ";
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
                                    finalHeader.Text = offensiveBuffs + yourAtkEVs.ToString() + " Atk " + firstMon + " " + moveName + " vs. " + defensiveBuffs + HPEV2.Text + " HP / " + DefEV2.Text + " Def " + secondMon + ": " + minDamage.ToString() + "-" + maxDamage.ToString() + " (" + Math.Round(minPercentHP, 1).ToString() + " - " + Math.Round(maxPercentHP, 1).ToString() + "%)";
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
                                    finalHeader.Text = offensiveBuffs + yourSpAtkEVs.ToString() + " SpA " + firstMon + " " + moveName + " vs. " + defensiveBuffs + HPEV2.Text + " HP / " + SpDefEV2.Text + " SpD " + secondMon + ": " + minDamage.ToString() + "-" + maxDamage.ToString() + " (" + Math.Round(minPercentHP, 1).ToString() + " - " + Math.Round(maxPercentHP, 1).ToString() + "%)";
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
                                if (effective == 0 || defAbility == 61) { damage = 0; } //Checks if the defending pokemon is immune to the attack, and sets damage to 0 if it is
                                else { damage = double.Parse(HPTot2.Text); } //If the defender is not immune, then it will always take damage equal to its HP
                            }
                            double percentHP = (damage / double.Parse(HPTot2.Text)) * 100;
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
            double atkNature = 1;
            double defNature = 1;
            double spAtkNature = 1;
            double spDefNature = 1;
            double speedNature = 1;

            int yourAbility = ability2.SelectedIndex;

            int yourBaseHP = ParseInput(baseHP2.Text, "Please enter a numerical value for the base HP.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool HPBaseParse = tryIt;
            alert.Text = alerts;
            int yourBaseAtk = ParseInput(baseAtk2.Text, "Please enter a numerical value for the base Attack.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool AtkBaseParse = tryIt;
            alert.Text = alerts;
            int yourBaseDef = ParseInput(baseDef2.Text, "Please enter a numerical value for the base Defense.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool DefBaseParse = tryIt;
            alert.Text = alerts;
            int yourBaseSpAtk = ParseInput(baseSpAtk2.Text, "Please enter a numerical value for the base Special Attack.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpAtkBaseParse = tryIt;
            alert.Text = alerts;
            int yourBaseSpDef = ParseInput(baseSpDef2.Text, "Please enter a numerical value for the base Special Defense.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpDefBaseParse = tryIt;
            alert.Text = alerts;
            int yourBaseSpeed = ParseInput(baseSpeed2.Text, "Please enter a numerical value for the base Speed.\n", "Pokémon can not have a base stat of over 255.\n", 255);
            bool SpeedBaseParse = tryIt;
            alert.Text = alerts;

            // EV input parsing
            int yourHPEVs = ParseInput(HPEV2.Text, "Please enter a numerical value for the HP EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool HPEVParse = tryIt;
            alert.Text = alerts;
            int yourAtkEVs = ParseInput(AtkEV2.Text, "Please enter a numerical value for the Attack EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool AtkEVParse = tryIt;
            alert.Text = alerts;
            int yourDefEVs = ParseInput(DefEV2.Text, "Please enter a numerical value for the Defense EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool DefEVParse = tryIt;
            alert.Text = alerts;
            int yourSpAtkEVs = ParseInput(SpAtkEV2.Text, "Please enter a numerical value for the Special Attack EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpAtkEVParse = tryIt;
            alert.Text = alerts;
            int yourSpDefEVs = ParseInput(SpDefEV2.Text, "Please enter a numerical value for the Special Defense EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpDefEVParse = tryIt;
            alert.Text = alerts;
            int yourSpeedEVs = ParseInput(SpeedEV2.Text, "Please enter a numerical value for the Speed EVs.\n", "Pokémon can not have an EV of over 252.\n", 252);
            bool SpeedEVParse = tryIt;
            alert.Text = alerts;

            // Level input parsing
            int yourLevel = ParseInput(level2.Text, "Please enter a numerical value for your level", "Pokémon have a maximum level of 100.\n", 100);
            bool levelParse = tryIt;
            alert.Text = alerts;

            // Nature determination
            string nature = nature2.Text.Substring(0, nature2.Text.IndexOf(" "));
            if (nature == "Lonely" || nature == "Adamant" || nature == "Naughty" || nature == "Brave")
            {
                atkNature = 1.1;
            }
            if (nature == "Bold" || nature == "Modest" || nature == "Calm" || nature == "Timid")
            {
                atkNature = 0.9;
            }
            if (nature == "Bold" || nature == "Impish" || nature == "Lax" || nature == "Relaxed")
            {
                defNature = 1.1;
            }
            if (nature == "Lonely" || nature == "Mild" || nature == "Gentle" || nature == "Hasty")
            {
                defNature = 0.9;
            }
            if (nature == "Modest" || nature == "Mild" || nature == "Rash" || nature == "Quiet")
            {
                spAtkNature = 1.1;
            }
            if (nature == "Adamant" || nature == "Impish" || nature == "Careful" || nature == "Jolly")
            {
                spAtkNature = 0.9;
            }
            if (nature == "Calm" || nature == "Gentle" || nature == "Careful" || nature == "Sassy")
            {
                spDefNature = 1.1;
            }
            if (nature == "Naughty" || nature == "Lax" || nature == "Rash" || nature == "Naive")
            {
                spDefNature = 0.9;
            }
            if (nature == "Timid" || nature == "Hasty" || nature == "Jolly" || nature == "Naive")
            {
                speedNature = 1.1;
            }
            if (nature == "Brave" || nature == "Relaxed" || nature == "Quiet" || nature == "Sassy")
            {
                speedNature = 0.9;
            }

            if (HPBaseParse && AtkBaseParse && DefBaseParse && SpAtkBaseParse && SpDefBaseParse && SpeedBaseParse && HPEVParse && AtkEVParse && DefEVParse && SpAtkEVParse && SpDefEVParse && SpeedEVParse && levelParse)
            {
                int HPCalc;
                if (monSelection2.SelectedIndex == 291) { HPCalc = 1; } //If the selected mon is Shedinja, HP is always set to 1
                else
                {
                    HPCalc = (((2 * yourBaseHP + HPIV2.SelectedIndex + (yourHPEVs / 4)) * yourLevel) / 100) + yourLevel + 10;
                }
                HPTot2.Text = HPCalc.ToString();

                double initialAtkCalc = ((((2 * yourBaseAtk + AtkIV2.SelectedIndex + (yourAtkEVs / 4)) * yourLevel) / 100) + 5) * atkNature;
                initialAtkCalc = Math.Floor(initialAtkCalc) * Buffs(atkBuffs2.SelectedIndex);
                if ((statusConditions2.SelectedIndex != 0 && yourAbility == 18) || yourAbility == 20) { initialAtkCalc = Math.Floor(initialAtkCalc * 1.5); } //checks if you are statused and have guts or have hustle
                int AtkCalc = Convert.ToInt32(Math.Floor(initialAtkCalc));
                if (yourAbility == 19 || yourAbility == 44) { AtkCalc *= 2; } //checks for huge / pure power
                if (statusConditions2.SelectedIndex == 1 && yourAbility != 18) { AtkCalc /= 2; } //checks if you are burned and if you don't have guts
                AtkTot2.Text = AtkCalc.ToString();

                double initialDefCalc = ((((2 * yourBaseDef + DefIV2.SelectedIndex + (yourDefEVs / 4)) * yourLevel) / 100) + 5) * defNature;
                initialDefCalc = Math.Floor(initialDefCalc) * Buffs(defBuffs2.SelectedIndex);
                if (statusConditions2.SelectedIndex != 0 && yourAbility == 34) { initialDefCalc = Math.Floor(initialDefCalc * 1.5); } //checks if you are statused and have marvel scale
                int DefCalc = Convert.ToInt32((Math.Floor(initialDefCalc)));
                DefTot2.Text = DefCalc.ToString();

                double initialSpAtkCalc = ((((2 * yourBaseSpAtk + SpAtkIV2.SelectedIndex + (yourSpAtkEVs / 4)) * yourLevel) / 100) + 5) * spAtkNature;
                initialSpAtkCalc = Math.Floor(initialSpAtkCalc) * Buffs(spAtkBuffs2.SelectedIndex);
                int spAtkCalc = Convert.ToInt32((Math.Floor(initialSpAtkCalc)));
                SpAtkTot2.Text = spAtkCalc.ToString();

                double initialSpDefCalc = ((((2 * yourBaseSpDef + SpDefIV2.SelectedIndex + (yourSpDefEVs / 4)) * yourLevel) / 100) + 5) * spDefNature;
                initialSpDefCalc = Math.Floor(initialSpDefCalc) * Buffs(spDefBuffs2.SelectedIndex);
                int spDefCalc = Convert.ToInt32((Math.Floor(initialSpDefCalc)));
                SpDefTot2.Text = spDefCalc.ToString();

                double initialSpeedCalc = ((((2 * yourBaseSpeed + SpeedIV2.SelectedIndex + (yourSpeedEVs / 4)) * yourLevel) / 100) + 5) * speedNature;
                initialSpeedCalc = Math.Floor(initialSpeedCalc) * Buffs(speedBuffs2.SelectedIndex);
                int SpeedCalc = Convert.ToInt32((Math.Floor(initialSpeedCalc)));
                if ((yourAbility == 4 && weather == "sun") || (yourAbility == 64 && weather == "rain")) { SpeedCalc *= 2; } //Checks for chlorophyll / swift swim
                SpeedTot2.Text = SpeedCalc.ToString();
                return true;
            }
            else { return false; }
        }

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
                TypeValues firstType = (TypeValues) Enum.Parse(typeof(TypeValues), Type1[monSelection1.SelectedIndex]);
                TypeValues secondType = (TypeValues)Enum.Parse(typeof(TypeValues), Type2[monSelection1.SelectedIndex]);
                type1Selection1.SelectedIndex = Convert.ToInt32(firstType);
                type2Selection1.SelectedIndex = Convert.ToInt32(secondType);
            }
        }

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
        }

        /// <summary>Gets the type and power of the attack that is selected every time a new attack is selected</summary>
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
    }
}
