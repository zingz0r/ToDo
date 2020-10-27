import {SignalBase} from './signal.base';

export class ToDoAdded extends SignalBase {
    id: string;
    isFinished: boolean;
    task: string;
    created: Date;

    constructor() {
        super(SignalBase.ToDoAdded);
    }
}
