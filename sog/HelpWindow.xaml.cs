
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using sog.src.model;

namespace sog
{
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}