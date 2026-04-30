import { Navigate, useLocation } from 'react-router-dom'
import type { PropsWithChildren } from 'react'
import { useAuth } from '../providers/AuthProvider'

export function ProtectedRoute({ children }: PropsWithChildren) {
  const location = useLocation()
  const { initialized, isAuthenticated } = useAuth()

  if (!initialized) {
    return (
      <div className="screen-shell">
        <div className="panel panel-centered">
          <p className="muted">Session is being restored...</p>
        </div>
      </div>
    )
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" replace state={{ from: location }} />
  }

  return <>{children}</>
}
