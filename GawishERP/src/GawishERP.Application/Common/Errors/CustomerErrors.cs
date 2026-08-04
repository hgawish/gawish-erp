using GawishERP.Application.Common.Results;

namespace GawishERP.Application.Common.Errors;

public static class CustomerErrors
{
    public static Error NotFound(Guid id)
    {
        return new Error(
            "Customer.NotFound",
            $"Customer with Id '{id}' was not found.",
            ErrorType.NotFound);
    }

    public static Error DuplicateCode(string code)
    {
        return new Error(
            "Customer.DuplicateCode",
            $"Customer code '{code}' already exists.",
            ErrorType.Conflict);
    }

    public static Error NameRequired()
    {
        return new Error(
            "Customer.NameRequired",
            "Customer name is required.",
            ErrorType.Validation);
    }
}