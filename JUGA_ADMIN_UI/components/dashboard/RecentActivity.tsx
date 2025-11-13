import { Clock, Bot, Zap, MessageSquare, AlertTriangle, CheckCircle } from "lucide-react";

const activities = [
  {
    id: 1,
    agent: "Müşteri Destek Ajanı",
    action: "yeni bir müşteri sorusunu çözdü",
    time: "2 dakika önce",
    icon: CheckCircle,
    type: "success",
  },
  {
    id: 2,
    agent: "Satış Ajanı Pro",
    action: "potansiyel müşteri ile görüşme tamamladı",
    time: "15 dakika önce",
    icon: MessageSquare,
    type: "info",
  },
  {
    id: 3,
    agent: "Analiz Ajanı",
    action: "haftalık raporu oluşturdu",
    time: "1 saat önce",
    icon: Bot,
    type: "info",
  },
  {
    id: 4,
    agent: "Güvenlik Ajanı",
    action: "şüpheli aktivite tespit etti",
    time: "2 saat önce",
    icon: AlertTriangle,
    type: "warning",
  },
  {
    id: 5,
    agent: "Optimizasyon Ajanı",
    action: "sistem performansını iyileştirdi",
    time: "3 saat önce",
    icon: Zap,
    type: "success",
  },
];

export function RecentActivity() {
  const getIconColor = (type: string) => {
    switch (type) {
      case "success":
        return "text-chart-1 bg-chart-1/10";
      case "warning":
        return "text-chart-3 bg-chart-3/10";
      default:
        return "text-accent-foreground bg-accent";
    }
  };

  return (
    <div className="bg-card p-6 rounded-lg border border-border shadow">
      <div className="mb-6">
        <h3 className="text-lg font-semibold text-card-foreground">Son Aktiviteler</h3>
        <p className="text-sm text-muted-foreground">AI ajanlarınızın son işlemleri ve sistem olayları</p>
      </div>

      <div className="space-y-4">
        {activities.map((activity) => (
          <div key={activity.id} className="flex items-start space-x-3">
            <div className={`w-8 h-8 rounded-full flex items-center justify-center ${getIconColor(activity.type)}`}>
              <activity.icon className="w-4 h-4" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm text-card-foreground">
                <span className="font-medium">{activity.agent}</span> {activity.action}
              </p>
              <div className="flex items-center text-xs text-muted-foreground mt-1">
                <Clock className="w-3 h-3 mr-1" />
                {activity.time}
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="mt-6">
        <button className="w-full text-center text-sm text-primary hover:text-primary/80 font-medium">Tüm aktiviteleri görüntüle</button>
      </div>
    </div>
  );
}
