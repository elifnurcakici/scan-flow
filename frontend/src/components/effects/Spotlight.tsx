import { cn } from "@/lib/utils"

export function Spotlight({
  className,
  size = "md",
}: {
  className?: string
  size?: "sm" | "md" | "lg"
}) {
  const sizeClass = {
    sm: "h-32 w-32 blur-[70px]",
    md: "h-44 w-44 blur-[90px]",
    lg: "h-64 w-64 blur-[120px]",
  }[size]

  return (
    <div
      className={cn(
        "pointer-events-none absolute rounded-full bg-emerald-400/12",
        sizeClass,
        className,
      )}
    />
  )
}
