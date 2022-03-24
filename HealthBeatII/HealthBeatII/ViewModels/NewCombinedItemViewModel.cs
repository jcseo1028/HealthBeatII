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
        public ObservableCollection<string> PracticeItems;

        private List<PracticeItem> listSelectedPractice;
        public List<PracticeItem> SelectedPracticeItems
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

            listSelectedPractice = new List<PracticeItem>();
            UpdatePracticeItems();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());


            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);

            ItemTapped = new Command<PracticeItem>(OnItemSelected);
            Items = new ObservableCollection<PracticeItem>();

            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        public void OnAppearing()
        {
            IsBusy = true;
            //SelectedItem = null;
        }

        async void OnItemSelected(PracticeItem item)
        {
            if (item == null)
                return;

            // 해당 아이템을 삭제할 것인 지 팝업 후 삭제.

        }

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();

                foreach(var item in listSelectedPractice)
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

        public async void UpdatePracticeItems()
        {
            m_listPractice = new List<PracticeItem>();
            m_strListPractice = new List<string>();
            PracticeItems = new ObservableCollection<string>();

            HistoryDatabase database = await HistoryDatabase.Instance;
            m_listPractice = await database.GetPracticeItemsAsync();

            // View Model 에서 Current Page 의 Control 에 접근하기.
            Picker pickerTemp = Shell.Current.CurrentPage.FindByName<Picker>("pickerPractice");
            pickerTemp.Items.Clear();

            for (int i = 0; i < m_listPractice.Count; i++)
            {
                m_strListPractice.Add(m_listPractice[i].Name);
                pickerTemp.Items.Add(m_listPractice[i].Name);
                PracticeItems.Add(m_listPractice[i].Name);
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
            newItem.PracticeItemList = MakePracticeItemList();

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

            listSelectedPractice.Add(m_listPractice[iSelectedIdx]);

            OnAppearing();
            //await ExecuteLoadItemsCommand();
        }

        private string MakePracticeItemList()
        {
            string strList = "";

            for (int i = 0; i < listSelectedPractice.Count; i++)
            {
                int Id = GetPracticeItemIndexFromName(listSelectedPractice[i].Name);

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
                    //Items.Add(m_listPractice[i]);
                    return m_listPractice[i].Id;
                }
            }

            return -1;
        }
    }
}
