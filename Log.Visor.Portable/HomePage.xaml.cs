﻿using Log.Visor.Portable.ViewModels;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Log.Visor.Portable
{
    public partial class HomePage: ContentPage
    {
        public HomePage()
        {
            BindingContext = LogViewModel.ViewModel;
            InitializeComponent();
        }
    }
}

