"use client";

import { useEffect, useState } from "react";
import Link from "next/link";
import { workOrderService } from "@/services/workOrderService";
import { WorkOrder } from "@/lib/types";
import { PagedResult } from "@/lib/api";
import { PageHeader } from "@/components/shared/PageHeader";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { TypeBadge } from "@/components/shared/TypeBadge";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { EmptyState } from "@/components/shared/EmptyState";
import { formatDate } from "@/lib/utils";
import { Search, ChevronLeft, ChevronRight } from "lucide-react";

export default function WorkOrdersPage() {
  const [data, setData] = useState<PagedResult<WorkOrder> | null>(null);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [statusFilter, setStatusFilter] = useState("");
  const [typeFilter, setTypeFilter] = useState("");

  const fetchData = () => {
    setLoading(true);
    workOrderService
      .list({
        page,
        pageSize: 20,
        status: statusFilter || undefined,
        type: typeFilter || undefined,
        search: search || undefined,
      })
      .then(setData)
      .catch(console.error)
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchData();
  }, [page, statusFilter, typeFilter]);

  const handleSearch = (e: React.FormEvent) => {
    e.preventDefault();
    setPage(1);
    fetchData();
  };

  return (
    <>
      <PageHeader title="İş Emirleri" description="Selsil'den gelen iş emirleri listesi" />

      {/* Filtreler */}
      <div className="card p-4 mb-6">
        <div className="flex flex-wrap gap-4">
          {/* Arama */}
          <form onSubmit={handleSearch} className="flex-1 min-w-[200px]">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-gray-400" />
              <input
                type="text"
                placeholder="Dosya no veya Selsil ID ara..."
                value={search}
                onChange={(e) => setSearch(e.target.value)}
                className="input-base pl-10"
              />
            </div>
          </form>

          {/* Statü filtre */}
          <select
            value={statusFilter}
            onChange={(e) => {
              setStatusFilter(e.target.value);
              setPage(1);
            }}
            className="input-base w-auto cursor-pointer"
          >
            <option value="">Tüm Statüler</option>
            <option value="Taslak">Taslak</option>
            <option value="Hazirlaniyor">Hazırlanıyor</option>
            <option value="EvrimeGonderildi">Evrim'e Gönderildi</option>
            <option value="TescilBekleniyor">Tescil Bekleniyor</option>
            <option value="TescilEdildi">Tescil Edildi</option>
            <option value="RedEdildi">Reddedildi</option>
            <option value="DuzeltmeGerekli">Düzeltme Gerekli</option>
            <option value="Kapandi">Kapandı</option>
          </select>

          {/* Tip filtre */}
          <select
            value={typeFilter}
            onChange={(e) => {
              setTypeFilter(e.target.value);
              setPage(1);
            }}
            className="input-base w-auto cursor-pointer"
          >
            <option value="">Tüm Tipler</option>
            <option value="Import">İthalat</option>
            <option value="Export">İhracat</option>
          </select>
        </div>
      </div>

      {/* Tablo */}
      <div className="card overflow-hidden">
        {loading ? (
          <LoadingSpinner />
        ) : !data || data.items.length === 0 ? (
          <EmptyState title="İş emri bulunamadı" description="Henüz iş emri oluşturulmamış veya filtrelere uygun sonuç yok." />
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full">
                <thead>
                  <tr className="border-b border-gray-200 bg-gray-50">
                    <th className="text-left px-4 py-3 text-xs font-medium text-gray-500 uppercase">
                      Dosya No
                    </th>
                    <th className="text-left px-4 py-3 text-xs font-medium text-gray-500 uppercase">
                      Selsil ID
                    </th>
                    <th className="text-left px-4 py-3 text-xs font-medium text-gray-500 uppercase">
                      Müşteri
                    </th>
                    <th className="text-left px-4 py-3 text-xs font-medium text-gray-500 uppercase">
                      Tip
                    </th>
                    <th className="text-left px-4 py-3 text-xs font-medium text-gray-500 uppercase">
                      Statü
                    </th>
                    <th className="text-left px-4 py-3 text-xs font-medium text-gray-500 uppercase">
                      Evrim
                    </th>
                    <th className="text-left px-4 py-3 text-xs font-medium text-gray-500 uppercase">
                      Tarih
                    </th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {data.items.map((wo) => (
                    <tr
                      key={wo.id}
                      className="hover:bg-gray-50 transition-colors"
                    >
                      <td className="px-4 py-3">
                        <Link
                          href={`/work-orders/${encodeURIComponent(wo.fileNumber)}`}
                          className="text-sm font-medium text-blue-600 hover:underline"
                        >
                          {wo.fileNumber}
                        </Link>
                      </td>
                      <td className="px-4 py-3 text-sm text-gray-600">
                        {wo.selsilOrderId || "-"}
                      </td>
                      <td className="px-4 py-3 text-sm text-gray-600 max-w-[200px] truncate" title={wo.customerName || ""}>
                        {wo.customerName || "-"}
                      </td>
                      <td className="px-4 py-3">
                        <TypeBadge type={wo.type} />
                      </td>
                      <td className="px-4 py-3">
                        <StatusBadge status={wo.status} size="sm" />
                      </td>
                      <td className="px-4 py-3">
                        {wo.sentToEvrim ? (
                          <span className="text-green-600 text-sm">✓ Gönderildi</span>
                        ) : (
                          <span className="text-gray-400 text-sm">—</span>
                        )}
                      </td>
                      <td className="px-4 py-3 text-sm text-gray-500">
                        {formatDate(wo.createdAt)}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Sayfalama */}
            <div className="flex items-center justify-between px-4 py-3 border-t border-gray-200">
              <p className="text-sm text-gray-500">
                Toplam {data.totalCount} kayıt, Sayfa {data.page}/{data.totalPages}
              </p>
              <div className="flex gap-2">
                <button
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  disabled={page === 1}
                  className="p-2 rounded-lg border border-gray-300 disabled:opacity-50 hover:bg-gray-50 transition-colors"
                >
                  <ChevronLeft className="h-4 w-4" />
                </button>
                <button
                  onClick={() => setPage((p) => Math.min(data.totalPages, p + 1))}
                  disabled={page >= data.totalPages}
                  className="p-2 rounded-lg border border-gray-300 disabled:opacity-50 hover:bg-gray-50 transition-colors"
                >
                  <ChevronRight className="h-4 w-4" />
                </button>
              </div>
            </div>
          </>
        )}
      </div>
    </>
  );
}
