using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Security.Principal;
using System.Text;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace SvichEx.ViewModel
{
    public class SettingViewModel 
    {
        public INavigation Navigation { get; set; }
        public List<SettingItem> Items { get; set; }

        public SettingViewModel()
        {
           
        }
        
        public SettingViewModel(INavigation navigation)
        {
            this.Navigation = navigation;
            Save = new Command(SaveSettings);
            Cancel = new Command(CancelSettings);
        }

        public ICommand Save { set; get; }

        public ICommand Cancel { set; get; }

      
        private void SaveSettings()
        {

        }

        private void CancelSettings()
        {
            Navigation.PopToRootAsync();
        }
       
    }

}
