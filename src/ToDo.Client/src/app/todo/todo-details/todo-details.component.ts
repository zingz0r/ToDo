import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { ToDoModel } from 'src/app/models/todo.model';

@Component({
  selector: 'app-todo-details',
  templateUrl: './todo-details.component.html',
  styleUrls: ['./todo-details.component.scss']
})
export class ToDoDetailsComponent implements OnInit {

  get toDoItems(): ToDoModel[] {
    return this.datasource.data;
  }
  @Input() set toDoItems(value: ToDoModel[]) {
    this.datasource.data = value;
  }

  @Output() editEvent = new EventEmitter<ToDoModel>();
  @Output() deleteEvent = new EventEmitter<ToDoModel>();
  @Output() finishedEvent = new EventEmitter<ToDoModel>();

  datasource = new MatTableDataSource<ToDoModel>();
  displayedColumns: string[] = ['isfinished', 'task', 'operations'];

  constructor() { }

  ngOnInit(): void {
  }

  onEdit(model: ToDoModel): void {
    this.editEvent.emit(model);
  }

  onDelete(model: ToDoModel): void {
    this.deleteEvent.emit(model);
  }

  onFinish(model: ToDoModel): void {
    if (!model.isFinished) {
      this.finishedEvent.emit(model);
    }
  }

}
