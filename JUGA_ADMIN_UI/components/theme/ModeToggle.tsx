"use client";

import * as React from "react";
import { Moon, Sun } from "lucide-react";
import { useTheme } from "next-themes";

import { Button } from "@/components/ui/button";

export function ModeToggle() {
  const [mounted, setMounted] = React.useState(false);
  const { theme, setTheme } = useTheme(); // ✅ Destructure both theme and setTheme

  React.useEffect(() => {
    setMounted(true);
  }, []);

  // Prevent hydration error
  if (!mounted) {
    return (
      <Button variant="ghost" size="icon" className="h-9 w-9 hover:bg-muted/50">
        <div className="h-4 w-4" />
        <span className="sr-only">Toggle theme</span>
      </Button>
    );
  }

  return (
    <Button
      variant="ghost"
      size="icon"
      onClick={() => setTheme(theme === "dark" ? "light" : "dark")} // ✅ Use arrow function without params
      className="h-9 w-9 hover:bg-muted/50"
    >
      {theme === "dark" ? <Moon className="h-4 w-4" /> : <Sun className="h-4 w-4" />}
      <span className="sr-only">Toggle theme</span>
    </Button>
  );
}
