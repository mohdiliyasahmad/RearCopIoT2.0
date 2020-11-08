using SvichEx.DbRepository;
using SvichEx.Services;
using SvichEx.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace SvichEx
{
    public partial class App : Application
    {
        static AppSettingDatabase database;
        static RestService appService;
        Task<List<SettingItem>> obj;

        public const string ApiUrl = "https://rciot.azure-api.net/gateway/";
 
        public static AppSettingDatabase Database
        {
            get
            {
                if (database == null)
                {
                    database = new AppSettingDatabase();
                }
                return database;
            }
        }


        public static Boolean IsInternetAvailable
        {
            get
            {
                var current = Connectivity.NetworkAccess;

                if (current != NetworkAccess.Internet)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        public static RestService AppService
        {
            get
            {
                if (appService == null)
                {
                    appService = new RestService();
                }
                return appService;
            }
        }


        public App()
        {
            InitializeComponent();
            AppCongiguration();
            MainPage = new NavigationPage(new MainPage());
        }

        protected override void OnStart()
        {
            //AppCongiguration();
        }

        protected override void OnSleep()
        {
            // Ensure our stopwatch is reset so the elapsed time is 0.
        }

        protected override void OnResume()
        {
            AppCongiguration();
            // App enters the foreground so start our stopwatch again.
        }

        private void AppCongiguration()
        {
            obj = Database.GetItems();
            Application.Current.Properties["tabs"] = obj;
        }
    }
}
