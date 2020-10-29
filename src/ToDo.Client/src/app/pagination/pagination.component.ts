import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';

@Component({
  selector: 'app-pagination',
  templateUrl: './pagination.component.html',
  styleUrls: ['./pagination.component.scss']
})
export class PaginationComponent implements OnInit {

  pages: Array<number>;
  current: number;

  @Input() set pagesNumber(value: number) {
    this.pages = Array.from(Array(value).keys());
  }

  @Input() set currentPage(value: number) {
    this.current = value;
  }

  @Output() pageChangeEvent = new EventEmitter<number>();

  constructor() { }

  ngOnInit(): void {
  }

  onPageChange(page: number): void {
    this.pageChangeEvent.emit(page);
  }

}
