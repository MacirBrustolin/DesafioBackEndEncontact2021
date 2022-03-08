using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;

namespace TesteBackendEnContact.Core.Interface.ContactBook.Contact
{
    public interface IContactCsv
    {
        int Id { get; }
        int ContactBookId { get; }
        IContactBook ContactBook { get; }
        int CompanyId { get; }
        ICompany Company { get; }
        string Name { get; }
        string Phone { get; }
        string Email { get; }
        string Address { get; }
    }
}
