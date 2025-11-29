using Core.Models.Location.Country;
using Domain;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
namespace Core.Validators.Country;
public class CountryCreateValidator : AbstractValidator<CountryCreateModel>
{
	public CountryCreateValidator(AppDbTransferContext db)
	{
		RuleFor(x => x.Name)
			.NotEmpty().WithMessage("Назва країни не може бути порожньою")
			.MaximumLength(100).WithMessage("Назва країни не може перевищувати 100 символів")
			.DependentRules(() =>
			{
				RuleFor(x => x.Name)
					.MustAsync(async (name, cancellation) =>
						!await db.Countries.AnyAsync(c => c.Name.ToLower() == name.ToLower().Trim(), cancellation))
					.WithMessage("Країна з такою назвою вже існує");
			});

		RuleFor(x => x.Code)
			.NotEmpty().WithMessage("Код країни не може бути порожнім")
			.MaximumLength(10).WithMessage("Код країни не може перевищувати 10 символів")
			.DependentRules(() =>
			{
				 RuleFor(x => x.Code)
					 .MustAsync(async (code, cancellation) =>
						 !await db.Countries.AnyAsync(c => c.Code.ToLower() == code.ToLower().Trim(), cancellation))
					 .WithMessage("Код країни з такою назвою вже існує");
			});

		RuleFor(x => x.Slug)
			.NotEmpty().WithMessage("Slug країни не може бути порожнім")
			.MaximumLength(100).WithMessage("Slug країни не може перевищувати 100 символів")
			.DependentRules(() =>
			 {
				 RuleFor(x => x.Slug)
					 .MustAsync(async (slug, cancellation) =>
						 !await db.Countries.AnyAsync(c => c.Slug.ToLower() == slug.ToLower().Trim(), cancellation))
					 .WithMessage("Slug країни з такою назвою вже існує");
			 });
		RuleFor(x => x.Image)
			.NotEmpty().WithMessage("Файл зображення є обов'язковим");
	}
}
