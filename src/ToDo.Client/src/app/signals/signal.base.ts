export abstract class SignalBase{
    public static readonly ToDoAdded = 'ToDoAdded';
    public static readonly ToDoModified = 'ToDoModified';
    public static readonly ToDoFinished = 'ToDoFinished';
    public static readonly ToDoDeleted = 'ToDoDeleted';

    public readonly className: string;
    constructor(className: string) {
        this.className = className;
    }
}
