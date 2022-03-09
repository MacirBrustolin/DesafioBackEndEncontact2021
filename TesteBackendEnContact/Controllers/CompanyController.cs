using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TesteBackendEnContact.Controllers.Models;
using TesteBackendEnContact.Core.Domain.ContactBook.Company;
using TesteBackendEnContact.Core.Interface.ContactBook.Company;
using TesteBackendEnContact.Repository.Interface;
using TesteBackendEnContact.Resources;
using TesteBackendEnContact.Wrapers;

namespace TesteBackendEnContact.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CompanyController : ControllerBase
    {
        private readonly ILogger<CompanyController> _logger;
        private readonly IMapper _mapper;

        public CompanyController(ILogger<CompanyController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        [SwaggerResponse(statusCode: 201, description: "Success creating a company")]
        [SwaggerResponse(statusCode: 400, description: "Failed to create a new company")]
        [HttpPost]
        public async Task<IActionResult> Post(SaveCompanyRequest resource, [FromServices] ICompanyRepository companyRepository)
        {
            try
            {
                var company = _mapper.Map<SaveCompanyRequest, Company>(resource);

                if (company == null)
                {
                    return BadRequest();
                }

                var response = await companyRepository.SaveAsync(company);
                return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error Creating Data. {ex.Message}");
            }
            
        }

        [SwaggerResponse(statusCode: 200, description: "Company Updated successfully")]
        [SwaggerResponse(statusCode: 404, description: "Company not found")]
        [HttpPut]
        public async Task<IActionResult> Update(int id, SaveCompanyRequest resource, [FromServices] ICompanyRepository companyRepository)
        {
            try
            {
                var company = _mapper.Map<SaveCompanyRequest, Company>(resource);

                if (id != company.Id)
                {
                    return BadRequest("Company ID mismatch");
                }

                var companyToUpdate = await companyRepository.GetAsync(id);
                if (companyToUpdate == null)
                {
                    return NotFound($"Company with Id = {id} not found");
                }

                await companyRepository.UpdateAsync(id, company);
                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error Updating Data. {ex.Message}");
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
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error Deleting Data. {ex.Message}");
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
                var resource = _mapper.Map<IEnumerable<ICompany>, IEnumerable<CompanyResource>>(response);
                return Ok(new Response<IEnumerable<CompanyResource>>(resource));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error Retrieving Data. {ex.Message}");
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
                var resource = _mapper.Map<ICompany, CompanyResource>(response);
                return Ok(new Response<CompanyResource>(resource));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    $"Error Retrieving Data. {ex.Message}");
            }
            
        }
    }
}
