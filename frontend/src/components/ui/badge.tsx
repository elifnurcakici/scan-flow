import * as React from "react"
import { cva, type VariantProps } from "class-variance-authority"

import { cn } from "@/lib/utils"

const badgeVariants = cva(
  "inline-flex items-center rounded-full border px-2.5 py-1 text-[11px] font-semibold uppercase tracking-[0.22em] transition-colors",
  {
    variants: {
      variant: {
        default: "border-emerald-300/20 bg-emerald-400/10 text-emerald-200",
        secondary: "border-cyan-300/20 bg-cyan-400/10 text-cyan-100",
        destructive: "border-rose-300/20 bg-rose-400/10 text-rose-100",
        outline: "border-white/10 bg-white/[0.03] text-slate-200",
        warning: "border-amber-300/20 bg-amber-400/10 text-amber-100",
      },
    },
    defaultVariants: {
      variant: "default",
    },
  },
)

function Badge({
  className,
  variant,
  ...props
}: React.ComponentProps<"div"> & VariantProps<typeof badgeVariants>) {
  return <div className={cn(badgeVariants({ variant }), className)} {...props} />
}

export { Badge, badgeVariants }
