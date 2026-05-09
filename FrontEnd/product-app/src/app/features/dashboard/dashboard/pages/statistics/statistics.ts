import { Component, OnInit, computed, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { StatisticsService } from '../../core/services/statistics-service';
import { StatsCard } from '../../../../../shared/components/stats-card/stats-card';
import { StatisticsResponse } from '../../core/models/StatisticsResponse.model';

@Component({
  selector: 'app-statistics',
  standalone: true,
  imports: [CommonModule, StatsCard],
  templateUrl: './statistics.html',
  styleUrl: './statistics.css',
})
export class StatisticsPage implements OnInit {
  private service = inject(StatisticsService);

  readonly loading = signal(false);
  readonly error = signal<string | null>(null);
  readonly stats = signal<StatisticsResponse | null>(null);

  readonly metricCards = [
    {
      title: 'Total products',
      value: () => this.stats()?.products?.total ?? 0,
      subtitle: () => `${this.availableRate() || 0}% available`,
      tone: 'teal',
    },
    {
      title: 'Available products',
      value: () => this.stats()?.products?.available ?? 0,
      subtitle: () => 'Ready for sale',
      tone: 'teal',
    },
    {
      title: 'Stock attention',
      value: () =>
        (this.stats()?.products?.outOfStock ?? 0) + (this.stats()?.products?.backOrder ?? 0),
      subtitle: () => 'Out of stock + back-order',
      tone: 'amber',
    },
    {
      title: 'Workflow changes',
      value: () => this.stats()?.statusChanges?.total ?? 0,
      subtitle: () => 'Recorded status moves',
      tone: 'blue',
    },
    {
      title: 'Total users',
      value: () => this.stats()?.users?.total ?? 0,
      subtitle: () => `${this.activeUserRate() || 0}% active accounts`,
      tone: 'violet',
    },
    {
      title: 'Inactive users',
      value: () => this.stats()?.users?.inactive ?? 0,
      subtitle: () => 'May need review',
      tone: 'slate',
    },
  ] as const;

  readonly productChart = computed<ChartDatum[]>(() => {
    const products = this.stats()?.products;
    const items = [
      { label: 'Available', value: products?.available ?? 0, color: '#14b8a6' },
      { label: 'Out of stock', value: products?.outOfStock ?? 0, color: '#f59e0b' },
      { label: 'Discontinued', value: products?.discontinued ?? 0, color: '#ef4444' },
      { label: 'Pre-order', value: products?.preOrder ?? 0, color: '#3b82f6' },
      { label: 'Back-order', value: products?.backOrder ?? 0, color: '#8b5cf6' },
      { label: 'Draft', value: products?.draft ?? 0, color: '#94a3b8' },
    ];

    const maxValue = Math.max(1, ...items.map((item) => item.value));
    const total = Math.max(
      1,
      items.reduce((sum, item) => sum + item.value, 0),
    );

    return items.map((item) => ({
      ...item,
      percent: (item.value / maxValue) * 100,
      share: (item.value / total) * 100,
    }));
  });

  readonly userChart = computed<ChartDatum[]>(() => {
    const users = this.stats()?.users;
    const items = [
      { label: 'Active', value: users?.active ?? 0, color: '#14b8a6' },
      { label: 'Inactive', value: users?.inactive ?? 0, color: '#94a3b8' },
    ];

    const total = Math.max(
      1,
      items.reduce((sum, item) => sum + item.value, 0),
    );

    return items.map((item) => ({
      ...item,
      percent: (item.value / total) * 100,
      share: (item.value / total) * 100,
    }));
  });

  readonly userDonutBackground = computed(() => {
    const active = this.userChart()[0]?.percent ?? 0;
    return `conic-gradient(#14b8a6 0 ${active}%, #94a3b8 ${active}% 100%)`;
  });

  readonly productDonutBackground = computed(() => {
    let cursor = 0;
    const segments = this.productChart().map((item) => {
      const start = cursor;
      cursor += item.share;
      return `${item.color} ${start}% ${cursor}%`;
    });

    return `conic-gradient(${segments.join(', ') || '#e2e8f0 0 100%'})`;
  });

  readonly userTotal = computed(() => this.stats()?.users?.total ?? 0);
  readonly statusChangeTotal = computed(() => this.stats()?.statusChanges?.total ?? 0);
  readonly productTotal = computed(() => this.stats()?.products?.total ?? 0);
  readonly availableRate = computed(() => this.percent(this.stats()?.products?.available ?? 0, this.productTotal()));
  readonly activeUserRate = computed(() => this.percent(this.stats()?.users?.active ?? 0, this.userTotal()));
  readonly riskProductTotal = computed(
    () =>
      (this.stats()?.products?.outOfStock ?? 0) +
      (this.stats()?.products?.backOrder ?? 0) +
      (this.stats()?.products?.discontinued ?? 0),
  );
  readonly riskProductRate = computed(() => this.percent(this.riskProductTotal(), this.productTotal()));
  readonly publishReadyTotal = computed(
    () => (this.stats()?.products?.available ?? 0) + (this.stats()?.products?.preOrder ?? 0),
  );
  readonly publishReadyRate = computed(() => this.percent(this.publishReadyTotal(), this.productTotal()));
  readonly topProductStatus = computed(() => {
    const [top] = [...this.productChart()].sort((a, b) => b.value - a.value);
    return top ?? null;
  });
  readonly topProductStatusLabel = computed(() => this.topProductStatus()?.label ?? 'No data');
  readonly topProductStatusValue = computed(() => this.topProductStatus()?.value ?? 0);

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading.set(true);
    this.error.set(null);
    this.service.getStatistics().subscribe({
      next: (res) => {
        if (res && res.success && res.data) {
          this.stats.set(res.data);
        } else {
          this.error.set(res?.message ?? 'Failed to load statistics.');
        }
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Failed to load statistics.');
        this.loading.set(false);
      },
    });
  }

  private percent(value: number, total: number): number {
    if (!total) return 0;
    return Math.round((value / total) * 100);
  }
}

interface ChartDatum {
  label: string;
  value: number;
  color: string;
  percent: number;
  share: number;
}
