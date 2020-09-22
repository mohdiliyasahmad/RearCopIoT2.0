using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using ZXing;
using SvichEx.ViewModel;
using SvichEx.DbRepository;
using System.Threading.Tasks;

namespace SvichEx
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SettingPage : ContentPage
    {
        SettingItem settingItem;
        List<SettingItem> itemsToBeSaved;
        public SettingPage()
        {
            InitializeComponent();
            settingItem = new SettingItem();
            itemsToBeSaved = new List<SettingItem>();
            lblError.Text = "";
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            PopulateData();

        }

        public void scanView1_OnScanResult(Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Scanned result", "The barcode's text is " + result.Text + ". The barcode's format is " + result.BarcodeFormat, "OK");
            });
        }

        public void scanView2_OnScanResult(Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Scanned result", "The barcode's text is " + result.Text + ". The barcode's format is " + result.BarcodeFormat, "OK");
            });
        }
        public void scanView3_OnScanResult(Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Scanned result 3", "The barcode's text is " + result.Text + ". The barcode's format is " + result.BarcodeFormat, "OK");
            });
        }
        public void scanView4_OnScanResult(Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Scanned result 4", "The barcode's text is " + result.Text + ". The barcode's format is " + result.BarcodeFormat, "OK");
            });
        }

        private void Command_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)(sender);
            Editor lbl = (Editor)btn.BindingContext;
            ValidateInput(lbl.Text);
            settingItem.DeviceCode = string.IsNullOrEmpty(lbl.Text) ? "Dummy" : lbl.Text;
            Navigation.PushAsync(new SettingDetails(settingItem));
        }

        private void ValidateInput(string strvalue)
        {
            if (strvalue != "Device Code...")
            {
                return;
            }
        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {
            Switch ctltgl;
            Editor ctlnickname;
            Editor ctldeviceCode;
            Label ctllbl;

            string deviceCode = "txtDeviceCode";
            string nickName = "txtScanView";
            string isVisible = "tglScanView";
            string lblid = "lblId";

            //App.Database.DeleteItem();

            for (int i = 1; i < 7; i++)
            {
                ctldeviceCode = grdControls.FindByName<Editor>(deviceCode + i.ToString());
                ctlnickname = grdControls.FindByName<Editor>(nickName + i.ToString());
                ctltgl = grdControls.FindByName<Switch>(isVisible + i.ToString());
                ctllbl = grdControls.FindByName<Label>(lblid + i.ToString());
            
                settingItem = new SettingItem();

                settingItem.DeviceCode = string.IsNullOrEmpty(ctldeviceCode.Text) ? "" : ctldeviceCode.Text;
                settingItem.NickName = string.IsNullOrEmpty(ctlnickname.Text) ? "" : ctlnickname.Text;
                settingItem.IsVisible = ctltgl.IsToggled;
                settingItem.Id = int.Parse(string.IsNullOrEmpty(ctllbl.Text) ? "0" : ctllbl.Text);

                SaveToDatabase(settingItem);

            }
            var obj = App.Database.GetItems();
            Application.Current.Properties["tabs"] = obj;

            Navigation.PushAsync(new MainPage());
    
        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();

        }


        private void SaveToDatabase(SettingItem item)
        {
            var a = App.Database.SaveItemAsync(item);
        }

        private void PopulateData()
        {
            var obj = (Task<List<SettingItem>>)Application.Current.Properties["tabs"];// App.Database.GetItems();

            Switch ctltgl;
            Editor ctlnickname;
            Editor ctldeviceCode;
            Label ctllbl;

            string deviceCode = "txtDeviceCode";
            string nickName = "txtScanView";
            string isVisible = "tglScanView";
            string lblid = "lblId";


            int i =1;

            foreach (var item in obj.Result)
            {
                ctldeviceCode = grdControls.FindByName<Editor>(deviceCode + i.ToString());
                ctlnickname = grdControls.FindByName<Editor>(nickName + i.ToString());
                ctltgl = grdControls.FindByName<Switch>(isVisible + i.ToString());
                ctllbl = grdControls.FindByName<Label>(lblid + i.ToString());

                if (item != null)
                { 
                    ctldeviceCode.Text = item.DeviceCode;
                    ctlnickname.Text = item.NickName;
                    ctltgl.IsToggled = item.IsVisible;
                    ctllbl.Text = item.Id.ToString();
                }

               i++;
            }

        }
    }
}
