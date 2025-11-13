"use client";
import HeroContent from "@/components/hero-content";
import { useState } from "react";
import { AnimatePresence } from "framer-motion";
import AppsPanel from "@/components/apps-panel";
import CenteredLayout from "@/components/centered-layout";

export default function ShaderShowcase({ apps }: { apps: any }) {
  const [isPanelOpen, setIsPanelOpen] = useState(false);

  return (
    <CenteredLayout>
      <HeroContent isPanelOpen={isPanelOpen} onOpenPanel={() => setIsPanelOpen(true)} />
      <AnimatePresence>{isPanelOpen && <AppsPanel apps={apps} onClose={() => setIsPanelOpen(false)} />}</AnimatePresence>
    </CenteredLayout>
  );
}
