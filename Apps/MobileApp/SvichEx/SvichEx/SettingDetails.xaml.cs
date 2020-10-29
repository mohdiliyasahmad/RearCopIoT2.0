using SvichEx.DbRepository;
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
    public partial class SettingDetails : ContentPage
    {
        string entry = "txtSwitch", tgl = "swhSwitch";
        int Id = 0;
        Editor txtEntry;
        Switch tglSwitch;
        SettingItemDetail settingItemDetail = null;
        Editor txtEdit;
      

        public SettingItem DeviceSetting { get; set; }
        Task<List<SettingItemDetail>> swiches;
        Task<List<SettingItemDetail>> swichesTemp;

        SettingItemDetail objSwitch;

        public SettingDetails(SettingItem settingItem)
        {
            InitializeComponent();
            DeviceSetting = settingItem;

            txtDeviceCode.Text = "for device \"" + DeviceSetting.DeviceCode + "\"";
            lblError.Text = "";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            lblError.Text = "";
            if (App.IsInternetAvailable)
            {
                swiches = App.Database.GetItemDetailAsync(DeviceSetting.DeviceCode);
                swichesTemp = swiches;
                PopulateElements(swiches);
            }
            else
            {
                lblError.Text = "No internet dectected";
            }
        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {
           
            lblError.Text = "";

            ((Button)sender).IsEnabled = false;

            swichesTemp.Result.Clear();

            if (App.IsInternetAvailable)
            {

                for (int i = 1; i < 9; i++)
                {
                    txtEntry = stkButtons.FindByName<Editor>(entry + i.ToString());
                    tglSwitch = stkButtons.FindByName<Switch>(tgl + i.ToString());
                    Id = string.IsNullOrEmpty(txtEntry.AutomationId) ? 0 : int.Parse(txtEntry.AutomationId);
                    settingItemDetail = null;
                    if (Id > 0)
                    {
                        settingItemDetail = App.Database.GetItemDetailAsync(DeviceSetting.DeviceCode, Id);
                    }

                    if (settingItemDetail != null)
                    {
                        settingItemDetail = SwitchDetail(settingItemDetail, txtEntry.Text, tglSwitch.IsToggled, entry + i.ToString(), tgl + i.ToString());
                    }
                    else
                    {
                        settingItemDetail = SwitchDetail(txtEntry.Text, tglSwitch.IsToggled, entry + i.ToString(), tgl + i.ToString(), Id);
                        //swiches.Result.Add(SwitchDetail(txtEntry.Text, tglSwitch.IsToggled, entry + i.ToString(), tgl + i.ToString(), Id));
                    }

                    settingItemDetail.SettingId = DeviceSetting.Id;
                    settingItemDetail.Id = App.Database.SaveItemDetailAsync(settingItemDetail);
                    App.Database.DeleteAppControlByDeviceCodeAsync(settingItemDetail);

                    settingItemDetail.AppSwitch.ItemId = settingItemDetail.Id;
                    App.Database.SaveAppControlAsync(settingItemDetail.AppSwitch);
                    settingItemDetail.AppToggle.ItemId = settingItemDetail.Id;
                    App.Database.SaveAppControlAsync(settingItemDetail.AppToggle);
                }
            }
            else
            {
                lblError.Text = "No internet dectected";
            }

            ((Button)sender).IsEnabled = true;
            Navigation.PopAsync();
        }



        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        AppControl pageControl;

        private SettingItemDetail SwitchDetail(string switchName, bool isVisible, string edtName, string tglName, int id)
        {
            objSwitch = new SettingItemDetail();
            objSwitch.Id = id;
            objSwitch.DeviceCode = DeviceSetting.DeviceCode;
            objSwitch.IsVisible = isVisible;

            pageControl = new AppControl { ControlName = edtName, Value = switchName, ItemId = id, DeviceCode = DeviceSetting.DeviceCode, IsVisible = isVisible };
            objSwitch.AppSwitch = pageControl;

            pageControl = new AppControl { ControlName = tglName, IsVisible = isVisible, ItemId = id, DeviceCode = DeviceSetting.DeviceCode };
            objSwitch.AppToggle = pageControl;

            return objSwitch;
        }


        private SettingItemDetail SwitchDetail(SettingItemDetail objSwitch, string switchName, bool isVisible, string edtName, string tglName)
        {
            objSwitch.DeviceCode = DeviceSetting.DeviceCode;
            objSwitch.IsVisible = isVisible;

            pageControl = new AppControl { ControlName = edtName, Value = switchName, ItemId = objSwitch.Id, DeviceCode = DeviceSetting.DeviceCode, IsVisible = isVisible };
            objSwitch.AppSwitch = pageControl;

            pageControl = new AppControl { ControlName = tglName, IsVisible = isVisible, ItemId = objSwitch.Id, DeviceCode = DeviceSetting.DeviceCode };
            objSwitch.AppToggle = pageControl;

            return objSwitch;
        }

        private void PopulateElements(Task<List<SettingItemDetail>> swiches)
        {
            string entry = "txtSwitch", tgl = "swhSwitch";
            int ctr = 1;
       
            foreach (var item in swiches.Result)
            {
                txtEdit = stkButtons.FindByName<Editor>(entry + ctr.ToString());
                tglSwitch = stkButtons.FindByName<Switch>(tgl + ctr.ToString());
                txtEdit.AutomationId = item.Id.ToString();
                PopulateDetails(item, txtEdit, tglSwitch, entry + ctr.ToString(), tgl + ctr.ToString());
                ctr++;
            }

        }

        private void PopulateDetails(SettingItemDetail pitem, Editor txt, Switch swch, string edtName, string tglName)
        {
            var obj = App.Database.GetAppControlAsync(pitem);
            foreach (var item in obj)
            {
                if (item != null && item.ControlName == edtName)
                {
                    txt.Text = item.ControlName == edtName ? item.Value : "";
                    pitem.AppSwitch = App.Database.GetAppControlElementAsync(pitem, edtName);
                }

                if (item != null && item.ControlName == tglName)
                {
                    pitem.AppToggle = App.Database.GetAppControlElementAsync(pitem, tglName);
                    swch.IsToggled = item.IsVisible;// string.IsNullOrEmpty(item.Value) ? false: bool.Parse(item.Value);
                }
            }

        }
    }
}