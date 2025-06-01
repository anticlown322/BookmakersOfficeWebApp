import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { Tournament } from '../../models/sport-data-service/entities/tournament/tournament.model';
import { PrematchUpdate } from '../../models/sport-data-service/entities/tournament/prematch-update.model';

@Injectable({
    providedIn: 'root',
})
export class TournamentSignalrService {
    private hubConnection!: signalR.HubConnection;

    private prematchUpdatedSource = new BehaviorSubject<PrematchUpdate | null>(
        null
    );
    prematchUpdated$ = this.prematchUpdatedSource.asObservable();

    public startConnection(): void {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:50012/hubs/prematch', {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets,
            })
            .withAutomaticReconnect()
            .build();

        this.hubConnection.start().then(() => console.log('SignalR connected'));

        this.hubConnection.on(
            'PrematchUpdated',
            (tournaments: Tournament[], metaData: any) => {
                console.log('Prematch updated:', { tournaments, metaData });
                this.prematchUpdatedSource.next({ tournaments, metaData });
            }
        );

        this.hubConnection.onclose((error) => {
            console.error('Connection closed due to error: ', error);
            this.tryReconnect();
        });
    }

    private tryReconnect(): void {
        setTimeout(() => {
            this.startConnection();
        }, 5000);
    }

    public stopConnection(): void {
        if (this.hubConnection) {
            this.hubConnection
                .stop()
                .then(() => console.log('[Connection stopped'));
        }
    }
}
