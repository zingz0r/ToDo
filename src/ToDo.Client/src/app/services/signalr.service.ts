import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import * as signalR from '@microsoft/signalr';
import { SignalBase } from '../signals/signal.base';

@Injectable({
    providedIn: 'root'
})
export class SignalRService {
    private hubConnection: signalR.HubConnection;

    async startConnection(): Promise<void> {
        this.stopConnection();

        const hub = `${environment.settings.signalrhub}`;

        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl(hub)
            .withHubProtocol(new signalR.JsonHubProtocol())
            .withAutomaticReconnect()
            .build();

        await this.hubConnection
            .start()
            .then(() => {
                console.log(`SignalR connected to ${hub}`);
            })
            .catch((err) => console.log('Error while starting connection: ' + err));
    }

    stopConnection(): void {
        if (!this.hubConnection) {
            return;
        }

        console.log('SignalR closing connection...');
        this.hubConnection.stop();
    }

    startListeningTo<T extends SignalBase>(ctor: new () => T, when: (x: T) => boolean, then: (signal: T) => void): void {
        const inst = new ctor();
        const name = inst.className;

        this.hubConnection.on(name, (signal: T) => {
            if (when(signal)) {
                console.log(`SignalR handling ${name} signal`);
                then(signal);
            }
        });
        console.log(`SignalR started listening to ${name} signals`);
    }

    stopListeningTo<T extends SignalBase>(ctor: new () => T): void {
        if (!this.hubConnection) {
            return;
        }

        const inst = new ctor();
        const name = inst.className;
        this.hubConnection.off(name);
        console.log(`SignalR stopped listening to ${name} signals`);
    }
}
