import { EditDialogConfigBase } from './edit.base';

export class TextEditDialogConfig extends EditDialogConfigBase{
    initialText: string;
    minLength: 3;

    constructor(init?: Partial<any>) {
        super(init);
    }
}
