import { EventEmitter, Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';
import { environment } from 'src/environments/environment';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {

  public heartDataReceived: EventEmitter<any> = new EventEmitter();

  private _isConnected: boolean = false;
  private _hubUrl: string = environment.dataHubUrl;

  private readonly _hubConnection: signalR.HubConnection = new signalR.HubConnectionBuilder()
    .withUrl(this._hubUrl)
    .withAutomaticReconnect([0, 2, 5, 10, 15, 25, 35])
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
      this.heartDataReceived.emit(JSON.parse(data));
    })
  }

  public sendNewMessage(): void {
    this._hubConnection.send('HeartData', "hello boy");
  }
}