using FluentValidation;
using ReviewEverything.Shared.Contracts.Requests;

namespace ReviewEverything.Server.Validators
{
    public class CategoryRequestValidator : AbstractValidator<CategoryRequest>
    {
        public CategoryRequestValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty();
        }
    }
}