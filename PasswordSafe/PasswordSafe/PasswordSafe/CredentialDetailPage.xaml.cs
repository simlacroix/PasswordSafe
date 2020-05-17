using PasswordSafe.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PasswordSafe
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CredentialDetailPage : ContentPage
    {
        public CredentialDetailPage(Credential credential, ObservableCollection<Credential> credentials)
        {
            InitializeComponent();
        }

        private void toolbarItemEdit_Clicked(object sender, EventArgs e)
        {

        }

        private void saveBtn_Clicked(object sender, EventArgs e)
        {

        }

        private void passwordPolicyBtn_Clicked(object sender, EventArgs e)
        {

        }

        private void menuItemModifyAcct_Clicked(object sender, EventArgs e)
        {

        }

        private void menuItemDeleteAcct_Clicked(object sender, EventArgs e)
        {

        }

        private void menuItemDeleteQst_Clicked(object sender, EventArgs e)
        {

        }

        private void menuItemModifyQst_Clicked(object sender, EventArgs e)
        {

        }
    }
}