// components/ChatMessageContent.tsx
"use client";

import type React from "react";
import { useMemo, useCallback, useState, memo } from "react";
import ReactMarkdown from "react-markdown";
import { Copy, Check, ExternalLink, Download } from "lucide-react";
import remarkGfm from "remark-gfm";

interface ChatMessageContentProps {
  content: string;
}

const CodeBlock = memo(({ className, children }: { className?: string; children: React.ReactNode }) => {
  const [isCopied, setIsCopied] = useState(false);

  const match = /language-(\w+)/.exec(className || "");
  const language = match ? match[1] : "text";
  const codeString = String(children).replace(/\n$/, "");

  const handleCopy = useCallback(() => {
    navigator.clipboard.writeText(codeString).then(() => {
      setIsCopied(true);
      setTimeout(() => setIsCopied(false), 2500);
    });
  }, [codeString]);

  const handleDownload = useCallback(() => {
    const blob = new Blob([codeString], { type: "text/plain" });
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `code-snippet.${language}.txt`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }, [codeString, language]);

  // Split code into lines for line numbers
  const lines = codeString.split("\n");

  return (
    <div className="relative my-4 rounded-xl shadow-xl bg-gradient-to-br from-zinc-900 via-zinc-900 to-zinc-800 backdrop-blur border border-zinc-700/50 overflow-hidden group/code">
      <div className="absolute top-0 left-0 right-0 h-[2px] bg-gradient-to-r from-blue-500/50 via-purple-500/50 to-blue-500/50" />

      <div className="flex items-center justify-between px-4 py-2.5 border-b border-zinc-700/50 bg-zinc-900/50">
        <span className="text-xs font-sans font-semibold text-zinc-300 uppercase tracking-wider">{language}</span>
        <div className="flex items-center gap-1">
          <button onClick={handleDownload} className="p-1.5 rounded-lg text-zinc-400 hover:text-white hover:bg-zinc-700/70 transition-all duration-200 hover:scale-105" title="Download code snippet">
            <Download className="w-4 h-4" />
          </button>
          <button onClick={handleCopy} className="p-1.5 rounded-lg text-zinc-400 hover:text-white hover:bg-zinc-700/70 transition-all duration-200 hover:scale-105" title="Copy code">
            {isCopied ? <Check className="w-4 h-4 text-emerald-400" /> : <Copy className="w-4 h-4" />}
          </button>
        </div>
      </div>

      <div className="overflow-x-auto">
        <div className="flex font-mono text-sm">
          {/* Line numbers */}
          <div className="select-none py-5 pl-4 pr-3 text-zinc-600 text-right border-r border-zinc-700/50 bg-zinc-900/30">
            {lines.map((_, i) => (
              <div key={i} className="leading-6">
                {i + 1}
              </div>
            ))}
          </div>

          {/* Code content */}
          <pre className="flex-1 py-5 px-4 m-0 bg-transparent">
            <code className="text-zinc-200 leading-6 block">{codeString}</code>
          </pre>
        </div>
      </div>
    </div>
  );
});
CodeBlock.displayName = "CodeBlock";

const LinkRenderer = memo(({ href, children }: { href?: string; children: React.ReactNode }) => {
  if (!href) return <span>{children}</span>;

  const isExternal = typeof window !== "undefined" && href.startsWith("http") && !href.includes(window.location.hostname);

  return (
    <a href={href} target={isExternal ? "_blank" : undefined} rel={isExternal ? "noopener noreferrer" : undefined} className="text-blue-400 hover:text-blue-300 underline decoration-blue-400/40 hover:decoration-blue-300/60 transition-all duration-200 underline-offset-2 hover:underline-offset-4">
      {children}
      {isExternal && <ExternalLink className="w-3.5 h-3.5 inline-block ml-1 opacity-70" />}
    </a>
  );
});
LinkRenderer.displayName = "LinkRenderer";

export const ChatMessageContent: React.FC<ChatMessageContentProps> = memo(({ content }) => {
  const components = useMemo(
    () => ({
      h1: ({ children }: any) => <h1 className="text-2xl font-bold mt-4 mb-2 pb-2 border-b border-zinc-700/70 text-white">{children}</h1>,
      h2: ({ children }: any) => <h2 className="text-xl font-bold mt-3 mb-2 text-zinc-100">{children}</h2>,
      h3: ({ children }: any) => <h3 className="text-lg font-semibold mt-3 mb-1.5 text-zinc-200">{children}</h3>,
      p: ({ children }: any) => <p className="leading-7 my-2 text-zinc-300">{children}</p>,

      ul: ({ children }: any) => <ul className="list-disc list-outside my-1.5 pl-6 space-y-0.5">{children}</ul>,
      ol: ({ children }: any) => <ol className="list-decimal list-outside my-1.5 pl-6 space-y-0.5">{children}</ol>,
      li: ({ children }: any) => <li className="pl-2 text-zinc-300 leading-relaxed">{children}</li>,

      blockquote: ({ children }: any) => (
        <blockquote className="relative border-l-4 border-blue-500/50 bg-zinc-800/30 rounded-r-lg pl-4 pr-4 py-2.5 my-2 text-zinc-300 italic">
          <div className="absolute left-0 top-0 bottom-0 w-1 bg-gradient-to-b from-blue-500/50 to-purple-500/50" />
          {children}
        </blockquote>
      ),

      code({ node, ref, className, children, ...rest }: any) {
        const match = /language-(\w+)/.exec(className || "");
        return match ? (
          <CodeBlock className={className}>{children}</CodeBlock>
        ) : (
          <code className="bg-zinc-800/90 border border-zinc-700/60 rounded-md px-2 py-0.5 font-mono text-sm text-blue-300 shadow-sm" ref={ref as React.Ref<HTMLElement>} {...rest}>
            {children}
          </code>
        );
      },

      a: LinkRenderer,

      table: ({ children }: any) => (
        <div className="overflow-x-auto my-3 rounded-xl border border-zinc-700/70 shadow-lg">
          <table className="min-w-full divide-y divide-zinc-700/70">{children}</table>
        </div>
      ),
      thead: ({ children }: any) => <thead className="bg-zinc-900/70">{children}</thead>,
      th: ({ children }: any) => <th className="px-4 py-3 text-left text-xs font-semibold text-zinc-200 uppercase tracking-wider">{children}</th>,
      tbody: ({ children }: any) => <tbody className="bg-zinc-800/30 divide-y divide-zinc-700/50">{children}</tbody>,
      td: ({ children }: any) => <td className="px-4 py-3 text-sm text-zinc-300">{children}</td>,
      tr: ({ children }: any) => <tr className="hover:bg-zinc-700/20 transition-colors duration-150">{children}</tr>,
    }),
    []
  );

  return (
    <div className="prose prose-sm prose-invert max-w-none prose-p:text-zinc-300 prose-li:text-zinc-300 prose-headings:text-white prose-strong:text-zinc-100 prose-strong:font-semibold">
      <ReactMarkdown components={components as any} remarkPlugins={[remarkGfm]}>
        {content}
      </ReactMarkdown>
    </div>
  );
});
ChatMessageContent.displayName = "ChatMessageContent";
