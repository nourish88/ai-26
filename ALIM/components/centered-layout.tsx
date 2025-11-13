import type React from "react";
interface CenteredLayoutProps {
  children: React.ReactNode;
}

export default function CenteredLayout({ children }: CenteredLayoutProps) {
  return <div className="flex flex-grow flex-col items-center justify-center px-6">{children}</div>;
}
