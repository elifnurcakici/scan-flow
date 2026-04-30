import type { PropsWithChildren, ReactNode } from "react"

import { BorderBeam } from "@/components/effects/BorderBeam"
import { Spotlight } from "@/components/effects/Spotlight"
import { Badge } from "@/components/ui/badge"
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card"
import { cn } from "@/lib/utils"

type DashboardPanelCardProps = PropsWithChildren<{
  badge: string
  title: string
  description: string
  contentClassName?: string
  cardClassName?: string
  headerClassName?: string
  spotlight?: ReactNode
  action?: ReactNode
  beam?: boolean
}>

export function DashboardPanelCard({
  badge,
  title,
  description,
  children,
  contentClassName,
  cardClassName,
  headerClassName,
  spotlight,
  action,
  beam = false,
}: DashboardPanelCardProps) {
  return (
    <Card className={cn("flex min-h-0 flex-col rounded-[26px] border-white/10 bg-[#08121b]/82", cardClassName)}>
      {spotlight}
      {beam ? <BorderBeam /> : null}
      <CardHeader className={cn("pb-3", headerClassName)}>
        <div className="flex items-start justify-between gap-4">
          <div>
            <Badge className="mb-3 w-fit">{badge}</Badge>
            <CardTitle className="text-xl tracking-[-0.04em] text-white">{title}</CardTitle>
            <CardDescription className="text-sm text-slate-400">{description}</CardDescription>
          </div>
          {action}
        </div>
      </CardHeader>
      <CardContent className={contentClassName}>{children}</CardContent>
    </Card>
  )
}

export function DashboardPanelSpotlight({
  className,
  size = "sm",
}: {
  className?: string
  size?: "sm" | "md" | "lg"
}) {
  return <Spotlight className={className} size={size} />
}
