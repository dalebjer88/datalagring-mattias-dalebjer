import { useEffect, useState } from "react";
import { api } from "../api";

type CourseDto = {
  id: number;
  courseCode: string;
  title: string;
  description: string;
  courseInstanceCount: number;
};

type LocationDto = {
  id: number;
  name: string;
  courseInstanceCount: number;
};

type TeacherDto = {
  id: number;
  email: string;
  firstName: string;
  lastName: string;
  expertise: string;
};

type CourseInstanceDto = {
  id: number;
  startDate: string;
  endDate: string;
  capacity: number;
  courseId: number;
  locationId: number;
  teacherIds: number[];
};

export function CourseInstancesPage() {
  const [items, setItems] = useState<CourseInstanceDto[]>([]);
  const [courses, setCourses] = useState<CourseDto[]>([]);
  const [locations, setLocations] = useState<LocationDto[]>([]);
  const [teachers, setTeachers] = useState<TeacherDto[]>([]);

  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [capacity, setCapacity] = useState<number>(10);
  const [courseId, setCourseId] = useState<number | "">("");
  const [locationId, setLocationId] = useState<number | "">("");
  const [teacherIds, setTeacherIds] = useState<number[]>([]);

  const [error, setError] = useState<string | null>(null);

  async function load() {
    setError(null);
    try {
      const [instancesData, coursesData, locationsData, teachersData] = await Promise.all([
        api.get<CourseInstanceDto[]>("/api/course-instances"),
        api.get<CourseDto[]>("/api/courses"),
        api.get<LocationDto[]>("/api/locations"),
        api.get<TeacherDto[]>("/api/teachers"),
      ]);

      setItems(instancesData);
      setCourses(coursesData);
      setLocations(locationsData);
      setTeachers(teachersData);
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

  function toggleTeacher(id: number) {
    setTeacherIds((prev) => (prev.includes(id) ? prev.filter((x) => x !== id) : [...prev, id]));
  }

  const normalizedTeacherIds = Array.from(new Set(teacherIds.filter((id) => id > 0)));
  const canCreate =
    startDate.trim().length > 0 &&
    endDate.trim().length > 0 &&
    endDate >= startDate &&
    capacity > 0 &&
    courseId !== "" &&
    locationId !== "" &&
    normalizedTeacherIds.length > 0;

  async function create() {
    if (!canCreate) return;

    setError(null);
    try {
      const created = await api.post<CourseInstanceDto>("/api/course-instances", {
        startDate,
        endDate,
        capacity,
        courseId,
        locationId,
        teacherIds: normalizedTeacherIds,
      });

      setItems((prev) => [created, ...prev]);
      setStartDate("");
      setEndDate("");
      setCapacity(10);
      setCourseId("");
      setLocationId("");
      setTeacherIds([]);
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function remove(id: number) {
    setError(null);
    try {
      await api.del(`/api/course-instances/${id}`);
      setItems((prev) => prev.filter((x) => x.id !== id));
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <div style={{ width: "100%" }}>
      <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto" }}>
        <h2>Course instances</h2>
      </div>

      {error && <div style={{ color: "red", marginBottom: 12 }}>{error}</div>}

      <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto 16px" }}>
        <input type="date" value={startDate} onChange={(e) => setStartDate(e.target.value)} />
        <input type="date" value={endDate} onChange={(e) => setEndDate(e.target.value)} />

        <input
          type="number"
          value={capacity}
          min={1}
          onChange={(e) => setCapacity(Number(e.target.value))}
          placeholder="Capacity"
        />

        <select value={courseId} onChange={(e) => setCourseId(e.target.value ? Number(e.target.value) : "")}>
          <option value="">Select course</option>
          {courses.map((c) => (
            <option key={c.id} value={c.id}>
              {c.courseCode} – {c.title} (Id: {c.id})
            </option>
          ))}
        </select>

        <select value={locationId} onChange={(e) => setLocationId(e.target.value ? Number(e.target.value) : "")}>
          <option value="">Select location</option>
          {locations.map((l) => (
            <option key={l.id} value={l.id}>
              {l.name} (Id: {l.id})
            </option>
          ))}
        </select>

        <div style={{ border: "1px solid #333", borderRadius: 6, padding: 8 }}>
          <div style={{ marginBottom: 6, opacity: 0.9 }}>Teachers (select at least 1)</div>
          <div style={{ display: "grid", gap: 6 }}>
            {teachers.map((t) => {
              const checked = teacherIds.includes(t.id);
              return (
                <label key={t.id} style={{ display: "flex", gap: 8, alignItems: "center" }}>
                  <input type="checkbox" checked={checked} onChange={() => toggleTeacher(t.id)} />
                  <span>
                    {t.firstName} {t.lastName} ({t.email}) – Id: {t.id}
                  </span>
                </label>
              );
            })}
          </div>
        </div>

        <button onClick={create} disabled={!canCreate}>
          Create
        </button>
      </div>

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
              <strong>CourseInstance</strong> (Id: {x.id})
              <div>
                {x.startDate} → {x.endDate}
              </div>
              <div>Capacity: {x.capacity}</div>
              <div>CourseId: {x.courseId}</div>
              <div>LocationId: {x.locationId}</div>
              <div>TeacherIds: {x.teacherIds}</div>
            </div>

            <button onClick={() => remove(x.id)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
