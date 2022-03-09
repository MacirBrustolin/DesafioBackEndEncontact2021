using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Interface.ContactBook;

namespace TesteBackendEnContact.Resources
{
    public class CompanyResource
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public IContactBook ContactBook { get; set; }
    }
}
