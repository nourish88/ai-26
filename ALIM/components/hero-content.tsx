// components/hero-content.tsx

"use client";

import { useAnimate, motion, Variants } from "framer-motion";
import React from "react";
import Image from "next/image";

// We must wrap the Next.js Image component in `motion` to animate it.
const MotionImage = motion(Image);

interface HeroContentProps {
  isPanelOpen: boolean;
  onOpenPanel: () => void;
}

// Variants for the parent container (h1) to control the "one by one" animation
const headlineVariants: Variants = {
  visible: {
    transition: {
      staggerChildren: 0.1,
    },
  },
  hidden: {
    transition: {
      staggerChildren: 0.05,
      staggerDirection: -1,
    },
  },
};

// Variants for the individual letters. The "hidden" state now uses a responsive unit.
const letterVariants: Variants = {
  visible: {
    opacity: 1,
    y: 0,
    transition: { type: "spring", stiffness: 300, damping: 24 },
  },
  hidden: {
    opacity: 1,
    y: "-43vh",
    transition: { duration: 0.5, ease: "easeInOut" },
  },
};

// A more descriptive variant for other content (subheading, button).
// It now includes a slight vertical movement for a more dynamic exit.
const contentVariants: Variants = {
  visible: { opacity: 1, y: 0, transition: { duration: 0.4, delay: 0.2 } },
  hidden: { opacity: 0, y: -20, transition: { duration: 0.2 } },
};

const buttonVariants: Variants = {
  visible: { opacity: 1, y: 0, transition: { duration: 0.4, delay: 0.3 } },
  hidden: { opacity: 0, y: 20, transition: { duration: 0.2 } },
};

const HeroContent = React.forwardRef<HTMLDivElement, HeroContentProps>(({ isPanelOpen, onOpenPanel }, ref) => {
  const [scope, animate] = useAnimate();

  const onMouseEnter = () => {
    animate([
      [".button-text", { x: 0, paddingRight: "8px" }, { duration: 0.3, at: "<" }],
      [".icon-container", { width: "24px", opacity: 1 }, { duration: 0.3, at: "<" }],
      [".enter-icon", { opacity: 1, x: 0 }, { duration: 0.2, at: "-0.1" }],
    ]);
  };

  const onMouseLeave = () => {
    animate([
      [".enter-icon", { opacity: 0, x: -8 }, { duration: 0.2 }],
      [".button-text", { x: 0, paddingRight: "0px" }, { duration: 0.3, at: "<" }],
      [".icon-container", { width: "0px", opacity: 0 }, { duration: 0.3, at: "<" }],
    ]);
  };

  return (
    <main className="flex flex-1 flex-col items-center justify-center p-4 text-center z-10 relative">
      <motion.div ref={ref} initial="visible" animate={isPanelOpen ? "hidden" : "visible"} className="flex flex-col items-center gap-6 md:gap-8">
        {/* Subheading (motto) will now fade and move up slightly */}
        <motion.div variants={contentVariants} className="flex flex-col items-center leading-tight mb-4">
          <h2 className="text-lg md:text-xl font-light text-white/80 tracking-wide">Yapay Zeka</h2>
          <h2 className="text-lg md:text-xl font-light text-white/80 tracking-wide">Uygulama Merkezi</h2>
        </motion.div>

        {/* The "ALIM" container that orchestrates the stagger animation */}
        <motion.h1 variants={headlineVariants} className={`flex items-center justify-center gap-4 sm:gap-6 md:gap-20 text-6xl sm:text-8xl ${isPanelOpen ? "lg:text-8xl" : "lg:text-9xl"} tracking-tight font-semibold text-white`}>
          <MotionImage variants={letterVariants} src="/A Harfi Tek Kullanım (Negatif)_Çalışma Yüzeyi 1.svg" alt="Alim Logo" width={500} height={130} className="w-auto h-[0.78em] -mb-[0.03em]" priority />
          <motion.span variants={letterVariants} className="tracking-tighter">
            L
          </motion.span>
          <motion.span variants={letterVariants} className="tracking-tighter">
            İ
          </motion.span>
          <motion.span variants={letterVariants} className="tracking-tighter">
            M
          </motion.span>
        </motion.h1>

        {/* The button will now fade and move down slightly */}
        <motion.div variants={buttonVariants} ref={scope} className="mt-4">
          <button onMouseEnter={onMouseEnter} onMouseLeave={onMouseLeave} onClick={onOpenPanel} className="relative flex items-center px-8 py-4 rounded-full bg-white text-black cursor-pointer font-medium text-base transition-colors duration-300 hover:bg-white/85">
            <span className="button-text block whitespace-nowrap">Keşfet</span>
            <div className="icon-container w-0 opacity-0 overflow-hidden flex items-center justify-center">
              <motion.svg initial={{ x: -8, opacity: 0 }} xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24" strokeWidth={2} stroke="currentColor" className="enter-icon h-5 w-5">
                <path strokeLinecap="round" strokeLinejoin="round" d="M13.5 4.5 21 12m0 0-7.5 7.5M21 12H3" />
              </motion.svg>
            </div>
          </button>
        </motion.div>
      </motion.div>
    </main>
  );
});

HeroContent.displayName = "HeroContent";
export default HeroContent;
