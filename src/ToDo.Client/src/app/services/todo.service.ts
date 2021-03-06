import { environment } from 'src/environments/environment';
import { AlertService } from './alert.service';
import { Endpoint } from 'src/common/endpoints';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { ToDoModel } from 'src/app/models/todo.model';
import { ToDoState } from 'src/common/todostate';
import { AddToDoModel } from 'src/app/models/addtodo.model';
import { ModifyToDoModel } from 'src/app/models/modifytodo.model';
import { PaginatedResult } from 'src/app/models/paginatedresult.model';

@Injectable({
    providedIn: 'root'
})

export class ToDoService {
    private baseUrl = environment.settings.backend;
    constructor(
        private http: HttpClient,
        private alertService: AlertService
    ) { }

    public Add(task: AddToDoModel): void {
        this.http
            .post<AddToDoModel>(`${this.baseUrl}/${Endpoint.ToDo.Base}`, task)
            .toPromise()
            .catch((reason) => {
                this.alertService.error(reason.message);
            });
    }

    public Modify(id: string, model: ModifyToDoModel): void {
        this.http
            .patch<ModifyToDoModel>(`${this.baseUrl}/${Endpoint.ToDo.Base}/${Endpoint.ToDo.Modify}/${id}`, model)
            .toPromise()
            .catch((reason) => {
                this.alertService.error(reason.message);
            });
    }

    public Delete(id: string): void {
        this.http
            .delete(`${this.baseUrl}/${Endpoint.ToDo.Base}/${id}`)
            .toPromise()
            .catch((reason) => {
                this.alertService.error(reason.message);
            });
    }

    public Finish(id: string): void {
        this.http
            .patch(`${this.baseUrl}/${Endpoint.ToDo.Base}/${Endpoint.ToDo.Finish}/${id}`, null)
            .toPromise()
            .catch((reason) => {
                this.alertService.error(reason.message);
            });
    }

    public Search(pattern: string, state: ToDoState, page: number, then: (res: PaginatedResult<ToDoModel>) => void): void {
        this.http
            .get<PaginatedResult<ToDoModel>>(`${this.baseUrl}/${Endpoint.ToDo.Base}/${Endpoint.ToDo.Search}/${pattern}/${state}/${page}`)
            .toPromise()
            .then((res) => {
                then(res);
            })
            .catch((reason) => {
                this.alertService.error(reason.message);
            });
    }
}
