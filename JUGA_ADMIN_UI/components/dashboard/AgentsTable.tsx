"use client";
// import { MoreHorizontal, Bot, Activity, Clock, Zap } from "lucide-react";

import { ColumnDef } from "@tanstack/react-table";
import { Agent } from "http";
import { Bot, Clock, Activity, Zap, MoreHorizontal } from "lucide-react";
import { DataTable } from "../ui/datatable";
import { Button } from "../ui/button";

export const getStatusColor = (status: string) => {
  switch (status) {
    case "Aktif":
      return "bg-chart-1/20 text-chart-1";
    case "Bakımda":
      return "bg-chart-3/20 text-chart-3";
    default:
      return "bg-muted text-muted-foreground";
  }
};

export const getTypeColor = (type: string) => {
  const colors = ["bg-primary/20 text-primary", "bg-chart-2/20 text-chart-2", "bg-chart-4/20 text-chart-4", "bg-accent/20 text-accent-foreground"];
  const hash = Array.from(type).reduce((acc, char) => acc + char.charCodeAt(0), 0);
  return colors[hash % colors.length];
};

export const columns: ColumnDef<any>[] = [
  {
    accessorKey: "name",
    header: "Ajan",
    cell: ({ row }) => {
      const agent = row.original;
      return (
        <div className="flex items-center">
          <div className="w-10 h-10 bg-primary/20 rounded-full flex items-center justify-center">
            <Bot className="w-5 h-5 text-primary" />
          </div>
          <div className="ml-4">
            <div className="text-sm font-medium text-card-foreground">{agent.name}</div>
            <div className="text-sm text-muted-foreground flex items-center">
              <Clock className="w-3 h-3 mr-1" />
              {agent.lastActive}
            </div>
          </div>
        </div>
      );
    },
  },
  {
    accessorKey: "type",
    header: "Tip",
    cell: ({ row }) => {
      const type = row.getValue("type") as string;
      return <span className={`inline-flex px-2 py-1 text-xs font-semibold rounded-md ${getTypeColor(type)}`}>{type}</span>;
    },
  },
  {
    accessorKey: "status",
    header: "Durum",
    cell: ({ row }) => {
      const status = row.getValue("status") as string;
      return (
        <span className={`inline-flex items-center px-2 py-1 text-xs font-semibold rounded-md ${getStatusColor(status)}`}>
          <Activity className="w-3 h-3 mr-1" />
          {status}
        </span>
      );
    },
  },
  {
    accessorKey: "conversations",
    header: "Konuşmalar",
    cell: ({ row }) => {
      const amount = parseFloat(row.getValue("conversations"));
      return <div className="text-sm text-card-foreground font-medium">{amount.toLocaleString("tr-TR")}</div>;
    },
  },
  {
    accessorKey: "successRate",
    header: "Başarı Oranı",
    cell: ({ row }) => <div className="text-sm text-card-foreground font-medium">%{row.getValue("successRate")}</div>,
  },
  {
    accessorKey: "avgResponseTime",
    header: "Yanıt Süresi",
    cell: ({ row }) => (
      <div className="flex items-center text-sm text-card-foreground">
        <Zap className="w-3 h-3 mr-1 text-chart-1" />
        {row.getValue("avgResponseTime")}
      </div>
    ),
  },
  {
    id: "actions",
    header: () => <div className="text-right">İşlemler</div>,
    cell: () => (
      <div className="text-right text-sm font-medium">
        <button className="text-muted-foreground hover:text-foreground">
          <MoreHorizontal className="w-5 h-5" onClick={() => alert("asd")} />
        </button>
      </div>
    ),
  },
];

export function AgentsTable() {
  const agentsData = [
    { id: 1, name: "Müşteri Destek Pro", type: "Müşteri Hizmetleri", status: "Aktif", conversations: 156, successRate: 94.8, avgResponseTime: "0.6s", lastActive: "2 dakika önce" },
    { id: 2, name: "Satış Uzmanı AI", type: "Satış & Pazarlama", status: "Aktif", conversations: 89, successRate: 91.2, avgResponseTime: "0.9s", lastActive: "5 dakika önce" },
    { id: 3, name: "Teknik Destek Ajanı", type: "Teknik Destek", status: "Bakımda", conversations: 203, successRate: 96.1, avgResponseTime: "1.2s", lastActive: "1 saat önce" },
    { id: 4, name: "Veri Analiz Ajanı", type: "Analitik", status: "Aktif", conversations: 45, successRate: 98.7, avgResponseTime: "0.4s", lastActive: "10 dakika önce" },
  ];

  return (
    // 3. Use the generic DataTable and pass props to configure it
    <DataTable
      columns={columns}
      data={agentsData}
      title="AI Ajanlar"
      description="Tüm AI ajanlarınızı yönetin ve izleyin"
      filterColumn="name"
      filterPlaceholder="Ajan adına göre filtrele..."
      actionButton={<Button className="px-4 py-2 bg-primary text-primary-foreground rounded-lg hover:bg-primary/90 transition-colors text-sm font-medium">Yeni Ajan Ekle</Button>}
    />
  );
}
