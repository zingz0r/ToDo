export class ConfirmDialogConfig {
    title: string;
    message: string;
    yesIcon = 'check_circle';
    yesCaption = 'OK';
    noIcon = 'cancel';
    noCaption = 'Cancel';

    public constructor(init?: Partial<ConfirmDialogConfig>) {
        Object.assign(this, init);
    }
}
