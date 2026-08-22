using FluentValidation;

namespace CleanArchitecture.Full.Application.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    private static readonly string[] AllowedDocumentTypes = ["DNI", "Pasaporte", "CUIT"];

    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.DocumentType).NotEmpty().Must(t => AllowedDocumentTypes.Contains(t))
            .WithMessage($"DocumentType debe ser uno de: {string.Join(", ", AllowedDocumentTypes)}.");
        RuleFor(x => x.DocumentNumber).NotEmpty().MaximumLength(20);
        RuleFor(x => x.FullName).NotEmpty().MaximumLength(150);
    }
}
