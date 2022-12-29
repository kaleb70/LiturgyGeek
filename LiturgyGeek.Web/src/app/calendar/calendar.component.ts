import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router'

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent {
  public calendarKey: string;
  public year: number;
  public month: number;

  public result?: CalendarMonth;

  constructor(http: HttpClient, route: ActivatedRoute) {
    const now = new Date();
    //http.get<WeatherForecast[]>('/weatherforecast').subscribe(result => {
    this.calendarKey = route.snapshot.paramMap.get('calendarKey') ?? 'oca';
    this.year = +(route.snapshot.paramMap.get("year") ?? now.getFullYear());
    this.month = +(route.snapshot.paramMap.get("month") ?? now.getMonth() + 1);

    http.get<CalendarMonth>(`https://localhost:7246/Calendar/${this.calendarKey}/${this.year}/${this.month}`).subscribe(result => {
      this.result = result;
    }, error => console.error(error));
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
  headlines: string[];
  items: string[];
  headingClass: string;
}
