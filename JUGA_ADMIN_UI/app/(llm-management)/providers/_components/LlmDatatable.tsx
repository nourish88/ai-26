"use client";

import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/ui/datatable";
import type { ColumnDef } from "@tanstack/react-table";
import { Bot, Check, X, Pencil, Trash2 } from "lucide-react";
import { useState, useMemo, useRef, useCallback } from "react";

import type { LLMProvider } from "../_types/types";
import { LLM_CONSTANTS } from "../_constants/LlmConstants";
import { useCreateLLM, useDeleteLLM, useLLMs, useUpdateLLM } from "../_hooks/useLLMProviders";
import { EditableCell } from "@/components/datatable/EditableCell";
import { toast } from "sonner";

type LLMTableRow = LLMProvider | (Omit<LLMProvider, "id"> & { id: "new" });

const INITIAL_NEW_LLM_STATE: Omit<LLMProvider, "id"> = {
  name: "",
};

function LlmProviderDatatable() {
  const { data, isLoading, error, isError } = useLLMs();
  const createLLM = useCreateLLM();
  const updateLLM = useUpdateLLM();
  const deleteLLM = useDeleteLLM();
  const [isAddingNew, setIsAddingNew] = useState(false);
  const [editingRowId, setEditingRowId] = useState<string | any>(null);
  const [validationError, setValidationError] = useState<string>("");

  const newLLMDataRef = useRef<Omit<LLMProvider, "id">>(INITIAL_NEW_LLM_STATE);
  const editingLLMDataRef = useRef<LLMProvider | null>(null);

  const handleAddNewClick = useCallback(() => {
    setIsAddingNew(true);
    newLLMDataRef.current = { ...INITIAL_NEW_LLM_STATE };
    setEditingRowId(null);
    editingLLMDataRef.current = null;
  }, []);

  const handleCancelAdd = useCallback(() => {
    setIsAddingNew(false);
  }, []);

  const handleCancelEdit = useCallback(() => {
    setEditingRowId(null);
    editingLLMDataRef.current = null;
  }, []);

  const validateLLMData = useCallback((data: Omit<LLMProvider, "id">): string => {
    if (!data.name.trim()) return "Model ismi zorunludur";
    return "";
  }, []);

  const handleSaveNewLLM = useCallback(() => {
    debugger;
    const error = validateLLMData(newLLMDataRef.current);
    if (error) {
      setValidationError(error);
      return;
    }
    setValidationError("");
    createLLM.mutate(newLLMDataRef.current, {
      onSuccess: (response) => {
        toast.success("LLM başarıyla eklendi");
        setIsAddingNew(false);
        newLLMDataRef.current = { ...INITIAL_NEW_LLM_STATE };
      },
      onError: (error) => {
        if (error?.status === 500) {
          toast.error(`LLM Eklerken Hata Oluştu ${error.status}`);
        }
        toast.error(`LLM eklerken Hata Oluştu ${error.data[0].errorMessage}`);
      },
    });
  }, [createLLM, validateLLMData]);

  const handleSaveEditLLM = useCallback(() => {
    if (!editingLLMDataRef.current) return;
    const error = validateLLMData(editingLLMDataRef.current);
    if (error) {
      setValidationError(error);
      return;
    }
    setValidationError("");
    updateLLM.mutate(editingLLMDataRef.current, {
      onSuccess: () => {
        toast.success("LLM başarıyla güncellendi");
        setEditingRowId(null);
        editingLLMDataRef.current = null;
      },
      onError: (error) => {
        if (error?.status === 500) {
          toast.error(`LLM Güncellerken Hata Oluştu ${error.status}`);
        }
        toast.error(`LLM Güncellerken Hata Oluştu ${error.data[0].errorMessage}`);
        setValidationError("Failed to update LLM. Please try again.");
      },
    });
  }, [updateLLM, validateLLMData]);

  const handleNewLLMChange = useCallback(
    (field: keyof Omit<LLMProvider, "id">, value: string | number) => {
      if (validationError) setValidationError("");

      const isNumberField = ["llmProviderId"].includes(field);
      const parsedValue = isNumberField ? Number.parseInt(value as string, 10) || 0 : value;
      newLLMDataRef.current = { ...newLLMDataRef.current, [field]: parsedValue as any };
    },
    [validationError]
  );

  const handleEditingLLMChange = useCallback(
    (field: keyof LLMProvider, value: string | number) => {
      if (validationError) setValidationError("");
      if (!editingLLMDataRef.current) return;

      const isNumberField = ["llmProviderId"].includes(field);
      const parsedValue = isNumberField ? Number.parseInt(value as string, 10) || 0 : value;
      editingLLMDataRef.current = { ...editingLLMDataRef.current, [field]: parsedValue };
    },
    [validationError]
  );

  const handleEditLlmProvider = useCallback((llm: LLMProvider) => {
    setIsAddingNew(false);
    setEditingRowId(llm.id);
    editingLLMDataRef.current = { ...llm };
  }, []);

  const handleDeleteLlmProvider = useCallback(
    (llmId: number) => {
      deleteLLM.mutate(llmId, {
        onSuccess: () => {
          toast.success("Llm Provider Başarıyla Silindi");
        },
        onError: (error) => {
          if (error?.status === 500) {
            toast.error(`LLM Provider silerken  Hata Oluştu ${error.status}`);
          }
          toast.error(`LLM Provider silerken Hata Oluştu ${error.data[0].errorMessage}`);
        },
      });
    },
    [deleteLLM]
  );

  const columns = useMemo<ColumnDef<LLMTableRow>[]>(
    () => [
      {
        accessorKey: "modelName",
        header: "Model Name",
        cell: ({ row }) => {
          if (row.original.id === "new") {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell placeholder="e.g., gpt-4-turbo" defaultValue={newLLMDataRef.current.name} onChange={(value) => handleNewLLMChange("name", value)} className="max-w-xs" />
              </div>
            );
          }

          const llm = row.original as LLMProvider;

          if (editingRowId === llm.id && editingLLMDataRef.current) {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell placeholder="e.g., gpt-4-turbo" defaultValue={editingLLMDataRef.current.name} onChange={(value) => handleEditingLLMChange("name", value)} className="max-w-xs" />
              </div>
            );
          }

          return (
            <div className="flex items-center">
              <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
                <Bot className="w-5 h-5 text-white" />
              </div>
              <div className="ml-4">
                <div className="text-sm font-medium text-card-foreground">{llm.name}</div>
                <div className="text-sm text-muted-foreground">ID: {llm.id}</div>
              </div>
            </div>
          );
        },
      },
      {
        id: "actions",
        header: () => <div className="text-right">İşlemler</div>,
        cell: ({ row }) => {
          if (row.original.id === "new") {
            return (
              <div className="flex justify-end items-center gap-2">
                <Button variant="ghost" size="icon" onClick={handleSaveNewLLM}>
                  <Check className="w-4 h-4 text-green-600" />
                </Button>
                <Button variant="ghost" size="icon" onClick={handleCancelAdd}>
                  <X className="w-4 h-4 text-red-600" />
                </Button>
              </div>
            );
          }

          const llm = row.original as LLMProvider;

          if (editingRowId === llm.id) {
            return (
              <div className="flex justify-end items-center gap-2">
                <Button variant="ghost" size="icon" onClick={handleSaveEditLLM}>
                  <Check className="w-4 h-4 text-green-600" />
                </Button>
                <Button variant="ghost" size="icon" onClick={handleCancelEdit}>
                  <X className="w-4 h-4 text-red-600" />
                </Button>
              </div>
            );
          }

          return (
            <div className="flex justify-end items-center gap-1">
              <Button variant="outline" size="icon" className="text-muted-foreground hover:text-foreground hover:bg-accent bg-transparent" onClick={() => handleEditLlmProvider(llm)}>
                <Pencil className="w-4 h-4 text-blue-500" />
                <span className="sr-only">Edit</span>
              </Button>
              <Button variant="outline" size="icon" className="text-muted-foreground hover:text-destructive-foreground bg-transparent" onClick={() => handleDeleteLlmProvider(llm.id as number)}>
                <Trash2 className="w-4 h-4 text-red-600" />
                <span className="sr-only">Delete</span>
              </Button>
            </div>
          );
        },
      },
    ],
    [createLLM.isPending, editingRowId, handleNewLLMChange, handleEditingLLMChange, handleSaveNewLLM, handleCancelAdd, handleSaveEditLLM, handleCancelEdit, handleEditLlmProvider, handleDeleteLlmProvider]
  );

  const originalData = data?.items || data || [];
  const tableData = useMemo(() => {
    if (isAddingNew) {
      const newRow: LLMTableRow = { ...INITIAL_NEW_LLM_STATE, id: "new" };
      return [newRow, ...originalData];
    }
    return originalData;
  }, [isAddingNew, originalData]);

  if (isError) {
    return <div>Error loading LLMs: {error?.message}</div>;
  }

  return (
    <div className="m-4">
      {validationError && (
        <div className="mb-4 p-3 bg-red-50 border border-red-200 rounded-md">
          <p className="text-sm text-red-600">{validationError}</p>
        </div>
      )}

      <DataTable
        isLoading={isLoading}
        data={tableData}
        columns={columns}
        title={LLM_CONSTANTS.title}
        description={LLM_CONSTANTS.description}
        actionButton={
          <Button onClick={handleAddNewClick} disabled={isAddingNew}>
            Yeni LLM Provider Ekle
          </Button>
        }
      />
    </div>
  );
}

export default LlmProviderDatatable;
