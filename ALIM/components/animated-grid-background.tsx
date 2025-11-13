// components/animated-grid-background.tsx
"use client";

import { motion } from "framer-motion";
import { useEffect, useMemo, useState } from "react";

interface AnimatedGridBackgroundProps {
  logo?: React.ReactNode;
}

export function AnimatedGridBackground({ logo }: AnimatedGridBackgroundProps) {
  const [dimensions, setDimensions] = useState({ width: 0, height: 0 });

  useEffect(() => {
    const updateDimensions = () => {
      setDimensions({
        width: window.innerWidth,
        height: window.innerHeight,
      });
    };

    updateDimensions();
    window.addEventListener("resize", updateDimensions);
    return () => window.removeEventListener("resize", updateDimensions);
  }, []);

  const spacing = 50;
  const dotSize = 1.5;
  const rippleSpeed = 0.005; // Smaller value = faster ripple

  // useMemo will re-calculate dots only when dimensions change
  const dots = useMemo(() => {
    if (dimensions.width === 0) return [];

    const centerX = dimensions.width / 2;
    const centerY = dimensions.height / 2;
    const cols = Math.ceil(dimensions.width / spacing);
    const rows = Math.ceil(dimensions.height / spacing);

    const newDots = [];
    for (let i = 0; i < cols; i++) {
      for (let j = 0; j < rows; j++) {
        const dotX = i * spacing;
        const dotY = j * spacing;

        // The key change: Calculate distance from the center
        const distance = Math.sqrt(Math.pow(dotX - centerX, 2) + Math.pow(dotY - centerY, 2));

        newDots.push({
          x: dotX,
          y: dotY,
          // Delay is now proportional to the distance from the center
          delay: distance * rippleSpeed,
          id: `${i}-${j}`,
        });
      }
    }
    return newDots;
  }, [dimensions]);

  return (
    <div className="fixed inset-0 pointer-events-none z-0 overflow-hidden flex items-center justify-center">
      {/* Optional: Add your logo here. It will be centered. */}
      {logo && <div className="relative z-20">{logo}</div>}

      {/* Static CSS Grid Lines */}
      <div
        className="absolute inset-0 z-0"
        style={{
          backgroundImage: `
            linear-gradient(hsl(var(--border) / 0.15) 1px, transparent 1px),
            linear-gradient(90deg, hsl(var(--border) / 0.15) 1px, transparent 1px)
          `,
          backgroundSize: `${spacing}px ${spacing}px`,
        }}
      />

      {/* Pulsing Dots SVG */}
      <svg className="absolute inset-0 w-full h-full z-10">
        {dots.map((dot) => (
          <motion.circle
            key={dot.id}
            cx={dot.x}
            cy={dot.y}
            r={dotSize}
            fill="hsl(var(--primary) / 0.5)"
            initial={{ opacity: 0, scale: 0 }}
            animate={{
              opacity: [0, 1, 0],
              scale: [0, 1.5, 0],
            }}
            transition={{
              duration: 2,
              delay: dot.delay,
              repeat: Number.POSITIVE_INFINITY,
              ease: "easeInOut",
            }}
          />
        ))}
      </svg>
    </div>
  );
}
