using FluentValidation;
using Orko.Portal.Contracts.WorkOrders;

namespace Orko.Portal.Application.WorkOrders;

public class CreateWorkOrderValidator : AbstractValidator<CreateWorkOrderDto>
{
    public CreateWorkOrderValidator()
    {
        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("Tip zorunludur.")
            .Must(t => t == "Import" || t == "Export")
            .WithMessage("Tip 'Import' veya 'Export' olmalidir.");
    }
}
