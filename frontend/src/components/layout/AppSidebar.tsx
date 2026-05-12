import { BarChart3, Boxes, ShieldAlert, Waves } from "lucide-react"
import { NavLink } from "react-router-dom"

import { cn } from "@/lib/utils"

const links = [
  { to: "/", label: "Dashboard", icon: BarChart3, end: true },
  { to: "/assets", label: "Assets", icon: Boxes },
  { to: "/scans", label: "Scans", icon: Waves },
  { to: "/vulnerabilities", label: "Vulnerabilities", icon: ShieldAlert },
]

export function AppSidebar() {
  return (
    <aside className="flex h-full w-full max-w-[260px] flex-col overflow-hidden rounded-[24px] border border-slate-800 bg-slate-950/85 p-4">
      <div className="mb-6 rounded-2xl border border-emerald-500/15 bg-emerald-500/10 p-4">
        <div className="text-[11px] font-semibold uppercase tracking-[0.28em] text-emerald-300">
          ScanFlow
        </div>
        <h1 className="mt-2 text-lg font-semibold text-white">Security workspace</h1>
        <p className="mt-2 text-sm text-slate-400">
          Assets, scans and vulnerabilities in one place.
        </p>
      </div>

      <nav className="space-y-2 overflow-y-auto pr-1">
        {links.map((link) => {
          const Icon = link.icon

          return (
            <NavLink
              key={link.to}
              to={link.to}
              end={link.end}
              className={({ isActive }) =>
                cn(
                  "flex items-center gap-3 rounded-2xl px-3 py-3 text-sm text-slate-300 transition hover:bg-slate-900 hover:text-white",
                  isActive && "bg-emerald-500/12 text-white ring-1 ring-emerald-400/20",
                )
              }
            >
              <Icon className="size-4" />
              {link.label}
            </NavLink>
          )
        })}
      </nav>
    </aside>
  )
}
