using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Nfc.Tech;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using SvichEx.Services;
using Xamarin.Forms;
using ZXing.Mobile;

[assembly: Dependency(typeof(SvichEx.Droid.AppServices.QRScannerService))]

namespace SvichEx.Droid.AppServices
{
    public class QRScannerService : IQRScannerService
    {
       async Task<string> IQRScannerService.ScanAsync()
        {
            var optionsCustom = new MobileBarcodeScanningOptions();

            var scanner = new MobileBarcodeScanner()
            {
                TopText = "Scan the QR Code",
                BottomText = "Please Wait",
            };

            var scanResult = await scanner.Scan(optionsCustom);
            return scanResult.Text;
        }
    }
}