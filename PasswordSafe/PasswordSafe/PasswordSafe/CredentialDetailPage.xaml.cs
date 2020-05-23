using Newtonsoft.Json;
using PasswordSafe.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace PasswordSafe
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CredentialDetailPage : ContentPage
    {
        private const int CVC_DIGITS = 3;
        private const int CARD_NUM_DIGITS = 16;
        private const string EMAIL_REGEX = @"^(?("")("".+?(?<!\\)""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                    @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-0-9a-z]*[0-9a-z]*\.)+[a-z0-9][\-a-z0-9]{0,22}[a-z0-9]))$";
        private const string PHONE_REGEX = @"[^0-9]+";
        private const int PHONE_DIGITS = 10;
        private const int NUM_OCTETS = 4;
        private const int OCT_MIN = 0;
        private const int OCT_MAX = 255;

        private bool isNewCredential = false;
        private ObservableCollection<Credential> listviewCredentialsSource;
        private Credential originalCredential = new Credential();
        private List<string> passwordHistoryList = new List<string>();

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
                Title = "New Credential";
                // hide edit on tool bar when it is new credential
                toolbarItemEdit.Text = "";
                toolbarItemEdit.IsEnabled = false;
                saveBtn.IsVisible = entryTitle.IsEnabled = entryPassword.IsEnabled = entryExpireDate.IsEnabled = entryNotes.IsEnabled = passOperationsBtn.IsEnabled =  true;
            }
            else
            {
                // hide save button when existing contact is first opened
                saveBtn.IsVisible = false;
            }
        }
        private void cloneCredential(Credential credential)
        {
            ObservableCollection<long> accounts;
            Dictionary<string, string> securityQuestions;
            if (credential is BankCredential bc)
            {
                if (bc.Accounts == null)
                    accounts = new ObservableCollection<long>();
                else
                    accounts = JsonConvert.DeserializeObject<ObservableCollection<long>>(bc.Accounts);
                if (bc.SecurityQuestions == null)
                    securityQuestions = new Dictionary<string, string>();
                else
                    securityQuestions = JsonConvert.DeserializeObject<Dictionary<string, string>>(bc.SecurityQuestions);


                originalCredential = new BankCredential()
                {
                    CardNumber = bc.CardNumber,
                    SecurityCode = bc.SecurityCode,
                    Address = bc.Address,
                    OnlineBankingUrl = bc.OnlineBankingUrl,
                    Accounts = JsonConvert.SerializeObject(accounts),
                    SecurityQuestions = JsonConvert.SerializeObject(securityQuestions)
                };

                // enable relevant entries
                if (credential.ID == 0)
                    entryCardNumber.IsEnabled = entrySecurityCode.IsEnabled = entryAddress.IsEnabled
                        = entryOnlineBankingUrl.IsEnabled = btnAddAcct.IsEnabled = btnAddQst.IsEnabled = true;

                // set source of listviewQuestions and listviewAccounts
                if (bc.Accounts != null)
                    listviewAccounts.ItemsSource = JsonConvert.DeserializeObject<ObservableCollection<long>>(bc.Accounts);
                if (bc.SecurityQuestions != null)
                    listviewQuestions.ItemsSource = JsonConvert.DeserializeObject<Dictionary<string, string>>(bc.SecurityQuestions);

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
            originalCredential.PasswordHistory = credential.PasswordHistory;
            
            if(!string.IsNullOrWhiteSpace(credential.PasswordHistory))
                passwordHistoryList = JsonConvert.DeserializeObject<List<string>>(credential.PasswordHistory);
        }

        async protected override void OnAppearing()
        {
            // paste the random password generated in PasswordGenerator Page to entryPassword
            if (Clipboard.HasText) { 
                string text = await Clipboard.GetTextAsync();
                if (!string.IsNullOrWhiteSpace(text)) {
                    entryPassword.Text = text;
                    await DisplayAlert("Generate Password", "We have pasted the random password to password entry.", "OK");
                    await Clipboard.SetTextAsync(string.Empty);
                }
            }
            base.OnAppearing();
        }

        private void toolbarItemEdit_Clicked(object sender, EventArgs e)
        {
            // enable or diable form entries accordingly
            if (this.Title == "Credential")
            {
                this.Title = "Edit Credential";
                saveBtn.IsVisible = entryTitle.IsEnabled = entryPassword.IsEnabled = entryExpireDate.IsEnabled = entryNotes.IsEnabled = passOperationsBtn.IsEnabled = true;
                switchEnablity(true);
            }
            else
            {
                this.Title = "Credential";
                saveBtn.IsVisible = entryTitle.IsEnabled = entryPassword.IsEnabled = entryExpireDate.IsEnabled = entryNotes.IsEnabled = passOperationsBtn.IsEnabled = false;
                switchEnablity(false);
            }
        }

        private void switchEnablity(bool isEnabledValue) {
            if (originalCredential is BankCredential)
                entryCardNumber.IsEnabled = entrySecurityCode.IsEnabled = entryAddress.IsEnabled = entryOnlineBankingUrl.IsEnabled = btnAddAcct.IsEnabled = btnAddQst.IsEnabled = isEnabledValue;
            else if (originalCredential is SocialMediaCredential)
                entryEmail.IsEnabled = entryPhone.IsEnabled = entryUsername.IsEnabled = entryName.IsEnabled = entryLink.IsEnabled = isEnabledValue;
            else if(originalCredential is WifiCredential)
                entryWifiName.IsEnabled = entryMacAddress.IsEnabled = entryIpAddress.IsEnabled = entrySubnetMask.IsEnabled = entryDefaultGateway.IsEnabled = entryDnsServer.IsEnabled = isEnabledValue;
        }

        async private void saveBtn_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(entryTitle.Text) || string.IsNullOrWhiteSpace(entryPassword.Text) || entryExpireDate.Date == null)
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
                    if (!string.IsNullOrWhiteSpace(smc.Email) && !Regex.IsMatch(smc.Email, EMAIL_REGEX, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)))
                    {
                        await DisplayAlert("Save Error", "Please enter a valid email address.", "OK");
                        return;
                    }

                    // validate if phone number has 10 digits
                    // https://stackoverflow.com/questions/29970244/how-to-validate-a-phone-number
                    // https://stackoverflow.com/a/44283376
                    if (!string.IsNullOrWhiteSpace(smc.PhoneNumber) && (Regex.Replace(smc.PhoneNumber, PHONE_REGEX, "").Length < PHONE_DIGITS || Regex.Replace(smc.PhoneNumber, PHONE_REGEX, "").Length > PHONE_DIGITS + 3)) {
                        await DisplayAlert("Save Error", "Please enter a valid phone number (10 digits).", "OK");
                        return;
                    }
                }
                else if (credential is BankCredential bc) {
                    // prevent the scenario user clicks save button too fast so that IPorpertyChanged isn't updated yet
                    if (entrySecurityCode.Text != bc.SecurityCode.ToString())
                        bc.SecurityCode = 0;
                    if (entryCardNumber.Text != bc.CardNumber.ToString())
                        bc.CardNumber = 0;

                    if (bc.SecurityCode != 0 && (bc.SecurityCode.ToString().Length != CVC_DIGITS)) {
                        await DisplayAlert("Save Error", "Please enter a valid 3-digit security code (credit card verification code).", "OK");
                        return;
                    }
                    if (bc.CardNumber != 0 && (bc.CardNumber.ToString().Length != CARD_NUM_DIGITS)) {
                        await DisplayAlert("Save Error", "Please enter a valid 16-digit card number.", "OK");
                        return;
                    }
                }
                else if (credential is WifiCredential wc) {
                    if (!isIPValid(entryDnsServer.Text) || !isIPValid(entryDefaultGateway.Text) 
                        || !isIPValid(entryIpAddress.Text) || !isIPValid(entrySubnetMask.Text)) {
                        await DisplayAlert("Save Error", "Please enter valid IP address (e.g. 10.39.167.52).", "OK");
                        return;
                    }
                }

                // expiration date entered is not a valid Datetime object
                if (entryExpireDate.Date <= DateTime.Now) {
                    await DisplayAlert("Save Error", "Please enter a valid expire time.", "OK");
                    return;
                }

                // prevent OnDisappearing() from resetting credential to the original credential
                credential.IsChanged = false;

                // update password history with the newest password
                if (!passwordHistoryList.Contains(credential.Password))
                {
                    passwordHistoryList.Add(credential.Password);
                    credential.PasswordHistory = JsonConvert.SerializeObject(passwordHistoryList);
                }

                // save the id of credential for new credential
                int primaryKey = await App.Database.SaveCredentialAsync(credential);

                if (isNewCredential)
                {
                    // make sure new credential displays normally when navigating back to CredentialList Page
                    credential.ID = primaryKey;

                    // Add credential to ui.
                    listviewCredentialsSource.Add(credential);
                }

                // Navigate back to CredentialListPage
                await Navigation.PopAsync();
            }
        }

        private bool isIPValid(string entryText) {
            if (string.IsNullOrWhiteSpace(entryText)) {
                return true;
            }

            // https://stackoverflow.com/questions/5096780/ip-address-validation
            string[] octets = entryText.Split('.');
            if (octets.Length != NUM_OCTETS) {
                return false;
            }
            foreach (string oct in octets) {
                if (!int.TryParse(oct, out int octValue))
                    return false;
                if (octValue < OCT_MIN || octValue > OCT_MAX)
                    return false;
            }
            return true;
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
            Console.WriteLine(Navigation.NavigationStack.Count);
            base.OnDisappearing();
        }

        async private void menuItemDeleteAcct_Clicked(object sender, EventArgs e)
        {
            ObservableCollection<long> accounts;            
            MenuItem m = sender as MenuItem;
            object account = m.CommandParameter as object;
            if (account != null)
            {
                BankCredential credential = this.BindingContext as BankCredential;
                bool answer = await DisplayAlert("Warning", "Are you sure you want like to delete " + account.ToString() + "?", "Yes", "No");
                if (answer)
                {
                    // update listviewAccounts UI
                    accounts = JsonConvert.DeserializeObject<ObservableCollection<long>>(credential.Accounts);
                    accounts.Remove(long.Parse(account.ToString()));
                    credential.Accounts = JsonConvert.SerializeObject(accounts);
                    listviewAccounts.ItemsSource = JsonConvert.DeserializeObject<ObservableCollection<long>>(credential.Accounts);
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
                    // update listviewQuestions UI
                    Dictionary<string, string> securityQuestions = JsonConvert.DeserializeObject<Dictionary<string, string>>(credential.SecurityQuestions);
                    securityQuestions.Remove(pair.Key);
                    credential.SecurityQuestions = JsonConvert.SerializeObject(securityQuestions);
                    listviewQuestions.ItemsSource = JsonConvert.DeserializeObject<Dictionary<string, string>>(credential.SecurityQuestions);
                }
            }
            else {
                await DisplayAlert("Delete Error", "Please enter valid security question and answer.", "OK");
            }
        }
        private void menuItemModifyAcct_Clicked(object sender, EventArgs e)
        {
            MenuItem m = sender as MenuItem;
            object account = m.CommandParameter as object;
            addModifyAccount(false,account.ToString());
        }
        private void btnAddAcct_Clicked(object sender, EventArgs e)
        {
            addModifyAccount();
        }
        private void menuItemModifyQst_Clicked(object sender, EventArgs e)
        {
            MenuItem m = sender as MenuItem;
            object qq = m.CommandParameter as object;            
            addModifyQuestionAnswer(true,qq.ToString());
        }

        private void btnAddQst_Clicked(object sender, EventArgs e)
        {
            addModifyQuestionAnswer();
        }

        async private void addModifyQuestionAnswer(bool modify=false,string oldQuestion="", string question="",string answer="") {
            question = await DisplayPromptAsync("Add Question", "Please enter security question below \n(previous value: "+question+")");
            answer = await DisplayPromptAsync("Add Answer", "Please enter answer to the security question below \n(previous value: " + answer + ")");

            Dictionary<string, string> securityQuestions;

            if ((this.BindingContext as BankCredential).SecurityQuestions != null)
                securityQuestions = JsonConvert.DeserializeObject<Dictionary<string, string>>((this.BindingContext as BankCredential).SecurityQuestions);
            else
                securityQuestions = new Dictionary<string, string>();

            if (!string.IsNullOrWhiteSpace(question) && !string.IsNullOrWhiteSpace(answer))
            {
                if (modify)
                {
                    oldQuestion = oldQuestion.Split(',')[0].Remove(0, 1);
                    securityQuestions.Remove(oldQuestion);
                }
                    

                securityQuestions.Add(question, answer);
                (this.BindingContext as BankCredential).SecurityQuestions = JsonConvert.SerializeObject(securityQuestions);
                listviewQuestions.ItemsSource = JsonConvert.DeserializeObject<Dictionary<string, string>>((this.BindingContext as BankCredential).SecurityQuestions);
            }
            else
            {
                await DisplayAlert("Add Error", "Please enter security question and answer.", "OK");
            }
        }

        async private void addModifyAccount(bool isAdd=true, string result="") {
            BankCredential bc = this.BindingContext as BankCredential;
            int index = -1;
            ObservableCollection<long> accounts;
            if (bc.Accounts != null)
                accounts = JsonConvert.DeserializeObject<ObservableCollection<long>>(bc.Accounts);
            else
                accounts = new ObservableCollection<long>();
            if (!isAdd) {                
                index = accounts.IndexOf(long.Parse(result));
            }

            result = await DisplayPromptAsync("Add Account", "Please enter new account number below (previous value: "+result+")", keyboard: Keyboard.Numeric);
            if (long.TryParse(result, out long newAccount))
            {
                if (isAdd)
                    accounts.Add(newAccount);
                else
                    accounts[index] = newAccount;
                bc.Accounts = JsonConvert.SerializeObject(accounts);
                listviewAccounts.ItemsSource = JsonConvert.DeserializeObject<ObservableCollection<long>>(bc.Accounts);
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
            string result = await DisplayActionSheet("Choose a password operation from the following:", "Cancel", null, "Copy password to clipboard", "View most recent password", "Generate random password");
            if (!string.IsNullOrWhiteSpace(result) && result.ToLower() != "cancel")
            {
                switch (result)
                {
                    case "View most recent password":
                        string prePassword = "N/A";
                        if (passwordHistoryList.Count > 1) {
                            prePassword = passwordHistoryList[passwordHistoryList.Count-2];
                        }
                        await DisplayAlert("Password Recovery", "The most recent password in history for this credential is: " + prePassword, "OK");
                        break;
                    case "Generate random password":
                        await Navigation.PushAsync(new PasswordGeneratorPage());
                        break;
                    case "Copy password to clipboard":
                        copyToClipBoard(entryPassword.Text);
                        break;
                    default:
                        await DisplayAlert("Password Operation Error", "Invalid password operation", "ok");
                        break;
                }
            }
        }

        private void menuItemCopyAcct_Clicked(object sender, EventArgs e)
        {
            copyToClipBoard((sender as MenuItem).CommandParameter as string,"Account Number");
        }

        private void entryCardNumber_Tapped(object sender, EventArgs e)
        {
            copyToClipBoard(entryCardNumber.Text,"Card Number");
        }

        async private void copyToClipBoard(string text,string entryType="Password") {
            if (!string.IsNullOrWhiteSpace(text)) {
                await Clipboard.SetTextAsync(text);
                await DisplayAlert("Clipboard", string.Format("{0} {1} is copied to clipboard.",entryType,text), "OK");
            }
        }
    }
}