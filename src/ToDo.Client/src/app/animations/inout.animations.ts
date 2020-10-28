import { trigger, transition, animate, style } from '@angular/animations';

export class InOutAnimations {
    public static readonly Fade = trigger('fade', [
        transition('void => *', [
            style({ opacity: 0 }),
            animate(600, style({ opacity: 1 }))
        ]),
        transition('* => void', [
            animate(600, style({ opacity: 0 }))
        ])
    ]);
}
