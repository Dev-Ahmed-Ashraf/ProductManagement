export interface ProductStatusHistoriesFilters {
  pageNumber: number;
  pageSize: number;
  productId?: number;
  oldStatus?: number;
  newStatus?: number;
  fromDate?: Date;
  toDate?: Date;
}
