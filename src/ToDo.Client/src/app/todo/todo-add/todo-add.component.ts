import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormControl } from '@angular/forms';

@Component({
  selector: 'app-todo-add',
  templateUrl: './todo-add.component.html',
  styleUrls: ['./todo-add.component.scss']
})
export class ToDoAddComponent implements OnInit {

  constructor() { }

  @Output() addEvent = new EventEmitter<string>();

  addForm = new FormGroup({
    task: new FormControl(),
  });

  ngOnInit(): void {
  }

  onAdd(): void {
    this.addEvent.emit(this.addForm.value.task);
  }
}
