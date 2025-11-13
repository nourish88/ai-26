// components/ui/DataTable.tsx
"use client";

import React from "react";
import { useReactTable, getCoreRowModel, getSortedRowModel, getFilteredRowModel, flexRender, ColumnDef, SortingState, ColumnFiltersState } from "@tanstack/react-table";
import { ArrowUpDown, Loader2, ChevronLeft, ChevronRight, ChevronsLeft, ChevronsRight, Search, ArrowUp, ArrowDown } from "lucide-react";
import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { cn } from "@/lib/utils";

interface PaginationData {
  items: any[];
  size: number;
  index: number;
  count: number;
  pages: number;
  hasNext: boolean;
  hasPrevious: boolean;
}

interface DataTableProps<T> {
  data: T[];
  columns: ColumnDef<T, any>[];
  title: string;
  description: string;
  actionButton?: React.ReactNode;
  filterColumn?: string;
  filterPlaceholder?: string;
  isLoading?: boolean;
  paginationData?: PaginationData;
  onPaginationChange?: (pageIndex: number, pageSize: number) => void;
}

export function DataTable<T>({ data, columns, title, description, actionButton, filterColumn, filterPlaceholder = "Filter...", isLoading, paginationData, onPaginationChange }: DataTableProps<T>) {
  const [sorting, setSorting] = React.useState<SortingState>([]);
  const [columnFilters, setColumnFilters] = React.useState<ColumnFiltersState>([]);

  const currentPage = paginationData?.index ?? 0;
  const pageSize = paginationData?.size ?? 10;
  const totalCount = paginationData?.count ?? 0;
  const totalPages = paginationData?.pages ?? 1;
  const hasNext = paginationData?.hasNext ?? false;
  const hasPrevious = paginationData?.hasPrevious ?? false;

  const table = useReactTable({
    data,
    columns,
    state: {
      sorting,
      columnFilters,
    },
    onSortingChange: setSorting,
    onColumnFiltersChange: setColumnFilters,
    getCoreRowModel: getCoreRowModel(),
    getSortedRowModel: getSortedRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    manualPagination: true,
    pageCount: totalPages,
  });

  const handlePageChange = (newPageIndex: number) => {
    if (onPaginationChange) {
      onPaginationChange(newPageIndex, pageSize);
    }
  };

  const handlePageSizeChange = (newPageSize: number) => {
    if (onPaginationChange) {
      onPaginationChange(0, newPageSize);
    }
  };

  const startRecord = totalCount > 0 ? currentPage * pageSize + 1 : 0;
  const endRecord = Math.min((currentPage + 1) * pageSize, totalCount);

  return (
    <div className="relative bg-gradient-to-br from-card via-card to-card/50 rounded-xl border-2 border-border/50 shadow-xl backdrop-blur-sm overflow-hidden">
      {/* Decorative gradient overlay */}
      <div className="absolute inset-0 bg-gradient-to-br from-primary/5 via-transparent to-purple-500/5 pointer-events-none" />

      {/* Loading Overlay with blur */}
      {isLoading && (
        <div className="absolute inset-0 z-50 flex items-center justify-center bg-background/80 backdrop-blur-md rounded-xl">
          <div className="flex flex-col items-center gap-3">
            <div className="relative">
              <Loader2 className="h-8 w-8 animate-spin text-primary" />
              <div className="absolute inset-0 h-8 w-8 animate-ping rounded-full bg-primary/20" />
            </div>
            <span className="text-sm font-medium text-muted-foreground animate-pulse">Yükleniyor...</span>
          </div>
        </div>
      )}

      {/* Header with gradient */}
      <div className="relative p-6 border-b border-border/50 bg-gradient-to-r from-muted/30 to-muted/10">
        <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div className="space-y-1">
            <h3 className="text-xl font-bold bg-gradient-to-r from-foreground to-foreground/70 bg-clip-text text-transparent">{title}</h3>
            <p className="text-sm text-muted-foreground">{description}</p>
            {totalCount > 0 && (
              <p className="text-xs text-muted-foreground/70">
                Toplam <span className="font-semibold text-primary">{totalCount}</span> kayıt
              </p>
            )}
          </div>
          {actionButton && <div className="shrink-0">{actionButton}</div>}
        </div>
      </div>

      {/* Filter Section with icon */}
      {filterColumn && (
        <div className="p-4 border-b border-border/30 bg-muted/20">
          <div className="relative max-w-sm">
            <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder={filterPlaceholder}
              value={(table.getColumn(filterColumn)?.getFilterValue() as string) ?? ""}
              onChange={(event) => table.getColumn(filterColumn)?.setFilterValue(event.target.value)}
              className="pl-9 bg-background/50 border-border/50 focus:border-primary transition-all"
            />
          </div>
        </div>
      )}

      {/* Table with hover effects */}
      <div className="overflow-x-auto">
        <table className="w-full">
          <thead className="bg-gradient-to-r from-muted/50 to-muted/30 sticky top-0 z-10">
            {table.getHeaderGroups().map((headerGroup) => (
              <tr key={headerGroup.id}>
                {headerGroup.headers.map((header) => {
                  const isSorted = header.column.getIsSorted();
                  return (
                    <th key={header.id} className="px-6 py-4 text-left text-xs font-semibold text-muted-foreground uppercase tracking-wider border-b border-border/30">
                      {header.isPlaceholder ? null : (
                        <div className={cn("flex items-center gap-2 group", header.column.getCanSort() && "cursor-pointer select-none hover:text-foreground transition-colors")} onClick={header.column.getToggleSortingHandler()}>
                          <span className="truncate">{flexRender(header.column.columnDef.header, header.getContext())}</span>
                          {header.column.getCanSort() && (
                            <div className="flex flex-col">{isSorted === "asc" ? <ArrowUp className="h-3 w-3 text-primary" /> : isSorted === "desc" ? <ArrowDown className="h-3 w-3 text-primary" /> : <ArrowUpDown className="h-3 w-3 opacity-0 group-hover:opacity-50 transition-opacity" />}</div>
                          )}
                        </div>
                      )}
                    </th>
                  );
                })}
              </tr>
            ))}
          </thead>

          <tbody className="divide-y divide-border/30">
            {table.getRowModel().rows?.length
              ? table.getRowModel().rows.map((row, idx) => (
                  <tr key={row.id} className={cn("transition-all duration-200", "hover:bg-muted/40 hover:shadow-sm", idx % 2 === 0 ? "bg-background/50" : "bg-background/30")}>
                    {row.getVisibleCells().map((cell) => (
                      <td key={cell.id} className="px-6 py-4 whitespace-nowrap text-sm">
                        {flexRender(cell.column.columnDef.cell, cell.getContext())}
                      </td>
                    ))}
                  </tr>
                ))
              : !isLoading && (
                  <tr>
                    <td colSpan={columns.length} className="h-32 text-center">
                      <div className="flex flex-col items-center justify-center gap-2 text-muted-foreground">
                        <Search className="h-8 w-8 opacity-20" />
                        <p className="text-sm font-medium">Sonuç bulunamadı</p>
                        <p className="text-xs">Farklı filtreler deneyin</p>
                      </div>
                    </td>
                  </tr>
                )}
          </tbody>
        </table>
      </div>

      {/* Enhanced Pagination Footer */}
      <div className="p-4 border-t border-border/50 bg-gradient-to-r from-muted/20 to-muted/10">
        <div className="flex flex-col sm:flex-row items-center justify-between gap-4">
          {/* Left: Info and page size */}
          <div className="flex flex-col sm:flex-row items-center gap-4 text-sm">
            <div className="flex items-center gap-2 text-muted-foreground">
              {totalCount > 0 ? (
                <>
                  <span className="font-medium text-foreground">{startRecord}</span>
                  <span>-</span>
                  <span className="font-medium text-foreground">{endRecord}</span>
                  <span>of</span>
                  <span className="font-medium text-primary">{totalCount}</span>
                </>
              ) : (
                <span>Kayıt bulunamadı</span>
              )}
            </div>

            <div className="flex items-center gap-2">
              <span className="text-muted-foreground whitespace-nowrap">Sayfa başına:</span>
              <Select value={pageSize.toString()} onValueChange={(value) => handlePageSizeChange(Number(value))} disabled={isLoading}>
                <SelectTrigger className="w-20 h-9 bg-background/50">
                  <SelectValue />
                </SelectTrigger>
                <SelectContent>
                  {[10, 20, 30, 50, 100].map((size) => (
                    <SelectItem key={size} value={size.toString()}>
                      {size}
                    </SelectItem>
                  ))}
                </SelectContent>
              </Select>
            </div>
          </div>

          {/* Right: Pagination controls */}
          <div className="flex items-center gap-1">
            <Button variant="outline" size="icon" onClick={() => handlePageChange(0)} disabled={!hasPrevious || isLoading} className="h-9 w-9 hover:bg-primary/10 hover:text-primary hover:border-primary/50 transition-all" title="İlk sayfa">
              <ChevronsLeft className="h-4 w-4" />
            </Button>

            <Button variant="outline" size="icon" onClick={() => handlePageChange(currentPage - 1)} disabled={!hasPrevious || isLoading} className="h-9 w-9 hover:bg-primary/10 hover:text-primary hover:border-primary/50 transition-all" title="Önceki sayfa">
              <ChevronLeft className="h-4 w-4" />
            </Button>

            <div className="flex items-center gap-1 px-4 py-2 bg-muted/30 rounded-md border border-border/30">
              <span className="text-sm font-medium text-primary">{currentPage + 1}</span>
              <span className="text-sm text-muted-foreground">/</span>
              <span className="text-sm font-medium text-muted-foreground">{totalPages}</span>
            </div>

            <Button variant="outline" size="icon" onClick={() => handlePageChange(currentPage + 1)} disabled={!hasNext || isLoading} className="h-9 w-9 hover:bg-primary/10 hover:text-primary hover:border-primary/50 transition-all" title="Sonraki sayfa">
              <ChevronRight className="h-4 w-4" />
            </Button>

            <Button variant="outline" size="icon" onClick={() => handlePageChange(totalPages - 1)} disabled={!hasNext || isLoading} className="h-9 w-9 hover:bg-primary/10 hover:text-primary hover:border-primary/50 transition-all" title="Son sayfa">
              <ChevronsRight className="h-4 w-4" />
            </Button>
          </div>
        </div>
      </div>
    </div>
  );
}
