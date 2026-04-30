import axios from "axios"

import type { ApiResponse, AuthPayload } from "../types/api"
import { clearSession, getSession, saveSession } from "./storage"

const baseURL = import.meta.env.VITE_API_BASE_URL ?? "http://localhost:5092"

const http = axios.create({
  baseURL,
})

const refreshClient = axios.create({
  baseURL,
})

const PUBLIC_ROUTES = [
  "/api/auth/login",
  "/api/auth/register",
  "/api/auth/refresh",
]

type RetriableRequestConfig = {
  _retry?: boolean
  headers: Record<string, string>
  url?: string
}

let refreshPromise: Promise<string | null> | null = null

async function refreshAccessToken() {
  const session = getSession()
  if (!session?.refreshToken || !session.email) {
    return null
  }

  if (!refreshPromise) {
    refreshPromise = refreshClient
      .post<ApiResponse<AuthPayload>>("/api/auth/refresh", {
        email: session.email,
        refreshToken: session.refreshToken,
      })
      .then((response) => {
        const payload = response.data.data
        if (!payload) {
          throw new Error("Refresh payload is empty.")
        }

        saveSession({
          accessToken: payload.accessToken,
          refreshToken: payload.refreshToken,
          email: payload.email,
        })

        return payload.accessToken
      })
      .catch(() => {
        clearSession()
        return null
      })
      .finally(() => {
        refreshPromise = null
      })
  }

  return refreshPromise
}

http.interceptors.request.use((config) => {
  const session = getSession()
  const isPublic = PUBLIC_ROUTES.some((route) =>
    String(config.url ?? "").includes(route),
  )

  if (session?.accessToken && !isPublic) {
    config.headers.Authorization = `Bearer ${session.accessToken}`
  }

  return config
})

http.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config as RetriableRequestConfig | undefined
    const isPublic = PUBLIC_ROUTES.some((route) =>
      String(originalRequest?.url ?? "").includes(route),
    )

    if (
      error.response?.status === 401 &&
      originalRequest &&
      !originalRequest._retry &&
      !isPublic
    ) {
      originalRequest._retry = true
      const nextAccessToken = await refreshAccessToken()

      if (nextAccessToken) {
        originalRequest.headers.Authorization = `Bearer ${nextAccessToken}`
        return http(originalRequest)
      }
    }

    return Promise.reject(error)
  },
)

export { baseURL, http }
