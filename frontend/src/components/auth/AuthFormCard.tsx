import { ArrowRight, Lock } from "lucide-react"

import { BorderBeam } from "@/components/effects/BorderBeam"
import { Spotlight } from "@/components/effects/Spotlight"
import { Badge } from "@/components/ui/badge"
import { Button } from "@/components/ui/button"
import { Checkbox } from "@/components/ui/checkbox"
import { Form, FormField, FormMessage } from "@/components/ui/form"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs"

type AuthMode = "login" | "register"

type AuthFormCardProps = {
  mode: AuthMode
  email: string
  password: string
  error: string | null
  isSubmitting: boolean
  onModeChange: (mode: AuthMode) => void
  onEmailChange: (value: string) => void
  onPasswordChange: (value: string) => void
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void
}

export function AuthFormCard({
  mode,
  email,
  password,
  error,
  isSubmitting,
  onModeChange,
  onEmailChange,
  onPasswordChange,
  onSubmit,
}: AuthFormCardProps) {
  return (
    <section className="flex items-center justify-center">
      <Card className="auth-card w-full max-w-[560px] rounded-[32px] border-white/10 bg-[#081019]/82 shadow-[0_40px_120px_rgba(0,0,0,0.55)] backdrop-blur-2xl">
        <Spotlight className="left-[-2rem] top-[-2rem]" size="md" />
        <Spotlight className="bottom-[-2rem] right-[-1rem] bg-cyan-400/10" size="sm" />
        <BorderBeam />
        <CardHeader className="pb-4">
          <div className="mb-4 flex size-16 items-center justify-center rounded-[22px] border border-emerald-300/15 bg-emerald-400/10 shadow-[0_0_35px_rgba(16,185,129,0.25)]">
            <Lock className="size-8 text-emerald-300" />
          </div>
          <Badge className="mb-4 w-fit">Workspace Access</Badge>
          <CardTitle className="text-4xl font-semibold tracking-[-0.04em] text-white">
            Enter the Scan Flow workspace
          </CardTitle>
          <CardDescription className="max-w-md text-base leading-7 text-slate-400">
            Use your account to create assets, start scans and watch the pipeline
            update live.
          </CardDescription>
        </CardHeader>

        <CardContent>
          <Tabs value={mode} onValueChange={(value) => onModeChange(value as AuthMode)}>
            <TabsList>
              <TabsTrigger value="login">Login</TabsTrigger>
              <TabsTrigger value="register">Register</TabsTrigger>
            </TabsList>

            <TabsContent value="login">
              <AuthFormFields
                email={email}
                password={password}
                error={error}
                isSubmitting={isSubmitting}
                mode={mode}
                onEmailChange={onEmailChange}
                onPasswordChange={onPasswordChange}
                onSubmit={onSubmit}
              />
            </TabsContent>

            <TabsContent value="register">
              <AuthFormFields
                email={email}
                password={password}
                error={error}
                isSubmitting={isSubmitting}
                mode={mode}
                onEmailChange={onEmailChange}
                onPasswordChange={onPasswordChange}
                onSubmit={onSubmit}
              />
            </TabsContent>
          </Tabs>
        </CardContent>
      </Card>
    </section>
  )
}

function AuthFormFields({
  mode,
  email,
  password,
  error,
  isSubmitting,
  onEmailChange,
  onPasswordChange,
  onSubmit,
}: {
  mode: AuthMode
  email: string
  password: string
  error: string | null
  isSubmitting: boolean
  onEmailChange: (value: string) => void
  onPasswordChange: (value: string) => void
  onSubmit: (event: React.FormEvent<HTMLFormElement>) => void
}) {
  return (
    <Form onSubmit={onSubmit}>
      <FormField>
        <Label htmlFor={`email-${mode}`}>
          Email
        </Label>
        <Input
          id={`email-${mode}`}
          type="email"
          value={email}
          onChange={(event) => onEmailChange(event.target.value)}
          placeholder="analyst@company.com"
          required
          className="h-14 rounded-2xl border-white/10 bg-white/[0.03] px-5 text-base text-white placeholder:text-slate-500 focus-visible:border-emerald-400/30 focus-visible:ring-emerald-400/20"
        />
      </FormField>

      <FormField>
        <Label htmlFor={`password-${mode}`}>
          Password
        </Label>
        <Input
          id={`password-${mode}`}
          type="password"
          value={password}
          onChange={(event) => onPasswordChange(event.target.value)}
          placeholder="••••••••••••"
          required
          className="h-14 rounded-2xl border-white/10 bg-white/[0.03] px-5 text-base text-white placeholder:text-slate-500 focus-visible:border-emerald-400/30 focus-visible:ring-emerald-400/20"
        />
      </FormField>

      <div className="flex items-center justify-between text-sm text-slate-400">
        <label className="flex items-center gap-2">
          <Checkbox checked />
          Remember me
        </label>
        <span className="text-emerald-300/90">Forgot password?</span>
      </div>

      {error ? (
        <FormMessage variant="destructive">
          {error}
        </FormMessage>
      ) : null}

      <Button
        type="submit"
        disabled={isSubmitting}
        className="auth-cta h-14 w-full rounded-2xl border-0 bg-gradient-to-r from-emerald-400 via-emerald-500 to-green-400 text-base font-semibold text-[#032212] shadow-[0_0_45px_rgba(16,185,129,0.28)] hover:brightness-105"
      >
        {isSubmitting
          ? mode === "login"
            ? "Signing in..."
            : "Creating account..."
          : "Continue to dashboard"}
        <ArrowRight className="ml-2 size-4" />
      </Button>
    </Form>
  )
}
