import { AfterViewChecked, Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router'
import 'src/lib/date.extensions';
import { CalendarService, CalendarMonth, CalendarDaySummary } from '../services/calendar.service'

@Component({
  selector: 'app-month',
  templateUrl: './month.component.html',
  styleUrls: ['./month.component.scss'],
})
export class MonthComponent {
  public calendarKey: string = "";
  public year: number = 0;
  public month: number = 0;

  public isDefaultDate: boolean = false;

  private dateShown: Date = new Date();
  private today: Date = new Date().asDateOnly();

  public prevMonth: Date = this.today.addMonths(-1);
  public nextMonth: Date = this.today.addMonths(1);

  public result?: CalendarMonth;

  constructor(route: ActivatedRoute, calendarService: CalendarService) {

    route.params.subscribe(params => {

      var today = new Date().asDateOnly();

      var calendarKey = params['calendarKey'] ?? 'oca';
      var year = +(params['year'] ?? this.today.getFullYear());
      var month = +(params['month'] ?? this.today.getMonth() + 1);

      calendarService.getMonth(calendarKey, year, month).subscribe(result => {
        this.today = today;

        this.calendarKey = calendarKey;
        this.year = year;
        this.month = month;

        this.isDefaultDate = params['year'] == null;

        this.dateShown = new Date(this.year, this.month - 1, 1);
        this.prevMonth = this.dateShown.addMonths(-1);
        this.nextMonth = this.dateShown.addMonths(1);

        this.result = result;
      }, error => console.error(error));
    });
  }

  getDateClasses(day: CalendarDaySummary): string {
    if (new Date(day.year, day.month - 1, day.day).equals(this.today))
      return "calendar-cell-today";

    if (day.year != this.year || day.month != this.month)
      return "calendar-cell-outside-month";

    return "";
  }

  getDayRouterLink(day: CalendarDaySummary): string {

    var result = `/calendar/${this.calendarKey}/day`;
    if (!(this.isDefaultDate && new Date(day.year, day.month - 1, day.day).equals(this.today)))
      result += `/${day.year}/${day.month}/${day.day}`;

    return result;
  }
  title = 'LiturgyGeek.Web';
}
