using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ERService.Business;
using ERWebApi.SQLDataAccess.Repositories;
using AutoMapper;
using ERWebApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;

namespace ERWebApi.Controllers
{
    [Produces("application/json", "application/xml")]
    [Route("api/customers/{customerId}/addresses")]
    [ApiController]
    public class AddressesController : ControllerBase
    {
        private readonly ICustomersRepository _customerRepository;
        private readonly IMapper _mapper;

        public AddressesController(ICustomersRepository customerRepository, IMapper mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        // GET: api/Addresses
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<CustomerAddressDto>>> GetCustomerAddresses(Guid customerId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                return NotFound();

            return Ok(_mapper.Map<IEnumerable<CustomerAddressDto>>(customer.CustomerAddresses));
        }

        // GET: api/Addresses/5
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CustomerAddressDto))]
        [HttpGet("{addressId}", Name = nameof(GetCustomerAddress))]
        public async Task<ActionResult<CustomerAddressDto>> GetCustomerAddress(Guid customerId, Guid addressId)
        {
            var customer = await _customerRepository.GetByIdAsync(customerId);
            if (customer == null)
                return NotFound();

            var address = customer.CustomerAddresses.FirstOrDefault(address => address.Id == addressId);
            if (address == null)
                return NotFound();

            return Ok(_mapper.Map<CustomerAddressDto>(address));
        }

        // PUT: api/Addresses/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{addressId}")]
        public async Task<IActionResult> UpdateCustomerAddress(
            Guid customerId,
            Guid addressId,
            CustomerAddressForUpdateDto customerAddress)
        {
            var customerFromRepo = await _customerRepository.GetByIdAsync(customerId);
            if (customerFromRepo == null)
                return NotFound();

            var addressFromRepo = customerFromRepo.CustomerAddresses.FirstOrDefault(address => address.Id == addressId);
            if (addressFromRepo == null)
                return NotFound();

            _mapper.Map(customerAddress, addressFromRepo);
            await _customerRepository.SaveAsync();

            return NoContent();
        }

        [HttpPatch("{addressId}")]
        public async Task<IActionResult> PartialyUpdateCustomerAddress(Guid customerId,
                                                                       Guid addressId,
                                                                       JsonPatchDocument<CustomerAddressForUpdateDto> patchDocument)
        {
            var customerFromRepo = await _customerRepository.GetByIdAsync(customerId);
            if (customerFromRepo == null)
                return NotFound();

            var addressFromRepo = customerFromRepo.CustomerAddresses.FirstOrDefault(address => address.Id == addressId);
            if (addressFromRepo == null)
                return NotFound();

            var addressToPatch = _mapper.Map<CustomerAddressForUpdateDto>(addressFromRepo);
            patchDocument.ApplyTo(addressToPatch, ModelState);
            if (!TryValidateModel(addressToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(addressToPatch, addressFromRepo);
            await _customerRepository.SaveAsync();

            return NoContent();
        }

        // POST: api/Addresses
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<CustomerAddress>> CreateAddressForCustomer(
            Guid customerId,
            CustomerAddressForCreateDto customerAddress)
        {
            var customerFromRepo = await _customerRepository.GetByIdAsync(customerId);
            if (customerFromRepo == null)
                return NotFound();

            var newAddress = _mapper.Map<CustomerAddress>(customerAddress);
            customerFromRepo.CustomerAddresses.Add(newAddress);
            await _customerRepository.SaveAsync();

            var addressToReturn = _mapper.Map<CustomerAddressDto>(newAddress);
            return CreatedAtAction(
                nameof(GetCustomerAddress),
                new { customerId = customerId, addressId = addressToReturn.Id, },
                addressToReturn);
        }

        // DELETE: api/Addresses/5
        [HttpDelete("{addressId}")]
        public async Task<ActionResult<CustomerAddressDto>> DeleteCustomerAddress(Guid customerId, Guid addressId)
        {
            var customerFromRepo = await _customerRepository.GetByIdAsync(customerId);
            if (customerFromRepo == null)
                return NotFound();

            var addressFromRepo = customerFromRepo.CustomerAddresses.FirstOrDefault(address => address.Id == addressId);
            if (addressFromRepo == null)
                return NotFound();

            customerFromRepo.CustomerAddresses.Remove(addressFromRepo);
            await _customerRepository.SaveAsync();

            return NoContent();
        }
    }
}
