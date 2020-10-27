import { environment } from 'src/environments/environment';
import { AlertService } from './alert.service';
import { Endpoint } from '../../common/endpoints';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToDoState } from '../../common/todostate';

@Injectable({
    providedIn: 'root'
})

export class BetService {
    private baseUrl = environment.settings.backend;
    constructor(
        private http: HttpClient,
        private alertService: AlertService
    ) { }

    public Search(pattern: string, state: ToDoState, then: (res) => void): void {
        this.http
            .get<any>(`${this.baseUrl}/${Endpoint.ToDo.Base}/${Endpoint.ToDo.Search}/${pattern}/${state}`)
            .toPromise()
            .then((res) => {
                then(res);
            })
            .catch((reason) => {
                this.alertService.error(reason.message);
            });
    }
}