import * as React from "react"

import { cn } from "@/lib/utils"

function TooltipProvider({ children }: { children: React.ReactNode }) {
  return <>{children}</>
}

function Tooltip({ children }: { children: React.ReactNode }) {
  return <>{children}</>
}

function TooltipTrigger({
  className,
  ...props
}: React.ComponentProps<"div">) {
  return <div data-slot="tooltip-trigger" className={cn("group/tooltip", className)} {...props} />
}

function TooltipContent({
  className,
  children,
  ...props
}: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="tooltip-content"
      className={cn(
        "pointer-events-none absolute left-1/2 top-full z-50 mt-2 hidden -translate-x-1/2 rounded-xl border border-white/10 bg-[#081019]/95 px-3 py-2 text-xs text-slate-200 shadow-[0_18px_60px_rgba(0,0,0,0.35)] backdrop-blur-xl group-hover/tooltip:block",
        className,
      )}
      {...props}
    >
      {children}
    </div>
  )
}

export { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger }
