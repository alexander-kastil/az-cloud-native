import { CartItem } from '../cart-item.model';
export class Order {
  constructor() {
    this.customer = new Customer();
    this.payment = new Payment();
    this.shippingAddress = new Address();
    this.items = [];
    this.status = 'cart';
  }

  id = '';
  type = 'order'
  customer: Customer;
  shippingAddress: Address;
  payment: Payment;
  items: CartItem[];
  total = 0;
  status: OrderStatus;
}

export class Payment {
  type = '';
  account = '';
}

export class Address {
  street = '';
  city = '';
  state = '';
  zip = '';
}

export class Customer {
  id = '';
  name = '';
  email = '';
  address: Address = new Address();
}

export declare type OrderStatus =
  | 'cart'
  | 'placed'
  | 'paid'
  | 'preparing'
  | 'ready_for_delivery'
  | 'delivered'
  | 'rejected';
