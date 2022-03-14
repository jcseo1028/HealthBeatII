using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using System.IO;

using Plugin.SimpleAudioPlayer;
using HealthBeatII.Models;

namespace HealthBeatII.Views
{
    public partial class AboutPage : ContentPage
    {
        DateTime dt_Start;
        TimeSpan m_ts;
        bool m_bStartMode;
        int m_iBPM;

        private ISimpleAudioPlayer _simpleAudioPlayer;

        public AboutPage()
        {
            InitializeComponent();

            _simpleAudioPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            Stream beepStream = GetType().Assembly.GetManifestResourceStream("HealthBeatII.beep.mp3");
            bool isSuccess = _simpleAudioPlayer.Load(beepStream);

            dt_Start = DateTime.Now;
            m_bStartMode = false;

            m_iBPM = 50;

        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            if ((sender as Button).Text == "Start")
            {
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
            //var historyItem = (TodoItem)BindingContext;

            HistoryItem historyitem = new HistoryItem();

            historyitem.BPM = m_iBPM;
            historyitem.StartTime = dt_Start.ToString("yyyyMMddHHmmss");
            historyitem.EndTime = DateTime.Now.ToString("yyyyMMddHHmmss");

            HistoryDatabase database = await HistoryDatabase.Instance;

            await database.SaveItemAsync(historyitem);
            //await Navigation.PopAsync();
        }

        private void StartOperation()
        {
            // 일단 한번 울리고 시작.
            PlaySound();

            int ibpm = (int)((60.0 * 1000) / m_iBPM);

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
    }
}