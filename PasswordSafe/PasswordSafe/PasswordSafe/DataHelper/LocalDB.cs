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
            //this connection string will encrpyt the database with a keyword
            SQLiteConnectionString connectionString = new SQLiteConnectionString(dbPath, true, "combination");

            _database = new SQLiteAsyncConnection(connectionString);
            _database.CreateTableAsync<Credential>().Wait();
            _database.CreateTableAsync<WifiCredential>().Wait();
            //_database.CreateTableAsync<BankCredential>().Wait();
            _database.CreateTableAsync<SocialMediaCredential>().Wait();
        }

        public Task<List<Credential>> GetAllCredentialsAsync()
        {            
            return _database.Table<Credential>().ToListAsync();
        }

        public Task<List<WifiCredential>> GetAllWifiCredentialsAsync()
        {
            return _database.Table<WifiCredential>().ToListAsync();
        }

        public Task<List<BankCredential>> GetAllBankCredentialsAsync()
        {
            return _database.Table<BankCredential>().ToListAsync();
        }

        public Task<List<SocialMediaCredential>> GetAllSocialMediaCredentialsAsync()
        {
            return _database.Table<SocialMediaCredential>().ToListAsync();
        }
        public Task<int> SaveCredentialAsync(Credential credential)
        {
            //ID = 0 , default value >> new user
            if (credential.ID == 0)
                return _database.InsertAsync(credential);

            //Update user
            return _database.UpdateAsync(credential);
        }
        public Task<int> SaveCredentialAsync(WifiCredential credential)
        {
            //ID = 0 , default value >> new user
            if (credential.ID == 0)
                return _database.InsertAsync(credential);

            //Update user
            return _database.UpdateAsync(credential);
        }

        public Task<int> SaveCredentialAsync(BankCredential credential)
        {
            //ID = 0 , default value >> new user
            if (credential.ID == 0)
                return _database.InsertAsync(credential);

            //Update user
            return _database.UpdateAsync(credential);
        }

        public Task<int> SaveCredentialAsync(SocialMediaCredential credential)
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
