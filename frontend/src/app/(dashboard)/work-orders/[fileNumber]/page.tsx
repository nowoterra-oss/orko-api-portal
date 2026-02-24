"use client";

import { useEffect, useState, useRef } from "react";
import { useParams, useRouter } from "next/navigation";
import Link from "next/link";
import { workOrderService } from "@/services/workOrderService";
import { declarationService } from "@/services/declarationService";
import { WorkOrderDetail, DeclarationDetail } from "@/lib/types";
import { PageHeader } from "@/components/shared/PageHeader";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { TypeBadge } from "@/components/shared/TypeBadge";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { formatDate } from "@/lib/utils";
import { ArrowLeft, FileEdit, Send, Clock, Archive, ExternalLink, Upload } from "lucide-react";

export default function WorkOrderDetailPage() {
  const params = useParams();
  const router = useRouter();
  const fileNumber = decodeURIComponent(params.fileNumber as string);

  const [wo, setWo] = useState<WorkOrderDetail | null>(null);
  const [declaration, setDeclaration] = useState<DeclarationDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [sending, setSending] = useState(false);
  const [uploading, setUploading] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    const load = async () => {
      try {
        const result = await workOrderService.getByFileNumber(fileNumber);
        setWo(result);
        if (result.declaration?.id) {
          const decl = await declarationService.get(result.declaration.id);
          setDeclaration(decl);
        }
      } catch {
        console.error("Is emri yuklenemedi");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [fileNumber]);

  const isDeclarationDataValid = (() => {
    if (!declaration?.declarationData) return false;
    try {
      const parsed = JSON.parse(declaration.declarationData);
      return !!(parsed.gumruk && parsed.musteriVergi && parsed.kalemler?.length > 0);
    } catch { return false; }
  })();

  const handleSendToEvrim = async () => {
    if (!declaration?.id || sending) return;
    if (!isDeclarationDataValid) {
      alert("Beyanname verileri eksik. Lütfen önce formu doldurun.");
      router.push(`/declarations/${declaration.id}/edit`);
      return;
    }
    if (!confirm("Beyannameyi Evrim'e göndermek istediğinize emin misiniz?")) return;
    setSending(true);
    try {
      const result = await declarationService.sendToEvrim(declaration.id);
      if (result.success) {
        alert("Evrim'e başarıyla gönderildi!");
        window.location.reload();
      } else {
        alert(`Hata: ${result.message}`);
      }
    } catch (err: any) {
      alert(err.response?.data?.message || "Gönderim hatası");
    } finally {
      setSending(false);
    }
  };

  const handleFileUpload = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const file = e.target.files?.[0];
    if (!file || !declaration?.id) return;
    e.target.value = "";

    const ext = file.name.split(".").pop()?.toLowerCase();
    if (ext !== "json" && ext !== "xml") {
      alert("Desteklenmeyen dosya formatı. Lütfen .json veya .xml dosyası seçin.");
      return;
    }

    if (!confirm(`"${file.name}" dosyasını Evrim'e göndermek istediğinize emin misiniz?\nBu işlem geri alınamaz.`))
      return;

    setUploading(true);
    try {
      const fileContent = await file.text();
      const result = await declarationService.uploadAndSend(declaration.id, fileContent, ext as "json" | "xml");
      if (result.success) {
        alert("Dosyadan yüklendi ve Evrim'e başarıyla gönderildi!");
        window.location.reload();
      } else {
        alert("Evrim hatası: " + result.message);
      }
    } catch (err: any) {
      alert("Yükleme hatası: " + (err.response?.data?.message || err.message || "Bilinmeyen hata"));
    } finally {
      setUploading(false);
    }
  };

  if (loading) return <LoadingSpinner />;
  if (!wo) return <div className="p-4 text-red-500">İş emri bulunamadı.</div>;

  return (
    <>
      <PageHeader title={wo.fileNumber} description={`${wo.type} iş emri detayı`}>
        <button
          onClick={() => router.push("/work-orders")}
          className="flex items-center gap-2 px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50"
        >
          <ArrowLeft className="h-4 w-4" /> Geri
        </button>
      </PageHeader>

      <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <div className="lg:col-span-2 space-y-6">
          {/* İş Emri Kartı */}
          <div className="card p-6">
            <h2 className="text-lg font-semibold mb-4">İş Emri Bilgileri</h2>
            <dl className="grid grid-cols-2 gap-4">
              <div>
                <dt className="text-xs font-medium text-gray-500">Dosya No</dt>
                <dd className="text-sm font-medium">{wo.fileNumber}</dd>
              </div>
              <div>
                <dt className="text-xs font-medium text-gray-500">Selsil ID</dt>
                <dd className="text-sm">{wo.selsilOrderId || "-"}</dd>
              </div>
              <div>
                <dt className="text-xs font-medium text-gray-500">Tip</dt>
                <dd><TypeBadge type={wo.type} /></dd>
              </div>
              <div>
                <dt className="text-xs font-medium text-gray-500">Statü</dt>
                <dd><StatusBadge status={wo.status} /></dd>
              </div>
              <div>
                <dt className="text-xs font-medium text-gray-500">Oluşturulma</dt>
                <dd className="text-sm">{formatDate(wo.createdAt)}</dd>
              </div>
              <div>
                <dt className="text-xs font-medium text-gray-500">Son Güncelleme</dt>
                <dd className="text-sm">{formatDate(wo.updatedAt)}</dd>
              </div>
            </dl>
          </div>

          {/* Beyanname */}
          {declaration && (
            <div className="card p-6">
              <div className="flex items-center justify-between mb-4">
                <h2 className="text-lg font-semibold">Beyanname</h2>
                {!declaration.sentToEvrim && (
                  <button
                    onClick={handleSendToEvrim}
                    disabled={sending || !isDeclarationDataValid}
                    className="flex items-center gap-1 px-3 py-1.5 text-sm bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:opacity-50"
                  >
                    <Send className="h-4 w-4" />
                    {sending ? "Gönderiliyor..." : "Evrim'e Gönder"}
                  </button>
                )}
              </div>
              <dl className="grid grid-cols-2 gap-4 mb-4">
                <div>
                  <dt className="text-xs font-medium text-gray-500">Evrim ID</dt>
                  <dd className="text-sm font-mono">{declaration.evrimDeclarationId || "Henüz gönderilmedi"}</dd>
                </div>
                <div>
                  <dt className="text-xs font-medium text-gray-500">Gönderim</dt>
                  <dd className="text-sm">
                    {declaration.sentToEvrim
                      ? <span className="text-green-600">✓ {formatDate(declaration.sentAt!)}</span>
                      : <span className="text-gray-400">Gönderilmedi</span>}
                  </dd>
                </div>
              </dl>
              <div>
                <div className="flex items-center justify-between mb-1">
                <label className="text-xs font-medium text-gray-500">Beyanname Verileri</label>
                <Link
                  href={`/declarations/${declaration.id}/edit`}
                  className="flex items-center gap-1 text-xs text-blue-600 hover:text-blue-700 font-medium"
                >
                  <FileEdit className="w-3.5 h-3.5" />
                  {declaration.sentToEvrim ? "Görüntüle" : "Düzenle"}
                </Link>
              </div>
                <pre className="p-3 bg-gray-50 rounded-lg text-xs overflow-auto max-h-48 border">
                  {declaration.declarationData
                    ? (() => { try { return JSON.stringify(JSON.parse(declaration.declarationData), null, 2); } catch { return declaration.declarationData; } })()
                    : "Henüz veri girilmedi."}
                </pre>
              </div>
            </div>
          )}

          {/* Statü Geçmişi */}
          {declaration && declaration.statusHistory.length > 0 && (
            <div className="card p-6">
              <h2 className="text-lg font-semibold mb-4">Statü Geçmişi</h2>
              <div className="space-y-4">
                {declaration.statusHistory.map((sh) => (
                  <div key={sh.id} className="flex items-start gap-3">
                    <div className="w-2 h-2 bg-blue-500 rounded-full mt-2 shrink-0" />
                    <div className="flex-1">
                      <div className="flex items-center gap-2 flex-wrap">
                        <StatusBadge status={sh.fromStatus} size="sm" />
                        <span className="text-gray-400">→</span>
                        <StatusBadge status={sh.toStatus} size="sm" />
                        {sh.evrimNotified && <span className="text-xs text-green-600 bg-green-50 px-1.5 py-0.5 rounded">✓ Evrim</span>}
                      </div>
                      <p className="text-xs text-gray-500 mt-1">
                        {sh.changedBy || "Sistem"} • {formatDate(sh.createdAt)}
                        {sh.note && ` • ${sh.note}`}
                      </p>
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>

        {/* Sag panel */}
        <div className="space-y-6">
          {declaration && (
            <div className="card p-6">
              <h2 className="text-lg font-semibold mb-4">İşlemler</h2>
              <div className="space-y-2">
                <Link href={`/declarations/${declaration.id}/status`}
                  className="flex items-center gap-3 w-full px-4 py-3 text-sm border border-gray-200 rounded-lg hover:bg-gray-50 hover:border-gray-300 transition-colors">
                  <Clock className="h-5 w-5 text-blue-500" />
                  <div><p className="font-medium">Statü Güncelle</p><p className="text-xs font-medium text-gray-500">Manuel statü değişikliği</p></div>
                </Link>
                <Link href={`/declarations/${declaration.id}/archives`}
                  className="flex items-center gap-3 w-full px-4 py-3 text-sm border border-gray-200 rounded-lg hover:bg-gray-50 hover:border-gray-300 transition-colors">
                  <Archive className="h-5 w-5 text-purple-500" />
                  <div><p className="font-medium">Arşiv Yönetimi</p><p className="text-xs font-medium text-gray-500">{declaration.archives.length} belge yüklü</p></div>
                </Link>
                {!declaration.sentToEvrim && (
                  <>
                    <input
                      ref={fileInputRef}
                      type="file"
                      accept=".json,.xml"
                      onChange={handleFileUpload}
                      className="hidden"
                    />
                    <button
                      onClick={() => fileInputRef.current?.click()}
                      disabled={uploading}
                      className="flex items-center gap-3 w-full px-4 py-3 text-sm border border-gray-200 rounded-lg hover:bg-gray-50 hover:border-gray-300 transition-colors disabled:opacity-50"
                    >
                      <Upload className="h-5 w-5 text-violet-500" />
                      <div className="text-left">
                        <p className="font-medium">{uploading ? "Yükleniyor..." : "Dosyadan Beyanname Yükle"}</p>
                        <p className="text-xs font-medium text-gray-500">JSON veya XML dosyası ile gönder</p>
                      </div>
                    </button>
                  </>
                )}
              </div>
            </div>
          )}
          {declaration && declaration.archives.length > 0 && (
            <div className="card p-6">
              <div className="flex items-center justify-between mb-4">
                <h2 className="text-lg font-semibold">Arşivler</h2>
                <Link href={`/declarations/${declaration.id}/archives`} className="text-xs text-blue-600 hover:underline flex items-center gap-1">
                  Tümü <ExternalLink className="h-3 w-3" />
                </Link>
              </div>
              <div className="space-y-2">
                {declaration.archives.slice(0, 5).map((a) => (
                  <div key={a.id} className="flex items-center justify-between p-2 bg-gray-50 rounded-lg">
                    <div className="min-w-0">
                      <p className="text-sm font-medium truncate">{a.fileName}</p>
                      <p className="text-xs font-medium text-gray-500">{a.documentType || "Belge"}</p>
                    </div>
                    {a.sentToEvrim ? <span className="text-xs text-green-600 shrink-0">✓</span> : <span className="text-xs text-gray-400 shrink-0">—</span>}
                  </div>
                ))}
              </div>
            </div>
          )}
        </div>
      </div>
    </>
  );
}
