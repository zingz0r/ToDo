import { Component, Input, OnInit } from '@angular/core';
import { MatTableDataSource } from '@angular/material/table';
import { ToDoModel } from 'src/app/models/todo.model';

@Component({
  selector: 'app-todo-details',
  templateUrl: './todo-details.component.html',
  styleUrls: ['./todo-details.component.scss']
})
export class ToDoDetailsComponent implements OnInit {

  constructor() { }

  get toDoItems(): ToDoModel[] {
      return this.datasource.data;
  }
  @Input() set toDoItems(value: ToDoModel[]) {
      this.datasource.data = value;
  }

  datasource = new MatTableDataSource<ToDoModel>();
  displayedColumns: string[] = ['isfinished', 'task', 'operations'];

  ngOnInit(): void {
  }

}
