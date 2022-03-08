using System;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;

namespace TesteBackendEnContact.Core.Interface.ContactBook.Contact
{
    public interface IContact
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
