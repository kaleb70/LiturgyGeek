import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes, UrlMatchResult, UrlSegment } from '@angular/router'

import { AppComponent } from './app.component';
import { CalendarComponent } from './calendar/calendar.component';

const calendarMatcher = (segments: UrlSegment[]): UrlMatchResult => {
  if (segments.length >= 1 && segments[0].path == 'calendar') {
    switch (segments.length) {
      case 1:
        return {
          consumed: segments,
          posParams: {}
        }
      case 2:
        return {
          consumed: segments,
          posParams: { calendarKey: segments[1] }
        }
      case 4:
        return {
          consumed: segments,
          posParams: {
            calendarKey: segments[1],
            year: segments[2],
            month: segments[3]
          }
        }
    }
  }
  return <UrlMatchResult>(null as any);
}

const routes: Routes = [
  { matcher: calendarMatcher, component: CalendarComponent },
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
