import { Component, OnDestroy, OnInit } from '@angular/core';
import { MatDialog, MatDialogRef } from '@angular/material/dialog';
import { SearchModel } from 'src/app/models/search.model';
import { SignalRService } from 'src/app/services/signalr.service';
import { ToDoService } from 'src/app/services/todo.service';
import { ToDoState } from 'src/common/todostate';
import { ConfirmDialogComponent } from '../dialogs/confirm/confirm.component';
import { ConfirmDialogConfig } from '../dialogs/confirm/confirm.model';
import { EditDialogComponent } from '../dialogs/edit/edit.component';
import { TextEditDialogConfig } from '../dialogs/edit/edit.model';
import { ToDoModel } from '../models/todo.model';
import { ToDoAdded, ToDoDeleted, ToDoFinished, ToDoModified } from '../signals/todo.signals';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class ToDoComponent implements OnInit, OnDestroy {

  filterState = ToDoState.Any;
  toDoItems: ToDoModel[];

  editDialogRef: MatDialogRef<EditDialogComponent, any>;
  confirmDialogRef: MatDialogRef<ConfirmDialogComponent, any>;

  constructor(
    private todoService: ToDoService,
    private signalRService: SignalRService,
    private dialog: MatDialog
  ) {
    this.todoService.Search('*', ToDoState.Any, (res: ToDoModel[]) => {
      this.toDoItems = res;
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
    this.todoService.Search(event.pattern, event.state, (res: ToDoModel[]) => {
      this.toDoItems = res;
      this.filterState = event.state;
    });
  }
}
