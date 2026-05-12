import {
  createContext,
  useContext,
  useEffect,
  useMemo,
  useState,
} from 'react'
import type { PropsWithChildren } from 'react'
import { http } from '../lib/api'
import { clearSession, getSession, saveSession } from '../lib/storage'
import type { ApiResponse, AuthPayload, Session, UserProfile } from '../types/api'

type AuthContextValue = {
  initialized: boolean
  isAuthenticated: boolean
  isSubmitting: boolean
  user: UserProfile | null
  login: (email: string, password: string) => Promise<void>
  register: (email: string, password: string) => Promise<void>
  logout: () => Promise<void>
}

const AuthContext = createContext<AuthContextValue | null>(null)

export function AuthProvider({ children }: PropsWithChildren) {
  const [session, setSession] = useState<Session | null>(() => getSession())
  const [user, setUser] = useState<UserProfile | null>(null)
  const [initialized, setInitialized] = useState(false)
  const [isSubmitting, setIsSubmitting] = useState(false)

  useEffect(() => {
    void restoreSession()
  }, [])

  async function restoreSession() {
    const existingSession = getSession()

    if (!existingSession?.accessToken) {
      setInitialized(true)
      return
    }

    try {
      const response = await http.get<ApiResponse<UserProfile>>('/api/auth/me')
      setSession(existingSession)
      setUser(response.data.data ?? null)
    } catch {
      clearSession()
      setSession(null)
      setUser(null)
    } finally {
      setInitialized(true)
    }
  }

  async function persistSession(authPayload: AuthPayload) {
    const nextSession: Session = {
      accessToken: authPayload.accessToken,
      refreshToken: authPayload.refreshToken,
      email: authPayload.email,
    }

    saveSession(nextSession)
    setSession(nextSession)

    const me = await http.get<ApiResponse<UserProfile>>('/api/auth/me')
    setUser(me.data.data ?? null)
  }

  async function login(email: string, password: string) {
    setIsSubmitting(true)

    try {
      const response = await http.post<ApiResponse<AuthPayload>>('/api/auth/login', {
        email,
        password,
      })
      const payload = response.data.data

      if (!payload) {
        throw new Error('Login payload is empty.')
      }

      await persistSession(payload)
    } finally {
      setIsSubmitting(false)
      setInitialized(true)
    }
  }

  async function register(email: string, password: string) {
    setIsSubmitting(true)

    try {
      await http.post<ApiResponse<null>>('/api/auth/register', {
        email,
        password,
      })
    } finally {
      setIsSubmitting(false)
      setInitialized(true)
    }
  }

  async function logout() {
    setIsSubmitting(true)

    try {
      if (session?.accessToken) {
        await http.post('/api/auth/logout', {})
      }
    } finally {
      clearSession()
      setSession(null)
      setUser(null)
      setIsSubmitting(false)
    }
  }

  const value = useMemo<AuthContextValue>(
    () => ({
      initialized,
      isAuthenticated: Boolean(session?.accessToken && user),
      isSubmitting,
      user,
      login,
      register,
      logout,
    }),
    [initialized, isSubmitting, session?.accessToken, user],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}

export function useAuth() {
  const context = useContext(AuthContext)

  if (!context) {
    throw new Error('useAuth must be used within AuthProvider.')
  }

  return context
}
