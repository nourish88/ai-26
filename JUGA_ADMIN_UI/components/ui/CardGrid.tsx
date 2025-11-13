// components/ui/CardGrid.tsx
import React from "react";

/**
 * Props for the CardGrid component
 */
interface CardGridProps {
  /** Child components to render in the grid (typically InfoCard components) */
  children: React.ReactNode;
  /**
   * Responsive column configuration for different screen sizes
   * @default { sm: 1, md: 2, lg: 4 }
   */
  columns?: {
    /** Columns on small screens (e.g., mobile) */
    sm?: 1 | 2 | 3 | 4 | 5 | 6;
    /** Columns on medium screens (e.g., tablet) */
    md?: 1 | 2 | 3 | 4 | 5 | 6;
    /** Columns on large screens (e.g., desktop) */
    lg?: 1 | 2 | 3 | 4 | 5 | 6;
    /** Columns on extra large screens (e.g., large desktop) */
    xl?: 1 | 2 | 3 | 4 | 5 | 6;
  };
  /** Additional CSS classes to apply to the grid container */
  className?: string;
}

/**
 * CardGrid - A responsive grid container for organizing cards and other components
 *
 * Features:
 * - Responsive design with customizable column counts per breakpoint
 * - Automatic gap spacing between items
 * - Works with any child components, not just InfoCard
 */
export function CardGrid({
  children,
  columns = { sm: 1, md: 2, lg: 4 }, // Default: 1 col mobile, 2 col tablet, 4 col desktop
  className = "mb-8", // Default margin bottom
}: CardGridProps) {
  /**
   * Generates Tailwind CSS grid classes based on the columns configuration.
   *
   * FIX: This function now uses a mapping to ensure complete class names are
   * present in the source code, which is required for Tailwind's JIT compiler.
   */
  const getGridClasses = () => {
    const baseClass = "grid gap-6";

    // Mappings from number to full Tailwind class name.
    // This ensures Tailwind's JIT compiler can find these classes.
    const smMap = {
      1: "grid-cols-1",
      2: "grid-cols-2",
      3: "grid-cols-3",
      4: "grid-cols-4",
      5: "grid-cols-5",
      6: "grid-cols-6",
    };
    const mdMap = {
      1: "md:grid-cols-1",
      2: "md:grid-cols-2",
      3: "md:grid-cols-3",
      4: "md:grid-cols-4",
      5: "md:grid-cols-5",
      6: "md:grid-cols-6",
    };
    const lgMap = {
      1: "lg:grid-cols-1",
      2: "lg:grid-cols-2",
      3: "lg:grid-cols-3",
      4: "lg:grid-cols-4",
      5: "lg:grid-cols-5",
      6: "lg:grid-cols-6",
    };
    const xlMap = {
      1: "xl:grid-cols-1",
      2: "xl:grid-cols-2",
      3: "xl:grid-cols-3",
      4: "xl:grid-cols-4",
      5: "xl:grid-cols-5",
      6: "xl:grid-cols-6",
    };

    const colClasses = [];
    if (columns.sm) colClasses.push(smMap[columns.sm]);
    if (columns.md) colClasses.push(mdMap[columns.md]);
    if (columns.lg) colClasses.push(lgMap[columns.lg]);
    if (columns.xl) colClasses.push(xlMap[columns.xl]);

    return `${baseClass} ${colClasses.join(" ")} ${className}`;
  };

  return <div className={getGridClasses()}>{children}</div>;
}
