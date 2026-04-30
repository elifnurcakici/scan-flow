import { cn } from "@/lib/utils"

type StormBackgroundProps = {
  intensity?: "low" | "high"
}

export function StormBackground({ intensity = "low" }: StormBackgroundProps) {
  return (
    <div className="pointer-events-none absolute inset-0 overflow-hidden">
      <div className="absolute inset-0 bg-[radial-gradient(circle_at_12%_20%,rgba(12,255,168,0.12),transparent_22%),radial-gradient(circle_at_62%_8%,rgba(11,154,255,0.08),transparent_20%),radial-gradient(circle_at_78%_72%,rgba(12,255,168,0.08),transparent_18%),linear-gradient(180deg,#030b11_0%,#051019_48%,#040b12_100%)]" />
      <div className="absolute inset-0 bg-[linear-gradient(to_right,rgba(148,163,184,0.045)_1px,transparent_1px),linear-gradient(to_bottom,rgba(148,163,184,0.045)_1px,transparent_1px)] bg-[size:90px_90px] opacity-35" />
      <div className="storm-vignette absolute inset-0" />

      <div className="storm-haze absolute -left-20 top-24 h-80 w-80 rounded-full bg-emerald-400/10 blur-[120px]" />
      <div className="storm-haze absolute right-8 top-0 h-[26rem] w-[26rem] rounded-full bg-cyan-400/10 blur-[140px]" />
      <div className="storm-haze absolute bottom-[-8rem] left-[30%] h-72 w-72 rounded-full bg-emerald-500/10 blur-[120px]" />

      <div className={cn("storm-strike storm-strike-primary", intensity === "high" && "opacity-100")} />
      <div className={cn("storm-strike storm-strike-secondary", intensity === "high" && "opacity-90")} />
      <div className="storm-strike storm-strike-tertiary" />
      <div className="storm-flash absolute inset-0" />
    </div>
  )
}
