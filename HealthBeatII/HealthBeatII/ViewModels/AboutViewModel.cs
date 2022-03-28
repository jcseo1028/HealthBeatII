using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

using HealthBeatII.Models;
using HealthBeatII.ViewModels;

namespace HealthBeatII.ViewModels
{
    public class AboutViewModel : BaseViewModel
    {
        public ICommand OpenWebCommand { get; }
        public Command LoadItemsCommand { get; }

        public List<CombinedPracticeItem> m_listCombinedItem;
        public List<PracticeItem> m_listPracticeItem;

        public AboutViewModel()
        {
            Title = "Health Beat II";
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            //OpenWebCommand = new Command(async () => await Browser.OpenAsync("https://aka.ms/xamarin-quickstart"));
        }

        public void OnAppearing()
        {
            IsBusy = true;
            //SelectedItem = null;
        }

        private async Task ExecuteLoadItemsCommand()
        {
            IsBusy = true;

            try
            {
                LoadData();
            }
            catch (Exception ex)
            {
               
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async void LoadData()
        {
            HistoryDatabase database = await HistoryDatabase.Instance;

            m_listCombinedItem = new List<CombinedPracticeItem>();
            m_listPracticeItem = new List<PracticeItem>();

            m_listCombinedItem = await database.GetCombinedPracticeItemsAsync();
            m_listPracticeItem = await database.GetPracticeItemsAsync();

            // Picker Item 에 추가하기
            //pickerCombined.SelectedIndex = -1;
            Picker pickerCombined = Shell.Current.CurrentPage.FindByName<Picker>("pickerCombined");
            //pickerCombined.SelectedItem = null;

            // 왜 Clear 가 안 되나?
            //pickerCombined.Items.Clear();

            foreach (var itemCombined in m_listCombinedItem)
            {
                string strPickerItem = "";
                strPickerItem += itemCombined.Name;
                strPickerItem += " ( ";
                string[] strs = itemCombined.PracticeItemList.Split(',');
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    int nId = Convert.ToInt32(strs[i]);
                    PracticeItem itemTemp = GetPracticeItemById(nId);

                    if (itemTemp != null)
                    {
                        strPickerItem += itemTemp.Name + ", ";
                    }
                }
                strPickerItem += " )";

                pickerCombined.Items.Add(strPickerItem);
            }

            //pickerCombined.SelectedIndex = 0;
            //await database.SaveItemAsync(historyitem);
        }

        private PracticeItem GetPracticeItemById(int nID)
        {
            foreach (var itemPractice in m_listPracticeItem)
            {
                if (itemPractice.Id == nID)
                    return itemPractice;
            }
            return null;
        }

    }
}