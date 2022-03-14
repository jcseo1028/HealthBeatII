using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using HealthBeatII.Models;
using HealthBeatII.Views;
using Xamarin.Forms;

namespace HealthBeatII.ViewModels
{
    public class PracticeViewModel : BaseViewModel
    {
        private PracticeItem _selectedItem;

        public PracticeItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public ObservableCollection<PracticeItem> Items { get; }
        public Command LoadItemsCommand { get; }

        public List<PracticeItem> m_listPractice;

        public Command AddItemCommand { get; }
        public Command<PracticeItem> ItemTapped { get; }

        public PracticeViewModel()
        {
            Title = "Practice Items";

            m_listPractice = new List<PracticeItem>();

            Items = new ObservableCollection<PracticeItem>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<PracticeItem>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            HistoryDatabase database = await HistoryDatabase.Instance;
            m_listPractice = await database.GetPracticeItemsAsync();

            //m_listPractice.Reverse();

            //m_listHistory.RemoveAt(2);
            //m_listHistory.RemoveAt(2);

            try
            {
                Items.Clear();
                //var items = await DataStore.GetItemsAsync(true);
                foreach (var item in m_listPractice)
                {
                    Items.Add(item);
                    /*
                    item.reserved1 = string.Format("{0} --- BPM : {1:D2}", DateTime.ParseExact(item.StartTime, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH 시 mm 분"), item.BPM);
                    TimeSpan ts = DateTime.ParseExact(item.EndTime, "yyyyMMddHHmmss", null) - DateTime.ParseExact(item.StartTime, "yyyyMMddHHmmss", null);
                    item.reserved2 = string.Format("{0:D2} Minutes", (int)ts.TotalMinutes);
                    Items.Add(item);
                    */
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        public void OnAppearing()
        {
            IsBusy = true;
            SelectedItem = null;
        }

        private async void OnAddItem(object obj)
        {
            // Page 추가 필요. NewPracticeItemPage
            await Shell.Current.GoToAsync(nameof(NewPracticeItemPage));
        }

        async void OnItemSelected(PracticeItem item)
        {
            return;

            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack

            // ViewModel 추가 필요. PracticeItemDetailViewModel
            // await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");
        }
    }
}
