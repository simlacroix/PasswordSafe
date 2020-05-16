using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using PasswordSafe.Models;
using System.Collections.ObjectModel;

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
        }
    }
}