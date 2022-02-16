using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;

namespace TesteBackendEnContact.Repository.Interface
{
    public interface IContactRepository
    {
        Task<IContact> SaveAsync(IContact contact);
        Task UpdateAsync(int id, IContact contact);
        Task<IEnumerable<IContact>> GetAllAsync();
        Task<IEnumerable<IContact>> GetAsync(int pageRows, int pageNumber, string searchString);
        Task<IEnumerable<string>> CompanyList();
        Task<int> RegistersCount(string searchString);
        Task<IEnumerable<IContact>> GetByCompanyAndContactBook(int companyId, int contactBookId);
        Task<IEnumerable<int>> ContactIdList();
    }
}
