using Dapper;
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

        public async Task<IEnumerable<IContact>> GetAsync(int pageRows, int pageNumber, string searchString)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT * FROM Contact WHERE ((Id LIKE '%' || @searchString || '%') OR (CompanyId LIKE '%' || @searchString || '%') OR (ContactBookId LIKE '%' || @searchString || '%') OR (Name LIKE '%' || @searchString || '%') OR (Phone LIKE '%' || @searchString || '%') OR (Email LIKE '%' || @searchString || '%') OR (Address LIKE '%' || @searchString || '%')) LIMIT @pageRows OFFSET @page";
            var result = await connection.QueryAsync<ContactDao>(query, new { pageRows, page = pageRows*pageNumber, searchString });

            return result?.Select(item => item.Export());
        }

        public async Task<IEnumerable<IContact>> GetByCompanyAndContactBook(int companyId, int contactBookId)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT Contact.Id, Contact.CompanyId, Contact.ContactBookId, Company.Id as company_id, Company.Name as company_name, ContactBook.Id as contactbook_id, ContactBook.Name as contactbook_name, Contact.Name, Contact.Phone, Contact.Email, Contact.Address FROM Contact LEFT JOIN Company ON Company.Id = Contact.CompanyId LEFT JOIN ContactBook ON ContactBook.Id = Contact.ContactBookId WHERE Contact.CompanyId = @companyId AND Contact.ContactBookId = @contactBookId";
            var result = await connection.QueryAsync<ContactDao>(query, new { companyId, contactBookId });

            return result?.Select(item => item.Export());
        }


        public async Task<int> RegistersCount(string searchString)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var query = "SELECT COUNT(*) FROM Contact WHERE ((Id LIKE '%' || @searchString || '%') OR (CompanyId LIKE '%' || @searchString || '%') OR (ContactBookId LIKE '%' || @searchString || '%') OR (Name LIKE '%' || @searchString || '%') OR (Phone LIKE '%' || @searchString || '%') OR (Email LIKE '%' || @searchString || '%') OR (Address LIKE '%' || @searchString || '%'))";

            var result = await connection.QueryAsync<int>(query, new { searchString });

            return result.FirstOrDefault();


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
