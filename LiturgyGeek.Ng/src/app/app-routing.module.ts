import { NgModule } from '@angular/core';
import { RouterModule, Routes, UrlMatchResult, UrlSegment } from '@angular/router'
import { DayComponent } from './day/day.component';
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

const dayMatcher = (segments: UrlSegment[]): UrlMatchResult => {

  if (segments.length > 2
    && segments[0].path.toLowerCase() == "calendar"
    && segments[2].path.toLowerCase() == "day") {

    switch (segments.length) {
      case 3:
        return {
          consumed: segments,
          posParams: { calendarKey: segments[1] }
        };

      case 6:
        return {
          consumed: segments,
          posParams: {
            calendarKey: segments[1],
            year: segments[3],
            month: segments[4],
            day: segments[5]
          }
        }
    }
  }
  return <UrlMatchResult>(null as any);
}

const routes: Routes = [
  { matcher: monthMatcher, component: MonthComponent },
  { matcher: dayMatcher, component: DayComponent },
  { path: '', redirectTo: '/calendar', pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
