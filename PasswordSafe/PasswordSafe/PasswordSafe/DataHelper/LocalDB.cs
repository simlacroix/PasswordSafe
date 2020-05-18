using Microsoft.Data.Sqlite;
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

            var connectionString = new SqliteConnectionStringBuilder()
            {
                DataSource = dbPath,
                Mode = SqliteOpenMode.ReadWriteCreate,
                Password = "combination"
            }.ToString();

            _database = new SQLiteAsyncConnection(connectionString);
            _database.CreateTableAsync<Credential>().Wait();
        }

        public Task<List<Credential>> GetAllCredentialsAsync()
        {            
            return _database.Table<Credential>().ToListAsync();
        }

        public Task<int> SaveCredentialAsync(Credential credential)
        {
            //ID = 0 , default value >> new user
            if (credential.ID == 0)
                return _database.InsertAsync(credential);

            //Update user
            return _database.UpdateAsync(credential);
        }

        public Task<int> DeleteCredentialAsync(Credential credential)
        {
            return _database.DeleteAsync(credential);
        }
    }
}
