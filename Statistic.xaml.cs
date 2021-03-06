﻿using System;
using System.Windows.Markup;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using statistics;
using System.Diagnostics;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace BushidoApp {

    public class SimulationType {
        /*
        * This class handle the different simulation to do
        */

        private static Dictionary<string, string> initializeAttackDictionnary() {
            Dictionary<string,string> d = new Dictionary<string,string>();
            d.Add("attack", "Melee dice");
            d.Add("shoot", "Ranged dice");
            d.Add("slMelee", "Melee dice");
            d.Add("slRanged", "Ranged dice");
            d.Add("slOpposed", "Attack dice");
            d.Add("slTarget", "Attack dice");

            return d;
        }

        private static Dictionary<string, string> initializeDefenseDictionnary() {
            Dictionary<string,string> d = new Dictionary<string,string>();
            d.Add("attack", "Defence dice");
            d.Add("shoot", "Difficulty dice");
            d.Add("slMelee", "Defense dice");
            d.Add("slRanged", "Difficulty dice");
            d.Add("slOpposed", "Defense dice");
            d.Add("slTarget", "Defense dice");

            return d;
        }

        static public Dictionary<string, string> attackTextDictionnary = initializeAttackDictionnary();
        static public Dictionary<string, string> defenseTextDictionnary = initializeDefenseDictionnary();

        static public double[] computeResult(Profile attacker, Profile defender, string simulationType, int nbRoll) {
            double[] result;    

            if (simulationType == "attack") {   
                result = ResultPresentation.Integral(attacker.Attack(defender, max: nbRoll));
            } else if (simulationType == "shoot") {
                // here defenceDice is used to have the difficulty of the test
                result = ResultPresentation.Integral(attacker.Shoot(defender, defender.DefenceDice, nbRoll));
            } else if (simulationType == "slMelee") {
                result = ResultPresentation.Integral(attacker.SuccessLevel_Melee(defender, max: nbRoll));
            } else if (simulationType == "slRanged") {
                // here defenceDice is used to have the difficulty of the test
                result = ResultPresentation.Integral(attacker.SuccessLevel_Ranged(defender.DefenceDice, max: nbRoll));
            } else if (simulationType == "slOpposed") {
                result = ResultPresentation.Integral(attacker.SuccessLevel_Opposed(defender, max: nbRoll));
            } else {
                // "slTarget"
                result = ResultPresentation.Integral(attacker.SuccessLevel_Opposed(defender, max: nbRoll));
            }

            return result;
        }
    }

    public partial class Statistic : PhoneApplicationPage, INotifyPropertyChanged {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName) {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler) {
                handler(this, 
                    new PropertyChangedEventArgs(propertyName));
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.GoBack();
        }

        private bool NotifyPropertyChanged<T>(ref T variable, T valeur, [CallerMemberName] string nomPropriete = null)
        {
            if (object.Equals(variable, valeur)) return false;

            variable = valeur;
            NotifyPropertyChanged(nomPropriete);
            return true;
        }

        // handle the modification of selection for the simulation type (attack / shoot / etc...)
        private void Picker(object sender, SelectionChangedEventArgs e)
        {
            if (loaded) {
                ListPickerItem whichRoll = (ListPickerItem)toCompute.SelectedItem;
                string simulationType = whichRoll.Name.ToString();
                
                // initialize the text
                attackValue.Text = SimulationType.attackTextDictionnary[simulationType];
                defendValue.Text = SimulationType.defenseTextDictionnary[simulationType];
            }
        }

        // List of property available for the attacker
        private List<string> _AttackerPropertyList;
        public List<string> AttackerPropertyList {
            get { return _AttackerPropertyList; }
            set { NotifyPropertyChanged(ref _AttackerPropertyList, value); }
        }

        // List of property available for the defender
        private List<string> _DefenderPropertyList;
        public List<string> DefenderPropertyList {
            get { return _DefenderPropertyList; }
            set { NotifyPropertyChanged(ref _DefenderPropertyList, value); }
        }

        private bool loaded = false;

        public Statistic() {
            InitializeComponent();
            loaded = true;

            DataContext = this;

            toCompute.SetValue(Microsoft.Phone.Controls.ListPicker.ItemCountThresholdProperty, 6);

            // Initialize all the list
            AttackerPropertyList = new List<string>();
            DefenderPropertyList = new List<string>();
            AttackTreatsController = new List<Tuple<Button, ListPicker, TextBox>>();
            DefenceTreatsController = new List<Tuple<Button, ListPicker, TextBox>>();
            AttackTreats = new List<Tuple<string, string>>();

            DefenceTreats = new List<Tuple<string, string>>();

            Profile tmp = new Profile(0, 0);
            //foreach (var value in tmp.Treats.getAttackerMeleeList()) {
            foreach (var value in tmp.Treats.getTreatList()) {  
                AttackerPropertyList.Add(value.Name);
            }
            AttackerPropertyList.Sort();
            foreach (var value in tmp.Treats.getTreatList()) {
                DefenderPropertyList.Add(value.Name);
            }
            DefenderPropertyList.Sort();
        }

        // Generate a result table with the value contained in "result"
        private void TableGeneration(double[] result) {
            ScrollViewer newScroll = new ScrollViewer();
            Grid newGrid = new Grid();
            RowDefinition newRowDef;
            ColumnDefinition newColDef;
            TextBlock newText;

            int rows = 6;
            int cols = 5;
            // Add columns
            for (int col = 0; col < cols; col++) {
                newColDef = new ColumnDefinition();
                newGrid.ColumnDefinitions.Add(newColDef);
            }
            // Add rows
            for (int row = 0; row < rows; row++) {
                newRowDef = new RowDefinition();
                newGrid.RowDefinitions.Add(newRowDef);
            }

            int maxIndex;
            for (maxIndex = result.Length - 1; maxIndex > 0 && result[maxIndex] == 0; maxIndex--);

            maxIndex++;

            for (int row = 0; row < rows; row++) {
                for (int col = 0; col < cols; col++) {
                    newText = new TextBlock();

                    // fill the header
                    if (row % 2 == 0 && col + 5 * (row / 2) < maxIndex) {
                        if (col == 0 && row == 0) {
                            newText.Text = "miss";
                        } else {
                            newText.Text = (col + 5 * (row / 2) - 1).ToString() + "+";
                        }
                        // fill the result
                    } else if (col + 5 * (row / 2) < maxIndex) {
                        newText.Text = result[col + 5 * (row / 2)].ToString();
                    } else {
                        newText.Text = "";
                    }
                    Grid.SetColumn(newText, col);
                    Grid.SetRow(newText, row);

                    newGrid.Children.Add(newText);
                }
            }

            Grid.SetRow(newScroll, 4);
            newScroll.Content = newGrid;
            if (ContentPanel.Children.Count == 5) ContentPanel.Children.RemoveAt(4);
            ContentPanel.Children.Add(newScroll);
        }

        // Number of dice in attack
        int AttackDices;
        // Number of dice in defence
        int DefenceDices;
        // List of controller for the attacker treats
        List<Tuple<Button, ListPicker, TextBox>> AttackTreatsController;
        // List of controller for the defender treats
        List<Tuple<Button, ListPicker, TextBox>> DefenceTreatsController;
        // List of attacker treats
        List<Tuple<string, string>> AttackTreats;
        // List of defender treats
        List<Tuple<string, string>> DefenceTreats;

        // Set the parameter of the simulation
        private void GetParameter() {
            // Set the Attack Dices
            try {
                AttackDices = int.Parse(AttackerDice.Text);
            } catch {
                AttackerDice.Text = "0";
                AttackDices = 0;
            }
            // Set the Defence Dices
            try {
                DefenceDices = int.Parse(DefenderDice.Text);
            } catch {
                DefenderDice.Text = "0";
                DefenceDices = 0;
            }
            // Get the treats
            // First reset the treat list
            AttackTreats.Clear();
            // Attacker
            string nameTmp, valueTmp;
            foreach (Tuple<Button, ListPicker, TextBox> t in AttackTreatsController) {
                nameTmp = t.Item2.SelectedItem.ToString();
                valueTmp = t.Item3.Text;
                AttackTreats.Add(new Tuple<string, string>(nameTmp, valueTmp));
            }
            // Defender
            foreach (Tuple<Button, ListPicker, TextBox> t in DefenceTreatsController) {
                nameTmp = t.Item2.SelectedItem.ToString();
                valueTmp = t.Item3.Text;
                DefenceTreats.Add(new Tuple<string, string>(nameTmp, valueTmp));
            }
        }

        // compute the result
        private double[] ComputeResult() {
            // Create the profile 
            Profile Hiro = new Profile(AttackDices, 0), Bakemono = new Profile(0, DefenceDices);
            Hiro.Treats.Modify(AttackTreats);
            Bakemono.Treats.Modify(DefenceTreats);
            
            // Get the result
            string whichRoll = toCompute.SelectedItem.ToString();
            int nbRoll = 100000;
            double[] result = SimulationType.computeResult(Hiro, Bakemono, whichRoll, nbRoll);

            return result;
        }
        private void ComputeSimulation(object sender, RoutedEventArgs e) {
            // Get the parameter of the simulation
            GetParameter();
            // Compute the result of the simulation
            double[] result = ComputeResult();
            // Display them on a table
            TableGeneration(result);
        }

        private void AddProperty(Grid Panel, List<String> PropertyList, List<Tuple<Button, ListPicker, TextBox>> ControlList, DelegateDeleteProperty DeleteProperty) {
            // Create a new row definition
            RowDefinition rowDef = new RowDefinition();
            // Add two new row definition (why? Dunno)
            Panel.RowDefinitions.Add(rowDef);
            rowDef = new RowDefinition();
            Panel.RowDefinitions.Add(rowDef);

            // Get the number of rows
            int row = Panel.Children.Count;
            // Create a new List picker
            ListPicker newList = new ListPicker();
            // Bind it with the attacker treats list 
            newList.ItemsSource = PropertyList;
            // Give it a place into the grid
            Grid.SetColumn(newList, 1);
            Grid.SetRow(newList, row);
            // Do the same with a TextBox
            TextBox newText = new TextBox();
            newText.Text = "0";
            newText.VerticalAlignment = VerticalAlignment.Center;
            Grid.SetColumn(newText, 2);
            Grid.SetRow(newText, row);
            // Delete button
            Button newButton = new Button();

            bool dark= ((Visibility)Application.Current.Resources["PhoneDarkThemeVisibility"] == Visibility.Visible);
            ImageBrush deleteBrush = new ImageBrush();
            if (! dark) {
                deleteBrush.ImageSource = new BitmapImage(new Uri("Resources\\other\\deleteDark.png", UriKind.Relative));
            }
            else {
                deleteBrush.ImageSource = new BitmapImage(new Uri("Resources\\other\\deleteWhite.png", UriKind.Relative));
            }
            deleteBrush.Stretch = Stretch.Uniform;
            newButton.Background = deleteBrush;
            
            //newButton.Background = new SolidColorBrush(System.Windows.Media.Color.FromArgb(255, 220, 20, 60));

            newButton.BorderThickness = new Thickness(0, 0, 0, 0);
            newButton.VerticalAlignment = VerticalAlignment.Center;
            newButton.Height = 65; // 65
            newButton.Width = 100; // 100
            // Test handler
            newButton.Click += new RoutedEventHandler(DeleteProperty);

            Grid.SetColumn(newButton, 0);
            Grid.SetRow(newButton, row);

            // Add the new controllers to the grid
            Panel.Children.Add(newButton);
            Panel.Children.Add(newList);
            Panel.Children.Add(newText);
            // Add the controllers to the dictionnary
            ControlList.Add(new Tuple<Button, ListPicker, TextBox>(newButton, newList, newText));
        }

        private delegate void DelegateDeleteProperty(object sender, RoutedEventArgs e);

        private void decreaseRow(List<Tuple<Button, ListPicker, TextBox>> TreatsController, int row) {
            foreach (Tuple<Button, ListPicker, TextBox> T in TreatsController) {
                if (Grid.GetRow(T.Item1) >= row) {
                    Grid.SetRow(T.Item1, Grid.GetRow(T.Item1) - 3);
                    Grid.SetRow(T.Item2, Grid.GetRow(T.Item2) - 3);
                    Grid.SetRow(T.Item3, Grid.GetRow(T.Item3) - 3);
                }
            }
        }

        private void DeleteProperty(Button sender, Grid Panel, List<Tuple<Button, ListPicker, TextBox>> TreatsController) {
            int row = Grid.GetRow(sender) / 3;
            Tuple<Button, ListPicker, TextBox> toRemove;

            toRemove = TreatsController.ToList()[row];

            Panel.Children.Remove(toRemove.Item1);
            Panel.Children.Remove(toRemove.Item2);
            Panel.Children.Remove(toRemove.Item3);

            TreatsController.RemoveAt(row);
            decreaseRow(TreatsController, row + 3);
        }

        public void DeleteAttackProperty(object sender, RoutedEventArgs e) {
            Button tmp = (Button)sender;
            DeleteProperty(tmp, AttackerPanel, AttackTreatsController);
        }

        public void DeleteDefenceProperty(object sender, RoutedEventArgs e) {
            Button tmp = (Button)sender;
            DeleteProperty(tmp, DefenderPanel, DefenceTreatsController);
        }

        private void AddAttackerProperty(object sender, RoutedEventArgs e) {
            DelegateDeleteProperty delete = DeleteAttackProperty;
            AddProperty(AttackerPanel, AttackerPropertyList, AttackTreatsController, delete);
        }

        private void AddDefenderProperty(object sender, RoutedEventArgs e) {
            DelegateDeleteProperty delete = DeleteDefenceProperty;
            AddProperty(DefenderPanel, DefenderPropertyList, DefenceTreatsController, delete);
        }

    }
}