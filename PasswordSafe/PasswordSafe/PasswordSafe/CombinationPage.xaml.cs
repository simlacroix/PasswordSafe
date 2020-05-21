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
    public partial class CombinationPage : ContentPage
    {
        private bool oldRemoved = false;

        public CombinationPage()
        {
            InitializeComponent();
            if (!App.PassOperations.CheckPass())
            {
                Table.Root.Remove(oldSection);
                oldRemoved = true;
            }
            else
            {
                oldRemoved = false;
            }
                
            
        }

        private async void submit_Clicked(object sender, EventArgs e)
        {
            if (!oldRemoved)
            {
                if (!App.PassOperations.VerifyPass(old.Text))
                {
                    await DisplayAlert("Error", "The old combination was incoreclty entered, please try again", "ok");
                    old.Text = string.Empty;
                    theNew.Text = string.Empty;
                    confirm.Text = string.Empty;
                    return;
                }                 
            }
            
            if (theNew.Text == confirm.Text && !String.IsNullOrWhiteSpace(theNew.Text))
            {                
                if (App.PassOperations.ChangePass(theNew.Text))
                {
                    await DisplayAlert("Confirmed", "Combination was successfully changed", "ok");
                    await Navigation.PopAsync();
                }
                    
                else
                {
                    await DisplayAlert("Error", "Combination is not complex enough. Must be at least 6 char long, have one uppercase, have one lowercase one number and one symbol", "ok");
                    theNew.Text = string.Empty;
                    confirm.Text = string.Empty;
                }
                    
            }
            else
            {
                await DisplayAlert("Error", "Combination must be the same in both fields and not empty", "ok");
                theNew.Text = string.Empty;
                confirm.Text = string.Empty;
            }                
        }
    }
}