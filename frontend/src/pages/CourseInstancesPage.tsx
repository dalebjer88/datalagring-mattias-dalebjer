// frontend/src/pages/CourseInstancesPage.tsx
import { useEffect, useMemo, useState } from "react";
import { api } from "../api";
import type { CourseDto, CourseInstanceDto, LocationDto } from "../types";

export function CourseInstancesPage() {
  const [items, setItems] = useState<CourseInstanceDto[]>([]);
  const [courses, setCourses] = useState<CourseDto[]>([]);
  const [locations, setLocations] = useState<LocationDto[]>([]);

  const [startDate, setStartDate] = useState("");
  const [endDate, setEndDate] = useState("");
  const [capacity, setCapacity] = useState<number>(20);
  const [courseId, setCourseId] = useState<number>(0);
  const [locationId, setLocationId] = useState<number>(0);

  const [error, setError] = useState<string | null>(null);

  const today = useMemo(() => new Date().toISOString().slice(0, 10), []);
  const endMin = startDate ? startDate : today;

  async function load() {
    setError(null);
    try {
      const [instancesData, coursesData, locationsData] = await Promise.all([
        api.get<CourseInstanceDto[]>("/api/course-instances"),
        api.get<CourseDto[]>("/api/courses"),
        api.get<LocationDto[]>("/api/locations"),
      ]);

      setItems(instancesData);
      setCourses(coursesData);
      setLocations(locationsData);

      if (coursesData.length > 0 && courseId === 0) setCourseId(coursesData[0].id);
      if (locationsData.length > 0 && locationId === 0) setLocationId(locationsData[0].id);
    } catch (e) {
      setError((e as Error).message);
    }
  }

  useEffect(() => {
    void load();
  }, []);

  useEffect(() => {
    if (!startDate || !endDate) return;
    if (endDate < startDate) setEndDate("");
  }, [startDate, endDate]);

  async function create() {
    setError(null);
    try {
      const created = await api.post<CourseInstanceDto>("/api/course-instances", {
        startDate,
        endDate,
        capacity,
        courseId,
        locationId,
      });

      setItems((prev) => [created, ...prev]);
      setStartDate("");
      setEndDate("");
      setCapacity(20);
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

  const canCreate =
    !!startDate &&
    !!endDate &&
    startDate >= today &&
    endDate >= startDate &&
    capacity > 0 &&
    courseId > 0 &&
    locationId > 0;

  return (
    <div>
      <h2>Course instances</h2>

      {error && <div style={{ color: "red", marginBottom: 12 }}>{error}</div>}

      <div style={{ display: "grid", gap: 8, maxWidth: 520, marginBottom: 16 }}>
        <label style={{ display: "grid", gap: 4 }}>
          <span>Start date</span>
          <input type="date" value={startDate} min={today} onChange={(e) => setStartDate(e.target.value)} />
        </label>

        <label style={{ display: "grid", gap: 4 }}>
          <span>End date</span>
          <input type="date" value={endDate} min={endMin} onChange={(e) => setEndDate(e.target.value)} />
        </label>

        <label style={{ display: "grid", gap: 4 }}>
          <span>Capacity</span>
          <input type="number" value={capacity} min={1} onChange={(e) => setCapacity(Number(e.target.value))} />
        </label>

        <label style={{ display: "grid", gap: 4 }}>
          <span>Course</span>
          <select value={courseId} onChange={(e) => setCourseId(Number(e.target.value))} disabled={courses.length === 0}>
            {courses.length === 0 ? (
              <option value={0}>No courses available</option>
            ) : (
              courses.map((c) => (
                <option key={c.id} value={c.id}>
                  {c.id} – {c.courseCode} – {c.title}
                </option>
              ))
            )}
          </select>
        </label>

        <label style={{ display: "grid", gap: 4 }}>
          <span>Location</span>
          <select
            value={locationId}
            onChange={(e) => setLocationId(Number(e.target.value))}
            disabled={locations.length === 0}
          >
            {locations.length === 0 ? (
              <option value={0}>No locations available</option>
            ) : (
              locations.map((l) => (
                <option key={l.id} value={l.id}>
                  {l.id} – {l.name}
                </option>
              ))
            )}
          </select>
        </label>

        <button onClick={create} disabled={!canCreate}>
          Create
        </button>
      </div>

      <button onClick={load} style={{ marginBottom: 12 }}>
        Refresh
      </button>

      <ul style={{ paddingLeft: 0, listStyle: "none" }}>
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
            <div>
              <strong>
                {x.startDate} → {x.endDate}
              </strong>{" "}
              (Id: {x.id})
              <div>
                Capacity: {x.capacity} – CourseId: {x.courseId} – LocationId: {x.locationId}
              </div>
            </div>

            <button onClick={() => remove(x.id)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
