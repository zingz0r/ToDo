import { Component, Input, OnInit } from '@angular/core';
import { FormControl, FormGroup } from '@angular/forms';
import { Output, EventEmitter } from '@angular/core';
import { SearchModel } from 'src/app/models/search.model';
import { ToDoState } from 'src/common/todostate';

interface State {
  value: string;
  viewValue: string;
}

@Component({
  selector: 'app-todo-search',
  templateUrl: './todo-search.component.html',
  styleUrls: ['./todo-search.component.scss']
})
export class ToDoSearchComponent implements OnInit {

  @Input() set defaultPattern(value: string) {
    if (value !== '*') {
      this.searchForm.get('pattern').setValue(value);
    }
  }

  @Input() set defaultState(value: string) {
    if (value !== '*') {
      this.selectedState = value;
    }
  }

  @Output() searchEvent = new EventEmitter<SearchModel>();

  selectedState = ToDoState.Any;

  todoStates: State[] = [
    { value: ToDoState.Any, viewValue: ToDoState.Any },
    { value: ToDoState.Finished, viewValue: ToDoState.Finished },
    { value: ToDoState.Ongoing, viewValue: ToDoState.Ongoing }
  ];

  searchForm = new FormGroup({
    pattern: new FormControl(),
    state: new FormControl(),
  });

  constructor() { }

  ngOnInit(): void {
  }

  onSearch(): void {
    this.searchEvent.emit({ pattern: this.searchForm.value.pattern ? this.searchForm.value.pattern : '*', state: this.selectedState });
  }

}
