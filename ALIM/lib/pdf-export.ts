import { Message } from "@/app/genel-maksatli-sohbet-ajani/_types/message";
import { jsPDF } from "jspdf";

export function exportToPDF(messages: Message[]) {
  const pdf = new jsPDF();
  pdf.setFont("helvetica");
  pdf.setFontSize(16);
  pdf.text("Chat Conversation Export", 20, 20);

  pdf.setFontSize(10);
  pdf.text(`Exported on: ${new Date().toLocaleString("tr-TR")}`, 20, 30);

  let y = 50;
  const pageHeight = pdf.internal.pageSize.height;
  const margin = 20;
  const maxWidth = pdf.internal.pageSize.width - 2 * margin;

  const exportMessages = messages.filter((m) => m.id !== "initial-welcome");

  for (const m of exportMessages) {
    if (y > pageHeight - 40) {
      pdf.addPage();
      y = 20;
    }
    pdf.setFontSize(12).setFont("helvetica", "bold");
    pdf.text(`${m.role === "user" ? "User" : "Assistant"} (${m.timestamp.toLocaleString("tr-TR")}):`, margin, y);
    y += 10;

    pdf.setFontSize(10).setFont("helvetica", "normal");
    const clean = m.content
      .replace(/```[\s\S]*?```/g, "[Code Block]")
      .replace(/`([^`]+)`/g, "$1")
      .replace(/\*\*(.*?)\*\*/g, "$1")
      .replace(/\*(.*?)\*/g, "$1")
      .replace(/#{1,6}\s/g, "");
    const lines = pdf.splitTextToSize(clean, maxWidth);

    for (const line of lines) {
      if (y > pageHeight - 20) {
        pdf.addPage();
        y = 20;
      }
      pdf.text(line, margin, y);
      y += 6;
    }
    y += 10;
  }

  const filename = `chat-export-${new Date().toISOString().split("T")[0]}.pdf`;
  pdf.save(filename);
}
