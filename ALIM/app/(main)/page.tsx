import HeroContent from "@/components/hero-content";
import { useState } from "react";
import { AnimatePresence } from "framer-motion";
import AppsPanel from "@/components/apps-panel";
import CenteredLayout from "@/components/centered-layout";
import { fetchAllApplications } from "@/data-access/fetch-all-apps";
import ShaderShowcase from "./_components/client";

export default async function Page() {
  const apps = await fetchAllApplications();
  console.log("apps", apps);
  return (
    <>
      <ShaderShowcase apps={apps} />
    </>
  );
}
