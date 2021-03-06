﻿import { Component, Inject, OnChanges, SimpleChanges,Input} from "@angular/core";
import { Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";

@Component({
    selector: "result-list",
    templateUrl: './result-list.component.html', 
    styleUrls: ['./result-list.component.css']
})

export class ResultListComponent implements OnChanges {
    @Input() quiz: Quiz;
    results:  Result[];
    title: string;

    constructor(private http: HttpClient,
        @Inject('BASE_URL') private baseUrl: string,
        private router: Router
    ) {
        this.results = [];
    }

    ngOnChanges(changes: SimpleChanges) {

        if (typeof changes['quiz'] !== "undefined") {
            var change = changes["quiz"];

            if (!change.isFirstChange()) { this.loadData();}
        }

    }

    loadData() {
       
        var url = this.baseUrl + "api/result/All/" + this.quiz.Id;
        this.http.
            get<Result[]>(url).
            subscribe(res => {
                this.results = res;
            },error=> console.log(error));
    }

    onCreate() {
        this.router.navigate(["/result/create", this.quiz.Id]);
    }

    onEdit(result:Result) {
        this.router.navigate(["/result/edit", result.Id]);
    }

    onDelete(result: Result) {
        if (confirm("Do you really want to delete this Result ?")) {
            var url = this.baseUrl + "api/result/" + result.Id;

            this.http.
                delete(url, { responseType: 'text' }).
                subscribe(res => {
                    this.loadData();
                }, error => console.log(error));
        }
    }
}