"use client";
import { useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { Card, CardHeader, CardTitle, CardDescription, CardContent } from "@/components/ui/card";
import { Label } from "@/components/ui/label";
import { Info, Server, CheckCircle2, AlertCircle, Radio } from "lucide-react";
import { toast } from "sonner";
import { cn } from "@/lib/utils";
import { useWizard } from "../../wizard-context";
import { WizardNavigation } from "../wizard-navigation";

interface McpServer {
  id: number;
  identifier: string;
  uri: string;
}

interface FormValues {
  mcpServerId: number | null;
}

export function Step4Mcp({ mcpServers }: { mcpServers: McpServer[] }) {
  const { state, updateData, completeStep, goToNextStep } = useWizard();

  const {
    control,
    handleSubmit,
    watch,
    setValue,
    formState: { errors },
  } = useForm<FormValues>({
    defaultValues: {
      mcpServerId: state.data.mcpServer?.mcpServerId || null,
    },
    mode: "onChange",
  });

  const selectedServerId = watch("mcpServerId");

  const onSubmit = (data: FormValues) => {
    updateData("mcpServer", {
      mcpServerId: data.mcpServerId || 0,
    });
    completeStep("mcp");
    goToNextStep();

    if (data.mcpServerId) {
      const server = mcpServers?.find((s) => s.id === data.mcpServerId);
      toast.success(`MCP Sunucusu seçildi: ${server?.identifier || ""}`);
    } else {
      toast.success("MCP yapılandırması atlandı");
    }
  };

  const handleNext = async () => {
    return new Promise<boolean>((resolve) => {
      handleSubmit(
        () => resolve(true),
        () => resolve(false)
      )();
    });
  };

  const handleServerSelect = (serverId: number) => {
    setValue("mcpServerId", serverId);
  };

  const handleClearSelection = () => {
    setValue("mcpServerId", null);
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div className="space-y-6">
        <Card>
          <CardHeader>
            <div className="flex items-center gap-3">
              <div className="h-10 w-10 rounded-full bg-primary/10 flex items-center justify-center flex-shrink-0">
                <span className="text-lg font-bold text-primary">4</span>
              </div>
              <div>
                <CardTitle>MCP Sunucu Yapılandırması</CardTitle>
                <CardDescription>AI ajanınızın kullanacağı Model Context Protocol sunucusunu seçin (opsiyonel)</CardDescription>
              </div>
            </div>
          </CardHeader>
          <CardContent className="space-y-6">
            {/* Info Box */}
            <div className="p-3 bg-blue-500/10 border border-blue-500/20 rounded-lg text-xs space-y-1">
              <p className="font-medium flex items-center gap-1">
                <Info className="h-3 w-3" />
                MCP (Model Context Protocol) Hakkında
              </p>
              <ul className="ml-4 space-y-0.5 text-muted-foreground list-disc">
                <li>MCP sunucusu AI ajanınıza ek yetenekler kazandırır</li>
                <li>Her sunucu farklı araçlar ve işlevler sağlar</li>
                <li>Tek bir sunucu seçebilirsiniz (opsiyonel)</li>
                <li>Sunucu seçmeden de devam edebilirsiniz</li>
              </ul>
            </div>

            {/* Server Selection */}
            <div className="space-y-4">
              <div className="flex items-center justify-between">
                <h3 className="text-sm font-semibold text-muted-foreground uppercase tracking-wide">Mevcut MCP Sunucuları</h3>
                {selectedServerId && (
                  <button type="button" onClick={handleClearSelection} className="text-xs text-muted-foreground hover:text-primary transition-colors">
                    Seçimi Temizle
                  </button>
                )}
              </div>

              {mcpServers && mcpServers.length > 0 ? (
                <Controller
                  name="mcpServerId"
                  control={control}
                  render={({ field }) => (
                    <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                      {mcpServers.map((server) => {
                        const isSelected = selectedServerId === server.id;
                        return (
                          <div key={server.id} className={cn("p-4 border-2 rounded-lg transition-all cursor-pointer group", isSelected ? "bg-primary/5 border-primary shadow-sm" : "bg-card border-border hover:border-primary/30 hover:shadow-sm")} onClick={() => handleServerSelect(server.id)}>
                            <div className="flex items-start gap-3">
                              <div className={cn("h-5 w-5 rounded-full border-2 flex items-center justify-center flex-shrink-0 mt-0.5 transition-all", isSelected ? "border-primary bg-primary" : "border-muted-foreground/30 group-hover:border-primary/50")}>
                                {isSelected && <div className="h-2 w-2 rounded-full bg-white" />}
                              </div>
                              <div className="flex-1 min-w-0">
                                <div className="flex items-start gap-2">
                                  <Server className="h-5 w-5 text-primary flex-shrink-0 mt-0.5" />
                                  <div className="flex-1 min-w-0">
                                    <Label htmlFor={`server-${server.id}`} className="text-base font-medium cursor-pointer">
                                      {server.identifier}
                                    </Label>
                                    <p className="text-xs text-muted-foreground mt-1 break-all">{server.uri}</p>
                                  </div>
                                  {isSelected && <CheckCircle2 className="h-5 w-5 text-primary flex-shrink-0" />}
                                </div>
                              </div>
                            </div>
                          </div>
                        );
                      })}
                    </div>
                  )}
                />
              ) : (
                <div className="p-8 border-2 border-dashed rounded-lg text-center">
                  <AlertCircle className="h-8 w-8 text-muted-foreground mx-auto mb-3" />
                  <p className="text-sm text-muted-foreground">Henüz yapılandırılmış MCP sunucusu bulunmuyor</p>
                </div>
              )}
            </div>

            {/* Selected Server Summary */}
            {selectedServerId && (
              <div className="p-4 border rounded-lg bg-green-500/10 border-green-500/20">
                <h4 className="text-sm font-semibold mb-2 flex items-center gap-2 text-green-700 dark:text-green-400">
                  <CheckCircle2 className="h-4 w-4" />
                  Seçili MCP Sunucusu
                </h4>
                {(() => {
                  const server = mcpServers?.find((s) => s.id === selectedServerId);
                  if (!server) return null;
                  return (
                    <div className="flex items-center gap-2 text-xs p-2 bg-background rounded">
                      <Server className="h-4 w-4 text-primary" />
                      <span className="font-medium">{server.identifier}</span>
                      <span className="text-muted-foreground">•</span>
                      <span className="text-muted-foreground truncate">{server.uri}</span>
                    </div>
                  );
                })()}
              </div>
            )}

            {/* Optional Note */}
            <div className="p-3 bg-orange-500/10 border border-orange-500/20 rounded-lg text-xs">
              <p className="font-medium text-orange-700 dark:text-orange-400 flex items-center gap-1">
                <Info className="h-3 w-3" />
                Opsiyonel Adım
              </p>
              <p className="text-muted-foreground mt-1">MCP sunucusu seçmek zorunlu değildir. Sunucu seçmeden de uygulamanızı oluşturabilirsiniz. İhtiyaç duyduğunuzda daha sonra ekleyebilirsiniz.</p>
            </div>
          </CardContent>
        </Card>

        <WizardNavigation nextLabel="İlerle: Özet" onNext={handleNext} />
      </div>
    </form>
  );
}
