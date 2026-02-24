"use client";

import { useEffect, useState } from "react";
import { useAuth } from "@/contexts/AuthContext";
import { useRouter } from "next/navigation";
import { userService } from "@/services/userService";
import { UserListItem, CreateUserRequest, UpdateUserRequest } from "@/lib/auth-types";
import { PageHeader } from "@/components/shared/PageHeader";
import { LoadingSpinner } from "@/components/shared/LoadingSpinner";
import { Plus, Pencil, UserX, UserCheck, X } from "lucide-react";

const ROLES = [
  { value: "Admin", label: "Admin" },
  { value: "Operator", label: "Operatör" },
  { value: "Viewer", label: "Görüntüleyici" },
];

export default function UsersPage() {
  const { hasRole } = useAuth();
  const router = useRouter();
  const [users, setUsers] = useState<UserListItem[]>([]);
  const [loading, setLoading] = useState(true);
  const [modalOpen, setModalOpen] = useState(false);
  const [editUser, setEditUser] = useState<UserListItem | null>(null);

  // Form state
  const [formEmail, setFormEmail] = useState("");
  const [formFullName, setFormFullName] = useState("");
  const [formPassword, setFormPassword] = useState("");
  const [formRole, setFormRole] = useState("Operator");
  const [formError, setFormError] = useState("");
  const [formLoading, setFormLoading] = useState(false);

  useEffect(() => {
    if (!hasRole("Admin")) {
      router.replace("/");
      return;
    }
    loadUsers();
  }, [hasRole, router]);

  const loadUsers = async () => {
    try {
      const res = await userService.list();
      if (res.success && res.data) {
        setUsers(res.data);
      }
    } catch {
      // ignore
    } finally {
      setLoading(false);
    }
  };

  const openCreateModal = () => {
    setEditUser(null);
    setFormEmail("");
    setFormFullName("");
    setFormPassword("");
    setFormRole("Operator");
    setFormError("");
    setModalOpen(true);
  };

  const openEditModal = (user: UserListItem) => {
    setEditUser(user);
    setFormEmail(user.email);
    setFormFullName(user.fullName);
    setFormPassword("");
    setFormRole(user.role);
    setFormError("");
    setModalOpen(true);
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError("");
    setFormLoading(true);

    try {
      if (editUser) {
        const data: UpdateUserRequest = {
          fullName: formFullName,
          role: formRole,
        };
        if (formPassword) data.password = formPassword;

        const res = await userService.update(editUser.id, data);
        if (res.success) {
          setModalOpen(false);
          loadUsers();
        } else {
          setFormError(res.message || "Güncelleme başarısız.");
        }
      } else {
        if (!formPassword || formPassword.length < 6) {
          setFormError("Şifre en az 6 karakter olmalıdır.");
          setFormLoading(false);
          return;
        }

        const data: CreateUserRequest = {
          email: formEmail,
          fullName: formFullName,
          password: formPassword,
          role: formRole,
        };

        const res = await userService.create(data);
        if (res.success) {
          setModalOpen(false);
          loadUsers();
        } else {
          setFormError(res.message || "Oluşturma başarısız.");
        }
      }
    } catch (err: any) {
      setFormError(
        err.response?.data?.message || "Bir hata oluştu."
      );
    } finally {
      setFormLoading(false);
    }
  };

  const handleToggleActive = async (user: UserListItem) => {
    const action = user.isActive ? "deaktif" : "aktif";
    if (!confirm(`${user.fullName} kullanıcısını ${action} etmek istediğinize emin misiniz?`))
      return;

    try {
      if (user.isActive) {
        await userService.remove(user.id);
      } else {
        await userService.update(user.id, { isActive: true });
      }
      loadUsers();
    } catch {
      // ignore
    }
  };

  if (loading) return <LoadingSpinner />;

  return (
    <>
      <div className="flex items-center justify-between mb-6">
        <PageHeader title="Kullanıcı Yönetimi" description="Sistem kullanıcılarını yönetin." />
        <button
          onClick={openCreateModal}
          className="flex items-center gap-1.5 px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors"
        >
          <Plus className="w-4 h-4" /> Yeni Kullanıcı
        </button>
      </div>

      {/* Table */}
      <div className="bg-white rounded-xl border border-gray-200 shadow-sm overflow-hidden">
        <table className="w-full text-sm">
          <thead className="bg-gray-50 border-b border-gray-200">
            <tr>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Ad Soyad</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">E-posta</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Rol</th>
              <th className="text-left px-4 py-3 font-medium text-gray-600">Durum</th>
              <th className="text-right px-4 py-3 font-medium text-gray-600">İşlem</th>
            </tr>
          </thead>
          <tbody className="divide-y divide-gray-100">
            {users.map((user) => (
              <tr key={user.id} className="hover:bg-gray-50">
                <td className="px-4 py-3 font-medium text-gray-900">{user.fullName}</td>
                <td className="px-4 py-3 text-gray-600">{user.email}</td>
                <td className="px-4 py-3">
                  <span
                    className={`inline-flex px-2 py-0.5 text-xs font-medium rounded-full ${
                      user.role === "Admin"
                        ? "bg-purple-100 text-purple-700"
                        : user.role === "Operator"
                        ? "bg-blue-100 text-blue-700"
                        : "bg-gray-100 text-gray-700"
                    }`}
                  >
                    {user.role === "Operator" ? "Operatör" : user.role === "Viewer" ? "Görüntüleyici" : user.role}
                  </span>
                </td>
                <td className="px-4 py-3">
                  <span
                    className={`inline-flex px-2 py-0.5 text-xs font-medium rounded-full ${
                      user.isActive
                        ? "bg-green-100 text-green-700"
                        : "bg-red-100 text-red-700"
                    }`}
                  >
                    {user.isActive ? "Aktif" : "Pasif"}
                  </span>
                </td>
                <td className="px-4 py-3 text-right">
                  <div className="flex items-center justify-end gap-1">
                    <button
                      onClick={() => openEditModal(user)}
                      className="p-1.5 text-gray-400 hover:text-blue-600 hover:bg-blue-50 rounded"
                      title="Düzenle"
                    >
                      <Pencil className="w-4 h-4" />
                    </button>
                    <button
                      onClick={() => handleToggleActive(user)}
                      className={`p-1.5 rounded ${
                        user.isActive
                          ? "text-gray-400 hover:text-red-600 hover:bg-red-50"
                          : "text-gray-400 hover:text-green-600 hover:bg-green-50"
                      }`}
                      title={user.isActive ? "Deaktif Et" : "Aktif Et"}
                    >
                      {user.isActive ? <UserX className="w-4 h-4" /> : <UserCheck className="w-4 h-4" />}
                    </button>
                  </div>
                </td>
              </tr>
            ))}
          </tbody>
        </table>

        {users.length === 0 && (
          <div className="text-center py-12 text-gray-400">
            Henüz kullanıcı bulunmuyor.
          </div>
        )}
      </div>

      {/* Modal */}
      {modalOpen && (
        <div className="fixed inset-0 z-50 flex items-center justify-center bg-black/50">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-md mx-4 p-6">
            <div className="flex items-center justify-between mb-5">
              <h3 className="text-lg font-medium text-gray-900">
                {editUser ? "Kullanıcı Düzenle" : "Yeni Kullanıcı"}
              </h3>
              <button onClick={() => setModalOpen(false)} className="text-gray-400 hover:text-gray-600">
                <X className="w-5 h-5" />
              </button>
            </div>

            {formError && (
              <div className="mb-4 p-3 bg-red-50 border border-red-200 text-red-700 text-sm rounded-lg">
                {formError}
              </div>
            )}

            <form onSubmit={handleSubmit} className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1.5">Ad Soyad</label>
                <input
                  type="text"
                  value={formFullName}
                  onChange={(e) => setFormFullName(e.target.value)}
                  required
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1.5">E-posta</label>
                <input
                  type="email"
                  value={formEmail}
                  onChange={(e) => setFormEmail(e.target.value)}
                  required
                  disabled={!!editUser}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent disabled:bg-gray-100"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1.5">
                  {editUser ? "Şifre (boş bırakılırsa değişmez)" : "Şifre"}
                </label>
                <input
                  type="password"
                  value={formPassword}
                  onChange={(e) => setFormPassword(e.target.value)}
                  required={!editUser}
                  placeholder="••••••••"
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1.5">Rol</label>
                <select
                  value={formRole}
                  onChange={(e) => setFormRole(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-lg text-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-transparent"
                >
                  {ROLES.map((r) => (
                    <option key={r.value} value={r.value}>
                      {r.label}
                    </option>
                  ))}
                </select>
              </div>

              <div className="flex gap-2 pt-2">
                <button
                  type="button"
                  onClick={() => setModalOpen(false)}
                  className="flex-1 px-4 py-2 border border-gray-300 text-gray-700 text-sm font-medium rounded-lg hover:bg-gray-50 transition-colors"
                >
                  İptal
                </button>
                <button
                  type="submit"
                  disabled={formLoading}
                  className="flex-1 px-4 py-2 bg-blue-600 text-white text-sm font-medium rounded-lg hover:bg-blue-700 transition-colors disabled:opacity-50"
                >
                  {formLoading ? "Kaydediliyor..." : editUser ? "Güncelle" : "Oluştur"}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </>
  );
}
