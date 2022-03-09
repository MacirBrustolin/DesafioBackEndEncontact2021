using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Resources;

namespace TesteBackendEnContact.Mapping
{
    public class ModelToResourceProfile : Profile
    {
        public ModelToResourceProfile (){

            CreateMap<ICompany, CompanyResource>();

            CreateMap<IContactBook, ContactBookResource>();

            CreateMap<IContact, ContactResource>();
        }
        
    }
}
