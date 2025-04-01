using Play.Customer.Service.Dtos;
using Play.Customer.Service.Entities;

namespace Play.Customer.Service;

public static class Extensions
{
    public static CustomerDto AsDto(this CustomerEntity customer)
    {
        return new CustomerDto(
            customer.Id,
            customer.Name,
            customer.ContactNumber,
            customer.Email,
            customer.Address,
            customer.CreatedDate
        );
    }
}
