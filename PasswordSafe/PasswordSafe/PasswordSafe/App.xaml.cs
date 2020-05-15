using PasswordSafe.DataHelper;
using System;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace PasswordSafe
{
    public partial class App : Application
    {

        private static LocalDB database;
        private static IOPassOperations passOperations;

        public static LocalDB Database
        {
            get
            {
                if (database == null)
                    database = new LocalDB(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "User.db3"));

                return database;
            }
        }

        public static IOPassOperations PassOperations
        {
            get
            {
                if (passOperations == null)
                    passOperations = new IOPassOperations(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "pass.txt"));

                return passOperations;
            }
        }

        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new StartPage());
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
