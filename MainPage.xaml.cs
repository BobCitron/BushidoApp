﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BushidoApp.Resources;

namespace BushidoApp
{
    public class Functionnality
    {
        public string Title { get; set; }
        public string XamlAdress { get; set;}

        public override string ToString()
        {
            return Title;
        }
    }

    public partial class MainPage : PhoneApplicationPage
    {
        /* 
         * This class is the entry point of the app.
         * Adding a "functionnality" will add a link named by Title redirecting to the XamlAdress.
         */
         
        // constructor
        public MainPage()
        {
            InitializeComponent();
            List<Functionnality> item = new List<Functionnality>()
            {
                // catalog: all the pictures & profile
                new Functionnality {Title = "Catalog", XamlAdress = "/Catalog.xaml"},
                // statistic simulator
                new Functionnality {Title = "Dice simulator", XamlAdress = "/Statistic.xaml"},
                // information about the app
                new Functionnality {Title = "about", XamlAdress = "/About.xaml"},
            };
            MenuList.ItemsSource = item;
        }

        // trigger the button_click with the redirecting function
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Button from = (Button)sender;
            NavigationService.Navigate(new Uri(from.CommandParameter.ToString(), UriKind.Relative));
        }
    }
}