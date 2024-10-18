import { Injectable } from '@angular/core';
import * as signalR from '@microsoft/signalr';

@Injectable({
  providedIn: 'root'
})
export class SignalrService {
  private hubConnection!: signalR.HubConnection;
  public temperature: string;

  constructor() {
    this.temperature = '';
  }

  public startConnection() {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl('https://localhost:7111/temperatureHub')  // Adjust the URL to match your backend
      .build();

    this.hubConnection
      .start()
      .then(() => console.log('SignalR connection started'))
      .catch(err => console.log('Error while starting SignalR connection: ' + err));
  }

  public addTemperatureListener() {
    this.hubConnection.on('ReceiveTemperature', (data: string) => {
      console.log('Temperature received: ' + data);
      this.temperature = data;
    });
  }
}
