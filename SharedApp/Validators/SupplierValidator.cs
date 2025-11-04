using FluentValidation;
using SharedApp.Models;
using SharedApp.Dto;

namespace SharedApp.Validators
{
    public class SupplierValidator : AbstractValidator<Supplier>
    {
        public SupplierValidator()
        {
            RuleFor(s => s.Name)
                .NotEmpty().WithMessage("Supplier name is required")
                .MaximumLength(100);

            RuleFor(s => s.Location)
                .MaximumLength(200);

            RuleFor(s => s.Email)
                .NotEmpty().WithMessage("Email is required")
                .EmailAddress().WithMessage("Invalid email format")
                .MaximumLength(100);
        }
    }

    // Validator for creating a new supplier
    public class SupplierCreateDtoValidator : AbstractValidator<SupplierCreateDto>
    {
        public SupplierCreateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Supplier name is required.")
                .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(100).WithMessage("Location cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100);
        }
    }

    // Validator for updating a supplier
    public class SupplierUpdateDtoValidator : AbstractValidator<SupplierUpdateDto>
    {
        public SupplierUpdateDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Supplier name is required.")
                .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters.");

            RuleFor(x => x.Location)
                .NotEmpty().WithMessage("Location is required.")
                .MaximumLength(100).WithMessage("Location cannot exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100);
        }
    }

    // Validator for partial updates (PATCH)
    public class SupplierPatchDtoValidator : AbstractValidator<SupplierPatchDto>
    {
        public SupplierPatchDtoValidator()
        {
            // Name is optional but if provided, validate
            RuleFor(x => x.Name)
                .MaximumLength(100).WithMessage("Supplier name cannot exceed 100 characters.")
                .When(x => x.Name != null);

            // Location is optional but if provided, validate
            RuleFor(x => x.Location)
                .MaximumLength(100).WithMessage("Location cannot exceed 100 characters.")
                .When(x => x.Location != null);

            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(100)
                .When(x => x.Email != null);
        }
    }
}