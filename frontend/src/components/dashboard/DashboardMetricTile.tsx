import { Radar } from "lucide-react"

import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip"

export function DashboardMetricTile({
  icon: Icon,
  label,
  value,
  accent,
}: {
  icon: typeof Radar
  label: string
  value: string
  accent: "emerald" | "cyan" | "rose"
}) {
  const accentClass = {
    emerald: "border-emerald-300/15 bg-emerald-400/10 text-emerald-200",
    cyan: "border-cyan-300/15 bg-cyan-400/10 text-cyan-100",
    rose: "border-rose-300/15 bg-rose-400/10 text-rose-100",
  }[accent]

  return (
    <TooltipProvider>
      <Tooltip>
        <TooltipTrigger className="relative">
          <div className="rounded-[20px] border border-white/10 bg-[#08111a]/70 p-3">
            <div className={`mb-4 flex size-9 items-center justify-center rounded-2xl border ${accentClass}`}>
              <Icon className="size-4" />
            </div>
            <div className="text-2xl font-semibold tracking-[-0.05em] text-white">{value}</div>
            <div className="mt-1 text-xs text-slate-400">{label}</div>
          </div>
          <TooltipContent>{label} snapshot</TooltipContent>
        </TooltipTrigger>
      </Tooltip>
    </TooltipProvider>
  )
}
