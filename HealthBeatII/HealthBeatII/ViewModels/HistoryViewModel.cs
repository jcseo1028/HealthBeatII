using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using HealthBeatII.Models;
using Xamarin.Forms;

namespace HealthBeatII.ViewModels
{
    public class HistoryViewModel : BaseViewModel
    {
        public ObservableCollection<HistoryItem> Items { get; }
        public Command LoadItemsCommand { get; }

        public List<HistoryItem> m_listHistory;
        public List<PracticeItem> m_listPractice;
        public List<CombinedPracticeItem> m_listCombined;

        public Command<HistoryItem> ItemTapped { get; }

        public HistoryViewModel()
        {
            Title = "History";

            m_listHistory = new List<HistoryItem>();
            m_listPractice = new List<PracticeItem>();
            m_listCombined = new List<CombinedPracticeItem>();

            Items = new ObservableCollection<HistoryItem>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            ItemTapped = new Command<HistoryItem>(OnItemSelected);

        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            HistoryDatabase database = await HistoryDatabase.Instance;
            m_listHistory = await database.GetHistoryItemsAsync();
            m_listCombined = await database.GetCombinedPracticeItemsAsync();
            m_listPractice = await database.GetPracticeItemsAsync();

            m_listHistory.Reverse();

            try
            {
                Items.Clear();
                //var items = await DataStore.GetItemsAsync(true);
                foreach (var item in m_listHistory)
                {
                    CombinedPracticeItem combinedItem = GetCombinedPracticeItem(item.CombinedItemId);

                    if (combinedItem != null)
                    {
                        item.reserved1 = string.Format("{0} --- BPM : {1:D2}", DateTime.ParseExact(item.StartTime, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH 시 mm 분"), item.BPM);
                        TimeSpan ts = DateTime.ParseExact(item.EndTime, "yyyyMMddHHmmss", null) - DateTime.ParseExact(item.StartTime, "yyyyMMddHHmmss", null);
                        item.reserved2 = string.Format("{0:D2} Minutes", (int)ts.TotalMinutes);
                        item.reserved3 = string.Format("{0} [ {1} ]", combinedItem.Name, GetPracticeItems(combinedItem.PracticeItemList));

                        Items.Add(item);
                    }
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
            //SelectedItem = null;
        }

        private CombinedPracticeItem GetCombinedPracticeItem(int Id)
        {
            foreach(var itemCombined in m_listCombined)
            {
                if (itemCombined.Id == Id)
                    return itemCombined;
            }
            return null;
        }

        private PracticeItem GetPracticeItem(int Id)
        {
            foreach(var itemPractice in m_listPractice)
            {
                if (itemPractice.Id == Id)
                    return itemPractice;
            }

            return null;
        }

        private string GetPracticeItems(string strIds)
        {
            string strPracticeItems = "";
            string[] strs = strIds.Split(',');

            for(int i=0; i<strs.Length; i++)
            {
                try
                {
                    int tempId = Convert.ToInt32(strs[i].Trim());
                    PracticeItem itemPractice = GetPracticeItem(tempId);
                    if (itemPractice != null)
                    {
                        strPracticeItems += string.Format("{0},", itemPractice.Name);
                    }
                }
                catch
                {
                    continue;
                }
            }

            if(strPracticeItems.Length > 0)
            {
                strPracticeItems = strPracticeItems.Substring(0, strPracticeItems.Length - 1);
            }

            return strPracticeItems;
        }

        private void UpdatePage()
        {
        }

        async void OnItemSelected(HistoryItem item)
        {
            if (item == null)
                return;

            // This will push the ItemDetailPage onto the navigation stack

            Debug.WriteLine("at OnItemSelected : " + item.Id);

            // 해당 아이템을 삭제할 것인 지 팝업 후 삭제.
            bool answer = await Shell.Current.CurrentPage.DisplayAlert("경고", GetCombinedPracticeItem(item.CombinedItemId).Name + "을/를 삭제하시겠습니까?", "확인", "취소");

            if (answer == false)
                return;

            HistoryDatabase database = await HistoryDatabase.Instance;
            await database.DeleteHistoryItemAsync(item);

            OnAppearing();

        }
    }
}