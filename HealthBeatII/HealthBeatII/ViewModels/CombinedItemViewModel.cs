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
    public class CombinedItemViewModel : BaseViewModel
    {
        private CombinedPracticeItem _selectedItem;

        public CombinedPracticeItem SelectedItem
        {
            get => _selectedItem;
            set
            {
                SetProperty(ref _selectedItem, value);
                OnItemSelected(value);
            }
        }

        public ObservableCollection<CombinedPracticeItem> Items { get; }
        public Command LoadItemsCommand { get; }

        public List<CombinedPracticeItem> m_listCobmbinedPractice;

        public Command AddItemCommand { get; }
        public Command<CombinedPracticeItem> ItemTapped { get; }

        public CombinedItemViewModel()
        {
            Title = "Combined Practice Items";

            m_listCobmbinedPractice = new List<CombinedPracticeItem>();

            Items = new ObservableCollection<CombinedPracticeItem>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<CombinedPracticeItem>(OnItemSelected);

            AddItemCommand = new Command(OnAddItem);
        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            HistoryDatabase database = await HistoryDatabase.Instance;
            m_listCobmbinedPractice = await database.GetCombinedPracticeItemsAsync();

            try
            {
                Items.Clear();
                //var items = await DataStore.GetItemsAsync(true);
                foreach (var item in m_listCobmbinedPractice)
                {
                    Items.Add(item);
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
            // Page 추가 필요. NewCombinedPracticeItemPage
            await Shell.Current.GoToAsync(nameof(NewCombinedItemPage));
        }

        async void OnItemSelected(CombinedPracticeItem item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack

            // ViewModel 추가 필요. CombinedItemDetailViewModel
            await Shell.Current.GoToAsync($"{nameof(CombinedItemDetailViewModel)}?{nameof(CombinedItemDetailViewModel.ItemId)}={item.Id.ToString()}");

        }
    }
}
