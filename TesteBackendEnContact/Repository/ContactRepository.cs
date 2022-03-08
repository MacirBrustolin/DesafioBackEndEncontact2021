using CsvHelper;
using CsvHelper.Configuration;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Dao;
using TesteBackendEnContact.Database;
using TesteBackendEnContact.Mapping;
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

        public async Task UpdateAsync(int id, IContact contact)
        {
            var dao = new ContactDao(contact) { Id = id };

            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT * FROM Contact where Id = @id";
            var result = await connection.QuerySingleOrDefaultAsync<ContactDao>(query, new { id });

            await connection.UpdateAsync(dao);
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
            var result = await connection.QueryAsync<ContactDao>(query, new { pageRows, page = pageRows * pageNumber, searchString });

            return result?.Select(item => item.Export());
        }

        public async Task<IEnumerable<IContact>> GetByCompanyAndContactBook(int companyId, int contactBookId)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var query = "SELECT Contact.Id, Contact.CompanyId, Contact.ContactBookId, Company.Id as company_id, Company.Name as company_name, ContactBook.Id as contactbook_id, ContactBook.Name as contactbook_name, Contact.Name, Contact.Phone, Contact.Email, Contact.Address FROM Contact " +
                "LEFT JOIN Company ON Company.Id = Contact.CompanyId " +
                "LEFT JOIN ContactBook ON ContactBook.Id = Contact.ContactBookId " +
                "WHERE Contact.CompanyId = @companyId AND Contact.ContactBookId = @contactBookId";
            var result = await connection.QueryAsync<ContactDao>(query, new { companyId, contactBookId });

            return result?.Select(item => item.Export());
        }


        public async Task<int> RegistersCount(string searchString)
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);
            var query = "SELECT COUNT(*) FROM Contact WHERE ((Id LIKE '%' || @searchString || '%') OR (CompanyId LIKE '%' || @searchString || '%') OR (ContactBookId LIKE '%' || @searchString || '%') OR (Name LIKE '%' || @searchString || '%') OR (Phone LIKE '%' || @searchString || '%') OR (Email LIKE '%' || @searchString || '%') OR (Address LIKE '%' || @searchString || '%'))";

            var result = await connection.QuerySingleOrDefaultAsync<int>(query, new { searchString });

            return result;
        }

        public async Task<List<int>> ContactIdList()
        {
            using var connection = new SqliteConnection(databaseConfig.ConnectionString);

            var sql = "SELECT DISTINCT Id FROM Contact";

            var contactsIds = await connection.QueryAsync<int>(sql);

            return contactsIds.ToList();
        }

        public async Task<List<ContactCsv>> GetDataFromCSVFile(IFormFile file)
        {
            using var memoryStream = new MemoryStream(new byte[file.Length]);
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var reader = new StreamReader(memoryStream);

            var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            csv.Context.RegisterClassMap<ContactMap>();
            var records = csv.GetRecords<ContactCsv>();

            return records.ToList();
        }

        public async Task UploadFile(List<ContactCsv> records, List<int> companyList, List<int> contactIdList)
        {
            var contatos = new List<Contact>();
            int companyIdAux = 0;

            foreach (var record in records)
            {
                foreach (var company in companyList)
                {
                    if (Convert.ToInt32(company) == record.CompanyId)
                    {
                        companyIdAux = record.CompanyId;
                        break;
                    }
                    else
                    {
                        companyIdAux = 0;
                    }
                }
                contatos.Add(new Contact(record.Id,
                                         record.ContactBookId,
                                         companyIdAux,
                                         record.Name,
                                         record.Phone,
                                         record.Email,
                                         record.Address));
            }

            foreach (var contato in contatos)
            {
                if (contactIdList.Contains(contato.Id))
                {
                    if (contato.ContactBookId > 0)
                    {
                        await UpdateAsync(contato.Id, contato);
                    }
                }
                else
                {
                    if (contato.ContactBookId > 0)
                    {
                        await SaveAsync(contato);
                    }

                }
            }
        }
    }
}

