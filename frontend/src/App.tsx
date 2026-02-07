import { useState } from "react";
import { CoursesPage } from "./pages/CoursesPage";
import { ParticipantsPage } from "./pages/ParticipantsPage";
import { LocationsPage } from "./pages/LocationsPage";
import { CourseInstancesPage } from "./pages/CourseInstancesPage";
import { EnrollmentsPage } from "./pages/EnrollmentsPage";
import { TeachersPage } from "./pages/TeachersPage";


type Tab = "courses" | "participants" | "locations" | "courseInstances" | "enrollments" | "teachers";

export default function App() {
  const [tab, setTab] = useState<Tab>("courses");

return (
  <div style={{ fontFamily: "system-ui, Arial" }}>
    <div style={{ width: "min(900px, 100%)", margin: "0 auto", padding: 16 }}>
      <h1 style={{ margin: "8px 0 16px", textAlign: "center" }}>CourseHub</h1>

      <div style={{ display: "flex", gap: 8, flexWrap: "wrap", justifyContent: "center", marginBottom: 16 }}>
        <button onClick={() => setTab("participants")}>Participants</button>
        <button onClick={() => setTab("enrollments")}>Enrollments</button>
        <button onClick={() => setTab("courses")}>Courses</button>
        <button onClick={() => setTab("locations")}>Locations</button>
        <button onClick={() => setTab("teachers")}>Teachers</button>
        <button onClick={() => setTab("courseInstances")}>Course instances</button>
        
        
      </div>

      <div style={{ width: "100%" }}>
        {tab === "participants" && <ParticipantsPage />}
        {tab === "enrollments" && <EnrollmentsPage />}
        {tab === "courses" && <CoursesPage />}
        {tab === "locations" && <LocationsPage />}
        {tab === "teachers" && <TeachersPage />}
        {tab === "courseInstances" && <CourseInstancesPage />}
      </div>
    </div>
  </div>
);

}
