import * as React from "react"

import { cn } from "@/lib/utils"

function Form({ className, ...props }: React.ComponentProps<"form">) {
  return <form data-slot="form" className={cn("space-y-6", className)} {...props} />
}

function FormField({ className, ...props }: React.ComponentProps<"div">) {
  return <div data-slot="form-field" className={cn("space-y-2.5", className)} {...props} />
}

function FormMessage({
  className,
  variant = "default",
  ...props
}: React.ComponentProps<"div"> & {
  variant?: "default" | "destructive"
}) {
  return (
    <div
      data-slot="form-message"
      data-variant={variant}
      className={cn(
        "rounded-2xl border px-4 py-3 text-sm",
        variant === "destructive"
          ? "border-rose-400/20 bg-rose-400/10 text-rose-100"
          : "border-emerald-400/14 bg-emerald-400/10 text-emerald-100",
        className,
      )}
      {...props}
    />
  )
}

export { Form, FormField, FormMessage }
