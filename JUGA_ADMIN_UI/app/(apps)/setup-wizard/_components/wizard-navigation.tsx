"use client";
import { Button } from "@/components/ui/button";

export function WizardNavigation({ onNext, nextLabel = "Devam Et", backLabel = "Geri", isNextDisabled = false, isLoading = false }: { onNext: () => Promise<boolean> | boolean; nextLabel?: string; backLabel?: string; isNextDisabled?: boolean; isLoading?: boolean }) {
  const handle = async () => {
    await onNext();
  };
  return (
    <div className="flex justify-end gap-2">
      {/* back button can be wired later if you keep a goBack */}
      <Button onClick={handle} disabled={isNextDisabled || isLoading}>
        {nextLabel}
      </Button>
    </div>
  );
}
