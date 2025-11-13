"use client";
import { useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { toast } from "sonner";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { AlertCircle, Info, Scissors, TrendingUp, Layers } from "lucide-react";
import { cn } from "@/lib/utils";
import { useWizard } from "../../../wizard-context";

interface FormValues {
  chunkingStrategyId: string;
  chunkSize: number;
  overlap: number;
  separator: string;
}

export function ChunkingTab({ fetchChunkingStrategies }: any) {
  const { state, updateData } = useWizard();

  const {
    control,
    watch,
    setValue,
    formState: { errors, touchedFields },
  } = useForm<FormValues>({
    defaultValues: {
      chunkingStrategyId: state.data.chunkingStrategy?.chunkingStrategyId?.toString() || "",
      chunkSize: state.data.chunkingStrategy?.chunkSize || 512,
      overlap: state.data.chunkingStrategy?.overlap || 50,
      separator: state.data.chunkingStrategy?.seperator || " ",
    },
    mode: "onChange",
  });

  const chunkSizeValue = watch("chunkSize");
  const overlapValue = watch("overlap");

  // Set default strategy if not selected
  useEffect(() => {
    if (!state.data.chunkingStrategy?.chunkingStrategyId && fetchChunkingStrategies?.length > 0) {
      const defaultId = fetchChunkingStrategies[0].id.toString();
      setValue("chunkingStrategyId", defaultId);
      updateData("chunkingStrategy", {
        chunkingStrategyId: Number(defaultId),
        chunkSize: 512,
        overlap: 50,
        seperator: " ",
      });
    }
  }, [fetchChunkingStrategies, setValue, state.data.chunkingStrategy, updateData]);

  const handleFieldChange = (field: keyof FormValues, value: any) => {
    setValue(field, value);

    // Get current data with defaults
    const currentData = state.data.chunkingStrategy || {
      chunkingStrategyId: 0,
      chunkSize: 512,
      overlap: 50,
      seperator: " ",
    };

    // Create properly typed payload based on which field changed
    let updatedData;

    if (field === "separator") {
      updatedData = {
        chunkingStrategyId: currentData.chunkingStrategyId || 0,
        chunkSize: currentData.chunkSize || 512,
        overlap: currentData.overlap || 50,
        seperator: value, // Map 'separator' to 'seperator'
      };
    } else if (field === "chunkingStrategyId") {
      updatedData = {
        chunkingStrategyId: Number(value),
        chunkSize: currentData.chunkSize || 512,
        overlap: currentData.overlap || 50,
        seperator: currentData.seperator || " ",
      };
    } else {
      // chunkSize or overlap
      updatedData = {
        chunkingStrategyId: currentData.chunkingStrategyId || 0,
        chunkSize: currentData.chunkSize || 512,
        overlap: currentData.overlap || 50,
        seperator: currentData.seperator || " ",
        [field]: value,
      };
    }

    updateData("chunkingStrategy", updatedData);
  };
  // Calculate quality indicators
  const getChunkSizeQuality = () => {
    if (chunkSizeValue >= 512 && chunkSizeValue <= 1024) return { label: "Optimal", color: "text-green-600" };
    if (chunkSizeValue >= 256 && chunkSizeValue < 512) return { label: "İyi", color: "text-blue-600" };
    if (chunkSizeValue > 1024 && chunkSizeValue <= 2048) return { label: "İyi", color: "text-blue-600" };
    return { label: "Dikkat", color: "text-orange-600" };
  };

  const getOverlapQuality = () => {
    const percentage = (overlapValue / chunkSizeValue) * 100;
    if (percentage >= 10 && percentage <= 20) return { label: "Optimal", color: "text-green-600" };
    if (percentage >= 5 && percentage < 10) return { label: "İyi", color: "text-blue-600" };
    if (percentage > 20 && percentage <= 30) return { label: "İyi", color: "text-blue-600" };
    return { label: "Dikkat", color: "text-orange-600" };
  };

  const chunkQuality = getChunkSizeQuality();
  const overlapQuality = getOverlapQuality();

  return (
    <div className="space-y-6">
      {/* Strategy Selection */}
      <div className="p-6 border-2 rounded-lg bg-card hover:border-primary/30 transition-colors">
        <div className="space-y-3">
          <div className="flex items-start gap-3">
            <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
              <Scissors className="h-5 w-5 text-primary" />
            </div>
            <div className="flex-1">
              <Label htmlFor="chunking-strategy" className="text-base font-medium">
                Parçalama Stratejisi <span className="text-destructive">*</span>
              </Label>
              <p className="text-sm text-muted-foreground mt-1">Metinlerin nasıl parçalanacağını belirler</p>
            </div>
          </div>
          <Controller
            name="chunkingStrategyId"
            control={control}
            rules={{ required: "Bir parçalama stratejisi seçmelisiniz" }}
            render={({ field }) => (
              <Select value={field.value} onValueChange={(v) => handleFieldChange("chunkingStrategyId", v)}>
                <SelectTrigger id="chunking-strategy" className={cn("h-12 text-base", touchedFields.chunkingStrategyId && errors.chunkingStrategyId && "border-destructive")}>
                  <SelectValue placeholder="Bir parçalama stratejisi seçin" />
                </SelectTrigger>
                <SelectContent>
                  {fetchChunkingStrategies?.map((strategy: any) => (
                    <SelectItem key={strategy.id} value={strategy.id.toString()} className="text-base py-3">
                      {strategy.identifier}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
          {touchedFields.chunkingStrategyId && errors.chunkingStrategyId && (
            <p className="text-xs text-destructive flex items-center gap-1">
              <AlertCircle className="h-3 w-3" />
              {errors.chunkingStrategyId.message}
            </p>
          )}
        </div>
      </div>

      {/* Parameters */}
      <div className="space-y-4">
        <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide">Parçalama Parametreleri</h3>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* Chunk Size */}
          <div className="space-y-2">
            <div className="flex items-center justify-between">
              <Label htmlFor="chunk-size" className="text-base font-medium">
                Parça Boyutu <span className="text-destructive">*</span>
              </Label>
              <div className="flex items-center gap-2">
                <span className="text-sm font-mono text-muted-foreground bg-muted px-2 py-0.5 rounded">{chunkSizeValue} token</span>
                <span className={cn("text-xs font-medium px-2 py-0.5 rounded", chunkQuality.color, "bg-current/10")}>{chunkQuality.label}</span>
              </div>
            </div>
            <Controller
              name="chunkSize"
              control={control}
              rules={{
                required: "Parça boyutu zorunludur",
                min: { value: 64, message: "Minimum 64 olmalıdır" },
                max: { value: 8192, message: "Maximum 8192 olabilir" },
              }}
              render={({ field }) => <Input {...field} id="chunk-size" type="number" min={64} max={8192} onChange={(e) => handleFieldChange("chunkSize", Number(e.target.value))} className={cn(touchedFields.chunkSize && errors.chunkSize && "border-destructive")} />}
            />
            {touchedFields.chunkSize && errors.chunkSize ? (
              <p className="text-xs text-destructive flex items-center gap-1">
                <AlertCircle className="h-3 w-3" />
                {errors.chunkSize.message}
              </p>
            ) : (
              <p className="text-xs text-muted-foreground">Önerilen: 512-1024 token</p>
            )}
          </div>

          {/* Overlap */}
          <div className="space-y-2">
            <div className="flex items-center justify-between">
              <Label htmlFor="overlap" className="text-base font-medium">
                Örtüşme <span className="text-destructive">*</span>
              </Label>
              <div className="flex items-center gap-2">
                <span className="text-sm font-mono text-muted-foreground bg-muted px-2 py-0.5 rounded">
                  {overlapValue} token ({((overlapValue / chunkSizeValue) * 100).toFixed(0)}%)
                </span>
                <span className={cn("text-xs font-medium px-2 py-0.5 rounded", overlapQuality.color, "bg-current/10")}>{overlapQuality.label}</span>
              </div>
            </div>
            <Controller
              name="overlap"
              control={control}
              rules={{
                required: "Örtüşme zorunludur",
                min: { value: 0, message: "Negatif olamaz" },
                validate: (value) => {
                  if (value >= chunkSizeValue) return "Parça boyutundan küçük olmalı";
                  if (value > chunkSizeValue * 0.5) return "Parça boyutunun %50'sinden küçük olmalı";
                  return true;
                },
              }}
              render={({ field }) => <Input {...field} id="overlap" type="number" min={0} max={Math.floor(chunkSizeValue * 0.5)} onChange={(e) => handleFieldChange("overlap", Number(e.target.value))} className={cn(touchedFields.overlap && errors.overlap && "border-destructive")} />}
            />
            {touchedFields.overlap && errors.overlap ? (
              <p className="text-xs text-destructive flex items-center gap-1">
                <AlertCircle className="h-3 w-3" />
                {errors.overlap.message}
              </p>
            ) : (
              <p className="text-xs text-muted-foreground">
                Önerilen: {Math.floor(chunkSizeValue * 0.1)}-{Math.floor(chunkSizeValue * 0.2)} (parça boyutunun %10-20'si)
              </p>
            )}
          </div>

          {/* Separator */}
          <div className="space-y-2 md:col-span-2">
            <Label htmlFor="separator" className="text-base font-medium">
              Ayırıcı
            </Label>
            <Controller name="separator" control={control} render={({ field }) => <Input {...field} id="separator" placeholder="Örn: boşluk, virgül, satır sonu" onChange={(e) => handleFieldChange("separator", e.target.value)} />} />
            <p className="text-xs text-muted-foreground">Varsayılan: boşluk karakteri</p>
          </div>
        </div>
      </div>

      {/* Info Box */}
      <div className="p-3 bg-blue-500/10 border border-blue-500/20 rounded-lg text-xs space-y-1">
        <p className="font-medium flex items-center gap-1">
          <Info className="h-3 w-3" />
          Parçalama Stratejisi Hakkında
        </p>
        <ul className="ml-4 space-y-0.5 text-muted-foreground list-disc">
          <li>
            <strong>Parça Boyutu:</strong> Her metnin kaç token'a bölüneceğini belirler
          </li>
          <li>
            <strong>Örtüşme:</strong> Komşu parçalar arasında paylaşılan token sayısı (bağlam korunması için)
          </li>
          <li>
            <strong>Ayırıcı:</strong> Metni bölerken kullanılacak karakter (boşluk, satır sonu vb.)
          </li>
          <li>
            <strong>Token:</strong> Yaklaşık olarak bir kelime veya kelime parçası (~4 karakter)
          </li>
        </ul>
      </div>
    </div>
  );
}
