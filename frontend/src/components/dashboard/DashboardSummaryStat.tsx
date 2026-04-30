import { Gauge } from "lucide-react"

export function DashboardSummaryStat({
  icon: Icon,
  label,
  value,
}: {
  icon: typeof Gauge
  label: string
  value: string
}) {
  return (
    <div className="rounded-[20px] border border-white/8 bg-white/[0.025] p-3">
      <div className="mb-4 flex size-9 items-center justify-center rounded-2xl border border-emerald-300/15 bg-emerald-400/10">
        <Icon className="size-4 text-emerald-200" />
      </div>
      <div className="text-2xl font-semibold tracking-[-0.05em] text-white">{value}</div>
      <div className="mt-1 text-xs text-slate-400">{label}</div>
    </div>
  )
}
