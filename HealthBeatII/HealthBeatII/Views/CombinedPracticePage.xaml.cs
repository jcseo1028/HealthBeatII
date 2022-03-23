using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HealthBeatII.ViewModels;

namespace HealthBeatII.Views
{
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CombinedPracticePage : ContentPage
    {
        CombinedItemViewModel combinedViewModel;

        public CombinedPracticePage()
        {
            InitializeComponent();
            BindingContext = combinedViewModel = new CombinedItemViewModel();

            combinedViewModel.OnAppearing();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            combinedViewModel.OnAppearing();
        }
    }
}