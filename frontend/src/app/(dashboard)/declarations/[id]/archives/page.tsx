"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { archiveService } from "@/services/archiveService";
import { declarationService } from "@/services/declarationService";
import { Archive, DeclarationDetail } from "@/lib/types";
import { PageHeader } from "@/components/shared/PageHeader";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { EmptyState } from "@/components/shared/EmptyState";
import { formatDate, formatFileSize } from "@/lib/utils";
import { ArrowLeft, Upload, Send, FileIcon } from "lucide-react";

export default function ArchivesPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;

  const [declaration, setDeclaration] = useState<DeclarationDetail | null>(null);
  const [archives, setArchives] = useState<Archive[]>([]);
  const [loading, setLoading] = useState(true);
  const [uploading, setUploading] = useState(false);

  useEffect(() => {
    const load = async () => {
      try {
        const [decl, archs] = await Promise.all([
          declarationService.get(id),
          archiveService.list(id),
        ]);
        setDeclaration(decl);
        setArchives(archs);
      } catch {
        console.error("Yuklenemedi");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [id]);

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file) return;

    setUploading(true);
    try {
      // Dosyayi Base64'e cevir
      const base64 = await new Promise<string>((resolve) => {
        const reader = new FileReader();
        reader.onload = () => {
          const result = reader.result as string;
          resolve(result.split(",")[1]); // data:...;base64, kismini cikar
        };
        reader.readAsDataURL(file);
      });

      const result = await archiveService.upload(id, {
        fileName: file.name,
        base64Data: base64,
        contentType: file.type,
        documentType: "Belge",
      });

      if (result.success && result.data) {
        setArchives((prev) => [result.data!, ...prev]);
        alert("Dosya yüklendi!");
      }
    } catch (err: any) {
      alert(err.response?.data?.message || "Yükleme hatası");
    } finally {
      setUploading(false);
      e.target.value = "";
    }
  };

  const handleSendToEvrim = async (archiveId: string) => {
    try {
      const result = await archiveService.sendToEvrim(id, archiveId);
      if (result.success) {
        alert("Arşiv Evrim'e gönderim kuyruğuna eklendi!");
        // Listeyi yenile
        const updated = await archiveService.list(id);
        setArchives(updated);
      }
    } catch (err: any) {
      alert(err.response?.data?.message || "Gönderim hatası");
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <>
      <PageHeader
        title="Arşiv Yönetimi"
        description={declaration?.fileNumber || ""}
      >
        <button
          onClick={() => router.back()}
          className="flex items-center gap-2 px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50"
        >
          <ArrowLeft className="h-4 w-4" />
          Geri
        </button>
      </PageHeader>

      {/* Dosya Yükleme */}
      <div className="card p-6 mb-6">
        <h2 className="text-lg font-semibold mb-4">Dosya Yükle</h2>
        <label className="flex flex-col items-center justify-center w-full h-32 border-2 border-dashed border-gray-300 rounded-lg cursor-pointer hover:bg-blue-50/50 hover:border-blue-300 transition-colors">
          <div className="flex flex-col items-center">
            <Upload className="h-8 w-8 text-gray-400 mb-2" />
            <p className="text-sm text-gray-500">
              {uploading ? "Yükleniyor..." : "Dosya seçmek için tıklayın"}
            </p>
            <p className="text-xs text-gray-400 mt-1">PDF, Excel, Word, resim</p>
          </div>
          <input
            type="file"
            className="hidden"
            onChange={handleFileUpload}
            disabled={uploading}
          />
        </label>
      </div>

      {/* Arşiv Listesi */}
      <div className="card overflow-hidden">
        {archives.length === 0 ? (
          <EmptyState title="Henüz arşiv yok" description="Yukarıdan dosya yükleyebilirsiniz." />
        ) : (
          <div className="divide-y divide-gray-100">
            {archives.map((a) => (
              <div key={a.id} className="flex items-center gap-4 p-4 hover:bg-gray-50">
                <FileIcon className="h-8 w-8 text-gray-400 shrink-0" />
                <div className="flex-1 min-w-0">
                  <p className="text-sm font-medium truncate">{a.fileName}</p>
                  <p className="text-xs text-gray-500">
                    {formatFileSize(a.fileSize)} • {a.documentType || "Belge"} •{" "}
                    {formatDate(a.createdAt)}
                    {a.uploadedBy && ` • ${a.uploadedBy}`}
                  </p>
                </div>
                <div className="shrink-0">
                  {a.sentToEvrim ? (
                    <span className="text-xs text-green-600 bg-green-50 px-2 py-1 rounded">
                      ✓ Evrim'e gönderildi
                    </span>
                  ) : declaration?.sentToEvrim ? (
                    <button
                      onClick={() => handleSendToEvrim(a.id)}
                      className="flex items-center gap-1 text-xs text-blue-600 bg-blue-50 px-2 py-1 rounded hover:bg-blue-100"
                    >
                      <Send className="h-3 w-3" />
                      Evrim'e Gönder
                    </button>
                  ) : (
                    <span className="text-xs text-gray-400">Bekliyor</span>
                  )}
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </>
  );
}
