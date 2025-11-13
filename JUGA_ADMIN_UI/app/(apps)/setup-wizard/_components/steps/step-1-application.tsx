"use client";
import { useEffect, useMemo } from "react";
import { useForm, Controller } from "react-hook-form";
import { toast } from "sonner";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Checkbox } from "@/components/ui/checkbox";
import { AlertCircle, Info } from "lucide-react";
import { cn } from "@/lib/utils";
import { useWizardParams } from "../../_hooks/use-wizard-params";
import { useWizard } from "../../wizard-context";
import { WizardNavigation } from "../wizard-navigation";
import { AgentType, ApplicationPayload } from "../../_types/types";

type TypeDto = { id: number; identifier: string };

interface FormValues {
  name: string;
  identifier: string;
  description: string;
  systemPrompt: string;
  applicationTypeId: string;
  memoryTypeId: string;
  outputTypeId: string;
  hasApplicationFile: boolean;
  hasUserFile: boolean;
  enableGuardRails: boolean;
  checkHallucination: boolean;
}

// Reusable mapping function to derive AgentType from the identifier string
const getAgentType = (identifier: string | undefined): AgentType => {
  switch (identifier) {
    case "CHATBOT":
      return "CHATBOT";
    case "REACT":
      return "REACT";
    case "AGENTIC_RAG":
      return "AGENTIC_RAG";
    case "MCP_POWERED_AGENTIC_RAG":
      return "MCP_POWERED_AGENTIC_RAG";
    default:
      return "CHATBOT";
  }
};

export function AppStep({ applicationTypes, memoryTypes, outputTypes }: any) {
  const { state, updateData, completeStep, goToNextStep } = useWizard();
  const { setMany } = useWizardParams();

  const {
    control,
    handleSubmit,
    watch,
    setValue, // Get setValue to programmatically update form state
    formState: { errors, touchedFields },
  } = useForm<FormValues>({
    defaultValues: {
      name: state.data.application?.name || "",
      identifier: state.data.application?.identifier || "",
      description: state.data.application?.description || "",
      systemPrompt: state.data.application?.systemPrompt || "",
      applicationTypeId: state.data.application?.applicationTypeId?.toString() || "",
      memoryTypeId: state.data.application?.memoryTypeId?.toString() || "",
      outputTypeId: state.data.application?.outputTypeId?.toString() || "",
      hasApplicationFile: state.data.application?.hasApplicationFile ?? true,
      hasUserFile: state.data.application?.hasUserFile ?? true,
      enableGuardRails: state.data.application?.enableGuardRails ?? true,
      checkHallucination: state.data.application?.checkHallucination ?? true,
    },
    mode: "onTouched",
  });

  // Watch the applicationTypeId field to react to its changes
  const watchedApplicationTypeId = watch("applicationTypeId");

  // Determine if the selected application type requires file handling using useMemo for efficiency
  const showFileOptions = useMemo(() => {
    if (!watchedApplicationTypeId) return false;
    const selectedAppType = applicationTypes.find((type: TypeDto) => type.id === parseInt(watchedApplicationTypeId));
    const agentType = getAgentType(selectedAppType?.identifier);

    // Only show file options for RAG-based agent types
    return agentType === "AGENTIC_RAG" || agentType === "MCP_POWERED_AGENTIC_RAG";
  }, [watchedApplicationTypeId, applicationTypes]);

  // Add a side effect to reset file options to false when they are hidden
  useEffect(() => {
    if (!showFileOptions) {
      setValue("hasApplicationFile", false);
      setValue("hasUserFile", false);
    }
  }, [showFileOptions, setValue]);

  const onSubmit = (data: FormValues) => {
    const selectedAppType = applicationTypes.find((type: TypeDto) => type.id === parseInt(data.applicationTypeId));
    const agentType = getAgentType(selectedAppType?.identifier);

    const payload: ApplicationPayload = {
      name: data.name.trim(),
      identifier: data.identifier.trim(),
      description: data.description.trim(),
      systemPrompt: data.systemPrompt.trim(),
      applicationTypeId: parseInt(data.applicationTypeId),
      memoryTypeId: parseInt(data.memoryTypeId),
      outputTypeId: parseInt(data.outputTypeId),
      hasApplicationFile: data.hasApplicationFile, // Value is now correctly false if options were hidden
      hasUserFile: data.hasUserFile, // Value is now correctly false if options were hidden
      enableGuardRails: data.enableGuardRails,
      checkHallucination: data.checkHallucination,
      agentType: agentType,
    };

    updateData("application", payload);
    setMany({ appName: data.name.trim(), identifier: data.identifier.trim(), step: "model" });
    completeStep("app");
    goToNextStep();
    toast.success("Uygulama bilgileri kaydedildi");
  };

  const onError = () => {
    toast.error("Lütfen tüm zorunlu alanları doğru şekilde doldurun");
  };

  const handleNext = async () => {
    return new Promise<boolean>((resolve) => {
      handleSubmit(
        () => resolve(true),
        () => resolve(false)
      )();
    });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit, onError)}>
      <div className="space-y-6">
        {/* Step 1: Basic Information */}
        <Card>
          <CardHeader>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                <span className="text-lg font-bold text-primary">1</span>
              </div>
              <div>
                <CardTitle>Temel Bilgiler</CardTitle>
                <CardDescription>Uygulamanızın adı, tanımlayıcısı ve açıklaması</CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent className="space-y-4">
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              {/* Application Name */}
              <div className="space-y-2">
                <Label htmlFor="app-name" className="text-base font-medium">
                  Uygulama Adı <span className="text-destructive">*</span>
                </Label>
                <Controller
                  name="name"
                  control={control}
                  rules={{
                    required: "Uygulama adı zorunludur",
                    minLength: { value: 3, message: "En az 3 karakter olmalıdır" },
                    maxLength: { value: 100, message: "En fazla 100 karakter olabilir" },
                    validate: (value) => value.trim().length > 0 || "Boş bırakılamaz",
                  }}
                  render={({ field }) => <Input {...field} id="app-name" placeholder="Örn: Müşteri Destek Asistanı" className={cn(touchedFields.name && errors.name && "border-destructive")} />}
                />
                {touchedFields.name && errors.name && (
                  <p className="text-xs text-destructive flex items-center gap-1">
                    <AlertCircle className="h-3 w-3" />
                    {errors.name.message}
                  </p>
                )}
                <p className="text-xs text-muted-foreground">{watch("name")?.length || 0}/100 karakter</p>
              </div>

              {/* Identifier */}
              <div className="space-y-2">
                <Label htmlFor="app-identifier" className="text-base font-medium">
                  Tanımlayıcı <span className="text-destructive">*</span>
                </Label>
                <Controller
                  name="identifier"
                  control={control}
                  rules={{
                    required: "Tanımlayıcı zorunludur",
                    minLength: { value: 3, message: "En az 3 karakter olmalıdır" },
                    maxLength: { value: 50, message: "En fazla 50 karakter olabilir" },
                    pattern: {
                      value: /^[a-z0-9-_]+$/i,
                      message: "Sadece harf, rakam, tire ve alt çizgi kullanılabilir",
                    },
                  }}
                  render={({ field }) => <Input {...field} id="app-identifier" placeholder="Örn: musteri-destek-asistani" className={cn(touchedFields.identifier && errors.identifier && "border-destructive")} />}
                />
                {touchedFields.identifier && errors.identifier && (
                  <p className="text-xs text-destructive flex items-center gap-1">
                    <AlertCircle className="h-3 w-3" />
                    {errors.identifier.message}
                  </p>
                )}
                <p className="text-xs text-muted-foreground">Benzersiz sistem tanımlayıcısı (URL-safe)</p>
              </div>
            </div>

            {/* Description */}
            <div className="space-y-2">
              <Label htmlFor="app-description" className="text-base font-medium">
                Açıklama <span className="text-destructive">*</span>
              </Label>
              <Controller
                name="description"
                control={control}
                rules={{
                  required: "Açıklama zorunludur",
                  minLength: { value: 10, message: "En az 10 karakter olmalıdır" },
                  maxLength: { value: 9999, message: "En fazla 9999 karakter olmalıdır" },
                  validate: (value) => value.trim().length >= 10 || "En az 10 karakter olmalıdır",
                }}
                render={({ field }) => <Textarea {...field} id="app-description" rows={3} placeholder="Uygulamanızın amacını ve işlevini kısaca açıklayın..." className={cn(touchedFields.description && errors.description && "border-destructive")} />}
              />
              {touchedFields.description && errors.description && (
                <p className="text-xs text-destructive flex items-center gap-1">
                  <AlertCircle className="h-3 w-3" />
                  {errors.description.message}
                </p>
              )}
              <p className="text-xs text-muted-foreground">{watch("description")?.length || 0}/500 karakter</p>
            </div>

            {/* System Prompt */}
            <div className="space-y-2">
              <Label htmlFor="system-prompt" className="text-base font-medium">
                Sistem Talimatı <span className="text-destructive">*</span>
              </Label>
              <Controller
                name="systemPrompt"
                control={control}
                rules={{
                  required: "Sistem talimatı zorunludur",
                  minLength: { value: 20, message: "En az 20 karakter olmalıdır" },
                  maxLength: { value: 2000, message: "En fazla 2000 karakter olabilir" },
                  validate: (value) => value.trim().length >= 20 || "En az 20 karakter olmalıdır",
                }}
                render={({ field }) => <Textarea {...field} id="system-prompt" rows={5} placeholder="AI ajanınızın davranışını, tonunu ve görevlerini tanımlayın..." className={cn(touchedFields.systemPrompt && errors.systemPrompt && "border-destructive")} />}
              />
              {touchedFields.systemPrompt && errors.systemPrompt && (
                <p className="text-xs text-destructive flex items-center gap-1">
                  <AlertCircle className="h-3 w-3" />
                  {errors.systemPrompt.message}
                </p>
              )}
              <p className="text-xs text-muted-foreground">{watch("systemPrompt")?.length || 0}/2000 karakter</p>
              <div className="p-3 bg-blue-500/10 border border-blue-500/20 rounded-lg text-xs space-y-1">
                <p className="font-medium flex items-center gap-1">
                  <Info className="h-3 w-3" />
                  Sistem Talimatı İpuçları:
                </p>
                <ul className="ml-4 space-y-0.5 text-muted-foreground list-disc">
                  <li>AI'nın rolünü açıkça belirtin</li>
                  <li>Yanıt tonu ve tarzını tanımlayın</li>
                  <li>Yapabilecekleri ve yapamayacaklarını belirtin</li>
                  <li>Örnek davranışlar ekleyin</li>
                </ul>
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Step 2: Type Configuration */}
        <Card>
          <CardHeader>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                <span className="text-lg font-bold text-primary">2</span>
              </div>
              <div>
                <CardTitle>Tip Yapılandırması</CardTitle>
                <CardDescription>Uygulamanızın davranışını belirleyen temel tipleri seçin</CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent className="grid grid-cols-1 md:grid-cols-3 gap-4">
            {/* Application Type */}
            <div className="p-4 border-2 rounded-lg bg-card hover:border-primary/30 transition-colors">
              <div className="space-y-3">
                <Label htmlFor="app-type" className="text-base font-medium">
                  Uygulama Tipi <span className="text-destructive">*</span>
                </Label>
                <p className="text-sm text-muted-foreground">Uygulamanızın temel yapısını ve davranışını belirler</p>
                <Controller
                  name="applicationTypeId"
                  control={control}
                  rules={{ required: "Bu alan zorunludur" }}
                  render={({ field }) => (
                    <Select value={field.value} onValueChange={field.onChange}>
                      <SelectTrigger id="app-type" className={cn("h-12 text-base", touchedFields.applicationTypeId && errors.applicationTypeId && "border-destructive")}>
                        <SelectValue placeholder="Bir uygulama tipi seçin" />
                      </SelectTrigger>
                      <SelectContent>
                        {applicationTypes.map((type: any) => (
                          <SelectItem key={type.id} value={type.id.toString()} className="text-base py-3">
                            {type.identifier}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
                {touchedFields.applicationTypeId && errors.applicationTypeId && (
                  <p className="text-xs text-destructive flex items-center gap-1">
                    <AlertCircle className="h-3 w-3" />
                    {errors.applicationTypeId.message}
                  </p>
                )}
              </div>
            </div>

            {/* Memory Type */}
            <div className="p-4 border-2 rounded-lg bg-card hover:border-primary/30 transition-colors">
              <div className="space-y-3">
                <Label htmlFor="memory-type" className="text-base font-medium">
                  Bellek Tipi <span className="text-destructive">*</span>
                </Label>
                <p className="text-sm text-muted-foreground">Konuşma geçmişinin nasıl saklanacağını belirler</p>
                <Controller
                  name="memoryTypeId"
                  control={control}
                  rules={{ required: "Bu alan zorunludur" }}
                  render={({ field }) => (
                    <Select value={field.value} onValueChange={field.onChange}>
                      <SelectTrigger id="memory-type" className={cn("h-12 text-base", touchedFields.memoryTypeId && errors.memoryTypeId && "border-destructive")}>
                        <SelectValue placeholder="Bir bellek tipi seçin" />
                      </SelectTrigger>
                      <SelectContent>
                        {memoryTypes.map((type: any) => (
                          <SelectItem key={type.id} value={type.id.toString()} className="text-base py-3">
                            {type.identifier}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
                {touchedFields.memoryTypeId && errors.memoryTypeId && (
                  <p className="text-xs text-destructive flex items-center gap-1">
                    <AlertCircle className="h-3 w-3" />
                    {errors.memoryTypeId.message}
                  </p>
                )}
              </div>
            </div>

            {/* Output Type */}
            <div className="p-4 border-2 rounded-lg bg-card hover:border-primary/30 transition-colors">
              <div className="space-y-3">
                <Label htmlFor="output-type" className="text-base font-medium">
                  Çıktı Tipi <span className="text-destructive">*</span>
                </Label>
                <p className="text-sm text-muted-foreground">Yanıtların format ve yapısını belirler</p>
                <Controller
                  name="outputTypeId"
                  control={control}
                  rules={{ required: "Bu alan zorunludur" }}
                  render={({ field }) => (
                    <Select value={field.value} onValueChange={field.onChange}>
                      <SelectTrigger id="output-type" className={cn("h-12 text-base", touchedFields.outputTypeId && errors.outputTypeId && "border-destructive")}>
                        <SelectValue placeholder="Bir çıktı tipi seçin" />
                      </SelectTrigger>
                      <SelectContent>
                        {outputTypes.map((type: any) => (
                          <SelectItem key={type.id} value={type.id.toString()} className="text-base py-3">
                            {type.identifier}
                          </SelectItem>
                        ))}
                      </SelectContent>
                    </Select>
                  )}
                />
                {touchedFields.outputTypeId && errors.outputTypeId && (
                  <p className="text-xs text-destructive flex items-center gap-1">
                    <AlertCircle className="h-3 w-3" />
                    {errors.outputTypeId.message}
                  </p>
                )}
              </div>
            </div>
          </CardContent>
        </Card>

        {/* Step 3: File Features (Conditional) */}
        {showFileOptions && (
          <Card>
            <CardHeader>
              <div className="flex items-center gap-3">
                <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                  <span className="text-lg font-bold text-primary">3</span>
                </div>
                <div>
                  <CardTitle>Dosya Özellikleri</CardTitle>
                  <CardDescription>Uygulamanızın dosya işleme yeteneklerini etkinleştirin</CardDescription>
                </div>
              </div>
            </CardHeader>
            <CardContent>
              <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                <div className="flex items-start gap-3 p-4 rounded-lg border-2 hover:border-primary/30 transition-colors">
                  <Controller name="hasApplicationFile" control={control} render={({ field }) => <Checkbox id="app-file" checked={field.value} onCheckedChange={field.onChange} className="mt-1" />} />
                  <div className="space-y-1">
                    <Label htmlFor="app-file" className="font-medium cursor-pointer text-base">
                      Uygulama Dosyası
                    </Label>
                    <p className="text-sm text-muted-foreground">Uygulamanın kendi veri dosyalarına erişebilir</p>
                  </div>
                </div>

                <div className="flex items-start gap-3 p-4 rounded-lg border-2 hover:border-primary/30 transition-colors">
                  <Controller name="hasUserFile" control={control} render={({ field }) => <Checkbox id="user-file" checked={field.value} onCheckedChange={field.onChange} className="mt-1" />} />
                  <div className="space-y-1">
                    <Label htmlFor="user-file" className="font-medium cursor-pointer text-base">
                      Kullanıcı Dosyası
                    </Label>
                    <p className="text-sm text-muted-foreground">Kullanıcıların yüklediği dosyalara erişebilir</p>
                  </div>
                </div>
              </div>
            </CardContent>
          </Card>
        )}

        {/* Step 4: Security Features (Always Visible) */}
        <Card>
          <CardHeader>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                <span className="text-lg font-bold text-primary">{showFileOptions ? 4 : 3}</span>
              </div>
              <div>
                <CardTitle>Güvenlik Özellikleri</CardTitle>
                <CardDescription>Uygulamanızın güvenlik ve doğruluk yeteneklerini etkinleştirin</CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div className="flex items-start gap-3 p-4 rounded-lg border-2 hover:border-primary/30 transition-colors">
                <Controller name="enableGuardRails" control={control} render={({ field }) => <Checkbox id="guard-rails" checked={field.value} onCheckedChange={field.onChange} className="mt-1" />} />
                <div className="space-y-1">
                  <Label htmlFor="guard-rails" className="font-medium cursor-pointer text-base">
                    Güvenlik Korumaları
                  </Label>
                  <p className="text-sm text-muted-foreground">Zararlı içerik ve güvenlik kontrollerini etkinleştir</p>
                </div>
              </div>

              <div className="flex items-start gap-3 p-4 rounded-lg border-2 hover:border-primary/30 transition-colors">
                <Controller name="checkHallucination" control={control} render={({ field }) => <Checkbox id="hallucination-check" checked={field.value} onCheckedChange={field.onChange} className="mt-1" />} />
                <div className="space-y-1">
                  <Label htmlFor="hallucination-check" className="font-medium cursor-pointer text-base">
                    Halüsinasyon Kontrolü
                  </Label>
                  <p className="text-sm text-muted-foreground">Yanıtların doğruluğunu ve tutarlılığını kontrol et</p>
                </div>
              </div>
            </div>
          </CardContent>
        </Card>

        <WizardNavigation nextLabel="İlerle: Model Seçimi" onNext={handleNext} />
      </div>
    </form>
  );
}
