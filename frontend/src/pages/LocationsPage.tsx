import { useEffect, useState } from "react";
import { api } from "../api";
import type { LocationDto } from "../types";

export function LocationsPage() {
  const [items, setItems] = useState<LocationDto[]>([]);
  const [name, setName] = useState("");
  const [error, setError] = useState<string | null>(null);

  async function load() {
    setError(null);
    try {
      const data = await api.get<LocationDto[]>("/api/locations");
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
      const created = await api.post<LocationDto>("/api/locations", { name });
      setItems((prev) => [created, ...prev]);
      setName("");
    } catch (e) {
      setError((e as Error).message);
    }
  }

  async function remove(id: number) {
    setError(null);
    try {
      await api.del(`/api/locations/${id}`);
      setItems((prev) => prev.filter((x) => x.id !== id));
    } catch (e) {
      setError((e as Error).message);
    }
  }

  return (
    <div>
      <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto" }}>
      <h2>Locations</h2>
      </div>
      {error && <div style={{ color: "red", marginBottom: 12 }}>{error}</div>}

      <div style={{ display: "grid", gap: 8, maxWidth: 520, margin: "0 auto 16px" }}>
        <input value={name} onChange={(e) => setName(e.target.value)} placeholder="Name" />
        <button onClick={create} disabled={!name.trim()}>
          Create
        </button>
      </div>

      <ul style={{ paddingLeft: 0, listStyle: "none" }}>
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
                alignItems: "center",
                padding: "8px 0",
                borderBottom: "1px solid #333",
              }}
            >
              <div>
                <strong>{x.name}</strong> (Id: {x.id})
                {x.courseInstanceCount > 0 && (
                  <div style={{ opacity: 0.8 }}>Instances: {x.courseInstanceCount}</div>
                )}
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
