import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { TextEditDialogConfig } from './edit.model';

@Component({
    selector: 'app-edit-dialog',
    templateUrl: './edit.component.html',
    styleUrls: ['./edit.component.scss']
})

export class EditDialogComponent implements OnInit {

    form: FormGroup;

    constructor(public dialogRef: MatDialogRef<EditDialogComponent>, @Inject(MAT_DIALOG_DATA) public data: any, private fb: FormBuilder) {
        const config = this.data as TextEditDialogConfig;

        this.form = this.fb.group({
            value: [config.initialText ? config.initialText : '', [Validators.required, Validators.minLength(config.minLength)]]
        });
    }

    ngOnInit(): void {}

    onCancelClick(): void {
        this.dialogRef.close();
    }

    onOKClick(): void {
        if (this.form.valid) {
            if (this.form.controls.descValue) {
                this.dialogRef.close({name: this.form.controls.value.value, description: this.form.controls.descValue.value});
            } else {
                this.dialogRef.close(this.form.controls.value.value);
            }
        }
    }
}
