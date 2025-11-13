"use client";

import { Button } from "@/components/ui/button";
import { cn } from "@/lib/utils";
import { AlertTriangle, type File, FileText, Info, Paperclip, X } from "lucide-react";
import type React from "react";
import { useCallback, useRef, useState } from "react";

export interface FileTypeConfig {
  id: string;
  label: string;
  accept: string;
  maxSize: number;
}

export const DOCUMENT_FILE_TYPE: FileTypeConfig = {
  id: "document",
  label: "Belge",
  accept: ".pdf,.doc,.docx,.txt",
  maxSize: 10 * 1024 * 1024, // 10MB
};

interface SelectedFile {
  file: File;
  error?: string;
}

export interface FileUploadProps {
  onFilesSelected?: (files: File[]) => void;
  onFilesRemoved?: (files: File[]) => void;
  maxFiles?: number;
  disabled?: boolean;
  className?: string;
  showPreviews?: boolean;
  compact?: boolean;
}

export function FileUpload({ onFilesSelected, onFilesRemoved, maxFiles = 5, disabled = false, className, showPreviews = true, compact = false }: FileUploadProps) {
  const [selectedFiles, setSelectedFiles] = useState<SelectedFile[]>([]);
  const [isDragging, setIsDragging] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const formatFileSize = (bytes: number): string => {
    if (bytes === 0) return "0 Bytes";
    const k = 1024;
    const sizes = ["Bytes", "KB", "MB", "GB"];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return Math.round((bytes / Math.pow(k, i)) * 100) / 100 + " " + sizes[i];
  };

  const validateFile = (file: File): string | null => {
    if (file.size > DOCUMENT_FILE_TYPE.maxSize) {
      return `Dosya boyutu ${formatFileSize(DOCUMENT_FILE_TYPE.maxSize)} limitini aşıyor`;
    }

    const fileExtension = "." + file.name.split(".").pop()?.toLowerCase();
    const acceptedExtensions = [".pdf", ".doc", ".docx", ".txt"];

    if (!acceptedExtensions.includes(fileExtension)) {
      return `Bu dosya tipi kabul edilmiyor. Sadece PDF, DOCX ve TXT dosyaları yükleyebilirsiniz`;
    }

    return null;
  };

  const handleFiles = useCallback(
    async (files: FileList | null) => {
      if (!files || files.length === 0) return;

      const newFiles: SelectedFile[] = [];
      const filesArray = Array.from(files);

      if (selectedFiles.length + filesArray.length > maxFiles) {
        alert(`En fazla ${maxFiles} dosya yükleyebilirsiniz`);
        return;
      }

      for (const file of filesArray) {
        const error = validateFile(file);
        newFiles.push({ file, error: error || undefined });
      }

      const updatedFiles = [...selectedFiles, ...newFiles];
      setSelectedFiles(updatedFiles);

      const validFiles = newFiles.filter((f) => !f.error).map((f) => f.file);
      if (validFiles.length > 0 && onFilesSelected) {
        onFilesSelected(validFiles);
      }
    },
    [selectedFiles, maxFiles, onFilesSelected]
  );

  const handleFileInputChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    handleFiles(e.target.files);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  const handleDragOver = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    if (!disabled) {
      setIsDragging(true);
    }
  };

  const handleDragLeave = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);
  };

  const handleDrop = (e: React.DragEvent) => {
    e.preventDefault();
    e.stopPropagation();
    setIsDragging(false);

    if (!disabled) {
      handleFiles(e.dataTransfer.files);
    }
  };

  const removeFile = (index: number) => {
    const fileToRemove = selectedFiles[index];
    const updatedFiles = selectedFiles.filter((_, i) => i !== index);
    setSelectedFiles(updatedFiles);

    if (onFilesRemoved) {
      onFilesRemoved([fileToRemove.file]);
    }
  };

  const triggerFileInput = () => {
    fileInputRef.current?.click();
  };

  if (compact) {
    return (
      <div className={cn("flex items-center gap-2", className)}>
        <input ref={fileInputRef} type="file" onChange={handleFileInputChange} accept={DOCUMENT_FILE_TYPE.accept} multiple={maxFiles > 1} className="hidden" disabled={disabled} />

        <Button type="button" size="icon" variant="ghost" onClick={triggerFileInput} disabled={disabled || selectedFiles.length >= maxFiles} className="h-9 w-9 text-muted-foreground hover:text-foreground">
          <Paperclip className="h-5 w-5" />
        </Button>

        {selectedFiles.length > 0 && (
          <div className="flex items-center gap-1">
            <span className="text-xs text-muted-foreground">{selectedFiles.length} dosya</span>
            <Button type="button" size="icon" variant="ghost" onClick={() => setSelectedFiles([])} className="h-6 w-6">
              <X className="h-3 w-3" />
            </Button>
          </div>
        )}
      </div>
    );
  }

  return (
    <div className={cn("w-full space-y-4", className)}>
      {/* Upload Area */}
      <div
        onDragOver={handleDragOver}
        onDragLeave={handleDragLeave}
        onDrop={handleDrop}
        className={cn("relative rounded-lg border-2 border-dashed transition-all duration-200", isDragging ? "border-blue-500 bg-blue-500/10" : "border-border bg-muted/20", disabled && "opacity-50 cursor-not-allowed", !disabled && "cursor-pointer hover:border-blue-500/50 hover:bg-muted/40")}
      >
        <input ref={fileInputRef} type="file" onChange={handleFileInputChange} accept={DOCUMENT_FILE_TYPE.accept} multiple={maxFiles > 1} className="hidden" disabled={disabled} />

        <div onClick={!disabled ? triggerFileInput : undefined} className="p-8 text-center">
          <Paperclip className="mx-auto h-12 w-12 text-muted-foreground mb-4" />
          <p className="text-sm font-medium text-foreground mb-2">Dosyaları buraya sürükleyin veya tıklayın</p>
          <div className="space-y-1">
            <p className="text-xs text-muted-foreground">PDF, DOCX, DOC, TXT</p>
            <p className="text-xs font-medium text-blue-500">
              Maksimum {formatFileSize(DOCUMENT_FILE_TYPE.maxSize)} • {maxFiles} dosyaya kadar
            </p>
          </div>
        </div>
      </div>

      {/* File Previews */}
      {showPreviews && selectedFiles.length > 0 && (
        <div className="space-y-2">
          <div className="flex items-center justify-between">
            <p className="text-sm font-medium text-foreground">
              Seçilen Dosyalar ({selectedFiles.length}/{maxFiles})
            </p>
            {selectedFiles.length < maxFiles && <p className="text-xs text-muted-foreground">{maxFiles - selectedFiles.length} dosya daha ekleyebilirsiniz</p>}
          </div>
          <div className="space-y-2">
            {selectedFiles.map((selectedFile, index) => (
              <div key={index} className={cn("flex items-center gap-3 p-3 rounded-lg border transition-colors", selectedFile.error ? "border-destructive bg-destructive/10" : "border-border bg-card")}>
                <div className="flex-shrink-0">
                  <div className="h-12 w-12 rounded bg-muted flex items-center justify-center">
                    <FileText className="h-4 w-4" />
                  </div>
                </div>

                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium text-foreground truncate">{selectedFile.file.name}</p>
                  <div className="flex items-center gap-2">
                    <p className="text-xs text-muted-foreground">{formatFileSize(selectedFile.file.size)}</p>
                    {!selectedFile.error && <span className="text-xs text-green-500">✓ {Math.round((selectedFile.file.size / DOCUMENT_FILE_TYPE.maxSize) * 100)}% limit</span>}
                  </div>
                  {selectedFile.error && (
                    <div className="flex items-center gap-1 mt-1">
                      <AlertTriangle className="h-3 w-3 text-destructive" />
                      <p className="text-xs text-destructive">{selectedFile.error}</p>
                    </div>
                  )}
                </div>

                <Button type="button" size="icon" variant="ghost" onClick={() => removeFile(index)} className="h-8 w-8 flex-shrink-0 text-muted-foreground hover:text-destructive">
                  <X className="h-4 w-4" />
                </Button>
              </div>
            ))}
          </div>
        </div>
      )}

      <div className="flex items-start gap-3 p-4 rounded-lg bg-blue-500/10 border border-blue-500/20">
        <Info className="h-5 w-5 text-blue-500 flex-shrink-0 mt-0.5" />
        <div className="flex-1 space-y-1">
          <p className="text-sm font-medium text-foreground">Dosya Yükleme Bilgileri</p>
          <div className="text-xs text-muted-foreground space-y-0.5">
            <p>
              • Maksimum dosya boyutu: <span className="font-semibold text-foreground">{formatFileSize(DOCUMENT_FILE_TYPE.maxSize)}</span>
            </p>
            <p>
              • İzin verilen dosya tipleri: <span className="font-semibold text-foreground">PDF, DOCX, DOC, TXT</span>
            </p>
            <p>
              • Maksimum dosya sayısı: <span className="font-semibold text-foreground">{maxFiles} dosya</span>
            </p>
          </div>
        </div>
      </div>
    </div>
  );
}
