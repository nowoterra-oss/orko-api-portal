import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

// Statu renkleri
export const statusColors: Record<string, string> = {
  Taslak: "bg-gray-100 text-gray-800",
  Hazirlaniyor: "bg-blue-100 text-blue-800",
  EvrimeGonderildi: "bg-purple-100 text-purple-800",
  TescilBekleniyor: "bg-yellow-100 text-yellow-800",
  TescilEdildi: "bg-green-100 text-green-800",
  RedEdildi: "bg-red-100 text-red-800",
  DuzeltmeGerekli: "bg-orange-100 text-orange-800",
  Kapandi: "bg-gray-300 text-gray-900",
};

// Statu Turkce isimleri
export const statusLabels: Record<string, string> = {
  Taslak: "Taslak",
  Hazirlaniyor: "Hazırlanıyor",
  EvrimeGonderildi: "Evrim'e Gönderildi",
  TescilBekleniyor: "Tescil Bekleniyor",
  TescilEdildi: "Tescil Edildi",
  RedEdildi: "Reddedildi",
  DuzeltmeGerekli: "Düzeltme Gerekli",
  Kapandi: "Kapandı",
};

// Tip Turkce isimleri
export const typeLabels: Record<string, string> = {
  Import: "İthalat",
  Export: "İhracat",
};

// Tarih formatlama
export function formatDate(dateStr: string): string {
  return new Date(dateStr).toLocaleDateString("tr-TR", {
    year: "numeric",
    month: "2-digit",
    day: "2-digit",
    hour: "2-digit",
    minute: "2-digit",
  });
}

// Dosya boyutu formatlama
export function formatFileSize(bytes: number): string {
  if (bytes < 1024) return `${bytes} B`;
  if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
  return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
}
