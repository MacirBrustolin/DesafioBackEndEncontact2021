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

        [HttpGet]
        public async Task<IEnumerable<IContact>> Get([FromServices] IContactRepository contactRepository)
        {
            return await contactRepository.GetAllAsync();
        }

        [HttpGet("{searchString}")]
        public async Task<IActionResult> Get(int pageRows, int pageNumber, string searchString, [FromServices] IContactRepository contactRepository)
        {
            var validFilter = new PaginationFilter(pageNumber, pageRows);
            var registersCount = contactRepository.RegistersCount(searchString);
            var pagedData = await contactRepository.GetAsync(validFilter.PageSize, validFilter.PageNumber, searchString);

            return Ok(new PagedResponse<IEnumerable<IContact>>(pagedData, validFilter.PageNumber, validFilter.PageSize, registersCount.Result));
        }

        [HttpPost]
        public async Task<ActionResult<IContact>> UploadFile(IFormFile file, [FromServices] IContactRepository contactRepository, [FromServices] IContactBookRepository contactBookRepository)
        {
            var csvData = new DataTable();
            try
            {
                using var csvReader = new TextFieldParser(file.OpenReadStream());
                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;
                string[] colFields = csvReader.ReadFields();
                foreach (string column in colFields)
                {
                    DataColumn datecolumn = new(column)
                    {
                        AllowDBNull = true
                    };
                    csvData.Columns.Add(datecolumn);
                }
                while (!csvReader.EndOfData)
                {
                    string[] fieldData = csvReader.ReadFields();
                    //Making empty value as null
                    for (int i = 0; i < fieldData.Length; i++)
                    {
                        if (fieldData[i] == "")
                        {
                            fieldData[i] = null;
                        }
                    }
                    csvData.Rows.Add(fieldData);
                }

                var contatos = new List<Contact>();
                var companyList = await contactRepository.CompanyList();

                

                for (int i = 0; i < csvData.Rows.Count; i++)
                {
                    int companyIdAux = 0;

                    foreach (var company in companyList)
                    { 
                        if (company.Equals(csvData.Rows[i].ItemArray[2].ToString()))
                        {
                            companyIdAux = int.Parse(csvData.Rows[i].ItemArray[2].ToString());
                            break;
                        }
                        else
                        {
                            companyIdAux = 0;
                        }
                    }

                    contatos.Add(new Contact(int.Parse(csvData.Rows[i].ItemArray[0].ToString()),
                                            int.Parse(csvData.Rows[i].ItemArray[1].ToString()),
                                            companyIdAux,
                                            csvData.Rows[i].ItemArray[3].ToString(),
                                            csvData.Rows[i].ItemArray[4].ToString(),
                                            csvData.Rows[i].ItemArray[5].ToString(),
                                            csvData.Rows[i].ItemArray[6].ToString()
                                            ));

                }

                foreach (var contato in contatos)
                {
                    await contactRepository.SaveAsync(contato);
                }
                //Id,ContactBookId,CompanyId,Name,Phone,Email,Address
            }
            catch (Exception ex)
            {
                return null;
            }
            return Ok();
        }
    }
}
