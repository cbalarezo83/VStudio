import { Component, Inject } from "@angular/core";
import { FormGroup, FormControl, FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router} from "@angular/router";
import { HttpClient } from "@angular/common/http";


@Component({
    selector: "answer-edit",
    templateUrl: './answer-edit.component.html',
    styleUrls: ['./answer-edit.component.css']
})


export class AnswerEditComponent {
    title: string;
    answer: Answer;
    editMode: boolean;
    form: FormGroup;

    constructor(private activatedRoute: ActivatedRoute,
                private router:Router,
                private http: HttpClient,   
                private fb:FormBuilder  ,
        @Inject('BASE_URL') private baseUrl: string) {

        this.answer = <Answer>{};
        this.createForm();

        this.editMode = (this.activatedRoute.snapshot.url[1].path == "edit");
        var id = this.activatedRoute.snapshot.params["id"];

        if (this.editMode) {

            var url = this.baseUrl + "api/answer/" + id;

            this.http.get<Answer>(url).subscribe(res => {
                this.answer = res;
                this.title = "Edit - " + this.answer.Text;
                this.updateForm();
            }, error => console.error(error));
        } else {
            this.answer.QuestionId = id;
            this.title = "Create a new answer";
        }

    }

    createForm() {
        this.form = this.fb.group({
            Text: ['', Validators.required],
            Value: ['', [Validators.required, Validators.min(-5), Validators.max(5)]]
        });
    }

    updateForm() {
        this.form.setValue({
            Text: this.answer.Text,
            Value:this.answer.Value
        });
    }

    //onSubmit(answer: Answer) {
    onSubmit() {

        var url = this.baseUrl + "api/answer";

        var tempAnswer = <Answer>{};

        tempAnswer.QuestionId = this.answer.QuestionId;
        tempAnswer.Text = this.form.value.Text;
        tempAnswer.Value = this.form.value.Value;

        if (this.editMode) {

            tempAnswer.Id = this.answer.Id;

            this.http.
                post<Answer>(url, tempAnswer ).
                subscribe(res => {
                    console.log("Answer " + res.Id + " has been updated.");
                    this.router.navigate(["question/edit", res.QuestionId]);
                }, error => console.log(error));

        } else {
            this.http.
                put<Answer>(url, tempAnswer).
                 subscribe(res => {
                    var v = res;
                    console.log("Question " + res.Id + " has been created.");
                    this.router.navigate(["question/edit", res.QuestionId]);
                }, error => console.log(error));
        }
    }

    onBack() {
        this.router.navigate(["question/edit", this.answer.QuestionId]);
    }

    getFormControl(name: string) {
        return this.form.get(name);
    }

    hasError(name: string) {
        var e = this.getFormControl(name);
        return e && (e.dirty || e.touched) && !e.valid;
    }
}