// components/ui/InfoCard.tsx
import React from "react";
import { ArrowUpRight, ArrowDownRight, LucideIcon } from "lucide-react";

/**
 * Props for the InfoCard component
 */
export interface InfoCardProps {
  /** The title/label displayed at the top of the card */
  title: string;
  /** The main value/metric displayed prominently */
  value: string;
  /** Lucide React icon component to display */
  icon: LucideIcon;
  /** Optional change indicator with trend arrow */
  change?: {
    /** The change value (e.g., "+12%", "-3 users", "+₺1,234") */
    value: string;
    /** Whether this change is positive (increase) or negative (decrease) */
    type: "increase" | "decrease";
    /** Optional description text shown after the change value */
    description?: string;
  };
  /** Additional CSS classes to apply to the card */
  className?: string;
}

/**
 * InfoCard - A reusable card component for displaying metrics, stats, or any key information
 *
 * @example
 * // Basic usage with just title, value, and icon
 * <InfoCard
 *   title="Total Users"
 *   value="1,234"
 *   icon={Users}
 * />
 *
 * @example
 * // With change indicator showing positive trend
 * <InfoCard
 *   title="Revenue"
 *   value="₺45,230"
 *   icon={DollarSign}
 *   change={{
 *     value: "+12.5%",
 *     type: "increase",
 *     description: "vs last month"
 *   }}
 * />
 *
 * @example
 * // With change indicator showing negative trend
 * <InfoCard
 *   title="Server Errors"
 *   value="23"
 *   icon={AlertTriangle}
 *   change={{
 *     value: "+5 errors",
 *     type: "decrease", // Note: decrease = red color, even if value increased
 *     description: "today"
 *   }}
 * />
 *
 * @example
 * // With custom styling
 * <InfoCard
 *   title="Status"
 *   value="Online"
 *   icon={Server}
 *   className="border-green-200 bg-green-50"
 * />
 */
export function InfoCard({ title, value, icon: Icon, change, className = "" }: InfoCardProps) {
  return (
    <div className={`bg-card p-6 rounded-lg border border-border shadow ${className}`}>
      {/* Header section with title/value and icon */}
      <div className="flex items-center justify-between">
        <div>
          {/* Card title - smaller, muted text */}
          <p className="text-sm font-medium text-muted-foreground">{title}</p>
          {/* Main value - large, prominent text */}
          <p className="text-3xl font-bold text-card-foreground">{value}</p>
        </div>

        {/* Icon container - circular background with primary color */}
        <div className="w-12 h-12 bg-primary/10 rounded-lg flex items-center justify-center">
          <Icon className="w-6 h-6 text-primary" />
        </div>
      </div>

      {/* Optional change indicator section */}
      {change && (
        <div className="mt-4 flex items-center">
          {/* Trend arrow - up for increase (green), down for decrease (red) */}
          {change.type === "increase" ? <ArrowUpRight className="w-4 h-4 text-chart-1 mr-1" /> : <ArrowDownRight className="w-4 h-4 text-destructive mr-1" />}

          {/* Change value with color based on trend type */}
          <span className={`text-sm font-medium ${change.type === "increase" ? "text-chart-1" : "text-destructive"}`}>{change.value}</span>

          {/* Optional description text */}
          {change.description && <span className="text-sm text-muted-foreground ml-1">{change.description}</span>}
        </div>
      )}
    </div>
  );
}
