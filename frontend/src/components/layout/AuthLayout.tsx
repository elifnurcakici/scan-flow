import type { PropsWithChildren } from "react"

import { StormBackground } from "@/components/effects/StormBackground"

export default function AuthLayout({ children }: PropsWithChildren) {
  return (
    <div className="relative min-h-screen overflow-hidden bg-[#030b11] text-white">
      <StormBackground intensity="high" />

      <div className="relative z-10 mx-auto flex min-h-screen w-full max-w-[1600px] items-center px-6 py-10 lg:px-10">
        {children}
      </div>
    </div>
  )
}
