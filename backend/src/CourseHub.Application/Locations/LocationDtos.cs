namespace CourseHub.Application.Locations;

public sealed record CreateLocationRequest(string Name);

public sealed record UpdateLocationRequest(string Name);

public sealed record LocationDto(int Id, string Name, int CourseInstanceCount);
