using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;

namespace TesteBackendEnContact.Core.Domain.ContactBook.Company
{
    public class Company : ICompany
    {
        public int Id { get; private set; }
        public string Name { get; private set; }
        public int ContactBookId { get; private set; }
        public IContactBook ContactBook { get; set; }

        public Company()
        {

        }

        public Company(int id, int contactBookId, string name)
        {
            Id = id;
            ContactBookId = contactBookId;
            Name = name;
        }
    }
}
