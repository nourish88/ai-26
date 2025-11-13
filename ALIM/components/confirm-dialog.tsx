"use client";

import React from "react";
import { AlertDialog, AlertDialogAction, AlertDialogCancel, AlertDialogContent, AlertDialogDescription, AlertDialogFooter, AlertDialogHeader, AlertDialogTitle, AlertDialogTrigger } from "@/components/ui/alert-dialog";
import { Button } from "@/components/ui/button";
import { Loader2, AlertTriangle } from "lucide-react";

// Bileşenin alacağı props'ları tanımlıyoruz
interface ConfirmDialogProps {
  children: React.ReactNode; // Bu, diyaloğu tetikleyecek olan butondur
  title: string;
  description: string;
  onConfirm: () => void;
  isPending?: boolean; // Onay işleminin yüklenme durumunu takip etmek için
}

export function ConfirmDialog({ children, title, description, onConfirm, isPending = false }: ConfirmDialogProps) {
  return (
    <AlertDialog>
      <AlertDialogTrigger asChild>{children}</AlertDialogTrigger>
      <AlertDialogContent className="dark bg-black/50 backdrop-blur-lg border-border text-foreground shadow-lg">
        <AlertDialogHeader>
          <div className="flex items-center gap-4">
            <div className="flex-shrink-0 rounded-full bg-destructive/10 p-3 border border-destructive/20">
              <AlertTriangle className="h-6 w-6 text-destructive" />
            </div>
            <div>
              <AlertDialogTitle className="text-lg font-semibold text-foreground">{title}</AlertDialogTitle>
              <AlertDialogDescription className="mt-1 text-muted-foreground">{description}</AlertDialogDescription>
            </div>
          </div>
        </AlertDialogHeader>
        <AlertDialogFooter className="mt-4 sm:justify-end">
          {/* ⬇️ DÜZELTME: İptal Butonu Stilleri */}
          <AlertDialogCancel asChild>
            <Button variant="outline" className="text-white hover:text-white hover:opacity-80 transition-opacity">
              İptal
            </Button>
          </AlertDialogCancel>
          {/* ⬇️ DÜZELTME: Onay Butonu Stilleri */}
          <AlertDialogAction asChild>
            <Button variant="destructive" onClick={onConfirm} className="text-white hover:opacity-80 transition-opacity" disabled={isPending}>
              {isPending && <Loader2 className="mr-2 h-4 w-4 animate-spin" />}
              Evet, Onayla
            </Button>
          </AlertDialogAction>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
