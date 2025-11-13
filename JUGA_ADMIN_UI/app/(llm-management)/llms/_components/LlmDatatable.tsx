/* eslint-disable @typescript-eslint/no-explicit-any */
"use client";

import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/ui/datatable";
import type { ColumnDef } from "@tanstack/react-table";
import { Bot, Check, X, Pencil, Trash2, Cpu } from "lucide-react";
import { useState, useMemo, useRef, useCallback } from "react";

import type { LLMModel } from "../_types/types";
import { LLM_MODALS_CONSTANTS } from "../_constants/LlmModelsConstants";

import { EditableCell } from "@/components/datatable/EditableCell";
import { toast } from "sonner";
import {
  useCreateLLMModel,
  useDeleteLLMModel,
  useLLMModels,
  useUpdateLLMModel,
} from "../_hooks/useLLMProviders";

type LLMTableRow =
  | LLMModel
  | (Omit<LLMModel, "llmProviderId"> & { llmProviderId: "new" | "update" });

const INITIAL_NEW_LLM_STATE: Omit<LLMModel, "llmProviderId" | "id"> = {
  maxInputTokenSize: 0,
  maxOutputTokenSize: 0,
  url: "",
  modelName: "",
};

function LLMModelDatatable() {
  const { data, isLoading, error, isError } = useLLMModels();
  const createLLM = useCreateLLMModel();
  const updateLLM = useUpdateLLMModel();
  const deleteLLM = useDeleteLLMModel();
  const [isAddingNew, setIsAddingNew] = useState(false);

  const [editingRowId, setEditingRowId] = useState<string | any>(null);
  const [validationError, setValidationError] = useState<string>("");

  const newLLMDataRef = useRef<Omit<LLMModel, "id">>(INITIAL_NEW_LLM_STATE);
  const editingLLMDataRef = useRef<LLMModel | null>(null);

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

  const validateLLMData = useCallback((data: Omit<LLMModel, "id">): string => {
    if (!data.modelName.trim()) return "Model ismi zorunludur";
    return "";
  }, []);

  const handleSaveNewLLM = useCallback(() => {
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
        debugger;
        console.log("createLLM", error);
        if (error?.status === 500) {
          toast.error(`LLM Eklerken Hata Oluştu ${error.status}`);
        }
        toast.error(`LLM Eklerken Hata Oluştu ${error.data[0].errorMessage}`);
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
          toast.error(`LLM güncellerken Hata Oluştu ${error.status}`);
        }
        toast.error(
          `LLM güncellerken Hata Oluştu ${error.data[0].errorMessage}`
        );
        setValidationError("Failed to update LLM. Please try again.");
      },
    });
    setEditingRowId(null);
    editingLLMDataRef.current = null;
  }, [updateLLM, validateLLMData]);

  const handleNewLLMChange = useCallback(
    (field: keyof Omit<LLMModel, "id">, value: string | number) => {
      if (validationError) setValidationError("");

      const isNumberField = [
        "maxInputTokenSize",
        "maxOutputTokenSize",
        "llmProviderId",
      ].includes(field);
      const parsedValue = isNumberField
        ? Number.parseInt(value as string, 10) || 0
        : value;
      newLLMDataRef.current = {
        ...newLLMDataRef.current,
        [field]: parsedValue,
      };
    },
    [validationError]
  );

  const handleEditingLLMChange = useCallback(
    (field: keyof LLMModel, value: string | number) => {
      if (validationError) setValidationError("");
      if (!editingLLMDataRef.current) return;

      const isNumberField = [
        "maxInputTokenSize",
        "maxOutputTokenSize",
        "llmProviderId",
        "id",
      ].includes(field);
      const parsedValue = isNumberField
        ? Number.parseInt(value as string, 10) || 0
        : value;
      editingLLMDataRef.current = {
        ...editingLLMDataRef.current,
        [field]: parsedValue,
      };
    },
    [validationError]
  );

  const handleEditLLMModel = useCallback((llm: LLMModel) => {
    setIsAddingNew(false);
    setEditingRowId(llm.id);
    editingLLMDataRef.current = { ...llm };
  }, []);

  const handleDeleteLLMModel = useCallback(
    (llmId: number) => {
      deleteLLM.mutate(llmId, {
        onSuccess: () => {
          toast.success("Llm Başarıyla Silindi");
        },
        onError: (error) => {
          if (error?.status === 500) {
            toast.error(`LLM Silerken Hata Oluştu ${error.status}`);
          }
          toast.error(`LLM Silerken Hata Oluştu ${error.data[0].errorMessage}`);
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
          const llm = row.original as LLMModel;
          if (row.original.llmProviderId === "new") {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell
                  placeholder="e.g., gpt-4-turbo"
                  defaultValue={newLLMDataRef.current.modelName}
                  onChange={(value) => handleNewLLMChange("modelName", value)}
                  className="max-w-xs"
                />
              </div>
            );
          }

          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell
                  placeholder="e.g., gpt-4-turbo"
                  defaultValue={editingLLMDataRef.current?.modelName || ""}
                  onChange={(value) =>
                    handleEditingLLMChange("modelName", value)
                  }
                  className="max-w-xs"
                />
              </div>
            );
          }

          return (
            <div className="flex items-center">
              <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
                <Bot className="w-5 h-5 text-white" />
              </div>
              <div className="ml-4">
                <div className="text-sm font-medium text-card-foreground">
                  {llm.modelName}
                </div>
                <div className="text-sm text-muted-foreground">
                  ID: {llm.id}
                </div>
              </div>
            </div>
          );
        },
      },
      {
        accessorKey: "llmProviderId",
        header: "LLM Provider ID",
        cell: ({ row }) => {
          if (row.original.llmProviderId === "new") {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell
                  placeholder="Provider ID"
                  defaultValue={String(
                    newLLMDataRef.current.llmProviderId || ""
                  )}
                  onChange={(value) =>
                    handleNewLLMChange("llmProviderId", value)
                  }
                  className="max-w-xs"
                />
              </div>
            );
          }

          const llm = row.original as LLMModel;

          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell
                  placeholder="Provider ID"
                  defaultValue={String(
                    editingLLMDataRef.current?.llmProviderId || ""
                  )}
                  onChange={(value) =>
                    handleEditingLLMChange("llmProviderId", value)
                  }
                  className="max-w-xs"
                />
              </div>
            );
          }

          return (
            <div className="flex items-center">
              <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
                <Bot className="w-5 h-5 text-white" />
              </div>
              <div className="ml-4">
                <div className="text-sm font-medium text-card-foreground">
                  {llm.llmProviderId}
                </div>
              </div>
            </div>
          );
        },
      },
      {
        accessorKey: "maxInputTokenSize",
        header: "Max Token Input Size",
        cell: ({ row }) => {
          const llm = row.original as LLMModel;
          if (row.original.llmProviderId === "new") {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell
                  placeholder="e.g., 4096"
                  defaultValue={String(newLLMDataRef.current.maxInputTokenSize)}
                  onChange={(value) =>
                    handleNewLLMChange("maxInputTokenSize", value)
                  }
                  className="max-w-xs"
                />
              </div>
            );
          }

          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell
                  placeholder="e.g., 4096"
                  defaultValue={String(
                    editingLLMDataRef.current?.maxInputTokenSize || 0
                  )}
                  onChange={(value) =>
                    handleEditingLLMChange("maxInputTokenSize", value)
                  }
                  className="max-w-xs"
                />
              </div>
            );
          }
          return (
            <div className="flex items-center">
              <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
                <Cpu className="w-5 h-5 text-white" />
              </div>
              <div className="ml-4">
                <div className="text-sm font-medium text-card-foreground">
                  {llm.maxInputTokenSize}
                </div>
              </div>
            </div>
          );
        },
      },
      {
        accessorKey: "maxOutputTokenSize",
        header: "Max Token Output Size",
        cell: ({ row }) => {
          const llm = row.original as LLMModel;

          if (row.original.llmProviderId === "new") {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell
                  placeholder="e.g., 2048"
                  defaultValue={String(
                    newLLMDataRef.current.maxOutputTokenSize
                  )}
                  onChange={(value) =>
                    handleNewLLMChange("maxOutputTokenSize", value)
                  }
                  className="max-w-xs"
                />
              </div>
            );
          }
          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell
                  placeholder="e.g., 2048"
                  defaultValue={String(
                    editingLLMDataRef.current?.maxOutputTokenSize || 0
                  )}
                  onChange={(value) =>
                    handleEditingLLMChange("maxOutputTokenSize", value)
                  }
                  className="max-w-xs"
                />
              </div>
            );
          }
          return (
            <div className="flex items-center">
              <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
                <Cpu className="w-5 h-5 text-white" />
              </div>
              <div className="ml-4">
                <div className="text-sm font-medium text-card-foreground">
                  {llm.maxOutputTokenSize}
                </div>
              </div>
            </div>
          );
        },
      },
      {
        accessorKey: "url",
        header: "URL",
        cell: ({ row }) => {
          const llm = row.original as LLMModel;

          if (row.original.llmProviderId === "new") {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell
                  placeholder="https://api.example.com"
                  defaultValue={newLLMDataRef.current.url}
                  onChange={(value) => handleNewLLMChange("url", value)}
                  className="max-w-xs"
                />
              </div>
            );
          }
          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center">
                <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center mr-4 shrink-0">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell
                  placeholder="https://api.example.com"
                  defaultValue={editingLLMDataRef.current?.url || ""}
                  onChange={(value) => handleEditingLLMChange("url", value)}
                  className="max-w-xs"
                />
              </div>
            );
          }
          return (
            <div className="flex items-center">
              <div className="w-10 h-10 bg-primary rounded-full flex items-center justify-center">
                <Cpu className="w-5 h-5 text-white" />
              </div>
              <div className="ml-4">
                <div className="text-sm font-medium text-card-foreground">
                  {llm.url}
                </div>
              </div>
            </div>
          );
        },
      },
      {
        id: "actions",
        header: () => <div className="text-right">İşlemler</div>,
        cell: ({ row }) => {
          if (row.original.llmProviderId === "new") {
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

          const llm = row.original as LLMModel;

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
              <Button
                variant="outline"
                size="icon"
                className="text-muted-foreground hover:text-foreground hover:bg-accent bg-transparent"
                onClick={() => handleEditLLMModel(llm)}
              >
                <Pencil className="w-4 h-4 text-blue-500" />
                <span className="sr-only">Edit</span>
              </Button>
              <Button
                variant="outline"
                size="icon"
                className="text-muted-foreground hover:text-destructive-foreground bg-transparent"
                onClick={() => handleDeleteLLMModel(llm.id as number)}
              >
                <Trash2 className="w-4 h-4 text-red-600" />
                <span className="sr-only">Delete</span>
              </Button>
            </div>
          );
        },
      },
    ],
    [
      createLLM.isPending,
      editingRowId,
      handleNewLLMChange,
      handleEditingLLMChange,
      handleSaveNewLLM,
      handleCancelAdd,
      handleSaveEditLLM,
      handleCancelEdit,
      handleEditLLMModel,
      handleDeleteLLMModel,
    ]
  );

  const originalData = data?.items || data || [];
  const tableData = useMemo(() => {
    if (isAddingNew) {
      const newRow: LLMTableRow = {
        ...INITIAL_NEW_LLM_STATE,
        llmProviderId: "new",
      };
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
        title={LLM_MODALS_CONSTANTS.title}
        description={LLM_MODALS_CONSTANTS.description}
        actionButton={
          <Button onClick={handleAddNewClick} disabled={isAddingNew}>
            Yeni LLM Provider Ekle
          </Button>
        }
      />
    </div>
  );
}

export default LLMModelDatatable;
