using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using HealthBeatII.Models;
using HealthBeatII.ViewModels;

namespace HealthBeatII.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CombinedItemDetailPage : ContentPage
    {
        private CombinedItemDetailViewModel _viewModel;

        public CombinedItemDetailPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new CombinedItemDetailViewModel();
            _viewModel.OnAppearing();
        }

        private void pickerPractice_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;

            _viewModel.AddPracticeItem(picker.SelectedIndex);
        }
    }
}