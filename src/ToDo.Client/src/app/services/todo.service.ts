import { debounce } from 'rxjs/internal/operators/debounce';
import { environment } from 'src/environments/environment';
import { timer } from 'rxjs';
import { AlertService } from './alert.service';
import { Endpoint } from 'src/common/endpoints';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToDoModel } from 'src/app/models/todo.model';
import { ToDoState } from 'src/common/todostate';

@Injectable({
    providedIn: 'root'
})

export class ToDoService {
    private baseUrl = environment.settings.backend;
    constructor(
        private http: HttpClient,
        private alertService: AlertService
    ) { }

    public Search(pattern: string, state: ToDoState, then: (res: ToDoModel[]) => void): void {
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
