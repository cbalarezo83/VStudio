import { Component, Inject, OnInit } from "@angular/core";
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from "@angular/router"
import { HttpClient } from "@angular/common/http";

@Component({
    selector: "question-edit",
    templateUrl: './question-edit.component.html',
    styleUrls: ['./question-edit.component.css']
})

export class QuestionEditComponent {
    title: string;
    question: Question;
    editMode: boolean;
    form: FormGroup;
    activityLog: string;


    constructor(private activatedRoute: ActivatedRoute,
        private router: Router,
        private http: HttpClient,
        private fb: FormBuilder,
        @Inject('BASE_URL') private baseUrl: string) {

        this.question = <Question>{};

        this.createForm();

        var id = +this.activatedRoute.snapshot.params["id"];

        this.editMode = (this.activatedRoute.snapshot.url[1].path === "edit");

        if (this.editMode) {

            var url = this.baseUrl + "api/question/" + id;

            this.http.get<Question>(url).subscribe(res => {
                this.question = res;
                this.title = "Edit - " + this.question.Text;

                this.updateForm();

            }, error => console.error(error));
        } else {
            this.question.QuizId = id;
            this.title = "Create a new question";
        }
    }

    createForm() {
        this.form = this.fb.group({
            Text: ['', Validators.required]
        });

        this.activityLog = '';
        this.log("Form has been initiailized ");

        //react to form changes 

        this.form.valueChanges.
            subscribe(val => {
                if (!this.form.dirty) {
                    this.log("Form Model has been loaded");
                } else {
                    this.log("Form updated by the user");
                }
            });

        // react to changes in the form.Text control
        this.form.get("Text")!.valueChanges
            .subscribe(val => {
                if (!this.form.dirty) {
                    this.log("Text control has been loaded with initial values.");
                }
                else {
                    this.log("Text control was updated by the user.");
                }
            });

    }

    log(str: string) {
        this.activityLog += "[" + new Date().toLocaleDateString() + "] " + str + "<br />";
    }

    updateForm() {
        this.form.setValue({
            Text: this.question.Text || ''
        });
    }

    //onSubmit(question: Question) {
    onSubmit() {

        var tempQuestion = <Question>{};
        tempQuestion.Text = this.form.value.Text;
        tempQuestion.QuizId = this.question.QuizId;

        var url = this.baseUrl + "api/question";

        if (this.editMode) {

            tempQuestion.Id = this.question.Id;

            this.http.
                post<Question>(url, tempQuestion).
                subscribe(res => {
                    var v = res;
                    console.log("Question " + v.Id + " has been updated.");
                    this.router.navigate(["quiz/edit", v.QuizId]);
                }, error => console.log(error));
        }
        else {
            this.http.
                put<Question>(url, tempQuestion).
                subscribe(res => {
                    var v = res;
                    console.log("Question " + v.Id + " has been created.");
                    this.router.navigate(["quiz/edit", v.QuizId]);
                }, error => console.log(error));
        }
    }

    onBack() {
        this.router.navigate(["quiz/edit", this.question.QuizId]);
    }

    getFormControl(name: string) {
        return this.form.get(name);
    }

    hasError(name: string) {
        var e = this.getFormControl(name);
        return e && (e.dirty || e.touched) && !e.valid;
    }

}