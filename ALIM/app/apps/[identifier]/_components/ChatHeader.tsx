"use client";

import { Button } from "@/components/ui/button";
import { PanelLeftClose, PanelLeftOpen, FileDown, MoreVertical } from "lucide-react";

interface ChatHeaderProps {
  isSidebarOpen: boolean;
  hasMessages: boolean;
  onToggleSidebar: () => void;
  onExportPDF: () => void;
  identifier?: string | null;
  appName?: string;
}

export function ChatHeader({ isSidebarOpen, hasMessages, onToggleSidebar, onExportPDF, appName }: ChatHeaderProps) {
  return (
    <header className="bg-black/10 backdrop-blur-lg border-b border-white/10 p-4 flex items-center justify-between z-10">
      <div className="flex items-center gap-4">
        <Button variant="ghost" size="icon" onClick={onToggleSidebar} className="h-10 w-10 hover:bg-white/5 transition-colors rounded-lg">
          {isSidebarOpen ? <PanelLeftClose className="w-5 h-5" /> : <PanelLeftOpen className="w-5 h-5" />}
        </Button>
        <div>
          <h2 className="font-semibold text-base text-foreground">{appName}</h2>
          <div className="flex items-center gap-2 mt-0.5">
            <div className="relative">
              <div className="w-2 h-2 bg-green-500 rounded-full"></div>
              <div className="absolute inset-0 w-2 h-2 bg-green-500 rounded-full animate-ping opacity-75"></div>
            </div>
            <span className="text-xs text-muted-foreground">Çevrimiçi</span>
          </div>
        </div>
      </div>

      <div className="flex items-center gap-2">
        {hasMessages && (
          <Button variant="ghost" size="icon" onClick={onExportPDF} title="Export conversation to PDF" className="h-10 w-10 hover:bg-white/5 hover:text-blue-400 transition-all duration-200 rounded-lg">
            <FileDown className="w-5 h-5" />
          </Button>
        )}
        <Button variant="ghost" size="icon" className="h-10 w-10 hover:bg-white/5 transition-colors rounded-lg">
          <MoreVertical className="w-5 h-5" />
        </Button>
      </div>
    </header>
  );
}
