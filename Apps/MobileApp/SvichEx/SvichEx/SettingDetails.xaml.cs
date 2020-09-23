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
        public SettingItem DeviceSetting { get; set; }
        //public SettingPage PreviousPage { get; set; }

        List<SettingItemDetail> swiches;
        SettingItemDetail objSwitch;
        //public SettingDetails(SettingItem settingItem, SettingPage previousPage)
        public SettingDetails(SettingItem settingItem)
        {
            InitializeComponent();
            DeviceSetting = settingItem;
            //PreviousPage = previousPage;
            txtDeviceCode.Text = "for device \"" + DeviceSetting.DeviceCode + "\"";
            swiches = App.Database.GetItemDetailAsync(DeviceSetting.DeviceCode).Result;
            PopulateElements(swiches);
            lblError.Text = "";
        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {
            string entry = "txtSwitch", tgl = "swhSwitch";
            Editor txtEntry;
            Switch tglSwitch;

            swiches.Clear();

            for (int i = 1; i < 9; i++)
            {
                txtEntry = stkButtons.FindByName<Editor>(entry + i.ToString());
                tglSwitch = stkButtons.FindByName<Switch>(tgl + i.ToString());
                swiches.Add(SwitchDetail(txtEntry.Text, tglSwitch.IsToggled, entry + i.ToString(), tgl + i.ToString()));
            }

            SaveToDatabase(swiches);
            Navigation.PopAsync();
        }



        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        AppControl pageControl;

        private SettingItemDetail SwitchDetail(string switchName, bool isVisible,string edtName,string tglName)
        {
            objSwitch = new SettingItemDetail();
            objSwitch.DeviceCode = DeviceSetting.DeviceCode;
            objSwitch.IsVisible = isVisible;

            pageControl = new AppControl { ControlName = edtName, Value = switchName};
            objSwitch.AppSwitch = pageControl;

            pageControl = new AppControl { ControlName = tglName, IsVisible =isVisible};
            objSwitch.AppToggle = pageControl;

            return objSwitch;
        }

        private void SaveToDatabase(List<SettingItemDetail> swiches)
        {
            var a= App.Database.DeleteItemDetailByDeviceCodeAsync(DeviceSetting.DeviceCode);
            var b =App.Database.DeleteAppControlByDeviceCodeAsync(DeviceSetting.DeviceCode);

            foreach (var item in swiches)
            {
                var itemid =  App.Database.SaveItemDetailAsync(item);
                item.AppSwitch.ItemId = itemid;
                item.AppToggle.ItemId = itemid;
                item.AppSwitch.DeviceCode = DeviceSetting.DeviceCode;
                item.AppToggle.DeviceCode = DeviceSetting.DeviceCode;
                item.AppSwitch.IsVisible = item.AppToggle.IsVisible;

                App.Database.SaveAppControlAsync(item.AppSwitch);
                App.Database.SaveAppControlAsync(item.AppToggle);

            }


        }

        private void PopulateElements(List<SettingItemDetail> swiches)
        {
            string entry = "txtSwitch", tgl = "swhSwitch";
            int ctr = 1;
            Editor txtEdit;
            Switch tglSwitch;

            foreach (var item in swiches)
            {
                txtEdit = stkButtons.FindByName<Editor>(entry + ctr.ToString());
                tglSwitch = stkButtons.FindByName<Switch>(tgl + ctr.ToString());
                PopulateDetails(item, txtEdit, tglSwitch, entry + ctr.ToString(), tgl + ctr.ToString());
                ctr++;
            }

        }

        private void PopulateDetails(SettingItemDetail pitem, Editor txt, Switch swch, string edtName,string tglName)
        {
            var obj = App.Database.GetAppControlAsync(pitem);
            foreach (var item in obj)
            {
                if (item != null && item.ControlName == edtName)
                {
                    txt.Text = item.ControlName == edtName ? item.Value : "";
                }

                if (item != null && item.ControlName == tglName)
                {
                    swch.IsToggled = item.IsVisible;// string.IsNullOrEmpty(item.Value) ? false: bool.Parse(item.Value);
                }
            }

        }
    }
}