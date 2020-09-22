using SvichEx.DbRepository;
using SvichEx.Services;
using SvichEx.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace SvichEx
{
    public partial class App : Application
    {
        static AppSettingDatabase database;
        public const string ApiUrl = "https://rciot.azure-api.net/gateway/";
        public RestService ApiService { get; set; }


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

        public App()
        {
            InitializeComponent();
            MainPage = new NavigationPage(new MainPage());

        }

        protected override void OnStart()
        {
            var obj = Database.GetItems();
            Application.Current.Properties["tabs"] = obj;

            //var obj2 = Database.GetItems().Result;
            //var obj1 = Database.GetAllItems().Result;


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
