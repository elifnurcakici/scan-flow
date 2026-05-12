import { Outlet } from "react-router-dom"
import { LogOut, UserCircle2 } from "lucide-react"

import { AppSidebar } from "@/components/layout/AppSidebar"
import { StormBackground } from "@/components/effects/StormBackground"
import { Button } from "@/components/ui/button"
import { useAuth } from "@/providers/AuthProvider"

export default function DashboardLayout() {
  const { user, logout, isSubmitting } = useAuth()

  return (
    <div className="relative h-screen overflow-hidden bg-slate-950 text-white">
      <StormBackground intensity="low" />

      <div className="relative z-10 mx-auto flex h-screen max-w-[1560px] gap-4 px-4 py-4">
        <div className="hidden h-full lg:block">
          <AppSidebar />
        </div>

        <div className="flex min-w-0 flex-1 flex-col gap-4 overflow-hidden">
          <header className="flex flex-col gap-3 rounded-[24px] border border-slate-800 bg-slate-950/75 px-5 py-4 lg:flex-row lg:items-center lg:justify-between">
            <div>
              <div className="text-[11px] font-semibold uppercase tracking-[0.28em] text-emerald-300">
                Workspace session
              </div>
              <p className="mt-2 text-sm text-slate-400">
                Signed in as {user?.email ?? "unknown user"}
              </p>
            </div>
            <div className="flex items-center gap-3">
              <div className="inline-flex items-center gap-2 rounded-2xl border border-slate-800 bg-slate-900 px-3 py-2 text-sm text-slate-300">
                <UserCircle2 className="size-4" />
                {user?.email ?? "Unknown user"}
              </div>
              <Button
                type="button"
                variant="outline"
                className="rounded-2xl border-slate-700 bg-slate-900 text-white hover:bg-slate-800"
                onClick={() => void logout()}
              >
                <LogOut className="mr-2 size-4" />
                {isSubmitting ? "Signing out..." : "Logout"}
              </Button>
            </div>
          </header>

          <main className="min-h-0 min-w-0 flex-1 overflow-hidden">
            <Outlet />
          </main>
        </div>
      </div>
    </div>
  )
}
