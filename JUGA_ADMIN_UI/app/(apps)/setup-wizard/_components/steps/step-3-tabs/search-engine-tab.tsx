"use client";
import { useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { toast } from "sonner";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { AlertCircle, Info, Search, Zap, Database } from "lucide-react";
import { cn } from "@/lib/utils";
import { useWizard } from "../../../wizard-context";
import type { SearchEngineDto, EmbeddingDto } from "../../../_types/api-types";

interface FormValues {
  searchEngineId: string;
  embeddingId: string;
  indexName: string;
}

export function SearchEngineTab({ searchEngines, embeddings }: any) {
  const { state, updateData } = useWizard();

  const {
    control,
    watch,
    setValue,
    formState: { errors, touchedFields },
  } = useForm<FormValues>({
    defaultValues: {
      searchEngineId: state.data.searchEngine?.searchEngineId?.toString() || "",
      embeddingId: state.data.searchEngine?.embeddingId?.toString() || "",
      indexName: state.data.searchEngine?.indexName || "",
    },
    mode: "onTouched",
  });

  const selectedSearchEngineId = watch("searchEngineId");
  const selectedEmbeddingId = watch("embeddingId");
  const indexNameValue = watch("indexName");

  useEffect(() => {
    let needsUpdate = false;
    const updates: Partial<{
      searchEngineId: number;
      embeddingId: number;
      indexName: string;
      identifier: string;
    }> = {};

    // Set default search engine
    if (!state.data.searchEngine?.searchEngineId && searchEngines?.length > 0) {
      const defaultSearchEngineId = searchEngines[0].id.toString();
      setValue("searchEngineId", defaultSearchEngineId);
      updates.searchEngineId = searchEngines[0].id; // Store as number
      needsUpdate = true;
      console.log("🔧 Setting default searchEngineId:", searchEngines[0].id);
    }

    // Set default embedding
    if (!state.data.searchEngine?.embeddingId && embeddings?.length > 0) {
      const defaultEmbeddingId = embeddings[0].id.toString();
      setValue("embeddingId", defaultEmbeddingId);
      updates.embeddingId = embeddings[0].id; // Store as number
      needsUpdate = true;
      console.log("🔧 Setting default embeddingId:", embeddings[0].id);
    }

    // ✅ UPDATE WIZARD STATE WITH DEFAULTS
    if (needsUpdate) {
      // Explicitly type currentData
      const currentData: {
        searchEngineId?: number;
        embeddingId?: number;
        indexName?: string;
        identifier?: string;
      } = state.data.searchEngine || {};

      const updatedData = {
        searchEngineId: currentData.searchEngineId || 0,
        embeddingId: currentData.embeddingId || 0,
        indexName: currentData.indexName || "",
        identifier: currentData.identifier || "",
        ...updates, // Apply updates last to override defaults
      };

      console.log("🔧 Updating wizard state with defaults:", updatedData);
      updateData("searchEngine", updatedData);
    }
  }, [searchEngines, embeddings, setValue, state.data.searchEngine, updateData]);

  const handleFieldChange = (field: keyof FormValues, value: any) => {
    console.log(`🔧 handleFieldChange - ${field}:`, value);
    setValue(field, value);

    const currentData = state.data.searchEngine || {
      searchEngineId: 0,
      embeddingId: 0,
      indexName: "",
    };

    // ✅ Always convert string IDs to numbers
    const updatedData = {
      searchEngineId: field === "searchEngineId" ? Number(value) : currentData.searchEngineId || 0,
      embeddingId: field === "embeddingId" ? Number(value) : currentData.embeddingId || 0,
      indexName: field === "indexName" ? value : currentData.indexName || "",
    };

    console.log("🔧 Updating wizard state:", updatedData);
    updateData("searchEngine", updatedData);
  };

  // Find selected items for additional info
  const selectedSearchEngine = searchEngines?.find((e: any) => e.id.toString() === selectedSearchEngineId);
  const selectedEmbedding = embeddings?.find((e: any) => e.id.toString() === selectedEmbeddingId);

  // ✅ DEBUG LOG
  useEffect(() => {
    console.log("🔍 SearchEngineTab - Current state:", {
      searchEngineId: state.data.searchEngine?.searchEngineId,
      embeddingId: state.data.searchEngine?.embeddingId,
      indexName: state.data.searchEngine?.indexName,
    });
  }, [state.data.searchEngine]);

  return (
    <div className="space-y-6">
      {/* Search Engine Selection */}
      <div className="p-6 border-2 rounded-lg bg-card hover:border-primary/30 transition-colors">
        <div className="space-y-3">
          <div className="flex items-start gap-3">
            <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
              <Search className="h-5 w-5 text-primary" />
            </div>
            <div className="flex-1">
              <Label htmlFor="search-engine" className="text-base font-medium">
                Arama Motoru <span className="text-destructive">*</span>
              </Label>
              <p className="text-sm text-muted-foreground mt-1">Vektör araması için kullanılacak motoru seçin</p>
            </div>
          </div>
          <Controller
            name="searchEngineId"
            control={control}
            rules={{ required: "Bir arama motoru seçmelisiniz" }}
            render={({ field }) => (
              <Select value={field.value} onValueChange={(v) => handleFieldChange("searchEngineId", v)}>
                <SelectTrigger id="search-engine" className={cn("h-12 text-base", touchedFields.searchEngineId && errors.searchEngineId && "border-destructive")}>
                  <SelectValue placeholder="Bir motor seçin" />
                </SelectTrigger>
                <SelectContent>
                  {searchEngines?.map((e: any) => (
                    <SelectItem key={e.id} value={e.id.toString()} className="text-base py-3">
                      {e.identifier}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
          {touchedFields.searchEngineId && errors.searchEngineId && (
            <p className="text-xs text-destructive flex items-center gap-1">
              <AlertCircle className="h-3 w-3" />
              {errors.searchEngineId.message}
            </p>
          )}
        </div>
      </div>

      {/* Embedding Model & Index Name */}
      <div className="space-y-4">
        <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide">Vektör Yapılandırması</h3>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* Embedding Model */}
          <div className="space-y-2">
            <div className="p-6 border-2 rounded-lg bg-card hover:border-primary/30 transition-colors">
              <div className="space-y-3">
                <div className="flex items-start gap-3">
                  <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                    <Zap className="h-5 w-5 text-primary" />
                  </div>
                  <div className="flex-1">
                    <Label htmlFor="embedding-model" className="text-base font-medium">
                      Vektör Modeli <span className="text-destructive">*</span>
                    </Label>
                    <p className="text-sm text-muted-foreground mt-1">Metinleri vektörlere dönüştürür</p>
                  </div>
                </div>
                <Controller
                  name="embeddingId"
                  control={control}
                  rules={{ required: "Bir vektör modeli seçmelisiniz" }}
                  render={({ field }) => (
                    <Select value={field.value} onValueChange={(v) => handleFieldChange("embeddingId", v)}>
                      <SelectTrigger id="embedding-model" className={cn("h-12 text-base", touchedFields.embeddingId && errors.embeddingId && "border-destructive")}>
                        <SelectValue placeholder="Bir model seçin" />
                      </SelectTrigger>
                      <SelectContent>
                        {embeddings?.map((e: any) => (
                          <SelectItem key={e.id} value={e.id.toString()} className="text-base py-3">
                            <div className="flex flex-col">
                              <span className="font-medium">{e.modelName}</span>
                              {e.dimensions && <span className="text-xs text-muted-foreground">{e.dimensions} boyut</span>}
                            </div>
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
                {touchedFields.embeddingId && errors.embeddingId && (
                  <p className="text-xs text-destructive flex items-center gap-1">
                    <AlertCircle className="h-3 w-3" />
                    {errors.embeddingId.message}
                  </p>
                )}
              </div>
            </div>
          </div>

          {/* Index Name */}
          <div className="space-y-2">
            <div className="p-6 border-2 rounded-lg bg-card hover:border-primary/30 transition-colors">
              <div className="space-y-3">
                <div className="flex items-start gap-3">
                  <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                    <Database className="h-5 w-5 text-primary" />
                  </div>
                  <div className="flex-1">
                    <Label htmlFor="index-name" className="text-base font-medium">
                      İndeks Adı <span className="text-destructive">*</span>
                    </Label>
                    <p className="text-sm text-muted-foreground mt-1">Benzersiz indeks tanımlayıcısı</p>
                  </div>
                </div>
                <Controller
                  name="indexName"
                  control={control}
                  rules={{
                    required: "İndeks adı zorunludur",
                    minLength: { value: 3, message: "En az 3 karakter olmalıdır" },
                    maxLength: { value: 50, message: "En fazla 50 karakter olabilir" },
                    pattern: {
                      value: /^[a-z0-9-_]+$/i,
                      message: "Sadece harf, rakam, tire ve alt çizgi kullanılabilir",
                    },
                  }}
                  render={({ field }) => <Input {...field} id="index-name" placeholder="ör. my-app-index" onChange={(e) => handleFieldChange("indexName", e.target.value)} className={cn(touchedFields.indexName && errors.indexName && "border-destructive")} />}
                />
                {touchedFields.indexName && errors.indexName && (
                  <p className="text-xs text-destructive flex items-center gap-1">
                    <AlertCircle className="h-3 w-3" />
                    {errors.indexName.message}
                  </p>
                )}
                <p className="text-xs text-muted-foreground">{indexNameValue.length}/50 karakter</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Selected Configuration Summary */}
      {selectedSearchEngine && selectedEmbedding && indexNameValue && (
        <div className="p-4 border rounded-lg bg-muted/50">
          <h4 className="text-sm font-semibold mb-3 flex items-center gap-2">
            <Info className="h-4 w-4" />
            Yapılandırma Özeti
          </h4>
          <div className="grid grid-cols-1 md:grid-cols-3 gap-4 text-xs">
            <div>
              <p className="text-muted-foreground mb-1">Arama Motoru</p>
              <p className="font-medium">{selectedSearchEngine.identifier}</p>
            </div>
            <div>
              <p className="text-muted-foreground mb-1">Vektör Modeli</p>
              <p className="font-medium">{selectedEmbedding.modelName}</p>
            </div>
            <div>
              <p className="text-muted-foreground mb-1">İndeks Adı</p>
              <p className="font-medium font-mono">{indexNameValue}</p>
            </div>
          </div>
        </div>
      )}

      {/* How It Works */}
      <div className="space-y-3">
        <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide">Nasıl Çalışır?</h3>
        <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
          <div className="p-4 border rounded-lg bg-card">
            <div className="flex items-start gap-3">
              <div className="h-8 w-8 rounded-full bg-blue-500/10 flex items-center justify-center flex-shrink-0">
                <span className="text-sm font-bold text-blue-600">1</span>
              </div>
              <div>
                <p className="text-sm font-medium">Metinden Vektöre</p>
                <p className="text-xs text-muted-foreground mt-1">Vektör modeli metinleri sayısal vektörlere dönüştürür</p>
              </div>
            </div>
          </div>

          <div className="p-4 border rounded-lg bg-card">
            <div className="flex items-start gap-3">
              <div className="h-8 w-8 rounded-full bg-green-500/10 flex items-center justify-center flex-shrink-0">
                <span className="text-sm font-bold text-green-600">2</span>
              </div>
              <div>
                <p className="text-sm font-medium">Sakla</p>
                <p className="text-xs text-muted-foreground mt-1">Vektörler arama motorunda indekslenir</p>
              </div>
            </div>
          </div>

          <div className="p-4 border rounded-lg bg-card">
            <div className="flex items-start gap-3">
              <div className="h-8 w-8 rounded-full bg-purple-500/10 flex items-center justify-center flex-shrink-0">
                <span className="text-sm font-bold text-purple-600">3</span>
              </div>
              <div>
                <p className="text-sm font-medium">Ara</p>
                <p className="text-xs text-muted-foreground mt-1">Benzer vektörler hızlıca bulunur</p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Info Box */}
      <div className="p-3 bg-blue-500/10 border border-blue-500/20 rounded-lg text-xs space-y-1">
        <p className="font-medium flex items-center gap-1">
          <Info className="h-3 w-3" />
          Vektör Araması Hakkında
        </p>
        <ul className="ml-4 space-y-0.5 text-muted-foreground list-disc">
          <li>
            <strong>Arama Motoru:</strong> Vektörlerin saklanacağı ve aranacağı sistem (Pinecone, Weaviate, vb.)
          </li>
          <li>
            <strong>Vektör Modeli:</strong> Metinleri sayısal vektörlere dönüştüren AI modeli
          </li>
          <li>
            <strong>İndeks Adı:</strong> Vektörlerin saklandığı benzersiz indeks ismi
          </li>
          <li>
            <strong>Benzerlik Araması:</strong> Anlamsal olarak benzer içerikleri bulur
          </li>
        </ul>
      </div>

      {/* Best Practices */}
      <div className="p-3 bg-green-500/10 border border-green-500/20 rounded-lg text-xs">
        <p className="font-medium text-green-700 dark:text-green-400 flex items-center gap-1 mb-2">
          <svg className="h-3 w-3" fill="none" viewBox="0 0 24 24" stroke="currentColor">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" />
          </svg>
          En İyi Uygulamalar
        </p>
        <ul className="ml-4 space-y-0.5 text-muted-foreground list-disc">
          <li>İndeks adını uygulama adınızla ilişkilendirin</li>
          <li>Farklı uygulamalar için farklı indeksler kullanın</li>
          <li>Vektör modelini değiştirirseniz indeksi yeniden oluşturun</li>
        </ul>
      </div>
    </div>
  );
}
