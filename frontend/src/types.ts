export type CourseDto = {
  id: number;
  courseCode: string;
  title: string;
  description: string;
  courseInstanceCount: number;
};

export type ParticipantDto = {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
};

export type LocationDto = {
  id: number;
  name: string;
  courseInstanceCount: number;
};

export type CourseInstanceDto = {
  id: number;
  startDate: string;
  endDate: string;
  capacity: number;
  courseId: number;
  locationId: number;
};

export type EnrollmentDto = {
  id: number;
  participantId: number;
  courseInstanceId: number;
  registeredAt: string;
  status: string;
};
