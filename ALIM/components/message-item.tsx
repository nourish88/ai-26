"use client";

import { likeMessageAction, dislikeMessageAction } from "@/app/apps/[identifier]/_actions/chatActions";
import { Message } from "@/app/apps/[identifier]/_types/message";
import { ChatMessageContent } from "@/components/chat-message-content";
import { Avatar, AvatarFallback } from "@/components/ui/avatar";
import { Button } from "@/components/ui/button";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogDescription, DialogFooter, DialogClose } from "@/components/ui/dialog";
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "@/components/ui/dropdown-menu";
import { Textarea } from "@/components/ui/textarea";
import { Label } from "@/components/ui/label";
import { cn } from "@/lib/utils";
import { Document, Packer, Paragraph, TextRun, AlignmentType } from "docx";
import jsPDF from "jspdf";
import { Bot, Copy, Download, ThumbsDown, ThumbsUp, User } from "lucide-react";
import { useSession } from "next-auth/react";
import { memo, useCallback, useState } from "react";
import { toast } from "sonner";

interface MessageItemProps {
  message: Message;
  onCopyMessage: (content: string) => void;
  identifier: string;
}

export const MessageItem = memo(({ message, onCopyMessage, identifier }: MessageItemProps) => {
  const session = useSession();

  const [isLiked, setIsLiked] = useState(false);
  const [isDisliked, setIsDisliked] = useState(false);
  const [isLiking, setIsLiking] = useState(false);
  const [isDisliking, setIsDisliking] = useState(false);

  // State for feedback dialog
  const [isFeedbackDialogOpen, setIsFeedbackDialogOpen] = useState(false);
  const [feedbackType, setFeedbackType] = useState<"like" | "dislike" | null>(null);
  const [comment, setComment] = useState("");

  const handleCopy = useCallback(() => {
    onCopyMessage(message.content);
  }, [message.content, onCopyMessage]);

  const openFeedbackDialog = (type: "like" | "dislike") => {
    if (isLiking || isDisliking) return;
    setFeedbackType(type);
    setIsFeedbackDialogOpen(true);
  };

  const handleSubmitFeedback = useCallback(async () => {
    if (!feedbackType) return;
    console.log("message", message);
    const action = feedbackType === "like" ? likeMessageAction : dislikeMessageAction;
    const setLoading = feedbackType === "like" ? setIsLiking : setIsDisliking;

    setLoading(true);
    try {
      const result = await action(message.id, identifier, comment);
      if (result.success) {
        if (feedbackType === "like") {
          setIsLiked(true);
          setIsDisliked(false);
          toast.success("Mesaj beğenildi!");
        } else {
          setIsDisliked(true);
          setIsLiked(false);
          toast.success("Mesaj beğenilmedi olarak işaretlendi.");
        }
      } else {
        toast.error(result.message);
      }
    } catch (error) {
      toast.error("Bir hata oluştu. Lütfen tekrar deneyin.");
    } finally {
      setLoading(false);
      // Close dialog and reset state
      setIsFeedbackDialogOpen(false);
      setComment("");
      setFeedbackType(null);
    }
  }, [message.id, identifier, comment, feedbackType]);

  const exportAsPDF = useCallback(() => {
    const pdf = new jsPDF({
      orientation: "portrait",
      unit: "mm",
      format: "a4",
    });

    pdf.setFont("helvetica");

    const pageWidth = pdf.internal.pageSize.getWidth();
    const pageHeight = pdf.internal.pageSize.getHeight();
    const margin = 20;
    const maxWidth = pageWidth - 2 * margin;
    let yPosition = 30;

    pdf.setFillColor(37, 99, 235);
    pdf.rect(0, 0, pageWidth, 25, "F");

    pdf.setTextColor(255, 255, 255);
    pdf.setFontSize(16);
    pdf.setFont("helvetica", "bold");
    pdf.text("AI Assistant Response", margin, 15);

    pdf.setFontSize(10);
    pdf.setFont("helvetica", "normal");
    pdf.text(message.timestamp.toLocaleString(), pageWidth - margin - 40, 15);

    yPosition = 40;
    pdf.setTextColor(0, 0, 0);

    const checkPageBreak = (requiredSpace: number) => {
      if (yPosition + requiredSpace > pageHeight - 30) {
        pdf.addPage();
        yPosition = 30;
        return true;
      }
      return false;
    };

    const addFormattedText = (text: string, fontSize: number, fontStyle: string, color: [number, number, number] = [0, 0, 0], indent = 0) => {
      pdf.setFontSize(fontSize);
      pdf.setFont("helvetica", fontStyle as any);
      pdf.setTextColor(color[0], color[1], color[2]);

      const cleanText = text.replace(/[^\x00-\x7F]/g, (char) => {
        const turkishMap: { [key: string]: string } = {
          ğ: "g",
          Ğ: "G",
          ü: "u",
          Ü: "U",
          ş: "s",
          Ş: "S",
          ı: "i",
          İ: "I",
          ö: "o",
          Ö: "O",
          ç: "c",
          Ç: "C",
        };
        return turkishMap[char] || char;
      });

      try {
        const wrappedText = pdf.splitTextToSize(cleanText, maxWidth - indent);
        checkPageBreak(wrappedText.length * (fontSize * 0.35) + 5);
        pdf.text(wrappedText, margin + indent, yPosition);
        yPosition += wrappedText.length * (fontSize * 0.35) + fontSize * 0.2;
      } catch (error) {
        console.log("[v0] PDF text rendering error, using fallback:", error);
        const fallbackText = text.replace(/[^\x00-\x7F]/g, "?");
        const wrappedText = pdf.splitTextToSize(fallbackText, maxWidth - indent);
        checkPageBreak(wrappedText.length * (fontSize * 0.35) + 5);
        pdf.text(wrappedText, margin + indent, yPosition);
        yPosition += wrappedText.length * (fontSize * 0.35) + fontSize * 0.2;
      }
    };

    const lines = message.content.split("\n");
    let inCodeBlock = false;
    let codeBlockContent: string[] = [];

    for (let i = 0; i < lines.length; i++) {
      const line = lines[i];

      if (line.trim() === "") {
        yPosition += 4;
        continue;
      }

      if (line.startsWith("```")) {
        if (inCodeBlock) {
          checkPageBreak(codeBlockContent.length * 4 + 10);

          pdf.setFillColor(248, 249, 250);
          pdf.rect(margin - 2, yPosition - 3, maxWidth + 4, codeBlockContent.length * 4 + 6, "F");

          pdf.setFontSize(9);
          pdf.setFont("helvetica", "normal");
          pdf.setTextColor(51, 51, 51);

          codeBlockContent.forEach((codeLine, index) => {
            const cleanCodeLine = codeLine.replace(/[^\x00-\x7F]/g, (char) => {
              const turkishMap: { [key: string]: string } = {
                ğ: "g",
                Ğ: "G",
                ü: "u",
                Ü: "U",
                ş: "s",
                Ş: "S",
                ı: "i",
                İ: "I",
                ö: "o",
                Ö: "O",
                ç: "c",
                Ç: "C",
              };
              return turkishMap[char] || char;
            });
            try {
              pdf.text(cleanCodeLine, margin + 2, yPosition + index * 4);
            } catch (error) {
              pdf.text(codeLine.replace(/[^\x00-\x7F]/g, "?"), margin + 2, yPosition + index * 4);
            }
          });

          yPosition += codeBlockContent.length * 4 + 10;
          codeBlockContent = [];
          inCodeBlock = false;
        } else {
          inCodeBlock = true;
          const language = line.replace("```", "").trim();
          if (language) {
            addFormattedText(`Code (${language}):`, 10, "bold", [100, 100, 100]);
          }
        }
        continue;
      }

      if (inCodeBlock) {
        codeBlockContent.push(line);
        continue;
      }

      if (line.startsWith("# ")) {
        yPosition += 8;
        addFormattedText(line.replace("# ", ""), 18, "bold", [17, 24, 39]);
        yPosition += 4;
      } else if (line.startsWith("## ")) {
        yPosition += 6;
        addFormattedText(line.replace("## ", ""), 16, "bold", [31, 41, 55]);
        yPosition += 3;
      } else if (line.startsWith("### ")) {
        yPosition += 4;
        addFormattedText(line.replace("### ", ""), 14, "bold", [55, 65, 81]);
        yPosition += 2;
      } else if (line.match(/^\s*[-*+]\s/)) {
        const text = line.replace(/^\s*[-*+]\s/, "");
        const bulletPoint = "• " + text;
        addFormattedText(bulletPoint, 11, "normal", [0, 0, 0], 8);
      } else if (line.match(/^\s*\d+\.\s/)) {
        addFormattedText(line.trim(), 11, "normal", [0, 0, 0], 8);
      } else if (line.startsWith(">")) {
        const quoteText = line.replace(/^>\s*/, "");
        checkPageBreak(15);

        pdf.setFillColor(249, 250, 251);
        pdf.rect(margin - 2, yPosition - 2, maxWidth + 4, 12, "F");

        pdf.setFillColor(59, 130, 246);
        pdf.rect(margin - 2, yPosition - 2, 2, 12, "F");

        addFormattedText(quoteText, 11, "italic", [75, 85, 99], 8);
      } else {
        if (line.includes("**") || line.includes("*")) {
          const parts = line.split(/(\*\*.*?\*\*|\*.*?\*)/g);

          parts.forEach((part) => {
            if (part.startsWith("**") && part.endsWith("**")) {
              const boldText = part.replace(/\*\*/g, "");
              addFormattedText(boldText, 11, "bold");
            } else if (part.startsWith("*") && part.endsWith("*")) {
              const italicText = part.replace(/\*/g, "");
              addFormattedText(italicText, 11, "italic");
            } else if (part.trim()) {
              addFormattedText(part, 11, "normal");
            }
          });
        } else {
          addFormattedText(line, 11, "normal");
        }
      }
    }

    const totalPages = pdf.internal.pages.length - 1;
    for (let i = 1; i <= totalPages; i++) {
      pdf.setPage(i);
      pdf.setFontSize(8);
      pdf.setTextColor(128, 128, 128);
      pdf.text(`Sayfa ${i} / ${totalPages}`, pageWidth - margin - 20, pageHeight - 10);
      pdf.text("AI Asistan tarafından oluşturuldu", margin, pageHeight - 10);
    }

    pdf.save(`ai-yanit-${message.id}.pdf`);
  }, [message]);

  const exportAsWord = useCallback(async () => {
    const userName = session?.data?.user?.name || "Kullanıcı";
    const currentDate = new Date().toLocaleDateString("tr-TR");

    const children: (Paragraph | any)[] = [];

    // MLA Style Header
    children.push(
      new Paragraph({
        alignment: AlignmentType.LEFT,
        children: [
          new TextRun({
            text: userName,
            size: 24,
          }),
        ],
      })
    );
    children.push(
      new Paragraph({
        alignment: AlignmentType.LEFT,
        children: [
          new TextRun({
            text: `Tarih: ${currentDate}`,
            size: 24,
          }),
        ],
        spacing: { after: 400 },
      })
    );

    const lines = message.content.split("\n");

    for (const line of lines) {
      if (line.trim() === "") {
        children.push(new Paragraph({ text: "" }));
        continue;
      }

      if (line.startsWith("### ")) {
        children.push(
          new Paragraph({
            children: [
              new TextRun({
                text: line.replace("### ", ""),
                bold: true,
                size: 28,
                color: "1f2937",
              }),
            ],
            spacing: { before: 300, after: 200 },
          })
        );
      } else if (line.startsWith("## ")) {
        children.push(
          new Paragraph({
            children: [
              new TextRun({
                text: line.replace("## ", ""),
                bold: true,
                size: 32,
                color: "1f2937",
              }),
            ],
            spacing: { before: 400, after: 200 },
          })
        );
      } else if (line.startsWith("# ")) {
        children.push(
          new Paragraph({
            children: [
              new TextRun({
                text: line.replace("# ", ""),
                bold: true,
                size: 36,
                color: "111827",
              }),
            ],
            spacing: { before: 500, after: 300 },
          })
        );
      } else if (line.match(/^\s*[-*]\s/)) {
        children.push(
          new Paragraph({
            children: [
              new TextRun({
                text: line.replace(/^\s*[-*]\s/, "• "),
                size: 22,
              }),
            ],
            spacing: { after: 100 },
            indent: { left: 400 },
          })
        );
      } else if (line.match(/^\s*\d+\.\s/)) {
        children.push(
          new Paragraph({
            children: [
              new TextRun({
                text: line.trim(),
                size: 22,
              }),
            ],
            spacing: { after: 100 },
            indent: { left: 400 },
          })
        );
      } else if (line.includes("**")) {
        const parts = line.split(/(\*\*.*?\*\*)/g);
        const textRuns = parts.map((part) => {
          if (part.startsWith("**") && part.endsWith("**")) {
            return new TextRun({
              text: part.replace(/\*\*/g, ""),
              bold: true,
              size: 22,
            });
          }
          return new TextRun({
            text: part,
            size: 22,
          });
        });

        children.push(
          new Paragraph({
            children: textRuns,
            spacing: { after: 150 },
          })
        );
      } else {
        children.push(
          new Paragraph({
            children: [
              new TextRun({
                text: line,
                size: 22,
              }),
            ],
            spacing: { after: 150 },
          })
        );
      }
    }

    const doc = new Document({
      sections: [
        {
          properties: {},
          children,
        },
      ],
    });

    const blob = await Packer.toBlob(doc);
    const url = URL.createObjectURL(blob);
    const a = document.createElement("a");
    a.href = url;
    a.download = `ai-response-${message.id}.docx`;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
    URL.revokeObjectURL(url);
  }, [message, session]);

  const isWaitingForStream = message.role === "assistant" && message.isStreaming && message.content === "";

  return (
    <>
      <Dialog open={isFeedbackDialogOpen} onOpenChange={setIsFeedbackDialogOpen}>
        <DialogContent className="sm:max-w-[425px] bg-zinc-900 border-zinc-700">
          <DialogHeader>
            <DialogTitle className="text-white">Geri Bildirim Sağla</DialogTitle>
            <DialogDescription>{feedbackType === "like" ? "Bu yanıtı neden beğendiğinizi" : "Bu yanıtta neyin yanlış olduğunu"} bize bildirerek geliştirmemize yardımcı olun.</DialogDescription>
          </DialogHeader>
          <div className="grid gap-4 py-4">
            <div className="grid grid-cols-4 items-center gap-4">
              <Label htmlFor="comment" className="text-right text-zinc-400">
                Yorum
              </Label>
              <Textarea id="comment" value={comment} onChange={(e) => setComment(e.target.value)} className="col-span-3 bg-zinc-800 border-zinc-600 text-white focus:ring-blue-500" placeholder="İsteğe bağlı yorumunuz..." />
            </div>
          </div>
          <DialogFooter>
            <DialogClose asChild>
              <Button type="button" variant="secondary">
                İptal
              </Button>
            </DialogClose>
            <Button type="submit" onClick={handleSubmitFeedback}>
              Gönder
            </Button>
          </DialogFooter>
        </DialogContent>
      </Dialog>

      <div className={cn("flex gap-4 animate-in fade-in slide-in-from-bottom-2 duration-500", message.role === "user" ? "justify-end" : "justify-start")}>
        {message.role === "assistant" && (
          <Avatar className="w-10 h-10 mt-1 flex-shrink-0 ring-2 ring-blue-500/20 shadow-lg">
            <AvatarFallback className="bg-gradient-to-br from-blue-600 to-purple-600 text-white">
              <Bot className="w-5 h-5" />
            </AvatarFallback>
          </Avatar>
        )}
        <div className={cn("max-w-[85%] sm:max-w-[75%] space-y-2 group", message.role === "user" && "items-end flex flex-col")}>
          <div
            className={cn(
              "p-4 rounded-2xl text-sm shadow-xl transition-all duration-300",
              message.role === "user"
                ? "bg-gradient-to-br from-blue-600 to-blue-700 text-white rounded-br-none shadow-blue-500/20 hover:shadow-blue-500/30"
                : "bg-gradient-to-br from-zinc-900/95 to-zinc-800/95 backdrop-blur-sm rounded-bl-none border border-zinc-700/50 hover:border-zinc-600/50 shadow-2xl"
            )}
          >
            {isWaitingForStream ? (
              <div className="flex items-center text-sm font-medium text-zinc-300">
                <svg className="mr-2.5 h-5 w-5 animate-spin text-blue-400" xmlns="http://www.w.org/2000/svg" fill="none" viewBox="0 0 24 24">
                  <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                  <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                </svg>
                <span className="bg-gradient-to-r from-blue-400 to-purple-400 bg-clip-text text-transparent">Cevabını hazırlıyorum...</span>
              </div>
            ) : (
              <>
                <ChatMessageContent content={message.content} />
                {message.isStreaming && <span className="inline-block w-2 h-4 bg-gradient-to-r from-blue-400 to-purple-400 ml-1.5 animate-pulse rounded-sm" />}
              </>
            )}
          </div>
          <div className="flex items-center gap-2 text-xs text-zinc-500 ">
            {message.role === "assistant" && !message.isStreaming && message.content && (
              <div className="flex items-center gap-1 opacity-0 group-hover:opacity-100 transition-all duration-300 translate-y-1 group-hover:translate-y-0">
                <Button variant="ghost" size="icon" className="h-8 w-8 hover:bg-zinc-800 hover:text-white transition-all duration-200 rounded-lg hover:scale-105" onClick={handleCopy}>
                  <Copy className="w-3.5 h-3.5" />
                </Button>
                <Button variant="ghost" size="icon" className={cn("h-8 w-8 hover:bg-zinc-800 transition-all duration-200 rounded-lg hover:scale-105", isLiked && "text-green-500 hover:text-green-400 bg-green-500/10")} onClick={() => openFeedbackDialog("like")} disabled={isLiking || isDisliking}>
                  <ThumbsUp className={cn("w-3.5 h-3.5", isLiking && "animate-pulse")} />
                </Button>
                <Button variant="ghost" size="icon" className={cn("h-8 w-8 hover:bg-zinc-800 transition-all duration-200 rounded-lg hover:scale-105", isDisliked && "text-red-500 hover:text-red-400 bg-red-500/10")} onClick={() => openFeedbackDialog("dislike")} disabled={isLiking || isDisliking}>
                  <ThumbsDown className={cn("w-3.5 h-3.5", isDisliking && "animate-pulse")} />
                </Button>
                <DropdownMenu>
                  <DropdownMenuTrigger asChild>
                    <Button variant="ghost" size="icon" className="h-8 w-8 hover:bg-zinc-800 hover:text-white transition-all duration-200 rounded-lg hover:scale-105">
                      <Download className="w-3.5 h-3.5" />
                    </Button>
                  </DropdownMenuTrigger>
                  <DropdownMenuContent align="end" className=" border-zinc-700">
                    <DropdownMenuItem onClick={exportAsPDF} className="hover:bg-zinc-800 cursor-pointer">
                      PDF Olarak İndir
                    </DropdownMenuItem>
                    <DropdownMenuItem onClick={exportAsWord} className="hover:bg-zinc-800 cursor-pointer">
                      Word Olarak İndir
                    </DropdownMenuItem>
                  </DropdownMenuContent>
                </DropdownMenu>
              </div>
            )}
            <span className="font-medium">{message.timestamp.toLocaleTimeString("tr-TR", { hour: "2-digit", minute: "2-digit" })}</span>
          </div>
        </div>
        {message.role === "user" && (
          <Avatar className="w-10 h-10 mt-1 flex-shrink-0 ring-2 ring-blue-500/20 shadow-lg">
            <AvatarFallback className="bg-gradient-to-br from-zinc-700 to-zinc-800 text-white">
              <User className="w-5 h-5" />
            </AvatarFallback>
          </Avatar>
        )}
      </div>
    </>
  );
});

MessageItem.displayName = "MessageItem";
