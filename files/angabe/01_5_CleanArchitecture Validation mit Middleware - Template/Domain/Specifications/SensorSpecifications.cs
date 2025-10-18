
using Domain.Common;
using Domain.Contracts;
using Domain.Exceptions;

namespace Domain.Specifications;
public static class SensorSpecifications
{
    public const int NameMinLength = 2;

    public static DomainValidationResult CheckLocation(string location) =>
        string.IsNullOrWhiteSpace(location)
            ? DomainValidationResult.Failure("Location", "Location darf nicht leer sein.")
            : DomainValidationResult.Success("Location");

    //public static ValidationResult CheckName(string name) =>
    //    string.IsNullOrWhiteSpace(name)
    //        ? ValidationResult.Failure("Name darf nicht leer sein.")
    //    : name.Trim().Length < NameMinLength
    //        ? ValidationResult.Failure($"Name muss mindestens {NameMinLength} Zeichen haben.")
    //        : ValidationResult.Success();

    public static DomainValidationResult CheckName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
        {
            return DomainValidationResult.Failure("Name", "Name darf nicht leer sein.");
        }
        if (name.Trim().Length < NameMinLength)
        {
            return DomainValidationResult.Failure("Name", $"Name muss mindestens {NameMinLength} Zeichen haben.");
        }
        return DomainValidationResult.Success("Name");
    }


    public static DomainValidationResult CheckNameNotEqualLocation(string name, string location) =>
        string.Equals(name.Trim(), location.Trim(), StringComparison.OrdinalIgnoreCase)
            ? DomainValidationResult.Failure("Name", "Name darf nicht der Location entsprechen.")
            : DomainValidationResult.Success("Name");

    //public static IEnumerable<ValidationResult> ValidateEntityInternal(string location, string name)
    //{
    //    yield return CheckLocation(location);
    //    yield return CheckName(name);
    //    yield return CheckNameNotEqualLocation(name, location);
    //}

    public static void ValidateEntityInternal(string location, string name)
    {
        var validationResults = new List<DomainValidationResult>
        {
            CheckLocation(location),
            CheckName(name),
            CheckNameNotEqualLocation(name, location)
        };
        foreach (var result in validationResults)
        {
            if (!result.IsValid)
            {
                throw new DomainValidationException(result.Property, result.ErrorMessage!);
            }
        }
    }

    public static async Task ValidateEntityExternal(int id, string location, string name, 
            ISensorUniquenessChecker uniquenessChecker, CancellationToken ct = default)
    {
        if (!await uniquenessChecker.IsUniqueAsync(id, location, name, ct))
            throw new DomainValidationException("", "Ein Sensor mit der gleichen Location und dem gleichen Namen existiert bereits.");
    }


}
