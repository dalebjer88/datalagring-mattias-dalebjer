import { useEffect, useState } from "react";
import { api } from "../api";
import type { CourseDto } from "../types";

export function CoursesPage() {
  const [items, setItems] = useState<CourseDto[]>([]);
  const [courseCode, setCourseCode] = useState("");
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [error, setError] = useState<string | null>(null);

  async function load() {
    setError(null);
    try {
      const data = await api.get<CourseDto[]>("/api/courses");
      setItems(data);
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
    setError(null);
    try {
      const created = await api.post<CourseDto>("/api/courses", { courseCode, title, description });
      setItems((prev) => [created, ...prev]);
      setCourseCode("");
      setTitle("");
      setDescription("");
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function remove(id: number) {
    setError(null);
    try {
      await api.del(`/api/courses/${id}`);
      setItems((prev) => prev.filter((x) => x.id !== id));
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
<div style={{ width: "100%", maxWidth: 900, margin: "0 auto", textAlign: "left" }}>
    <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto" }}>
      <h2>Courses</h2>
    </div>
      
      {error && <div style={{ color: "red", marginBottom: 12 }}>{error}</div>}

    <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto 16px" }}>

        <input value={courseCode} onChange={(e) => setCourseCode(e.target.value)} placeholder="Course code" />
        <input value={title} onChange={(e) => setTitle(e.target.value)} placeholder="Title" />
        <input value={description} onChange={(e) => setDescription(e.target.value)} placeholder="Description" />
        <button onClick={create} disabled={!courseCode.trim() || !title.trim() || !description.trim()}>
          Create
        </button>
    </div>

      <ul style={{ paddingLeft: 0, listStyle: "none", maxWidth: 900, margin: "0 auto" }}>
        {items.map((x) => {
          const canDelete = x.courseInstanceCount === 0;
          const disabledTitle = canDelete ? "" : "Remove course instances first.";

          return (
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
                <strong>
                  {x.courseCode} – {x.title}
                </strong>{" "}
                (Id: {x.id})
                <div>{x.description}</div>
                {x.courseInstanceCount > 0 && <div style={{ opacity: 0.8 }}>Instances: {x.courseInstanceCount}</div>}
              </div>

              <span title={disabledTitle} style={{ display: "inline-block" }}>
                <button
                  onClick={() => remove(x.id)}
                  disabled={!canDelete}
                  style={!canDelete ? { cursor: "not-allowed", opacity: 0.6 } : undefined}
                >
                  Delete
                </button>
              </span>
            </li>
          );
        })}
      </ul>
    </div>
  );
}
