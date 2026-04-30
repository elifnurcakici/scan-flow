export function DashboardPanelNotice({
  message,
  compact = false,
}: {
  message: string
  compact?: boolean
}) {
  return (
    <div
      className={[
        "rounded-[20px] border border-dashed border-white/10 bg-white/[0.02] text-sm text-slate-400",
        compact ? "px-4 py-4" : "px-4 py-6",
      ].join(" ")}
    >
      {message}
    </div>
  )
}
