namespace Domain.Contracts;

/// <summary>
/// Domain-Service zur Pr�fung der fachlichen Eindeutigkeit eines Sensors (Location + Name).
/// </summary>
public interface ISensorUniquenessChecker
{
    Task<bool> IsUniqueAsync(int id, string location, string name, CancellationToken ct = default);
}
