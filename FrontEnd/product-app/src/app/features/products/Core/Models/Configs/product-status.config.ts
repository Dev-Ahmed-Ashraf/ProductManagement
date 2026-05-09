export const STATUS_ENUM_MAP = {
  0: 'available',
  1: 'out_of_stock',
  2: 'discontinued',
  3: 'pre_order',
  4: 'back_order',
  5: 'draft',
} as const;

export type ProductStatusKey = (typeof STATUS_ENUM_MAP)[keyof typeof STATUS_ENUM_MAP];

export interface StatusConfig {
  label: string;
  color: string;
}

export const PRODUCT_STATUS_MAP: Record<ProductStatusKey, StatusConfig> = {
  available: {
    label: 'Available',
    color: 'bg-green-100 text-green-700',
  },
  out_of_stock: {
    label: 'Out of Stock',
    color: 'bg-yellow-100 text-yellow-700',
  },
  discontinued: {
    label: 'Discontinued',
    color: 'bg-gray-100 text-gray-600',
  },
  pre_order: {
    label: 'Pre-Order',
    color: 'bg-blue-100 text-blue-700',
  },
  back_order: {
    label: 'Back Order',
    color: 'bg-purple-100 text-purple-700',
  },
  draft: {
    label: 'Draft',
    color: 'bg-gray-100 text-gray-600',
  },
};
