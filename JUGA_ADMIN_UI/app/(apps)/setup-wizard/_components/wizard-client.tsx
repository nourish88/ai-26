"use client";
import { Toaster } from "sonner";
import { WizardProvider, useWizard } from "../wizard-context";
import { WizardProgress } from "./wizard-progress";
import { AppStep } from "./steps/step-1-application";
import { Step2LLM } from "./steps/step-2-llm";
import { Step3DataIntegration } from "./steps/step-3-data-integration";
import { Step4Mcp } from "./steps/step-4-mcp";
import { Step5Summary } from "./steps/step-5-summary";

function WizardContent({ mcpServers, applicationTypes, memoryTypes, outputTypes, existingLlms, fetchChunkingStrategies, fileStores, searchEngines, extractors, embeddings }: any) {
  const { state } = useWizard();

  const renderStep = () => {
    switch (state.currentStep) {
      case "app":
        return <AppStep applicationTypes={applicationTypes} memoryTypes={memoryTypes} outputTypes={outputTypes} />;
      case "model":
        return <Step2LLM existingLlms={existingLlms} />;
      case "data-integration":
        return <Step3DataIntegration embeddings={embeddings} fetchChunkingStrategies={fetchChunkingStrategies} fileStores={fileStores} searchEngines={searchEngines} extractors={extractors} />;
      case "mcp":
        return <Step4Mcp mcpServers={mcpServers} />;
      case "summary":
        return <Step5Summary />;
      default:
        return <AppStep applicationTypes={applicationTypes} memoryTypes={memoryTypes} outputTypes={outputTypes} />;
    }
  };

  return (
    <div className="min-h-screen bg-background">
      {/* Header Section - Full Width Background */}
      <div className="border-b bg-card">
        <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
          <h1 className="text-3xl sm:text-4xl font-bold mb-2">Uygulama Kurulum Sihirbazı</h1>
          <p className="text-muted-foreground text-base sm:text-lg">AI ajanınızı yapılandırmak için adımları takip edin</p>
        </div>
      </div>

      {/* Progress Bar - Full Width */}
      <div className="border-b bg-muted/30">
        <div className="max-w-screen mx-auto px-4 sm:px-6 lg:px-8 py-6">
          <WizardProgress />
        </div>
      </div>

      {/* Main Content - Constrained Width */}
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-8">
        <div className="mt-4">{renderStep()}</div>
      </div>

      <Toaster position="top-center" richColors />
    </div>
  );
}

export function WizardClient({ mcpServers, applicationTypes, memoryTypes, outputTypes, existingLlms, fetchChunkingStrategies, fileStores, searchEngines, extractors, embeddings }: any) {
  return (
    <WizardProvider>
      <WizardContent
        mcpServers={mcpServers}
        applicationTypes={applicationTypes}
        memoryTypes={memoryTypes}
        outputTypes={outputTypes}
        existingLlms={existingLlms}
        fetchChunkingStrategies={fetchChunkingStrategies}
        fileStores={fileStores}
        searchEngines={searchEngines}
        extractors={extractors}
        embeddings={embeddings}
      />
    </WizardProvider>
  );
}
