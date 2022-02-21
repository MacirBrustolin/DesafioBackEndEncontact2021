using CsvHelper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic.FileIO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Domain.ContactBook.Contact;
using TesteBackendEnContact.Core.Interface.ContactBook.Contact;
using TesteBackendEnContact.Filters;
using TesteBackendEnContact.Wrapers;
using TesteBackendEnContact.Repository.Interface;
using CsvHelper.Configuration;
using TesteBackendEnContact.Mapping;
using System.Linq;
using Swashbuckle.AspNetCore.Annotations;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly ILogger<ContactController> _logger;

        public ContactController(ILogger<ContactController> logger)
        {
            _logger = logger;
        }

        [SwaggerResponse(statusCode: 200, description: "Contacts Retrieved successfully")]
        [SwaggerResponse(statusCode: 404, description: "Contacts not found")]
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] IContactRepository contactRepository)
        {
            try
            {
                var response = await contactRepository.GetAllAsync();
                if (response is null)
                {
                    return NotFound();
                }
                return Ok(new Response<IEnumerable<IContact>>(response));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error Retrieving Data.");
            }
        }

        [SwaggerResponse(statusCode: 200, description: "Contacts Retrieved successfully")]
        [SwaggerResponse(statusCode: 404, description: "Contacts not found")]
        [HttpGet("{companyId}, {contactBookId}")]
        public async Task<IActionResult> Get(int companyId, int contactBookId, [FromServices] IContactRepository contactRepository)
        {
            try
            {
                var response = await contactRepository.GetByCompanyAndContactBook(companyId, contactBookId);
                if (response is null)
                {
                    return NotFound();
                }
                return Ok(new Response<IEnumerable<IContact>>(response));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error Retrieving Data.");
            }

        }

        [SwaggerResponse(statusCode: 200, description: "Contacts Retrieved successfully")]
        [SwaggerResponse(statusCode: 404, description: "Contacts not found")]
        [HttpGet("{searchString}")]
        public async Task<IActionResult> Get(int pageRows, int pageNumber, string searchString, [FromServices] IContactRepository contactRepository)
        {
            try
            {
                var validFilter = new PaginationFilter(pageNumber, pageRows);
                var registersCount = contactRepository.RegistersCount(searchString);
                var pagedData = await contactRepository.GetAsync(validFilter.PageSize, validFilter.PageNumber, searchString);

                if (pagedData is null)
                {
                    return NotFound();
                }
                return Ok(new PagedResponse<IEnumerable<IContact>>(pagedData, validFilter.PageNumber, validFilter.PageSize, registersCount.Result));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error Retrieving Data.");
            }
        }

        [SwaggerResponse(statusCode: 201, description: "Success creating the new contacts")]
        [SwaggerResponse(statusCode: 400, description: "Failed to create the new contacts")]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file, [FromServices] IContactRepository contactRepository)
        {
            try
            {
                var contatos = new List<Contact>();

                var fileextension = Path.GetExtension(file.FileName);
                var filename = Guid.NewGuid().ToString() + fileextension;
                var filepath = Path.Combine(Directory.GetCurrentDirectory(), filename);

                using (FileStream fs = System.IO.File.Create(filepath))
                {
                    file.CopyTo(fs);
                }

                using (var reader = new StreamReader(filepath))
                {

                    var csvConfig = new CsvConfiguration(CultureInfo.InvariantCulture);
                    using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                    csv.Context.RegisterClassMap<ContactMap>();
                    var records = csv.GetRecords<ContactCsv>();

                    if (records == null)
                    {
                        return BadRequest();
                    }

                    int companyIdAux = 0;
                    var companyList = await contactRepository.CompanyList();
                    var contactIdEnum = await contactRepository.ContactIdList();
                    var contactIdList = contactIdEnum.ToList();

                    foreach (var record in records)
                    {
                        foreach (var company in companyList)
                        {
                            if (Convert.ToInt32(company) == record.CompanyId)
                            {
                                companyIdAux = record.CompanyId;
                                break;
                            }
                            else
                            {
                                companyIdAux = 0;
                            }
                        }
                        contatos.Add(new Contact(record.Id,
                                                 record.ContactBookId,
                                                 companyIdAux,
                                                 record.Name,
                                                 record.Phone,
                                                 record.Email,
                                                 record.Address));
                    }

                    foreach (var contato in contatos)
                    {
                        if (contactIdList.Contains(contato.Id))
                        {
                            if (contato.ContactBookId > 0)
                            {
                                await contactRepository.UpdateAsync(contato.Id, contato);
                            }
                        }
                        else
                        {
                            if (contato.ContactBookId > 0)
                            {
                                await contactRepository.SaveAsync(contato);
                            }

                        }
                    }
                }
                System.IO.File.Delete(filepath);

                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error Uploading Data.");
            }
            
        }
    }
}
