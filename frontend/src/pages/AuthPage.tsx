import { useState } from "react"

import { AuthFormCard } from "@/components/auth/AuthFormCard"
import { AuthHeroPanel } from "@/components/auth/AuthHeroPanel"
import AuthLayout from "@/components/layout/AuthLayout"
import { useAuth } from "@/providers/AuthProvider"

export default function AuthPage() {
  const { login, register, isSubmitting } = useAuth()
  const [mode, setMode] = useState<"login" | "register">("login")
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [error, setError] = useState<string | null>(null)

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError(null)

    try {
      if (mode === "login") {
        await login(email, password)
        return
      }

      await register(email, password)
    } catch (submissionError) {
      if (submissionError instanceof Error) {
        setError(submissionError.message)
        return
      }

      setError("Authentication request failed.")
    }
  }

  return (
    <AuthLayout>
      <div className="grid w-full gap-10 lg:grid-cols-[1.08fr_0.92fr] xl:gap-16">
        <AuthHeroPanel />
        <AuthFormCard
          mode={mode}
          email={email}
          password={password}
          error={error}
          isSubmitting={isSubmitting}
          onModeChange={setMode}
          onEmailChange={setEmail}
          onPasswordChange={setPassword}
          onSubmit={handleSubmit}
        />
      </div>
    </AuthLayout>
  )
}
