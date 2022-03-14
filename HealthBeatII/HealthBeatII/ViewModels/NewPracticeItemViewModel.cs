using System;
using System.Collections.Generic;
using System.Text;
using HealthBeatII.Models;
using Xamarin.Forms;


namespace HealthBeatII.ViewModels
{
    public class NewPracticeItemViewModel : BaseViewModel
    {
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

        public Command SaveCommand { get; }
        public Command CancelCommand { get; }

        public NewPracticeItemViewModel()
        {
            SaveCommand = new Command(OnSave, ValidateSave);
            CancelCommand = new Command(OnCancel);
            this.PropertyChanged +=
                (_, __) => SaveCommand.ChangeCanExecute();
        }

        private bool ValidateSave()
        {
            return !String.IsNullOrWhiteSpace(name)
                && !String.IsNullOrWhiteSpace(part)
                && !String.IsNullOrWhiteSpace(description);
        }
        
        private async void OnCancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private async void OnSave()
        {
            PracticeItem newItem = new PracticeItem()
            {
                Name = Name,
                Part = Part,
                Description = Description
            };

            HistoryDatabase database = await HistoryDatabase.Instance;

            await database.SavePracticeItemAsync(newItem);

            //await DataStore.AddItemAsync(newItem);

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

    }
}
