"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/contexts/AuthContext";
import { useRouter } from "next/navigation";
import {
  settingsService,
  AppSettingDto,
  SettingItemDto,
} from "@/services/settingsService";
import { PageHeader } from "@/components/shared/PageHeader";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { Save, TestTube, CheckCircle, XCircle } from "lucide-react";

const TABS = [
  { key: "Smtp", label: "SMTP Ayarları" },
  { key: "WorkOrderEmail", label: "İş Emri Bildirimi" },
  { key: "Reminder", label: "Hatırlatma Servisi" },
];

const CRON_PRESETS = [
  { label: "Her saat", value: "0 * * * *" },
  { label: "Her 2 saatte", value: "0 */2 * * *" },
  { label: "Her gün 09:00", value: "0 9 * * *" },
  { label: "Hafta içi 09:00", value: "0 9 * * 1-5" },
  { label: "Hafta içi 09:00 ve 17:00", value: "0 9,17 * * 1-5" },
  { label: "Her Pazartesi 09:00", value: "0 9 * * 1" },
];

export default function SettingsPage() {
  const { hasRole } = useAuth();
  const router = useRouter();
  const [settings, setSettings] = useState<AppSettingDto[]>([]);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState("Smtp");
  const [saving, setSaving] = useState(false);
  const [testing, setTesting] = useState(false);
  const [message, setMessage] = useState<{ type: "success" | "error"; text: string } | null>(null);

  useEffect(() => {
    if (!hasRole("Admin")) {
      router.replace("/");
      return;
    }
    loadSettings();
  }, [hasRole, router]);

  const loadSettings = async () => {
    try {
      const res = await settingsService.getAll();
      if (res.success && res.data) {
        setSettings(res.data);
      }
    } catch {
      // ignore
    } finally {
      setLoading(false);
    }
  };

  const getValue = (key: string) => {
    return settings.find((s) => s.key === key)?.value ?? "";
  };

  const setValue = (key: string, value: string) => {
    setSettings((prev) => {
      const existing = prev.find((s) => s.key === key);
      if (existing) {
        return prev.map((s) => (s.key === key ? { ...s, value } : s));
      }
      return [
        ...prev,
        { id: 0, key, value, description: null, category: activeTab, updatedAt: "", updatedBy: null },
      ];
    });
  };

  const getCategorySettings = (category: string): SettingItemDto[] => {
    return settings
      .filter((s) => s.category === category)
      .map((s) => ({ key: s.key, value: s.value, category: s.category }));
  };

  const handleSave = async (category: string) => {
    setSaving(true);
    setMessage(null);
    try {
      const items = getCategorySettings(category);
      const res = await settingsService.update(items);
      if (res.success) {
        setMessage({ type: "success", text: "Ayarlar kaydedildi." });
      } else {
        setMessage({ type: "error", text: res.message || "Kaydetme başarısız." });
      }
    } catch (err: any) {
      setMessage({ type: "error", text: err.response?.data?.message || "Bir hata oluştu." });
    } finally {
      setSaving(false);
      setTimeout(() => setMessage(null), 4000);
    }
  };

  const handleTestSmtp = async () => {
    setTesting(true);
    setMessage(null);
    try {
      // First save SMTP settings, then test
      const smtpItems = getCategorySettings("Smtp");
      await settingsService.update(smtpItems);

      const res = await settingsService.testSmtp();
      if (res.success && res.data) {
        setMessage({
          type: res.data.success ? "success" : "error",
          text: res.data.message || (res.data.success ? "Bağlantı başarılı." : "Bağlantı başarısız."),
        });
      }
    } catch (err: any) {
      setMessage({ type: "error", text: err.response?.data?.message || "Test başarısız." });
    } finally {
      setTesting(false);
      setTimeout(() => setMessage(null), 6000);
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <>
      <PageHeader title="Ayarlar" description="E-posta ve bildirim ayarlarını yönetin." />

      {/* Message */}
      {message && (
        <div
          className={`mb-4 p-3 flex items-center gap-2 text-sm rounded-lg border ${
            message.type === "success"
              ? "bg-green-50 border-green-200 text-green-700"
              : "bg-red-50 border-red-200 text-red-700"
          }`}
        >
          {message.type === "success" ? (
            <CheckCircle className="w-4 h-4 flex-shrink-0" />
          ) : (
            <XCircle className="w-4 h-4 flex-shrink-0" />
          )}
          {message.text}
        </div>
      )}

      {/* Tabs */}
      <div className="border-b border-gray-200 mb-6">
        <nav className="flex gap-6">
          {TABS.map((tab) => (
            <button
              key={tab.key}
              onClick={() => { setActiveTab(tab.key); setMessage(null); }}
              className={`pb-3 text-sm font-medium border-b-2 transition-colors ${
                activeTab === tab.key
                  ? "border-blue-600 text-blue-600"
                  : "border-transparent text-gray-500 hover:text-gray-700"
              }`}
            >
              {tab.label}
            </button>
          ))}
        </nav>
      </div>

      {/* Tab Content */}
      <div className="bg-white rounded-xl border border-gray-200 shadow-sm p-6">
        {activeTab === "Smtp" && <SmtpTab getValue={getValue} setValue={setValue} />}
        {activeTab === "WorkOrderEmail" && <WorkOrderEmailTab getValue={getValue} setValue={setValue} />}
        {activeTab === "Reminder" && <ReminderTab getValue={getValue} setValue={setValue} />}

        {/* Actions */}
        <div className="flex items-center gap-3 mt-8 pt-6 border-t border-gray-200">
          <button
            onClick={() => handleSave(activeTab)}
            disabled={saving}
            className="flex items-center gap-1.5 px-5 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
          >
            <Save className="w-4 h-4" />
            {saving ? "Kaydediliyor..." : "Kaydet"}
          </button>

          {activeTab === "Smtp" && (
            <button
              onClick={handleTestSmtp}
              disabled={testing}
              className="flex items-center gap-1.5 px-5 py-2 border border-gray-300 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-50 transition-colors disabled:opacity-50"
            >
              <TestTube className="w-4 h-4" />
              {testing ? "Test ediliyor..." : "Bağlantıyı Test Et"}
            </button>
          )}
        </div>
      </div>
    </>
  );
}

// --- Sub Components ---

function InputField({
  label,
  value,
  onChange,
  type = "text",
  placeholder,
  help,
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
  type?: string;
  placeholder?: string;
  help?: string;
}) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1.5">{label}</label>
      <input
        type={type}
        value={value}
        onChange={(e) => onChange(e.target.value)}
        placeholder={placeholder}
        className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
      />
      {help && <p className="mt-1 text-xs text-gray-500">{help}</p>}
    </div>
  );
}

function ToggleField({
  label,
  value,
  onChange,
  help,
}: {
  label: string;
  value: boolean;
  onChange: (v: boolean) => void;
  help?: string;
}) {
  return (
    <div className="flex items-center justify-between">
      <div>
        <label className="text-sm font-medium text-gray-700">{label}</label>
        {help && <p className="text-xs text-gray-500">{help}</p>}
      </div>
      <button
        type="button"
        onClick={() => onChange(!value)}
        className={`relative inline-flex h-6 w-11 items-center rounded-full transition-colors ${
          value ? "bg-blue-600" : "bg-gray-300"
        }`}
      >
        <span
          className={`inline-block h-4 w-4 transform rounded-full bg-white transition-transform ${
            value ? "translate-x-6" : "translate-x-1"
          }`}
        />
      </button>
    </div>
  );
}

function TextAreaField({
  label,
  value,
  onChange,
  rows = 6,
  help,
}: {
  label: string;
  value: string;
  onChange: (v: string) => void;
  rows?: number;
  help?: string;
}) {
  return (
    <div>
      <label className="block text-sm font-medium text-gray-700 mb-1.5">{label}</label>
      <textarea
        value={value}
        onChange={(e) => onChange(e.target.value)}
        rows={rows}
        className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm font-mono focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
      />
      {help && <p className="mt-1 text-xs text-gray-500">{help}</p>}
    </div>
  );
}

// --- Tabs ---

function SmtpTab({ getValue, setValue }: { getValue: (k: string) => string; setValue: (k: string, v: string) => void }) {
  return (
    <div className="space-y-4">
      <h3 className="text-base font-medium text-gray-900 mb-4">SMTP Sunucu Ayarları</h3>
      <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
        <InputField
          label="SMTP Sunucu"
          value={getValue("Smtp:Host")}
          onChange={(v) => setValue("Smtp:Host", v)}
          placeholder="smtp.gmail.com"
        />
        <InputField
          label="Port"
          value={getValue("Smtp:Port")}
          onChange={(v) => setValue("Smtp:Port", v)}
          type="number"
          placeholder="587"
        />
        <InputField
          label="Kullanıcı Adı"
          value={getValue("Smtp:Username")}
          onChange={(v) => setValue("Smtp:Username", v)}
          placeholder="user@example.com"
        />
        <InputField
          label="Şifre"
          value={getValue("Smtp:Password")}
          onChange={(v) => setValue("Smtp:Password", v)}
          type="password"
          placeholder="••••••••"
        />
        <InputField
          label="Gönderen E-posta"
          value={getValue("Smtp:FromAddress")}
          onChange={(v) => setValue("Smtp:FromAddress", v)}
          placeholder="noreply@orko.com"
        />
        <InputField
          label="Gönderen Adı"
          value={getValue("Smtp:FromName")}
          onChange={(v) => setValue("Smtp:FromName", v)}
          placeholder="Orko Portal"
        />
      </div>
      <ToggleField
        label="SSL Kullan"
        value={getValue("Smtp:EnableSsl") === "true"}
        onChange={(v) => setValue("Smtp:EnableSsl", v ? "true" : "false")}
      />
    </div>
  );
}

function WorkOrderEmailTab({ getValue, setValue }: { getValue: (k: string) => string; setValue: (k: string, v: string) => void }) {
  return (
    <div className="space-y-4">
      <h3 className="text-base font-medium text-gray-900 mb-4">İş Emri E-posta Bildirimi</h3>
      <ToggleField
        label="Bildirimi Aktifleştir"
        value={getValue("WorkOrderEmail:Enabled") === "true"}
        onChange={(v) => setValue("WorkOrderEmail:Enabled", v ? "true" : "false")}
        help="Yeni iş emri oluşturulduğunda e-posta bildirimi gönderir."
      />
      <InputField
        label="Alıcılar"
        value={getValue("WorkOrderEmail:Recipients")}
        onChange={(v) => setValue("WorkOrderEmail:Recipients", v)}
        placeholder="ali@orko.com, veli@orko.com"
        help="Birden fazla alıcıyı virgülle ayırın."
      />
      <InputField
        label="E-posta Konusu"
        value={getValue("WorkOrderEmail:Subject")}
        onChange={(v) => setValue("WorkOrderEmail:Subject", v)}
        placeholder="Yeni İş Emri: {FileNumber}"
        help="Kullanılabilir değişkenler: {FileNumber}, {CustomerName}"
      />
      <TextAreaField
        label="E-posta Şablonu (HTML)"
        value={getValue("WorkOrderEmail:Template")}
        onChange={(v) => setValue("WorkOrderEmail:Template", v)}
        rows={10}
        help="Kullanılabilir değişkenler: {FileNumber}, {CustomerName}, {Type}, {CreatedAt}. Boş bırakılırsa varsayılan şablon kullanılır."
      />
    </div>
  );
}

function ReminderTab({ getValue, setValue }: { getValue: (k: string) => string; setValue: (k: string, v: string) => void }) {
  const cronValue = getValue("Reminder:CronExpression");

  return (
    <div className="space-y-4">
      <h3 className="text-base font-medium text-gray-900 mb-4">Taslak İş Emri Hatırlatma Servisi</h3>
      <ToggleField
        label="Hatırlatma Servisini Aktifleştir"
        value={getValue("Reminder:Enabled") === "true"}
        onChange={(v) => setValue("Reminder:Enabled", v ? "true" : "false")}
        help="Beyanname gönderilmemiş taslak iş emirleri için periyodik hatırlatma e-postası gönderir."
      />
      <div>
        <label className="block text-sm font-medium text-gray-700 mb-1.5">Çalışma Periyodu</label>
        <select
          value={CRON_PRESETS.some((p) => p.value === cronValue) ? cronValue : "__custom"}
          onChange={(e) => {
            if (e.target.value !== "__custom") setValue("Reminder:CronExpression", e.target.value);
          }}
          className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
        >
          {CRON_PRESETS.map((p) => (
            <option key={p.value} value={p.value}>
              {p.label}
            </option>
          ))}
          {!CRON_PRESETS.some((p) => p.value === cronValue) && (
            <option value="__custom">Özel: {cronValue}</option>
          )}
        </select>
        <p className="mt-1 text-xs text-gray-500">Cron ifadesi: {cronValue || "-"}</p>
      </div>
      <InputField
        label="Bekleme Eşiği (Saat)"
        value={getValue("Reminder:DraftThresholdHours")}
        onChange={(v) => setValue("Reminder:DraftThresholdHours", v)}
        type="number"
        placeholder="24"
        help="Bu süreden uzun süredir taslak durumda bekleyen iş emirleri hatırlatılır."
      />
      <InputField
        label="Alıcılar"
        value={getValue("Reminder:Recipients")}
        onChange={(v) => setValue("Reminder:Recipients", v)}
        placeholder="ali@orko.com, veli@orko.com"
        help="Birden fazla alıcıyı virgülle ayırın."
      />
      <InputField
        label="E-posta Konusu"
        value={getValue("Reminder:Subject")}
        onChange={(v) => setValue("Reminder:Subject", v)}
        placeholder="Taslak İş Emirleri Hatırlatması ({Count} adet)"
        help="Kullanılabilir değişkenler: {Count}"
      />
      <TextAreaField
        label="E-posta Şablonu (HTML)"
        value={getValue("Reminder:Template")}
        onChange={(v) => setValue("Reminder:Template", v)}
        rows={10}
        help="Kullanılabilir değişkenler: {Count}, {TableRows}. Boş bırakılırsa varsayılan şablon kullanılır."
      />
    </div>
  );
}
