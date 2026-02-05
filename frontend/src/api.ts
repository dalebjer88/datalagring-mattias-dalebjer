const baseUrl = import.meta.env.VITE_API_BASE_URL;

async function request<T>(path: string, init?: RequestInit): Promise<T> {
  const res = await fetch(`${baseUrl}${path}`, {
    headers: { "Content-Type": "application/json" },
    ...init,
  });

  const contentType = res.headers.get("content-type") ?? "";

  if (!res.ok) {
    const text = await res.text();

    if (contentType.includes("application/json")) {
      try {
        const body = JSON.parse(text);
        if (body?.message) throw new Error(body.message);
      } catch {
        // ignore JSON parse errors, fall back to raw text
      }
    }

    throw new Error(text || `HTTP ${res.status}`);
  }

  if (res.status === 204) {
    return undefined as T;
  }

  if (!contentType.includes("application/json")) {
    const text = await res.text();
    throw new Error(`Expected JSON but got: ${contentType}. Body: ${text}`);
  }

  return (await res.json()) as T;
}

export const api = {
  get: <T>(path: string) => request<T>(path),
  post: <T>(path: string, body: unknown) =>
    request<T>(path, { method: "POST", body: JSON.stringify(body) }),
  put: <T>(path: string, body: unknown) =>
    request<T>(path, { method: "PUT", body: JSON.stringify(body) }),
  del: (path: string) => request<void>(path, { method: "DELETE" }),
};
