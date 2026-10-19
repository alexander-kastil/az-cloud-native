import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { VanMap } from './van-map/van-map';

@Component({
  imports: [RouterOutlet, VanMap],
  selector: 'app-root',
  styleUrl: './app.css',
  templateUrl: './app.html',
})
export class App {
  protected readonly title = signal('ops-console');
}
