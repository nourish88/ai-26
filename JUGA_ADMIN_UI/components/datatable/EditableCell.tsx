"use client";

import * as React from "react";
import { Input } from "@/components/ui/input";
import { cn } from "@/lib/utils";

interface EditableCellProps {
  value?: string;
  defaultValue?: string | number;
  onChange: (value: string) => void;
  placeholder?: string;
  className?: string;
  type?: "text" | "number" | "email";
}

export function EditableCell({ value, defaultValue, onChange, placeholder, className, type = "text" }: EditableCellProps) {
  const [internalValue, setInternalValue] = React.useState(defaultValue || value || "");

  React.useEffect(() => {
    if (defaultValue !== undefined) {
      setInternalValue(defaultValue);
    }
  }, [defaultValue]);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const newValue = e.target.value;
    setInternalValue(newValue);
    onChange(newValue);
  };

  return <Input type={type} value={internalValue} onChange={handleChange} placeholder={placeholder} className={cn("", className)} />;
}
