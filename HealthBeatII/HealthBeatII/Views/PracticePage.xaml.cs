using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HealthBeatII.Models;
using HealthBeatII.ViewModels;
using HealthBeatII.Views;

namespace HealthBeatII.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PracticePage : ContentPage
    {
        PracticeViewModel practiceViewModel;
        public PracticePage()
        {
            InitializeComponent();

            BindingContext = practiceViewModel = new PracticeViewModel();

            practiceViewModel.OnAppearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            practiceViewModel.OnAppearing();
        }
    }
}