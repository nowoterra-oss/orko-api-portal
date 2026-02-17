// Work Order
export interface WorkOrder {
  id: string;
  fileNumber: string;
  selsilOrderId?: string;
  type: string;
  status: string;
  customerName?: string;
  hasDeclaration: boolean;
  sentToEvrim: boolean;
  createdAt: string;
}

export interface WorkOrderDetail {
  id: string;
  fileNumber: string;
  selsilOrderId?: string;
  type: string;
  status: string;
  createdBy?: string;
  createdAt: string;
  updatedAt: string;
  declaration?: DeclarationSummary;
}

// Declaration
export interface DeclarationSummary {
  id: string;
  declarationType: string;
  status: string;
  evrimDeclarationId?: string;
  sentToEvrim: boolean;
  sentAt?: string;
  archiveCount: number;
}

export interface DeclarationDetail {
  id: string;
  workOrderId: string;
  fileNumber: string;
  declarationType: string;
  status: string;
  declarationData?: string;
  evrimDeclarationId?: string;
  evrimResponse?: string;
  sentToEvrim: boolean;
  sentAt?: string;
  updatedBy?: string;
  createdAt: string;
  updatedAt: string;
  archives: Archive[];
  statusHistory: StatusHistory[];
}

// Archive
export interface Archive {
  id: string;
  fileName: string;
  fileSize: number;
  contentType?: string;
  documentType?: string;
  sentToEvrim: boolean;
  sentAt?: string;
  uploadedBy?: string;
  createdAt: string;
}

// Status
export interface StatusHistory {
  id: string;
  fromStatus: string;
  toStatus: string;
  changedBy?: string;
  note?: string;
  evrimNotified: boolean;
  createdAt: string;
}

export interface AllowedTransitions {
  currentStatus: string;
  allowedTransitions: string[];
}

// Dashboard
export interface DashboardSummary {
  totalWorkOrders: number;
  pendingDeclarations: number;
  sentToEvrim: number;
  completedToday: number;
  statusDistribution: Record<string, number>;
  recentActivities: RecentActivity[];
}

export interface RecentActivity {
  fileNumber: string;
  action: string;
  user?: string;
  timestamp: string;
}
