using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TesteBackendEnContact.Core.Interface.ContactBook.Contact
{
    public interface IContactCsv
    {
        int Id { get; }
        int ContactBookId { get; }
        int CompanyId { get; }
        string Name { get; }
        string Phone { get; }
        string Email { get; }
        string Address { get; }
    }
}
