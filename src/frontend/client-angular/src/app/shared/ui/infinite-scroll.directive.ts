import {
    Directive,
    HostListener,
    Output,
    EventEmitter,
    ElementRef,
} from '@angular/core';

@Directive({
    selector: '[appInfiniteScroll]',
    standalone: true,
})
export class InfiniteScrollDirective {
    @Output() scrolled = new EventEmitter<void>();
    private threshold = 300;

    constructor(private el: ElementRef) {}

    @HostListener('window:scroll', ['$event'])
    onScroll(): void {
        const element = this.el.nativeElement;
        const windowHeight = window.innerHeight;
        const elementBottom = element.getBoundingClientRect().bottom;

        if (elementBottom - windowHeight < this.threshold) {
            this.scrolled.emit();
        }
    }
}
