// Role-based page access
const ROLE_PAGES: Record<string, string[]> = {
  Admin: ["/", "/work-orders", "/logs", "/users", "/settings"],
  Operator: ["/", "/work-orders", "/logs"],
  Viewer: ["/", "/work-orders"],
};

// Role-based feature flags
const ROLE_FEATURES: Record<string, string[]> = {
  Admin: [
    "declaration:edit",
    "declaration:send",
    "status:update",
    "archive:upload",
    "archive:send",
    "user:manage",
  ],
  Operator: [
    "declaration:edit",
    "declaration:send",
    "status:update",
    "archive:upload",
    "archive:send",
  ],
  Viewer: [],
};

export function canAccessPath(role: string, path: string): boolean {
  const pages = ROLE_PAGES[role];
  if (!pages) return false;

  // Exact match or starts with a valid path
  return pages.some((page) =>
    page === "/" ? path === "/" : path.startsWith(page)
  );
}

export function hasFeature(role: string, feature: string): boolean {
  const features = ROLE_FEATURES[role];
  if (!features) return false;
  return features.includes(feature);
}

export function getNavigationItems(role: string) {
  const pages = ROLE_PAGES[role] || [];
  return pages;
}
