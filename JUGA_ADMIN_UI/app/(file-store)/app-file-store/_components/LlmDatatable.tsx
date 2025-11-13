"use client";

import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/ui/datatable";
import type { ColumnDef } from "@tanstack/react-table";
import { Bot, Check, X, Pencil, Trash2, Cpu } from "lucide-react";
import { useState, useMemo, useRef, useCallback } from "react";

import type { FileStore } from "../_types/types";
import { LLM_FIRE_STORE } from "../_constants/LlmModelsConstants";

import { EditableCell } from "@/components/datatable/EditableCell";
import { toast } from "sonner";
import { useCreateLLMEmbedding, useDeleteLLMEmbedding, useLLMEmbedding, useUpdateLLMEmbedding } from "../_hooks/useLLMEmbedding";
type LLMTableRow = FileStore | (Omit<FileStore, "id"> & { id: "new" | "update" });

const INITIAL_NEW_LLM_STATE: FileStore = {
  applicationId: 0,
  fileStoreId: 0,
};

function LLMEmbeddingDatatable() {
  const { data, isLoading, error, isError } = useLLMEmbedding();
  const createLLM = useCreateLLMEmbedding();
  const updateLLM = useUpdateLLMEmbedding();
  const deleteLLM = useDeleteLLMEmbedding();
  const [isAddingNew, setIsAddingNew] = useState(false);

  const [editingRowId, setEditingRowId] = useState<string | any>(null);
  const [validationError, setValidationError] = useState<string>("");

  const newLLMDataRef = useRef<FileStore>(INITIAL_NEW_LLM_STATE);
  const editingLLMDataRef = useRef<FileStore | null>(null);

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

  const validateLLMData = useCallback((data: any): string => {
    return "";
  }, []);

  const handleSaveNewLLM = useCallback(() => {
    const error = validateLLMData(newLLMDataRef.current);
    if (error) {
      setValidationError(error);
      return;
    }
    setValidationError("");
    const rawPayload = { ...newLLMDataRef.current };

    const payload = {
      ...rawPayload,
      applicationId: Number(rawPayload.applicationId) || 0,
      fileStoreId: Number(rawPayload.fileStoreId) || 0,
    };

    createLLM.mutate(payload as any, {
      onSuccess: (response) => {
        console.log("first", response);
        toast.success("LLM başarıyla eklendi");
        setIsAddingNew(false);
        newLLMDataRef.current = { ...INITIAL_NEW_LLM_STATE };
      },
      onError: (error: any) => {
        if (error?.status === 500) {
          toast.error(`LLM Eklerken Hata Oluştu ${error.status}`);
        }
        toast.error(`LLM Eklerken Hata Oluşstu ${error.data[0].errorMessage}`);
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
    const rawPayload = { ...editingLLMDataRef.current };

    const payload = {
      ...rawPayload,
      id: Number(rawPayload.id) || 0,
      applicationId: Number(rawPayload.applicationId) || 0,
      fileStoreId: Number(rawPayload.fileStoreId) || 0,
    };

    setValidationError("");
    updateLLM.mutate(payload as any, {
      onSuccess: () => {
        toast.success("LLM başarıyla güncellendi");
        setEditingRowId(null);
        editingLLMDataRef.current = null;
      },
      onError: (error: any) => {
        if (error?.status === 500) {
          toast.error(`LLM Güncellerken Hata Oluştu ${error.status}`);
        }
        toast.error(`LLM Güncellerken Hata Oluşstu ${error.data[0].errorMessage}`);
      },
    });
    setEditingRowId(null);
    editingLLMDataRef.current = null;
  }, [updateLLM, validateLLMData]);

  const handleNewLLMChange = useCallback(
    (field: keyof FileStore, value: string | number) => {
      if (validationError) setValidationError("");

      const isNumberField = ["maxInputTokenSize", "maxOutputTokenSize", "llmProviderId"].includes(field);
      const parsedValue = isNumberField ? Number.parseInt(value as string, 10) || 0 : value;
      newLLMDataRef.current = { ...newLLMDataRef.current, [field]: parsedValue };
    },
    [validationError]
  );

  const handleEditingLLMChange = useCallback(
    (field: keyof FileStore, value: string | number) => {
      if (validationError) setValidationError("");
      if (!editingLLMDataRef.current) return;

      const isNumberField = ["maxInputTokenSize", "maxOutputTokenSize", "llmProviderId", "id"].includes(field);
      const parsedValue = isNumberField ? Number.parseInt(value as string, 10) || 0 : value;
      editingLLMDataRef.current = { ...editingLLMDataRef.current, [field]: parsedValue };
    },
    [validationError]
  );

  const handleEditLLMModel = useCallback((llm: FileStore) => {
    setIsAddingNew(false);
    setEditingRowId(llm.id);
    editingLLMDataRef.current = { ...llm };
  }, []);

  const handleDeleteLLMModel = useCallback(
    (llmId: number) => {
      deleteLLM.mutate(llmId as any, {
        onSuccess: () => {
          toast.success("Llm Başarıyla Silindi");
        },
        onError: (error: any) => {
          if (error?.status === 500) {
            toast.error(`LLM Silerken Hata Oluştu ${error.status}`);
          }
          toast.error(`LLM Silerken Hata Oluşstu ${error.data[0].errorMessage}`);
        },
      });
    },
    [deleteLLM]
  );

  const columns = useMemo<ColumnDef<LLMTableRow>[]>(
    () => [
      {
        accessorKey: "id",
        header: "id",
        cell: ({ row }) => {
          const llm = row.original as FileStore;
          if (row.original.id === "new") {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                {newLLMDataRef.current.id}
              </div>
            );
          }

          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell placeholder="e.g., gpt-4-turbo" defaultValue={editingLLMDataRef.current?.id || ""} onChange={(value) => handleEditingLLMChange("id", value)} className="max-w-xs" />
              </div>
            );
          }

          return (
            <div className="flex items-center">
              <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
                <Bot className="w-5 h-5 text-white" />
              </div>
              <div className="ml-4">
                <div className="text-sm font-medium text-card-foreground">{llm.id}</div>
              </div>
            </div>
          );
        },
      },

      {
        accessorKey: "applicationId",
        header: "applicationId",
        cell: ({ row }) => {
          const llm = row.original as FileStore;
          if (row.original.id === "new") {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell placeholder="e.g., 4096" defaultValue={String(newLLMDataRef.current.applicationId)} onChange={(value) => handleNewLLMChange("applicationId", value)} className="max-w-xs" />
              </div>
            );
          }

          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell placeholder="e.g., 4096" defaultValue={String(editingLLMDataRef.current?.applicationId || 0)} onChange={(value) => handleEditingLLMChange("applicationId", value)} className="max-w-xs" />
              </div>
            );
          }
          return (
            <div className="flex items-center">
              <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
                <Cpu className="w-5 h-5 text-white" />
              </div>
              <div className="ml-4">
                <div className="text-sm font-medium text-card-foreground">{llm.applicationId}</div>
              </div>
            </div>
          );
        },
      },
      {
        accessorKey: "fileStoreId",
        header: "fileStoreId",
        cell: ({ row }) => {
          const llm = row.original as FileStore;
          if (row.original.id === "new") {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell placeholder="e.g., 4096" defaultValue={String(newLLMDataRef.current.fileStoreId)} onChange={(value) => handleNewLLMChange("fileStoreId", value)} className="max-w-xs" />
              </div>
            );
          }

          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell placeholder="e.g., 4096" defaultValue={String(editingLLMDataRef.current?.fileStoreId || 0)} onChange={(value) => handleEditingLLMChange("fileStoreId", value)} className="max-w-xs" />
              </div>
            );
          }
          return (
            <div className="flex items-center">
              <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
                <Cpu className="w-5 h-5 text-white" />
              </div>
              <div className="ml-4">
                <div className="text-sm font-medium text-card-foreground">{llm.fileStoreId}</div>
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

          const llm = row.original as FileStore;

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
              <Button variant="outline" size="icon" className="text-muted-foreground hover:text-foreground hover:bg-accent bg-transparent" onClick={() => handleEditLLMModel(llm)}>
                <Pencil className="w-4 h-4 text-blue-500" />
                <span className="sr-only">Edit</span>
              </Button>
              <Button variant="outline" size="icon" className="text-muted-foreground hover:text-destructive-foreground bg-transparent" onClick={() => handleDeleteLLMModel(llm.id as number)}>
                <Trash2 className="w-4 h-4 text-red-600" />
                <span className="sr-only">Delete</span>
              </Button>
            </div>
          );
        },
      },
    ],
    [createLLM.isPending, editingRowId, handleNewLLMChange, handleEditingLLMChange, handleSaveNewLLM, handleCancelAdd, handleSaveEditLLM, handleCancelEdit, handleEditLLMModel, handleDeleteLLMModel]
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
        title={LLM_FIRE_STORE.title}
        description={LLM_FIRE_STORE.description}
        actionButton={
          <Button onClick={handleAddNewClick} disabled={isAddingNew}>
            Yeni LLM Provider Ekle
          </Button>
        }
      />
    </div>
  );
}

export default LLMEmbeddingDatatable;
