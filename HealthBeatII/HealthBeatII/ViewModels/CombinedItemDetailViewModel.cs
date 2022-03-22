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
    public class CombinedItemDetailViewModel : BaseViewModel
    {
        private string itemId;

        public string ItemId
        {
            get
            {
                return itemId;
            }
            set
            {
                itemId = value;

                // 임시 주석
                //LoadItemId(value);
            }
        }

        public CombinedItemDetailViewModel()
        {
            Title = "Selected Combined Item";

        }
    }
}
