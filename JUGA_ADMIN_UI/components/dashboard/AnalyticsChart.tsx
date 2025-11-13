import { ArrowUpRight, ArrowDownRight, Bot, Zap, MessageSquare, TrendingUp } from "lucide-react";

export function AnalyticsChart() {
  const data = [
    { month: "Oca", conversations: 420 },
    { month: "Şub", conversations: 680 },
    { month: "Mar", conversations: 590 },
    { month: "Nis", conversations: 820 },
    { month: "May", conversations: 750 },
    { month: "Haz", conversations: 920 },
    { month: "Tem", conversations: 890 },
    { month: "Ağu", conversations: 1150 },
    { month: "Eyl", conversations: 980 },
    { month: "Eki", conversations: 1050 },
    { month: "Kas", conversations: 1180 },
    { month: "Ara", conversations: 1300 },
  ];

  const maxValue = Math.max(...data.map((d) => d.conversations));

  return (
    <div className="bg-card p-6 rounded-lg border border-border shadow">
      <div className="mb-6">
        <h3 className="text-lg font-semibold text-card-foreground">AI Ajan Performansı</h3>
        <p className="text-sm text-muted-foreground">Aylık konuşma sayısı ve başarı oranı</p>
      </div>

      <div className="h-80 flex items-end justify-between space-x-2">
        {data.map((item, index) => (
          <div key={index} className="flex-1 flex flex-col items-center">
            <div className="w-full bg-primary rounded-t-sm transition-all hover:bg-primary/80 cursor-pointer group relative" style={{ height: `${(item.conversations / maxValue) * 100}%` }}>
              <div className="absolute -top-8 left-1/2 transform -translate-x-1/2 bg-popover border border-border rounded px-2 py-1 text-xs opacity-0 group-hover:opacity-100 transition-opacity">{item.conversations}</div>
            </div>
            <span className="text-xs text-muted-foreground mt-2">{item.month}</span>
          </div>
        ))}
      </div>

      <div className="mt-6 flex items-center justify-between text-sm">
        <div className="flex items-center space-x-4">
          <div className="flex items-center">
            <div className="w-3 h-3 bg-primary rounded-full mr-2"></div>
            <span className="text-muted-foreground">Toplam Konuşma</span>
          </div>
        </div>
        <span className="text-card-foreground font-medium">Bu ay: {data[data.length - 1].conversations} konuşma</span>
      </div>
    </div>
  );
}
