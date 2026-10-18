import { Component, OnDestroy, OnInit, computed, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';

interface VanPosition {
  vanId: string;
  sequence: number;
  latitude: number;
  longitude: number;
  observedAt: string;
}

@Component({
  selector: 'app-van-map',
  templateUrl: './van-map.html',
  styleUrl: './van-map.css',
})
export class VanMap implements OnInit, OnDestroy {
  private readonly size = 100;
  private readonly padding = 0.01;
  private connection?: HubConnection;

  protected readonly positions = signal<Record<string, VanPosition>>({});
  protected readonly vans = computed(() => Object.values(this.positions()));
  protected readonly viewBox = computed(() => {
    const vans = this.vans();
    if (vans.length === 0) {
      return `0 0 ${this.size} ${this.size}`;
    }
    const longitudes = vans.map((v) => v.longitude);
    const latitudes = vans.map((v) => v.latitude);
    const minX = Math.min(...longitudes) - this.padding;
    const maxX = Math.max(...longitudes) + this.padding;
    const minY = Math.min(...latitudes) - this.padding;
    const maxY = Math.max(...latitudes) + this.padding;
    return `${minX} ${-maxY} ${maxX - minX} ${maxY - minY}`;
  });
  protected readonly radius = computed(() => {
    const [, , width] = this.viewBox().split(' ').map(Number);
    return width / 80;
  });

  async ngOnInit() {
    this.connection = new HubConnectionBuilder()
      .withUrl(new URL('/hubs/van-positions', document.baseURI).href, { withCredentials: true })
      .withAutomaticReconnect()
      .build();
    this.connection.on('vanPositionReported', (position: VanPosition) =>
      this.positions.update((current) => ({ ...current, [position.vanId]: position })),
    );
    await this.connection.start();
  }

  async ngOnDestroy() {
    await this.connection?.stop();
  }
}
