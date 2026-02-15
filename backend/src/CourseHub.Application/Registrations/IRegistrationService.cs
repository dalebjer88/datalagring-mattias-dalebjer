namespace CourseHub.Application.Registrations;

public interface IRegistrationService
{
    Task<RegistrationResultDto> CreateCourseInstanceWithEnrollmentsAsync(
        CreateCourseInstanceWithEnrollmentsRequest request,
        CancellationToken ct = default);
}
