import { Component, OnInit } from '@angular/core';
import { Router, RouterLink } from '@angular/router';

@Component({
    selector: 'app-home',
    templateUrl: './home.component.html',
    styleUrls: ['./home.component.scss'],
    imports: [RouterLink]
})
export class HomeComponent implements OnInit {
    currentSlide = 0;
    slidesCount = 4;

    constructor(private router: Router) {}

    ngOnInit() {
        setInterval(() => {
            this.nextSlide();
        }, 5000);
    }

    nextSlide() {
        this.currentSlide = (this.currentSlide + 1) % this.slidesCount;
    }

    goToSlide(index: number) {
        this.currentSlide = index;
    }

    navigateTo(path: string) {
        this.router.navigate([path]);
    }
}
