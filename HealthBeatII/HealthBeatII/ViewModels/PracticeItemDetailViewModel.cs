using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Xamarin.Forms;
using HealthBeatII.Models;
using HealthBeatII.Views;


namespace HealthBeatII.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    class PracticeItemDetailViewModel : BaseViewModel
    {
        private string itemId;
        private string text;
        private string description;
        public string Id { get; set; }

        public string Text
        {
            get => text;
            set => SetProperty(ref text, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;
                LoadItemId(value);
            }
        }

        public async void LoadItemId(string itemId)
        {
            try
            {
                HistoryDatabase database = await HistoryDatabase.Instance;

                //var item = await database.GetHistoryItemAsync(Convert.ToInt32(itemId));
                var item = await database.GetPracticeItemAsync(Convert.ToInt32(itemId));

                Id = item.Id.ToString();
                Text = item.Name;
                Description = item.Description;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }
    }
}
