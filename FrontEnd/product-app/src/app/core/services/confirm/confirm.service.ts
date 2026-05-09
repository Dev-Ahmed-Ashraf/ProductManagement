import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root',
})
export class ConfirmService {
  async ask(message: string, title = 'Confirm'): Promise<boolean> {
    const result = await Swal.fire({
      title,
      text: message,
      icon: 'warning',
      showCancelButton: true,
      confirmButtonColor: '#14b8a6',
      cancelButtonColor: '#6b7280',
      confirmButtonText: 'Yes, proceed',
      cancelButtonText: 'Cancel',
      allowOutsideClick: false,
      allowEscapeKey: true,
    });

    return result.isConfirmed;
  }
}
