"use client";
import { Button } from "@/components/ui/button";
import { DataTable } from "@/components/ui/datatable";
import { Switch } from "@/components/ui/switch";
import type { ColumnDef } from "@tanstack/react-table";
import { Bot, Check, X, Pencil, Trash2 } from "lucide-react";
import { useState, useMemo, useRef, useCallback } from "react";
import { toast } from "sonner";
import type { AppChunkingStrategies } from "../_types/types";
import { LLM_APP_STRATEGY_TYPE } from "../_constants/LlmModelsConstants";
import { EditableCell } from "@/components/datatable/EditableCell";
import { useCreateLLMEmbedding, useDeleteLLMEmbedding, useLLMEmbedding, useUpdateLLMEmbedding } from "../_hooks/useLLMEmbedding";
import { useRouter } from "next/navigation";
import Image from "next/image";

type LLMTableRow = AppChunkingStrategies | { id: "new" | "update" };

const INITIAL_NEW_LLM_STATE: Omit<AppChunkingStrategies, "id"> = {
  name: "",
  identifier: "",
  hasApplicationFile: false,
  hasUserFile: false,
  description: "",
  systemPrompt: "",
  applicationTypeId: 0,
  memoryTypeId: 0,
  outputTypeId: 0,
};

function LLMEmbeddingDatatable() {
  const [pageIndex, setPageIndex] = useState(0);
  const [pageSize, setPageSize] = useState(10);

  const { data, isLoading, error, isError } = useLLMEmbedding(pageIndex, pageSize);

  console.log("data", data);
  const createLLM = useCreateLLMEmbedding();
  const updateLLM = useUpdateLLMEmbedding();
  const deleteLLM = useDeleteLLMEmbedding();
  const router = useRouter();
  const [isAddingNew, setIsAddingNew] = useState(false);
  const [editingRowId, setEditingRowId] = useState<string | number | null>(null);
  const [validationError, setValidationError] = useState<string>("");

  const handlePaginationChange = (newPageIndex: number, newPageSize: number) => {
    setPageIndex(newPageIndex);
    setPageSize(newPageSize);
  };

  // Refs for non-reactive input data
  const newLLMDataRef = useRef<Omit<AppChunkingStrategies, "id">>(INITIAL_NEW_LLM_STATE);
  const editingLLMDataRef = useRef<AppChunkingStrategies | null>(null);

  const booleanFields = ["hasApplicationFile", "hasUserFile"];
  const numberFields = ["applicationTypeId", "memoryTypeId", "outputTypeId"];

  const [newBooleans, setNewBooleans] = useState({
    hasApplicationFile: false,
    hasUserFile: false,
  });

  const [editBooleans, setEditBooleans] = useState({
    hasApplicationFile: false,
    hasUserFile: false,
  });

  const coerce = (field: string, value: any) => {
    if (numberFields.includes(field)) return Number(value) || 0;
    if (booleanFields.includes(field)) return value === true || value === "true";
    return value;
  };

  const handleAddNewClick = useCallback(() => {
    // setIsAddingNew(true);
    // newLLMDataRef.current = { ...INITIAL_NEW_LLM_STATE };
    // setEditingRowId(null);
    // editingLLMDataRef.current = null;

    // // Reset boolean state
    // setNewBooleans({
    //   hasApplicationFile: false,
    //   hasUserFile: false,
    // });
    router.push("/setup-wizard");
  }, []);
  const handleCancelAdd = useCallback(() => {
    setIsAddingNew(false);
    setValidationError("");
  }, []);

  const handleCancelEdit = useCallback(() => {
    setEditingRowId(null);
    editingLLMDataRef.current = null;
    setValidationError("");
  }, []);

  const validateLLMData = useCallback((data: Omit<AppChunkingStrategies, "id">): string => {
    if (!data.name?.trim()) return "Name is required.";
    if (!data.identifier?.trim()) return "Identifier is required.";
    if (!data.applicationTypeId) return "Application type is required.";
    if (!data.memoryTypeId) return "Memory type is required.";
    if (!data.outputTypeId) return "Output type is required.";
    return "";
  }, []);

  const handleSaveNewLLM = useCallback(() => {
    const payload = { ...newLLMDataRef.current };

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
  }, [createLLM, validateLLMData]);

  const handleSaveEditLLM = useCallback(() => {
    if (!editingLLMDataRef.current) return;

    const payload = { ...editingLLMDataRef.current };

    const error = validateLLMData(payload);
    if (error) {
      setValidationError(error);
      return;
    }
    console.log("payload", payload);
    setValidationError("");
    updateLLM.mutate(payload, {
      onSuccess: () => {
        toast.success("LLM successfully updated");
        setEditingRowId(null);
        editingLLMDataRef.current = null;
      },
      onError: (error: any) => {
        toast.error(`Error updating LLM: ${error.data?.[0]?.errorMessage || error.message || "Unknown error"}`);
      },
    });
  }, [updateLLM, validateLLMData]);

  const handleNewLLMChange = useCallback(
    (field: any, value: any) => {
      if (validationError) setValidationError("");

      if (booleanFields.includes(field)) {
        setNewBooleans((prev) => ({ ...prev, [field]: value }));
      }

      newLLMDataRef.current = {
        ...newLLMDataRef.current,
        [field]: coerce(field, value),
      };
    },
    [validationError]
  );

  const handleEditingLLMChange = useCallback(
    (field: keyof AppChunkingStrategies, value: any) => {
      if (validationError) setValidationError("");
      if (!editingLLMDataRef.current) return;

      if (booleanFields.includes(field as string)) {
        setEditBooleans((prev) => ({ ...prev, [field]: value }));
      }

      editingLLMDataRef.current = {
        ...editingLLMDataRef.current,
        [field]: coerce(field as string, value),
      };
    },
    [validationError]
  );
  const handleEditLLMModel = useCallback((llm: AppChunkingStrategies) => {
    setIsAddingNew(false);
    setEditingRowId(llm.id as any);
    editingLLMDataRef.current = { ...llm };
    setEditBooleans({ hasApplicationFile: editingLLMDataRef.current.hasApplicationFile, hasUserFile: editingLLMDataRef.current.hasUserFile });
    console.log("editingLLMDataRef", editingLLMDataRef.current);
  }, []);

  const handleDeleteLLMModel = useCallback(
    (llmId: number) => {
      deleteLLM.mutate(llmId, {
        onSuccess: () => toast.success("LLM successfully deleted"),
        onError: (error: any) => toast.error(`Error deleting LLM: ${error.data?.[0]?.errorMessage || error.message || "Unknown error"}`),
      });
    },
    [deleteLLM]
  );

  const columns = useMemo<ColumnDef<LLMTableRow>[]>(
    () => [
      {
        accessorKey: "id",
        header: "ID",
        cell: ({ row }) => {
          const llm = row.original as AppChunkingStrategies;

          if (row.original.id === "new") {
            return <span className="text-muted-foreground">New</span>;
          }

          return (
            <div className="flex items-center gap-3">
              <div>
                <div className="font-medium text-foreground">{llm.id}</div>
              </div>
            </div>
          );
        },
      },
      {
        accessorKey: "name",
        header: "İsim",
        cell: ({ row }) => {
          const llm = row.original as AppChunkingStrategies;
          if (row.original.id === "new") {
            return <EditableCell placeholder="Enter name" defaultValue={newLLMDataRef.current.name} onChange={(value) => handleNewLLMChange("name", value)} className="max-w-xs font-medium  truncate" />;
          }

          if (editingRowId === llm.id) {
            return <EditableCell placeholder="Enter name" defaultValue={editingLLMDataRef.current?.name || ""} onChange={(value) => handleEditingLLMChange("name", value)} className="max-w-xs font-medium truncate" />;
          }
          return <div className="font-medium text-foreground truncate">{llm.name}</div>;
        },
      },
      {
        accessorKey: "identifier",
        header: "Identifier",
        cell: ({ row }) => {
          const llm = row.original as AppChunkingStrategies;
          if (row.original.id === "new") {
            return <EditableCell placeholder="Enter identifier" defaultValue={newLLMDataRef.current.identifier} onChange={(value) => handleNewLLMChange("identifier", value)} className="max-w-xs font-medium truncate" />;
          }

          if (editingRowId === llm.id) {
            return <EditableCell placeholder="Enter identifier" defaultValue={editingLLMDataRef.current?.identifier || ""} onChange={(value) => handleEditingLLMChange("identifier", value)} className="max-w-xs font-medium truncate" />;
          }
          return <div className="font-medium text-foreground truncate">{llm.identifier}</div>;
        },
      },
      {
        accessorKey: "hasApplicationFile",
        header: "Has App File",
        cell: ({ row }) => {
          const llm = row.original as AppChunkingStrategies;

          if (row.original.id === "new") {
            return <Switch checked={newBooleans.hasApplicationFile} onCheckedChange={(value) => handleNewLLMChange("hasApplicationFile", value)} />;
          }

          if (editingRowId === llm.id) {
            return <Switch checked={editBooleans.hasApplicationFile} onCheckedChange={(value) => handleEditingLLMChange("hasApplicationFile", value)} />;
          }

          // ✅ NEW: Display icon instead of text
          return (
            <div className="flex items-center justify-center">
              {llm.hasApplicationFile ? (
                <div className="flex items-center justify-center h-6 w-6 rounded-full bg-green-100 dark:bg-green-950">
                  <Check className="h-4 w-4 text-green-600 dark:text-green-400" />
                </div>
              ) : (
                <div className="flex items-center justify-center h-6 w-6 rounded-full bg-red-100 dark:bg-red-950">
                  <X className="h-4 w-4 text-red-600 dark:text-red-400" />
                </div>
              )}
            </div>
          );
        },
      },
      {
        accessorKey: "hasUserFile",
        header: "Has User File",
        cell: ({ row }) => {
          const llm = row.original as AppChunkingStrategies;

          if (row.original.id === "new") {
            return <Switch checked={newBooleans.hasUserFile} onCheckedChange={(value) => handleNewLLMChange("hasUserFile", value)} />;
          }

          if (editingRowId === llm.id) {
            return <Switch checked={editBooleans.hasUserFile} onCheckedChange={(value) => handleEditingLLMChange("hasUserFile", value)} />;
          }

          // ✅ NEW: Display icon instead of text
          return (
            <div className="flex items-center justify-center">
              {llm.hasUserFile ? (
                <div className="flex items-center justify-center h-6 w-6 rounded-full bg-green-100 dark:bg-green-950">
                  <Check className="h-4 w-4 text-green-600 dark:text-green-400" />
                </div>
              ) : (
                <div className="flex items-center justify-center h-6 w-6 rounded-full bg-red-100 dark:bg-red-950">
                  <X className="h-4 w-4 text-red-600 dark:text-red-400" />
                </div>
              )}
            </div>
          );
        },
      },
      {
        accessorKey: "description",
        header: "Description",
        cell: ({ row }) => {
          const llm = row.original as AppChunkingStrategies;
          if (row.original.id === "new") {
            return <EditableCell placeholder="Enter description" defaultValue={newLLMDataRef.current.description} onChange={(value) => handleNewLLMChange("description", value)} className="max-w-xs font-medium" />;
          }

          if (editingRowId === llm.id) {
            return <EditableCell placeholder="Enter description" defaultValue={editingLLMDataRef.current?.description || ""} onChange={(value) => handleEditingLLMChange("description", value)} className="max-w-xs font-medium" />;
          }
          return <div className="font-medium text-foreground">{llm.description}</div>;
        },
      },
      {
        accessorKey: "systemPrompt",
        header: "System Prompt",
        cell: ({ row }) => {
          const llm = row.original as AppChunkingStrategies;
          if (row.original.id === "new") {
            // Bu kısım zaten truncate sınıfına sahip, doğru.
            return <EditableCell placeholder="Enter system prompt" defaultValue={newLLMDataRef.current.systemPrompt} onChange={(value) => handleNewLLMChange("systemPrompt", value)} className="max-w-xs font-medium truncate" />;
          }

          if (editingRowId === llm.id) {
            // Düzenleme moduna da truncate ekleyelim ki düzenleme sırasında da düzen bozulmasın.
            return <EditableCell placeholder="Enter system prompt" defaultValue={editingLLMDataRef.current?.systemPrompt || ""} onChange={(value) => handleEditingLLMChange("systemPrompt", value)} className="max-w-xs font-medium truncate" />;
          }

          // GÜNCELLENEN KISIM BURASI
          // Metnin gösterildiği normal durum için truncate, max-w-xs ve title özellikleri eklendi.
          return (
            <div className="max-w-xs truncate font-medium text-foreground" title={llm.systemPrompt}>
              {llm.systemPrompt}
            </div>
          );
        },
      },
      {
        accessorKey: "applicationTypeId",
        header: "App Type ID",
        cell: ({ row }) => {
          const llm = row.original as AppChunkingStrategies;
          if (row.original.id === "new") {
            return <EditableCell placeholder="Enter app type ID" defaultValue={newLLMDataRef.current.applicationTypeId?.toString() || ""} onChange={(value) => handleNewLLMChange("applicationTypeId", value)} className="max-w-xs font-medium" />;
          }

          if (editingRowId === llm.id) {
            return <EditableCell placeholder="Enter app type ID" defaultValue={editingLLMDataRef.current?.applicationTypeId?.toString() || ""} onChange={(value) => handleEditingLLMChange("applicationTypeId", value)} className="max-w-xs font-medium" />;
          }
          return <div className="font-medium text-foreground">{llm.applicationTypeId}</div>;
        },
      },
      {
        accessorKey: "memoryTypeId",
        header: "Memory Type ID",
        cell: ({ row }) => {
          const llm = row.original as AppChunkingStrategies;
          if (row.original.id === "new") {
            return <EditableCell placeholder="Enter memory type ID" defaultValue={newLLMDataRef.current.memoryTypeId?.toString() || ""} onChange={(value) => handleNewLLMChange("memoryTypeId", value)} className="max-w-xs font-medium" />;
          }

          if (editingRowId === llm.id) {
            return <EditableCell placeholder="Enter memory type ID" defaultValue={editingLLMDataRef.current?.memoryTypeId?.toString() || ""} onChange={(value) => handleEditingLLMChange("memoryTypeId", value)} className="max-w-xs font-medium" />;
          }
          return <div className="font-medium text-foreground">{llm.memoryTypeId}</div>;
        },
      },
      {
        accessorKey: "outputTypeId",
        header: "Output Type ID",
        cell: ({ row }) => {
          const llm = row.original as AppChunkingStrategies;
          if (row.original.id === "new") {
            return <EditableCell placeholder="Enter output type ID" defaultValue={newLLMDataRef.current.outputTypeId?.toString() || ""} onChange={(value) => handleNewLLMChange("outputTypeId", value)} className="max-w-xs font-medium" />;
          }

          if (editingRowId === llm.id) {
            return <EditableCell placeholder="Enter output type ID" defaultValue={editingLLMDataRef.current?.outputTypeId?.toString() || ""} onChange={(value) => handleEditingLLMChange("outputTypeId", value)} className="max-w-xs font-medium" />;
          }
          return <div className="font-medium text-foreground">{llm.outputTypeId}</div>;
        },
      },
      {
        id: "actions",
        header: () => <div className="text-right">İşlemler</div>,
        cell: ({ row }) => {
          if (row.original.id === "new") {
            return (
              <div className="flex justify-end items-center gap-2">
                <Button variant="ghost" size="icon" onClick={handleSaveNewLLM} className="h-8 w-8 hover:bg-green-50 hover:text-green-600 dark:hover:bg-green-950 transition-all">
                  <Check className="w-4 h-4" />
                </Button>
                <Button variant="ghost" size="icon" onClick={handleCancelAdd} className="h-8 w-8 hover:bg-red-50 hover:text-red-600 dark:hover:bg-red-950 transition-all">
                  <X className="w-4 h-4" />
                </Button>
              </div>
            );
          }

          const llm = row.original as AppChunkingStrategies;

          if (editingRowId === llm.id) {
            return (
              <div className="flex justify-end items-center gap-2">
                <Button variant="ghost" size="icon" onClick={handleSaveEditLLM} className="h-8 w-8 hover:bg-green-50 hover:text-green-600 dark:hover:bg-green-950 transition-all">
                  <Check className="w-4 h-4" />
                </Button>
                <Button variant="ghost" size="icon" onClick={handleCancelEdit} className="h-8 w-8 hover:bg-red-50 hover:text-red-600 dark:hover:bg-red-950 transition-all">
                  <X className="w-4 h-4" />
                </Button>
              </div>
            );
          }

          return (
            <div className="flex justify-end items-center gap-1">
              <Button variant="ghost" size="icon" className="h-8 w-8 hover:bg-blue-50 hover:text-blue-600 dark:hover:bg-blue-950 transition-all group" onClick={() => handleEditLLMModel(llm)}>
                <Pencil className="w-4 h-4 transition-transform group-hover:scale-110" />
                <span className="sr-only">Düzenle</span>
              </Button>

              <Button variant="ghost" size="icon" className="h-8 w-8 hover:bg-red-50 hover:text-red-600 dark:hover:bg-red-950 transition-all group" onClick={() => handleDeleteLLMModel(llm.id as number)}>
                <Trash2 className="w-4 h-4 transition-transform group-hover:scale-110" />
                <span className="sr-only">Sil</span>
              </Button>
            </div>
          );
        },
      },
    ],
    [editingRowId, handleCancelAdd, handleCancelEdit, handleEditLLMModel, handleEditingLLMChange, handleDeleteLLMModel, handleNewLLMChange, handleSaveEditLLM, handleSaveNewLLM]
  );

  const originalData: AppChunkingStrategies[] = data?.items || [];
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
        title={LLM_APP_STRATEGY_TYPE.title}
        description={LLM_APP_STRATEGY_TYPE.description}
        actionButton={
          <Button onClick={handleAddNewClick} disabled={isAddingNew || editingRowId !== null}>
            Yeni Uygulama Ekle
          </Button>
        }
        filterColumn="name"
        filterPlaceholder="İsim ara..."
        // ✅ Pass the entire pagination data object
        paginationData={data}
        onPaginationChange={handlePaginationChange}
      />
    </div>
  );
}

export default LLMEmbeddingDatatable;
