import { HttpClient } from '@angular/common/http';
import { Injectable, inject } from '@angular/core';
import { AILoggerService } from 'src/app/logger/ai-logger.service';
import { environment } from '../../../../environments/environment.development';
import { CartItem } from '../cart-item.model';

@Injectable({
  providedIn: 'root'
})
export class OrdersService {
  http = inject(HttpClient);
  logger = inject(AILoggerService);

  addToCart(item: CartItem) {

  }

  removeFromCart(item: CartItem) {

  }

  checkout(order: any) {
    this.logger.logEvent('ShopUI - Checkout Order', order);
    return this.http.post(`${environment.ordersApi}/orders/create`, order);
  }
}
