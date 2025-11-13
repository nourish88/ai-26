"use client";
import { useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { toast } from "sonner";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { AlertCircle, Info, FileText, CheckCircle2 } from "lucide-react";
import { cn } from "@/lib/utils";
import { useWizard } from "../../../wizard-context";
import type { ExtractorEngineTypeDto } from "../../../_types/api-types";

interface FormValues {
  extractorEngineTypeId: string;
}

export function ExtractorTab({ extractors }: any) {
  const { state, updateData } = useWizard();

  const {
    control,
    watch,
    setValue,
    formState: { errors, touchedFields },
  } = useForm<FormValues>({
    defaultValues: {
      extractorEngineTypeId: state.data.extractorEngine?.extractorEngineTypeId?.toString() || "",
    },
    mode: "onTouched",
  });

  const selectedExtractorId = watch("extractorEngineTypeId");

  // Set default extractor if not selected
  useEffect(() => {
    if (!state.data.extractorEngine?.extractorEngineTypeId && extractors?.length > 0) {
      const defaultId = extractors[0].id.toString();
      setValue("extractorEngineTypeId", defaultId);
      updateData("extractorEngine", { extractorEngineTypeId: Number(defaultId) });
    }
  }, [extractors, setValue, state.data.extractorEngine, updateData]);

  const handleChange = (value: string) => {
    setValue("extractorEngineTypeId", value);
    updateData("extractorEngine", { extractorEngineTypeId: Number(value) });
  };

  // Find selected extractor for additional info
  const selectedExtractor = extractors?.find((e: any) => e.id.toString() === selectedExtractorId);

  return (
    <div className="space-y-6">
      {/* Main Selection */}
      <div className="p-6 border-2 rounded-lg bg-card hover:border-primary/30 transition-colors">
        <div className="space-y-3">
          <div className="flex items-start gap-3">
            <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
              <FileText className="h-5 w-5 text-primary" />
            </div>
            <div className="flex-1">
              <Label htmlFor="extractor-engine" className="text-base font-medium">
                Veri Çıkarma Motoru <span className="text-destructive">*</span>
              </Label>
              <p className="text-sm text-muted-foreground mt-1">Dosyalardan metin çıkarmak için kullanılacak motoru seçin</p>
            </div>
          </div>
          <Controller
            name="extractorEngineTypeId"
            control={control}
            rules={{ required: "Bir veri çıkarıcı motoru seçmelisiniz" }}
            render={({ field }) => (
              <Select value={field.value} onValueChange={handleChange}>
                <SelectTrigger id="extractor-engine" className={cn("h-12 text-base", touchedFields.extractorEngineTypeId && errors.extractorEngineTypeId && "border-destructive")}>
                  <SelectValue placeholder="Bir motor seçin" />
                </SelectTrigger>
                <SelectContent>
                  {extractors?.map((engine: any) => (
                    <SelectItem key={engine.id} value={engine.id.toString()} className="text-base py-3">
                      <div className="flex items-center gap-2">
                        <span className="font-medium">{engine.identifier}</span>
                      </div>
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
          {touchedFields.extractorEngineTypeId && errors.extractorEngineTypeId && (
            <p className="text-xs text-destructive flex items-center gap-1">
              <AlertCircle className="h-3 w-3" />
              {errors.extractorEngineTypeId.message}
            </p>
          )}
        </div>
      </div>

      {/* Selected Extractor Info */}
      {selectedExtractor && (
        <div className="p-4 border rounded-lg bg-muted/50">
          <div className="flex items-start gap-3">
            <CheckCircle2 className="h-5 w-5 text-primary mt-0.5 flex-shrink-0" />
            <div className="space-y-2">
              <p className="font-medium text-sm">Seçili Motor: {selectedExtractor.identifier}</p>
              <div className="space-y-1 text-xs text-muted-foreground">
                <p>Bu motor ile çıkarılabilecek dosya formatları:</p>
                <div className="flex flex-wrap gap-2 mt-2">
                  {["PDF", "DOCX", "DOC", "TXT", "HTML", "MD"].map((format) => (
                    <span key={format} className="px-2 py-1 bg-primary/10 text-primary rounded text-xs font-medium">
                      {format}
                    </span>
                  ))}
                </div>
              </div>
            </div>
          </div>
        </div>
      )}

      {/* Supported Features */}
      <div className="space-y-3">
        <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide">Desteklenen Özellikler</h3>
        <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
          <div className="flex items-start gap-3 p-3 rounded-lg border bg-card">
            <div className="h-8 w-8 rounded-lg bg-green-500/10 flex items-center justify-center flex-shrink-0">
              <svg className="h-4 w-4 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            </div>
            <div>
              <p className="text-sm font-medium">Metin Çıkarma</p>
              <p className="text-xs text-muted-foreground mt-0.5">Tüm metin içeriğini otomatik olarak çıkarır</p>
            </div>
          </div>

          <div className="flex items-start gap-3 p-3 rounded-lg border bg-card">
            <div className="h-8 w-8 rounded-lg bg-green-500/10 flex items-center justify-center flex-shrink-0">
              <svg className="h-4 w-4 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            </div>
            <div>
              <p className="text-sm font-medium">Metadata Korunması</p>
              <p className="text-xs text-muted-foreground mt-0.5">Yazar, tarih gibi bilgileri saklar</p>
            </div>
          </div>

          <div className="flex items-start gap-3 p-3 rounded-lg border bg-card">
            <div className="h-8 w-8 rounded-lg bg-green-500/10 flex items-center justify-center flex-shrink-0">
              <svg className="h-4 w-4 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            </div>
            <div>
              <p className="text-sm font-medium">Tablo Tanıma</p>
              <p className="text-xs text-muted-foreground mt-0.5">Tablolar yapısal olarak korunur</p>
            </div>
          </div>

          <div className="flex items-start gap-3 p-3 rounded-lg border bg-card">
            <div className="h-8 w-8 rounded-lg bg-green-500/10 flex items-center justify-center flex-shrink-0">
              <svg className="h-4 w-4 text-green-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M5 13l4 4L19 7" />
              </svg>
            </div>
            <div>
              <p className="text-sm font-medium">Çoklu Dil Desteği</p>
              <p className="text-xs text-muted-foreground mt-0.5">100+ dilde metin çıkarma</p>
            </div>
          </div>
        </div>
      </div>

      {/* Info Box */}
      <div className="p-3 bg-blue-500/10 border border-blue-500/20 rounded-lg text-xs space-y-1">
        <p className="font-medium flex items-center gap-1">
          <Info className="h-3 w-3" />
          Veri Çıkarıcı Hakkında
        </p>
        <ul className="ml-4 space-y-0.5 text-muted-foreground list-disc">
          <li>
            <strong>Metin Çıkarma:</strong> PDF, Word, Excel gibi dosyalardan metin içeriğini ayıklar
          </li>
          <li>
            <strong>Format Desteği:</strong> Her motor farklı dosya formatlarını destekler
          </li>
          <li>
            <strong>AI Entegrasyonu:</strong> Çıkarılan metin otomatik olarak AI tarafından işlenir
          </li>
          <li>
            <strong>Aranabilirlik:</strong> Çıkarılan içerik vektör veritabanında saklanır ve aranabilir
          </li>
        </ul>
      </div>

      {/* Performance Note */}
      <div className="p-3 bg-orange-500/10 border border-orange-500/20 rounded-lg text-xs">
        <p className="font-medium text-orange-700 dark:text-orange-400 flex items-center gap-1">
          <AlertCircle className="h-3 w-3" />
          Performans Notu
        </p>
        <p className="text-muted-foreground mt-1">Büyük dosyalar (10MB+) için çıkarma işlemi birkaç saniye sürebilir. OCR gerektiren görüntü tabanlı PDF'ler daha uzun süre alabilir.</p>
      </div>
    </div>
  );
}
