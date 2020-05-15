using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PasswordSafe
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StartPage : ContentPage
    {
        public StartPage()
        {
            InitializeComponent();
        }

        private void access_Clicked(object sender, EventArgs e)
        {            
            if (!App.PassOperations.CheckPass())
            {
                DisplayAlert("Error", "No combination has been defined yet, click the button below to set your Password Safe combination", "ok");
                return;
            }
            string safeCombination = combination.Text;
            if (App.PassOperations.VerifyPass(safeCombination))
            {
                //navigate to password list page
            }
        }

        private void change_Clicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new CombinationPage());
        }
    }
}