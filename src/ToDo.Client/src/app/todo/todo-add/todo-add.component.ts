import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';

@Component({
  selector: 'app-todo-add',
  templateUrl: './todo-add.component.html',
  styleUrls: ['./todo-add.component.scss']
})
export class ToDoAddComponent implements OnInit {

  @Output() addEvent = new EventEmitter<string>();

  addForm: FormGroup;

  placeBetForm: FormGroup;

  constructor(private formBuilder: FormBuilder) {
    this.addForm = this.formBuilder.group({
      task: ['', [Validators.required, Validators.min(1)]]
    });
   }

  ngOnInit(): void {
  }

  onAdd(): void {
    this.addEvent.emit(this.addForm.value.task);
  }
}
