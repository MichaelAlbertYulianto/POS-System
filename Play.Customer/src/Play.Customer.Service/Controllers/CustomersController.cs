using Microsoft.AspNetCore.Mvc;
using Play.Customer.Service.Dtos;
using Play.Customer.Service.Entities;
using Play.Common;

namespace Play.Customer.Service.Controllers;

[ApiController]
[Route("customers")]
public class CustomersController : ControllerBase
{
    private readonly IRepository<CustomerEntity> customersRepository;

    public CustomersController(IRepository<CustomerEntity> customersRepository)
    {
        this.customersRepository = customersRepository;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerDto>>> GetAsync()
    {
        var customers = await customersRepository.GetAllAsync();
        return Ok(customers.Select(c => c.AsDto()));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CustomerDto>> GetByIdAsync(Guid id)
    {
        var customer = await customersRepository.GetAsync(id);
        if (customer == null)
            return NotFound();

        return Ok(customer.AsDto());
    }

    [HttpPost]
    public async Task<ActionResult<CustomerDto>> PostAsync(CreateCustomerDto createCustomerDto)
    {
        var customer = new CustomerEntity
        {
            Name = createCustomerDto.Name,
            ContactNumber = createCustomerDto.ContactNumber,
            Email = createCustomerDto.Email,
            Address = createCustomerDto.Address,
            CreatedDate = DateTimeOffset.UtcNow
        };

        await customersRepository.CreateAsync(customer);

        return CreatedAtAction(nameof(GetByIdAsync), new { id = customer.Id }, customer.AsDto());
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutAsync(Guid id, UpdateCustomerDto updateCustomerDto)
    {
        var existingCustomer = await customersRepository.GetAsync(id);
        if (existingCustomer == null)
            return NotFound();

        existingCustomer.Name = updateCustomerDto.Name;
        existingCustomer.ContactNumber = updateCustomerDto.ContactNumber;
        existingCustomer.Email = updateCustomerDto.Email;
        existingCustomer.Address = updateCustomerDto.Address;

        await customersRepository.UpdateAsync(existingCustomer);

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(Guid id)
    {
        var customer = await customersRepository.GetAsync(id);
        if (customer == null)
            return NotFound();

        await customersRepository.DeleteAsync(id);

        return NoContent();
    }
}
