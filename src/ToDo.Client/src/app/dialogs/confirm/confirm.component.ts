import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { ConfirmDialogConfig } from './confirm.model';

@Component({
    selector: 'app-confirm-dialog',
    templateUrl: './confirm.component.html',
    styleUrls: ['./confirm.component.scss']
})

export class ConfirmDialogComponent implements OnInit {

    form: FormGroup;

    constructor(public dialogRef: MatDialogRef<ConfirmDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: any) {
    }

    ngOnInit(): void {}

    onCancelClick(): void {
        this.dialogRef.close();
    }

    onOKClick(): void {
        this.dialogRef.close('Confirmed');
    }
}
