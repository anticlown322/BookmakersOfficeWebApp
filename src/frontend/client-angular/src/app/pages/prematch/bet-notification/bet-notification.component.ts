import { Component, EventEmitter, Input, Output } from '@angular/core';

@Component({
    selector: 'app-bet-notification',
    templateUrl: './bet-notification.component.html',
    styleUrls: ['./bet-notification.component.scss'],
})
export class BetNotificationComponent {
    @Input() isOpen = false;
    @Input() isLoading = false;
    @Input() isSuccess = false;
    @Input() message = '';
    @Output() closed = new EventEmitter<void>();

    closeNotification(): void {
        this.closed.emit();
    }
}
