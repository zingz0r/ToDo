export class Alert {
    id: string;
    type: AlertType;
    message: string;
    title: string;
    autoClose: boolean;
    keepAfterRouteChange: boolean;

    constructor(init?: Partial<Alert>) {
        Object.assign(this, init);
    }
}

export enum AlertType {
    Success,
    Error,
    Info,
    Warning
}
