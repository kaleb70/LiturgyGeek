import { AfterViewChecked, Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router'
import { Tooltip } from 'bootstrap';
import 'src/lib/date.extensions';

@Component({
  selector: 'app-calendar',
  templateUrl: './month.component.html',
  styleUrls: ['./month.component.css']
})
export class MonthComponent implements AfterViewChecked {
  public calendarKey: string = "";
  public year: number = 0;
  public month: number = 0;

  public isDefaultDate: boolean = false;

  private dateShown: Date = new Date();
  private today: Date = new Date().asDateOnly();

  public prevMonth: Date = this.today.addMonths(-1);
  public nextMonth: Date = this.today.addMonths(1);

  public result?: CalendarMonth;

  private tooltipsInvalid: boolean;

  constructor(http: HttpClient, route: ActivatedRoute) {

    this.tooltipsInvalid = false;

    route.params.subscribe(params => {
      this.today = new Date().asDateOnly();

      this.calendarKey = params['calendarKey'] ?? 'oca';
      this.year = +(params['year'] ?? this.today.getFullYear());
      this.month = +(params['month'] ?? this.today.getMonth() + 1);

      this.isDefaultDate = params['year'] == null;

      this.dateShown = new Date(this.year, this.month - 1, 1);
      this.prevMonth = this.dateShown.addMonths(-1);
      this.nextMonth = this.dateShown.addMonths(1);

      http.get<CalendarMonth>(`https://localhost:7246/Calendar/${this.calendarKey}/${this.year}/${this.month}`).subscribe(result => {
        this.result = result;
        this.tooltipsInvalid = true;
      }, error => console.error(error));
    });
  }
  ngAfterViewChecked(): void {
    if (this.tooltipsInvalid) {
      var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
      tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new Tooltip(tooltipTriggerEl);
      });
      this.tooltipsInvalid = false;
    }
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

interface CalendarMonth {
  traditionKey: string;
  calendarKey: string;
  year: number;
  month: number;
  monthName: string;
  weeks: CalendarWeekSummary[];
}

interface CalendarWeekSummary {
  days: CalendarDaySummary[];
  hasHeadlines: boolean;
}

interface CalendarDaySummary {
  year: number;
  month: number;
  day: number;
  monthName: string;
  headlines: CalendarDaySummaryItem[];
  items: CalendarDaySummaryItem[];
  headingClass: string;
}

interface CalendarDaySummaryItem {
  summary: string;
  elaboration: string;
  class: string;
}
