import { cn } from "@/lib/utils"

export function BorderBeam({ className }: { className?: string }) {
  return (
    <div
      className={cn(
        "pointer-events-none absolute inset-0 overflow-hidden rounded-[inherit]",
        className,
      )}
    >
      <div className="border-beam absolute -left-1/4 top-0 h-px w-1/3" />
    </div>
  )
}
