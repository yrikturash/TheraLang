import { Injectable } from '@angular/core';
import { MatSnackBar, MatSnackBarConfig } from '@angular/material';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  constructor(public snackBar: MatSnackBar) { }

  config: MatSnackBarConfig = {
    duration: 10000,
    horizontalPosition: 'center',
    verticalPosition: 'top',
    
  }


  success(msg) {
    this.config['panelClass'] = ['notification', 'success'];
    this.snackBar.open(msg, '',this.config);
  }

  warn(msg) {
    this.config['panelClass'] = ['notification', 'warn'];
    this.snackBar.open(msg, '', this.config);
  }
}
