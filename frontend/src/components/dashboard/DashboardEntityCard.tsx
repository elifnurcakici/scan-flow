import type { PropsWithChildren, ReactNode } from "react"

import { cn } from "@/lib/utils"

export function DashboardEntityCard({
  children,
  className,
}: PropsWithChildren<{ className?: string }>) {
  return (
    <div className={cn("rounded-[20px] border border-white/8 bg-white/[0.025] p-3.5", className)}>
      {children}
    </div>
  )
}

export function DashboardEntityHeader({
  title,
  subtitle,
  action,
}: {
  title: ReactNode
  subtitle?: ReactNode
  action?: ReactNode
}) {
  return (
    <div className="mb-3 flex items-start justify-between gap-4">
      <div className="min-w-0">
        <div className="truncate text-sm font-medium text-white">{title}</div>
        {subtitle ? <div className="mt-1 truncate text-xs text-slate-400">{subtitle}</div> : null}
      </div>
      {action}
    </div>
  )
}
