using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace SvichEx.Services
{
    public class RestService
    {
        HttpClient client;

        public RestService()
        {
            client = new HttpClient();
            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "3d6cc6a29c8c43549769a7ba23d6f6ca");
        }

        public async Task<bool> GetDeviceStatusAsync(string deviceCode)
        {
            string url = App.ApiUrl + deviceCode + "/IsHardwareConnected";
            bool objResponce = false;
            try
            {
                var response = await client.GetAsync(url);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    var responseStream = response.Content.ReadAsStringAsync().Result;
                    objResponce = bool.Parse(responseStream);
                }
            }
            catch
            {
                throw new ApplicationException("Unable to connect to server");
            }

            return objResponce;
        }

        public async Task<bool> SwitchOnOff(string pin, int value, string deviceCode)
        {

            string apiUrl = App.ApiUrl + deviceCode + "/" + pin + "/" + value;

            try
            {
                var response = await client.GetAsync(apiUrl);
                return response.StatusCode == HttpStatusCode.OK;
            }
            catch (Exception)
            {
                throw new ApplicationException("Unable to connect to server");
            }

        }

        public async Task<string> GetSwitches(string deviceCode)
        {
            string apiUrl = App.ApiUrl + deviceCode + "/Get";

            try
            {
                var response = client.GetAsync(apiUrl).Result;
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.Content.ReadAsStringAsync().Result;
                }
                else
                {
                    return null;
                }


            }
            catch (Exception)
            {
                throw new ApplicationException("Unable to connect to server");
            }

        }
    }
}
