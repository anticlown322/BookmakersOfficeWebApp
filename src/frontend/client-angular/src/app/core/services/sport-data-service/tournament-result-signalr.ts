import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';
import * as signalR from '@microsoft/signalr';
import { TournamentResult } from '../../models/sport-data-service/entities/tournament-result/tournament-result.model';
import { ResultsUpdate } from '../../models/sport-data-service/entities/tournament-result/results-update.model copy';

@Injectable({
    providedIn: 'root',
})
export class TournamentResultSignalrService {
    private hubConnection!: signalR.HubConnection;

    private resultsUpdatedSource = new BehaviorSubject<ResultsUpdate | null>(
        null
    );
    resultsUpdated$ = this.resultsUpdatedSource.asObservable();

    public startConnection(): void {
        this.hubConnection = new signalR.HubConnectionBuilder()
            .withUrl('http://localhost:50012/hubs/results', {
                skipNegotiation: true,
                transport: signalR.HttpTransportType.WebSockets,
            })
            .withAutomaticReconnect()
            .build();

        this.hubConnection.start().then(() => console.log('SignalR connected'));

        this.hubConnection.on(
            'ResultsUpdated',
            (tournaments: TournamentResult[], metaData: any) => {
                console.log('ResultsUpdated updated:', { tournaments: tournaments, metaData });
                this.resultsUpdatedSource.next({ tournaments: tournaments, metaData });
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
