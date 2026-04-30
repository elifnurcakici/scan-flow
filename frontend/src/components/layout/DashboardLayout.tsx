import { Outlet } from "react-router-dom"
import { Bolt, ChevronDown, LogOut, ShieldCheck, UserCircle2 } from "lucide-react"

import { StormBackground } from "@/components/effects/StormBackground"
import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"
import { useAuth } from "@/providers/AuthProvider"

export default function DashboardLayout() {
  const { user, logout, isSubmitting } = useAuth()

  return (
    <div className="relative min-h-screen overflow-hidden bg-[#030b11] text-white">
      <StormBackground intensity="low" />

      <div className="relative z-10 mx-auto flex min-h-screen w-full max-w-[1560px] flex-col px-4 py-4 lg:px-6">
        <header className="mb-4 flex flex-col gap-3 rounded-[24px] border border-emerald-400/10 bg-white/[0.03] px-4 py-3 backdrop-blur-2xl lg:flex-row lg:items-center lg:justify-between">
          <div className="flex items-center gap-3">
            <div className="flex size-10 items-center justify-center rounded-2xl border border-emerald-300/25 bg-emerald-400/10 shadow-[0_0_30px_rgba(16,185,129,0.18)]">
              <Bolt className="size-4.5 text-emerald-300" />
            </div>

            <div>
              <div className="flex items-center gap-2 text-[10px] font-semibold uppercase tracking-[0.28em] text-emerald-300/80">
                <ShieldCheck className="size-3.5" />
                ScanFlow Workspace
              </div>
              <p className="mt-1 text-xs text-slate-300/75 lg:text-sm">
                Live view of assets, scans and findings.
              </p>
            </div>
          </div>

          <div className="flex flex-col gap-2 sm:flex-row sm:items-center">
            <DropdownMenu>
              <DropdownMenuTrigger asChild>
                <Button
                  type="button"
                  variant="outline"
                  className="h-10 rounded-2xl border-white/10 bg-white/[0.03] px-4 text-white hover:bg-white/[0.08]"
                >
                  <UserCircle2 className="mr-2 size-4" />
                  <span className="max-w-[210px] truncate">{user?.email ?? "Unknown user"}</span>
                  <ChevronDown className="ml-2 size-4 text-slate-400" />
                </Button>
              </DropdownMenuTrigger>
              <DropdownMenuContent align="end">
                <DropdownMenuLabel>Workspace session</DropdownMenuLabel>
                <DropdownMenuItem inset className="cursor-default text-slate-300 hover:bg-transparent">
                  {user?.email ?? "Unknown user"}
                </DropdownMenuItem>
                <DropdownMenuSeparator />
                <DropdownMenuItem onClick={() => void logout()} className="text-rose-100">
                  <LogOut className="mr-2 size-4" />
                  {isSubmitting ? "Signing out..." : "Logout"}
                </DropdownMenuItem>
              </DropdownMenuContent>
            </DropdownMenu>
          </div>
        </header>

        <main className="flex-1 overflow-hidden">
          <Outlet />
        </main>
      </div>
    </div>
  )
}
