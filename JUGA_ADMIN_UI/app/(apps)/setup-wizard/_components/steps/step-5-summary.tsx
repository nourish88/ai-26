"use client";
import { useEffect, useState, useRef } from "react";
import { motion, AnimatePresence } from "framer-motion";
import confetti from "canvas-confetti";
import { toast } from "sonner";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Loader2, CheckCircle2, AlertCircle, Server, Database, Zap, FileText, Scissors, Search, Info, XCircle, RefreshCcw, Sparkles, Brain, Network, ChevronDown } from "lucide-react";
import { Badge } from "@/components/ui/badge";
import { useRouter } from "next/navigation";
import { useWizardParams } from "../../_hooks/use-wizard-params";
import { createApplication } from "../../_services/application.service";
import { createAppChunkingStrategy } from "../../_services/chunking.service";
import { createExtractorEngine } from "../../_services/extractor.service";
import { createApplicationFileStore } from "../../_services/file-store.service";
import { createIndex } from "../../_services/index.service";
import { createapplicationllms } from "../../_services/llm.service";
import { createAppSearchEngines } from "../../_services/search-engines.service";
import { useWizard } from "../../wizard-context";
import { createApplicationMcp } from "../../_services/mcp_mutation.service";
import { cn } from "@/lib/utils";
import { createApplicationEmbedding } from "../../_services/embeddings.service";

// AI Brain Loading Animation
const AIBrainLoader = () => {
  return (
    <div className="relative w-24 h-24">
      <div className="absolute inset-0 border-4 border-primary/20 rounded-full animate-ping" />
      <div className="absolute inset-0">
        {[...Array(8)].map((_, i) => (
          <motion.div
            key={i}
            className="absolute w-3 h-3 bg-primary rounded-full"
            initial={{ scale: 0 }}
            animate={{
              scale: [1, 1.5, 1],
              opacity: [0.5, 1, 0.5],
            }}
            transition={{
              duration: 2,
              repeat: Infinity,
              delay: i * 0.2,
            }}
            style={{
              top: `${50 + 38 * Math.sin((i * Math.PI) / 4)}%`,
              left: `${50 + 38 * Math.cos((i * Math.PI) / 4)}%`,
            }}
          />
        ))}
      </div>
      <div className="absolute inset-0 flex items-center justify-center">
        <motion.div
          animate={{
            rotate: [0, 360],
            scale: [1, 1.1, 1],
          }}
          transition={{
            duration: 3,
            repeat: Infinity,
            ease: "easeInOut",
          }}
        >
          <Brain className="h-10 w-10 text-primary" />
        </motion.div>
      </div>
      <motion.div className="absolute inset-0 border-2 border-primary/30 rounded-full" animate={{ scale: [1, 1.5, 1], opacity: [1, 0, 1] }} transition={{ duration: 2, repeat: Infinity }} />
    </div>
  );
};

// Animated Progress Item
const AnimatedProgressItem = ({ item, index }: any) => {
  return (
    <motion.div
      initial={{ opacity: 0, x: -20 }}
      animate={{ opacity: 1, x: 0 }}
      transition={{ delay: index * 0.1 }}
      className={cn(
        "flex items-start gap-3 p-4 rounded-xl border-2 transition-all duration-300",
        item.status === "success" && "bg-gradient-to-r from-green-500/10 to-green-600/10 border-green-500/30 shadow-lg shadow-green-500/20",
        item.status === "error" && "bg-gradient-to-r from-red-500/10 to-red-600/10 border-red-500/30 shadow-lg shadow-red-500/20",
        item.status === "pending" && "bg-gradient-to-r from-blue-500/10 to-primary/10 border-primary/30 shadow-lg shadow-primary/20"
      )}
    >
      <div className="relative">
        {item.status === "success" && (
          <motion.div initial={{ scale: 0, rotate: -180 }} animate={{ scale: 1, rotate: 0 }} transition={{ type: "spring", stiffness: 200 }}>
            <CheckCircle2 className="h-5 w-5 text-green-600 mt-0.5 flex-shrink-0" />
          </motion.div>
        )}
        {item.status === "error" && (
          <motion.div initial={{ scale: 0 }} animate={{ scale: [1, 1.2, 1] }} transition={{ duration: 0.3 }}>
            <AlertCircle className="h-5 w-5 text-red-600 mt-0.5 flex-shrink-0" />
          </motion.div>
        )}
        {item.status === "pending" && <Loader2 className="h-5 w-5 animate-spin text-primary mt-0.5 flex-shrink-0" />}
        {item.status === "success" && <div className="absolute inset-0 bg-green-500 rounded-full blur-md opacity-50 animate-pulse" />}
      </div>
      <motion.span className="text-sm font-medium" initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: 0.2 }}>
        {item.message}
      </motion.span>
    </motion.div>
  );
};

// Minimal Progress Component
const MinimalProgress = ({ progress }: { progress: any[] }) => {
  const totalSteps = progress.length;
  const completedSteps = progress.filter((p) => p.status === "success").length;
  const currentStep = progress[progress.length - 1];
  const progressPercent = totalSteps > 0 ? (completedSteps / totalSteps) * 100 : 0;

  return (
    <div className="max-w-2xl mx-auto space-y-6">
      <div className="space-y-3">
        <div className="flex justify-between items-center">
          <span className="font-semibold text-lg">İlerleme</span>
          <Badge variant="secondary" className="font-mono">
            {completedSteps} / {totalSteps}
          </Badge>
        </div>

        <div className="relative h-4 bg-muted/50 rounded-full overflow-hidden border border-border">
          <div className="absolute inset-0 bg-gradient-to-r from-transparent via-white/5 to-transparent animate-shimmer" />
          <motion.div className="absolute inset-y-0 left-0 bg-gradient-to-r from-green-500 via-blue-500 to-purple-500 rounded-full shadow-lg shadow-primary/50" initial={{ width: 0 }} animate={{ width: `${progressPercent}%` }} transition={{ duration: 0.8, ease: "easeOut" }}>
            <div className="absolute inset-0 bg-gradient-to-r from-transparent via-white/30 to-transparent animate-shimmer" />
            <motion.div className="absolute right-0 inset-y-0 w-1 bg-white/50" animate={{ opacity: [1, 0.5, 1] }} transition={{ duration: 1, repeat: Infinity }} />
          </motion.div>
          <div className="absolute inset-0 flex items-center justify-center">
            <motion.span key={progressPercent} initial={{ scale: 1.2, opacity: 0 }} animate={{ scale: 1, opacity: 1 }} className="text-xs font-bold text-foreground drop-shadow-lg">
              {Math.round(progressPercent)}%
            </motion.span>
          </div>
        </div>
      </div>

      <AnimatePresence mode="wait">
        {currentStep && (
          <motion.div key={currentStep?.message} initial={{ opacity: 0, scale: 0.95, y: 20 }} animate={{ opacity: 1, scale: 1, y: 0 }} exit={{ opacity: 0, scale: 0.95, y: -20 }} transition={{ duration: 0.4, ease: "easeOut" }} className="relative">
            <div className="p-6 rounded-2xl bg-gradient-to-br from-primary/10 via-purple-500/10 to-blue-500/10 border-2 border-primary/30 shadow-xl shadow-primary/20 overflow-hidden">
              <div className="absolute inset-0 bg-gradient-to-r from-primary/5 via-purple-500/5 to-blue-500/5 animate-gradient" />
              <div className="relative flex items-start gap-4">
                <div className="relative flex-shrink-0">
                  {currentStep?.status === "pending" && (
                    <>
                      <motion.div className="absolute inset-0 bg-primary/20 rounded-full" animate={{ scale: [1, 1.5, 1], opacity: [0.5, 0, 0.5] }} transition={{ duration: 2, repeat: Infinity }} />
                      <div className="relative h-12 w-12 rounded-full bg-primary/20 flex items-center justify-center border-2 border-primary">
                        <Loader2 className="h-6 w-6 animate-spin text-primary" />
                      </div>
                    </>
                  )}
                  {currentStep?.status === "success" && (
                    <motion.div initial={{ scale: 0, rotate: -180 }} animate={{ scale: 1, rotate: 0 }} transition={{ type: "spring", stiffness: 200 }} className="h-12 w-12 rounded-full bg-green-500/20 flex items-center justify-center border-2 border-green-500">
                      <CheckCircle2 className="h-6 w-6 text-green-600" />
                    </motion.div>
                  )}
                </div>
                <div className="flex-1 min-w-0">
                  <motion.p className="font-semibold text-lg" initial={{ opacity: 0, x: -10 }} animate={{ opacity: 1, x: 0 }} transition={{ delay: 0.1 }}>
                    {currentStep?.message}
                  </motion.p>
                  <motion.div initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: 0.2 }} className="flex items-center gap-2 mt-2">
                    <div className="flex items-center gap-1 text-xs text-muted-foreground">
                      <span className="font-mono">Adım {completedSteps + 1}</span>
                      <span>/</span>
                      <span className="font-mono">{totalSteps}</span>
                    </div>
                    {currentStep?.status === "pending" && (
                      <motion.div className="flex gap-1" initial={{ opacity: 0 }} animate={{ opacity: 1 }} transition={{ delay: 0.3 }}>
                        {[0, 0.2, 0.4].map((delay, i) => (
                          <motion.div key={i} className="h-1.5 w-1.5 rounded-full bg-primary" animate={{ scale: [1, 1.2, 1], opacity: [1, 0.5, 1] }} transition={{ duration: 1, repeat: Infinity, delay }} />
                        ))}
                      </motion.div>
                    )}
                  </motion.div>
                </div>
              </div>
            </div>
          </motion.div>
        )}
      </AnimatePresence>
    </div>
  );
};

// Success Celebration
const SuccessCelebration = () => {
  useEffect(() => {
    const duration = 3000;
    const end = Date.now() + duration;
    const colors = ["#10b981", "#3b82f6", "#8b5cf6", "#ec4899", "#f59e0b"];

    const frame = () => {
      confetti({
        particleCount: 5,
        angle: 60,
        spread: 55,
        origin: { x: 0, y: 0.6 },
        colors,
      });
      confetti({
        particleCount: 5,
        angle: 120,
        spread: 55,
        origin: { x: 1, y: 0.6 },
        colors,
      });
      if (Date.now() < end) requestAnimationFrame(frame);
    };
    frame();
  }, []);

  return (
    <motion.div initial={{ scale: 0 }} animate={{ scale: 1 }} transition={{ type: "spring", stiffness: 200 }} className="flex flex-col items-center justify-center space-y-6">
      <div className="relative">
        <motion.div className="absolute inset-0 bg-green-500/20 rounded-full blur-2xl" animate={{ scale: [1, 1.5, 1], opacity: [0.5, 0, 0.5] }} transition={{ duration: 2, repeat: Infinity }} />
        <div className="relative h-28 w-28 rounded-full bg-gradient-to-br from-green-400 to-green-600 flex items-center justify-center shadow-2xl shadow-green-500/50">
          <motion.div animate={{ rotate: [0, 360] }} transition={{ duration: 2, ease: "linear", repeat: Infinity }}>
            <Sparkles className="h-14 w-14 text-white" />
          </motion.div>
        </div>
      </div>
      <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.3 }} className="text-center">
        <h3 className="text-3xl font-bold bg-gradient-to-r from-green-400 to-blue-500 bg-clip-text text-transparent">Aktivasyon Tamamlandı!</h3>
        <p className="text-muted-foreground mt-2 text-lg">AI ajanınız başarıyla oluşturuldu ve kullanıma hazır 🎉</p>
      </motion.div>
    </motion.div>
  );
};

// Animated Activation Button
const AnimatedActivationButton = ({ onClick, processing }: any) => {
  return (
    <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
      <Button onClick={onClick} size="lg" className="relative gap-2 min-w-[240px] h-14 text-lg font-semibold overflow-hidden group" disabled={processing}>
        <div className="absolute inset-0 bg-gradient-to-r from-primary via-purple-500 to-primary bg-[length:200%_100%] animate-gradient" />
        <motion.div className="absolute inset-0 bg-gradient-to-r from-transparent via-white/20 to-transparent" animate={{ x: ["-100%", "200%"] }} transition={{ duration: 2, repeat: Infinity, repeatDelay: 1 }} />
        <span className="relative z-10 flex items-center gap-2">
          {processing ? (
            <>
              <Loader2 className="h-5 w-5 animate-spin" />
              İşleniyor...
            </>
          ) : (
            <>
              <Sparkles className="h-5 w-5" />
              Ajanı Aktif Et
              <Zap className="h-5 w-5" />
            </>
          )}
        </span>
      </Button>
    </motion.div>
  );
};

export function Step5Summary() {
  const { state, completeStep } = useWizard();
  const { setMany } = useWizardParams();
  const router = useRouter();

  const [client, setClient] = useState(false);
  const [processing, setProcessing] = useState(false);
  const [done, setDone] = useState(false);
  const [hasError, setHasError] = useState(false);
  const [progress, setProgress] = useState<Array<{ message: string; status: "pending" | "success" | "error" }>>([]);

  const isExecutingRef = useRef(false);

  useEffect(() => {
    setClient(true);
  }, []);

  if (!client) return null;

  const addProgress = (message: string, status: "pending" | "success" | "error" = "pending") => {
    setProgress((p) => [...p, { message, status }]);
  };

  const updateLastProgress = (status: "success" | "error") => {
    setProgress((p) => {
      const newProgress = [...p];
      if (newProgress.length > 0) {
        newProgress[newProgress.length - 1].status = status;
      }
      return newProgress;
    });
  };

  const handleStartActivation = async () => {
    if (isExecutingRef.current) {
      console.warn("⚠️ Activation already in progress, skipping");
      return;
    }

    const llmData = state.data.llm;
    const appPayload = state.data.application;
    const agentType = appPayload?.agentType;

    console.log("═══════════════════════════════════════");
    console.log("🚀 START ACTIVATION");
    console.log("═══════════════════════════════════════");
    console.log("📋 Agent Type:", agentType);
    console.log("📋 Full State:", JSON.stringify(state.data, null, 2));

    if (!appPayload) {
      toast.error("Uygulama bilgileri eksik.");
      return;
    }

    // Determine what's needed based on agent type
    const needsDataIntegration = agentType === "AGENTIC_RAG" || agentType === "MCP_POWERED_AGENTIC_RAG";
    const canUseMCP = agentType === "REACT" || agentType === "MCP_POWERED_AGENTIC_RAG";
    const needsMCP = agentType === "MCP_POWERED_AGENTIC_RAG";

    console.log("🔍 Requirements Check:");
    console.log("  - Needs Data Integration:", needsDataIntegration);
    console.log("  - Can Use MCP:", canUseMCP);
    console.log("  - MCP Required:", needsMCP);

    isExecutingRef.current = true;
    setProcessing(true);
    setProgress([]);
    setHasError(false);

    let applicationId: number;
    let shouldContinue = true;

    try {
      // STEP 1: Create Application
      console.log("\n┌─────────────────────────────────────┐");
      console.log("│ STEP 1: Creating Application        │");
      console.log("└─────────────────────────────────────┘");

      addProgress("Uygulama oluşturuluyor...");
      const appRes = await createApplication(appPayload);

      if (!appRes?.success || !appRes?.data) {
        const errorMsg = appRes?.message || "Uygulama oluşturulamadı.";
        toast.error(`Uygulama Oluşturma: ${errorMsg}`);
        updateLastProgress("error");
        addProgress(`❌ Uygulama oluşturma başarısız: ${errorMsg}`, "error");
        shouldContinue = false;
        setHasError(true);
        // It's acceptable to return here, as nothing else can proceed without an application.
        return;
      }

      applicationId = appRes.data.id as number;
      updateLastProgress("success");
      addProgress(`Uygulama oluşturuldu (ID: ${applicationId})`, "success");

      // STEP 2: Configure LLM
      if (llmData && shouldContinue) {
        addProgress("Model yapılandırılıyor...");
        const llmPayload = { ...llmData, applicationId };
        const llmResult = await createapplicationllms(llmPayload);

        if (!llmResult) {
          const errorMsg = llmResult?.message || "LLM yapılandırması başarısız.";
          toast.error(`Model Yapılandırması: ${errorMsg}`);
          updateLastProgress("error");
          addProgress(`❌ Model yapılandırma başarısız: ${errorMsg}`, "error");
          shouldContinue = false;
          setHasError(true);
        } else {
          updateLastProgress("success");
          addProgress("Model yapılandırıldı", "success");
        }
      }

      // STEP 3: Data Integration (Conditional)
      if (needsDataIntegration && shouldContinue) {
        // File Store
        if (state.data.fileStore?.fileStoreId && shouldContinue) {
          addProgress("Dosya deposu yapılandırılıyor...");
          const fileStoreResult = await createApplicationFileStore({
            applicationId,
            fileStoreId: state.data.fileStore.fileStoreId,
          });

          if (!fileStoreResult?.success) {
            const errorMsg = fileStoreResult?.message || "Dosya deposu yapılandırması başarısız.";
            toast.error(`Dosya Deposu: ${errorMsg}`);
            updateLastProgress("error");
            addProgress(`❌ Dosya deposu başarısız: ${errorMsg}`, "error");
            shouldContinue = false;
            setHasError(true);
          } else {
            updateLastProgress("success");
            addProgress("Dosya deposu yapılandırıldı", "success");
          }
        }

        // Extractor Engine
        if (state.data.extractorEngine?.extractorEngineTypeId && shouldContinue) {
          addProgress("Veri çıkarıcı motoru yapılandırılıyor...");
          const extractorResult = await createExtractorEngine({
            applicationId,
            extractorEngineTypeId: state.data.extractorEngine.extractorEngineTypeId,
          });

          if (!extractorResult) {
            const errorMsg = extractorResult?.message || "Veri çıkarıcı motoru yapılandırması başarısız.";
            toast.error(`Veri Çıkarıcı: ${errorMsg}`);
            updateLastProgress("error");
            addProgress(`❌ Veri çıkarıcı başarısız: ${errorMsg}`, "error");
            shouldContinue = false;
            setHasError(true);
          } else {
            updateLastProgress("success");
            addProgress("Veri çıkarıcı motoru yapılandırıldı", "success");
          }
        }

        // Chunking Strategy
        if (state.data.chunkingStrategy?.chunkingStrategyId && shouldContinue) {
          addProgress("Parçalama stratejisi yapılandırılıyor...");
          const chunkingResult = await createAppChunkingStrategy({
            applicationId,
            chunkingStrategyId: state.data.chunkingStrategy.chunkingStrategyId,
            chunkSize: state.data.chunkingStrategy.chunkSize,
            overlap: state.data.chunkingStrategy.overlap,
            seperator: state.data.chunkingStrategy.seperator,
          });

          if (!chunkingResult?.success) {
            const errorMsg = chunkingResult?.message || "Parçalama stratejisi yapılandırması başarısız.";
            toast.error(`Parçalama Stratejisi: ${errorMsg}`);
            updateLastProgress("error");
            addProgress(`❌ Parçalama stratejisi başarısız: ${errorMsg}`, "error");
            shouldContinue = false;
            setHasError(true);
          } else {
            updateLastProgress("success");
            addProgress("Parçalama stratejisi yapılandırıldı", "success");
          }
        }

        // Search Engine & Embedding
        if (state.data.searchEngine?.searchEngineId && state.data.searchEngine?.embeddingId && state.data.searchEngine?.indexName && shouldContinue) {
          addProgress("Arama motoru yapılandırılıyor...");
          const searchEngineResult = await createAppSearchEngines({
            applicationId,
            searchEngineId: state.data.searchEngine.searchEngineId,
            embeddingId: state.data.searchEngine.embeddingId,
            indexName: state.data.searchEngine.indexName,
            identifier: state.data.application?.identifier || "",
          });

          if (!searchEngineResult) {
            const errorMsg = searchEngineResult?.message || "Arama motoru yapılandırması başarısız.";
            toast.error(`Arama Motoru: ${errorMsg}`);
            updateLastProgress("error");
            addProgress(`❌ Arama motoru başarısız: ${errorMsg}`, "error");
            shouldContinue = false;
            setHasError(true);
          } else {
            const embeddingEngineResult = await createApplicationEmbedding({
              applicationId,
              embeddingId: state.data.searchEngine.embeddingId,
            });

            if (!embeddingEngineResult) {
              const errorMsg = "Embedding motoru yapılandırması başarısız.";
              toast.error(`Embedding Motoru: ${errorMsg}`);
              updateLastProgress("error");
              addProgress(`❌ Embedding motoru başarısız: ${errorMsg}`, "error");
              shouldContinue = false;
              setHasError(true);
            } else {
              updateLastProgress("success");
              addProgress("Arama motoru yapılandırıldı", "success");
            }
          }
        }

        // Create Index
        if (shouldContinue) {
          addProgress("Arama indeksi oluşturuluyor...");
          const indexResult = await createIndex(applicationId);

          if (indexResult.isCreated === false) {
            const errorMsg = indexResult?.message || "Arama indeksi oluşturma başarısız.";
            toast.error(`Arama İndeksi: ${errorMsg}`);
            updateLastProgress("error");
            addProgress(`❌ Arama indeksi başarısız: ${errorMsg}`, "error");
            shouldContinue = false;
            setHasError(true);
          } else {
            updateLastProgress("success");
            addProgress("Arama indeksi oluşturuldu", "success");
          }
        }
      }

      // STEP 4: Configure MCP (Conditional)
      if (canUseMCP && shouldContinue) {
        if (state.data.mcpServer?.mcpServerId) {
          addProgress("MCP sunucusu yapılandırılıyor...");
          const mcpResult = await createApplicationMcp({
            applicationId,
            mcpServerId: state.data.mcpServer.mcpServerId,
          });

          if (!mcpResult) {
            const errorMsg = mcpResult?.message || "MCP sunucusu yapılandırması başarısız.";
            toast.error(`MCP Sunucusu: ${errorMsg}`);
            updateLastProgress("error");
            addProgress(`❌ MCP sunucusu başarısız: ${errorMsg}`, "error");

            if (needsMCP) {
              shouldContinue = false;
              setHasError(true);
            }
          } else {
            updateLastProgress("success");
            addProgress("MCP sunucusu yapılandırıldı", "success");
          }
        } else if (needsMCP) {
          toast.error("MCP sunucusu gerekli ancak yapılandırılmamış");
          addProgress("❌ MCP sunucusu eksik", "error");
          shouldContinue = false;
          setHasError(true);
        }
      }

      // FINALIZATION
      if (shouldContinue) {
        addProgress("Ajan aktif ediliyor...");
        await new Promise((r) => setTimeout(r, 500));
        updateLastProgress("success");
        addProgress("Ajan başarıyla aktif edildi!", "success");

        setDone(true);
        completeStep("summary");

        setMany({
          applicationId,
          appName: appRes.data.name,
          identifier: appRes.data.identifier,
        });

        toast.success("Ajan başarıyla aktif edildi!");
      }
    } catch (err: any) {
      console.error("❌ ERROR:", err);
      const errorMessage = err?.message || "Aktivasyon sırasında beklenmeyen bir hata oluştu.";
      toast.error(`Beklenmeyen Hata: ${errorMessage}`);
      addProgress(`❌ Beklenmeyen hata: ${errorMessage}`, "error");
      setHasError(true);
    } finally {
      isExecutingRef.current = false;
      setProcessing(false);
      localStorage.removeItem("wizardState");
    }
  };

  const handleRetry = () => {
    setHasError(false);
    setProgress([]);
    setDone(false);
    handleStartActivation();
  };

  const handleGoBack = () => {
    router.push("/applications");
  };

  const hasFiles = state.data.application?.hasApplicationFile || state.data.application?.hasUserFile;

  return (
    <div className="relative min-h-screen">
      {/* Animated Background */}
      <div className="fixed inset-0 pointer-events-none overflow-hidden opacity-20">
        <div className="absolute inset-0 bg-gradient-to-br from-primary/10 via-transparent to-purple-500/10" />
        <motion.div
          className="absolute top-1/4 left-1/4 w-96 h-96 bg-primary/30 rounded-full blur-3xl"
          animate={{
            scale: [1, 1.2, 1],
            opacity: [0.3, 0.5, 0.3],
          }}
          transition={{ duration: 8, repeat: Infinity }}
        />
        <motion.div
          className="absolute bottom-1/4 right-1/4 w-96 h-96 bg-purple-500/30 rounded-full blur-3xl"
          animate={{
            scale: [1.2, 1, 1.2],
            opacity: [0.5, 0.3, 0.5],
          }}
          transition={{ duration: 8, repeat: Infinity }}
        />
      </div>

      <div className="relative space-y-6">
        <Card className="border-2 border-primary/20 shadow-2xl shadow-primary/10">
          <CardHeader>
            <motion.div className="flex items-center gap-3" initial={{ opacity: 0, y: -20 }} animate={{ opacity: 1, y: 0 }}>
              <div className="h-12 w-12 rounded-full bg-gradient-to-br from-primary to-primary/50 flex items-center justify-center flex-shrink-0 shadow-lg shadow-primary/50">
                <span className="text-xl font-bold text-white">5</span>
              </div>
              <div>
                <CardTitle className="text-2xl">Yapılandırma Özeti ve Aktivasyon</CardTitle>
                <CardDescription className="text-base">{!done && !hasError ? "Yapılandırmanızı gözden geçirin ve aktivasyonu başlatın" : done ? "Ajanınız başarıyla aktif edildi!" : "Aktivasyon sırasında hata oluştu"}</CardDescription>
              </div>
            </motion.div>
          </CardHeader>

          <CardContent className="space-y-8">
            <AnimatePresence mode="wait">
              {/* Initial State - Summary */}
              {!processing && !done && !hasError && (
                <motion.div key="summary" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="space-y-6">
                  {/* Application Info */}
                  <div className="p-6 border-2 rounded-lg bg-card">
                    <div className="flex items-center gap-3 mb-4">
                      <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                        <Server className="h-5 w-5 text-primary" />
                      </div>
                      <div>
                        <h3 className="text-base font-semibold">Uygulama Bilgileri</h3>
                        <Badge variant="secondary" className="mt-1">
                          Hazır
                        </Badge>
                      </div>
                    </div>
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      <div>
                        <span className="text-xs text-muted-foreground uppercase tracking-wide">Uygulama Adı</span>
                        <p className="text-sm font-medium mt-1">{state.data.application?.name}</p>
                      </div>
                      <div>
                        <span className="text-xs text-muted-foreground uppercase tracking-wide">Tanımlayıcı</span>
                        <p className="text-sm font-medium mt-1 font-mono">{state.data.application?.identifier}</p>
                      </div>
                      <div>
                        <span className="text-xs text-muted-foreground uppercase tracking-wide">Ajan Tipi</span>
                        <p className="text-sm font-medium mt-1">{state.data.application?.agentType ?? "SIMPLE"}</p>
                      </div>
                      <div>
                        <span className="text-xs text-muted-foreground uppercase tracking-wide">Uygulama Tipi</span>
                        <p className="text-sm font-medium mt-1">ID: {state.data.application?.applicationTypeId}</p>
                      </div>
                      <div className="md:col-span-2">
                        <span className="text-xs text-muted-foreground uppercase tracking-wide">Açıklama</span>
                        <p className="text-sm font-medium mt-1">{state.data.application?.description}</p>
                      </div>
                    </div>
                  </div>

                  {/* LLM Settings */}
                  {state.data.llm && (
                    <div className="p-6 border-2 rounded-lg bg-card">
                      <div className="flex items-center gap-3 mb-4">
                        <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                          <Zap className="h-5 w-5 text-primary" />
                        </div>
                        <div>
                          <h3 className="text-base font-semibold">Model Ayarları</h3>
                          <Badge variant="secondary" className="mt-1">
                            Hazır
                          </Badge>
                        </div>
                      </div>
                      <div className="grid grid-cols-2 md:grid-cols-4 gap-4">
                        <div>
                          <span className="text-xs text-muted-foreground uppercase tracking-wide">LLM ID</span>
                          <p className="text-sm font-medium mt-1">{state.data.llm.llmId}</p>
                        </div>
                        <div>
                          <span className="text-xs text-muted-foreground uppercase tracking-wide">Top P</span>
                          <p className="text-sm font-medium mt-1">{state.data.llm.topP}</p>
                        </div>
                        <div>
                          <span className="text-xs text-muted-foreground uppercase tracking-wide">Sıcaklık</span>
                          <p className="text-sm font-medium mt-1">{state.data.llm.temperature}</p>
                        </div>
                        <div>
                          <span className="text-xs text-muted-foreground uppercase tracking-wide">Düşünme</span>
                          <p className="text-sm font-medium mt-1">{state.data.llm.enableThinking ? "Aktif" : "Pasif"}</p>
                        </div>
                      </div>
                    </div>
                  )}

                  {/* Data Integration - Conditional */}
                  {hasFiles && (state.data.application?.agentType === "AGENTIC_RAG" || state.data.application?.agentType === "MCP_POWERED_AGENTIC_RAG") && (
                    <div className="p-6 border-2 rounded-lg bg-card">
                      <div className="flex items-center gap-3 mb-4">
                        <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                          <Database className="h-5 w-5 text-primary" />
                        </div>
                        <div>
                          <h3 className="text-base font-semibold">Veri Entegrasyonu</h3>
                          <Badge variant="secondary" className="mt-1">
                            Hazır
                          </Badge>
                        </div>
                      </div>
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                        {state.data.fileStore && (
                          <div className="p-3 bg-muted/50 rounded-lg flex items-start gap-3">
                            <Database className="h-4 w-4 text-primary mt-0.5" />
                            <div>
                              <span className="text-xs font-medium">Dosya Deposu</span>
                              <p className="text-xs text-muted-foreground mt-1">ID: {state.data.fileStore.fileStoreId}</p>
                            </div>
                          </div>
                        )}
                        {state.data.extractorEngine && (
                          <div className="p-3 bg-muted/50 rounded-lg flex items-start gap-3">
                            <FileText className="h-4 w-4 text-primary mt-0.5" />
                            <div>
                              <span className="text-xs font-medium">Veri Çıkarıcı</span>
                              <p className="text-xs text-muted-foreground mt-1">ID: {state.data.extractorEngine.extractorEngineTypeId}</p>
                            </div>
                          </div>
                        )}
                        {state.data.chunkingStrategy && (
                          <div className="p-3 bg-muted/50 rounded-lg flex items-start gap-3">
                            <Scissors className="h-4 w-4 text-primary mt-0.5" />
                            <div>
                              <span className="text-xs font-medium">Parçalama</span>
                              <p className="text-xs text-muted-foreground mt-1">
                                {state.data.chunkingStrategy.chunkSize} token, {state.data.chunkingStrategy.overlap} örtüşme
                              </p>
                            </div>
                          </div>
                        )}
                        {state.data.searchEngine && (
                          <div className="p-3 bg-muted/50 rounded-lg flex items-start gap-3">
                            <Search className="h-4 w-4 text-primary mt-0.5" />
                            <div>
                              <span className="text-xs font-medium">Arama Motoru</span>
                              <p className="text-xs text-muted-foreground mt-1">{state.data.searchEngine.indexName}</p>
                            </div>
                          </div>
                        )}
                      </div>
                    </div>
                  )}

                  {/* MCP Server - Conditional */}
                  {(state.data.application?.agentType === "REACT" || state.data.application?.agentType === "MCP_POWERED_AGENTIC_RAG") && state.data.mcpServer?.mcpServerId && (
                    <div className="p-6 border-2 rounded-lg bg-card">
                      <div className="flex items-center gap-3 mb-4">
                        <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center">
                          <Server className="h-5 w-5 text-primary" />
                        </div>
                        <div>
                          <h3 className="text-base font-semibold">MCP Sunucusu</h3>
                          <Badge variant="secondary" className="mt-1">
                            Hazır
                          </Badge>
                        </div>
                      </div>
                      <div className="p-3 bg-muted/50 rounded-lg flex items-start gap-3">
                        <Server className="h-4 w-4 text-primary mt-0.5" />
                        <div>
                          <span className="text-xs font-medium">MCP Server ID</span>
                          <p className="text-xs text-muted-foreground mt-1">{state.data.mcpServer.mcpServerId}</p>
                        </div>
                      </div>
                    </div>
                  )}

                  {/* Info Banner */}
                  <div className="p-3 bg-blue-500/10 border border-blue-500/20 rounded-lg text-xs">
                    <p className="font-medium flex items-center gap-1">
                      <Info className="h-3 w-3" />
                      Aktivasyon Hakkında
                    </p>
                    <p className="text-muted-foreground mt-1">Aktivasyonla birlikte tüm yapılandırmalar sırasıyla backend'e gönderilecek ve ajan kullanıma hazır hale gelecektir.</p>
                  </div>

                  {/* Activation Button */}
                  <div className="flex justify-center pt-4">
                    <AnimatedActivationButton onClick={handleStartActivation} processing={processing} />
                  </div>
                </motion.div>
              )}

              {/* Processing State */}
              {processing && (
                <motion.div key="processing" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="py-12 space-y-8">
                  <div className="flex flex-col items-center justify-center gap-6">
                    <AIBrainLoader />
                    <div className="text-center">
                      <h3 className="text-2xl font-bold bg-gradient-to-r from-primary to-purple-500 bg-clip-text text-transparent">AI Ajanı Oluşturuluyor...</h3>
                      <p className="text-muted-foreground mt-2">Lütfen bekleyin, sistem yapılandırılıyor</p>
                    </div>
                  </div>
                  <MinimalProgress progress={progress} />
                </motion.div>
              )}

              {/* Error State */}
              {hasError && !processing && (
                <motion.div key="error" initial={{ opacity: 0, scale: 0.9 }} animate={{ opacity: 1, scale: 1 }} exit={{ opacity: 0, scale: 0.9 }} className="py-12 space-y-8">
                  <div className="flex flex-col items-center justify-center space-y-6">
                    <motion.div initial={{ scale: 0, rotate: -180 }} animate={{ scale: 1, rotate: 0 }} transition={{ type: "spring", stiffness: 200 }} className="h-24 w-24 rounded-full bg-gradient-to-br from-red-500 to-red-600 flex items-center justify-center shadow-2xl shadow-red-500/50">
                      <XCircle className="h-12 w-12 text-white" />
                    </motion.div>
                    <div className="text-center">
                      <h3 className="text-2xl font-bold">Aktivasyon Başarısız</h3>
                      <p className="text-muted-foreground mt-2">Bir veya daha fazla adım başarısız oldu</p>
                    </div>
                  </div>

                  <div className="max-w-3xl mx-auto space-y-3">
                    {progress.map((item, i) => (
                      <AnimatedProgressItem key={i} item={item} index={i} />
                    ))}
                  </div>

                  <div className="flex gap-4 justify-center pt-6">
                    <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                      <Button onClick={handleRetry} className="gap-2">
                        <RefreshCcw className="h-4 w-4" />
                        Tekrar Dene
                      </Button>
                    </motion.div>
                    <motion.div whileHover={{ scale: 1.05 }} whileTap={{ scale: 0.95 }}>
                      <Button onClick={handleGoBack} variant="outline">
                        Uygulamalara Dön
                      </Button>
                    </motion.div>
                  </div>
                </motion.div>
              )}

              {/* Success State */}
              {done && (
                <motion.div key="success" initial={{ opacity: 0 }} animate={{ opacity: 1 }} exit={{ opacity: 0 }} className="py-12 space-y-8">
                  <SuccessCelebration />

                  <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ delay: 0.3 }} className="max-w-4xl mx-auto">
                    <div className="grid grid-cols-2 md:grid-cols-4 gap-4 mb-8">
                      <div className="p-4 rounded-xl bg-gradient-to-br from-green-500/10 to-green-600/10 border border-green-500/20">
                        <div className="flex items-center gap-2 mb-2">
                          <CheckCircle2 className="h-4 w-4 text-green-600" />
                          <span className="text-xs font-medium text-green-600">Tamamlandı</span>
                        </div>
                        <p className="text-2xl font-bold">{progress.filter((p) => p.status === "success").length}</p>
                        <p className="text-xs text-muted-foreground mt-1">Başarılı adım</p>
                      </div>

                      <div className="p-4 rounded-xl bg-gradient-to-br from-blue-500/10 to-blue-600/10 border border-blue-500/20">
                        <div className="flex items-center gap-2 mb-2">
                          <Zap className="h-4 w-4 text-blue-600" />
                          <span className="text-xs font-medium text-blue-600">Süre</span>
                        </div>
                        <p className="text-2xl font-bold">~{Math.floor(progress.length * 0.5)}s</p>
                        <p className="text-xs text-muted-foreground mt-1">Toplam süre</p>
                      </div>

                      <div className="p-4 rounded-xl bg-gradient-to-br from-purple-500/10 to-purple-600/10 border border-purple-500/20">
                        <div className="flex items-center gap-2 mb-2">
                          <Brain className="h-4 w-4 text-purple-600" />
                          <span className="text-xs font-medium text-purple-600">Durum</span>
                        </div>
                        <p className="text-2xl font-bold">Aktif</p>
                        <p className="text-xs text-muted-foreground mt-1">Kullanıma hazır</p>
                      </div>

                      <div className="p-4 rounded-xl bg-gradient-to-br from-yellow-500/10 to-yellow-600/10 border border-yellow-500/20">
                        <div className="flex items-center gap-2 mb-2">
                          <Server className="h-4 w-4 text-yellow-600" />
                          <span className="text-xs font-medium text-yellow-600">ID</span>
                        </div>
                        <p className="text-2xl font-bold">{progress.find((p) => p.message.includes("ID:"))?.message.match(/\d+/)?.[0] || "N/A"}</p>
                        <p className="text-xs text-muted-foreground mt-1">Uygulama ID</p>
                      </div>
                    </div>

                    <details className="group bg-card rounded-xl border-2 border-border overflow-hidden">
                      <summary className="cursor-pointer p-4 hover:bg-muted/50 transition-colors flex items-center justify-between">
                        <div className="flex items-center gap-2">
                          <Info className="h-4 w-4 text-muted-foreground" />
                          <span className="font-medium text-sm">Detaylı Aktivasyon Adımları</span>
                          <Badge variant="secondary" className="ml-2">
                            {progress.length} adım
                          </Badge>
                        </div>
                        <ChevronDown className="h-4 w-4 transition-transform group-open:rotate-180" />
                      </summary>
                      <div className="border-t">
                        <div className="p-4 space-y-2 max-h-[300px] overflow-y-auto">
                          {progress.map((item, i) => (
                            <motion.div key={i} initial={{ opacity: 0, x: -10 }} animate={{ opacity: 1, x: 0 }} transition={{ delay: i * 0.05 }} className="flex items-center gap-3 p-2 rounded-lg hover:bg-muted/50 transition-colors">
                              <div className="flex-shrink-0">
                                {item.status === "success" && <CheckCircle2 className="h-4 w-4 text-green-600" />}
                                {item.status === "error" && <AlertCircle className="h-4 w-4 text-red-600" />}
                              </div>
                              <span className="text-sm text-muted-foreground flex-1">{item.message}</span>
                              <span className="text-xs text-muted-foreground/60">#{i + 1}</span>
                            </motion.div>
                          ))}
                        </div>
                      </div>
                    </details>

                    <div className="mt-8 p-6 rounded-xl bg-gradient-to-br from-primary/5 via-purple-500/5 to-blue-500/5 border border-primary/10">
                      <h4 className="font-semibold mb-3 flex items-center gap-2">
                        <Sparkles className="h-4 w-4 text-primary" />
                        Sonraki Adımlar
                      </h4>
                      <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
                        <button onClick={() => router.push(`/applications/${state.application?.id}`)} className="p-3 rounded-lg bg-card border border-border hover:border-primary/30 hover:bg-primary/5 transition-all text-left group">
                          <div className="flex items-center gap-2 mb-1">
                            <Server className="h-4 w-4 text-primary" />
                            <span className="text-sm font-medium">Uygulamayı Görüntüle</span>
                          </div>
                          <p className="text-xs text-muted-foreground">Detayları inceleyin</p>
                        </button>

                        <button onClick={() => router.push(`/chat/${state.application?.identifier}`)} className="p-3 rounded-lg bg-card border border-border hover:border-green-500/30 hover:bg-green-500/5 transition-all text-left group">
                          <div className="flex items-center gap-2 mb-1">
                            <Zap className="h-4 w-4 text-green-600" />
                            <span className="text-sm font-medium">Ajanı Test Et</span>
                          </div>
                          <p className="text-xs text-muted-foreground">Hemen deneyin</p>
                        </button>

                        <button onClick={() => router.push("/applications")} className="p-3 rounded-lg bg-card border border-border hover:border-blue-500/30 hover:bg-blue-500/5 transition-all text-left group">
                          <div className="flex items-center gap-2 mb-1">
                            <Database className="h-4 w-4 text-blue-600" />
                            <span className="text-sm font-medium">Tüm Uygulamalar</span>
                          </div>
                          <p className="text-xs text-muted-foreground">Listeye dönün</p>
                        </button>
                      </div>
                    </div>
                  </motion.div>
                </motion.div>
              )}
            </AnimatePresence>
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
