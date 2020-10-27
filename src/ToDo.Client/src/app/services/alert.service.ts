import { Injectable } from '@angular/core';
import { Observable, Subject } from 'rxjs';
import { filter } from 'rxjs/operators';
import { Alert, AlertType } from '../models/alert.model';


@Injectable({ providedIn: 'root' })
export class AlertService {
    private subject = new Subject<Alert>();
    private defaultId = 'default-alert';

    onAlert(id = this.defaultId): Observable<Alert> {
        return this.subject.asObservable().pipe(filter(x => x && x.id === id));
    }

    success(message: string, title?: string, options?: any): void {
        this.alert(new Alert({ ...options, type: AlertType.Success, message, title }));
    }

    error(message: string, title?: string, options?: any): void {
        this.alert(new Alert({ ...options, type: AlertType.Error, message, title }));
    }

    info(message: string, title?: string, options?: any): void {
        this.alert(new Alert({ ...options, type: AlertType.Info, message, title }));
    }

    warn(message: string, title?: string, options?: any): void {
        this.alert(new Alert({ ...options, type: AlertType.Warning, message, title }));
    }

    clear(id = this.defaultId): void {
        this.subject.next(new Alert({ id }));
    }

    private alert(alert: Alert): void {
        alert.id = alert.id || this.defaultId;
        alert.autoClose = true;
        this.subject.next(alert);
    }
}
