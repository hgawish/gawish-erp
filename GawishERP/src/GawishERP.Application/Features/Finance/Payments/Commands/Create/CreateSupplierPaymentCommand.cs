using MediatR;

namespace GawishERP.Application.Features.Finance.Payments.Commands.Create;

public sealed record CreateSupplierPaymentCommand(
    Guid FiscalYearId,
    Guid SupplierId,
    Guid CashAccountId,
    decimal Amount,
    DateTime TransactionDate,
    string ReferenceNumber,
    string? Notes,
    Guid? PurchaseId = null,
    Guid? CompanyId = null,
    Guid? BranchId = null) : IRequest<CreateSupplierPaymentResponse>;
