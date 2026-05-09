export interface ProductListQuery {
  pageNumber: number;
  pageSize: number;
  name: string | null;
  description?: string | null;
  price?: number | null;
  quantity?: number | null;
}
