using SQLite;
using SQLiteNetExtensions.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace SvichEx.ViewModel
{
    [Table("SettingItemDetail")]
    public class SettingItemDetail
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int DeviceID { get; set; }
        public string DeviceCode { get; set; }
        public bool IsVisible { get; set; }
        
        [OneToMany("ItemId", "Id")]
        public AppControl AppSwitch { get; set; }
        
        [OneToMany("ItemId", "Id")]
        public AppControl AppToggle { get; set; }
    }

    [Table("AppControl")]
    public class AppControl
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public string ControlName { get; set; }
        public string Value { get; set; }
        public bool IsScheduler { get; set; }
        public string DeviceCode { get; set; }
        public bool IsVisible { get; set; }

    }

}
