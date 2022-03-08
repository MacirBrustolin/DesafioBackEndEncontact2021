using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
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

        [SwaggerResponse(statusCode: 201, description: "Success creating a new contact book")]
        [SwaggerResponse(statusCode: 400, description: "Failed to create a new contact book")]
        [HttpPost]
        public async Task<IActionResult> Post(SaveContactBookRequest contactBook, [FromServices] IContactBookRepository contactBookRepository)
        {
            try
            {
                if (contactBook == null)
                {
                    return BadRequest();
                }

                var response = await contactBookRepository.SaveAsync(contactBook.ToContactBook());
                return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error Creating Data. {ex.Message}");
            }
        }

        [SwaggerResponse(statusCode: 200, description: "Contact Book Updated successfully")]
        [SwaggerResponse(statusCode: 404, description: "Contact Book not found")]
        [HttpPut]
        public async Task<IActionResult> Update(int id, SaveContactBookRequest contactBook, [FromServices] IContactBookRepository contactBookRepository)
        {
            try
            {
                if (id != contactBook.Id)
                {
                    return BadRequest("Company ID mismatch");
                }

                var contackBookToUpdate = await contactBookRepository.GetAsync(id);
                if (contackBookToUpdate == null)
                {
                    return NotFound($"Company with Id = {id} not found");
                }

                await contactBookRepository.UpdateAsync(id, contactBook.ToContactBook());
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error Updating Data. {ex.Message}");
            }
        }

        [SwaggerResponse(statusCode: 200, description: "Contact Book Deleted successfully")]
        [SwaggerResponse(statusCode: 404, description: "Contact Book not found")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id, [FromServices] IContactBookRepository contactBookRepository)
        {
            try
            {
                var contactBookToDelete = await contactBookRepository.GetAsync(id);
                if (contactBookToDelete is null)
                {
                    return NotFound();
                }
                await contactBookRepository.DeleteAsync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error Deleting Data. {ex.Message}");
            }
        }

        [SwaggerResponse(statusCode: 200, description: "Contact Books Retrieved successfully")]
        [SwaggerResponse(statusCode: 404, description: "Contact Books not found")]
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] IContactBookRepository contactBookRepository)
        {
            try
            {
                var response = await contactBookRepository.GetAllAsync();
                if (response is null)
                {
                    return NotFound();
                }
                return Ok(new Response<IEnumerable<IContactBook>>(response));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error Retrieving Data. {ex.Message}");
            }
        }

        [SwaggerResponse(statusCode: 200, description: "Contact Book Retrieved successfully")]
        [SwaggerResponse(statusCode: 404, description: "Contact Book not found")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromServices] IContactBookRepository contactBookRepository)
        {
            try
            {
                var response = await contactBookRepository.GetAsync(id);
                if (response is null)
                {
                    return NotFound();
                }
                return Ok(new Response<IContactBook>(response));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error Retrieving Data. {ex.Message}");
            }
        }
    }
}
