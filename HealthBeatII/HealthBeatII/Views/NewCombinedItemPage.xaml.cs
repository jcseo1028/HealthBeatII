using System;
using System.Diagnostics;
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
    public partial class NewCombinedItemPage : ContentPage
    {
        public CombinedPracticeItem Item { get; set; }
        public NewCombinedItemViewModel _viewModel;

        public NewCombinedItemPage()
        {
            InitializeComponent();

            BindingContext = _viewModel = new NewCombinedItemViewModel();
        }

        private void Picker_SelectedIndexChanged(object sender, EventArgs e)
        {
            Picker picker = (Picker)sender;

            _viewModel.AddPracticeItem(picker.SelectedIndex);
        }
    }
}