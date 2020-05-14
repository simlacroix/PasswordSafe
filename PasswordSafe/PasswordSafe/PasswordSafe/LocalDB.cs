using PasswordSafe.Models;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PasswordSafe
{
    public class LocalDB
    {
        //Connection to the SQLite DB
        private readonly SQLiteAsyncConnection _database;

        public LocalDB(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Credential>().Wait();
        }

        public Task<List<Credential>> GetAllUsersAsync()
        {
            return _database.Table<Credential>().ToListAsync();
        }

        public Task<int> SaveUserAsync(Credential credential)
        {
            //ID = 0 , default value >> new user
            if (credential.ID == 0)
                return _database.InsertAsync(credential);

            //Update user
            return _database.UpdateAsync(credential);
        }

        public Task<int> DeleteUserAsync(Credential credential)
        {
            return _database.DeleteAsync(credential);
        }
    }
}
