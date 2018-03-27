import { Component, Inject } from "@angular/core";
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router";
import { HttpClient } from "@angular/common/http";

@Component({
    selector: "result-edit",
    templateUrl: './result-edit.component.html',
    styleUrls: ['./result-edit.component.css']
})

export class ResultEditComponent {
    title: string;
    result: Result;
    editMode: Boolean;
    form: FormGroup;

    constructor(private activatedRoute: ActivatedRoute,
                private router: Router,
                private http: HttpClient,
                private fb: FormBuilder,
                @Inject('BASE_URL') private baseUrl: string
    ) {

        this.result = <Result>{};

        this.createForm();

        var id = this.activatedRoute.snapshot.params["id"];
        this.editMode = (this.activatedRoute.snapshot.url[1].path == "edit");

        if (this.editMode) {
            var url = this.baseUrl + "api/result/" + id;

            this.http.
                get<Result>(url).
                subscribe(res => {
                    this.result = res;
                    this.title = "Edit Result - " + id;

                    this.updateForm();

                }, error => console.log(error));
            

        } else {
            this.result.QuizId = id;
            this.title = "Create a new result";
        }

    }

    createForm() {
        this.form = this.fb.group({
            Text : ['', Validators.required],
            MinValue: ['', Validators.pattern(/^\d*$/)],
            MaxValue: ['', Validators.pattern(/^\d*$/)]
        });
    }

    updateForm() {
        this.form.setValue({
            Text: this.result.Text,
            MinValue: this.result.MinValue || '',
            MaxValue: this.result.MaxValue || ''
        });
    }

    onSubmit() {

        var url = this.baseUrl + "api/result";
        var tempResult = <Result>{};
        tempResult.Text = this.form.value.Text;
        tempResult.MinValue = this.form.value.MinValue;
        tempResult.MaxValue = this.form.value.MaxValue;
        tempResult.QuizId = this.result.QuizId;

        if (this.editMode) {

            tempResult.Id = this.result.Id;

            this.http.
                post<Result>(url, tempResult).
                subscribe(res => {
                    console.log("Result " + res.Id + " has been updated.");
                    this.router.navigate(["quiz/edit", res.QuizId]);
                }, error => console.log(error));
        } else {
            this.http.
                put<Result>(url, tempResult).
                subscribe(res => {
                    var v = res;
                    console.log("Result " + v.Id + " has been created.");
                    this.router.navigate(["quiz/edit", v.QuizId]);
                }, error => console.log(error));
        }
    }

    onBack() {
        this.router.navigate(["quiz/edit", this.result.QuizId]);
    }

    getFormControl(name: string) {
        return this.form.get(name);
    }

    hasError(name: string) {
        var e = this.getFormControl(name);
        return e && (e.dirty || e.touched) && !e.valid;
    }
}