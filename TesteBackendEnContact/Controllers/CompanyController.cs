using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
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

        [SwaggerResponse(statusCode: 201, description: "Success creating a company")]
        [SwaggerResponse(statusCode: 400, description: "Failed to create a new company")]
        [HttpPost]
        public async Task<IActionResult> Post(SaveCompanyRequest company, [FromServices] ICompanyRepository companyRepository)
        {
            try
            {
                if (company == null)
                {
                    return BadRequest();
                }

                var response = await companyRepository.SaveAsync(company.ToCompany());
                return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error Creating Data.");
            }
            
        }

        [SwaggerResponse(statusCode: 200, description: "Company Updated successfully")]
        [SwaggerResponse(statusCode: 404, description: "Company not found")]
        [HttpPut]
        public async Task<IActionResult> Update(int id, SaveCompanyRequest company, [FromServices] ICompanyRepository companyRepository)
        {
            try
            {
                if (id != company.Id)
                {
                    return BadRequest("Company ID mismatch");
                }

                var companyToUpdate = await companyRepository.GetAsync(id);
                if (companyToUpdate == null)
                {
                    return NotFound($"Company with Id = {id} not found");
                }

                await companyRepository.UpdateAsync(id, company.ToCompany());
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error Updating Data.");
            }
            
        }

        [SwaggerResponse(statusCode: 200, description: "Company Deleted successfully")]
        [SwaggerResponse(statusCode: 404, description: "Company not found")]
        [HttpDelete]
        public async Task<IActionResult> Delete(int id, [FromServices] ICompanyRepository companyRepository)
        {
            try
            {
                var companyToDelete = await companyRepository.GetAsync(id);
                if (companyToDelete is null)
                {
                    return NotFound();
                }
                await companyRepository.DeleteAsync(id);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error Deleting Data.");
            }
            
        }

        [SwaggerResponse(statusCode: 200, description: "Companies Retrieved successfully")]
        [SwaggerResponse(statusCode: 404, description: "Companies not found")]
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] ICompanyRepository companyRepository)
        {
            try
            {
                var response = await companyRepository.GetAllAsync();
                if (response is null)
                {
                    return NotFound();
                }
                return Ok(new Response<IEnumerable<ICompany>>(response));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error Retrieving Data.");
            }
            
        }

        [SwaggerResponse(statusCode: 200, description: "Company Retrieved successfully")]
        [SwaggerResponse(statusCode: 404, description: "Company not found")]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, [FromServices] ICompanyRepository companyRepository)
        {
            try
            {
                var response = await companyRepository.GetAsync(id);
                if (response is null)
                {
                    return NotFound();
                }
                return Ok(new Response<ICompany>(response));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error Retrieving Data.");
            }
            
        }
    }
}
