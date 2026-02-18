using System.Net;
using System.Net.Http.Json;

using CourseHub.Application.Courses;
using CourseHub.Application.Locations;
using CourseHub.Application.Participants;
using CourseHub.Application.Teachers;
using CourseHub.Application.CourseInstances;
using CourseHub.Application.Enrollments;
using CourseHub.Application.Registrations;

namespace CourseHub.Tests.Integration.TestHelpers;

public sealed class ApiTestClient
{
    private readonly HttpClient _client;

    public ApiTestClient(HttpClient client)
    {
        _client = client;
    }

    public async Task<CourseDto> CreateCourseAsync(string code = "NET-101", string title = "Intro", string description = "Basics")
    {
        var res = await _client.PostAsJsonAsync("/api/courses", new CreateCourseRequest(code, title, description));
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<CourseDto>();
        Assert.NotNull(body);

        return body!;
    }

    public async Task<LocationDto> CreateLocationAsync(string name = "Karlstad")
    {
        var res = await _client.PostAsJsonAsync("/api/locations", new CreateLocationRequest(name));
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<LocationDto>();
        Assert.NotNull(body);

        return body!;
    }

    public async Task<TeacherDto> CreateTeacherAsync(string email = "teacher@test.com", string firstName = "Tina", string lastName = "Teach", string expertise = "C#")
    {
        var res = await _client.PostAsJsonAsync("/api/teachers", new CreateTeacherRequest(email, firstName, lastName, expertise));
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<TeacherDto>();
        Assert.NotNull(body);

        return body!;
    }

    public async Task<ParticipantDto> CreateParticipantAsync(string email = "p@test.com", string firstName = "Pat", string lastName = "Part", string phoneNumber = "0700000000")
    {
        var res = await _client.PostAsJsonAsync("/api/participants", new CreateParticipantRequest(email, firstName, lastName, phoneNumber));
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<ParticipantDto>();
        Assert.NotNull(body);

        return body!;
    }

    public async Task<CourseInstanceDto> CreateCourseInstanceAsync(
        int courseId,
        int locationId,
        int[] teacherIds,
        DateOnly? start = null,
        DateOnly? end = null,
        int capacity = 10)
    {
        var s = start ?? new DateOnly(2030, 1, 10);
        var e = end ?? new DateOnly(2030, 1, 12);

        var res = await _client.PostAsJsonAsync("/api/course-instances",
            new CreateCourseInstanceRequest(s, e, capacity, courseId, locationId, teacherIds));

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<CourseInstanceDto>();
        Assert.NotNull(body);

        return body!;
    }

    public async Task<EnrollmentDto> CreateEnrollmentAsync(int participantId, int courseInstanceId, string status = "Active")
    {
        var res = await _client.PostAsJsonAsync("/api/enrollments", new CreateEnrollmentRequest(participantId, courseInstanceId, status));
        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<EnrollmentDto>();
        Assert.NotNull(body);

        return body!;
    }

    public async Task<RegistrationResultDto> RegisterCourseInstanceWithEnrollmentsAsync(
        int courseId,
        int locationId,
        int[] teacherIds,
        int[] participantIds,
        string status = "Active",
        DateOnly? start = null,
        DateOnly? end = null,
        int capacity = 10)
    {
        var s = start ?? new DateOnly(2030, 2, 1);
        var e = end ?? new DateOnly(2030, 2, 2);

        var res = await _client.PostAsJsonAsync("/api/registrations/course-instance-with-enrollments",
            new CreateCourseInstanceWithEnrollmentsRequest(s, e, capacity, courseId, locationId, teacherIds, participantIds, status));

        Assert.Equal(HttpStatusCode.Created, res.StatusCode);

        var body = await res.Content.ReadFromJsonAsync<RegistrationResultDto>();
        Assert.NotNull(body);

        return body!;
    }
}
