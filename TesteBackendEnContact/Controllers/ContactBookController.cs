using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Domain.ContactBook;
using TesteBackendEnContact.Core.Interface.ContactBook;
using TesteBackendEnContact.Repository.Interface;
using TesteBackendEnContact.Wrapers;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactBookController : ControllerBase
    {
        private readonly ILogger<ContactBookController> _logger;

        public ContactBookController(ILogger<ContactBookController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(ContactBook contactBook, [FromServices] IContactBookRepository contactBookRepository)
        {
            var response = await contactBookRepository.SaveAsync(contactBook);
            return Ok(new Response<IContactBook>(response));
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, SaveContactBookRequest contactBook, [FromServices] IContactBookRepository contactBookRepository)
        {
            await contactBookRepository.UpdateAsync(id, contactBook.ToContactBook());
            return Ok();
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id, [FromServices] IContactBookRepository contactBookRepository)
        {
            await contactBookRepository.DeleteAsync(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] IContactBookRepository contactBookRepository)
        {
            var response = await contactBookRepository.GetAllAsync();
            return Ok(new Response<IEnumerable<IContactBook>>(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromServices] IContactBookRepository contactBookRepository)
        {
            var response = await contactBookRepository.GetAsync(id);
            return Ok(new Response<IContactBook>(response));
        }
    }
}
