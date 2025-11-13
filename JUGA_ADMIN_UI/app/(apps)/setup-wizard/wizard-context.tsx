"use client";
import { createContext, useContext, useEffect, useState, type ReactNode } from "react";
import { useSearchParams, useRouter, usePathname } from "next/navigation";
import { toast } from "sonner";
import type { WizardState, WizardStep } from "./_types/types";

interface WizardContextType {
  state: WizardState;
  // ✅ 1. Expose the new dynamic step list to all components
  visibleSteps: WizardStep[];
  updateData: <K extends keyof WizardState["data"]>(key: K, value: WizardState["data"][K]) => void;
  updateState: (updates: Partial<WizardState>) => void;
  goToStep: (step: WizardStep) => void;
  goToNextStep: () => void;
  completeStep: (step: WizardStep) => void;
  isStepCompleted: (step: WizardStep) => boolean;
  canNavigateToStep: (step: WizardStep) => boolean;
  resetWizard: () => void;
}

// ✅ 2. This constant now represents ALL possible steps in the flow.
const ALL_POSSIBLE_STEPS: WizardStep[] = ["app", "model", "data-integration", "mcp", "summary"];

const STORAGE_KEY = "wizardState";

const defaultState: WizardState = {
  currentStep: "app",
  completedSteps: [],
  data: {
    application: undefined,
    llm: undefined,
    fileStore: null,
    searchEngine: null,
    chunkingStrategy: null,
    extractorEngine: null,
    mcpServer: null,
  },
  application: null,
};

const WizardContext = createContext<WizardContextType | undefined>(undefined);

export function WizardProvider({ children }: { children: ReactNode }) {
  const [state, setState] = useState<WizardState>(defaultState);
  const [hydrated, setHydrated] = useState(false);

  // ✅ 3. Create the dynamic state for the visible steps.
  const [visibleSteps, setVisibleSteps] = useState<WizardStep[]>(["app", "model", "summary"]);

  const sp = useSearchParams();
  const router = useRouter();
  const pathname = usePathname();

  // Rehydration logic remains the same
  useEffect(() => {
    const saved = (() => {
      try {
        return JSON.parse(localStorage.getItem(STORAGE_KEY) || "null");
      } catch {
        return null;
      }
    })();
    const stepFromUrl = (sp.get("step") as WizardStep) || undefined;
    const appIdFromUrl = sp.get("applicationId");

    const base = (saved ?? defaultState) as WizardState;
    setState({ ...base, currentStep: stepFromUrl || base.currentStep });

    if (!appIdFromUrl) localStorage.removeItem(STORAGE_KEY);
    setHydrated(true);
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, []);

  // Persistence logic remains the same
  useEffect(() => {
    if (!hydrated) return;
    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(state));
    } catch {}
  }, [state, hydrated]);

  // ✅ 4. The main logic: This effect recalculates the workflow whenever the application data changes.
  useEffect(() => {
    const appConfig = state.data.application;

    // If there's no config yet, we can't determine the full path.
    // We start with a minimal path. This will be updated as soon as Step 1 is complete.
    if (!appConfig) {
      return;
    }

    const newVisibleSteps = ALL_POSSIBLE_STEPS.filter((step) => {
      // Core steps are always required.
      if (step === "app" || step === "model" || step === "summary") {
        return true;
      }

      // The 'data-integration' step is required only if file handling is enabled.
      if (step === "data-integration") {
        return appConfig.hasApplicationFile || appConfig.hasUserFile;
      }

      // The 'mcp' step is required only for the specific MCP-powered agent type.
      if (step === "mcp") {
        return appConfig.agentType === "MCP_POWERED_AGENTIC_RAG";
      }

      return false; // Should not happen, but good for safety.
    });

    setVisibleSteps(newVisibleSteps);
  }, [state.data.application]);

  const updateState = (newState: Partial<WizardState>) => {
    setState((prevState) => ({ ...prevState, ...newState }));
  };

  const updateData = <K extends keyof WizardState["data"]>(key: K, value: WizardState["data"][K]) => {
    setState((prev) => ({ ...prev, data: { ...prev.data, [key]: value } }));
  };

  const goToStep = (step: WizardStep) => {
    const params = new URLSearchParams(sp.toString());
    params.set("step", step);
    router.replace(`${pathname}?${params.toString()}`, { scroll: false });
    setState((prev) => ({ ...prev, currentStep: step }));
  };

  const goToNextStep = () => {
    // ✅ 5. Use the DYNAMIC visibleSteps for navigation, not the static one.
    const idx = visibleSteps.indexOf(state.currentStep);
    if (idx < visibleSteps.length - 1) {
      goToStep(visibleSteps[idx + 1]);
    }
  };

  const completeStep = (step: WizardStep) => {
    setState((prev) => (prev.completedSteps.includes(step) ? prev : { ...prev, completedSteps: [...prev.completedSteps, step] }));
  };

  const isStepCompleted = (step: WizardStep) => state.completedSteps.includes(step);

  const canNavigateToStep = (step: WizardStep) => {
    // ✅ 6. Use the DYNAMIC visibleSteps for validation.
    const idx = visibleSteps.indexOf(step);
    if (idx === -1) return false; // Can't navigate to a step that isn't in the current workflow.
    if (idx === 0) return true;
    // Check if all *previous visible* steps are complete.
    return visibleSteps.slice(0, idx).every((s) => state.completedSteps.includes(s));
  };

  const resetWizard = () => {
    router.replace(pathname, { scroll: false });
    localStorage.removeItem(STORAGE_KEY);
    setState(defaultState);
    toast.info("Sihirbaz sıfırlandı.");
  };

  const value: WizardContextType = {
    state,
    visibleSteps, // ✅ 7. Provide the dynamic steps to the rest of the app.
    updateData,
    updateState,
    goToStep,
    goToNextStep,
    completeStep,
    isStepCompleted,
    canNavigateToStep,
    resetWizard,
  };

  return <WizardContext.Provider value={value}>{hydrated ? children : null}</WizardContext.Provider>;
}

export function useWizard() {
  const ctx = useContext(WizardContext);
  if (!ctx) throw new Error("useWizard must be used within a WizardProvider");
  return ctx;
}
