import { Bolt, Shield, Sparkles } from "lucide-react"

import { Badge } from "@/components/ui/badge"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"

const riskSeries = [26, 38, 34, 55, 47, 42, 66, 58]

const previewFindings = [
  { label: "SQL Injection", severity: "High", variant: "destructive" as const },
  { label: "XSS", severity: "High", variant: "destructive" as const },
  { label: "Broken Access Control", severity: "Medium", variant: "warning" as const },
  { label: "Outdated Component", severity: "Low", variant: "outline" as const },
]

export function AuthHeroPanel() {
  return (
    <section className="relative hidden min-h-[720px] overflow-hidden rounded-[36px] border border-emerald-400/10 bg-white/[0.025] p-8 backdrop-blur-2xl lg:flex lg:flex-col">
      <div className="mb-12 flex items-center gap-3">
        <div className="flex size-11 items-center justify-center rounded-2xl border border-emerald-300/20 bg-emerald-400/10 shadow-[0_0_30px_rgba(16,185,129,0.35)]">
          <Bolt className="size-5 text-emerald-300" />
        </div>
        <span className="text-xl font-semibold tracking-tight text-white">ScanFlow</span>
      </div>

      <Badge className="w-fit">Real-time Scan Orchestration</Badge>

      <h1 className="mt-8 max-w-3xl text-balance text-5xl font-semibold leading-[1.02] tracking-[-0.05em] text-white xl:text-7xl">
        Unify visibility,
        <br />
        automate response,
        <br />
        <span className="bg-gradient-to-r from-emerald-300 via-emerald-400 to-cyan-300 bg-clip-text text-transparent">
          strengthen posture.
        </span>
      </h1>

      <p className="mt-6 max-w-xl text-lg leading-8 text-slate-300/82">
        Bring assets, scan workflows and vulnerability intelligence into one
        focused security operations surface with live pipeline awareness.
      </p>

      <div className="mt-auto grid gap-5 xl:grid-cols-[1.15fr_0.85fr]">
        <Card className="rounded-[30px] border-emerald-400/10 bg-[#08131d]/75 shadow-[0_30px_120px_rgba(0,0,0,0.4)] backdrop-blur-2xl">
          <CardHeader className="pb-3">
            <div className="mb-4 flex gap-2">
              <span className="size-2.5 rounded-full bg-white/12" />
              <span className="size-2.5 rounded-full bg-white/12" />
              <span className="size-2.5 rounded-full bg-white/12" />
            </div>
            <CardTitle className="text-lg text-white">SecOps Overview</CardTitle>
            <CardDescription className="text-slate-400">
              Monitor posture drift and active pipeline movement.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid gap-4 sm:grid-cols-2">
              <PreviewMetric
                icon={<Shield className="size-5 text-emerald-300" />}
                value="1,247"
                label="Total vulnerabilities"
                iconClassName="border-emerald-300/15 bg-emerald-400/10"
              />
              <PreviewMetric
                icon={<Sparkles className="size-5 text-cyan-200" />}
                value="42"
                label="Running scans"
                iconClassName="border-cyan-300/15 bg-cyan-400/10"
              />
            </div>

            <div className="rounded-3xl border border-white/6 bg-white/[0.025] p-4">
              <div className="mb-3 text-sm font-medium text-slate-200">Risk overview</div>
              <div className="flex h-36 items-end gap-2">
                {riskSeries.map((value, index) => (
                  <div key={index} className="relative flex-1 rounded-full bg-white/[0.04]">
                    <div
                      className="absolute inset-x-0 bottom-0 rounded-full bg-gradient-to-t from-emerald-500/85 to-emerald-300/70 shadow-[0_0_25px_rgba(16,185,129,0.25)]"
                      style={{ height: `${value}%` }}
                    />
                  </div>
                ))}
              </div>
            </div>
          </CardContent>
        </Card>

        <Card className="rounded-[30px] border-white/8 bg-[#09131a]/72 backdrop-blur-2xl">
          <CardHeader>
            <CardTitle className="text-lg text-white">Top Vulnerabilities</CardTitle>
            <CardDescription className="text-slate-400">
              Live threat categories from active workloads.
            </CardDescription>
          </CardHeader>
          <CardContent className="space-y-3">
            {previewFindings.map((finding) => (
              <div
                key={finding.label}
                className="flex items-center justify-between rounded-2xl border border-white/6 bg-white/[0.02] px-4 py-3"
              >
                <div className="flex items-center gap-3 text-sm text-slate-100">
                  <span className="size-2 rounded-full bg-emerald-300 shadow-[0_0_12px_rgba(110,231,183,0.9)]" />
                  {finding.label}
                </div>
                <Badge variant={finding.variant}>{finding.severity}</Badge>
              </div>
            ))}
          </CardContent>
        </Card>
      </div>
    </section>
  )
}

function PreviewMetric({
  icon,
  value,
  label,
  iconClassName,
}: {
  icon: React.ReactNode
  value: string
  label: string
  iconClassName: string
}) {
  return (
    <div className="rounded-3xl border border-white/6 bg-white/[0.03] p-4">
      <div className={`mb-6 flex size-10 items-center justify-center rounded-2xl border ${iconClassName}`}>
        {icon}
      </div>
      <div className="text-4xl font-semibold tracking-tight text-white">{value}</div>
      <div className="mt-2 text-sm text-slate-400">{label}</div>
    </div>
  )
}
