"use client";
import { useState, useRef, useCallback, useMemo } from "react";
import type React from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardDescription, CardHeader, CardTitle } from "@/components/ui/card";
import { Alert, AlertDescription } from "@/components/ui/alert";
import { Loader2, Upload, CheckCircle, XCircle, ImageIcon, Sparkles, X, RefreshCw } from "lucide-react";
import { Separator } from "@/components/ui/separator";
import Image from "next/image";

// API'nin analiz sonucu için bir tip tanımı
interface AnalysisCheck {
  passed: boolean;
  message: string;
  category: string;
  details: any;
}

interface AnalysisResult {
  overall_compliance: boolean;
  summary: string;
  checks: Record<string, AnalysisCheck>;
  annotated_image: string; // Base64 veri URI'si
}

export default function ModernImageUpload() {
  const [file, setFile] = useState<File | null>(null);
  const [uploading, setUploading] = useState(false);
  const [message, setMessage] = useState("");
  const [analysisResult, setAnalysisResult] = useState<AnalysisResult | null>(null);
  const [messageType, setMessageType] = useState<"success" | "error" | "info">("info");
  const [isDragOver, setIsDragOver] = useState(false);
  const [imagePreview, setImagePreview] = useState<string | null>(null);
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileChange = useCallback((selectedFile: File) => {
    if (!selectedFile.type.startsWith("image/")) {
      setMessage("Lütfen geçerli bir resim dosyası (PNG, JPG, vb.) yükleyin.");
      setMessageType("error");
      return;
    }
    setFile(selectedFile);
    setMessage("");
    setAnalysisResult(null);

    const reader = new FileReader();
    reader.onload = (e) => {
      setImagePreview(e.target?.result as string);
    };
    reader.readAsDataURL(selectedFile);
  }, []);

  const handleInputChange = (event: React.ChangeEvent<HTMLInputElement>) => {
    const selectedFile = event.target.files?.[0];
    if (selectedFile) {
      handleFileChange(selectedFile);
    }
  };

  const handleDrop = useCallback(
    (event: React.DragEvent<HTMLDivElement>) => {
      event.preventDefault();
      setIsDragOver(false);
      const droppedFile = event.dataTransfer.files[0];
      if (droppedFile) {
        handleFileChange(droppedFile);
      }
    },
    [handleFileChange]
  );

  const handleDragOver = useCallback((event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();
    setIsDragOver(true);
  }, []);

  const handleDragLeave = useCallback((event: React.DragEvent<HTMLDivElement>) => {
    event.preventDefault();
    setIsDragOver(false);
  }, []);

  const handleUpload = async () => {
    if (!file) {
      setMessage("Lütfen önce bir dosya seçin.");
      setMessageType("error");
      return;
    }

    setUploading(true);
    setMessage("Yüz özellikleri analiz ediliyor...");
    setMessageType("info");
    setAnalysisResult(null);

    const formData = new FormData();
    formData.append("file", file);

    try {
      const response = await fetch("/api/analyze-face", {
        method: "POST",
        body: formData,
      });

      if (!response.ok) {
        const errorData = await response.json().catch(() => ({ error: "Bilinmeyen sunucu hatası" }));
        throw new Error(errorData.error || `İstek başarısız oldu: ${response.status}`);
      }

      const data: AnalysisResult = await response.json();
      setAnalysisResult(data);
      setMessage(""); // Yükleme mesajını temizle
    } catch (error: any) {
      console.error("Dosya yükleme sırasında hata:", error);
      setMessage(`Analiz başarısız oldu: ${error.message}`);
      setMessageType("error");
    } finally {
      setUploading(false);
    }
  };

  // Kontrolleri yapılandırılmış gösterim için kategoriye göre grupla
  const groupedChecks = useMemo(() => {
    if (!analysisResult?.checks) return {};
    return Object.values(analysisResult.checks).reduce((acc, check) => {
      const category = check.category || "Genel";
      if (!acc[category]) {
        acc[category] = [];
      }
      acc[category].push(check);
      return acc;
    }, {} as Record<string, AnalysisCheck[]>);
  }, [analysisResult]);

  const clearFile = () => {
    setFile(null);
    setImagePreview(null);
    setMessage("");
    setAnalysisResult(null);
    if (fileInputRef.current) {
      fileInputRef.current.value = "";
    }
  };

  return (
    <>
      {/* === DEĞİŞİKLİK: Sonuç varken daha geniş bir alan kullanmak için dinamik genişlik === */}
      <div className={`max-w-screen mx-auto space-y-6 relative z-10 transition-all duration-500 ease-in-out ${analysisResult ? "max-w-6xl" : "max-w-2xl"}`}>
        <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }} transition={{ duration: 0.6 }}>
          <Card className="bg-transparent backdrop-blur-md shadow-2xl w-full">
            <CardHeader className="text-center">
              <motion.div initial={{ scale: 0.8 }} animate={{ scale: 1 }} transition={{ duration: 0.5, delay: 0.2 }} className="flex items-center justify-center gap-3 mb-2">
                <div className="p-2 rounded-full bg-primary/20">
                  <Sparkles className="h-6 w-6 text-primary" />
                </div>
                <CardTitle className="text-2xl font-bold tracking-tight text-foreground">Yapay Zeka Destekli Fotoğraf Analizi</CardTitle>
              </motion.div>
              <CardDescription>Yapay Zeka kullanılarak fotoğrafları analiz etmek için fotoğraf yükleyin.</CardDescription>
            </CardHeader>

            <CardContent className="space-y-6">
              {!analysisResult && (
                <AnimatePresence>
                  <motion.div initial={{ opacity: 1 }} exit={{ opacity: 0, height: 0, marginBottom: 0, marginTop: 0 }} className="space-y-6 overflow-hidden">
                    <motion.div
                      className={`relative border-2 border-dashed rounded-xl p-8 transition-all duration-300 ${isDragOver ? "border-primary bg-primary/20 scale-105" : "border-border hover:border-primary/50 hover:bg-muted"}`}
                      onDrop={handleDrop}
                      onDragOver={handleDragOver}
                      onDragLeave={handleDragLeave}
                      onClick={() => !file && fileInputRef.current?.click()}
                    >
                      <input ref={fileInputRef} type="file" accept="image/*" onChange={handleInputChange} disabled={uploading} className="hidden" />
                      <div className="flex flex-col items-center justify-center space-y-4 text-center cursor-pointer">
                        <motion.div animate={{ scale: isDragOver ? 1.2 : 1 }} transition={{ duration: 0.3 }}>
                          <Upload className="h-10 w-10 text-muted-foreground" />
                        </motion.div>
                        <p className="text-lg font-medium text-foreground">Resminizi buraya sürükleyin</p>
                        <p className="text-sm text-muted-foreground">
                          veya <span className="text-primary font-semibold">göz atmak için tıklayın</span>
                        </p>
                      </div>
                    </motion.div>

                    {imagePreview && (
                      <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }}>
                        <Card className="border-primary/20 overflow-hidden">
                          <CardContent className="p-4">
                            <div className="flex items-center justify-between mb-3">
                              <div className="flex items-center gap-2">
                                <ImageIcon className="h-4 w-4 text-primary" />
                                <span className="text-sm font-medium text-foreground">Görsel Önizleme</span>
                              </div>
                              <Button variant="ghost" size="icon" onClick={clearFile} className="h-7 w-7 rounded-full hover:bg-destructive/20">
                                <X className="h-4 w-4 text-muted-foreground hover:text-destructive" />
                              </Button>
                            </div>
                            <img src={imagePreview} alt="Önizleme" className="w-full h-48 object-cover rounded-md" />
                            <p className="text-center text-sm text-muted-foreground mt-2 truncate">{file?.name}</p>
                          </CardContent>
                        </Card>
                      </motion.div>
                    )}

                    <motion.div whileHover={{ scale: 1.02 }} whileTap={{ scale: 0.98 }}>
                      <Button onClick={handleUpload} disabled={!file || uploading} className="w-full h-12 text-base font-semibold">
                        {uploading ? <Loader2 className="mr-2 h-5 w-5 animate-spin" /> : <Sparkles className="mr-2 h-5 w-5" />}
                        Resmi Analiz Et
                      </Button>
                    </motion.div>
                  </motion.div>
                </AnimatePresence>
              )}

              <AnimatePresence>
                {message && (
                  <motion.div initial={{ opacity: 0, y: 10 }} animate={{ opacity: 1, y: 0 }} exit={{ opacity: 0, y: -10 }}>
                    <Alert variant={messageType === "error" ? "destructive" : "default"} className="bg-opacity-50">
                      {messageType === "info" && <Loader2 className="h-4 w-4 animate-spin" />}
                      <AlertDescription>{message}</AlertDescription>
                    </Alert>
                  </motion.div>
                )}
              </AnimatePresence>

              <AnimatePresence>
                {analysisResult && (
                  <motion.div initial={{ opacity: 0, y: 20 }} animate={{ opacity: 1, y: 0 }}>
                    <div className="space-y-6">
                      <div className="text-center space-y-2">
                        <h3 className="text-xl font-bold tracking-tight text-foreground">Analiz Sonucu</h3>
                        <div className={`inline-flex items-center gap-2 rounded-full px-4 py-1 text-sm font-medium ${analysisResult.overall_compliance ? "bg-green-100 text-green-800 dark:bg-green-900/50 dark:text-green-300" : "bg-red-100 text-red-800 dark:bg-red-900/50 dark:text-red-300"}`}>
                          {analysisResult.overall_compliance ? <CheckCircle className="h-4 w-4" /> : <XCircle className="h-4 w-4" />}
                          Uyumluluk Kontrolü: {analysisResult.overall_compliance ? "Uyumlu" : "Uyumsuz"}
                        </div>
                        <p className="text-muted-foreground text-sm">{analysisResult.summary}</p>
                      </div>
                      <Separator />
                      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                        <div className="space-y-3">
                          <h4 className="font-semibold text-foreground">Analiz Edilmiş Fotoğraf</h4>
                          <div className="relative w-full aspect-square rounded-lg border overflow-hidden">
                            <Image src={analysisResult.annotated_image} alt="Analiz edilmiş fotoğraf sonucu" fill className="object-cover" />
                          </div>
                        </div>
                        <div className="space-y-4">
                          <h4 className="font-semibold text-foreground">Detaylar</h4>
                          <div className="space-y-4 max-h-96 md:max-h-[calc((100vw-3rem-1.5rem)/2)] lg:max-h-[calc((1024px*5/6-3rem-1.5rem)/2)]  pr-2">
                            {Object.entries(groupedChecks).map(([category, checks]) => (
                              <div key={category}>
                                <p className="text-sm font-semibold text-muted-foreground mb-2">{category}</p>
                                <div className="space-y-2">
                                  {checks.map((check, index) => (
                                    <div key={index} className="flex items-start gap-3">
                                      {check.passed ? <CheckCircle className="h-5 w-5 text-green-500 mt-0.5 flex-shrink-0" /> : <XCircle className="h-5 w-5 text-red-500 mt-0.5 flex-shrink-0" />}
                                      <p className="text-sm text-foreground">{check.message}</p>
                                    </div>
                                  ))}
                                </div>
                              </div>
                            ))}
                          </div>
                        </div>
                      </div>
                      <Button onClick={clearFile} className="w-full">
                        <RefreshCw className="mr-2 h-4 w-4" />
                        Başka Bir Resim Analiz Et
                      </Button>
                    </div>
                  </motion.div>
                )}
              </AnimatePresence>
            </CardContent>
          </Card>
        </motion.div>
      </div>
    </>
  );
}
