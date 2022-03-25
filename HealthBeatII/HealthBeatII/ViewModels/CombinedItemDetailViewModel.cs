using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        private string name;
        private string description;
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

        public Command UpdateCommand { get; }
        public Command DeleteCommand { get; }

        public Command LoadItemsCommand { get; }
        public Command<PracticeItem> ItemTapped { get; }

        private CombinedPracticeItem thisItem;
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

        public List<PracticeItem> m_listSelectedPractice;
        public List<PracticeItem> m_listAllPractice;
        public ObservableCollection<PracticeItem> Items { get; set; }

        public CombinedItemDetailViewModel()
        {
            Title = "Selected Combined Item";

            m_listSelectedPractice = new List<PracticeItem>();
            m_listAllPractice = new List<PracticeItem>();
            Items = new ObservableCollection<PracticeItem>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            UpdateCommand = new Command(OnUpdate, ValidateSave);
            DeleteCommand = new Command(OnDelete);
            this.PropertyChanged +=
                (_, __) => UpdateCommand.ChangeCanExecute();
            ItemTapped = new Command<PracticeItem>(OnItemSelected);
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
            bool answer = await Shell.Current.CurrentPage.DisplayAlert("확인", item.Name + "을/를 삭제하시겠습니까?", "확인", "취소");

            if (answer == false)
                return;

            m_listSelectedPractice.Remove(item);
            Items.Remove(item);

        }

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                Items.Clear();

                foreach (var item in m_listSelectedPractice)
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

        public async void LoadItemId(string itemId)
        {
            try
            {
                HistoryDatabase database = await HistoryDatabase.Instance;

                //var item = await database.GetHistoryItemAsync(Convert.ToInt32(itemId));
                thisItem = await database.GetCombinedPracticeItemAsync(Convert.ToInt32(itemId));

                Name = thisItem.Name;
                Description = thisItem.Description;

                m_listAllPractice = await database.GetPracticeItemsAsync();

                // Pratice Item List 도 추가해야 한다.
                string[] strPractices = thisItem.PracticeItemList.Split(',');
                int ID = -1;
                Items.Clear();
                m_listSelectedPractice.Clear();

                // View Model 에서 Current Page 의 Control 에 접근하기.
                Picker pickerTemp = Shell.Current.CurrentPage.FindByName<Picker>("pickerPractice");
                pickerTemp.Items.Clear();

                foreach (var prItem in m_listAllPractice)
                {
                    pickerTemp.Items.Add(prItem.Name);
                }

                for (int i = 0; i < strPractices.Length - 1; i++)
                {
                    try
                    {
                        ID = Convert.ToInt32(strPractices[i].Trim());
                    }
                    catch
                    {
                        continue;
                    }

                    if (ID < 1)
                        continue;

                    var item = GetPraticeItemById(ID);

                    if(item != null)
                    {
                        m_listSelectedPractice.Add(item);
                        Items.Add(item);
                    }
                }

                OnAppearing();

            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to Load Item - " + ex.Message);
            }
        }

        private PracticeItem GetPraticeItemById(int ID)
        {
            foreach(var practiceItem in m_listAllPractice)
            {
                if (practiceItem.Id == ID)
                    return practiceItem;
            }
            return null;
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(name)
                && !String.IsNullOrWhiteSpace(description);
        }

        private async void OnUpdate()
        {
            thisItem.Name = Name;
            thisItem.Description = Description;            

            // 수정된 Pratice Item List 도 업데이트해야 함.
            if (m_listSelectedPractice.Count == 0)
            {
                await Shell.Current.CurrentPage.DisplayAlert("확인", "Practice Item 이 추가되지 않았습니다. ", "확인");
                return;
            }

            // CombinedItem 에 추가된 PracticeItemList 생성
            thisItem.PracticeItemList = MakePracticeItemList();

            HistoryDatabase database = await HistoryDatabase.Instance;
            await database.SaveCommbinedItemAsync(thisItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnDelete()
        {
            // Page 에서 DisplayAlert 를 호출하면됨.
            bool answer = await Shell.Current.CurrentPage.DisplayAlert("확인", "삭제하시겠습니까?", "확인", "취소");

            if (answer == false)
                return;

            HistoryDatabase database = await HistoryDatabase.Instance;
            await database.DeleteCombinedPracticeItemAsync(thisItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private string MakePracticeItemList()
        {
            string strList = "";

            for (int i = 0; i < m_listSelectedPractice.Count; i++)
            {
                int Id = GetPracticeItemIndexFromName(m_listSelectedPractice[i].Name);

                if (Id != -1)
                {
                    strList += Id.ToString() + ",";
                }
            }

            return strList;
        }

        private int GetPracticeItemIndexFromName(string strName)
        {
            for (int i = 0; i < m_listAllPractice.Count; i++)
            {
                if (m_listAllPractice[i].Name.CompareTo(strName) == 0)
                {
                    //Items.Add(m_listPractice[i]);
                    return m_listAllPractice[i].Id;
                }
            }

            return -1;
        }

        public async void AddPracticeItem(int iSelectedIdx)
        {
            bool answer = await Shell.Current.CurrentPage.DisplayAlert("확인", m_listAllPractice[iSelectedIdx].Name + " 을/를 추가하시겠습니까?", "확인", "취소");

            if (!answer)
                return;

            m_listSelectedPractice.Add(m_listAllPractice[iSelectedIdx]);

            OnAppearing();
        }
    }
}
