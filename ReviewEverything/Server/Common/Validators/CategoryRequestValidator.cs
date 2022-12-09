using FluentValidation;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Common.Validators
{
    public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
    {
        public CategoryRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty()
                .MinimumLength(3);
        }
    }
}