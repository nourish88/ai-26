"use client";
import { useEffect } from "react";
import { useForm, Controller } from "react-hook-form";
import { toast } from "sonner";
import { Label } from "@/components/ui/label";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { AlertCircle, Info, Database } from "lucide-react";
import { cn } from "@/lib/utils";
import { useWizard } from "../../../wizard-context";
import type { FileStoreDto } from "../../../_types/api-types";

interface FormValues {
  fileStoreId: string;
}

export function FileStoreTab({ fileStores }: any) {
  const { state, updateData } = useWizard();

  const {
    control,
    watch,
    setValue,
    formState: { errors, touchedFields },
  } = useForm<FormValues>({
    defaultValues: {
      fileStoreId: state.data.fileStore?.fileStoreId?.toString() || "",
    },
    mode: "onTouched",
  });

  // Set default file store if not selected
  useEffect(() => {
    if (!state.data.fileStore?.fileStoreId && fileStores?.length > 0) {
      const defaultId = fileStores[0].id.toString();
      setValue("fileStoreId", defaultId);
      updateData("fileStore", { fileStoreId: Number(defaultId) });
    }
  }, [fileStores, setValue, state.data.fileStore, updateData]);

  const handleChange = (value: string) => {
    setValue("fileStoreId", value);
    updateData("fileStore", { fileStoreId: Number(value) });
  };

  return (
    <div className="space-y-4">
      <div className="p-6 border-2 rounded-lg bg-card hover:border-primary/30 transition-colors">
        <div className="space-y-3">
          <div className="flex items-start gap-3">
            <div className="h-10 w-10 rounded-lg bg-primary/10 flex items-center justify-center flex-shrink-0">
              <Database className="h-5 w-5 text-primary" />
            </div>
            <div className="flex-1">
              <Label htmlFor="file-store" className="text-base font-medium">
                Dosya Deposu <span className="text-destructive">*</span>
              </Label>
              <p className="text-sm text-muted-foreground mt-1">Dosyaların saklanacağı depolama sistemini seçin</p>
            </div>
          </div>
          <Controller
            name="fileStoreId"
            control={control}
            rules={{ required: "Bir dosya deposu seçmelisiniz" }}
            render={({ field }) => (
              <Select value={field.value} onValueChange={handleChange}>
                <SelectTrigger id="file-store" className={cn("h-12 text-base", touchedFields.fileStoreId && errors.fileStoreId && "border-destructive")}>
                  <SelectValue placeholder="Bir dosya deposu seçin" />
                </SelectTrigger>
                <SelectContent>
                  {fileStores?.map((store: any) => (
                    <SelectItem key={store.id} value={store.id.toString()} className="text-base py-3">
                      {store.identifier}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            )}
          />
          {touchedFields.fileStoreId && errors.fileStoreId && (
            <p className="text-xs text-destructive flex items-center gap-1">
              <AlertCircle className="h-3 w-3" />
              {errors.fileStoreId.message}
            </p>
          )}
        </div>
      </div>

      {/* Info Box */}
      <div className="p-3 bg-blue-500/10 border border-blue-500/20 rounded-lg text-xs space-y-1">
        <p className="font-medium flex items-center gap-1">
          <Info className="h-3 w-3" />
          Dosya Deposu Hakkında
        </p>
        <ul className="ml-4 space-y-0.5 text-muted-foreground list-disc">
          <li>Yüklenen dosyalar bu depoda saklanır</li>
          <li>Hem uygulama hem de kullanıcı dosyaları için kullanılır</li>
          <li>Farklı uygulamalar aynı depoyu paylaşabilir</li>
        </ul>
      </div>
    </div>
  );
}
