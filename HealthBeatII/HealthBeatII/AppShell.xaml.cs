using HealthBeatII.ViewModels;
using HealthBeatII.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace HealthBeatII
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

            // "Unable to figure out route for" 에러 시, 페이지 추가 시 아래 Route 처리 해줘야 함.
            Routing.RegisterRoute(nameof(NewPracticeItemPage), typeof(NewPracticeItemPage));
            Routing.RegisterRoute(nameof(PracticeItemDetailPage), typeof(PracticeItemDetailPage));
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
