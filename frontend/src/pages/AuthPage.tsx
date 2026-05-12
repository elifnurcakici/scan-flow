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
  const [successMessage, setSuccessMessage] = useState<string | null>(null)

  async function handleSubmit(event: React.FormEvent<HTMLFormElement>) {
    event.preventDefault()
    setError(null)
    setSuccessMessage(null)

    try {
      if (mode === "login") {
        await login(email, password)
        return
      }

      await register(email, password)
      setMode("login")
      setPassword("")
      setSuccessMessage("Account created. You can sign in now.")
    } catch (submissionError) {
      setError(
        submissionError instanceof Error
          ? submissionError.message
          : "Authentication request failed.",
      )
    }
  }

  function handleModeChange(nextMode: "login" | "register") {
    setError(null)
    setSuccessMessage(null)
    setMode(nextMode)
  }

  return (
    <AuthLayout>
      <div className="grid w-full gap-8 lg:grid-cols-[1.15fr_0.85fr] xl:gap-10">
        <AuthHeroPanel />

        <AuthFormCard
          mode={mode}
          email={email}
          password={password}
          error={error}
          successMessage={successMessage}
          isSubmitting={isSubmitting}
          onModeChange={handleModeChange}
          onEmailChange={setEmail}
          onPasswordChange={setPassword}
          onSubmit={handleSubmit}
        />
      </div>
    </AuthLayout>
  )
}
