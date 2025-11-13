"use client";

import { useOptimistic, useTransition, useCallback, useState } from "react";
import { useRouter, usePathname } from "next/navigation";
import Link from "next/link";
import { toast } from "sonner";
import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { MessageSquare, Plus, Settings, Trash2, Loader2, Clock } from "lucide-react";
import { Tooltip, TooltipContent, TooltipProvider, TooltipTrigger } from "@/components/ui/tooltip";

import { deleteChatAction } from "../_actions/chatActions";
import type { Conversation } from "../_types/message";
import { ConfirmDialog } from "@/components/confirm-dialog";
import Image from "next/image";

function optimisticReducer(state: Conversation[], { action, payload }: { action: "DELETE"; payload: string }) {
  switch (action) {
    case "DELETE":
      return state.filter((c) => c.thread_id !== payload);
    default:
      return state;
  }
}

function getRelativeTime(date: Date): string {
  const now = new Date();
  const diffInSeconds = Math.floor((now.getTime() - date.getTime()) / 1000);

  if (diffInSeconds < 60) return "Az önce";
  if (diffInSeconds < 3600) return `${Math.floor(diffInSeconds / 60)} dakika önce`;
  if (diffInSeconds < 86400) return `${Math.floor(diffInSeconds / 3600)} saat önce`;
  if (diffInSeconds < 604800) return `${Math.floor(diffInSeconds / 86400)} gün önce`;
  return date.toLocaleDateString("tr-TR", { month: "short", day: "numeric" });
}

interface SidebarProps {
  initialConversations: Conversation[];
  identifier: string; // Added identifier prop
}

export function Sidebar({ initialConversations, identifier }: SidebarProps) {
  const [optimisticConversations, dispatchOptimistic] = useOptimistic(initialConversations, optimisticReducer);
  const [isPending, startTransition] = useTransition();
  const [hoveredChat, setHoveredChat] = useState<string | null>(null);
  const isSidebarOpen = true;
  const router = useRouter();
  const pathname = usePathname();

  const handleDeleteChat = useCallback(
    (threadIdToDelete: string) => {
      startTransition(async () => {
        dispatchOptimistic({ action: "DELETE", payload: threadIdToDelete });
        const res = await deleteChatAction(threadIdToDelete, identifier);

        if (res.success) {
          toast.success("Sohbet başarıyla silindi.");
          if (pathname.includes(threadIdToDelete)) {
            router.push(`/apps/${identifier}?new=1`);
          }
        } else {
          toast.error("Bir hata oluştu: " + res.message);
          router.refresh();
        }
      });
    },
    [dispatchOptimistic, pathname, router, startTransition, identifier]
  );

  return (
    <TooltipProvider delayDuration={100}>
      <aside className={cn("bg-black/10 backdrop-blur-lg flex flex-col h-screen transition-all duration-300 ease-in-out border-r border-white/10", isSidebarOpen ? "w-72" : "w-20")}>
        <div className="p-4 border-b border-white/10 flex items-center justify-center p flex-shrink-0 bg-gradient-to-r from-blue-500/5 to-purple-500/5">
          <Link href={"/"}>
            <Image src="/Negatif-Yazılı_Çalışma Yüzeyi 1.svg" width={120} height={120} alt="alim" />
          </Link>
        </div>

        <div className="p-3 flex-shrink-0">
          <Link
            href={`/apps/${identifier}?new=1`}
            legacyBehavior={false}
            onClick={() => {
              router.push(`/apps/${identifier}?new=1`);
            }}
            className={cn(
              "flex items-center justify-start gap-3 w-full",
              "h-11 px-4 py-2",
              "bg-gradient-to-r from-blue-600 to-blue-500 text-white hover:from-blue-500 hover:to-blue-400",
              "inline-flex items-center justify-center whitespace-nowrap rounded-lg text-sm font-medium ring-offset-background transition-all duration-300 focus-visible:outline-none focus-visible:ring-2 focus-visible:ring-blue-500 focus-visible:ring-offset-2 disabled:pointer-events-none disabled:opacity-50",
              "shadow-lg shadow-blue-500/20 hover:shadow-xl hover:shadow-blue-500/30 hover:-translate-y-0.5"
            )}
          >
            <Plus className="w-5 h-5 flex-shrink-0" />
            <span className={cn("transition-opacity font-medium", !isSidebarOpen && "opacity-0")}>Yeni Sohbet</span>
          </Link>
        </div>

        <div className="flex-1 overflow-y-auto">
          <div className="p-3 pt-0 space-y-1.5">
            <div className={cn("text-xs font-semibold text-muted-foreground uppercase tracking-wider mb-3", !isSidebarOpen && "opacity-0")}>Son Sohbetler</div>
            {optimisticConversations.length > 0 ? (
              optimisticConversations.map((i) => {
                const isActive = pathname.includes(i.thread_id);
                const isHovered = hoveredChat === i.thread_id;

                return (
                  <div key={i.thread_id} className="flex items-center w-full gap-2 group" onMouseEnter={() => setHoveredChat(i.thread_id)} onMouseLeave={() => setHoveredChat(null)}>
                    <Link href={`/apps/${identifier}/${i.thread_id}`} className="flex-1 w-0" legacyBehavior={false}>
                      <Tooltip>
                        <TooltipTrigger asChild>
                          <div className={cn("w-full p-3 rounded-lg transition-all duration-200", "hover:bg-white/5 cursor-pointer", isActive && "bg-blue-500/10 border border-blue-500/30", !isActive && "border border-transparent")}>
                            <div className="flex items-start gap-3">
                              <MessageSquare className={cn("w-5 h-5 flex-shrink-0 mt-0.5 transition-colors", isActive ? "text-blue-400" : "text-foreground/60")} />
                              <div className="flex-1 min-w-0">
                                <p className={cn("truncate text-sm font-medium transition-colors", isActive ? "text-blue-300" : "text-foreground/90")}>{i.title}</p>
                                <p className="text-xs text-muted-foreground mt-0.5 flex items-center gap-1">
                                  <Clock className="w-3 h-3" />
                                  {getRelativeTime(new Date(i.created_at || Date.now()))}
                                </p>
                              </div>
                            </div>
                          </div>
                        </TooltipTrigger>
                        <TooltipContent side="right" align="center">
                          <p className="max-w-xs">{i.title}</p>
                        </TooltipContent>
                      </Tooltip>
                    </Link>

                    <ConfirmDialog title="Sohbeti Kalıcı Olarak Sil" description={`Bu eylem geri alınamaz. "${i.title}" başlıklı sohbeti kalıcı olarak silmek istediğinizden emin misiniz?`} onConfirm={() => handleDeleteChat(i.thread_id)} isPending={isPending}>
                      <Button variant="ghost" size="icon" className={cn("h-8 w-8 text-muted-foreground hover:text-red-400 hover:bg-red-500/10 transition-all duration-200", !isSidebarOpen && "hidden", isHovered ? "opacity-100" : "opacity-0")} disabled={isPending}>
                        {isPending ? <Loader2 className="h-4 w-4 animate-spin" /> : <Trash2 className="h-4 w-4" />}
                      </Button>
                    </ConfirmDialog>
                  </div>
                );
              })
            ) : (
              <div className={cn("p-8 text-center text-muted-foreground animate-in fade-in-50 duration-500", !isSidebarOpen && "hidden")}>
                <div className="p-4 bg-blue-500/10 rounded-full w-fit mx-auto mb-4">
                  <MessageSquare className="w-10 h-10 text-blue-400" />
                </div>
                <p className="font-medium text-foreground/80 mb-1">Henüz Sohbet Yok</p>
                <p className="text-sm leading-relaxed">Yeni bir sohbet başlatmak için yukarıdaki butonu kullanın.</p>
              </div>
            )}
          </div>
        </div>

        <div className="p-3 border-t border-white/10 flex-shrink-0">
          <Button variant="ghost" className="w-full justify-start gap-3 text-foreground/80 hover:text-foreground hover:bg-white/5 transition-all duration-200 rounded-lg">
            <Settings className="w-5 h-5 flex-shrink-0" />
            <span className={cn("transition-opacity", !isSidebarOpen && "opacity-0")}>Ayarlar</span>
          </Button>
        </div>
      </aside>
    </TooltipProvider>
  );
}
