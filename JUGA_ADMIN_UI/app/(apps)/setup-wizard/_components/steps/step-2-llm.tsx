"use client";
import { useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { toast } from "sonner";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Input } from "@/components/ui/input";
import { Checkbox } from "@/components/ui/checkbox";
import { Select, SelectTrigger, SelectContent, SelectItem, SelectValue } from "@/components/ui/select";
import { AlertCircle, Info } from "lucide-react";
import { cn } from "@/lib/utils";
import { LlmPayload } from "../../_types/types";
import { useWizard } from "../../wizard-context";
import { WizardNavigation } from "../wizard-navigation";

interface FormValues {
  llmId: string;
  topP: number;
  temperature: number;
  enableThinking: boolean;
}

export function Step2LLM({ existingLlms }: { existingLlms: any[] }) {
  const { state, updateData, completeStep, goToNextStep } = useWizard();

  const {
    control,
    handleSubmit,
    watch,
    setValue,
    formState: { errors, touchedFields },
  } = useForm<FormValues>({
    defaultValues: {
      llmId: state.data.llm?.llmId?.toString() || "",
      topP: state.data.llm?.topP ?? 1.0,
      temperature: state.data.llm?.temperature ?? 0.7,
      enableThinking: state.data.llm?.enableThinking ?? true,
    },
    mode: "onTouched",
  });

  // Set default LLM if not already selected
  useEffect(() => {
    if (!state.data.llm?.llmId && existingLlms?.length > 0) {
      setValue("llmId", existingLlms[0].id.toString());
    }
  }, [existingLlms, setValue, state.data.llm]);

  const onSubmit = (data: FormValues) => {
    const payload: LlmPayload = {
      llmId: parseInt(data.llmId),
      topP: data.topP,
      temperature: data.temperature,
      enableThinking: data.enableThinking,
    };

    console.log("Step 2: Payload to be saved:", payload);
    updateData("llm", payload);
    completeStep("model");
    goToNextStep();
    toast.success("Model ayarları kaydedildi");
  };

  const onError = () => {
    toast.error("Lütfen tüm alanları doğru şekilde doldurun");
  };

  const handleNext = async () => {
    return new Promise<boolean>((resolve) => {
      handleSubmit(
        () => resolve(true),
        () => resolve(false)
      )();
    });
  };

  // Watch values for display
  const watchedTopP = watch("topP");
  const watchedTemperature = watch("temperature");

  return (
    <form onSubmit={handleSubmit(onSubmit, onError)}>
      <div className="space-y-6">
        {/* Main Configuration Card */}
        <Card>
          <CardHeader>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                <span className="text-lg font-bold text-primary">4</span>
              </div>
              <div>
                <CardTitle>Model Yapılandırması</CardTitle>
                <CardDescription>AI modelini ve parametrelerini ayarlayın</CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* LLM Selection */}
            <div className="p-6 border-2 rounded-lg bg-card hover:border-primary/30 transition-colors">
              <div className="space-y-3">
                <div className="flex items-start gap-3">
                  <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
                    <svg className="h-5 w-5 text-primary" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M9.663 17h4.673M12 3v1m6.364 1.636l-.707.707M21 12h-1M4 12H3m3.343-5.657l-.707-.707m2.828 9.9a5 5 0 117.072 0l-.548.547A3.374 3.374 0 0014 18.469V19a2 2 0 11-4 0v-.531c0-.895-.356-1.754-.988-2.386l-.548-.547z"
                      />
                    </svg>
                  </div>
                  <div className="flex-1">
                    <Label htmlFor="llm-provider" className="text-base font-medium">
                      Yapay Zeka Modeli <span className="text-destructive">*</span>
                    </Label>
                    <p className="text-sm text-muted-foreground mt-1">Kullanılacak AI modelini seçin</p>
                  </div>
                </div>
                <Controller
                  name="llmId"
                  control={control}
                  rules={{ required: "Bir LLM sağlayıcısı seçmelisiniz" }}
                  render={({ field }) => (
                    <Select value={field.value} onValueChange={field.onChange}>
                      <SelectTrigger id="llm-provider" className={cn("h-12 text-base", touchedFields.llmId && errors.llmId && "border-destructive")}>
                        <SelectValue placeholder="Bir model seçin" />
                      </SelectTrigger>
                      <SelectContent>
                        {existingLlms?.map((llm: any) => (
                          <SelectItem key={llm.id} value={llm.id.toString()} className="text-base py-3">
                            <div className="flex flex-col">
                              <span className="font-medium">{llm.modelName}</span>
                              {llm.provider && <span className="text-xs text-muted-foreground">{llm.provider}</span>}
                            </div>
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
                {touchedFields.llmId && errors.llmId && (
                  <p className="text-xs text-destructive flex items-center gap-1">
                    <AlertCircle className="h-3 w-3" />
                    {errors.llmId.message}
                  </p>
                )}
              </div>
            </div>

            {/* Model Parameters */}
            <div className="space-y-4">
              <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide">Model Parametreleri</h3>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {/* Top P */}
                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <Label htmlFor="top-p" className="text-base font-medium">
                      Top P <span className="text-destructive">*</span>
                    </Label>
                    <span className="text-sm font-mono text-muted-foreground bg-muted px-2 py-0.5 rounded">{typeof watchedTopP === "number" ? watchedTopP.toFixed(2) : "1.00"}</span>
                  </div>
                  <Controller
                    name="topP"
                    control={control}
                    rules={{
                      required: "Top P zorunludur",
                      min: { value: 0, message: "Minimum 0 olmalıdır" },
                      max: { value: 1, message: "Maximum 1 olmalıdır" },
                      validate: (value) => {
                        const num = Number(value);
                        if (isNaN(num)) return "Geçerli bir sayı giriniz";
                        return true;
                      },
                    }}
                    render={({ field }) => <Input {...field} id="top-p" type="number" step="0.01" min="0" max="1" placeholder="0.0 - 1.0" onChange={(e) => field.onChange(parseFloat(e.target.value))} className={cn(touchedFields.topP && errors.topP && "border-destructive")} />}
                  />
                  {touchedFields.topP && errors.topP && (
                    <p className="text-xs text-destructive flex items-center gap-1">
                      <AlertCircle className="h-3 w-3" />
                      {errors.topP.message}
                    </p>
                  )}
                  <p className="text-xs text-muted-foreground">Çeşitlilik kontrolü (0 = deterministik, 1 = maksimum çeşitlilik)</p>
                </div>

                {/* Temperature */}
                <div className="space-y-2">
                  <div className="flex items-center justify-between">
                    <Label htmlFor="temperature" className="text-base font-medium">
                      Sıcaklık (Temperature) <span className="text-destructive">*</span>
                    </Label>
                    <span className="text-sm font-mono text-muted-foreground bg-muted px-2 py-0.5 rounded">{typeof watchedTemperature === "number" ? watchedTemperature.toFixed(2) : "0.70"}</span>
                  </div>
                  <Controller
                    name="temperature"
                    control={control}
                    rules={{
                      required: "Sıcaklık zorunludur",
                      min: { value: 0, message: "Minimum 0 olmalıdır" },
                      max: { value: 2, message: "Maximum 2 olmalıdır" },
                      validate: (value) => {
                        const num = Number(value);
                        if (isNaN(num)) return "Geçerli bir sayı giriniz";
                        return true;
                      },
                    }}
                    render={({ field }) => <Input {...field} id="temperature" type="number" step="0.01" min="0" max="2" placeholder="0.0 - 2.0" onChange={(e) => field.onChange(parseFloat(e.target.value))} className={cn(touchedFields.temperature && errors.temperature && "border-destructive")} />}
                  />
                  {touchedFields.temperature && errors.temperature && (
                    <p className="text-xs text-destructive flex items-center gap-1">
                      <AlertCircle className="h-3 w-3" />
                      {errors.temperature.message}
                    </p>
                  )}
                  <p className="text-xs text-muted-foreground">Yaratıcılık seviyesi (0 = tutarlı, 2 = yaratıcı)</p>
                </div>
              </div>

              {/* Info Box */}
              <div className="p-3 bg-blue-500/10 border border-blue-500/20 rounded-lg text-xs space-y-1">
                <p className="font-medium flex items-center gap-1">
                  <Info className="h-3 w-3" />
                  Model Parametreleri Hakkında
                </p>
                <ul className="ml-4 space-y-0.5 text-muted-foreground list-disc">
                  <li>
                    <strong>Top P (0-1):</strong> Yanıtların ne kadar çeşitli olacağını kontrol eder
                  </li>
                  <li>
                    <strong>Temperature (0-2):</strong> Yanıtların yaratıcılık seviyesini belirler
                  </li>
                  <li>Daha tutarlı sonuçlar için düşük değerler kullanın (0.3-0.7)</li>
                  <li>Daha yaratıcı sonuçlar için yüksek değerler kullanın (0.8-1.5)</li>
                </ul>
              </div>
            </div>

            {/* Advanced Features */}
            <div className="space-y-3">
              <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide">Gelişmiş Özellikler</h3>
              <div className="flex items-start gap-3 p-4 rounded-lg border-2 hover:border-primary/30 transition-colors">
                <Controller name="enableThinking" control={control} render={({ field }) => <Checkbox id="enable-thinking" checked={field.value} onCheckedChange={field.onChange} className="mt-1" />} />
                <div className="space-y-1">
                  <Label htmlFor="enable-thinking" className="font-medium cursor-pointer text-base">
                    Düşünme Modunu Etkinleştir
                  </Label>
                  <p className="text-sm text-muted-foreground">Modelin düşünce modunu etkinleştirir, cevap vermesi uzun sürer.</p>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <WizardNavigation nextLabel="İlerle: Veri Entegrasyonu" onNext={handleNext} />
      </div>
    </form>
  );
}
