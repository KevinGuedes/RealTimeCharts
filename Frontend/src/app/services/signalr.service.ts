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
    .withAutomaticReconnect([5000, 10000, 30000, 60000, 120000, 180000])
    .build();
  public dataReceived: EventEmitter<DataPoint> = new EventEmitter<DataPoint>();
  public dataGenerationFinished: EventEmitter<boolean> = new EventEmitter<boolean>();
  public connectionStatus: EventEmitter<boolean> = new EventEmitter<boolean>();

  constructor() {
    this.startConnection();
    this.subscribeToEvents();
    this.configureConnection();
  }

  private configureConnection() {
    this._hubConnection.onreconnecting(_ => {
      console.error('Connection with SignalR was lost, trying to reconnect')
      this.connectionStatus.emit(false);
    })

    this._hubConnection.onreconnected(_ => {
      this.connectionStatus.emit(true);
      console.log('Connection with SignalR Hub established')
    })
  }

  private async startConnection(): Promise<void> {
    await this._hubConnection.start().then(() => {
      this.connectionStatus.emit(true);
    });
  }

  private subscribeToEvents(): void {
    this.onDataReceived();
    this.onDataGenerationFinished();
  }

  private onDataReceived(): void {
    this._hubConnection.on("DataPointDispatched", (receivedData: string) => {
      const dataPoint = new DataPoint(JSON.parse(receivedData));
      this.dataReceived.emit(dataPoint);
    })
  }

  private onDataGenerationFinished(): void {
    this._hubConnection.on("DataGenerationFinished", (success: boolean) => {
      this.dataGenerationFinished.emit(success);
    })
  }

  public getConnectionId(): string {
    return this._hubConnection.connectionId ? this._hubConnection.connectionId : '';
  }
}
