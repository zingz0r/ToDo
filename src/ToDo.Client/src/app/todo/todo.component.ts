import { Component, OnInit } from '@angular/core';
import { SearchModel } from 'src/app/models/search.model';
import { ToDoService } from 'src/app/services/todo.service';
import { ToDoState } from 'src/common/todostate';
import { ToDoModel } from '../models/todo.model';

@Component({
  selector: 'app-todo',
  templateUrl: './todo.component.html',
  styleUrls: ['./todo.component.scss']
})
export class ToDoComponent implements OnInit {

  constructor(private todoService: ToDoService) { 
    this.todoService.Search('*', ToDoState.Any, (res: ToDoModel[]) => {
      this.toDoItems = res;
     });
  }

  toDoItems: ToDoModel[];

  ngOnInit(): void {
  }

  onSearch(event: SearchModel): void {
    this.todoService.Search(event.pattern, event.state, (res: ToDoModel[]) => {
      this.toDoItems = res;
     });
  }
}
