export interface ProductFeatureItem {
  title: string;
  route: string;
  permission: string;
  icon: 'list' | 'plus' | 'status';
}

export interface ProductActionItem {
  title: string;
  permission: string;
  icon: 'trash' | 'status';
}

export const PRODUCT_FEATURES: ProductFeatureItem[] = [
  {
    title: 'Product List',
    route: '/dashboard/products',
    permission: 'products:view',
    icon: 'list',
  },
  {
    title: 'Add Product',
    route: '/dashboard/products/add',
    permission: 'products:create',
    icon: 'plus',
  },
  {
    title: 'Status Histories',
    route: '/dashboard/product-histories',
    permission: 'product-status-histories:view',
    icon: 'status',
  },
];

export const PRODUCT_DETAIL_ACTIONS: ProductActionItem[] = [
  {
    title: 'Delete Product',
    permission: 'products:delete',
    icon: 'trash',
  },
  {
    title: 'Change Status',
    permission: 'products:change-status',
    icon: 'status',
  },
];

export function filterProductFeatures(
  permissions: ReadonlySet<string>,
  features: ProductFeatureItem[] = PRODUCT_FEATURES,
): ProductFeatureItem[] {
  return features.filter((feature) => permissions.has(feature.permission.toLowerCase()));
}

export function filterProductActions(
  permissions: ReadonlySet<string>,
  actions: ProductActionItem[] = PRODUCT_DETAIL_ACTIONS,
): ProductActionItem[] {
  return actions.filter((action) => permissions.has(action.permission.toLowerCase()));
}
