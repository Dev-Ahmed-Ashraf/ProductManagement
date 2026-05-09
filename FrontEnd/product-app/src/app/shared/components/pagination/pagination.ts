import { ChangeDetectionStrategy, Component, computed, input, output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  changeDetection: ChangeDetectionStrategy.OnPush,
  templateUrl: './pagination.html',
  styleUrl: './pagination.css',
})
export class Pagination {
  readonly currentPage = input(1);
  readonly totalPages = input(0);
  readonly maxVisiblePages = input(5);
  readonly disabled = input(false);

  readonly pageChange = output<number>();

  readonly activePage = computed(() => this.getActivePage());
  readonly visiblePages = computed(() =>
    this.buildVisiblePages(this.activePage(), this.totalPages(), this.maxVisiblePages()),
  );
  readonly hasPages = computed(() => this.totalPages() > 0);

  previousPage(): void {
    this.goToPage(this.activePage() - 1);
  }

  nextPage(): void {
    this.goToPage(this.activePage() + 1);
  }

  goToPage(page: number): void {
    if (this.isNavigationDisabled()) {
      return;
    }

    const totalPages = this.totalPages();
    if (totalPages === 0) {
      return;
    }

    const nextPage = this.clampPage(page, totalPages);
    if (nextPage === this.activePage()) {
      return;
    }

    this.pageChange.emit(nextPage);
  }

  private getActivePage(): number {
    const totalPages = this.totalPages();
    if (totalPages === 0) {
      return 0;
    }

    return this.clampPage(this.currentPage(), totalPages);
  }

  private buildVisiblePages(
    activePage: number,
    totalPages: number,
    maxVisiblePages: number,
  ): number[] {
    if (totalPages === 0) {
      return [];
    }

    const safeMaxVisiblePages = Math.max(1, this.normalizePositiveInteger(maxVisiblePages));
    if (totalPages <= safeMaxVisiblePages) {
      return this.createPageRange(1, totalPages);
    }

    const halfWindow = Math.floor(safeMaxVisiblePages / 2);
    let startPage = Math.max(1, activePage - halfWindow);
    let endPage = startPage + safeMaxVisiblePages - 1;

    if (endPage > totalPages) {
      endPage = totalPages;
      startPage = Math.max(1, endPage - safeMaxVisiblePages + 1);
    }

    return this.createPageRange(startPage, endPage);
  }

  private createPageRange(startPage: number, endPage: number): number[] {
    return Array.from({ length: endPage - startPage + 1 }, (_, index) => startPage + index);
  }

  private clampPage(page: number, totalPages: number): number {
    return Math.min(Math.max(1, this.normalizePositiveInteger(page)), totalPages);
  }

  private normalizePositiveInteger(value: number): number {
    return Number.isFinite(value) && value > 0 ? Math.floor(value) : 1;
  }

  private isNavigationDisabled(): boolean {
    return this.disabled();
  }
}
