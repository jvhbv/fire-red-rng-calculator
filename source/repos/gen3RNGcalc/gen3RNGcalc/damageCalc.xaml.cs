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

        public damageCalc()
        {
            InitializeComponent();
            var directory = System.IO.Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var file = System.IO.Path.Combine(directory, "baseStats.csv");
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
                    monSelection1.Items.Add(mons);
                    monSelection2.Items.Add(mons);
                }
            }
        }

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
                return finalDefender1 * finalDefender2;
            }
        }

        private void Go(object sender, RoutedEventArgs e)
        {
            alerts = "";
            alert.Text = alerts;
            finalCalc.Text = "";

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

            bool? clear = noWeather.IsChecked;
            bool? rain = raining.IsChecked;
            bool? sun = sunny.IsChecked;
            bool? sand = sandstorm.IsChecked;
            bool? hail = hailing.IsChecked;

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

            // Nature determination
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
                    int HPCalc = (((2 * yourBaseHP + HPIV1.SelectedIndex + (yourHPEVs / 4)) * yourLevel) / 100) + yourLevel + 10;
                    HPTot1.Text = HPCalc.ToString();
                    double initialAtkCalc = ((((2 * yourBaseAtk + AtkIV1.SelectedIndex + (yourAtkEVs / 4)) * yourLevel) / 100) + 5) * atkNature * atkBadge;
                    int AtkCalc = Convert.ToInt32(Math.Floor(initialAtkCalc));
                    AtkTot1.Text = AtkCalc.ToString();
                    double initialDefCalc = ((((2 * yourBaseDef + DefIV1.SelectedIndex + (yourDefEVs / 4)) * yourLevel) / 100) + 5) * defNature * defBadge;
                    int DefCalc = Convert.ToInt32(Math.Floor(initialDefCalc));
                    DefTot1.Text = DefCalc.ToString();
                    double initialSpAtkCalc = ((((2 * yourBaseSpAtk + SpAtkIV1.SelectedIndex + (yourSpAtkEVs / 4)) * yourLevel) / 100) + 5) * spAtkNature * spAtkBadge;
                    int spAtkCalc = Convert.ToInt32(Math.Floor(initialSpAtkCalc));
                    SpAtkTot1.Text = spAtkCalc.ToString();
                    double initialSpDefCalc = ((((2 * yourBaseSpDef + SpDefIV1.SelectedIndex + (yourSpDefEVs / 4)) * yourLevel) / 100) + 5) * spDefNature * spDefBadge;
                    int spDefCalc = Convert.ToInt32(Math.Floor(initialSpDefCalc));
                    SpDefTot1.Text = spDefCalc.ToString();
                    double initialSpeedCalc = ((((2 * yourBaseSpeed + SpeedIV1.SelectedIndex + (yourSpeedEVs / 4)) * yourLevel) / 100) + 5) * speedNature * speedBadge;
                    int SpeedCalc = Convert.ToInt32(Math.Floor(initialSpeedCalc));
                    SpeedTot1.Text = SpeedCalc.ToString();
                    EnemyCalc();

                    int power = ParseInput(basePower1.Text, "Please enter a numerical value for the base power.", "A move's base power can not be higher than 255.", 255);
                    bool bpParse = tryIt;
                    alert.Text = alerts;

                    if (bpParse)
                    {
                        double firePower = 1;
                        double waterPower = 1;
                        double crit = 1;
                        double STAB = 1;
                        double effective = 1;
                        if (rain == true)
                        {
                            firePower = 0.5;
                            waterPower = 1.5;
                        }
                        if (sun == true)
                        {
                            firePower = 1.5;
                            waterPower = 0.5;
                        }
                        if (forceCrit1.IsChecked == true)
                        {
                            crit = 2;
                        }
                        if (moveTypeSelection.SelectedIndex == type1Selection1.SelectedIndex || moveTypeSelection.SelectedIndex == type2Selection1.SelectedIndex)
                        {
                            STAB = 1.5;
                        }
                        effective = effectiveness(moveTypeSelection.SelectedIndex, type1Selection2.SelectedIndex, type2Selection2.SelectedIndex);
                        double[] rolls = {0.85, 0.86, 0.87, 0.88, 0.89, 0.90, 0.91, 0.92, 0.93, 0.94, 0.95, 0.96, 0.97, 0.98, 0.99, 1.0};
                        double modifier;
                        bool isPhysical = false;
                        int typeOfMove = moveTypeSelection.SelectedIndex;
                        if (typeOfMove == 0 || typeOfMove == 1 || typeOfMove == 2 || typeOfMove == 9 || typeOfMove == 10 || typeOfMove == 11 || typeOfMove == 12 || typeOfMove == 13 || typeOfMove == 16)
                        {
                            isPhysical = true;
                        }
                        //foreach (double roll in rolls)
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
                            double damage = Math.Floor(Math.Floor(Math.Floor(2 * lvdouble / 5 + 2) * atkdouble * power / double.Parse(defOfDefender)) / 50);
                            damage += 2;
                            finalCalc.Text = finalCalc.Text + (damage * roll/100).ToString() + " ";
                            damage = Math.Floor((damage * modifier)/100);
                            int finalDamage = Convert.ToInt32(Math.Floor(damage));
                            finalCalc.Text = finalCalc.Text + finalDamage.ToString() + ", ";
                        }
                    }
                }
            }
        }

        public void EnemyCalc()
        {
            double atkNature = 1;
            double defNature = 1;
            double spAtkNature = 1;
            double spDefNature = 1;
            double speedNature = 1;

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
                int HPCalc = (((2 * yourBaseHP + HPIV2.SelectedIndex + (yourHPEVs / 4)) * yourLevel) / 100) + yourLevel + 10;
                HPTot2.Text = HPCalc.ToString();
                double initialAtkCalc = ((((2 * yourBaseAtk + AtkIV2.SelectedIndex + (yourAtkEVs / 4)) * yourLevel) / 100) + 5) * atkNature;
                int AtkCalc = Convert.ToInt32((Math.Floor(initialAtkCalc)));
                AtkTot2.Text = AtkCalc.ToString();
                double initialDefCalc = ((((2 * yourBaseDef + DefIV2.SelectedIndex + (yourDefEVs / 4)) * yourLevel) / 100) + 5) * defNature;
                int DefCalc = Convert.ToInt32((Math.Floor(initialDefCalc)));
                DefTot2.Text = DefCalc.ToString();
                double initialSpAtkCalc = ((((2 * yourBaseSpAtk + SpAtkIV2.SelectedIndex + (yourSpAtkEVs / 4)) * yourLevel) / 100) + 5) * spAtkNature;
                int spAtkCalc = Convert.ToInt32((Math.Floor(initialSpAtkCalc)));
                SpAtkTot2.Text = spAtkCalc.ToString();
                double initialSpDefCalc = ((((2 * yourBaseSpDef + SpDefIV2.SelectedIndex + (yourSpDefEVs / 4)) * yourLevel) / 100) + 5) * spDefNature;
                int spDefCalc = Convert.ToInt32((Math.Floor(initialSpDefCalc)));
                SpDefTot2.Text = spDefCalc.ToString();
                double initialSpeedCalc = ((((2 * yourBaseSpeed + SpeedIV2.SelectedIndex + (yourSpeedEVs / 4)) * yourLevel) / 100) + 5) * speedNature;
                int SpeedCalc = Convert.ToInt32((Math.Floor(initialSpeedCalc)));
                SpeedTot2.Text = SpeedCalc.ToString();
            }
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
    }
}
