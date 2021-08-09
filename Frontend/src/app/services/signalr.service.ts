import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  public heartDataReceived: EventEmitter<any> = new EventEmitter();
  private _isConnected: boolean = false;
  private readonly _hubConnection: signalR.HubConnection = new signalR.HubConnectionBuilder()
    .withUrl('https://localhost:6001/data')
    .build();

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
    });
  }

  private onHeartDataReceived(): void {
    this._hubConnection.on("HeartData", (data: string) => {
      this.heartDataReceived.emit(data);
    })
  }

  public sendNewMessage(): void {
    this._hubConnection.send('HeartData', "hello boy");
  }
}
