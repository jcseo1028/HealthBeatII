using System;
using System.Collections.Generic;

using System.Data;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.IO;

using Plugin.SimpleAudioPlayer;
using HealthBeatII.Models;
using HealthBeatII.ViewModels;

namespace HealthBeatII.Views
{
    public partial class AboutPage : ContentPage
    {
        DateTime dt_Start;
        TimeSpan m_ts;
        bool m_bStartMode;
        int m_iBPM;

        private ISimpleAudioPlayer _simpleAudioPlayer;

        private int m_iSelectedCombinedIndex = -1;

        public List<CombinedPracticeItem> m_listCombinedItem;
        public List<PracticeItem> m_listPracticeItem;

        public AboutViewModel _vewModel;

        public AboutPage()
        {
            InitializeComponent();
            BindingContext = _vewModel = new AboutViewModel();

            LoadData();

            _simpleAudioPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            Stream beepStream = GetType().Assembly.GetManifestResourceStream("HealthBeatII.beep.mp3");
            bool isSuccess = _simpleAudioPlayer.Load(beepStream);

            dt_Start = DateTime.Now;
            m_bStartMode = false;

            m_iBPM = 50;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
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
            pickerCombined.Items.Clear();

            foreach(var itemCombined in m_listCombinedItem)
            {
                string strPickerItem = "";
                strPickerItem += itemCombined.Name;
                strPickerItem += " ( ";
                string[] strs = itemCombined.PracticeItemList.Split(',');
                for (int i = 0; i < strs.Length - 1; i++)
                {
                    int nId = Convert.ToInt32(strs[i]);
                    PracticeItem itemTemp = GetPracticeItemById(nId);

                    if(itemTemp != null)
                    {
                        strPickerItem += itemTemp.Name + ", ";
                    }
                }
                strPickerItem = strPickerItem.Substring(0, strPickerItem.Length - 2);
                strPickerItem += " )";

                pickerCombined.Items.Add(strPickerItem);
            }
        }

        private PracticeItem GetPracticeItemById(int nID)
        {
            foreach(var itemPractice in m_listPracticeItem)
            {
                if (itemPractice.Id == nID)
                    return itemPractice;
            }
            return null;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if ((sender as Button).Text == "Start")
            {
                // Item 이 선택되어 있지 않으면 시작 금지.
                if (m_iSelectedCombinedIndex == -1)
                {
                    DisplayAlert("확인", "Item 을 선택해 주세요.", "확인");
                    return;
                }

                dt_Start = DateTime.Now;
                m_bStartMode = true;

                StartOperation();

                (sender as Button).Text = "Stop";
            }
            else
            {
                (sender as Button).Text = "Start";
                //LabelTime.Text = "시작 버튼을 누르세요.";
                LabelTime.Text = string.Format("{0:D2} : {1:D2} . {2:D2}", m_ts.Minutes, m_ts.Seconds, m_ts.Milliseconds / 10);
                m_bStartMode = false;
                SaveHistory();
            }
        }

        async void SaveHistory()
        {
            HistoryItem historyitem = new HistoryItem();

            historyitem.BPM = m_iBPM;
            historyitem.StartTime = dt_Start.ToString("yyyyMMddHHmmss");
            historyitem.EndTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            string[] strs = pickerCombined.SelectedItem.ToString().Split('(');

            historyitem.CombinedItemId = GetCombinedItemByName(strs[0].Trim());

            HistoryDatabase database = await HistoryDatabase.Instance;

            await database.SaveItemAsync(historyitem);

            pickerCombined.Items.Clear();
            _vewModel.OnAppearing();
        }

        private int GetCombinedItemByName(string strName)
        {
            foreach(var itemCombined in m_listCombinedItem)
            {
                if (itemCombined.Name.CompareTo(strName) == 0)
                    return itemCombined.Id;
            }

            return -1;
        }

        private void StartOperation()
        {
            // 일단 한번 울리고 시작.
            PlaySound();

            int ibpm = (int)((60.0 * 1000) / m_iBPM);

            // 타이머 이렇게 쓰는 게 맞는건가? 
            // BPM 시간이 일정하지 않은 거 같음. 
            // Xamarin 카페에 문의해보자!! 
            // async method??
              
            //Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            Device.StartTimer(TimeSpan.FromMilliseconds(ibpm), () =>
            {
                PlaySound();

                TimeSpan m_ts = DateTime.Now - dt_Start;

                if (m_bStartMode)
                {
                    return true; // return true to repeat counting, false to stop timer
                }
                else
                {
                    return false;
                }
            });

            Device.StartTimer(TimeSpan.FromMilliseconds(50), () =>
            {
                m_ts = DateTime.Now - dt_Start;
                LabelTime.Text = string.Format("{0:D2} : {1:D2} . {2:D2}", m_ts.Minutes, m_ts.Seconds, m_ts.Milliseconds / 10);

                if (m_bStartMode)
                {
                    return true; // return true to repeat counting, false to stop timer
                }
                else
                {
                    return false;
                }
            });
        }

        private void PlaySound()
        {
            _simpleAudioPlayer.Play();
        }

        void OnStepperValueChanged(object sender, ValueChangedEventArgs e)
        {
            double value = e.NewValue;

            m_iBPM = (int)value;

            LabelBPM.Text = "BPM : " + m_iBPM.ToString();
        }

        private void pickerCombined_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (pickerCombined.SelectedIndex == -1)
                return;

            m_iSelectedCombinedIndex = pickerCombined.SelectedIndex;

            LabelCombined.Text = pickerCombined.SelectedItem.ToString();
        }
    }
}