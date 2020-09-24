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

namespace SvichEx
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainPage : ContentPage
    {
        public RestService ApiService { get; set; }
        public string DeviceCode { get; set; }
        public bool IsDeviceEnabled { get; set; }

        public MainPage()
        {
            InitializeComponent();
            ApiService = new RestService();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            try
            {
                PopulateTabs();
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }

        private void PopulateSwitchesByDeviceCode()
        {
            Task<List<SettingItemDetail>> swiches;

            if (!Application.Current.Properties.ContainsKey(DeviceCode))
            {
                swiches = App.Database.GetItemDetailAsync(DeviceCode);
                Application.Current.Properties[DeviceCode] = swiches;
            }
            else
            {
                swiches = (Task<List<SettingItemDetail>>)Application.Current.Properties[DeviceCode];
            }
           
            PopulateElements(swiches);
        }


        private void PopulateElements(Task<List<SettingItemDetail>> swiches)
        {
            string lbl = "txtSwitch", tgl = "swhSwitch";
            int ctr = 1;
            Label ctlLbl;
            Switch ctlSwitch;

            for (int i = 0; i < 8; i++)
            {
                ctlLbl = stkButtons.FindByName<Label>(lbl + ctr.ToString());
                ctlSwitch = stkButtons.FindByName<Switch>(tgl + ctr.ToString());
                ctlLbl.IsVisible = false;
                ctlSwitch.IsVisible = false;
            }


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

                if (item != null && item.ControlName == tglName)
                {
                    if (item.IsVisible)
                    {
                        Application.Current.Properties[swch.Id.ToString()] = item;
                        swch.IsToggled = string.IsNullOrEmpty(item.Value) ? false : bool.Parse(item.Value);
                    }
                }
            }

        }


        private void PopulateTabs()
        {
            try
            {


                var obj = (Task<List<SettingItem>>)Application.Current.Properties["tabs"];

                Button ctlBtn;
                string btnid = "btnTab";

                int i = 1;

                foreach (var item in obj.Result)
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
            try
            {
                DeviceCode = ((Button)sender).ClassId;
                IsDeviceEnabled = false;
                SetTab((Button)sender);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }


        private void ResetElements()
        {
            btnTab1.BackgroundColor = Color.LightGray;
            btnTab2.BackgroundColor = Color.LightGray;
            btnTab3.BackgroundColor = Color.LightGray;
            btnTab4.BackgroundColor = Color.LightGray;
            btnTab5.BackgroundColor = Color.LightGray;
            btnTab6.BackgroundColor = Color.LightGray;
            lblError.Text = "";
        }

        private void SetTab(Button ctlBtn)
        {
            try
            {
                ResetElements();
                ctlBtn.BackgroundColor = Color.Green;
                DeviceCode = ctlBtn.ClassId;
                PopulateSwitchesByDeviceCode();
                _ = SetDeviceOnlineStatusAsync();
            }
            catch (Exception)
            {

                throw;
            }

        }

        private async Task SetDeviceOnlineStatusAsync()
        {
            lblDeviceStatus.Text = "OffLine";
            lblDeviceStatus.BackgroundColor = Color.Red;

            try
            {

                if (!string.IsNullOrEmpty(DeviceCode))
                {
                    IsDeviceEnabled = await ApiService.GetDeviceStatusAsync(DeviceCode);
                }

                if (IsDeviceEnabled)
                {
                    lblDeviceStatus.Text = "Online";
                    lblDeviceStatus.BackgroundColor = Color.Green;
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
            var item = (AppControl)Application.Current.Properties[tgl.Id.ToString()];
            item.Value = tgl.IsToggled.ToString();
            try
            {
                _ = App.Database.UpdateAppControlAsync(item);
                _ = ApiService.SwitchOnOff(pin, value, DeviceCode);
            }
            catch (Exception ex)
            {
                lblError.Text = ex.Message;
            }
        }




    }
}