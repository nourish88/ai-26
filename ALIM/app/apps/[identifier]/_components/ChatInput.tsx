"use client";

import { Button } from "@/components/ui/button";
import { Textarea } from "@/components/ui/textarea";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import { cn } from "@/lib/utils";
import { Loader2, Send, X, Paperclip, FileText, Trash2, File, FolderOpen } from "lucide-react";
import type React from "react";
import { forwardRef, useState, useTransition, useRef } from "react";

import { toast } from "sonner";
import { type UploadActionState, uploadFilesAction } from "../_actions/fileActions";
import { deleteFile } from "../_actions/fileActions";
import { useAutosizeTextArea } from "../_hooks/useAutosizeTextArea";

interface UserFile {
  id: string;
  fileName: string;
}

interface ChatInputWithUploadProps {
  input: string;
  isLoading: boolean;
  canSendMessage: boolean;
  onInputChange: (e: React.ChangeEvent<HTMLTextAreaElement>) => void;
  onSendMessage: () => void;
  onKeyDown: (e: React.KeyboardEvent<HTMLTextAreaElement>) => void;
  maxFiles?: number;
  onFilesUploaded?: (success: boolean, message: string) => void;
  hasUserFile?: boolean;
  identifier?: string | null;
  allFiles?: UserFile[];
  onSelectionChange?: (selectedIds: string[]) => void;
}

export const ChatInputWithUpload = forwardRef<HTMLTextAreaElement, ChatInputWithUploadProps>(({ hasUserFile = false, input, isLoading, canSendMessage, onInputChange, onSendMessage, onKeyDown, maxFiles = 5, onFilesUploaded, identifier, allFiles = [], onSelectionChange }, ref) => {
  const [selectedFiles, setSelectedFiles] = useState<File[]>([]);
  const [isUploading, startUploadTransition] = useTransition();
  const fileInputRef = useRef<HTMLInputElement>(null);
  const textareaRef = ref as React.RefObject<HTMLTextAreaElement>;

  const [dbFiles, setDbFiles] = useState<UserFile[]>(allFiles);
  const [selectedDbFileIds, setSelectedDbFileIds] = useState<string[]>([]);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [isDbFilesOpen, setIsDbFilesOpen] = useState(false);
  console.log("dbFiles", dbFiles);
  useAutosizeTextArea(textareaRef?.current, input);

  const handleFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = Array.from(e.target.files || []);
    if (files.length === 0) return;
    if (selectedFiles.length + files.length > maxFiles) {
      toast.error(`Maksimum ${maxFiles} dosya yükleyebilirsiniz`);
      return;
    }
    const validFiles: File[] = [];
    const acceptedExtensions = [".pdf", ".docx", ".doc", ".txt"];
    for (const file of files) {
      if (file.size > 10 * 1024 * 1024) {
        toast.error(`${file.name} çok büyük (Max 10MB)`);
        continue;
      }
      const fileExtension = `.${file.name.split(".").pop()?.toLowerCase()}`;
      if (!acceptedExtensions.includes(fileExtension)) {
        toast.error(`${file.name} desteklenmeyen dosya türü`);
        continue;
      }
      validFiles.push(file);
    }
    if (validFiles.length > 0) {
      setSelectedFiles((prev) => [...prev, ...validFiles]);
      toast.success(`${validFiles.length} dosya eklendi`);
    }
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const removeFile = (index: number) => {
    setSelectedFiles((prev) => prev.filter((_, i) => i !== index));
  };

  const clearAllFiles = () => {
    setSelectedFiles([]);
  };

  const handleDbFileToggle = (fileId: string) => {
    const newSelection = selectedDbFileIds.includes(fileId) ? selectedDbFileIds.filter((id) => id !== fileId) : [...selectedDbFileIds, fileId];
    setSelectedDbFileIds(newSelection);
    onSelectionChange?.(newSelection);
  };

  const handleDbFileDelete = async (fileId: string, e: React.MouseEvent) => {
    e.stopPropagation();
    if (!confirm("Bu dosyayı kalıcı olarak silmek istediğinizden emin misiniz?")) {
      return;
    }
    if (!identifier) return;
    setDeletingId(fileId);
    const result = await deleteFile(fileId, identifier);
    setDeletingId(null);
    if (result.success) {
      setDbFiles((current) => current.filter((file) => file.id !== fileId));
      setSelectedDbFileIds((current) => current.filter((id) => id !== fileId));
      onSelectionChange?.(selectedDbFileIds.filter((id) => id !== fileId));
      toast.success("Dosya silindi");
    } else {
      toast.error(`Hata: ${result.message}`);
    }
  };

  const removeDbFileFromSelection = (fileId: string) => {
    const newSelection = selectedDbFileIds.filter((id) => id !== fileId);
    setSelectedDbFileIds(newSelection);
    onSelectionChange?.(newSelection);
  };

  const handleUploadFiles = () => {
    if (selectedFiles.length === 0) return;
    if (!identifier) {
      toast.error("App identifier eksik");
      return;
    }
    startUploadTransition(async () => {
      const formData = new FormData();
      console.log("selectedFiles", selectedFiles);
      selectedFiles.forEach((file, index) => {
        formData.append(`files[${index}].File`, file);
      });

      console.log("FormData", formData);
      const result: UploadActionState = await uploadFilesAction(identifier, formData);
      if (result.success) {
        toast.success(result.message);
        clearAllFiles();
        onFilesUploaded?.(true, result.message);
      } else {
        toast.error(result.message);
        onFilesUploaded?.(false, result.message);
      }
    });
  };

  const handleSendWithFiles = () => {
    if (hasUserFile && selectedFiles.length > 0) {
      handleUploadFiles();
    }
    if (input.trim()) {
      onSendMessage();
    }
  };

  const normalizeFileName = (name: string) => {
    // Sunucu tarafında render ediliyorsa veya 'document' yoksa, değişik yapma.
    if (typeof document === "undefined") {
      return name;
    }
    const textarea = document.createElement("textarea");
    textarea.innerHTML = name;
    return textarea.value;
  };

  // Dosya adını render etmeden önce normalleştir.

  const selectedDbFiles = dbFiles.filter((file) => selectedDbFileIds.includes(file.id));

  return (
    <div className="bg-black/10 backdrop-blur-lg border-t border-white/10 p-4 sm:p-6">
      <div className="max-w-4xl mx-auto space-y-3">
        {hasUserFile && selectedFiles.length === 0 && (
          <div className="text-center">
            <p className="text-xs text-muted-foreground">PDF, DOCX, DOC, TXT • Max 10MB • {maxFiles} dosyaya kadar</p>
          </div>
        )}

        {hasUserFile && selectedFiles.length > 0 && (
          <div className="rounded-xl bg-gradient-to-br from-black/30 to-black/20 p-3 ring-1 ring-white/10 backdrop-blur-sm">
            <div className="flex items-center justify-between mb-2">
              <p className="text-xs font-semibold text-foreground flex items-center gap-2">
                <FileText className="h-4 w-4 text-blue-400" />
                {selectedFiles.length} yeni dosya seçildi
              </p>
              <Button type="button" size="sm" variant="ghost" onClick={clearAllFiles} className="h-6 text-xs text-muted-foreground hover:text-red-400 px-2 transition-colors">
                Tümünü sil
              </Button>
            </div>
            <div className="space-y-1.5">
              {selectedFiles.map((file, index) => (
                <div key={index} className="flex items-center justify-between p-2.5 rounded-lg bg-black/40 text-xs group hover:bg-black/50 transition-colors">
                  <span className="text-foreground truncate flex-1 font-medium"> {normalizeFileName(file.name)} </span>
                  <div className="flex items-center gap-3">
                    <span className="text-muted-foreground text-xs">{(file.size / 1024).toFixed(0)}KB</span>
                    <Button type="button" size="sm" variant="ghost" onClick={() => removeFile(index)} className="h-6 w-6 p-0 opacity-0 group-hover:opacity-100 transition-opacity hover:bg-red-500/10">
                      <X className="h-3.5 w-3.5 text-muted-foreground hover:text-red-400" />
                    </Button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {hasUserFile && selectedDbFiles.length > 0 && (
          <div className="rounded-xl bg-gradient-to-br from-blue-500/10 to-blue-500/10 p-3 ring-1 ring-blue-500/20 backdrop-blur-sm">
            <div className="flex items-center justify-between mb-2">
              <p className="text-xs font-semibold text-foreground flex items-center gap-2">
                <FolderOpen className="h-4 w-4 text-blue-400" />
                {selectedDbFiles.length} mevcut dosya seçildi
              </p>
            </div>
            <div className="flex flex-wrap gap-2">
              {selectedDbFiles.map((file) => (
                <Badge key={file.id} variant="secondary" className="pl-2 pr-1 py-1 gap-1.5 max-w-[200px] bg-blue-500/20">
                  <File className="h-3 w-3 shrink-0" />
                  {/* === THIS IS A FIX === */}
                  <span className="truncate text-xs">{normalizeFileName(file.fileName)}</span>
                  <button onClick={() => removeDbFileFromSelection(file.id)} className="ml-auto hover:bg-muted rounded-sm p-0.5 transition-colors">
                    <X className="h-3 w-3" />
                  </button>
                </Badge>
              ))}
            </div>
          </div>
        )}

        <div className="flex flex-col w-full gap-2 rounded-2xl bg-gradient-to-br from-black/30 to-black/20 p-3 ring-1 ring-white/10 focus-within:ring-2 focus-within:ring-blue-500/50 focus-within:shadow-lg focus-within:shadow-blue-500/10 transition-all duration-300">
          <div className="relative flex items-end w-full gap-3">
            <Textarea
              ref={textareaRef}
              value={input}
              onChange={onInputChange}
              onKeyDown={onKeyDown}
              placeholder="Bir mesaj gönder..."
              className="flex-1 bg-transparent border-0 resize-none overflow-y-hidden focus-visible:ring-0 focus-visible:ring-offset-0 p-2 text-base placeholder:text-muted-foreground/60 max-h-48 leading-relaxed"
              rows={1}
              disabled={isLoading || isUploading}
              autoFocus
            />
            <div className="flex items-center gap-2 flex-shrink-0">
              {hasUserFile && (
                <>
                  <input ref={fileInputRef} type="file" multiple accept=".pdf,.docx,.doc,.txt" onChange={handleFileInputChange} className="hidden" />

                  {dbFiles.length > 0 && (
                    <Popover open={isDbFilesOpen} onOpenChange={setIsDbFilesOpen}>
                      <PopoverTrigger asChild>
                        <Button
                          type="button"
                          size="icon"
                          variant="ghost"
                          disabled={isLoading || isUploading}
                          className={cn("h-10 w-10 transition-all duration-200 rounded-lg relative", selectedDbFileIds.length > 0 ? "text-blue-400 hover:text-blue-300 hover:bg-blue-500/10" : "text-muted-foreground hover:text-blue-400 hover:bg-blue-500/10")}
                          title="Mevcut dosyalarım"
                        >
                          <FolderOpen className="h-5 w-5" />
                          {selectedDbFileIds.length > 0 ? (
                            <Badge variant="default" className="absolute -top-1 -right-1 h-5 min-w-5 px-1 bg-blue-500">
                              {selectedDbFileIds.length}
                            </Badge>
                          ) : (
                            <span className="absolute -top-1 -right-1 flex h-3 w-3">
                              <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-blue-400 opacity-75"></span>
                              <span className="relative inline-flex rounded-full h-3 w-3 bg-blue-500"></span>
                            </span>
                          )}
                        </Button>
                      </PopoverTrigger>
                      <PopoverContent className="w-[95vw] max-w-3xl p-0 rounded-xl border border-white/10 bg-black/50 backdrop-blur-lg shadow-2xl" align="center" side="top" sideOffset={8}>
                        <div className="flex items-center justify-between border-b border-white/10 px-4 py-3 bg-black/20">
                          <h4 className="font-semibold text-sm text-foreground">Dosyalarım</h4>
                          <span className="text-xs text-muted-foreground">{dbFiles.length} dosya</span>
                        </div>
                        <ScrollArea className="h-[300px]">
                          <div className="p-2 space-y-1">
                            {dbFiles.map((file) => {
                              const isSelected = selectedDbFileIds.includes(file.id);
                              const isDeleting = deletingId === file.id;

                              return (
                                <div
                                  key={file.id}
                                  className={cn("flex items-center gap-3 rounded-lg p-3 transition-all group", "cursor-pointer border", isSelected ? "bg-blue-500/15 hover:bg-blue-500/20 border-blue-500/30" : "bg-white/5 hover:bg-white/10 border-transparent")}
                                  onClick={() => handleDbFileToggle(file.id)}
                                >
                                  <input type="checkbox" checked={isSelected} readOnly className="h-4 w-4 shrink-0 cursor-pointer accent-blue-500 rounded focus:ring-0 focus:ring-offset-0" />
                                  <File className={cn("h-4 w-4 shrink-0", isSelected ? "text-blue-400" : "text-muted-foreground")} />
                                  {/* === THIS IS A FIX === */}
                                  <span className="flex-1 text-sm font-medium min-w-0 text-foreground" title={normalizeFileName(file.fileName)}>
                                    <span className="block truncate">{normalizeFileName(file.fileName)}</span>
                                  </span>
                                  <Button variant="ghost" size="sm" disabled={isDeleting} onClick={(e) => handleDbFileDelete(file.id, e)} className="h-8 w-8 p-0 shrink-0 opacity-0 group-hover:opacity-100 transition-opacity hover:bg-red-500/10 hover:text-red-400">
                                    {isDeleting ? <Loader2 className="h-4 w-4 animate-spin" /> : <Trash2 className="h-4 w-4" />}
                                  </Button>
                                </div>
                              );
                            })}
                          </div>
                        </ScrollArea>
                      </PopoverContent>
                    </Popover>
                  )}

                  <Button
                    type="button"
                    size="icon"
                    variant="ghost"
                    onClick={() => fileInputRef.current?.click()}
                    disabled={isLoading || isUploading || selectedFiles.length >= maxFiles}
                    className="h-10 w-10 text-muted-foreground hover:text-blue-400 hover:bg-blue-500/10 transition-all duration-200 rounded-lg"
                    title="Yeni dosya ekle"
                  >
                    <Paperclip className="h-5 w-5" />
                  </Button>
                </>
              )}

              <Button
                type="button"
                onClick={handleSendWithFiles}
                disabled={!canSendMessage && (!hasUserFile || selectedFiles.length === 0)}
                className={cn(
                  "h-10 w-10 transition-all duration-300 rounded-lg",
                  canSendMessage || (hasUserFile && selectedFiles.length > 0) ? "bg-gradient-to-r from-blue-600 to-blue-500 hover:from-blue-500 hover:to-blue-400 text-white shadow-lg shadow-blue-500/30 hover:shadow-xl hover:shadow-blue-500/40" : "bg-transparent text-muted-foreground"
                )}
                size="icon"
                variant="ghost"
              >
                {isLoading || isUploading ? <Loader2 className="h-5 w-5 animate-spin" /> : <Send className="w-5 h-5" />}
              </Button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
});

ChatInputWithUpload.displayName = "ChatInputWithUpload";
