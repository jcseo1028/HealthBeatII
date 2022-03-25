using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using HealthBeatII.Models;

namespace HealthBeatII
{
    class HistoryDatabase
    {
        static SQLiteAsyncConnection Database;

        public static readonly AsyncLazy<HistoryDatabase> Instance = new AsyncLazy<HistoryDatabase>(async () =>
        {
            var instance = new HistoryDatabase();

            CreateTableResult result_History = await Database.CreateTableAsync<HistoryItem>();
            CreateTableResult result_Practice = await Database.CreateTableAsync<PracticeItem>();
            CreateTableResult result_Combined = await Database.CreateTableAsync<CombinedPracticeItem>();

            return instance;
        });

        public HistoryDatabase()
        {
            Database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        }

        public Task<List<HistoryItem>> GetHistoryItemsAsync()
        {
            return Database.Table<HistoryItem>().ToListAsync();
        }

        public Task<List<PracticeItem>> GetPracticeItemsAsync()
        {
            return Database.Table<PracticeItem>().ToListAsync();
        }

        public Task<PracticeItem> GetPracticeItemAsync(int id)
        {
            return Database.Table<PracticeItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<CombinedPracticeItem> GetCombinedPracticeItemAsync(int id)
        {
            return Database.Table<CombinedPracticeItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<List<CombinedPracticeItem>> GetCombinedPracticeItemsAsync()
        {
            return Database.Table<CombinedPracticeItem>().ToListAsync();
        }

        public Task<List<HistoryItem>> GetItemsNotDoneAsync()
        {
            return Database.QueryAsync<HistoryItem>("SELECT * FROM [HistoryItem]");
            //return Database.QueryAsync<HistoryItem>("SELECT * FROM [HistoryItem] WHERE [Done] = 0");
        }

        public Task<HistoryItem> GetHistoryItemAsync(int id)
        {
            return Database.Table<HistoryItem>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveItemAsync(HistoryItem item)
        {
            if (item.Id != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> SavePracticeItemAsync(PracticeItem item)
        {
            if (item.Id != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> SaveCommbinedItemAsync(CombinedPracticeItem item)
        {
            if (item.Id != 0)
            {
                return Database.UpdateAsync(item);
            }
            else
            {
                return Database.InsertAsync(item);
            }
        }

        public Task<int> DeleteItemAsync(HistoryItem item)
        {
            return Database.DeleteAsync(item);
        }

        public Task<int> DeletePracticeItemAsync(PracticeItem item)
        {
            return Database.DeleteAsync(item);
        }

        public Task<int> DeleteCombinedPracticeItemAsync(CombinedPracticeItem item)
        {
            return Database.DeleteAsync(item);
        }
    }
}
