"use client";

import type React from "react";

import { deleteFile } from "@/app/apps/[identifier]/_actions/fileActions";
import { useState } from "react";
import { Button } from "@/components/ui/button";
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover";
import { Badge } from "@/components/ui/badge";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Paperclip, X, File, Trash2 } from "lucide-react";
import { cn } from "@/lib/utils";

// Define a clear type for the file object
interface UserFile {
  id: string;
  fileName: string;
}

interface FilePickerProps {
  initialFiles: UserFile[];
  identifier: string;
  onSelectionChange: (selectedIds: string[]) => void;
}

export function FilePicker({ initialFiles, identifier, onSelectionChange }: FilePickerProps) {
  const [files, setFiles] = useState<UserFile[]>(initialFiles);
  const [selectedFileIds, setSelectedFileIds] = useState<string[]>([]);
  const [deletingId, setDeletingId] = useState<string | null>(null);
  const [isOpen, setIsOpen] = useState(false);

  const handleFileToggle = (fileId: string) => {
    const newSelection = selectedFileIds.includes(fileId) ? selectedFileIds.filter((id) => id !== fileId) : [...selectedFileIds, fileId];

    setSelectedFileIds(newSelection);
    onSelectionChange(newSelection);
  };

  const handleDelete = async (fileId: string, e: React.MouseEvent) => {
    e.stopPropagation(); // Prevent checkbox toggle when clicking delete

    if (!confirm("Are you sure you want to permanently delete this file?")) {
      return;
    }

    setDeletingId(fileId);
    const result = await deleteFile(fileId, identifier);
    setDeletingId(null);

    if (result.success) {
      setFiles((currentFiles) => currentFiles.filter((file) => file.id !== fileId));
      // Remove from selection if it was selected
      setSelectedFileIds((current) => current.filter((id) => id !== fileId));
      onSelectionChange(selectedFileIds.filter((id) => id !== fileId));
    } else {
      alert(`Error: ${result.message}`);
    }
  };

  const removeFromSelection = (fileId: string) => {
    const newSelection = selectedFileIds.filter((id) => id !== fileId);
    setSelectedFileIds(newSelection);
    onSelectionChange(newSelection);
  };

  const selectedFiles = files.filter((file) => selectedFileIds.includes(file.id));

  if (files.length === 0) {
    return null; // Don't show anything if there are no files
  }

  return (
    <div className="flex flex-col gap-2 px-4 pb-2">
      {/* Selected files display */}
      {selectedFiles.length > 0 && (
        <div className="flex flex-wrap gap-2">
          {selectedFiles.map((file) => (
            <Badge key={file.id} variant="secondary" className="pl-2 pr-1 py-1 gap-1.5 max-w-[200px]">
              <File className="h-3 w-3 shrink-0" />
              <span className="truncate text-xs">{file.fileName}</span>
              <button onClick={() => removeFromSelection(file.id)} className="ml-auto hover:bg-muted rounded-sm p-0.5 transition-colors">
                <X className="h-3 w-3" />
              </button>
            </Badge>
          ))}
        </div>
      )}

      {/* File picker trigger */}
      <Popover open={isOpen} onOpenChange={setIsOpen}>
        <PopoverTrigger asChild>
          <Button variant="outline" size="sm" className="w-fit gap-2 bg-transparent">
            <Paperclip className="h-4 w-4" />
            <span>Attach Files</span>
            {selectedFileIds.length > 0 && (
              <Badge variant="default" className="ml-1 h-5 min-w-5 px-1.5">
                {selectedFileIds.length}
              </Badge>
            )}
          </Button>
        </PopoverTrigger>
        <PopoverContent className="w-80 p-0" align="start">
          <div className="flex items-center justify-between border-b px-4 py-3">
            <h4 className="font-semibold text-sm">Your Files</h4>
            <span className="text-xs text-muted-foreground">
              {files.length} {files.length === 1 ? "file" : "files"}
            </span>
          </div>
          <ScrollArea className="h-[300px]">
            <div className="p-2">
              {files.map((file) => {
                const isSelected = selectedFileIds.includes(file.id);
                const isDeleting = deletingId === file.id;

                return (
                  <div key={file.id} className={cn("flex items-center gap-3 rounded-md p-2 transition-colors", "hover:bg-accent cursor-pointer", isSelected && "bg-accent/50")} onClick={() => handleFileToggle(file.id)}>
                    <input
                      type="checkbox"
                      checked={isSelected}
                      onChange={() => {}} // Handled by parent div onClick
                      className="h-4 w-4 shrink-0 cursor-pointer"
                    />
                    <File className="h-4 w-4 shrink-0 text-muted-foreground" />
                    <span className="flex-1 truncate text-sm">{file.fileName}</span>
                    <Button variant="ghost" size="sm" disabled={isDeleting} onClick={(e) => handleDelete(file.id, e)} className="h-7 w-7 p-0 hover:bg-destructive hover:text-destructive-foreground">
                      {isDeleting ? <span className="text-xs">...</span> : <Trash2 className="h-3.5 w-3.5" />}
                    </Button>
                  </div>
                );
              })}
            </div>
          </ScrollArea>
        </PopoverContent>
      </Popover>
    </div>
  );
}
