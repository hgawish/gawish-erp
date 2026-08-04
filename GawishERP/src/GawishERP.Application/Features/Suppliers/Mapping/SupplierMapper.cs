using GawishERP.Application.Features.Suppliers.DTOs;
using GawishERP.Domain.Entities;

namespace GawishERP.Application.Common.Mapping;

public static class SupplierMapper
{
    public static SupplierDto ToDto(Supplier supplier)
    {
        return new SupplierDto
        {
            Id = supplier.Id,
            Code = supplier.Code,
            Name = supplier.Name,
            ArabicName = supplier.ArabicName,
            ContactPerson = supplier.ContactPerson,
            Phone = supplier.Phone,
            Mobile = supplier.Mobile,
            Email = supplier.Email,
            TaxNumber = supplier.TaxNumber,
            CommercialRegistration = supplier.CommercialRegistration,
            Address = supplier.Address,
            City = supplier.City,
            Country = supplier.Country,
            Notes = supplier.Notes,
            IsActive = supplier.IsActive,

            // ============================================
            // Accounting
            // ============================================

            AccountId = supplier.AccountId,
            AccountCode = supplier.Account?.Code,
            AccountName = supplier.Account?.Name
        };
    }

    public static List<SupplierDto> ToDtoList(
        IEnumerable<Supplier> suppliers)
    {
        return suppliers
            .Select(ToDto)
            .ToList();
    }
}