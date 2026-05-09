import { ProductStatistics } from './ProductStatistics.model';
import { StatusChangeStatistics } from './StatusChangeStatistics.mpdel';
import { UsersStatistics } from './UserStatistics.model';

export interface StatisticsResponse {
  products: ProductStatistics;
  users: UsersStatistics;
  statusChanges: StatusChangeStatistics;
}
