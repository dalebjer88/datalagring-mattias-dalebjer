// frontend/src/pages/ParticipantsPage.tsx
import { useEffect, useState } from "react";
import { api } from "../api";
import type { ParticipantDto } from "../types";

export function ParticipantsPage() {
  const [items, setItems] = useState<ParticipantDto[]>([]);
  const [email, setEmail] = useState("");
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [phoneNumber, setPhoneNumber] = useState("");
  const [error, setError] = useState<string | null>(null);

  async function load() {
    setError(null);
    try {
      const data = await api.get<ParticipantDto[]>("/api/participants");
      setItems(data);
    } catch (e) {
      setError((e as Error).message);
    }
  }

useEffect(() => {
  const run = async () => {
    await load();
  };

  run();
}, []);


  async function create() {
    setError(null);
    try {
      const created = await api.post<ParticipantDto>("/api/participants", {
        email,
        firstName,
        lastName,
        phoneNumber,
      });
      setItems((prev) => [created, ...prev]);
      setEmail("");
      setFirstName("");
      setLastName("");
      setPhoneNumber("");
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function remove(id: number) {
    setError(null);
    try {
      await api.del(`/api/participants/${id}`);
      setItems((prev) => prev.filter((x) => x.id !== id));
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <div>
      <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto" }}>
      <h2>Participants</h2>
      </div>
      {error && <div style={{ color: "red", marginBottom: 12 }}>{error}</div>}

      <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto 16px" }}>
        <input value={email} onChange={(e) => setEmail(e.target.value)} placeholder="Email" />
        <input value={firstName} onChange={(e) => setFirstName(e.target.value)} placeholder="First name" />
        <input value={lastName} onChange={(e) => setLastName(e.target.value)} placeholder="Last name" />
        <input value={phoneNumber} onChange={(e) => setPhoneNumber(e.target.value)} placeholder="Phone number" />
        <button
          onClick={create}
          disabled={!email.trim() || !firstName.trim() || !lastName.trim() || !phoneNumber.trim()}
        >
          Create
        </button>
      </div>

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
                {x.firstName} {x.lastName}
              </strong>{" "}
              (Id: {x.id})
              <div>
                {x.email} – {x.phoneNumber}
              </div>
            </div>

            <button onClick={() => remove(x.id)}>Delete</button>
          </li>
        ))}
      </ul>
    </div>
  );
}
