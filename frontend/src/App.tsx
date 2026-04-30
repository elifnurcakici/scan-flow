import { Navigate, Route, Routes } from "react-router-dom"

import DashboardLayout from "./components/layout/DashboardLayout"
import AuthPage from "./pages/AuthPage"
import DashboardPage from "./pages/DashboardPage"
import { useAuth } from "./providers/AuthProvider"

function App() {
  const { isAuthenticated, initialized } = useAuth()

  if (!initialized) {
    return null
  }

  return (
    <Routes>
      <Route
        path="/login"
        element={isAuthenticated ? <Navigate to="/" replace /> : <AuthPage />}
      />
      <Route
        element={
          isAuthenticated ? <DashboardLayout /> : <Navigate to="/login" replace />
        }
      >
        <Route path="/" element={<DashboardPage />} />
      </Route>
      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  )
}

export default App
