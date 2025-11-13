"use client";

import { PulsingBorder } from "@paper-design/shaders-react";
import { motion } from "framer-motion";
import Image from "next/image";

export default function PulsingCircle() {
  return (
    <motion.div className="fixed bottom-8 right-8 z-50 flex items-center justify-center" whileHover={{ scale: 1.1 }} transition={{ type: "spring", stiffness: 300 }}>
      <div className="relative w-32 h-32 flex items-center justify-center">
        {/* Pulsing Border Glow */}

        {/* 1. PERFECTLY CENTERED LOGO */}
        <div className="absolute inset-0 flex justify-center items-center">
          <Image src="/Jandarma Beyaz Logo_Çalışma Yüzeyi 1.svg" alt="Logo" width={30} height={30} className="opacity-70" />
        </div>

        {/* Inner Rotating Text - YAPAY ZEKA VE YAZILIM GELIŞTIRME */}
        <motion.svg
          className="absolute inset-0 w-full h-full"
          viewBox="0 0 100 100"
          animate={{ rotate: -360 }}
          transition={{
            duration: 30,
            repeat: Infinity,
            ease: "linear",
          }}
          style={{ transform: "scale(0.85)" }}
        >
          <defs>
            <path id="circle2" d="M 50, 50 m -30, 0 a 30,30 0 1,1 60,0 a 30,30 0 1,1 -60,0" />
          </defs>
          <text className="text-[6px] fill-white font-light tracking-wider uppercase">
            <textPath href="#circle2">YAPAY ZEKA VE YAZILIM GELIŞTIRME ŞUBE MÜDÜRLÜĞÜ</textPath>
          </text>
        </motion.svg>

        {/* Outer Rotating Text Ring */}
        <motion.svg
          className="absolute inset-0 w-full h-full"
          viewBox="0 0 100 100"
          animate={{ rotate: 360 }}
          transition={{
            duration: 45,
            repeat: Infinity,
            ease: "linear",
          }}
        >
          <defs>
            <path id="outerCircle" d="M 50, 50 m -40, 0 a 40,40 0 1,1 80,0 a 40,40 0 1,1 -80,0" />
          </defs>
          <text className="text-[5px] fill-white font-light tracking-widest uppercase">
            <textPath href="#outerCircle">YAZILIM GELİŞTİRME DAİRE BAŞKANLIĞI</textPath>
          </text>
        </motion.svg>
      </div>
    </motion.div>
  );
}
