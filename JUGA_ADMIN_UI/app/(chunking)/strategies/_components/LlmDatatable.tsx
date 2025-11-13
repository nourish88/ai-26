// Your LLMEmbeddingDatatable.tsx file

"use client";

import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/ui/datatable";
import { Switch } from "@/components/ui/switch";
import type { ColumnDef } from "@tanstack/react-table";
import { Bot, Check, X, Pencil, Trash2, FileText, FileImage, Hash } from "lucide-react";
import { useState, useMemo, useRef, useCallback } from "react";
import { toast } from "sonner";

import type { ChunkingStrategies } from "../_types/types";
import { LLM_EXTRACTOR_ENGINE_TYPE } from "../_constants/LlmModelsConstants";
import { EditableCell } from "@/components/datatable/EditableCell"; // Make sure path is correct
import { useCreateLLMEmbedding, useDeleteLLMEmbedding, useLLMEmbedding, useUpdateLLMEmbedding } from "../_hooks/useLLMEmbedding";

type LLMTableRow = ChunkingStrategies | { id: "new" | "update" };
type SwitchStates = { isChunkingSizeRequired: boolean; isOverlapRequired: boolean };

const INITIAL_NEW_LLM_STATE: Omit<ChunkingStrategies, "id"> = {
  identifier: "",
  isChunkingSizeRequired: false,
  isOverlapRequired: false,
};

const INITIAL_SWITCH_STATE: SwitchStates = {
  isChunkingSizeRequired: false,
  isOverlapRequired: false,
};

function LLMEmbeddingDatatable() {
  const { data, isLoading, error, isError } = useLLMEmbedding();
  const createLLM = useCreateLLMEmbedding();
  const updateLLM = useUpdateLLMEmbedding();
  const deleteLLM = useDeleteLLMEmbedding();

  const [isAddingNew, setIsAddingNew] = useState(false);
  const [editingRowId, setEditingRowId] = useState<string | number | null>(null);
  const [validationError, setValidationError] = useState<string>("");

  // Refs for non-reactive input data
  const newLLMDataRef = useRef<any>(INITIAL_NEW_LLM_STATE);
  const editingLLMDataRef = useRef<any>(null);

  // --- CHANGE: State for reactive Switch components ---
  const [newSwitchStates, setNewSwitchStates] = useState<SwitchStates>(INITIAL_SWITCH_STATE);
  const [editingSwitchStates, setEditingSwitchStates] = useState<SwitchStates | null>(null);

  const handleAddNewClick = useCallback(() => {
    setIsAddingNew(true);
    newLLMDataRef.current = { ...INITIAL_NEW_LLM_STATE };
    setNewSwitchStates({ ...INITIAL_SWITCH_STATE }); // Reset switch state
    setEditingRowId(null);
    editingLLMDataRef.current = null;
    setEditingSwitchStates(null);
  }, []);

  const handleCancelAdd = useCallback(() => {
    setIsAddingNew(false);
    setValidationError("");
  }, []);

  const handleCancelEdit = useCallback(() => {
    setEditingRowId(null);
    editingLLMDataRef.current = null;
    setEditingSwitchStates(null);
    setValidationError("");
  }, []);

  const validateLLMData = useCallback((data: Omit<ChunkingStrategies, "id">): string => {
    if (!data.identifier.trim()) return "Model name is required";
    return "";
  }, []);

  const handleSaveNewLLM = useCallback(() => {
    // --- CHANGE: Combine data from ref and state before mutation ---
    const payload = { ...newLLMDataRef.current, ...newSwitchStates };

    const error = validateLLMData(payload);
    if (error) {
      setValidationError(error);
      return;
    }
    setValidationError("");
    createLLM.mutate(payload, {
      onSuccess: () => {
        toast.success("LLM successfully added");
        setIsAddingNew(false);
        newLLMDataRef.current = { ...INITIAL_NEW_LLM_STATE };
      },
      onError: (error: any) => {
        toast.error(`Error adding LLM: ${error.data?.[0]?.errorMessage || error.message || "Unknown error"}`);
      },
    });
  }, [createLLM, validateLLMData, newSwitchStates]); // Depend on switch state

  const handleSaveEditLLM = useCallback(() => {
    if (!editingLLMDataRef.current) return;

    // --- CHANGE: Combine data from ref and state before mutation ---
    const payload = { ...editingLLMDataRef.current, ...editingSwitchStates };

    const error = validateLLMData(payload);
    if (error) {
      setValidationError(error);
      return;
    }
    setValidationError("");
    updateLLM.mutate(payload, {
      onSuccess: () => {
        toast.success("LLM successfully updated");
        setEditingRowId(null);
        editingLLMDataRef.current = null;
        setEditingSwitchStates(null);
      },
      onError: (error: any) => {
        toast.error(`Error updating LLM: ${error.data?.[0]?.errorMessage || error.message || "Unknown error"}`);
      },
    });
  }, [updateLLM, validateLLMData, editingSwitchStates]); // Depend on switch state

  // For non-reactive text inputs (updates ref)
  const handleNewLLMChange = useCallback(
    (field: any, value: string | number) => {
      if (validationError) setValidationError("");
      const isNumberField = ["maxInputTokenSize", "maxOutputTokenSize", "llmProviderId"].includes(field);
      const parsedValue = isNumberField ? Number.parseInt(value as string, 10) || 0 : value;
      newLLMDataRef.current = { ...newLLMDataRef.current, [field]: parsedValue };
    },
    [validationError]
  );

  // For non-reactive text inputs (updates ref)
  const handleEditingLLMChange = useCallback(
    (field: keyof ChunkingStrategies, value: string | number) => {
      if (validationError) setValidationError("");
      if (!editingLLMDataRef.current) return;
      const isNumberField = ["maxInputTokenSize", "maxOutputTokenSize", "llmProviderId", "id"].includes(field);
      const parsedValue = isNumberField ? Number.parseInt(value as string, 10) || 0 : value;
      editingLLMDataRef.current = { ...editingLLMDataRef.current, [field]: parsedValue };
    },
    [validationError]
  );

  // --- CHANGE: Handlers for reactive Switch components (updates state) ---
  const handleNewSwitchChange = useCallback((field: keyof SwitchStates, checked: boolean) => {
    setNewSwitchStates((prev) => ({ ...prev, [field]: checked }));
  }, []);

  const handleEditingSwitchChange = useCallback((field: keyof SwitchStates, checked: boolean) => {
    setEditingSwitchStates((prev) => (prev ? { ...prev, [field]: checked } : null));
  }, []);

  const handleEditLLMModel = useCallback((llm: ChunkingStrategies) => {
    setIsAddingNew(false);
    setEditingRowId(llm.id as any);
    editingLLMDataRef.current = { ...llm };

    setEditingSwitchStates({
      isChunkingSizeRequired: llm.isChunkingSizeRequired,
      isOverlapRequired: llm.isOverlapRequired,
    });
  }, []);

  const handleDeleteLLMModel = useCallback(
    (llmId: number) => {
      deleteLLM.mutate(llmId as any, {
        onSuccess: () => toast.success("LLM successfully deleted"),
        onError: (error: any) => toast.error(`Error deleting LLM: ${error.data?.[0]?.errorMessage || error.message || "Unknown error"}`),
      });
    },
    [deleteLLM]
  );

  const columns = useMemo<ColumnDef<LLMTableRow>[]>(
    () => [
      {
        accessorKey: "identifier",
        header: "Model Name",
        cell: ({ row }) => {
          const llm = row.original as ChunkingStrategies;
          if (row.original.id === "new") {
            return (
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center shadow-sm">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell placeholder="e.g., gpt-4-turbo" defaultValue={newLLMDataRef.current.identifier} onChange={(value) => handleNewLLMChange("identifier", value)} className="max-w-xs font-medium" />
              </div>
            );
          }

          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center gap-3">
                <div className="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center shadow-sm">
                  <Bot className="w-5 h-5 text-white" />
                </div>
                <EditableCell placeholder="e.g., gpt-4-turbo" defaultValue={editingLLMDataRef.current?.identifier || ""} onChange={(value) => handleEditingLLMChange("identifier", value)} className="max-w-xs font-medium" />
              </div>
            );
          }
          return (
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 bg-gradient-to-br from-blue-500 to-purple-600 rounded-lg flex items-center justify-center shadow-sm">
                <Bot className="w-5 h-5 text-white" />
              </div>
              <div>
                <div className="font-medium text-foreground">{llm.identifier}</div>
                <div className="text-sm text-muted-foreground">ID: {llm.id}</div>
              </div>
            </div>
          );
        },
      },

      {
        accessorKey: "isChunkingSizeRequired",
        header: () => (
          <div className="flex items-center justify-center gap-2">
            <FileText className="w-4 h-4 text-blue-600" />
            <span>isChunkingSizeRequired</span>
          </div>
        ),
        cell: ({ row }) => {
          const llm = row.original as ChunkingStrategies;
          if (row.original.id === "new") {
            return (
              <div className="flex items-center justify-center">
                {/* --- CHANGE: Bind Switch to reactive state --- */}
                <Switch checked={newSwitchStates.isChunkingSizeRequired} onCheckedChange={(checked) => handleNewSwitchChange("isChunkingSizeRequired", checked)} />
              </div>
            );
          }
          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center justify-center">
                {/* --- CHANGE: Bind Switch to reactive state --- */}
                <Switch checked={editingSwitchStates?.isChunkingSizeRequired || false} onCheckedChange={(checked) => handleEditingSwitchChange("isChunkingSizeRequired", checked)} />
              </div>
            );
          }
          return (
            <div className="flex items-center justify-center">
              <div className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${llm.isChunkingSizeRequired ? "bg-green-100 text-green-800" : "bg-gray-100 text-gray-800"}`}>{llm.isChunkingSizeRequired ? "Enabled" : "Disabled"}</div>
            </div>
          );
        },
      },
      // --- All other Switch columns (pdf, txt) follow the same pattern ---
      {
        accessorKey: "isOverlapRequired",
        header: () => (
          <div className="flex items-center justify-center gap-2">
            <FileImage className="w-4 h-4 text-red-600" />
            <span>isOverlapRequired</span>
          </div>
        ),
        cell: ({ row }) => {
          const llm = row.original as ChunkingStrategies;
          if (row.original.id === "new") {
            return (
              <div className="flex items-center justify-center">
                <Switch checked={newSwitchStates.isOverlapRequired} onCheckedChange={(checked) => handleNewSwitchChange("isOverlapRequired", checked)} />
              </div>
            );
          }
          if (editingRowId === llm.id) {
            return (
              <div className="flex items-center justify-center">
                <Switch checked={editingSwitchStates?.isOverlapRequired || false} onCheckedChange={(checked) => handleEditingSwitchChange("isOverlapRequired", checked)} />
              </div>
            );
          }
          return (
            <div className="flex items-center justify-center">
              <div className={`inline-flex items-center px-2 py-1 rounded-full text-xs font-medium ${llm.isOverlapRequired ? "bg-green-100 text-green-800" : "bg-gray-100 text-gray-800"}`}>{llm.isOverlapRequired ? "Enabled" : "Disabled"}</div>
            </div>
          );
        },
      },

      // ... Actions Column ...
      {
        id: "actions",
        header: () => <div className="text-right">Actions</div>,
        cell: ({ row }) => {
          if (row.original.id === "new") {
            return (
              <div className="flex justify-end items-center gap-2">
                <Button variant="ghost" size="icon" onClick={handleSaveNewLLM} className="hover:bg-green-50 hover:text-green-600">
                  <Check className="w-4 h-4" />
                </Button>
                <Button variant="ghost" size="icon" onClick={handleCancelAdd} className="hover:bg-red-50 hover:text-red-600">
                  <X className="w-4 h-4" />
                </Button>
              </div>
            );
          }
          const llm = row.original as ChunkingStrategies;
          if (editingRowId === llm.id) {
            return (
              <div className="flex justify-end items-center gap-2">
                <Button variant="ghost" size="icon" onClick={handleSaveEditLLM} className="hover:bg-green-50 hover:text-green-600">
                  <Check className="w-4 h-4" />
                </Button>
                <Button variant="ghost" size="icon" onClick={handleCancelEdit} className="hover:bg-red-50 hover:text-red-600">
                  <X className="w-4 h-4" />
                </Button>
              </div>
            );
          }
          return (
            <div className="flex justify-end items-center gap-1">
              <Button variant="outline" size="icon" className="hover:bg-blue-50 hover:text-blue-600" onClick={() => handleEditLLMModel(llm)}>
                <Pencil className="w-4 h-4 text-blue-500" />
                <span className="sr-only">Edit</span>
              </Button>

              <Button variant="outline" size="icon" className="hover:bg-red-50 hover:text-red-600" onClick={() => handleDeleteLLMModel(llm.id as number)}>
                <Trash2 className="w-4 h-4 text-red-500" />
                <span className="sr-only">Delete</span>
              </Button>
            </div>
          );
        },
      },
    ],
    // --- CHANGE: Add switch states and handlers to dependency array ---
    [editingRowId, newSwitchStates, editingSwitchStates, handleNewSwitchChange, handleEditingSwitchChange, handleCancelAdd, handleCancelEdit, handleEditLLMModel, handleEditingLLMChange, handleDeleteLLMModel, handleNewLLMChange, handleSaveEditLLM, handleSaveNewLLM]
  );

  const originalData: ChunkingStrategies[] = data?.items || data || [];
  const tableData = useMemo(() => {
    if (isAddingNew) {
      const newRow: LLMTableRow = { ...INITIAL_NEW_LLM_STATE, id: "new" };
      return [newRow, ...originalData];
    }
    return originalData;
  }, [isAddingNew, originalData]);

  if (isError) {
    return (
      <div className="m-4 p-6 bg-red-50 border border-red-200 rounded-lg">
        <div className="flex items-center gap-2 text-red-800">
          <X className="w-5 h-5" />
          <h3 className="font-medium">Error loading LLMs</h3>
        </div>
        <p className="text-red-600 mt-1">{error?.message}</p>
      </div>
    );
  }

  return (
    <div className="m-4 space-y-4">
      {validationError && (
        <div className="p-4 bg-red-50 border border-red-200 rounded-lg">
          <div className="flex items-center gap-2">
            <X className="w-4 h-4 text-red-600" />
            <p className="text-sm text-red-600 font-medium">{validationError}</p>
          </div>
        </div>
      )}
      <DataTable
        isLoading={isLoading}
        data={tableData}
        columns={columns}
        title={LLM_EXTRACTOR_ENGINE_TYPE.title}
        description={LLM_EXTRACTOR_ENGINE_TYPE.description}
        actionButton={
          <Button onClick={handleAddNewClick} disabled={isAddingNew || editingRowId !== null} className="bg-gradient-to-r from-blue-600 to-purple-600 hover:from-blue-700 hover:to-purple-700">
            <Bot className="w-4 h-4 mr-2" />
            Add New LLM Provider
          </Button>
        }
      />
    </div>
  );
}

export default LLMEmbeddingDatatable;
