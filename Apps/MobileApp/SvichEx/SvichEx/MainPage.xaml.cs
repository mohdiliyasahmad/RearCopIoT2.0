using Newtonsoft.Json;
using SvichEx.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;

namespace SvichEx
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        PropertyInfo[] props;

        public string DeviceCode { get; set; }
        public bool IsDeviceEnabled { get; set; }

        public bool IsDefault { get; set; }

        public Task<List<SettingItem>> Tabs { get; set; }

        public Task<List<SettingItemDetail>> Swiches;

        public List<AppControl> AppControls;

        public Task<string> ObjSwitches { get; set; }

        public ReturnModel OnSwitches { get; set; }


        string lbl = "txtSwitch", tgl = "swhSwitch";
        int ctr = 1;
        Label ctlLbl;
        Switch ctlSwitch;
        Button ctlBtn;
        string btnid = "btnTab";
        int i = 1;
        bool isTgl;
        object o;

        string pin = string.Empty;
        int value;

        private bool _isRunning;



        public MainPage()
        {
            OnSwitches = new ReturnModel();
            InitializeComponent();

            var existingPages = Navigation.NavigationStack.ToList();
            foreach (var page in existingPages)
            {
                if (page.GetType().Name != "MainPage")
                {
                    Navigation.RemovePage(page);
                }
            }


        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ResetElements();
            try
            {
                if (App.IsInternetAvailable)
                {
                    PopulateTabs();
                }
                else
                {
                    lblError.Text = "No internet dectected";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }

        }

        private void PopulateSwitchesByDeviceCode()
        {
            Swiches = App.Database.GetItemDetailAsync(DeviceCode);
            PopulateElements(Swiches);
        }


        private void PopulateElements(Task<List<SettingItemDetail>> swiches)
        {

            for (ctr = 1; ctr < 9; ctr++)
            {
                ctlLbl = stkButtons.FindByName<Label>(lbl + ctr.ToString());
                ctlSwitch = stkButtons.FindByName<Switch>(tgl + ctr.ToString());
                ctlLbl.IsVisible = false;
                ctlSwitch.IsVisible = false;
            }

            ctr = 1;

            foreach (var item in swiches.Result)
            {
                ctlLbl = stkButtons.FindByName<Label>(lbl + ctr.ToString());
                ctlSwitch = stkButtons.FindByName<Switch>(tgl + ctr.ToString());
                PopulateDetails(item, ctlLbl, ctlSwitch, lbl + ctr.ToString(), tgl + ctr.ToString());
                ctr++;
            }

        }

        private void PopulateDetails(SettingItemDetail pitem, Label txt, Switch swch, string edtName, string tglName)
        {
            AppControls = App.Database.GetAppControlAsync(pitem);

            foreach (var item in AppControls)
            {
                if (item != null && item.ControlName == edtName)
                {
                    txt.Text = item.Value;
                    txt.IsVisible = item.IsVisible;
                    swch.IsVisible = item.IsVisible;
                }

            }

        }


        private void PopulateTabs()
        {
            try
            {

                Tabs = (Task<List<SettingItem>>)Application.Current.Properties["tabs"];
                i = 1;

                foreach (var item in Tabs.Result)
                {
                    ctlBtn = grdControls.FindByName<Button>(btnid + i.ToString());

                    if (item != null)
                    {
                        ctlBtn.ClassId = item.DeviceCode == null ? "" : item.DeviceCode;
                        ctlBtn.Text = item.NickName == null ? "" : item.NickName;
                        ctlBtn.IsVisible = item.IsVisible;
                        if (!item.IsVisible)
                        {
                            grdControls.ColumnDefinitions[i - 1].Width = 0;
                        }
                    }

                    i++;
                }

                ctlBtn = grdControls.FindByName<Button>(btnid + "1");

                Application.Current.Properties["btn"] = ctlBtn;

                SetTab(ctlBtn,true);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Setting_Pressed(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SettingPage());
        }

        private void btnTab_Pressed(object sender, EventArgs e)
        {
            lblError.Text = "";
            try
            {
                if (App.IsInternetAvailable)
                {
                    Application.Current.Properties["btn"] = ((Button)sender);

                    DeviceCode = ((Button)sender).ClassId;
                    IsDeviceEnabled = false;
                    SetTab((Button)sender,false);
                }
                else
                {
                    lblError.Text = "No internet dectected";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }


        private void ResetElements()
        {
            btnTab1.BackgroundColor = Color.SlateGray;
            btnTab1.TextColor = Color.White;

            btnTab2.BackgroundColor = Color.SlateGray;
            btnTab2.TextColor = Color.White;

            btnTab3.BackgroundColor = Color.SlateGray;
            btnTab3.TextColor = Color.White;

            btnTab4.BackgroundColor = Color.SlateGray;
            btnTab4.TextColor = Color.White;

            btnTab5.BackgroundColor = Color.SlateGray;
            btnTab5.TextColor = Color.White;

            btnTab6.BackgroundColor = Color.SlateGray;
            btnTab6.TextColor = Color.White;

            lblError.Text = "";
            lblStatus.IsVisible = true;
            
            lblStatus.TextColor = Color.GreenYellow;
            lblStatus.HorizontalOptions = LayoutOptions.Start;
            lblStatus.Text = "Device Online";

        }

        private void SetTab(Button ctlBtn, bool isOnline=false)
        {
            try
            {
                ResetElements();
                ctlBtn.BackgroundColor = Color.Red;

                if (Tabs.Result.Count > 0 )
                {
                    lblStatus.IsVisible = true;
                    lblStatus.TextColor = Color.Red;
                    lblStatus.HorizontalOptions = LayoutOptions.Start;
                    lblStatus.Text = "Checking device oneline status, please wait...";
                }

                DeviceCode = ctlBtn.ClassId;
                PopulateSwitchesByDeviceCode();
                RunTimer();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }

        }


        private void SetToggleButtonOnlineStatus()
        {
            if (!string.IsNullOrEmpty(DeviceCode))
            {
                ObjSwitches = App.AppService.GetSwitches(DeviceCode);

                if (ObjSwitches.Result != null)
                {
                    OnSwitches = JsonConvert.DeserializeObject<ReturnModel>(ObjSwitches.Result);
                }

                IsDefault = true;

                props = OnSwitches.GetType().GetProperties();

                for (ctr = 1; ctr < 9; ctr++)
                {

                    ctlSwitch = grdControls.FindByName<Switch>(tgl + ctr.ToString());
                    isTgl = false;

                    if (ctlSwitch.IsVisible)
                    {

                        o = (string)props.Single(p => p.Name.ToUpper() == ctlSwitch.ClassId.ToUpper()).GetValue(OnSwitches);

                        if (o != null)
                        {
                            if (Convert.ToInt32(o.ToString()) == 1)
                            {
                                isTgl = true;
                            }
                            ctlSwitch.IsToggled = isTgl;
                        }
                    }
                }
                IsDefault = false;
                _isRunning = false;
            }

        }

        private async Task SetDeviceOnlineStatusAsync(Button ctlBtn)
        {
            Tabs = (Task<List<SettingItem>>)Application.Current.Properties["tabs"];
       
            ctlBtn.BackgroundColor = Color.Red;

            try
            {

                if (!string.IsNullOrEmpty(DeviceCode))
                {
                    IsDeviceEnabled = await App.AppService.GetDeviceStatusAsync(DeviceCode);// ConfigureAwait(false);
                }

                if (IsDeviceEnabled)
                {
                    ctlBtn.BackgroundColor = Color.Green;
                    lblStatus.TextColor = Color.GreenYellow;
                    lblStatus.HorizontalOptions = LayoutOptions.Start;
                    lblStatus.Text = "Device Online";
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }


        private void swhSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            ctlSwitch = (Switch)sender;
            pin = ctlSwitch.ClassId;
            value = ctlSwitch.IsToggled ? 1 : 0;
            Task<bool> returnValue;
            lblError.Text = "";

            try
            {
                if (App.IsInternetAvailable)
                {
                    if (!IsDefault)
                    {
                        returnValue = App.AppService.SwitchOnOff(pin, value, DeviceCode);
                    }
                }
                else
                {
                    lblError.Text = "No internet dectected";
                }

            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                ctlSwitch.IsEnabled = true;
            }
        }


        public void RunTimer()
        {
            _isRunning = true;
            Device.StartTimer(new TimeSpan(0, 0, 0, 0, 500), TimerCallBack);
        }

       
        private bool TimerCallBack()
        {
            try
            {
                _ = SetDeviceOnlineStatusAsync((Button)Application.Current.Properties["btn"]);
                SetToggleButtonOnlineStatus();
                return _isRunning;
            }
            catch (Exception)
            {
                _isRunning = false;
                throw;
            }
      
            
        }

    }
}