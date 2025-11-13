// components/DashboardStats.tsx
import { Bot, Zap, MessageSquare, TrendingUp } from "lucide-react";
import { CardGrid } from "../ui/CardGrid";
import { InfoCard } from "../ui/InfoCard";

/**
 * DashboardStats - Main dashboard statistics section
 *
 * This component demonstrates how to use InfoCard and CardGrid together
 * to create a responsive dashboard with key metrics.
 *
 * Layout:
 * - Mobile (sm): 1 column - cards stack vertically
 * - Tablet (md): 2 columns - cards in 2x2 grid
 * - Desktop (lg): 4 columns - all cards in single row
 */
export function DashboardStats() {
  return (
    <CardGrid
      // FIX: Changed columns to match the responsive behavior described above.
      // It will now be 1 column on small screens, 2 on medium, and 4 on large.
      columns={{ sm: 1, md: 2, lg: 4 }}
      className="mb-8" // Custom spacing
    >
      {/* Active Agents Card - Shows number with positive trend */}
      <InfoCard
        title="Aktif Ajanlar"
        value="15"
        icon={Bot}
        change={{
          value: "+3 ajan", // Custom change format (not percentage)
          type: "increase", // Green color with up arrow
          description: "Bu ay", // Context for the change
        }}
      />

      {/* Total Conversations Card - Shows count with percentage increase */}
      <InfoCard
        title="Toplam Konuşma"
        value="2,847"
        icon={MessageSquare}
        change={{
          value: "+18.2%", // Percentage increase
          type: "increase",
          description: "Son 30 gün",
        }}
      />

      {/* Success Rate Card - Shows percentage with improvement */}
      <InfoCard
        title="Başarı Oranı"
        value="%94.8"
        icon={TrendingUp}
        change={{
          value: "+2.1%",
          type: "increase",
          description: "Görev tamamlama",
        }}
      />

      {/* Response Time Card - Shows decrease as positive (faster = better) */}
      <InfoCard
        title="Yanıt Süresi"
        value="0.8s"
        icon={Zap}
        change={{
          value: "-0.3s", // Negative value but positive trend
          type: "increase", // Green because faster response is better
          description: "Ortalama",
        }}
      />
    </CardGrid>
  );
}
