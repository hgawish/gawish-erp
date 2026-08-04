using GawishERP.Application.Features.Customers.DTOs;
using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Mapping;

public static class CustomerMapper
{
    public static CustomerDto ToDto(Customer customer)
    {
        return new CustomerDto
        {
            Id = customer.Id,
            Code = customer.Code,
            Name = customer.Name,
            ArabicName = customer.ArabicName,
            Phone = customer.Phone,
            Email = customer.Email,
            Address = customer.Address,
            Notes = customer.Notes,
            IsActive = customer.IsActive,

            // ============================================
            // Accounting
            // ============================================

            AccountId = customer.AccountId,
            AccountCode = customer.Account?.Code,
            AccountName = customer.Account?.Name
        };
    }

    public static List<CustomerDto> ToDtoList(IEnumerable<Customer> customers)
    {
        return customers
            .Select(ToDto)
            .ToList();
    }
}