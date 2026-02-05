USE CourseHubDB;
GO

DELETE FROM dbo.CourseInstances;
DELETE FROM dbo.Participants;
DELETE FROM dbo.Locations;
DELETE FROM dbo.Courses;
GO

DBCC CHECKIDENT ('dbo.CourseInstances', RESEED, 0);
DBCC CHECKIDENT ('dbo.Participants', RESEED, 0);
DBCC CHECKIDENT ('dbo.Locations', RESEED, 0);
DBCC CHECKIDENT ('dbo.Courses', RESEED, 0);
GO
