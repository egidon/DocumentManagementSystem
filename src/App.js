import React from "react";
import { useEffect, useState } from "react";
import { BrowserRouter as Router, Routes, Route,useLocation } from "react-router-dom";
import "./App.css";
import Login from "./components/Login/Login";
import Dashboard from "./components/Dashboard/Dashboard";
import ViewFiles from "./components/Dashboard/ViewFiles";
import ViewTeamFiles from "./components/Dashboard/ViewTeamFiles";
import ShowAllFolders from "./components/Dashboard/ShowAllFolders";
import Sidebar from "./Sidebar";
import TopBar from './components/TopBar/TopBar'
import CreateRoles from "./components/Settings/CreateRoles";

const App = () => {
  return (
    <Router>
      <AppContent />
    </Router>
  );
};

function AppContent() {
  const location = useLocation();
  const [hideTopbar, setHideTopbar] = useState(false);
  const [hideSidebar, setHideSidebar] = useState(false);

  useEffect(() => {
    const handleRouteChange = () => {
      const { pathname } = location;
      setHideTopbar(pathname === "/" || pathname === "/login");
      setHideSidebar(pathname === "/" || pathname === "/login");
    };

    handleRouteChange();

    const unlisten = () => {
      const { pathname } = location;
      setHideTopbar(pathname === "/" || pathname === "/login");
      setHideSidebar(pathname === "/" || pathname === "/login" || pathname === "/dashboard");
    }

    window.addEventListener("popstate", handleRouteChange);
    return () => {
      window.removeEventListener("popstate", handleRouteChange);
    };
  }, [location]);
  return (
    <div className="App">
      {!hideTopbar && <TopBar />}
      <div>
      {!hideSidebar && (
          <div>
            <Sidebar />
          </div>
        )}
      </div>
      <div>
          <Routes>
            <Route path="/" element={<Login />} />
            <Route path="/login" element={<Login />} />
            <Route path="/dashboard" element={<Dashboard />} />
            <Route path="/myfolder" element={<Dashboard />} />
            <Route path="/viewfiles" element={<ViewFiles />} />
            <Route path="/viewteamfiles" element={<ViewTeamFiles />} />
            <Route path="/showallfolders" element={<ShowAllFolders />} />
            <Route path="/createrole" element={<CreateRoles />} />
          </Routes>
      </div>
    </div>
  );
}

export default App;
