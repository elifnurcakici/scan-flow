import * as React from "react"
import { Check } from "lucide-react"

import { cn } from "@/lib/utils"

function Checkbox({
  checked = false,
  className,
  ...props
}: Omit<React.ComponentProps<"button">, "onChange"> & {
  checked?: boolean
}) {
  return (
    <button
      type="button"
      role="checkbox"
      aria-checked={checked}
      data-state={checked ? "checked" : "unchecked"}
      className={cn(
        "peer inline-flex size-5 shrink-0 items-center justify-center rounded-md border border-white/10 bg-white/[0.03] text-emerald-200 transition outline-none focus-visible:ring-2 focus-visible:ring-emerald-400/30 data-[state=checked]:border-emerald-400/30 data-[state=checked]:bg-emerald-400/12",
        className,
      )}
      {...props}
    >
      <Check className="size-3.5" />
    </button>
  )
}

export { Checkbox }
