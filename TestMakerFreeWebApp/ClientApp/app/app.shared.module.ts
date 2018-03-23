import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';

import { AppComponent } from './components/app/app.component';
import { NavMenuComponent } from './components/navmenu/navmenu.component';
import { HomeComponent } from './components/home/home.component';

import { QuizComponent } from './components/quiz/quiz.component';
import { QuizListComponent } from './components/quiz/quiz-list.component';
import { QuizEditComponent } from './components/quiz/quiz-edit.component';

import { QuestionListComponent } from './components/question/question-list.component';
import { QuestionEditComponent } from './components/question/question-edit.component';

import { AnswerListComponent } from './components/answer/answer-list.component';
import { AnswerEditComponent } from './components/answer/answer-edit.component';

import { ResultListComponent } from './components/result/result-list.component';
import { ResultEditComponent } from './components/result/result-edit.component';

import { AboutComponent } from './components/about/about.component';
import { LoginComponent } from './components/login/login.component';
import { PageNotFoundComponent } from './components/pagenotfound/pagenotfound.component';

//import { FetchDataComponent } from './components/fetchdata/fetchdata.component';
//import { CounterComponent } from './components/counter/counter.component';
        //AnswerEditComponent,

@NgModule({
    declarations: [
        AppComponent,
        NavMenuComponent,
        HomeComponent,
        QuizComponent,
        QuizListComponent,
        QuizEditComponent,
        QuestionListComponent,
        QuestionEditComponent,
        AnswerListComponent,
        AnswerEditComponent,
        ResultListComponent,
        ResultEditComponent,
        AboutComponent,
        LoginComponent,
        PageNotFoundComponent
    ],
    imports: [
        CommonModule,
        HttpClientModule,
        FormsModule,
        RouterModule.forRoot([
            { path: '', redirectTo: 'home', pathMatch: 'full' },

            { path: 'home', component: HomeComponent },
            { path: 'login', component: LoginComponent },

            { path: 'quiz/:id', component: QuizComponent },
            { path: 'quiz/create', component: QuizEditComponent },
            { path: 'quiz/edit/:id', component: QuizEditComponent },


            { path: 'question/:id', component: QuestionListComponent },
            { path: 'question/create/:id', component: QuestionEditComponent },
            { path: 'question/edit/:id', component: QuestionEditComponent },

            { path: 'answer/:id', component: AnswerListComponent },
            { path: 'answer/create/:id', component: AnswerEditComponent },
            { path: 'answer/edit/:id', component: AnswerEditComponent },

            { path: 'result/create/:id', component: ResultEditComponent },
            { path: 'result/edit/:id', component: ResultEditComponent  },

            { path: 'about', component: AboutComponent },
            { path: '**', component : PageNotFoundComponent }
        ])
    ]
})
export class AppModuleShared {
}

        //CounterComponent,
        //FetchDataComponent,
        //QuizListComponent,
        //QuizEditComponent,



            //{ path: 'fetch-data', component: FetchDataComponent },
            //{ path: '**', redirectTo: 'home' }
            //{ path: 'quiz/create', component: QuizEditComponent },
            //{ path: 'quiz/edit/:id', component: QuizEditComponent },