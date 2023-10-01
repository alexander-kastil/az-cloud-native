export interface OrderItem {
  id: number;
  name: string;
  price: number;
  quantity: number;
}

export interface Order {
  id: string;
  customer: Customer;
  payment: Payment;
  items: OrderItem[];
  status: OrderStatus;
}

export interface Payment {
  type: string;
  account: string;
}

export interface Customer {
  id: string;
  name: string;
  address: string;
  email: string;
}

export declare type OrderStatus =
  | 'cart'
  | 'placed'
  | 'paid'
  | 'preparing'
  | 'ready_for_delivery'
  | 'delivered'
  | 'rejected';
