import { ChangeDetectionStrategy, ChangeDetectorRef, Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpErrorResponse } from '@angular/common/http';
import { ProductService } from '../../Core/Service/product.service';
import { ErrorHandlerService } from '../../../../core/services/error-handler/error-handler.service';
import { finalize } from 'rxjs';
import { AlertService } from '../../../../core/services/alert/alert';

@Component({
  selector: 'app-add-product',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './add-product.html',
  styleUrl: './add-product.css',
})
export class AddProduct {
  private fb = inject(FormBuilder);
  private productService = inject(ProductService);
  private errorHandler = inject(ErrorHandlerService);
  private alert = inject(AlertService);
  private cdr = inject(ChangeDetectorRef);

  isLoading = false;
  private submitting = false;

  private getErrorMessage(error: HttpErrorResponse) {
    return this.errorHandler.getMessage(error) || 'Error adding product';
  }

  form: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(200)]],
    description: ['', [Validators.maxLength(1000)]],
    price: ['', [Validators.required, Validators.min(1)]],
    quantity: ['', [Validators.required, Validators.min(0)]],
  });

  onSubmit() {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    if (this.submitting) return;

    this.submitting = true;
    this.isLoading = true;

    this.productService
      .createProduct(this.form.value)
      .pipe(
        finalize(() => {
          this.isLoading = false;
          this.submitting = false;
          this.cdr.markForCheck();
        }),
      )
      .subscribe({
        next: (response) => {
          this.alert.success('Product added successfully.');
          this.form.reset();
          this.cdr.markForCheck();
        },
      });
  }

  get f() {
    return this.form.controls;
  }
}
