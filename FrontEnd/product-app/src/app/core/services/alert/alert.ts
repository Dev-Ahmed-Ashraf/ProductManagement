import { Injectable } from '@angular/core';
import Swal from 'sweetalert2';

@Injectable({
  providedIn: 'root',
})
export class AlertService {
  success(message: string, title = 'Success') {
    void Swal.fire({
      title,
      text: message,
      icon: 'success',
      confirmButtonColor: '#14b8a6',
      timer: 3000,
      timerProgressBar: true,
      showConfirmButton: true,
    });
  }

  error(message: string, title = 'Error') {
    void Swal.fire({
      title,
      text: message,
      icon: 'error',
      confirmButtonColor: '#ef4444',
      showConfirmButton: true,
    });
  }

  info(message: string, title = 'Info') {
    void Swal.fire({
      title,
      text: message,
      icon: 'info',
      confirmButtonColor: '#14b8a6',
      timer: 3000,
      timerProgressBar: true,
      showConfirmButton: true,
    });
  }
}
