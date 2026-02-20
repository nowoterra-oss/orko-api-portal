"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { declarationService } from "@/services/declarationService";
import { statusService } from "@/services/statusService";
import { DeclarationDetail, AllowedTransitions } from "@/lib/types";
import { PageHeader } from "@/components/shared/PageHeader";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { statusLabels, formatDate } from "@/lib/utils";
import { ArrowLeft, ArrowRight } from "lucide-react";

export default function StatusUpdatePage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;

  const [declaration, setDeclaration] = useState<DeclarationDetail | null>(null);
  const [transitions, setTransitions] = useState<AllowedTransitions | null>(null);
  const [loading, setLoading] = useState(true);
  const [note, setNote] = useState("");
  const [updating, setUpdating] = useState(false);

  useEffect(() => {
    const load = async () => {
      try {
        const [decl, trans] = await Promise.all([
          declarationService.get(id),
          statusService.getAllowedTransitions(id),
        ]);
        setDeclaration(decl);
        setTransitions(trans);
      } catch {
        console.error("Yuklenemedi");
      } finally {
        setLoading(false);
      }
    };
    load();
  }, [id]);

  const handleStatusUpdate = async (newStatus: string) => {
    if (updating) return;
    const label = statusLabels[newStatus] || newStatus;
    if (!confirm(`Statüyü "${label}" olarak güncellemek istediğinize emin misiniz?`))
      return;

    setUpdating(true);
    try {
      const result = await statusService.update(id, newStatus, note || undefined);
      if (result.success) {
        alert("Statü güncellendi!");
        router.back();
      } else {
        alert(`Hata: ${result.message}`);
      }
    } catch (err: any) {
      alert(err.response?.data?.message || "Güncelleme hatası");
    } finally {
      setUpdating(false);
    }
  };

  if (loading) return <LoadingSpinner />;
  if (!declaration || !transitions) return <div className="p-4 text-red-500">Bulunamadı.</div>;

  return (
    <>
      <PageHeader title="Statü Güncelle" description={declaration.fileNumber}>
        <button
          onClick={() => router.back()}
          className="flex items-center gap-2 px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50"
        >
          <ArrowLeft className="h-4 w-4" />
          Geri
        </button>
      </PageHeader>

      <div className="max-w-2xl space-y-6">
        {/* Mevcut Statü */}
        <div className="card p-6">
          <h2 className="text-sm text-gray-500 mb-2">Mevcut Statü</h2>
          <StatusBadge status={transitions.currentStatus} />
        </div>

        {/* Geçiş Seçenekleri */}
        <div className="card p-6">
          <h2 className="text-lg font-semibold mb-4">Geçiş Yapılabilir Statüler</h2>

          {transitions.allowedTransitions.length === 0 ? (
            <p className="text-sm text-gray-500">
              Bu statüden başka bir statüye geçiş yapılamaz.
            </p>
          ) : (
            <div className="space-y-3">
              {/* Not alanı */}
              <div className="mb-4">
                <label className="text-sm text-gray-600">Not (opsiyonel)</label>
                <textarea
                  value={note}
                  onChange={(e) => setNote(e.target.value)}
                  placeholder="Statü değişikliği ile ilgili not ekleyin..."
                  className="input-base mt-1"
                  rows={2}
                />
              </div>

              {transitions.allowedTransitions.map((status) => (
                <button
                  key={status}
                  onClick={() => handleStatusUpdate(status)}
                  disabled={updating}
                  className="flex items-center justify-between w-full px-4 py-3 border border-gray-200 rounded-lg hover:bg-blue-50 hover:border-blue-300 transition-colors disabled:opacity-50"
                >
                  <div className="flex items-center gap-3">
                    <StatusBadge status={transitions.currentStatus} size="sm" />
                    <ArrowRight className="h-4 w-4 text-gray-400" />
                    <StatusBadge status={status} size="sm" />
                  </div>
                  <span className="text-sm text-blue-600 font-medium">
                    {updating ? "..." : "Güncelle →"}
                  </span>
                </button>
              ))}
            </div>
          )}
        </div>

        {/* Geçmiş */}
        {declaration.statusHistory.length > 0 && (
          <div className="card p-6">
            <h2 className="text-lg font-semibold mb-4">Geçmiş</h2>
            <div className="space-y-3">
              {declaration.statusHistory.map((sh) => (
                <div key={sh.id} className="flex items-start gap-3 pb-3 border-b border-gray-100 last:border-0">
                  <div className="w-2 h-2 bg-blue-500 rounded-full mt-2 shrink-0" />
                  <div>
                    <div className="flex items-center gap-2">
                      <StatusBadge status={sh.fromStatus} size="sm" />
                      <span className="text-gray-400">→</span>
                      <StatusBadge status={sh.toStatus} size="sm" />
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
    </>
  );
}
