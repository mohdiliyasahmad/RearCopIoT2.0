using Newtonsoft.Json;
using SvichEx.DbRepository;
using SvichEx.Services;
using SvichEx.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Reflection;

namespace SvichEx
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {

        public string DeviceCode { get; set; }
        public bool IsDeviceEnabled { get; set; }

        public Task<List<SettingItem>> Tabs { get; set; }


        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
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
            Task<List<SettingItemDetail>> swiches;
            swiches = App.Database.GetItemDetailAsync(DeviceCode);
            PopulateElements(swiches);
        }


        private void PopulateElements(Task<List<SettingItemDetail>> swiches)
        {
            string lbl = "txtSwitch", tgl = "swhSwitch";
            int ctr = 1;
            Label ctlLbl;
            Switch ctlSwitch;

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
            var obj = App.Database.GetAppControlAsync(pitem);

            foreach (var item in obj)
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

                Button ctlBtn;
                string btnid = "btnTab";

                int i = 1;

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
                SetTab(ctlBtn);
            }
            catch (Exception)
            {

                throw;
            }
        }

        private void Button_Pressed(object sender, EventArgs e)
        {
            Navigation.PopAsync();
            Navigation.PushAsync(new SettingPage());

        }

        private void btnTab_Pressed(object sender, EventArgs e)
        {
            lblError.Text = "";
            try
            {
                if (App.IsInternetAvailable)
                {
                    DeviceCode = ((Button)sender).ClassId;
                    IsDeviceEnabled = false;
                    SetTab((Button)sender);
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
        }

        private void SetTab(Button ctlBtn)
        {
            try
            {
                ResetElements();
                ctlBtn.BackgroundColor = Color.Green;
                DeviceCode = ctlBtn.ClassId;
                _ = SetDeviceOnlineStatusAsync(ctlBtn);
                PopulateSwitchesByDeviceCode();
                _ = SetToggleButtonOnlineStatus();

            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }

        }

        private async Task SetToggleButtonOnlineStatus()
        {
            var objSwitches = App.AppService.GetSwitches(DeviceCode);
            ReturnModel onSwitch = JsonConvert.DeserializeObject<ReturnModel>(objSwitches.Result);

            Switch ctlSwitch;

            string tgl = "swhSwitch";
            int ctr = 1;
            var isTgl = false;
            object o;

            PropertyInfo[] props = onSwitch.GetType().GetProperties();

            for (ctr = 1; ctr < 9; ctr++)
            {

                ctlSwitch = grdControls.FindByName<Switch>(tgl + ctr.ToString());
                isTgl = false;

                if (ctlSwitch.IsVisible)
                {

                    o = (string)props.Single(p => p.Name.ToUpper() == ctlSwitch.ClassId.ToUpper()).GetValue(onSwitch);

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

        }

        private async Task SetDeviceOnlineStatusAsync(Button ctlBtn)
        {
            Tabs = (Task<List<SettingItem>>)Application.Current.Properties["tabs"];
            var buttonText = Tabs.Result.Find(f => f.NickName == ctlBtn.Text && f.DeviceCode == DeviceCode);

            ctlBtn.BackgroundColor = Color.Red;

            try
            {

                if (!string.IsNullOrEmpty(DeviceCode))
                {
                    IsDeviceEnabled = await App.AppService.GetDeviceStatusAsync(DeviceCode);
                }

                if (IsDeviceEnabled)
                {
                    ctlBtn.BackgroundColor = Color.Green;
                }
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
                //throw;
            }
        }

        private void swhSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            var tgl = (Switch)sender;
            var pin = tgl.ClassId;
            var value = tgl.IsToggled ? 1 : 0;
            lblError.Text = "";

            try
            {
                if (App.IsInternetAvailable)
                { 
                _ = App.AppService.SwitchOnOff(pin, value, DeviceCode);
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




    }
}