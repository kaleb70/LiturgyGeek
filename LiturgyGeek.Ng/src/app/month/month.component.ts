import { AfterViewChecked, Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router'
import 'src/lib/date.extensions';
import { CalendarService, CalendarMonth, CalendarDaySummary } from '../services/calendar.service'

@Component({
  selector: 'app-month',
  templateUrl: './month.component.html',
  styleUrls: ['./month.component.css'],
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
      this.today = new Date().asDateOnly();

      this.calendarKey = params['calendarKey'] ?? 'oca';
      this.year = +(params['year'] ?? this.today.getFullYear());
      this.month = +(params['month'] ?? this.today.getMonth() + 1);

      this.isDefaultDate = params['year'] == null;

      this.dateShown = new Date(this.year, this.month - 1, 1);
      this.prevMonth = this.dateShown.addMonths(-1);
      this.nextMonth = this.dateShown.addMonths(1);

      calendarService.getMonth(this.calendarKey, this.year, this.month).subscribe(result => {
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
