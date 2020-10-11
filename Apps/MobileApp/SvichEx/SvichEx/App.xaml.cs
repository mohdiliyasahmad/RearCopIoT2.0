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

namespace SvichEx
{
    public partial class App : Application
    {
        static AppSettingDatabase database;
        static RestService appService;
      
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
            MainPage = new NavigationPage(new MainPage());

        }

        protected override void OnStart()
        {
            var obj = Database.GetItems();
            Application.Current.Properties["tabs"] = obj;
        }

        protected override void OnSleep()
        {
            // Ensure our stopwatch is reset so the elapsed time is 0.
        }

        protected override void OnResume()
        {
            var obj = Database.GetItems();
            Application.Current.Properties["tabs"] = obj;
            // App enters the foreground so start our stopwatch again.
        }

    }
}
