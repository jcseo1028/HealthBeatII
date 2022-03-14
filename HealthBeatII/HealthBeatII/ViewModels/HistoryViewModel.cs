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

        public HistoryViewModel()
        {
            Title = "History";

            m_listHistory = new List<HistoryItem>();

            Items = new ObservableCollection<HistoryItem>();

            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

        }

        async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            HistoryDatabase database = await HistoryDatabase.Instance;
            m_listHistory = await database.GetHistoryItemsAsync();

            m_listHistory.Reverse();

            //m_listHistory.RemoveAt(2);
            //m_listHistory.RemoveAt(2);

            try
            {
                Items.Clear();
                //var items = await DataStore.GetItemsAsync(true);
                foreach (var item in m_listHistory)
                {
                    item.reserved1 = string.Format("{0} --- BPM : {1:D2}", DateTime.ParseExact(item.StartTime, "yyyyMMddHHmmss", null).ToString("yyyy-MM-dd HH 시 mm 분"), item.BPM);
                    TimeSpan ts = DateTime.ParseExact(item.EndTime, "yyyyMMddHHmmss", null) - DateTime.ParseExact(item.StartTime, "yyyyMMddHHmmss", null);
                    item.reserved2 = string.Format("{0:D2} Minutes", (int)ts.TotalMinutes);
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
            //SelectedItem = null;
        }

        private void UpdatePage()
        {
        }
    }
}