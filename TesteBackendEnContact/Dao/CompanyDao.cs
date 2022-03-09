using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;

namespace TesteBackendEnContact.Dao
{
    [System.ComponentModel.DataAnnotations.Schema.Table("Company")]
    public class CompanyDao
    {
        [Dapper.Contrib.Extensions.Key]
        public int Id { get; set; }
        [Required]
        public int ContactBookId { get; set; }
        [Write(false)]
        public IContactBook ContactBook { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public CompanyDao()
        {
        }

        public CompanyDao(ICompany company)
        {
            Id = company.Id;
            ContactBookId = company.ContactBookId;
            ContactBook = company.ContactBook;
            Name = company.Name;
        }

        public ICompany Export() => new Company(Id, ContactBookId, ContactBook, Name);
    }
}
