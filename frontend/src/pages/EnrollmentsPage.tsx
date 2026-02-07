import { useEffect, useState } from "react";
import { api } from "../api";
import type { EnrollmentDto } from "../types";

type ParticipantDto = {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  phoneNumber: string;
};

type CourseInstanceDto = {
  id: number;
  courseId: number;
  locationId: number;
  startDate: string;
  endDate: string;
};

export function EnrollmentsPage() {
  const [items, setItems] = useState<EnrollmentDto[]>([]);
  const [participants, setParticipants] = useState<ParticipantDto[]>([]);
  const [courseInstances, setCourseInstances] = useState<CourseInstanceDto[]>([]);
  const [participantId, setParticipantId] = useState<number | "">("");
  const [courseInstanceId, setCourseInstanceId] = useState<number | "">("");
  const [error, setError] = useState<string | null>(null);

  async function load() {
    setError(null);
    try {
      const [enrollmentsData, participantsData, instancesData] = await Promise.all([
        api.get<EnrollmentDto[]>("/api/enrollments"),
        api.get<ParticipantDto[]>("/api/participants"),
        api.get<CourseInstanceDto[]>("/api/course-instances"),
      ]);

      setItems(enrollmentsData);
      setParticipants(participantsData);
      setCourseInstances(instancesData);
    } catch (e) {
      setError((e as Error).message);
    }
  }

  useEffect(() => {
    const run = async () => {
      await load();
    };

    void run();
  }, []);

  async function create() {
    if (participantId === "" || courseInstanceId === "") return;

    setError(null);
    try {
      const created = await api.post<EnrollmentDto>("/api/enrollments", {
        participantId,
        courseInstanceId,
        status: "Registered",
      });

      setItems((prev) => [created, ...prev]);
      setParticipantId("");
      setCourseInstanceId("");
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function remove(id: number) {
    setError(null);
    try {
      await api.del(`/api/enrollments/${id}`);
      setItems((prev) => prev.filter((x) => x.id !== id));
    } catch (e) {
      setError((e as Error).message);
    }
  }

  const canCreate = participantId !== "" && courseInstanceId !== "";

  return (
    <div style={{ width: "100%" }}>
      <h2>Enrollments</h2>

      {error && <div style={{ color: "red", marginBottom: 12 }}>{error}</div>}

      <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto 16px" }}>
        <select
          value={participantId}
          onChange={(e) => setParticipantId(e.target.value ? Number(e.target.value) : "")}
        >
          <option value="">Select participant</option>
          {participants.map((p) => (
            <option key={p.id} value={p.id}>
              {p.firstName} {p.lastName} ({p.email}) – Id: {p.id}
            </option>
          ))}
        </select>

        <select
          value={courseInstanceId}
          onChange={(e) => setCourseInstanceId(e.target.value ? Number(e.target.value) : "")}
        >
          <option value="">Select course instance</option>
          {courseInstances.map((ci) => (
            <option key={ci.id} value={ci.id}>
              Id: {ci.id} | CourseId: {ci.courseId} | LocationId: {ci.locationId} | {ci.startDate} → {ci.endDate}
            </option>
          ))}
        </select>

        <button onClick={create} disabled={!canCreate}>
          Create
        </button>
      </div>

      <button onClick={load} style={{ marginBottom: 12 }}>
        Refresh
      </button>

      <ul style={{ paddingLeft: 0, listStyle: "none", maxWidth: 900, margin: "0 auto" }}>
        {items.map((x) => (
          <li
            key={x.id}
            style={{
              display: "flex",
              justifyContent: "space-between",
              gap: 12,
              alignItems: "flex-start",
              padding: "8px 0",
              borderBottom: "1px solid #333",
            }}
          >
            <div style={{ textAlign: "left" }}>
              <strong>Enrollment</strong> (Id: {x.id})
              <div>ParticipantId: {x.participantId}</div>
              <div>CourseInstanceId: {x.courseInstanceId}</div>
              <div>Status: {x.status}</div>
              <div style={{ opacity: 0.8 }}>RegisteredAt: {x.registeredAt}</div>
            </div>

            <button onClick={() => remove(x.id)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
