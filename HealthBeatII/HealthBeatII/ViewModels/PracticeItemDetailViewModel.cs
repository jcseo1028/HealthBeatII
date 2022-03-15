using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Xamarin.Forms;
using HealthBeatII.Models;
using HealthBeatII.Views;


namespace HealthBeatII.ViewModels
{
    [QueryProperty(nameof(ItemId), nameof(ItemId))]
    public class PracticeItemDetailViewModel : BaseViewModel
    {
        private string itemId;
        private string name;
        private string part;
        private string description;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string Part
        {
            get => part;
            set => SetProperty(ref part, value);
        }

        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public Command UpdateCommand { get; }
        public Command DeleteCommand { get; }

        private PracticeItem thisItem;

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

        public PracticeItemDetailViewModel()
        {
            Title = "Detailed Practice";

            UpdateCommand = new Command(OnUpdate, ValidateSave);
            DeleteCommand = new Command(OnDelete);
            this.PropertyChanged +=
                (_, __) => UpdateCommand.ChangeCanExecute();
        }

        public async void LoadItemId(string itemId)
        {
            try
            {
                HistoryDatabase database = await HistoryDatabase.Instance;

                //var item = await database.GetHistoryItemAsync(Convert.ToInt32(itemId));
                thisItem = await database.GetPracticeItemAsync(Convert.ToInt32(itemId));

                Name = thisItem.Name;
                Part = thisItem.Part;
                Description = thisItem.Description;
            }
            catch (Exception)
            {
                Debug.WriteLine("Failed to Load Item");
            }
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(name)
                && !String.IsNullOrWhiteSpace(part)
                && !String.IsNullOrWhiteSpace(description);
        }

        private async void OnUpdate()
        {
            thisItem.Name = Name;
            thisItem.Part = Part;
            thisItem.Description = Description;

            HistoryDatabase database = await HistoryDatabase.Instance;
            await database.SavePracticeItemAsync(thisItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnDelete()
        {
            // 삭제할까요? 메시지 추가 필요.            
            // Page 에서 DisplayAlert 를 호출하면됨.
            bool answer = await Shell.Current.CurrentPage.DisplayAlert("확인", "삭제하시겠습니까?", "확인", "취소");
            //Debug.WriteLine("Answer: " + answer);

            if (answer == false)
                return;

            HistoryDatabase database = await HistoryDatabase.Instance;
            await database.DeletePracticeItemAsync(thisItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }
    }
}
