using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using HealthBeatII.Models;
using HealthBeatII.Views;
using Xamarin.Forms;

namespace HealthBeatII.ViewModels
{
    public class NewCombinedItemViewModel : BaseViewModel
    {
        private string name;
        private string description;

        public ObservableCollection<PracticeItem> Items { get; }
        private List<PracticeItem> m_listPractice;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        private List<string> m_strListPractice;
        public IList<string> PracticeItems
        {
            get
            {
                return m_strListPractice;
            }
            /*
             * Picker 에 값이 초기화되는 경우가 있는 거 같음.
            set
            {
                m_strListPractice = value;
                OnPropertyChanged();
            }
            */
            //set => SetProperty{ ref }
        }

        private List<string> listSelectedPractice;
        public IList<string> SelectedPracticeItems
        {
            get
            {
                return listSelectedPractice;
            }
            /*
            set
            {
                m_strListPractice = value;
                OnPropertyChanged();
            }
            */
            //set => SetProperty{ ref }
        }

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public Command LoadItemsCommand { get; }
        public Command<PracticeItem> ItemTapped { get; }


        public NewCombinedItemViewModel()
        {
            Title = "New Combined Practice Item";

            UpdatePracticeItems();


            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());
            ItemTapped = new Command<PracticeItem>(OnItemSelected);
            Items = new ObservableCollection<PracticeItem>();


            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();

            listSelectedPractice = new List<string>();
        }

        async void OnItemSelected(PracticeItem item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack

            // ViewModel 추가 필요. CombinedItemDetailViewModel
            //await Shell.Current.GoToAsync($"{nameof(CombinedItemDetailViewModel)}?{nameof(CombinedItemDetailViewModel.ItemId)}={item.Id.ToString()}");

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
                    //Items.Add(item);
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

        public async void UpdatePracticeItems()
        {
            m_listPractice = new List<PracticeItem>();
            m_strListPractice = new List<string>();

            HistoryDatabase database = await HistoryDatabase.Instance;
            m_listPractice = await database.GetPracticeItemsAsync();

            for (int i = 0; i < m_listPractice.Count; i++)
            {
                m_strListPractice.Add(m_listPractice[i].Name);
            }
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(name)
                && !String.IsNullOrWhiteSpace(description);
        }

        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            CombinedPracticeItem newItem = new CombinedPracticeItem()
            {
                Name = Name,
                Description = Description
            };

            if(listSelectedPractice.Count == 0)
            {
                await Shell.Current.CurrentPage.DisplayAlert("확인", "Practice Item 이 추가되지 않았습니다. ", "확인");
                return;
            }

            // CombinedItem 에 추가된 PracticeItemList 생성
            newItem.PracticeItemList = MarkPracticeItemList();

            HistoryDatabase database = await HistoryDatabase.Instance;
            await database.SaveCommbinedItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        public async void AddPracticeItem(int iSelectedIdx)
        {
            bool answer = await Shell.Current.CurrentPage.DisplayAlert("확인", PracticeItems[iSelectedIdx] + " 을/를 추가하시겠습니까?", "확인", "취소");

            if (!answer)
                return;

            listSelectedPractice.Add(PracticeItems[iSelectedIdx]);
        }

        private string MarkPracticeItemList()
        {
            string strList = "";

            for (int i = 0; i < listSelectedPractice.Count; i++)
            {
                int Id = GetPracticeItemIndexFromName(listSelectedPractice[i]);

                if (Id != -1)
                {
                    strList += Id.ToString() + ",";
                }
            }

            return strList;
        }

        private int GetPracticeItemIndexFromName(string strName)
        {
            for (int i = 0; i < m_listPractice.Count; i++)
            {
                if (m_listPractice[i].Name.CompareTo(strName) == 0)
                {
                    Items.Add(m_listPractice[i]);
                    return m_listPractice[i].Id;
                }
            }

            return -1;
        }
    }
}
