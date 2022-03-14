
using HealthBeatII.Models;
using HealthBeatII.ViewModels;
using HealthBeatII.Views;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;



namespace HealthBeatII.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class HistoryPage : ContentPage
    {
        HistoryViewModel historyViewModel;

        public HistoryPage()
        {
            InitializeComponent();

            BindingContext = historyViewModel = new HistoryViewModel();

            historyViewModel.OnAppearing();
        }
    }
}