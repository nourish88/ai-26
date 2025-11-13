"use client";
import { useEffect, useState } from "react";
import { Check } from "lucide-react";
import { cn } from "@/lib/utils";
// ✅ 1. REMOVE the static STEP_ORDER import. We get the steps from the hook now.
import { useWizard } from "../wizard-context";

const TITLES: Record<string, string> = {
  app: "Uygulamaya İlişkin Temel Tanımlamalar",
  model: "Model Tanımlama",
  "data-integration": "Modelin Kurumsal Veri Entegrasyonu",
  mcp: "Modelin Kurumsal Servis Entegrasyonu ",
  summary: "Özet ve Aktivasyon ",
};

export function WizardProgress() {
  // ✅ 2. Get `visibleSteps` from the useWizard hook. This is our dynamic array.
  const { state, goToStep, canNavigateToStep, isStepCompleted, visibleSteps } = useWizard();
  const [client, setClient] = useState(false);

  useEffect(() => {
    setClient(true);
  }, []);

  if (!client) return null;

  return (
    <nav aria-label="İlerleme">
      <ol className="flex items-center justify-center gap-2 overflow-x-auto pb-2">
        {/* ✅ 3. Map over the DYNAMIC `visibleSteps` array. */}
        {visibleSteps.map((step, index) => {
          const active = state.currentStep === step;
          const done = isStepCompleted(step);
          const can = canNavigateToStep(step);
          return (
            <li key={step} className="flex items-center gap-2 flex-shrink-0">
              <button
                type="button"
                onClick={() => can && goToStep(step)}
                disabled={!can}
                className={cn(
                  "flex items-center gap-3 rounded-lg px-4 py-3 transition-all border-2 min-w-[160px]",
                  active && "border-primary bg-primary/5 shadow-sm ring-2 ring-primary/20",
                  done && !active && "border-primary/30 bg-primary/5 hover:bg-primary/10",
                  !active && !done && can && "border-border hover:border-primary/50 hover:bg-accent",
                  !can && "opacity-50 cursor-not-allowed"
                )}
              >
                <div
                  className={cn(
                    "flex h-8 w-8 items-center justify-center rounded-full border-2 text-sm font-semibold",
                    done && "border-primary bg-primary text-primary-foreground",
                    active && !done && "border-primary bg-background text-primary",
                    !active && !done && "border-muted-foreground/30 bg-background text-muted-foreground"
                  )}
                >
                  {done ? <Check className="h-5 w-5" /> : <span>{index + 1}</span>}
                </div>
                <span className={cn("text-sm font-medium", active ? "text-foreground" : "text-muted-foreground")}>{TITLES[step]}</span>
              </button>
              {/* ✅ 4. The connector line now correctly uses the length of the dynamic array. */}
              {index < visibleSteps.length - 1 && <div className={cn("h-0.5 w-8", isStepCompleted(step) ? "bg-primary" : "bg-border")} />}
            </li>
          );
        })}
      </ol>
    </nav>
  );
}
