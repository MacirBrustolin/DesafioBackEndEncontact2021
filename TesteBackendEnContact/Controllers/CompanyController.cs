using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;
using TesteBackendEnContact.Repository.Interface;
using TesteBackendEnContact.Wrapers;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;

        public CompanyController(ILogger<CompanyController> logger)
        {
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(SaveCompanyRequest company, [FromServices] ICompanyRepository companyRepository)
        {
            var response = await companyRepository.SaveAsync(company.ToCompany());
            return Ok(new Response<ICompany>(response));
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, SaveCompanyRequest company, [FromServices] ICompanyRepository companyRepository)
        {
            await companyRepository.UpdateAsync(id, company.ToCompany());
            return Ok();
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id, [FromServices] ICompanyRepository companyRepository)
        {
            await companyRepository.DeleteAsync(id);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromServices] ICompanyRepository companyRepository)
        {
            var response = await companyRepository.GetAllAsync();
            return Ok(new Response<IEnumerable<ICompany>>(response));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromServices] ICompanyRepository companyRepository)
        {
            var response = await companyRepository.GetAsync(id);
            return Ok(new Response<ICompany>(response));
        }
    }
}
