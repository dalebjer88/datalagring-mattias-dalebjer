namespace CourseHub.Application.Participants;

public sealed record CreateParticipantRequest(string Email, string FirstName, string LastName, string PhoneNumber);

public sealed record UpdateParticipantRequest(string Email, string FirstName, string LastName, string PhoneNumber);

public sealed record ParticipantDto(int Id, string Email, string FirstName, string LastName, string PhoneNumber);
