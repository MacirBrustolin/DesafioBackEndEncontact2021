﻿using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Dao;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Repository.Interface;

namespace TesteBackendEnContact.Repository
{
    public class ContactRepository : IContactRepository
    {
        private readonly DatabaseConfig databaseConfig;

        public ContactRepository(DatabaseConfig databaseConfig)
        {
            this.databaseConfig = databaseConfig;
        }

        public async Task<IContact> SaveAsync(IContact contact)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var dao = new ContactDao(contact);

            dao.Id = await connection.InsertAsync(dao);

            return dao.Export();
        }

        public async Task<IEnumerable<IContact>> GetAllAsync()
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT * FROM Contact";
            var result = await connection.QueryAsync<ContactDao>(query);

            return result?.Select(item => item.Export());
        }

        public async Task<IEnumerable<IContact>> GetAsync(string searchString)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT * FROM Contact WHERE ((Id LIKE '%' || @searchString || '%') OR (CompanyId LIKE '%' || @searchString || '%') OR (ContactBookId LIKE '%' || @searchString || '%') OR (Name LIKE '%' || @searchString || '%') OR (Phone LIKE '%' || @searchString || '%') OR (Email LIKE '%' || @searchString || '%') OR (Address LIKE '%' || @searchString || '%'))";
            var result = await connection.QueryAsync<ContactDao>(query, new { searchString });

            return result?.Select(item => item.Export());
        }

        public async Task<IEnumerable<string>> CompanyList()
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            connection.Open();
            using var transaction = connection.BeginTransaction();

            var sql = "select distinct Id from Company";

            var companies = await connection.QueryAsync<string>(sql, transaction);
            transaction.Commit();
            connection.Close();

            return companies;
        }
    }
}
