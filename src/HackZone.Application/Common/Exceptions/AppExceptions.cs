using FluentValidation.Results;

namespace HackZone.Application.Common.Exceptions;

public class NotFoundException(string name, object key) : Exception($"{name} ({key}) topilmadi.");

public class AppValidationException(IEnumerable<ValidationFailure> failures) : Exception("Validatsiya xatosi.")
{
    public IDictionary<string, string[]> Errors { get; } = failures
        .GroupBy(f => f.PropertyName, f => f.ErrorMessage)
        .ToDictionary(g => g.Key, g => g.ToArray());
}

public class UnauthorizedException(string message = "Autentifikatsiya talab qilinadi.") : Exception(message);
public class ForbiddenException(string message = "Kirish taqiqlangan.") : Exception(message);
public class ConflictException(string message) : Exception(message);
public class BusinessException(string message) : Exception(message);
