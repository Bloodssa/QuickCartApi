using System.ComponentModel.DataAnnotations;

namespace QuickCartApi.Validators;

[AttributeUsage(AttributeTargets.Property)]
public class MinImagesAttribute(int minCount) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext context) =>
        value is IReadOnlyCollection<IFormFile> files && files.Count >= minCount
            ? ValidationResult.Success
            : new ValidationResult(ErrorMessage ?? $"At least {minCount} image is required.", [context.MemberName!]);
}