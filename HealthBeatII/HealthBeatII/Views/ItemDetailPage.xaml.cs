using HealthBeatII.ViewModels;
using System.ComponentModel;
using Xamarin.Forms;

namespace HealthBeatII.Views
{
    public partial class ItemDetailPage : ContentPage
    {
        public ItemDetailPage()
        {
            InitializeComponent();
            BindingContext = new ItemDetailViewModel();
        }
    }
}