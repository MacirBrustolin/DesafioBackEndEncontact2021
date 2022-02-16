using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;

namespace TesteBackendEnContact.Mapping
{
    public class ContactMap : ClassMap<ContactCsv>
    {
        public ContactMap()
        {
            Map(x => x.Id).Name("Id");
            Map(x => x.ContactBookId).Name("ContactBookId");
            Map(x => x.CompanyId).Name("CompanyId");
            Map(x => x.Name).Name("Name");
            Map(x => x.Phone).Name("Phone");
            Map(x => x.Email).Name("Email");
            Map(x => x.Address).Name("Address");

        }
    }
}
