type PageHeaderProps = {
  eyebrow: string
  title: string
  description: string
  action?: React.ReactNode
}

export function PageHeader({ eyebrow, title, description, action }: PageHeaderProps) {
  return (
    <div className="flex shrink-0 flex-col gap-4 rounded-[24px] border border-slate-800 bg-slate-950/75 p-4 lg:flex-row lg:items-end lg:justify-between">
      <div className="min-w-0">
        <div className="text-[11px] font-semibold uppercase tracking-[0.28em] text-emerald-300">
          {eyebrow}
        </div>
        <h2 className="mt-2 text-2xl font-semibold text-white">{title}</h2>
        <p className="mt-2 max-w-2xl text-sm text-slate-400">{description}</p>
      </div>
      {action ? <div className="shrink-0">{action}</div> : null}
    </div>
  )
}
