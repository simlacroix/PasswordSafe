using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PasswordSafe.Models;
using System.Collections.ObjectModel;
using Xamarin.Essentials;

namespace PasswordSafe
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CredentialListPage : ContentPage
    {
        private ObservableCollection<Credential> _credentials;
        private string sortKeyword;
        public CredentialListPage()
        {
            InitializeComponent();

            // Both the method and assigment will possibly populate the listViewContacts
            populateListviewCredentials();
            sortPicker.SelectedItem = sortKeyword = "Default";

            toolbarItemAdd.IconImageSource = ImageSource.FromResource("PasswordSafe.Images.AddIcon.png");
        }

        async private void populateListviewCredentials()
        {
            _credentials = new ObservableCollection<Credential>(await App.Database.GetAllCredentialsAsync());
            
            ObservableCollection<Credential> bank = new ObservableCollection<Credential>(await App.Database.GetAllBankCredentialsAsync());
            ObservableCollection<Credential> wifi = new ObservableCollection<Credential>(await App.Database.GetAllWifiCredentialsAsync());
            ObservableCollection<Credential> social = new ObservableCollection<Credential>(await App.Database.GetAllSocialMediaCredentialsAsync());

            foreach (Credential c in bank)
                _credentials.Add(c);
            foreach (Credential c in wifi)
                _credentials.Add(c);
            foreach (Credential c in social)
                _credentials.Add(c);

            // populate the listViewCredentials here too, double insurance if sortPicker_SelectedIndexChanged
            // happens before GetAllCredentialsAsync() gets back the contacts from database
            listViewCredentials.ItemsSource = _credentials;
        }

        protected override void OnAppearing()
        {
            // sort list each time after navigation
            sortList();
            base.OnAppearing();
        }

        async private void toolbarItemAdd_Clicked(object sender, EventArgs e)
        {
            Credential newCredential = newCredential = new Credential();
            // create new credential
            string result = await DisplayActionSheet("Choose a credential type from the following:", "Cancel", null, Credential.credentialTypes);
            if (!string.IsNullOrWhiteSpace(result) && result.ToLower() != "cancel")
            {
                switch (result)
                {
                    case "Default":
                        break;
                    case "Social Media":
                        newCredential = new SocialMediaCredential();
                        break;
                    case "Banking":
                        newCredential = new BankCredential();
                        break;
                    case "Wifi":
                        newCredential = new WifiCredential();
                        break;
                    default:
                        await DisplayAlert("Error", "Invalid credential type", "ok");
                        break;
                }
                await Clipboard.SetTextAsync(string.Empty);
                await Navigation.PushAsync(new CredentialDetailPage(newCredential, _credentials));
            }
        }

        private void searchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
            string filter = e.NewTextValue.ToLower();
            if (string.IsNullOrWhiteSpace(filter))
            {
                listViewCredentials.ItemsSource = _credentials;
            }
            else
            {
                listViewCredentials.ItemsSource = _credentials.Where(c => c.CredentialTitle.ToLower().Contains(filter) || c.Notes.ToLower().Contains(filter) ||
                    c.ExpirationDate.ToString("F").ToLower().Contains(filter));
            }
        }

        private void sortPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            sortKeyword = (sender as Picker).SelectedItem.ToString();
            sortList();
        }
        private void sortList()
        {
            // prevent sortPicker.SelectedItem = sortKeyword = "Default"; & sortList() call in OnAppearing from 
            // sorting _credentials before the _credentials gets populated from database
            if (listViewCredentials.ItemsSource != null && _credentials != null)
            {
                switch (sortKeyword)
                {
                    case "Credential Title ↓":
                        _credentials = new ObservableCollection<Credential>(_credentials.OrderBy(n => n.CredentialTitle));
                        break;
                    case "Expiration Date ↑":
                        _credentials = new ObservableCollection<Credential>(_credentials.OrderBy(n => n.ExpirationDate));
                        break;
                    case "Default":
                        searchBar.Placeholder = "Search...";
                        searchBar.IsEnabled = true;
                        break;
                }

                listViewCredentials.ItemsSource = _credentials;

                // disable search bar when sorting
                if (!string.IsNullOrWhiteSpace(sortKeyword) && sortKeyword != "Default")
                {
                    searchBar.Text = string.Empty;
                    searchBar.Placeholder = "Change to default sort to search";
                    searchBar.IsEnabled = false;
                }
            }
        }

        async private void listViewCredentials_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            Credential selectedCredential = e.Item as Credential;
            // edit existing credential
            await Clipboard.SetTextAsync(string.Empty);
            await Navigation.PushAsync(new CredentialDetailPage(selectedCredential, _credentials));
        }

        async private void menuItemDelete_Clicked(object sender, EventArgs e)
        {
            Credential credential = (sender as MenuItem).CommandParameter as Credential;
            bool answer = await DisplayAlert("Warning", "Are you sure you want like to delete " + credential.CredentialTitle + "?", "Yes", "No");
            if (answer)
            {
                await App.Database.DeleteCredentialAsync(credential);
                // update ListviewCredentials UI
                _credentials.Remove(credential);
            }
        }
    }
}