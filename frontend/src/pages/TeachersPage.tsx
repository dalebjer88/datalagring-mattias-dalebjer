import { useEffect, useState } from "react";
import { api } from "../api";
import type { TeacherDto } from "../types";

export function TeachersPage() {
  const [items, setItems] = useState<TeacherDto[]>([]);
  const [email, setEmail] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [expertise, setExpertise] = useState("");
  const [error, setError] = useState<string | null>(null);

  async function load() {
    setError(null);
    try {
      const data = await api.get<TeacherDto[]>("/api/teachers");
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
      const created = await api.post<TeacherDto>("/api/teachers", {
        email,
        firstName,
        lastName,
        expertise,
      });

      setItems((prev) => [created, ...prev]);
      setEmail("");
      setFirstName("");
      setLastName("");
      setExpertise("");
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function remove(id: number) {
    setError(null);
    try {
      await api.del(`/api/teachers/${id}`);
      setItems((prev) => prev.filter((x) => x.id !== id));
    } catch (e) {
      setError((e as Error).message);
    }
  }

  const canCreate =
    email.trim().length > 0 &&
    firstName.trim().length > 0 &&
    lastName.trim().length > 0 &&
    expertise.trim().length > 0;

  return (
    <div style={{ width: "100%"}}>

      <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto" }}>
      <h2>Teachers</h2>
      </div>
      {error && <div style={{ color: "red", marginBottom: 12 }}>{error}</div>}

      <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto 16px" }}>
        <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
        <input value={firstName} onChange={(e) => setFirstName(e.target.value)} placeholder="First name" />
        <input value={lastName} onChange={(e) => setLastName(e.target.value)} placeholder="Last name" />
        <input value={expertise} onChange={(e) => setExpertise(e.target.value)} placeholder="Expertise" />
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
              <strong>
                {x.firstName} {x.lastName}
              </strong>{" "}
              (Id: {x.id})
              <div>{x.email}</div>
              <div style={{ opacity: 0.8 }}>{x.expertise}</div>
            </div>

            <button onClick={() => remove(x.id)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
