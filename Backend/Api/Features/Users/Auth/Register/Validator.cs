using FluentValidation;

namespace Api.Features.Users.Auth.Register
{
    public class Validator : AbstractValidator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .Matches(@"^[A-Za-z][A-Za-z0-9]*$")
                .WithMessage("Username must start with a letter, contain only letters and numbers")
                .MinimumLength(8)
                .MaximumLength(24);

            RuleFor(x => x.Password)
               .NotEmpty()
               .Matches(@"^[A-Za-z0-9]*$")
               .WithMessage("Password must contain only letters and numbers")
               .MinimumLength(8)
               .MaximumLength(24);

            RuleFor(x => x.Firstname)
                .NotEmpty()
                .Matches(@"^[A-Za-z]*$")
                .MinimumLength(1)
                .MaximumLength(32);

            RuleFor(x => x.Lastname)
                .NotEmpty()
                .Matches(@"^[A-Za-z]*$")
                .MinimumLength(1)
                .MaximumLength(32);

            RuleFor(x => x.PhoneNumber)
                .NotEmpty()
                .Matches(@"^\d*$")
                .Length(10);
        }
    }
}
