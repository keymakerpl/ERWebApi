using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERWebApi.Models;
using AutoMapper;
using ERWebApi.SQLDataAccess.Repositories;
using Microsoft.AspNetCore.JsonPatch;
using ERService.Business;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using ERWebApi.Filters;
using Microsoft.Extensions.Caching.Memory;

namespace ERWebApi.Controllers
{
    [ApiController]
    [Authorize]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/customers")]
    [Produces("application/json", "application/xml")]
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public class CustomersController : ControllerBase
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
    {
        private readonly ICustomersRepository _customersRepository;
        private readonly IMapper _mapper;
        private readonly IMemoryCache _memoryCache;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
        public CustomersController(ICustomersRepository customersRepository, IMapper mapper, IMemoryCache memoryCache)
        {
            _customersRepository = customersRepository;
            _mapper = mapper;
            _memoryCache = memoryCache;
        }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        // GET: api/Customers
        /// <summary>
        /// Get customers list.
        /// </summary>
        /// <param name="page">Page number</param>
        /// <param name="perPage">Items per page</param>
        /// <returns>Customers list</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesDefaultResponseType]
        [CustomersResultFilter]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers([FromQuery] int page = 1, [FromQuery] int perPage = 10)
        {
            var cacheKey = nameof(GetCustomers);
            IEnumerable<Customer> customers;

            if (!_memoryCache.TryGetValue(cacheKey, out customers))
            {
                // if not found in cache, fetch from repo
                customers = await _customersRepository.GetEntitiesAsync(page, perPage);

                // add to cache
                _memoryCache.Set(cacheKey, customers, new MemoryCacheEntryOptions()
                    .SetAbsoluteExpiration(TimeSpan.FromMinutes(1)));
            }

            return Ok(customers);
        }

        // GET: api/Customers/5
        /// <summary>
        /// Get customer by specific Id
        /// </summary>
        /// <param name="customerId">Id of customer</param>
        /// <returns>Customer</returns>
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        [HttpGet("{customerId}", Name = nameof(GetCustomer))]
        [CustomerResultFilter]
        public async Task<ActionResult<CustomerDto>> GetCustomer(Guid customerId)
        {
            var customer = await _customersRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            return Ok(customer);
        }

        // PUT: api/Customers/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Update customer, overriding all properties.
        /// </summary>
        /// <param name="customerId">Id of customer to update.</param>
        /// <param name="customerToUpdate">Customer data to update.</param>
        /// <returns>204 No content</returns>
        [HttpPut("{customerId}")]
        [Consumes("application/json")] // określa media type jaki może przyjąć akcja w body        
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdateCustomer(Guid customerId, CustomerForUpdateDto customerToUpdate)
        {
            var customerFromRepo = _customersRepository.GetById(customerId);
            if (customerFromRepo == null)
            {
                return NotFound();
            }

            // can be upserting?

            _mapper.Map(customerToUpdate, customerFromRepo);
            await _customersRepository.SaveAsync();

            return NoContent();
        }

        /// <summary>
        /// Partially update customer with JsonPatch method.
        /// </summary>
        /// <param name="customerId">Id of customer to update.</param>
        /// <param name="patchDocument">JsonPatch Document.</param>
        /// <returns>204 No content</returns>
        /// <remarks>
        /// Sample request \
        /// [ \
        ///    { \
        ///      "op": "replace", \
        ///      "path": "/houseNumber", \
        ///      "value": "33/5" \
        ///    }, \
        ///    { \
        ///      "op": "replace", \
        ///      "path": "/postcode", \
        ///      "value": "42-400" \
        ///    } \
        /// ]
        /// </remarks>
        [HttpPatch("{customerId}")]
        [Consumes("application/json")] // określa media type jaki może przyjąć akcja w body        
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> PartiallyUpdateCustomer(Guid customerId, JsonPatchDocument<CustomerForUpdateDto> patchDocument)
        {
            var customerFromRepo = _customersRepository.GetById(customerId);
            if (customerFromRepo == null)
            {
                return NotFound();
            }

            // can be upserting?

            var customerToPatch = _mapper.Map<CustomerForUpdateDto>(customerFromRepo);
            patchDocument.ApplyTo(customerToPatch, ModelState);
            if (!TryValidateModel(customerToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(customerToPatch, customerFromRepo);
            await _customersRepository.SaveAsync();

            return NoContent();
        }

        // POST: api/Customers
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        /// <summary>
        /// Create new customer with data from body.
        /// </summary>
        /// <param name="customer">Customer data to add.</param>
        /// <returns>201 Created</returns>
        [HttpPost]
        [Consumes("application/json")] // określa media type jaki może przyjąć akcja w body
        [ProducesResponseType(StatusCodes.Status201Created)]        
        [ProducesDefaultResponseType]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CustomerForCreationDto customer)
        {
            var customerToAdd = _mapper.Map<Customer>(customer);
            _customersRepository.Add(customerToAdd);
            await _customersRepository.SaveAsync();

            var customerToReturn = _mapper.Map<CustomerDto>(customerToAdd);

            return CreatedAtAction(nameof(GetCustomer), new { customerId = customerToReturn.Id }, customerToReturn);
        }

        // DELETE: api/Customers/5
        /// <summary>
        /// Delete customer of specified id from database.
        /// </summary>
        /// <param name="customerId">Id of customer.</param>
        /// <returns>200 Ok</returns>
        [HttpDelete("{id}")]
        [Consumes("application/json")] // określa media type jaki może przyjąć akcja w body
        [CustomerResultFilter]
        [ProducesDefaultResponseType]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> DeleteCustomer(Guid customerId)
        {
            var customer = await _customersRepository.GetByIdAsync(customerId);
            if (customer == null)
            {
                return NotFound();
            }

            _customersRepository.Remove(customer);
            await _customersRepository.SaveAsync();

            return Ok(customer);
        }
    }
}
