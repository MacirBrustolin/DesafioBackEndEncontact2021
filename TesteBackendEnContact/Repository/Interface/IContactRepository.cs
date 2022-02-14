﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface IContactRepository
    {
        Task<IContact> SaveAsync(IContact contact);
        Task<IEnumerable<IContact>> GetAllAsync();
        Task<IEnumerable<IContact>> GetAsync(string searchString);
        Task<IEnumerable<string>> CompanyList();
    }
}
