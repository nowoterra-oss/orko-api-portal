"use client";

import { useEffect, useState } from "react";
import { logService, ApiLog } from "@/services/logService";
import { PageHeader } from "@/components/shared/PageHeader";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { EmptyState } from "@/components/shared/EmptyState";
import { formatDate } from "@/lib/utils";
import { ChevronLeft, ChevronRight, RefreshCw } from "lucide-react";

const methodColors: Record<string, string> = {
  GET: "bg-green-100 text-green-700",
  POST: "bg-blue-100 text-blue-700",
  PUT: "bg-yellow-100 text-yellow-700",
  DELETE: "bg-red-100 text-red-700",
};

const getStatusColor = (code: number) => {
  if (code >= 200 && code < 300) return "text-green-600";
  if (code >= 400 && code < 500) return "text-yellow-600";
  if (code >= 500) return "text-red-600";
  return "text-gray-600";
};

export default function LogsPage() {
  const [items, setItems] = useState<ApiLog[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [page, setPage] = useState(1);
  const [expandedId, setExpandedId] = useState<number | null>(null);

  const fetchLogs = () => {
    setLoading(true);
    logService
      .list({ page, pageSize: 30 })
      .then((result) => {
        setItems(result.items);
        setTotalCount(result.totalCount);
        setTotalPages(result.totalPages);
      })
      .catch(console.error)
      .finally(() => setLoading(false));
  };

  useEffect(() => {
    fetchLogs();
  }, [page]);

  const formatBody = (body?: string) => {
    if (!body) return "(boş)";
    try {
      return JSON.stringify(JSON.parse(body), null, 2);
    } catch {
      return body;
    }
  };

  return (
    <>
      <PageHeader title="API Logları" description="Gelen ve giden tüm API trafiği">
        <button
          onClick={fetchLogs}
          className="flex items-center gap-2 px-4 py-2 text-sm text-gray-600 border border-gray-300 rounded-lg hover:bg-gray-50"
        >
          <RefreshCw className="h-4 w-4" />
          Yenile
        </button>
      </PageHeader>

      <div className="card overflow-hidden">
        {loading ? (
          <LoadingSpinner />
        ) : items.length === 0 ? (
          <EmptyState title="Henüz log yok" description="API çağrıları yapıldıkça burada görünecek." />
        ) : (
          <>
            <div className="overflow-x-auto">
              <table className="w-full text-left">
                <thead>
                  <tr className="border-b border-gray-200 bg-gray-50">
                    <th className="px-4 py-3 text-xs font-medium text-gray-500 uppercase">Yön</th>
                    <th className="px-4 py-3 text-xs font-medium text-gray-500 uppercase">Method</th>
                    <th className="px-4 py-3 text-xs font-medium text-gray-500 uppercase">Endpoint</th>
                    <th className="px-4 py-3 text-xs font-medium text-gray-500 uppercase">Kullanıcı</th>
                    <th className="px-4 py-3 text-xs font-medium text-gray-500 uppercase">Durum</th>
                    <th className="px-4 py-3 text-xs font-medium text-gray-500 uppercase">Süre</th>
                    <th className="px-4 py-3 text-xs font-medium text-gray-500 uppercase">Tarih</th>
                  </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                  {items.map((log) => (
                    <tr key={log.id} className="group">
                      <td colSpan={7} className="p-0">
                        {/* Satir */}
                        <div
                          className="grid grid-cols-[100px_70px_1fr_120px_60px_70px_150px] items-center hover:bg-gray-50 cursor-pointer px-4 py-3"
                          onClick={() => setExpandedId(expandedId === log.id ? null : log.id)}
                        >
                          <span className={`text-xs font-medium px-2 py-0.5 rounded w-fit ${
                            log.direction === "INCOMING"
                              ? "bg-blue-50 text-blue-700"
                              : "bg-purple-50 text-purple-700"
                          }`}>
                            {log.direction === "INCOMING" ? "← Gelen" : "→ Giden"}
                          </span>
                          <span className={`text-xs font-mono font-medium px-2 py-0.5 rounded w-fit ${
                            methodColors[log.method] || "bg-gray-100 text-gray-700"
                          }`}>
                            {log.method}
                          </span>
                          <span className="text-sm font-mono text-gray-700 truncate">
                            {log.endpoint}
                          </span>
                          <span className="text-sm text-gray-600 truncate">
                            {log.userName || <span className="text-gray-400 italic">Sistem</span>}
                          </span>
                          <span className={`text-sm font-medium ${getStatusColor(log.statusCode)}`}>
                            {log.statusCode}
                          </span>
                          <span className="text-sm text-gray-500">
                            {log.durationMs}ms
                          </span>
                          <span className="text-sm text-gray-500 whitespace-nowrap">
                            {formatDate(log.createdAt)}
                          </span>
                        </div>

                        {/* Detay */}
                        {expandedId === log.id && (
                          <div className="px-4 py-4 bg-gray-50 border-t border-gray-100">
                            {log.userName && (
                              <div className="mb-3 flex gap-4 text-xs text-gray-600">
                                <span><strong>Kullanıcı:</strong> {log.userName}</span>
                                {log.userEmail && <span><strong>E-posta:</strong> {log.userEmail}</span>}
                                {log.userRole && <span><strong>Rol:</strong> {log.userRole}</span>}
                              </div>
                            )}
                            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                              <div>
                                <p className="text-xs font-medium text-gray-500 mb-1">Request Body</p>
                                <pre className="text-xs bg-white p-3 rounded border overflow-auto max-h-48 whitespace-pre-wrap break-all">
                                  {formatBody(log.requestBody)}
                                </pre>
                              </div>
                              <div>
                                <p className="text-xs font-medium text-gray-500 mb-1">Response Body</p>
                                <pre className="text-xs bg-white p-3 rounded border overflow-auto max-h-48 whitespace-pre-wrap break-all">
                                  {formatBody(log.responseBody)}
                                </pre>
                              </div>
                            </div>
                          </div>
                        )}
                      </td>
                    </tr>
                  ))}
                </tbody>
              </table>
            </div>

            {/* Sayfalama */}
            <div className="flex items-center justify-between px-4 py-3 border-t border-gray-200">
              <p className="text-sm text-gray-500">
                Toplam {totalCount} kayıt, Sayfa {page}/{totalPages}
              </p>
              <div className="flex gap-2">
                <button
                  onClick={() => setPage((p) => Math.max(1, p - 1))}
                  disabled={page === 1}
                  className="p-2 rounded-lg border border-gray-300 disabled:opacity-50 hover:bg-gray-50"
                >
                  <ChevronLeft className="h-4 w-4" />
                </button>
                <button
                  onClick={() => setPage((p) => Math.min(totalPages, p + 1))}
                  disabled={page >= totalPages}
                  className="p-2 rounded-lg border border-gray-300 disabled:opacity-50 hover:bg-gray-50"
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
