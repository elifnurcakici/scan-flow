import * as React from "react"

import { cn } from "@/lib/utils"

type DropdownMenuContextValue = {
  open: boolean
  setOpen: React.Dispatch<React.SetStateAction<boolean>>
}

const DropdownMenuContext = React.createContext<DropdownMenuContextValue | null>(null)

function DropdownMenu({ children }: { children: React.ReactNode }) {
  const [open, setOpen] = React.useState(false)

  return (
    <DropdownMenuContext.Provider value={{ open, setOpen }}>
      <div className="relative">{children}</div>
    </DropdownMenuContext.Provider>
  )
}

function DropdownMenuTrigger({
  className,
  asChild = false,
  children,
  ...props
}: React.ComponentProps<"button"> & {
  asChild?: boolean
}) {
  const context = React.useContext(DropdownMenuContext)

  if (asChild && React.isValidElement(children)) {
    return React.cloneElement(children, {
      onClick: () => context?.setOpen((current) => !current),
    } as Record<string, unknown>)
  }

  return (
    <button
      type="button"
      data-slot="dropdown-menu-trigger"
      className={cn(className)}
      onClick={() => context?.setOpen((current) => !current)}
      {...props}
    >
      {children}
    </button>
  )
}

function DropdownMenuContent({
  className,
  align = "end",
  children,
  ...props
}: React.ComponentProps<"div"> & {
  align?: "start" | "end"
}) {
  const context = React.useContext(DropdownMenuContext)

  if (!context?.open) {
    return null
  }

  return (
    <div
      data-slot="dropdown-menu-content"
      className={cn(
        "absolute top-full z-50 mt-2 min-w-52 rounded-2xl border border-white/10 bg-[#081019]/96 p-1.5 shadow-[0_18px_60px_rgba(0,0,0,0.38)] backdrop-blur-xl",
        align === "end" ? "right-0" : "left-0",
        className,
      )}
      {...props}
    >
      {children}
    </div>
  )
}

function DropdownMenuLabel({ className, ...props }: React.ComponentProps<"div">) {
  return (
    <div
      data-slot="dropdown-menu-label"
      className={cn("px-3 py-2 text-xs font-medium uppercase tracking-[0.18em] text-slate-400", className)}
      {...props}
    />
  )
}

function DropdownMenuSeparator({ className, ...props }: React.ComponentProps<"div">) {
  return <div data-slot="dropdown-menu-separator" className={cn("my-1 h-px bg-white/8", className)} {...props} />
}

function DropdownMenuItem({
  className,
  inset = false,
  ...props
}: React.ComponentProps<"button"> & {
  inset?: boolean
}) {
  const context = React.useContext(DropdownMenuContext)

  return (
    <button
      type="button"
      data-slot="dropdown-menu-item"
      className={cn(
        "flex w-full items-center rounded-xl px-3 py-2 text-sm text-white transition hover:bg-white/[0.06]",
        inset && "pl-8",
        className,
      )}
      onClick={(event) => {
        props.onClick?.(event)
        context?.setOpen(false)
      }}
      {...props}
    />
  )
}

export {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuLabel,
  DropdownMenuSeparator,
  DropdownMenuTrigger,
}
