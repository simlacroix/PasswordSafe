using PasswordSafe.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;

namespace PasswordSafe
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class PasswordGeneratorPage : ContentPage
    {
        private PasswordPolicy passwordPolicy;

        public PasswordGeneratorPage()
        {
            InitializeComponent();
            passwordPolicy = new PasswordPolicy();
            lower.Text = passwordPolicy.numLowerCase.ToString();
            upper.Text = passwordPolicy.numUpperCase.ToString();
            digits.Text = passwordPolicy.numDigits.ToString();
            symbols.Text = passwordPolicy.numSymbols.ToString();
        }

        async private void generate_Clicked(object sender, EventArgs e)
        {
            passwordPolicy.numLowerCase = int.Parse(lower.Text);
            passwordPolicy.numUpperCase = int.Parse(upper.Text);
            passwordPolicy.numDigits = int.Parse(digits.Text);
            passwordPolicy.numSymbols = int.Parse(symbols.Text);
            string pass = passwordPolicy.GenerateRandomPassword();
            generatedPass.Text = pass;
            await Clipboard.SetTextAsync(pass);
        }
    }
}