using SQLite;
using SvichEx.ExtensionMethods;
using SvichEx.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvichEx.DbRepository
{
    public static class DBConstants
    {
        public const string DatabaseFilename = "RearCop.db3";

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;
    
        public static string DatabasePath
        {
            get
            {
                var basePath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                return Path.Combine(basePath, DatabaseFilename);
            }
        }
    }

    public class AppSettingDatabase
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(DBConstants.DatabasePath, DBConstants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public AppSettingDatabase()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(SettingItem).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.AutoIncPK, typeof(SettingItem)).ConfigureAwait(false);
                    initialized = true;
                }

                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(SettingItemDetail).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.AutoIncPK, typeof(SettingItemDetail)).ConfigureAwait(false);
                    initialized = true;
                }

                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(AppControl).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.AutoIncPK, typeof(AppControl)).ConfigureAwait(false);
                    initialized = true;
                }
            }
        }


        #region "SettingItem Methods"

        public Task<List<SettingItem>> GetAllItems()
        {
            var sql = "SELECT SettingItem.*,SettingItemDetail.*,AppControl.* FROM SettingItem " +
                " INNER JOIN SettingItemDetail ON SettingItemDetail.DeviceCode =SettingItem.DeviceCode  " +
                " INNER JOIN AppControl ON  AppControl.ItemId = SettingItemDetail.ID ";

            var obj = Database.QueryAsync<SettingItem>(sql);

            //var obj =Database.TableMappings.Select


            return obj;
        }


        public Task<List<SettingItem>> GetItems()
        {
            return Database.Table<SettingItem>().ToListAsync();
        }


        public Task<SettingItem> GetItemAsync(string deviceCode)
        {
            return Database.Table<SettingItem>().Where(i => i.DeviceCode == deviceCode).FirstOrDefaultAsync();
        }

        public int SaveItemAsync(SettingItem item)
        {
            if (item.Id!=0)
            {
                return Database.UpdateAsync(item).Result;
            }
            else
            {
                return Database.InsertAsync(item).Result;
            }
        }

        public int DeleteItemAsync(SettingItem item)
        {
            return Database.DeleteAsync(item).Result;
        }

        public Task<int> DeleteItem()
        {
            return Database.Table<SettingItem>().DeleteAsync();
        }


        #endregion

        #region "SettingItemDetail Methods"
        public Task<List<SettingItemDetail>> GetItemDetailAsync()
        {
            return Database.Table<SettingItemDetail>().ToListAsync();
        }

        public Task<List<SettingItemDetail>> GetItemsDetailNotDoneAsync()
        {
            // SQL queries are also possible
            return Database.QueryAsync<SettingItemDetail>("SELECT * FROM [SettingItemDetail]");
        }

        public Task<List<SettingItemDetail>> GetItemDetailAsync(string deviceCode)
        {
            return Database.Table<SettingItemDetail>().Where(i => i.DeviceCode == deviceCode).ToListAsync();
            //return Database.QueryAsync<SettingItemDetail>("SELECT * FROM [SettingItemDetail] WHERE [DeviceCode] = '" + deviceCode + "'");
            //return Database.QueryAsync<SettingItemDetail>("SELECT  [SettingItemDetail].*, [AppControl].* FROM [SettingItemDetail] INNER JOIN [AppControl] ON [AppControl].[ItemId] = [SettingItemDetail].[ID] WHERE [SettingItemDetail].[DeviceCode] ='" + deviceCode + "'");
            //Where DeviceCode = '" + deviceCode + "'" AppControl
        }

        private bool IsExistSettingItemDetail(string deviceCode)
        {
            bool returnValue = false;
            var obj = Database.Table<SettingItemDetail>().Where(i => i.DeviceCode == deviceCode).FirstOrDefaultAsync().Result;
            if (obj != null)
            {
                returnValue = obj.DeviceCode == deviceCode;
            }
            return returnValue;
        }

        public int SaveItemDetailAsync(SettingItemDetail item)
        {
            if (item.Id != 0)
            {
                _ = Database.UpdateAsync(item).Result;
                return item.Id;
            }
            else
            {
                _ = Database.InsertAsync(item).Result;
                var obj = Database.QueryAsync<SettingItemDetail>("Select Max(id) as [Id] from SettingItemDetail").Result.FirstOrDefault();
                return obj.Id;
            }
        }

        public Task<int> DeleteItemDetailAsync(SettingItemDetail item)
        {
            return Database.DeleteAsync(item);
        }

        public int DeleteItemDetailByDeviceCodeAsync(string deviceCode)
        {
            try
            {
                return Database.ExecuteAsync("DELETE FROM [SettingItemDetail] Where DeviceCode = '" + deviceCode +"'").Result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion

        #region "AppControl Methods"

        public int SaveAppControlAsync(AppControl item)
        {
            if (item.Id != 0)
            {
                return Database.UpdateAsync(item).Result;
            }
            else
            {
                return Database.InsertAsync(item).Result;
            }
        }

        public List<AppControl> GetAppControlAsync(SettingItemDetail item)
        {
            // SQL queries are also possible
            //return Database.<AppControl>("SELECT * FROM [AppControl] WHERE ItemId="+ item.Id);
            var o = Database.Table<AppControl>().Where(w => w.ItemId== item.Id).ToListAsync().Result;
            return o;
        }

        public AppControl GetAppControlElementAsync(SettingItemDetail item, string controlName)
        {
            // SQL queries are also possible
            //return Database.<AppControl>("SELECT * FROM [AppControl] WHERE ItemId="+ item.Id);
            var o = Database.Table<AppControl>().Where(w => w.ItemId == item.Id && w.ControlName == controlName && w.DeviceCode == item.DeviceCode).FirstOrDefaultAsync().Result;
            return o;
        }

        public Task<int> UpdateAppControlAsync(AppControl item)
        {
            var o = Database.UpdateAsync(item);
            return o;
        }

        public int DeleteAppControlByDeviceCodeAsync(string deviceCode)
        {
            try
            {
                return Database.ExecuteAsync("DELETE FROM [AppControl] Where DeviceCode = '" + deviceCode + "'").Result;
            }
            catch (Exception)
            {

                throw;
            }
        }

        #endregion
    }


}
