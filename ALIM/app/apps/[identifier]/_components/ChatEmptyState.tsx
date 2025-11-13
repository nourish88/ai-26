"use client";

import { Bot, Lightbulb, Code, Sparkles, MessageCircle, Zap } from "lucide-react";
import Image from "next/image";

const examplePrompts = [
  { text: "Bana React'ta bir bileşen yaz", icon: Code },
  { text: "Tailwind CSS için en iyi pratikler nelerdir?", icon: Sparkles },
  { text: "Bir sonraki projem için fikirler üret", icon: Lightbulb },
  { text: "JavaScript'teki 'closure' kavramını açıkla", icon: MessageCircle },
];

interface ChatEmptyStateProps {
  onSendMessage: (message: string) => void;
}

export function ChatEmptyState({ onSendMessage }: ChatEmptyStateProps) {
  return (
    <div className="flex flex-col items-center justify-center h-full pt-20 text-center animate-in fade-in duration-500">
      <div className="relative p-6   shadow-blue-500/10">
        <div className="absolute inset-0 bg-gradient-to-br rounded-2xl blur-xl"></div>
        <Image
          src="/A Harfi Tek Kullanım (Renkli).svg"
          alt="Logo"
          width={240}
          height={240}
          className="opacity-60 " // Added a subtle blur for better integration
        />
        {/* <Bot className="w-14 h-14 text-blue-400 relative z-10 animate-pulse" /> */}
      </div>

      <h1 className="text-4xl font-bold mb-3 text-balance bg-gradient-to-r from-blue-400 to-purple-400 bg-clip-text text-transparent">Nasıl yardımcı olabilirim?</h1>
      <p className="text-muted-foreground mb-12 max-w-md leading-relaxed text-base">Bana bir soru sorun, bir görevi tamamlamamı isteyin veya aşağıdaki örneklerden birini deneyin.</p>

      {/* <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 w-full max-w-2xl px-4">
        {examplePrompts.map((prompt, i) => {
          const Icon = prompt.icon;
          return (
            <button
              key={i}
              onClick={() => onSendMessage(prompt.text)}
              className="group text-left p-5 rounded-xl bg-gradient-to-br from-black/30 to-black/20 hover:from-blue-500/20 hover:to-purple-500/20 backdrop-blur-sm transition-all duration-300 text-sm text-foreground/90 border border-white/10 hover:border-blue-500/50 hover:shadow-lg hover:shadow-blue-500/10 hover:-translate-y-1"
            >
              <div className="flex items-start gap-3">
                <div className="p-2 rounded-lg bg-blue-500/10 group-hover:bg-blue-500/20 transition-colors">
                  <Icon className="w-5 h-5 text-blue-400 group-hover:text-blue-300 transition-colors" />
                </div>
                <span className="flex-1 leading-relaxed pt-1">{prompt.text}</span>
              </div>
            </button>
          );
        })}
      </div> */}

      {/* <div className="mt-12 flex items-center gap-2 text-xs text-muted-foreground">
        <Zap className="w-4 h-4" />
        <span>Shift + Enter ile yeni satır ekleyebilirsiniz</span>
      </div> */}
    </div>
  );
}
