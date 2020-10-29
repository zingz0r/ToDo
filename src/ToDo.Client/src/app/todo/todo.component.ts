import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { SearchModel } from 'src/app/models/search.model';
import { SignalRService } from 'src/app/services/signalr.service';
import { ToDoService } from 'src/app/services/todo.service';
import { ToDoState } from 'src/common/todostate';
import { ConfirmDialogComponent } from '../dialogs/confirm/confirm.component';
import { ConfirmDialogConfig } from '../dialogs/confirm/confirm.model';
import { EditDialogComponent } from '../dialogs/edit/edit.component';
import { TextEditDialogConfig } from '../dialogs/edit/edit.model';
import { PaginatedResult } from '../models/paginatedresult.model';
import { ToDoModel } from '../models/todo.model';
import { ToDoAdded, ToDoDeleted, ToDoFinished, ToDoModified } from '../signals/todo.signals';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class ToDoComponent implements OnInit, OnDestroy {

  filterState: string;
  searchPattern: string;

  pagesNumber: number;
  currentPageNumber: number;

  isLoading: boolean;

  toDoItems: ToDoModel[];

  editDialogRef: MatDialogRef<EditDialogComponent, any>;
  confirmDialogRef: MatDialogRef<ConfirmDialogComponent, any>;

  navigationSubscription: Subscription;

  constructor(
    private todoService: ToDoService,
    private signalRService: SignalRService,
    private dialog: MatDialog,
    private router: Router,
    activatedRoute: ActivatedRoute
  ) {
    this.navigationSubscription = this.router.events.subscribe((ev: any) => {
      if (ev instanceof NavigationEnd) {
        const page = activatedRoute.snapshot.queryParamMap.get('page');
        this.currentPageNumber = page ? Number(page) : 0;

        const state = activatedRoute.snapshot.queryParamMap.get('state');
        this.filterState = state ? state : ToDoState.Any;

        const pattern = activatedRoute.snapshot.queryParamMap.get('pattern');
        this.searchPattern = pattern ? pattern : '*';

        this.isLoading = true;
        this.todoService.Search(this.searchPattern, this.filterState, this.currentPageNumber, (res: PaginatedResult<ToDoModel>) => {
          this.toDoItems = res.result;
          this.pagesNumber = res.allPage;
          this.isLoading = false;
        });
      }
    });
  }

  ngOnInit(): void {
    this.signalRService.startConnection().then(() => {
      this.signalRService.startListeningTo(
        ToDoAdded,
        (signal) => this.filterState === ToDoState.Any ||
          this.filterState === ToDoState.Finished && signal.isFinished ||
          this.filterState === ToDoState.Ongoing && !signal.isFinished,
        (signal) => {
          this.toDoItems.push({ id: signal.id, isFinished: signal.isFinished, created: signal.created, task: signal.task });
          this.toDoItems.sort((a, b) => +new Date(b.created) - +new Date(a.created));

          if (this.toDoItems.length > 25) {
            this.toDoItems.splice(25, this.toDoItems.length - 25)
          }

          this.toDoItems = this.toDoItems.splice(0, this.toDoItems.length);
        });

      this.signalRService.startListeningTo(
        ToDoModified,
        (signal) => this.filterState === ToDoState.Any ||
          this.filterState === ToDoState.Finished && signal.isFinished ||
          this.filterState === ToDoState.Ongoing && !signal.isFinished,
        (signal) => {
          const item = this.toDoItems.find(x => x.id === signal.id);
          if (item) {
            item.task = signal.task;
          }
        });

      this.signalRService.startListeningTo(
        ToDoDeleted,
        (signal) => this.filterState === ToDoState.Any ||
          this.filterState === ToDoState.Finished && signal.isFinished ||
          this.filterState === ToDoState.Ongoing && !signal.isFinished,
        (signal) => {
          const index = this.toDoItems.findIndex(x => x.id === signal.id);
          if (index > -1) {
            this.toDoItems.splice(index, 1);
            this.toDoItems = this.toDoItems.splice(0, this.toDoItems.length);
          }
        });

      this.signalRService.startListeningTo(
        ToDoFinished,
        (signal) => true,
        (signal) => {
          const item = this.toDoItems.find(x => x.id === signal.id);
          if (item) {
            item.isFinished = signal.isFinished;
          }
        });
    });
  }

  ngOnDestroy(): void {
    this.signalRService.stopListeningTo(ToDoAdded);
    this.signalRService.stopListeningTo(ToDoModified);
    this.signalRService.stopListeningTo(ToDoDeleted);
    this.signalRService.stopListeningTo(ToDoFinished);
    this.signalRService.stopConnection();
  }

  onAdd(event: string): void {
    this.todoService.Add({ task: event });
  }

  onEdit(event: ToDoModel): void {
    this.editDialogRef = this.dialog.open(EditDialogComponent, {
      width: '500px',
      data: new TextEditDialogConfig(
        {
          title: 'Edit Task',
          requiredError: 'Task description is required.',
          initialText: event.task
        })
    });
    this.editDialogRef.afterClosed().subscribe((result) => {
      if (result && result !== event.task) {
        this.todoService.Modify(event.id, { task: result });
      }
    });
  }

  onDelete(event: ToDoModel): void {
    this.confirmDialogRef = this.dialog.open(ConfirmDialogComponent, {
      width: '500px',
      data: new ConfirmDialogConfig(
        {
          title: 'Delete Task',
          message: `Are you sure you want delete the following task: \'${event.task}\'`
        })
    });
    this.confirmDialogRef.afterClosed().subscribe((result) => {
      if (result) {
        this.todoService.Delete(event.id);
      }
    });
  }

  onFinish(event: ToDoModel): void {
    this.todoService.Finish(event.id);
  }

  onSearch(event: SearchModel): void {
    this.router.navigate(['/'], { queryParams: { pattern: event.pattern, state: event.state, page: 0 }, queryParamsHandling: 'merge' });
  }

  onPageChange(event: number): void {
    this.router.navigate(['/'], { queryParams: { pattern: this.searchPattern, state: this.filterState, page: event }, queryParamsHandling: 'merge' });
  }
}
