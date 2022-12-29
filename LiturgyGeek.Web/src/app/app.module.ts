import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes } from '@angular/router'

import { AppComponent } from './app.component';
import { CalendarComponent } from './calendar/calendar.component';

const routes: Routes = [
  { path: 'calendar', component: CalendarComponent },
  { path: 'calendar/:calendarKey/:year/:month', component: CalendarComponent },
  { path: '', redirectTo: '/calendar', pathMatch: 'full' },
]

@NgModule({
  declarations: [
    AppComponent,
    CalendarComponent
  ],
  imports: [
    BrowserModule, HttpClientModule, RouterModule.forRoot(routes)
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
