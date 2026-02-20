"use client";

import { useEffect, useState } from "react";
import { dashboardService } from "@/services/dashboardService";
import { DashboardSummary } from "@/lib/types";
import { PageHeader } from "@/components/shared/PageHeader";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { StatusBadge } from "@/components/shared/StatusBadge";
import { formatDate, statusLabels } from "@/lib/utils";
import {
  ClipboardList,
  FileText,
  Send,
  CheckCircle,
} from "lucide-react";

export default function DashboardPage() {
  const [data, setData] = useState<DashboardSummary | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    dashboardService
      .getSummary()
      .then(setData)
      .catch((err) => setError(err.message))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <LoadingSpinner />;
  if (error) return <div className="text-red-500 p-4">Hata: {error}</div>;
  if (!data) return null;

  const cards = [
    {
      title: "Toplam İş Emri",
      value: data.totalWorkOrders,
      icon: ClipboardList,
      color: "bg-blue-500",
    },
    {
      title: "Bekleyen Beyanname",
      value: data.pendingDeclarations,
      icon: FileText,
      color: "bg-yellow-500",
    },
    {
      title: "Evrim'e Gönderilen",
      value: data.sentToEvrim,
      icon: Send,
      color: "bg-purple-500",
    },
    {
      title: "Bugün Kapanan",
      value: data.completedToday,
      icon: CheckCircle,
      color: "bg-green-500",
    },
  ];

  return (
    <>
      <PageHeader
        title="Dashboard"
        description="Gümrük beyanname yönetim sistemi genel görünüm"
      />

      {/* Özet Kartlar */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-4 mb-8">
        {cards.map((card) => (
          <div
            key={card.title}
            className="card p-6"
          >
            <div className="flex items-center justify-between">
              <div>
                <p className="text-sm font-medium text-gray-500">{card.title}</p>
                <p className="text-3xl font-bold mt-1">{card.value}</p>
              </div>
              <div className={`${card.color} p-3 rounded-lg`}>
                <card.icon className="h-6 w-6 text-white" />
              </div>
            </div>
          </div>
        ))}
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        {/* Statü Dağılımı */}
        <div className="card p-6">
          <h2 className="text-lg font-semibold mb-4">Statü Dağılımı</h2>
          <div className="space-y-3">
            {Object.entries(data.statusDistribution).map(([status, count]) => {
              const total = data.totalWorkOrders || 1;
              const pct = Math.round((count / total) * 100);
              return (
                <div key={status} className="flex items-center gap-3">
                  <div className="w-32">
                    <StatusBadge status={status} size="sm" />
                  </div>
                  <div className="flex-1 bg-gray-100 rounded-full h-2.5">
                    <div
                      className="bg-blue-500 h-2.5 rounded-full"
                      style={{ width: `${pct}%` }}
                    />
                  </div>
                  <span className="text-sm text-gray-600 w-12 text-right">
                    {count}
                  </span>
                </div>
              );
            })}
          </div>
        </div>

        {/* Son Aktiviteler */}
        <div className="card p-6">
          <h2 className="text-lg font-semibold mb-4">Son Aktiviteler</h2>
          {data.recentActivities.length === 0 ? (
            <p className="text-sm text-gray-500">Henüz aktivite yok.</p>
          ) : (
            <div className="space-y-3">
              {data.recentActivities.map((activity, i) => (
                <div
                  key={i}
                  className="flex items-start gap-3 pb-3 border-b border-gray-100 last:border-0"
                >
                  <div className="w-2 h-2 bg-blue-500 rounded-full mt-2 shrink-0" />
                  <div className="flex-1 min-w-0">
                    <p className="text-sm font-medium text-gray-900">
                      {activity.fileNumber}
                    </p>
                    <p className="text-xs text-gray-500">{activity.action}</p>
                  </div>
                  <div className="text-xs text-gray-400 shrink-0">
                    {formatDate(activity.timestamp)}
                  </div>
                </div>
              ))}
            </div>
          )}
        </div>
      </div>
    </>
  );
}
