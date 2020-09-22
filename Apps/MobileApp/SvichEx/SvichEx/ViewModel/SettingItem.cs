using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SvichEx.ViewModel
{
    [Table("SettingItem")]
    public class SettingItem
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string DeviceCode { get; set; }
        public string NickName { get; set; }
        public bool IsEnabled { get; set; }
        public bool IsVisible { get; set; }

        [OneToMany("DeviceCode", "DeviceCode")]
        public List<SettingItemDetail> Switches { get; set; }
    }
}
