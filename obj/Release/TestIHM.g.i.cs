﻿#pragma checksum "C:\Users\XavierP\Dropbox\developpement\Projects\BushidoApp\BushidoApp\TestIHM.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "4E51F80570EC7C4A10B64E2B9665AD6F"
//------------------------------------------------------------------------------
// <auto-generated>
//     Ce code a été généré par un outil.
//     Version du runtime :4.0.30319.34003
//
//     Les modifications apportées à ce fichier peuvent provoquer un comportement incorrect et seront perdues si
//     le code est régénéré.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace ListBoxVisibleItems {
    
    
    public partial class MainWindow : System.Windows.Window {
        
        internal System.Windows.Controls.ListBox listbox;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/BushidoApp;component/TestIHM.xaml", System.UriKind.Relative));
            this.listbox = ((System.Windows.Controls.ListBox)(this.FindName("listbox")));
        }
    }
}

