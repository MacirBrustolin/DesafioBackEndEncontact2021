using System.ComponentModel.DataAnnotations;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;

namespace TesteBackendEnContact.Controllers.Models
{
    public class SaveContactRequest
    {
        public int Id { get; set; }
        [Required]
        public int ContactBookId { get; set; }
        public int CompanyId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(20)]
        public string Phone { get; set; }
        [StringLength(50)]
        public string Email { get; set; }
        [StringLength(100)]
        public string Address { get; set; }

        public IContact ToContact() => new Contact(Id, ContactBookId, CompanyId, Name, Phone, Email, Address);

    }
}
