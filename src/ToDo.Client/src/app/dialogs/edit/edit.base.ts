export class EditDialogConfigBase {
    title: string;
    label: string;
    hint: string;
    requiredError: string;
    yesIcon = 'check_circle';
    yesCaption = 'OK';
    noIcon = 'cancel';
    noCaption = 'Cancel';

    public constructor(init?: Partial<EditDialogConfigBase>) {
        Object.assign(this, init);
    }
}