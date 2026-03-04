"use client";

import { useEffect, useState, useRef } from "react";
import { useParams, useRouter } from "next/navigation";
import { declarationService } from "@/services/declarationService";
import { DeclarationDetail } from "@/lib/types";
import { PageHeader } from "@/components/shared/PageHeader";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { ArrowLeft, Save, Send, Plus, Trash2, ChevronDown, ChevronUp, FlaskConical, Upload } from "lucide-react";
import { useAuth } from "@/contexts/AuthContext";
import { hasFeature } from "@/lib/permissions";

// =====================
// TYPES
// =====================
interface DeclarationKalem {
  detayNo?: number;
  gtipNo?: string;
  menseiUlke?: string;
  brutAgirlik?: number;
  netAgirlik?: number;
  istatistikiMiktar?: number;
  kalemFiyati?: number;
  miktar?: number;
  miktarBirimi?: string;
  doviz?: string;
  ticariTanim?: string;
  faturaNo?: string;
  faturaTarihi?: string;
  cinsi?: string;
  adedi?: string;
  navlunMiktari?: number;
  sigortaMiktari?: number;
  birimFiyat?: number;
  kdvOrani?: string;
  kalemNotu?: string;
}

interface DeclarationFormData {
  // Dosya
  refId?: string;
  dosyaTipi?: string;
  dosyaNo?: string;
  referansNo?: string;
  ihracat?: boolean;
  olusturanKullanici?: string;
  dosyaTarihi?: string;
  // Musteri
  musteriVergi?: string;
  musteriUnvani?: string;
  musteriAccountNumber?: string;
  // Beyanname
  rejimKodu?: string;
  gumruk?: string;
  basitlestirilmisUsul?: string;
  // Ulke
  ilkUlke?: string;
  gidecegiUlke?: string;
  sevkUlkesi?: string;
  ticaretUlkesi?: string;
  cikisUlkesi?: string;
  varisUlkesi?: string;
  // Tasima
  teslimSekli?: string;
  teslimYeri?: string;
  konteyner?: string;
  sinirdakiTasimaSekli?: string;
  sinirdakiAracinKimligi?: string;
  sinirdakiAracinTipi?: string;
  sinirdakiAracinUlkesi?: string;
  cikistakiAracinTipi?: string;
  cikistakiAracinKimligi?: string;
  cikistakiAracinUlkesi?: string;
  // Mali
  toplamFatura?: number;
  toplamFaturaDovizi?: string;
  toplamNavlun?: number;
  toplamNavlunDovizi?: string;
  toplamSigorta?: number;
  toplamSigortaDovizi?: string;
  toplamBrutAgirlik?: number;
  toplamNetAgirlik?: number;
  // Kap/Yuk
  kapAdedi?: string;
  yukBelgeleriSayisi?: number;
  yuklemeBosaltmaYeri?: string;
  // Diger
  odemeSekli?: string;
  bankaKodu?: string;
  isleminNiteligi?: string;
  aciklamalar?: string;
  tahminiVarisTarihi?: string;
  beyanSahibiUnvan?: string;
  beyanSahibiVergiNo?: string;
  musavirVergiNo?: string;
  // Kalemler
  kalemler: DeclarationKalem[];
}

const emptyKalem: DeclarationKalem = {
  detayNo: undefined,
  gtipNo: "",
  menseiUlke: "",
  brutAgirlik: undefined,
  netAgirlik: undefined,
  kalemFiyati: undefined,
  miktar: undefined,
  miktarBirimi: "",
  doviz: "",
  ticariTanim: "",
  faturaNo: "",
  faturaTarihi: "",
  birimFiyat: undefined,
};

const defaultForm: DeclarationFormData = {
  kalemler: [],
};

const TABS = [
  { id: "genel", label: "Genel Bilgiler" },
  { id: "musteri", label: "Müşteri" },
  { id: "ulke", label: "Ülke Bilgileri" },
  { id: "tasima", label: "Taşıma" },
  { id: "mali", label: "Mali Bilgiler" },
  { id: "diger", label: "Diğer" },
  { id: "kalemler", label: "Kalemler" },
] as const;

type TabId = (typeof TABS)[number]["id"];

// =====================
// FORM INPUT COMPONENTS
// =====================

function FormField({
  label,
  children,
  span = 1,
}: {
  label: string;
  children: React.ReactNode;
  span?: number;
}) {
  return (
    <div className={span === 2 ? "sm:col-span-2" : ""}>
      <label className="block text-sm font-medium text-gray-700 mb-1.5">{label}</label>
      {children}
    </div>
  );
}

function TextInput({
  value,
  onChange,
  placeholder,
  maxLength,
  disabled,
}: {
  value?: string;
  onChange: (v: string) => void;
  placeholder?: string;
  maxLength?: number;
  disabled?: boolean;
}) {
  return (
    <input
      type="text"
      value={value || ""}
      onChange={(e) => onChange(e.target.value)}
      placeholder={placeholder}
      maxLength={maxLength}
      disabled={disabled}
      className="input-base"
    />
  );
}

function NumberInput({
  value,
  onChange,
  placeholder,
  disabled,
}: {
  value?: number;
  onChange: (v: number | undefined) => void;
  placeholder?: string;
  disabled?: boolean;
}) {
  return (
    <input
      type="number"
      step="any"
      value={value ?? ""}
      onChange={(e) => onChange(e.target.value ? parseFloat(e.target.value) : undefined)}
      placeholder={placeholder}
      disabled={disabled}
      className="input-base"
    />
  );
}

function SelectInput({
  value,
  onChange,
  options,
  placeholder,
  disabled,
}: {
  value?: string;
  onChange: (v: string) => void;
  options: { value: string; label: string }[];
  placeholder?: string;
  disabled?: boolean;
}) {
  return (
    <select
      value={value || ""}
      onChange={(e) => onChange(e.target.value)}
      disabled={disabled}
      className="input-base"
    >
      <option value="">{placeholder || "Seçiniz..."}</option>
      {options.map((o) => (
        <option key={o.value} value={o.value}>
          {o.label}
        </option>
      ))}
    </select>
  );
}

// =====================
// PAGE
// =====================

export default function DeclarationEditPage() {
  const params = useParams();
  const router = useRouter();
  const id = params.id as string;
  const { user } = useAuth();
  const canEdit = user ? hasFeature(user.role, "declaration:edit") : false;

  const [declaration, setDeclaration] = useState<DeclarationDetail | null>(null);
  const [form, setForm] = useState<DeclarationFormData>(defaultForm);
  const [activeTab, setActiveTab] = useState<TabId>("genel");
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [sending, setSending] = useState(false);
  const [uploading, setUploading] = useState(false);
  const fileInputRef = useRef<HTMLInputElement>(null);

  useEffect(() => {
    declarationService.get(id).then((d) => {
      setDeclaration(d);
      if (d.declarationData) {
        try {
          const parsed = JSON.parse(d.declarationData);
          setForm({ ...defaultForm, ...parsed, kalemler: parsed.kalemler || [] });
        } catch {
          setForm(defaultForm);
        }
      }
      setLoading(false);
    }).catch(() => setLoading(false));
  }, [id]);

  const updateForm = (key: keyof DeclarationFormData, value: any) => {
    setForm((prev) => ({ ...prev, [key]: value }));
  };

  const updateKalem = (index: number, key: keyof DeclarationKalem, value: any) => {
    setForm((prev) => {
      const kalemler = [...prev.kalemler];
      kalemler[index] = { ...kalemler[index], [key]: value };
      return { ...prev, kalemler };
    });
  };

  const addKalem = () => {
    setForm((prev) => ({
      ...prev,
      kalemler: [...prev.kalemler, { ...emptyKalem, detayNo: prev.kalemler.length + 1 }],
    }));
  };

  const removeKalem = (index: number) => {
    if (!confirm("Bu kalemi silmek istediğinize emin misiniz?")) return;
    setForm((prev) => ({
      ...prev,
      kalemler: prev.kalemler.filter((_, i) => i !== index),
    }));
  };

  const handleSave = async () => {
    setSaving(true);
    try {
      const jsonData = JSON.stringify(form);
      const result = await declarationService.update(id, jsonData);
      if (result.success) {
        alert("Beyanname kaydedildi!");
      } else {
        alert("Hata: " + result.message);
      }
    } catch (err: any) {
      alert("Kaydetme hatası: " + (err.message || "Bilinmeyen hata"));
    } finally {
      setSaving(false);
    }
  };

  const handleSendToEvrim = async () => {
    if (!confirm("Beyannameyi Evrim'e göndermek istediğinize emin misiniz?\nBu işlem geri alınamaz."))
      return;

    // Önce kaydet
    const jsonData = JSON.stringify(form);
    await declarationService.update(id, jsonData);

    setSending(true);
    try {
      const result = await declarationService.sendToEvrim(id);
      if (result.success) {
        alert("Evrim'e başarıyla gönderildi!");
        router.push(`/work-orders/${declaration?.fileNumber}`);
      } else {
        alert("Evrim hatası: " + result.message);
      }
    } catch (err: any) {
      alert("Gönderim hatası: " + (err.message || "Bilinmeyen hata"));
    } finally {
      setSending(false);
    }
  };

  const handleFileSelected = async (e: React.ChangeEvent<HTMLInputElement>) => {
    const files = e.target.files;
    if (!files || files.length === 0) return;

    // Reset input so same files can be re-selected
    e.target.value = "";

    // Validate all files
    for (const file of Array.from(files)) {
      const ext = file.name.split(".").pop()?.toLowerCase();
      if (ext !== "json" && ext !== "xml") {
        alert(`Desteklenmeyen dosya formatı: ${file.name}. Lütfen .json veya .xml dosyası seçin.`);
        return;
      }
    }

    setUploading(true);
    try {
      // Read all files
      const fileEntries = await Promise.all(
        Array.from(files).map(async (file) => ({
          fileName: file.name,
          content: await file.text(),
          ext: file.name.split(".").pop()?.toLowerCase() as string,
        }))
      );

      // Detect main XML (contains <Gelen) vs additional files
      let mainFile = fileEntries[0];
      const additionalFiles: { fileName: string; fileContent: string }[] = [];

      if (fileEntries.length > 1) {
        // Find the main beyanname XML (<Gelen root element)
        const gelenFile = fileEntries.find((f) => f.content.includes("<Gelen"));
        if (gelenFile) {
          mainFile = gelenFile;
          for (const f of fileEntries) {
            if (f !== gelenFile) {
              additionalFiles.push({ fileName: f.fileName, fileContent: f.content });
            }
          }
        } else {
          // No <Gelen found — use first file as main, rest as additional
          for (let i = 1; i < fileEntries.length; i++) {
            additionalFiles.push({ fileName: fileEntries[i].fileName, fileContent: fileEntries[i].content });
          }
        }
      }

      const fileFormat = mainFile.ext as "json" | "xml";
      const result = await declarationService.parseFile(
        id,
        mainFile.content,
        fileFormat,
        additionalFiles.length > 0 ? additionalFiles : undefined
      );

      if (result.success && result.data) {
        const parsed = result.data as Record<string, any>;
        setForm({
          ...defaultForm,
          ...parsed,
          kalemler: parsed.kalemler || [],
        });
        setActiveTab("genel");
        const msg = additionalFiles.length > 0
          ? `${fileEntries.length} dosya başarıyla yüklendi! Lütfen verileri kontrol edip 'Evrim'e Gönder' butonunu kullanın.`
          : "Dosya başarıyla yüklendi! Lütfen verileri kontrol edip 'Evrim'e Gönder' butonunu kullanın.";
        alert(msg);
      } else {
        alert("Parse hatası: " + result.message);
      }
    } catch (err: any) {
      alert("Yükleme hatası: " + (err.response?.data?.message || err.message || "Bilinmeyen hata"));
    } finally {
      setUploading(false);
    }
  };

  const fillTestData = () => {
    if (!declaration) return;
    const isExport = declaration.declarationType === "Export";
    const fileNumber = declaration.fileNumber || "ORK-TEST";
    const today = new Date().toISOString().slice(0, 10);

    if (isExport) {
      setForm({
        refId: "ORK01",
        dosyaTipi: "H",
        dosyaNo: fileNumber,
        referansNo: fileNumber,
        ihracat: true,
        musteriVergi: "5678901234",
        musteriUnvani: "XYZ Tekstil Ltd. Sti.",
        rejimKodu: "1000",
        gumruk: "340300",
        gidecegiUlke: "GB",
        sevkUlkesi: "TR",
        ticaretUlkesi: "GB",
        cikisUlkesi: "TR",
        teslimSekli: "FOB",
        teslimYeri: "Mersin",
        toplamFatura: 48500,
        toplamFaturaDovizi: "USD",
        toplamBrutAgirlik: 12000,
        toplamNetAgirlik: 11500,
        kapAdedi: "25",
        beyanSahibiUnvan: "XYZ Tekstil Ltd. Sti.",
        beyanSahibiVergiNo: "5678901234",
        musavirVergiNo: "9876543210",
        kalemler: [
          {
            detayNo: 1,
            gtipNo: "5209.42.00.00.00",
            menseiUlke: "TR",
            brutAgirlik: 12000,
            netAgirlik: 11500,
            kalemFiyati: 48500,
            miktar: 25000,
            miktarBirimi: "MTR",
            doviz: "USD",
            ticariTanim: "Pamuklu denim kumas, indigo boyali",
            faturaNo: "EXP-2026-089",
            faturaTarihi: today,
            birimFiyat: 1.94,
            kdvOrani: "0",
          },
        ],
      });
    } else {
      // Basarili curl testiyle birebir ayni minimal veri
      setForm({
        refId: "ORK01",
        dosyaTipi: "T",
        dosyaNo: fileNumber,
        referansNo: fileNumber,
        musteriVergi: "1234567890",
        musteriUnvani: "Test Firma A.S.",
        rejimKodu: "4000",
        gumruk: "340100",
        ilkUlke: "DE",
        sevkUlkesi: "DE",
        ticaretUlkesi: "DE",
        varisUlkesi: "TR",
        teslimSekli: "CIF",
        teslimYeri: "Istanbul",
        toplamFatura: 10000.0,
        toplamFaturaDovizi: "EUR",
        toplamBrutAgirlik: 500.0,
        toplamNetAgirlik: 400.0,
        kapAdedi: "1",
        kalemler: [
          {
            detayNo: 1,
            gtipNo: "8458.11.20.00.00",
            menseiUlke: "DE",
            brutAgirlik: 500.0,
            netAgirlik: 400.0,
            kalemFiyati: 10000.0,
            miktar: 1,
            miktarBirimi: "C62",
            doviz: "EUR",
            ticariTanim: "Test Urun",
            birimFiyat: 10000.0,
          },
        ],
      });
    }
  };

  const isReadOnly = declaration?.sentToEvrim || !canEdit;

  if (loading) return <LoadingSpinner />;
  if (!declaration) return <div className="p-8 text-center text-gray-500">Beyanname bulunamadı.</div>;

  // =====================
  // TAB CONTENTS
  // =====================

  const renderGenelTab = () => (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
      <FormField label="Dosya Tipi">
        <SelectInput
          value={form.dosyaTipi}
          onChange={(v) => updateForm("dosyaTipi", v)}
          disabled={isReadOnly}
          options={[
            { value: "T", label: "İthalat" },
            { value: "H", label: "İhracat" },
            { value: "T-ANT", label: "Antrepo" },
          ]}
        />
      </FormField>
      <FormField label="Referans No">
        <TextInput value={form.referansNo} onChange={(v) => updateForm("referansNo", v)} disabled={isReadOnly} maxLength={100} placeholder="Firma referans numarası" />
      </FormField>
      <FormField label="Dosya No">
        <TextInput value={form.dosyaNo} onChange={(v) => updateForm("dosyaNo", v)} disabled={isReadOnly} maxLength={15} />
      </FormField>
      <FormField label="Ref ID">
        <TextInput value={form.refId} onChange={(v) => updateForm("refId", v)} disabled={isReadOnly} maxLength={5} placeholder="Müşavir referansı" />
      </FormField>
      <FormField label="Rejim Kodu">
        <TextInput value={form.rejimKodu} onChange={(v) => updateForm("rejimKodu", v)} disabled={isReadOnly} maxLength={4} placeholder="Ör: 4000" />
      </FormField>
      <FormField label="Gümrük Kodu">
        <TextInput value={form.gumruk} onChange={(v) => updateForm("gumruk", v)} disabled={isReadOnly} maxLength={10} placeholder="Ör: 340100" />
      </FormField>
      <FormField label="Dosya Tarihi">
        <input type="datetime-local" value={form.dosyaTarihi?.slice(0, 16) || ""} onChange={(e) => updateForm("dosyaTarihi", e.target.value ? e.target.value + ":00" : undefined)} disabled={isReadOnly} className="input-base" />
      </FormField>
      <FormField label="Basitleştirilmiş Usul">
        <TextInput value={form.basitlestirilmisUsul} onChange={(v) => updateForm("basitlestirilmisUsul", v)} disabled={isReadOnly} maxLength={2} />
      </FormField>
      <FormField label="İşlemin Niteliği">
        <TextInput value={form.isleminNiteligi} onChange={(v) => updateForm("isleminNiteligi", v)} disabled={isReadOnly} maxLength={3} />
      </FormField>
      <FormField label="Oluşturan Kullanıcı">
        <TextInput value={form.olusturanKullanici} onChange={(v) => updateForm("olusturanKullanici", v)} disabled={isReadOnly} maxLength={10} />
      </FormField>
      <FormField label="Açıklamalar" span={2}>
        <textarea value={form.aciklamalar || ""} onChange={(e) => updateForm("aciklamalar", e.target.value)} disabled={isReadOnly} maxLength={250} rows={2} className="input-base" placeholder="Açıklama giriniz..." />
      </FormField>
    </div>
  );

  const renderMusteriTab = () => (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
      <FormField label="Müşteri VKN">
        <TextInput value={form.musteriVergi} onChange={(v) => updateForm("musteriVergi", v)} disabled={isReadOnly} maxLength={15} placeholder="Vergi kimlik numarası" />
      </FormField>
      <FormField label="Müşteri Ünvanı" span={2}>
        <TextInput value={form.musteriUnvani} onChange={(v) => updateForm("musteriUnvani", v)} disabled={isReadOnly} maxLength={500} placeholder="Müşteri ünvanı" />
      </FormField>
      <FormField label="Müşteri Account No">
        <TextInput value={form.musteriAccountNumber} onChange={(v) => updateForm("musteriAccountNumber", v)} disabled={isReadOnly} maxLength={50} />
      </FormField>
      <FormField label="Beyan Sahibi Ünvan">
        <TextInput value={form.beyanSahibiUnvan} onChange={(v) => updateForm("beyanSahibiUnvan", v)} disabled={isReadOnly} maxLength={100} />
      </FormField>
      <FormField label="Beyan Sahibi VKN">
        <TextInput value={form.beyanSahibiVergiNo} onChange={(v) => updateForm("beyanSahibiVergiNo", v)} disabled={isReadOnly} maxLength={15} />
      </FormField>
      <FormField label="Müşavir VKN">
        <TextInput value={form.musavirVergiNo} onChange={(v) => updateForm("musavirVergiNo", v)} disabled={isReadOnly} maxLength={15} />
      </FormField>
    </div>
  );

  const renderUlkeTab = () => (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
      <FormField label="İlk Ülke">
        <TextInput value={form.ilkUlke} onChange={(v) => updateForm("ilkUlke", v)} disabled={isReadOnly} maxLength={4} placeholder="Ör: DE" />
      </FormField>
      <FormField label="Gideceği Ülke">
        <TextInput value={form.gidecegiUlke} onChange={(v) => updateForm("gidecegiUlke", v)} disabled={isReadOnly} maxLength={4} />
      </FormField>
      <FormField label="Sevk Ülkesi">
        <TextInput value={form.sevkUlkesi} onChange={(v) => updateForm("sevkUlkesi", v)} disabled={isReadOnly} maxLength={4} />
      </FormField>
      <FormField label="Ticaret Ülkesi">
        <TextInput value={form.ticaretUlkesi} onChange={(v) => updateForm("ticaretUlkesi", v)} disabled={isReadOnly} maxLength={4} />
      </FormField>
      <FormField label="Çıkış Ülkesi">
        <TextInput value={form.cikisUlkesi} onChange={(v) => updateForm("cikisUlkesi", v)} disabled={isReadOnly} maxLength={4} />
      </FormField>
      <FormField label="Varış Ülkesi">
        <TextInput value={form.varisUlkesi} onChange={(v) => updateForm("varisUlkesi", v)} disabled={isReadOnly} maxLength={4} />
      </FormField>
    </div>
  );

  const renderTasimaTab = () => (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
      <FormField label="Teslim Şekli">
        <SelectInput value={form.teslimSekli} onChange={(v) => updateForm("teslimSekli", v)} disabled={isReadOnly}
          options={[
            { value: "CIF", label: "CIF" }, { value: "CFR", label: "CFR" }, { value: "FOB", label: "FOB" },
            { value: "EXW", label: "EXW" }, { value: "FCA", label: "FCA" }, { value: "CPT", label: "CPT" },
            { value: "CIP", label: "CIP" }, { value: "DAP", label: "DAP" }, { value: "DPU", label: "DPU" },
            { value: "DDP", label: "DDP" }, { value: "FAS", label: "FAS" },
          ]}
        />
      </FormField>
      <FormField label="Teslim Yeri">
        <TextInput value={form.teslimYeri} onChange={(v) => updateForm("teslimYeri", v)} disabled={isReadOnly} maxLength={30} />
      </FormField>
      <FormField label="Konteyner">
        <SelectInput value={form.konteyner} onChange={(v) => updateForm("konteyner", v)} disabled={isReadOnly}
          options={[{ value: "0", label: "Hayır" }, { value: "1", label: "Evet" }]}
        />
      </FormField>
      <FormField label="Sınırdaki Taşıma Şekli">
        <TextInput value={form.sinirdakiTasimaSekli} onChange={(v) => updateForm("sinirdakiTasimaSekli", v)} disabled={isReadOnly} maxLength={2} />
      </FormField>
      <FormField label="Sınırdaki Araç Kimliği">
        <TextInput value={form.sinirdakiAracinKimligi} onChange={(v) => updateForm("sinirdakiAracinKimligi", v)} disabled={isReadOnly} maxLength={50} />
      </FormField>
      <FormField label="Sınırdaki Araç Tipi">
        <TextInput value={form.sinirdakiAracinTipi} onChange={(v) => updateForm("sinirdakiAracinTipi", v)} disabled={isReadOnly} maxLength={4} />
      </FormField>
      <FormField label="Sınırdaki Araç Ülkesi">
        <TextInput value={form.sinirdakiAracinUlkesi} onChange={(v) => updateForm("sinirdakiAracinUlkesi", v)} disabled={isReadOnly} maxLength={4} />
      </FormField>
      <FormField label="Çıkıştaki Araç Tipi">
        <TextInput value={form.cikistakiAracinTipi} onChange={(v) => updateForm("cikistakiAracinTipi", v)} disabled={isReadOnly} maxLength={2} />
      </FormField>
      <FormField label="Çıkıştaki Araç Kimliği">
        <TextInput value={form.cikistakiAracinKimligi} onChange={(v) => updateForm("cikistakiAracinKimligi", v)} disabled={isReadOnly} maxLength={50} />
      </FormField>
      <FormField label="Çıkıştaki Araç Ülkesi">
        <TextInput value={form.cikistakiAracinUlkesi} onChange={(v) => updateForm("cikistakiAracinUlkesi", v)} disabled={isReadOnly} maxLength={4} />
      </FormField>
      <FormField label="Kap Adedi">
        <TextInput value={form.kapAdedi} onChange={(v) => updateForm("kapAdedi", v)} disabled={isReadOnly} maxLength={10} />
      </FormField>
      <FormField label="Yük Belgeleri Sayısı">
        <NumberInput value={form.yukBelgeleriSayisi} onChange={(v) => updateForm("yukBelgeleriSayisi", v)} disabled={isReadOnly} />
      </FormField>
      <FormField label="Yükleme/Boşaltma Yeri">
        <TextInput value={form.yuklemeBosaltmaYeri} onChange={(v) => updateForm("yuklemeBosaltmaYeri", v)} disabled={isReadOnly} maxLength={50} />
      </FormField>
    </div>
  );

  const renderMaliTab = () => (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
      <FormField label="Toplam Fatura">
        <NumberInput value={form.toplamFatura} onChange={(v) => updateForm("toplamFatura", v)} disabled={isReadOnly} placeholder="0.00" />
      </FormField>
      <FormField label="Fatura Dövizi">
        <SelectInput value={form.toplamFaturaDovizi} onChange={(v) => updateForm("toplamFaturaDovizi", v)} disabled={isReadOnly}
          options={[
            { value: "USD", label: "USD" }, { value: "EUR", label: "EUR" }, { value: "GBP", label: "GBP" },
            { value: "TRY", label: "TRY" }, { value: "CHF", label: "CHF" }, { value: "JPY", label: "JPY" },
            { value: "CNY", label: "CNY" },
          ]}
        />
      </FormField>
      <div />
      <FormField label="Toplam Navlun">
        <NumberInput value={form.toplamNavlun} onChange={(v) => updateForm("toplamNavlun", v)} disabled={isReadOnly} placeholder="0.00" />
      </FormField>
      <FormField label="Navlun Dövizi">
        <SelectInput value={form.toplamNavlunDovizi} onChange={(v) => updateForm("toplamNavlunDovizi", v)} disabled={isReadOnly}
          options={[
            { value: "USD", label: "USD" }, { value: "EUR", label: "EUR" }, { value: "GBP", label: "GBP" },
            { value: "TRY", label: "TRY" },
          ]}
        />
      </FormField>
      <div />
      <FormField label="Toplam Sigorta">
        <NumberInput value={form.toplamSigorta} onChange={(v) => updateForm("toplamSigorta", v)} disabled={isReadOnly} placeholder="0.00" />
      </FormField>
      <FormField label="Sigorta Dövizi">
        <SelectInput value={form.toplamSigortaDovizi} onChange={(v) => updateForm("toplamSigortaDovizi", v)} disabled={isReadOnly}
          options={[
            { value: "USD", label: "USD" }, { value: "EUR", label: "EUR" }, { value: "GBP", label: "GBP" },
            { value: "TRY", label: "TRY" },
          ]}
        />
      </FormField>
      <div />
      <FormField label="Toplam Brüt Ağırlık (Kg)">
        <NumberInput value={form.toplamBrutAgirlik} onChange={(v) => updateForm("toplamBrutAgirlik", v)} disabled={isReadOnly} />
      </FormField>
      <FormField label="Toplam Net Ağırlık (Kg)">
        <NumberInput value={form.toplamNetAgirlik} onChange={(v) => updateForm("toplamNetAgirlik", v)} disabled={isReadOnly} />
      </FormField>
    </div>
  );

  const renderDigerTab = () => (
    <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-5">
      <FormField label="Ödeme Şekli">
        <TextInput value={form.odemeSekli} onChange={(v) => updateForm("odemeSekli", v)} disabled={isReadOnly} maxLength={15} />
      </FormField>
      <FormField label="Banka Kodu">
        <TextInput value={form.bankaKodu} onChange={(v) => updateForm("bankaKodu", v)} disabled={isReadOnly} maxLength={12} />
      </FormField>
      <FormField label="Tahmini Varış Tarihi">
        <input type="date" value={form.tahminiVarisTarihi || ""} onChange={(e) => updateForm("tahminiVarisTarihi", e.target.value)} disabled={isReadOnly} className="input-base" />
      </FormField>
    </div>
  );

  const renderKalemlerTab = () => (
    <div>
      <div className="flex items-center justify-between mb-4">
        <h3 className="text-sm font-semibold text-gray-700">
          Kalem Listesi ({form.kalemler.length} kalem)
        </h3>
        {!isReadOnly && (
          <button onClick={addKalem} className="flex items-center gap-1.5 px-3 py-1.5 bg-blue-600 text-white text-sm rounded-lg hover:bg-blue-700 transition-colors">
            <Plus className="w-4 h-4" /> Kalem Ekle
          </button>
        )}
      </div>

      {form.kalemler.length === 0 ? (
        <div className="text-center py-12 text-gray-400 bg-gray-50 rounded-xl border-2 border-dashed border-gray-200">
          <p className="text-sm">Henüz kalem eklenmedi.</p>
          {!isReadOnly && (
            <button onClick={addKalem} className="mt-2 text-sm text-blue-600 hover:text-blue-700">
              + İlk kalemi ekle
            </button>
          )}
        </div>
      ) : (
        <div className="space-y-4">
          {form.kalemler.map((kalem, idx) => (
            <KalemCard
              key={idx}
              index={idx}
              kalem={kalem}
              onChange={updateKalem}
              onRemove={removeKalem}
              readOnly={isReadOnly}
            />
          ))}
        </div>
      )}
    </div>
  );

  return (
    <>
      <PageHeader
        title={`Beyanname Düzenle — ${declaration.fileNumber}`}
        description={isReadOnly ? "Bu beyanname Evrim'e gönderilmiş, düzenlenemez." : "Evrim'e gönderilecek beyanname verilerini doldurun."}
      />

      {/* Top bar: back + actions */}
      <div className="flex items-center justify-between mb-6">
        <button onClick={() => router.back()} className="flex items-center gap-1 text-sm text-gray-600 hover:text-gray-900">
          <ArrowLeft className="w-4 h-4" /> Geri
        </button>
        {!isReadOnly && (
          <div className="flex gap-2">
            {process.env.NODE_ENV === "development" && (
              <button onClick={fillTestData} className="flex items-center gap-1.5 px-4 py-2 bg-amber-500 text-white text-sm rounded-lg hover:bg-amber-600 transition-colors">
                <FlaskConical className="w-4 h-4" /> Test Verisi Doldur
              </button>
            )}
            <button onClick={handleSave} disabled={saving} className="flex items-center gap-1.5 px-4 py-2 bg-gray-800 text-white text-sm rounded-lg hover:bg-gray-900 transition-colors disabled:opacity-50">
              <Save className="w-4 h-4" /> {saving ? "Kaydediliyor..." : "Kaydet"}
            </button>
            <button onClick={handleSendToEvrim} disabled={sending} className="flex items-center gap-1.5 px-4 py-2 bg-emerald-600 text-white text-sm rounded-lg hover:bg-emerald-700 transition-colors disabled:opacity-50">
              <Send className="w-4 h-4" /> {sending ? "Gönderiliyor..." : "Evrim'e Gönder"}
            </button>
            <input
              ref={fileInputRef}
              type="file"
              accept=".json,.xml"
              multiple
              onChange={handleFileSelected}
              className="hidden"
            />
            <button onClick={() => fileInputRef.current?.click()} disabled={uploading} className="flex items-center gap-1.5 px-4 py-2 bg-violet-600 text-white text-sm rounded-lg hover:bg-violet-700 transition-colors disabled:opacity-50">
              <Upload className="w-4 h-4" /> {uploading ? "Yükleniyor..." : "Dosyalardan Yükle"}
            </button>
          </div>
        )}
      </div>

      {/* Tabs */}
      <div className="border-b border-gray-200 mb-6 overflow-x-auto">
        <nav className="flex gap-0 -mb-px">
          {TABS.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`whitespace-nowrap px-4 py-3 text-sm font-medium border-b-2 transition-colors ${
                activeTab === tab.id
                  ? "border-blue-600 text-blue-600"
                  : "border-transparent text-gray-500 hover:text-gray-700 hover:border-gray-300"
              }`}
            >
              {tab.label}
              {tab.id === "kalemler" && form.kalemler.length > 0 && (
                <span className="ml-1.5 px-1.5 py-0.5 text-xs bg-blue-100 text-blue-700 rounded-full">
                  {form.kalemler.length}
                </span>
              )}
            </button>
          ))}
        </nav>
      </div>

      {/* Tab Content */}
      <div className="card p-6">
        {activeTab === "genel" && renderGenelTab()}
        {activeTab === "musteri" && renderMusteriTab()}
        {activeTab === "ulke" && renderUlkeTab()}
        {activeTab === "tasima" && renderTasimaTab()}
        {activeTab === "mali" && renderMaliTab()}
        {activeTab === "diger" && renderDigerTab()}
        {activeTab === "kalemler" && renderKalemlerTab()}
      </div>

      {/* Bottom actions (mobile friendly) */}
      {!isReadOnly && (
        <div className="mt-6 flex gap-2 justify-end sm:hidden">
          <button onClick={handleSave} disabled={saving} className="flex-1 flex items-center justify-center gap-1.5 px-4 py-2.5 bg-gray-800 text-white text-sm rounded-lg">
            <Save className="w-4 h-4" /> Kaydet
          </button>
          <button onClick={handleSendToEvrim} disabled={sending} className="flex-1 flex items-center justify-center gap-1.5 px-4 py-2.5 bg-emerald-600 text-white text-sm rounded-lg">
            <Send className="w-4 h-4" /> Gönder
          </button>
        </div>
      )}
    </>
  );
}

// =====================
// KALEM CARD COMPONENT
// =====================

function KalemCard({
  index,
  kalem,
  onChange,
  onRemove,
  readOnly,
}: {
  index: number;
  kalem: DeclarationKalem;
  onChange: (index: number, key: keyof DeclarationKalem, value: any) => void;
  onRemove: (index: number) => void;
  readOnly: boolean;
}) {
  const [expanded, setExpanded] = useState(true);

  return (
    <div className="card overflow-hidden">
      {/* Header */}
      <div
        className="flex items-center justify-between px-4 py-3 bg-gray-50 cursor-pointer"
        onClick={() => setExpanded(!expanded)}
      >
        <div className="flex items-center gap-3">
          <span className="w-7 h-7 flex items-center justify-center bg-blue-100 text-blue-700 text-xs font-bold rounded-full">
            {index + 1}
          </span>
          <span className="text-sm font-medium text-gray-700">
            {kalem.gtipNo || "GTIP girilmedi"} — {kalem.ticariTanim || "Tanım girilmedi"}
          </span>
        </div>
        <div className="flex items-center gap-2">
          {!readOnly && (
            <button
              onClick={(e) => { e.stopPropagation(); onRemove(index); }}
              className="p-1 text-red-400 hover:text-red-600 hover:bg-red-50 rounded"
            >
              <Trash2 className="w-4 h-4" />
            </button>
          )}
          {expanded ? <ChevronUp className="w-4 h-4 text-gray-400" /> : <ChevronDown className="w-4 h-4 text-gray-400" />}
        </div>
      </div>

      {/* Body */}
      {expanded && (
        <div className="p-5 grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4">
          <FormField label="GTIP No">
            <TextInput value={kalem.gtipNo} onChange={(v) => onChange(index, "gtipNo", v)} disabled={readOnly} maxLength={16} placeholder="8544.42.90.00.00" />
          </FormField>
          <FormField label="Ticari Tanım" span={2}>
            <TextInput value={kalem.ticariTanim} onChange={(v) => onChange(index, "ticariTanim", v)} disabled={readOnly} maxLength={250} placeholder="Ürün açıklaması" />
          </FormField>
          <FormField label="Menşe Ülke">
            <TextInput value={kalem.menseiUlke} onChange={(v) => onChange(index, "menseiUlke", v)} disabled={readOnly} maxLength={4} placeholder="CN" />
          </FormField>
          <FormField label="Miktar">
            <NumberInput value={kalem.miktar} onChange={(v) => onChange(index, "miktar", v)} disabled={readOnly} placeholder="0" />
          </FormField>
          <FormField label="Miktar Birimi">
            <TextInput value={kalem.miktarBirimi} onChange={(v) => onChange(index, "miktarBirimi", v)} disabled={readOnly} maxLength={15} placeholder="KGM, C62" />
          </FormField>
          <FormField label="Kalem Fiyatı">
            <NumberInput value={kalem.kalemFiyati} onChange={(v) => onChange(index, "kalemFiyati", v)} disabled={readOnly} placeholder="0.00" />
          </FormField>
          <FormField label="Döviz">
            <TextInput value={kalem.doviz} onChange={(v) => onChange(index, "doviz", v)} disabled={readOnly} maxLength={3} placeholder="USD" />
          </FormField>
          <FormField label="Brüt Ağırlık">
            <NumberInput value={kalem.brutAgirlik} onChange={(v) => onChange(index, "brutAgirlik", v)} disabled={readOnly} />
          </FormField>
          <FormField label="Net Ağırlık">
            <NumberInput value={kalem.netAgirlik} onChange={(v) => onChange(index, "netAgirlik", v)} disabled={readOnly} />
          </FormField>
          <FormField label="Birim Fiyat">
            <NumberInput value={kalem.birimFiyat} onChange={(v) => onChange(index, "birimFiyat", v)} disabled={readOnly} />
          </FormField>
          <FormField label="Fatura No">
            <TextInput value={kalem.faturaNo} onChange={(v) => onChange(index, "faturaNo", v)} disabled={readOnly} maxLength={15} />
          </FormField>
          <FormField label="Fatura Tarihi">
            <input type="date" value={kalem.faturaTarihi || ""} onChange={(e) => onChange(index, "faturaTarihi", e.target.value)} disabled={readOnly} className="input-base" />
          </FormField>
          <FormField label="Navlun">
            <NumberInput value={kalem.navlunMiktari} onChange={(v) => onChange(index, "navlunMiktari", v)} disabled={readOnly} />
          </FormField>
          <FormField label="Sigorta">
            <NumberInput value={kalem.sigortaMiktari} onChange={(v) => onChange(index, "sigortaMiktari", v)} disabled={readOnly} />
          </FormField>
          <FormField label="KDV Oranı">
            <TextInput value={kalem.kdvOrani} onChange={(v) => onChange(index, "kdvOrani", v)} disabled={readOnly} maxLength={2} placeholder="18" />
          </FormField>
          <FormField label="Kalem Notu" span={2}>
            <TextInput value={kalem.kalemNotu} onChange={(v) => onChange(index, "kalemNotu", v)} disabled={readOnly} maxLength={500} placeholder="Not..." />
          </FormField>
        </div>
      )}
    </div>
  );
}
