import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';
import { DataPoint } from '../models/data-point.model';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  private readonly _hubUrl: string = environment.dataHubUrl;
  private readonly _hubConnection: signalR.HubConnection = new signalR.HubConnectionBuilder()
    .withUrl(this._hubUrl)
    .withAutomaticReconnect([0, 2, 5, 10, 15, 25, 35])
    .build();
  public heartDataReceived: EventEmitter<DataPoint> = new EventEmitter<DataPoint>();
  private _isConnected: boolean = false;
  private _connectionId!: string;

  constructor() {
    this.startConnection();
    this.onHeartDataReceived();
    this._hubConnection.onclose(async () => {
      this._isConnected = false;
      await this.startConnection();
    })
  }

  private async startConnection(): Promise<void> {
    await this._hubConnection.start().then(() => {
      this._isConnected = true;
      this._connectionId = this._hubConnection.connectionId as string;
    });
  }

  private onHeartDataReceived(): void {
    this._hubConnection.on("HeartData", (data: string) => {
      const dataPoint = new DataPoint(JSON.parse(data));
      this.heartDataReceived.emit(dataPoint);
    })
  }

  public GetConnectionId(): string {
    return this._isConnected ? this._connectionId : "";
  }
}
