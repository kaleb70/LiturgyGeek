import { AfterContentInit, AfterViewChecked, Component } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router'
import { Tooltip } from 'bootstrap';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent implements AfterViewChecked {
  public calendarKey: string = "";
  public year: number = 0;
  public month: number = 0;

  public result?: CalendarMonth;

  private tooltipsInvalid: boolean;

  constructor(http: HttpClient, route: ActivatedRoute) {
    //const now = new Date();
    ////http.get<WeatherForecast[]>('/weatherforecast').subscribe(result => {
    //this.calendarKey = route.snapshot.paramMap.get('calendarKey') ?? 'oca';
    //this.year = +(route.snapshot.paramMap.get('year') ?? now.getFullYear());
    //this.month = +(route.snapshot.paramMap.get('month') ?? now.getMonth() + 1);
    this.tooltipsInvalid = false;

    route.params.subscribe(params => {
      const now = new Date();
      this.calendarKey = params['calendarKey'] ?? 'oca';
      this.year = +(params['year'] ?? now.getFullYear());
      this.month = +(params['month'] ?? now.getMonth() + 1);

      http.get<CalendarMonth>(`https://localhost:7246/Calendar/${this.calendarKey}/${this.year}/${this.month}`).subscribe(result => {
        this.result = result;
        this.tooltipsInvalid = true;
      }, error => console.error(error));
    });

    //http.get<CalendarMonth>(`https://localhost:7246/Calendar/${this.calendarKey}/${this.year}/${this.month}`).subscribe(result => {
    //  this.result = result;
    //}, error => console.error(error));
  }
  ngAfterViewChecked(): void {
    if (this.tooltipsInvalid) {
      var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'))
      var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new Tooltip(tooltipTriggerEl);
      });
      this.tooltipsInvalid = false;
    }
  }

  getDateClasses(day: CalendarDaySummary): string {
    const now = new Date();

    if (day.year == now.getFullYear() && day.month == now.getMonth() + 1 && day.day == now.getDate())
      return "calendar-cell-today";

    if (day.year != this.year || day.month != this.month)
      return "calendar-cell-outside-month";

    return "";
  }

  title = 'LiturgyGeek.Web';
}

interface CalendarMonth {
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
  headlines: CalendarDayLineItem[];
  items: CalendarDayLineItem[];
  headingClass: string;
}

interface CalendarDayLineItem {
  summary: string;
  elaboration: string;
  class: string;
}
