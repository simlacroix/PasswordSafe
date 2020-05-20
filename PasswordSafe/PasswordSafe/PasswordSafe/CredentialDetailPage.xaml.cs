using PasswordSafe.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PasswordSafe
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CredentialDetailPage : ContentPage
    {
        private bool isNewCredential = false;
        private ObservableCollection<Credential> listviewCredentialsSource;
        private Credential originalCredential = new Credential();

        public CredentialDetailPage(Credential credential, ObservableCollection<Credential> credentials)
        {
            InitializeComponent();
            this.BindingContext = credential;

            // clone original credential so that when user presses physical/toolbar back button
            // wihout pressing SAVE button first, the changes made in CredentialDetail Page won't be
            // in effect in CredentialList Page due to BindingContext and PropertyChanged in Credential class
            cloneCredential(credential);
            listviewCredentialsSource = credentials;

            if (credential.ID == 0)
            {
                isNewCredential = true;
                Title = "New Contact";
                // hide edit on tool bar when it is new credential
                toolbarItemEdit.Text = "";
                toolbarItemEdit.IsEnabled = false;
                saveBtn.IsVisible = entryTitle.IsEnabled = entryPassword.IsEnabled = entryExpireDate.IsEnabled = entryNotes.IsEnabled =  true;
            }
            else
            {
                // hide save button when existing contact is first opened
                saveBtn.IsVisible = false;
            }
        }
        private void cloneCredential(Credential credential)
        {
            if (credential is BankCredential bc)
            {
                if (bc.Accounts == null)
                    bc.Accounts = new ObservableCollection<long>();
                if (bc.SecurityQuestions == null)
                    bc.SecurityQuestions = new Dictionary<string, string>();

                originalCredential = new BankCredential()
                {
                    CardNumber = bc.CardNumber,
                    SecuirityCode = bc.SecuirityCode,
                    Address = bc.Address,
                    OnlineBankingUrl = bc.OnlineBankingUrl,
                    Accounts = new ObservableCollection<long>(bc.Accounts),
                    SecurityQuestions = new Dictionary<string, string>(bc.SecurityQuestions)
                };

                // enable relevant entries
                if (credential.ID == 0)
                    entryCardNumber.IsEnabled = entrySecuirityCode.IsEnabled = entryAddress.IsEnabled
                        = entryOnlineBankingUrl.IsEnabled = btnAddAcct.IsEnabled = btnAddQst.IsEnabled = true;

                // set source of listviewQuestions and listviewAccounts
                listviewAccounts.ItemsSource = bc.Accounts;
                listviewQuestions.ItemsSource = bc.SecurityQuestions;

                // hide irrelevant table sections
                // https://stackoverflow.com/questions/37093342/how-to-hide-or-remove-particular-tablesectionxamarin-forms-which-created-by-xa
                tableRoot.Remove(socialMediaTbSection);
                tableRoot.Remove(wifiTbSection);
            }
            else if (credential is SocialMediaCredential smc)
            {
                originalCredential = new SocialMediaCredential()
                {
                    Email = smc.Email,
                    PhoneNumber = smc.PhoneNumber,
                    Username = smc.Username,
                    Name = smc.Name,
                    Link = smc.Link
                };

                if (credential.ID == 0)
                    entryEmail.IsEnabled = entryPhone.IsEnabled = entryUsername.IsEnabled = entryName.IsEnabled = entryLink.IsEnabled = true;

                tableRoot.Remove(bankingTbSection);
                tableRoot.Remove(wifiTbSection);
            }
            else if (credential is WifiCredential wc)
            {
                originalCredential = new WifiCredential()
                {
                    WifiName = wc.WifiName,
                    MacAddress = wc.MacAddress,
                    IpAddress = wc.IpAddress,
                    SubnetMask = wc.SubnetMask,
                    DefaultGateway = wc.DefaultGateway,
                    DnsServer = wc.DnsServer
                };
                if (credential.ID == 0)
                    entryWifiName.IsEnabled = entryMacAddress.IsEnabled = entryIpAddress.IsEnabled =
                        entrySubnetMask.IsEnabled = entryDefaultGateway.IsEnabled = entryDnsServer.IsEnabled = true;

                tableRoot.Remove(socialMediaTbSection);
                tableRoot.Remove(bankingTbSection);
            }
            else {
                // hide irrelevant table sections when it is default credential
                tableRoot.Remove(socialMediaTbSection);
                tableRoot.Remove(wifiTbSection);
                tableRoot.Remove(bankingTbSection);
            }

            originalCredential.CredentialTitle = credential.CredentialTitle;
            originalCredential.Password = credential.Password;
            originalCredential.ExpirationDate = credential.ExpirationDate;
            originalCredential.Notes = credential.Notes;
            originalCredential.ID = credential.ID;
            originalCredential.IsChanged = false;
        }

        private void toolbarItemEdit_Clicked(object sender, EventArgs e)
        {
            // enable or diable form entries accordingly
            if (this.Title == "Contact")
            {
                this.Title = "Edit Contact";
                saveBtn.IsVisible = entryTitle.IsEnabled = entryPassword.IsEnabled = entryExpireDate.IsEnabled = entryNotes.IsEnabled = true;
                switchEnablity(true);
            }
            else
            {
                this.Title = "Contact";
                saveBtn.IsVisible = entryTitle.IsEnabled = entryPassword.IsEnabled = entryExpireDate.IsEnabled = entryNotes.IsEnabled = false;
                switchEnablity(false);
            }
        }

        private void switchEnablity(bool isEnabledValue) {
            if (originalCredential is BankCredential)
                entryCardNumber.IsEnabled = entrySecuirityCode.IsEnabled = entryAddress.IsEnabled = entryOnlineBankingUrl.IsEnabled = btnAddAcct.IsEnabled = btnAddQst.IsEnabled = isEnabledValue;
            else if (originalCredential is SocialMediaCredential)
                entryEmail.IsEnabled = entryPhone.IsEnabled = entryUsername.IsEnabled = entryName.IsEnabled = entryLink.IsEnabled = isEnabledValue;
            else if(originalCredential is WifiCredential)
                entryWifiName.IsEnabled = entryMacAddress.IsEnabled = entryIpAddress.IsEnabled = entrySubnetMask.IsEnabled = entryDefaultGateway.IsEnabled = entryDnsServer.IsEnabled = isEnabledValue;
        }

        async private void saveBtn_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(entryTitle.Text) || string.IsNullOrWhiteSpace(entryPassword.Text) || entryExpireDate.Date != null)
            {
                await DisplayAlert("Save Error", "Please enter a title, a password and an expiration date to save credential.", "OK");
            }
            else
            {
                // Add to or update in database
                Credential credential = this.BindingContext as Credential;
                if (credential is SocialMediaCredential smc)
                {
                    // Check if email is in valid format
                    // Source: https://docs.microsoft.com/en-us/dotnet/standard/base-types/how-to-verify-that-strings-are-in-valid-email-format
                    if (!Regex.IsMatch(smc.Email,
                    @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
                    {
                        await DisplayAlert("Save Error", "Please enter a title, a password and an expiration date to save credential.", "OK");
                        return;
                    }
                }

                /*DateTime expiration;
                // expiration date entered is not a valid Datetime object
                if (!DateTime.TryParse(entryExpireDate.Text, out expiration)) {
                    await DisplayAlert("Save Error", "Please enter a valid expire time.", "OK");
                    return;
                }
                credential.ExpirationDate = expiration;*/

                // prevent OnDisappearing() from resetting credential to the original credential
                credential.IsChanged = false;

                // save the id of credential for new credential
                int primaryKey = await App.Database.SaveCredentialAsync(credential);

                if (isNewCredential)
                {
                    // make sure new contact displays normally when navigating back to CredentialList Page
                    credential.ID = primaryKey;

                    // Add contact to ui.
                    listviewCredentialsSource.Add(credential);
                }

                // update password history with the newest password
                if (!credential.GetPasswordHistory().Contains(credential.Password))
                    credential.GetPasswordHistory().Add(credential.Password);

                // Navigate back to CredentialListPage
                await Navigation.PopAsync();

            }
        }

        protected override bool OnBackButtonPressed()
        {
            Navigation.PopAsync();
            return false;
        }

        protected override void OnDisappearing()
        {
            // When physical/toolbar back button is clicked,
            // Do not save credential if SAVE button is not already clicked
            // Revert the existing credential to its original state before edit
            Credential credential = this.BindingContext as Credential;
            if (!isNewCredential && credential.IsChanged)
            {
                int index = listviewCredentialsSource.IndexOf(credential);
                listviewCredentialsSource[index] = originalCredential;
            }
            base.OnDisappearing();
        }

        async private void menuItemDeleteAcct_Clicked(object sender, EventArgs e)
        {
            long account = 0;
            if (long.TryParse((sender as MenuItem).CommandParameter as string, out account))
            {
                BankCredential credential = this.BindingContext as BankCredential;
                bool answer = await DisplayAlert("Warning", "Are you sure you want like to delete " + account + "?", "Yes", "No");
                if (answer)
                {
                    // update listviewAccounts UI
                    credential.Accounts.Remove(account);
                }
            }
            else {
                await DisplayAlert("Delete Error", "Please enter a valid account number.", "OK");
            }
        }

        async private void menuItemDeleteQst_Clicked(object sender, EventArgs e)
        {
            KeyValuePair<string, string> pair = new KeyValuePair<string, string>();
            if ((sender as MenuItem).CommandParameter is KeyValuePair<string, string>)
            {
                pair = (KeyValuePair<string, string>)(sender as MenuItem).CommandParameter;

                BankCredential credential = this.BindingContext as BankCredential;
                bool answer = await DisplayAlert("Warning", "Are you sure you want like to delete \n'Q: " + pair.Key + " A: " + pair.Value + "'?", "Yes", "No");
                if (answer)
                {
                    // update listviewAccounts UI
                    credential.SecurityQuestions.Remove(pair.Key);
                    //todo!! Do we need to update the ui if implement IPropertyChange on SecurityQuestions in Credential class
                }
            }
            else {
                await DisplayAlert("Delete Error", "Please enter valid security question and answer.", "OK");
            }
        }
        private void menuItemModifyAcct_Clicked(object sender, EventArgs e)
        {
            addModifyAccount(false);
        }
        private void btnAddAcct_Clicked(object sender, EventArgs e)
        {
            addModifyAccount();
        }
        private void menuItemModifyQst_Clicked(object sender, EventArgs e)
        {
            addModifyQuestionAnswer();
        }

        private void btnAddQst_Clicked(object sender, EventArgs e)
        {
            addModifyQuestionAnswer();
        }

        async private void addModifyQuestionAnswer(string question="",string answer="") {
            question = await DisplayPromptAsync("Add Question", "Please enter security question below \n(previous value: "+question+")");
            answer = await DisplayPromptAsync("Add Answer", "Please enter answer to the security question below \n(previous value: " + answer + ")");
            if (!string.IsNullOrWhiteSpace(question) && !string.IsNullOrWhiteSpace(answer))
            {
                (this.BindingContext as BankCredential).SecurityQuestions.Add(question, answer);
            }
            else
            {
                await DisplayAlert("Add Error", "Please enter security question and answer.", "OK");
            }
        }

        async private void addModifyAccount(bool isAdd=true, string result="") {
            BankCredential bc = this.BindingContext as BankCredential;
            int index = -1;
            if (!isAdd) {
                index = bc.Accounts.IndexOf(long.Parse(result));
            }

            result = await DisplayPromptAsync("Add Account", "Please enter new account number below (previous value: "+result+")", keyboard: Keyboard.Numeric);
            if (long.TryParse(result, out long newAccount))
            {
                if (isAdd)
                    bc.Accounts.Add(newAccount);
                else
                    bc.Accounts[index] = newAccount;          
            }
            else
            {
                await DisplayAlert("Add Error", "Please enter valid account number.", "OK");
            }
        }

        private void showHidePasswordSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (showHidePasswordSwitch.IsToggled) {
                entryPassword.IsPassword = false;
            }
            else {
                entryPassword.IsPassword = true;
            }
        }

        async private void passOperationsBtn_Clicked(object sender, EventArgs e)
        {
            string result = await DisplayActionSheet("Choose a password operation from the following:", "Cancel", null, "View most recent password", "Generate random password");
            if (!string.IsNullOrWhiteSpace(result))
            {
                switch (result)
                {
                    case "View most recent password":
                        await DisplayAlert("Password Recovery", "The most recent password in history for this credential is: " + (this.BindingContext as Credential).GetMostRecentPassword(), "OK");
                        break;
                    case "Generate random password":
                        await Navigation.PushAsync(new PasswordGeneratorPage());
                        break;
                    default:
                        await DisplayAlert("Password Operation Error", "Invalid password operation", "ok");
                        break;
                }
            }
            else
            {
                await DisplayAlert("Password Operation Error", "Invalid password operation", "ok");
            }
        }
    }
}