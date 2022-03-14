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
    //[XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NewPracticeItemPage : ContentPage
    {
        public PracticeItem Item { get; set; }
        public NewPracticeItemPage()
        {
            InitializeComponent();

            BindingContext = new NewPracticeItemViewModel();
        }
    }
}