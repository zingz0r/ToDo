import { Component, OnDestroy, OnInit } from '@angular/core';
import { SearchModel } from 'src/app/models/search.model';
import { SignalRService } from 'src/app/services/signalr.service';
import { ToDoService } from 'src/app/services/todo.service';
import { ToDoState } from 'src/common/todostate';
import { ToDoModel } from '../models/todo.model';
import { ToDoAdded } from '../signals/todo.signals';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class ToDoComponent implements OnInit, OnDestroy {

  filterState = ToDoState.Any;

  constructor(private todoService: ToDoService, private signalRService: SignalRService) {
    this.todoService.Search('*', ToDoState.Any, (res: ToDoModel[]) => {
      this.toDoItems = res;
    });
  }

  toDoItems: ToDoModel[];

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
    });
  }

  ngOnDestroy(): void {
    this.signalRService.stopListeningTo(ToDoAdded);
    this.signalRService.stopConnection();
  }

  onAdd(event: string): void
  {
    this.todoService.Add({task: event});
  }

  onSearch(event: SearchModel): void {
    this.todoService.Search(event.pattern, event.state, (res: ToDoModel[]) => {
      this.toDoItems = res;
      this.filterState = event.state;
    });
  }
}
