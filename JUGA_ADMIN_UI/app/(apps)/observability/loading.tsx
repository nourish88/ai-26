// app/loading.tsx
import { Loader2 } from "lucide-react";
import Image from "next/image";

export default function Loading() {
  return (
    <div className="absolute inset-0 z-50 flex items-center justify-center bg-background/80 backdrop-blur-md rounded-xl">
      <div className="flex flex-col items-center gap-3">
        <div className="relative">
          <Loader2 className="h-8 w-8 animate-spin text-primary" />
          <div className="absolute inset-0 h-8 w-8 animate-ping rounded-full bg-primary/20" />
        </div>
        <span className="text-sm font-medium text-muted-foreground animate-pulse">Yükleniyor...</span>
      </div>
    </div>
  );
}
