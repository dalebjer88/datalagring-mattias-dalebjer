import { useState } from "react";
import { CoursesPage } from "./pages/CoursesPage";
import { ParticipantsPage } from "./pages/ParticipantsPage";
import { LocationsPage } from "./pages/LocationsPage";
import { CourseInstancesPage } from "./pages/CourseInstancesPage";

type Tab = "courses" | "participants" | "locations" | "courseInstances";

export default function App() {
  const [tab, setTab] = useState<Tab>("courses");

  return (
    <div
      style={{
        fontFamily: "system-ui, Arial",
        width: "min(900px, 100%)",
        margin: "0 auto",
        padding: 16,
        display: "flex",
        flexDirection: "column",
        alignItems: "center",
      }}
    >
      <h1 style={{ margin: "8px 0 16px" }}>CourseHub</h1>

      <div style={{ display: "flex", gap: 8, flexWrap: "wrap", justifyContent: "center", marginBottom: 16 }}>
        <button onClick={() => setTab("courses")}>Courses</button>
        <button onClick={() => setTab("participants")}>Participants</button>
        <button onClick={() => setTab("locations")}>Locations</button>
        <button onClick={() => setTab("courseInstances")}>Course instances</button>
      </div>

      <div style={{ width: "100%" }}>
        {tab === "courses" && <CoursesPage />}
        {tab === "participants" && <ParticipantsPage />}
        {tab === "locations" && <LocationsPage />}
        {tab === "courseInstances" && <CourseInstancesPage />}
      </div>
    </div>
  );
}
