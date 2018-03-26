import { Component , Input } from "@angular/core";
import { ViewEncapsulation } from "@angular/compiler/src/core";

@Component({
    selector: "quiz-search",
    templateUrl: './quiz-search.component.html',
    styleUrls: ['./quiz-search.component.css']
    //, encapsulation: ViewEncapsulation.None   usefult to propagate css down
})

export class QuizSearchComponent {
    @Input() class: string;
    @Input() placeholder: string;
}