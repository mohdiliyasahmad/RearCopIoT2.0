using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xamarin.Forms;
using ZXing;
using SvichEx.ViewModel;
using SvichEx.DbRepository;
using System.Threading.Tasks;
using ZXing.Net.Mobile.Forms;
using Xamarin.Forms.Markup;
using ZXing.Mobile;

namespace SvichEx
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class SettingPage : ContentPage
    {

        public Task<List<SettingItem>> Tabs { get; set; }

        SettingItem settingItem;
        List<SettingItem> itemsToBeSaved;

        Editor ctlnickname;
        Editor ctldeviceCode;
        Label ctllbl;
        StackLayout ctlScn;

        string deviceCode = "txtDeviceCode";
        string nickName = "txtScanView";
        string lblid = "lblId";
        string scn = "secRoom";



        public SettingPage()
        {
            InitializeComponent();
            settingItem = new SettingItem();
            itemsToBeSaved = new List<SettingItem>();

            var options = new MobileBarcodeScanningOptions
            {
                AutoRotate = true
            };

            scanView1.Options = options;
           
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            lblError.Text = "";

            if (App.IsInternetAvailable)
            {
                PopulateData();
            }
            else
            {
                lblError.Text = "No internet dectected";
            }
        }

        public void scanView1_OnScanResult(Result result)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                for (int i = 6; i > 0; i--)
                {
                    ctlScn = grdControls.FindByName<StackLayout>(scn + i.ToString());
                    ctldeviceCode = grdControls.FindByName<Editor>(deviceCode + i.ToString());
                    
                    if (ctlScn.IsVisible)
                    {
                        ctldeviceCode.Text = result.Text;
                        break;
                    }
                }
            });
        }

    
        private void Command_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)(sender);
            Editor lbl = (Editor)btn.BindingContext;
            lblError.Text = "";
            if (App.IsInternetAvailable)
            {
                if (!ValidateInput(lbl.Text))
                {
                    return;
                }
                
                SaveSettings();
               
                settingItem.DeviceCode = string.IsNullOrEmpty(lbl.Text) ? "No Device" : lbl.Text;
                Navigation.PushAsync(new SettingDetails(settingItem));
            }
            else
            {
                lblError.Text = "No internet dectected";
            }
        }

        private bool ValidateInput(string strvalue)
        {

            if (strvalue == "Device Code..." ||  strvalue.Contains("No Device"))
            {
                DisplayAlert("SvichEx", "Device code is missing,Please provide device code","Cancel");
                return false;
            }
            else
            {
                return true;
            }
        }

        private void btnSave_Clicked(object sender, EventArgs e)
        {
            SaveSettings();
            Navigation.PopAsync();
            Navigation.PushAsync(new MainPage());
        }

        private void SaveSettings()
        {
            if (App.IsInternetAvailable)
            {

                for (int i = 1; i < 7; i++)
                {
                    ctldeviceCode = grdControls.FindByName<Editor>(deviceCode + i.ToString());
                    ctlnickname = grdControls.FindByName<Editor>(nickName + i.ToString());
                    ctlScn = grdControls.FindByName<StackLayout>(scn + i.ToString());
                    ctllbl = grdControls.FindByName<Label>(lblid + i.ToString());

                    settingItem = new SettingItem();

                    settingItem.DeviceCode = string.IsNullOrEmpty(ctldeviceCode.Text) ? "No Device " + i : ctldeviceCode.Text;
                    settingItem.NickName = string.IsNullOrEmpty(ctlnickname.Text) ? "" : ctlnickname.Text;
                    settingItem.IsVisible = ctlScn.IsVisible;
                    settingItem.Id = int.Parse(string.IsNullOrEmpty(ctllbl.Text) ? "0" : ctllbl.Text);

                    SaveToDatabase(settingItem);
                }
               
                //PopulateData(App.Database.GetItems().Result);
                Tabs = App.Database.GetItems();
                Application.Current.Properties["tabs"] = Tabs;
            }
            else
            {
                lblError.Text = "No internet dectected";
            }


        }

        private void btnCancel_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }


        private void SaveToDatabase(SettingItem item)
        {
            App.Database.SaveItemAsync(item);
        }

        private void PopulateData()
        {
            Tabs = (Task<List<SettingItem>>)Application.Current.Properties["tabs"];// App.Database.GetItems();

            int i = 1;

            foreach (var item in Tabs.Result)
            {
                ctldeviceCode = grdControls.FindByName<Editor>(deviceCode + i.ToString());
                ctlnickname = grdControls.FindByName<Editor>(nickName + i.ToString());
                ctlScn = grdControls.FindByName<StackLayout>(scn + i.ToString());
                ctllbl = grdControls.FindByName<Label>(lblid + i.ToString());

                if (item != null)
                {
                    ctldeviceCode.Text = item.DeviceCode;
                    ctlnickname.Text = item.NickName;
                    ctlScn.IsVisible = item.IsVisible;
                    ctllbl.Text = item.Id.ToString();
                }

                i++;
            }

        }

        private void PopulateData(List<SettingItem> lst)
        {
            
            int i = 1;

            foreach (var item in lst)
            {
                ctldeviceCode = grdControls.FindByName<Editor>(deviceCode + i.ToString());
                ctlnickname = grdControls.FindByName<Editor>(nickName + i.ToString());
                ctlScn = grdControls.FindByName<StackLayout>(scn + i.ToString());
                ctllbl = grdControls.FindByName<Label>(lblid + i.ToString());

                if (item != null)
                {
                    ctldeviceCode.Text = item.DeviceCode;
                    ctlnickname.Text = item.NickName;
                    ctlScn.IsVisible = item.IsVisible;
                    ctllbl.Text = item.Id.ToString();
                }

                i++;
            }

        }
        private void tglScanView_Toggled(object sender, ToggledEventArgs e)
        {
            Switch sw = (Switch)(sender);
            //scnr = (ZXingScannerView)sw.BindingContext;
            //scnr.IsEnabled = sw.IsToggled;
        }

        private void btnScanView_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            ZXingScannerView scnner = (ZXingScannerView)btn.BindingContext;
            scnner.IsScanning = true;



        }

        private void Button_Clicked(object sender, EventArgs e)
        {
            StackLayout scnRoom;
            for (int i = 2; i < 7; i++)
            {
                scnRoom = grdControls.FindByName<StackLayout>("secRoom" + i.ToString());
                if (!scnRoom.IsVisible)
                {
                    scnRoom.IsVisible = true;
                    break;
                }

            }

        }

        private void btnDelete_Clicked(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            StackLayout scnRoom;
            for (int i = 6; i > 1; i--)
            {
                scnRoom = grdControls.FindByName<StackLayout>("secRoom" + i.ToString());
                if (scnRoom.IsVisible)
                {
                    scnRoom.IsVisible = false;
                    break;
                }

            }
        }
    }
}
