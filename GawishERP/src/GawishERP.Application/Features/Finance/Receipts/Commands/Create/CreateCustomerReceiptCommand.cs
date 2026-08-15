using MediatR;

namespace GawishERP.Application.Features.Finance.Receipts.Commands.Create;

public sealed record CreateCustomerReceiptCommand(
    Guid FiscalYearId,
    Guid CustomerId,
    Guid CashAccountId,
    decimal Amount,
    DateTime TransactionDate,
    string ReferenceNumber,
    string? Notes,
    Guid? CompanyId = null,
    Guid? BranchId = null) : IRequest<CreateCustomerReceiptResponse>;
