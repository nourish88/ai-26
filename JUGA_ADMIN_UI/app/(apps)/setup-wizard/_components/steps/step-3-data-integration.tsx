"use client";
import { useEffect, useState } from "react";
import { toast } from "sonner";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { Tabs, TabsList, TabsTrigger, TabsContent } from "@/components/ui/tabs";
import { Database, FileText, Scissors, Search, Info, CheckCircle2 } from "lucide-react";
import { cn } from "@/lib/utils";
import { FileStoreTab } from "./step-3-tabs/file-store-tab";
import { SearchEngineTab } from "./step-3-tabs/search-engine-tab";
import { ChunkingTab } from "./step-3-tabs/chunking-tab";
import { ExtractorTab } from "./step-3-tabs/extractor-tab";
import { useWizard } from "../../wizard-context";
import { WizardNavigation } from "../wizard-navigation";

export function Step3DataIntegration({ fetchChunkingStrategies, fileStores, searchEngines, extractors, embeddings }: any) {
  const { state, completeStep, goToNextStep } = useWizard();
  const [isClient, setIsClient] = useState(false);

  useEffect(() => setIsClient(true), []);

  if (!isClient) return null;

  const hasFiles = !!(state.data.application?.hasApplicationFile || state.data.application?.hasUserFile);

  // Check completion status for each tab
  const isFileStoreComplete = !!state.data.fileStore?.fileStoreId;
  const isExtractorComplete = !!state.data.extractorEngine?.extractorEngineTypeId;
  const isChunkingComplete = !!(state.data.chunkingStrategy?.chunkingStrategyId && state.data.chunkingStrategy?.chunkSize && state.data.chunkingStrategy?.overlap);
  const isSearchEngineComplete = !!(state.data.searchEngine?.searchEngineId && state.data.searchEngine?.embeddingId && state.data.searchEngine?.indexName);

  const handleNext = async (): Promise<boolean> => {
    if (!hasFiles) {
      completeStep("data-integration");
      goToNextStep();
      return true;
    }

    completeStep("data-integration");
    goToNextStep();
    toast.success("Veri entegrasyonu tamamlandı!");
    return true;
  };

  return (
    <div className="space-y-6">
      <Card>
        <CardHeader>
          <div className="flex items-center gap-3">
            <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
              <span className="text-lg font-bold text-primary">5</span>
            </div>
            <div>
              <CardTitle>Veri Entegrasyonu</CardTitle>
              <CardDescription>{hasFiles ? "Dosya işleme ve vektör araması yapılandırmasını tamamlayın" : "Bu uygulama dosya işleme gerektirmiyor"}</CardDescription>
            </div>
          </div>
        </CardHeader>
        <CardContent>
          {hasFiles ? (
            <div className="space-y-6">
              {/* Info Box */}
              <div className="p-3 bg-blue-500/10 border border-blue-500/20 rounded-lg text-xs">
                <p className="font-medium flex items-center gap-1">
                  <Info className="h-3 w-3" />
                  RAG (Retrieval-Augmented Generation) Yapılandırması
                </p>
                <p className="text-muted-foreground mt-1">Bu adımda dosyalarınızın nasıl işleneceğini ve AI tarafından nasıl aranacağını yapılandırıyorsunuz.</p>
              </div>

              {/* Tabs */}
              <Tabs defaultValue="file-store" className="w-full">
                <TabsList className="grid w-full grid-cols-4 h-auto">
                  <TabsTrigger value="file-store" className={cn("flex items-center gap-2 py-3 relative", isFileStoreComplete && "data-[state=active]:bg-green-500/20 data-[state=active]:text-green-700 dark:data-[state=active]:text-green-400")}>
                    {isFileStoreComplete ? <CheckCircle2 className="h-4 w-4 text-green-600 dark:text-green-400" /> : <Database className="h-4 w-4" />}
                    <span className="hidden sm:inline">Dosya Deposu</span>
                    <span className="sm:hidden">Depo</span>
                  </TabsTrigger>

                  <TabsTrigger value="extractor" className={cn("flex items-center gap-2 py-3 relative", isExtractorComplete && "data-[state=active]:bg-green-500/20 data-[state=active]:text-green-700 dark:data-[state=active]:text-green-400")}>
                    {isExtractorComplete ? <CheckCircle2 className="h-4 w-4 text-green-600 dark:text-green-400" /> : <FileText className="h-4 w-4" />}
                    <span className="hidden sm:inline">Veri Çıkarıcı</span>
                    <span className="sm:hidden">Çıkarıcı</span>
                  </TabsTrigger>

                  <TabsTrigger value="chunking" className={cn("flex items-center gap-2 py-3 relative", isChunkingComplete && "data-[state=active]:bg-green-500/20 data-[state=active]:text-green-700 dark:data-[state=active]:text-green-400")}>
                    {isChunkingComplete ? <CheckCircle2 className="h-4 w-4 text-green-600 dark:text-green-400" /> : <Scissors className="h-4 w-4" />}
                    <span className="hidden sm:inline">Parçalama</span>
                    <span className="sm:hidden">Parçala</span>
                  </TabsTrigger>

                  <TabsTrigger value="search" className={cn("flex items-center gap-2 py-3 relative", isSearchEngineComplete && "data-[state=active]:bg-green-500/20 data-[state=active]:text-green-700 dark:data-[state=active]:text-green-400")}>
                    {isSearchEngineComplete ? <CheckCircle2 className="h-4 w-4 text-green-600 dark:text-green-400" /> : <Search className="h-4 w-4" />}
                    <span className="hidden sm:inline">Arama Motoru</span>
                    <span className="sm:hidden">Arama</span>
                  </TabsTrigger>
                </TabsList>

                <TabsContent value="file-store" className="mt-6">
                  <FileStoreTab fileStores={fileStores} />
                </TabsContent>

                <TabsContent value="extractor" className="mt-6">
                  <ExtractorTab extractors={extractors} />
                </TabsContent>

                <TabsContent value="chunking" className="mt-6">
                  <ChunkingTab fetchChunkingStrategies={fetchChunkingStrategies} />
                </TabsContent>

                <TabsContent value="search" className="mt-6">
                  <SearchEngineTab searchEngines={searchEngines} embeddings={embeddings} />
                </TabsContent>
              </Tabs>
            </div>
          ) : (
            <div className="p-12 text-center bg-muted/30 rounded-lg border-2 border-dashed">
              <div className="max-w-md mx-auto space-y-4">
                <div className="h-16 w-16 rounded-full bg-muted flex items-center justify-center mx-auto">
                  <Info className="h-8 w-8 text-muted-foreground" />
                </div>
                <div>
                  <p className="text-lg font-medium">Veri Entegrasyonu Gerekmiyor</p>
                  <p className="text-sm text-muted-foreground mt-2">Bu uygulama dosya işleme veya RAG kullanmıyor. Bir sonraki adıma geçebilirsiniz.</p>
                </div>
              </div>
            </div>
          )}
        </CardContent>
      </Card>

      <WizardNavigation nextLabel="İlerle: MCP Yapılandırması" onNext={handleNext} />
    </div>
  );
}
