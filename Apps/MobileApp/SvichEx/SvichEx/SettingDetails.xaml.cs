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
        Task<List<SettingItemDetail>> swiches;
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
            swiches = App.Database.GetItemDetailAsync(DeviceSetting.DeviceCode);
            PopulateElements(swiches);
        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {
            string entry = "txtSwitch", tgl = "swhSwitch";
            int Id = 0;
            Editor txtEntry;
            Switch tglSwitch;
            SettingItemDetail settingItemDetail;


            for (int i = 1; i < 9; i++)
            {
                txtEntry = stkButtons.FindByName<Editor>(entry + i.ToString());
                tglSwitch = stkButtons.FindByName<Switch>(tgl + i.ToString());
                Id = string.IsNullOrEmpty(txtEntry.AutomationId) ? 0 : int.Parse(txtEntry.AutomationId);

                settingItemDetail = swiches.Result.Find(f => f.Id == Id);
                if (settingItemDetail != null)
                {
                    SwitchDetail(settingItemDetail, tglSwitch.IsToggled, txtEntry.Text);
                }
                else
                {
                    swiches.Result.Add(SwitchDetail(txtEntry.Text, tglSwitch.IsToggled, entry + i.ToString(), tgl + i.ToString(), Id));
                }
            }

            SaveToDatabase(swiches);
            Navigation.PopAsync();
        }



        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        AppControl pageControl;

        private SettingItemDetail SwitchDetail(string switchName, bool isVisible,string edtName,string tglName,int id)
        {
            objSwitch = new SettingItemDetail();
            objSwitch.Id = id;
            objSwitch.DeviceCode = DeviceSetting.DeviceCode;
            objSwitch.IsVisible = isVisible;

            pageControl = new AppControl { ControlName = edtName, Value = switchName, ItemId =id};
            objSwitch.AppSwitch = pageControl;

            pageControl = new AppControl { ControlName = tglName, IsVisible =isVisible,ItemId = id};
            objSwitch.AppToggle = pageControl;

            return objSwitch;
        }

        private SettingItemDetail SwitchDetail(SettingItemDetail objSwitch, bool isVisible, string txtEntry)
        {
            objSwitch.IsVisible = isVisible;
            objSwitch.AppSwitch.IsVisible = isVisible;
            objSwitch.AppSwitch.Value = txtEntry;

            objSwitch.AppToggle.IsVisible = IsVisible;
            return objSwitch;
        }

        private void SaveToDatabase(Task<List<SettingItemDetail>> swiches)
        {
            //var a= App.Database.DeleteItemDetailByDeviceCodeAsync(DeviceSetting.DeviceCode);
            //var b =App.Database.DeleteAppControlByDeviceCodeAsync(DeviceSetting.DeviceCode);

            foreach (var item in swiches.Result)
            {
                App.Database.SaveAppControlAsync(item.AppSwitch);
                App.Database.SaveAppControlAsync(item.AppToggle);
            }

            Application.Current.Properties[DeviceSetting.DeviceCode] = swiches;

        }

        private void PopulateElements(Task<List<SettingItemDetail>> swiches)
        {
            string entry = "txtSwitch", tgl = "swhSwitch";
            int ctr = 1;
            Editor txtEdit;
            Switch tglSwitch;

            foreach (var item in swiches.Result)
            {
                txtEdit = stkButtons.FindByName<Editor>(entry + ctr.ToString());
                tglSwitch = stkButtons.FindByName<Switch>(tgl + ctr.ToString());
                txtEdit.AutomationId = item.Id.ToString();
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
                    pitem.AppSwitch = App.Database.GetAppControlElementAsync(pitem,edtName);
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