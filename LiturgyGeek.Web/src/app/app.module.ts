import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { RouterModule, Routes, UrlMatchResult, UrlSegment } from '@angular/router'

import { AppComponent } from './app.component';
import { MonthComponent } from './month/month.component';

const monthMatcher = (segments: UrlSegment[]): UrlMatchResult => {

  if (segments.length > 0
      && segments[0].path.toLowerCase() == "calendar"
      && (segments.length <= 2 || segments[2].path.toLowerCase() == "month")) {

    switch (segments.length) {
      case 1:
        return {
          consumed: segments,
          posParams: {}
        };

      case 2:
      case 3:
        return {
          consumed: segments,
          posParams: { calendarKey: segments[1] }
        };

      case 5:
        return {
          consumed: segments,
          posParams: {
            calendarKey: segments[1],
            year: segments[3],
            month: segments[4]
          }
        }
    }
  }
  return <UrlMatchResult>(null as any);
}

const routes: Routes = [
  { matcher: monthMatcher, component: MonthComponent },
  { path: '', redirectTo: '/calendar', pathMatch: 'full' },
]

@NgModule({
  declarations: [
    AppComponent,
    MonthComponent
  ],
  imports: [
    BrowserModule, HttpClientModule, RouterModule.forRoot(routes)
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
