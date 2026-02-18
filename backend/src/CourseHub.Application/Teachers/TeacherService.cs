using CourseHub.Application.Common.Exceptions;
using CourseHub.Domain.Entities;

namespace CourseHub.Application.Teachers;

public sealed class TeacherService : ITeacherService
{
    private readonly ITeacherRepository _repo;

    public TeacherService(ITeacherRepository repo)
    {
        _repo = repo;
    }

    public async Task<TeacherDto> CreateAsync(CreateTeacherRequest request, CancellationToken ct = default)
    {
        var email = NormalizeEmail(request.Email);
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var expertise = request.Expertise.Trim();

        Validate(email, firstName, lastName, expertise);

        if (await _repo.EmailExistsAsync(email, ct))
            throw new ConflictException("Email already exists.");

        var teacher = new Teacher
        {
            Email = email,
            FirstName = firstName,
            LastName = lastName,
            Expertise = expertise
        };

        await _repo.AddAsync(teacher, ct);
        await _repo.SaveChangesAsync(ct);

        return new TeacherDto(teacher.Id, teacher.Email, teacher.FirstName, teacher.LastName, teacher.Expertise);
    }

    public async Task<IReadOnlyList<TeacherDto>> GetAllAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetAllAsync(ct);

        return items
            .Select(x => new TeacherDto(x.Id, x.Email, x.FirstName, x.LastName, x.Expertise))
            .ToList();
    }

    public async Task<TeacherDto> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var teacher = await _repo.GetByIdAsync(id, ct);
        if (teacher is null) throw new NotFoundException("Teacher not found.");

        return new TeacherDto(teacher.Id, teacher.Email, teacher.FirstName, teacher.LastName, teacher.Expertise);
    }

    public async Task<TeacherDto> UpdateAsync(int id, UpdateTeacherRequest request, CancellationToken ct = default)
    {
        var teacher = await _repo.GetForUpdateAsync(id, ct);
        if (teacher is null) throw new NotFoundException("Teacher not found.");

        var email = NormalizeEmail(request.Email);
        var firstName = request.FirstName.Trim();
        var lastName = request.LastName.Trim();
        var expertise = request.Expertise.Trim();

        Validate(email, firstName, lastName, expertise);

        if (!string.Equals(teacher.Email, email, StringComparison.Ordinal))
        {
            var existing = await _repo.GetByEmailAsync(email, ct);
            if (existing is not null && existing.Id != id)
                throw new ConflictException("Email already exists.");
        }

        teacher.Email = email;
        teacher.FirstName = firstName;
        teacher.LastName = lastName;
        teacher.Expertise = expertise;

        await _repo.SaveChangesAsync(ct);

        return new TeacherDto(teacher.Id, teacher.Email, teacher.FirstName, teacher.LastName, teacher.Expertise);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var teacher = await _repo.GetForUpdateAsync(id, ct);
        if (teacher is null) throw new NotFoundException("Teacher not found.");

        if (await _repo.IsUsedByCourseInstancesAsync(id, ct))
            throw new ConflictException("Teacher is used by course instances. Remove links first.");

        await _repo.RemoveAsync(teacher, ct);
        await _repo.SaveChangesAsync(ct);
    }

    private static string NormalizeEmail(string value)
    {
        return value.Trim().ToLowerInvariant();
    }

    private static void Validate(string email, string firstName, string lastName, string expertise)
    {
        if (string.IsNullOrWhiteSpace(email)) throw new ValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(firstName)) throw new ValidationException("First name is required.");
        if (string.IsNullOrWhiteSpace(lastName)) throw new ValidationException("Last name is required.");
        if (string.IsNullOrWhiteSpace(expertise)) throw new ValidationException("Expertise is required.");

        if (email.Length > 254) throw new ValidationException("Email is too long.");
        if (firstName.Length > 100) throw new ValidationException("First name is too long.");
        if (lastName.Length > 100) throw new ValidationException("Last name is too long.");
        if (expertise.Length > 200) throw new ValidationException("Expertise is too long.");
    }
}
