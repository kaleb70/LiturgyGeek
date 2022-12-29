import { Component, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';

@Component({
  selector: 'app-calendar',
  templateUrl: './calendar.component.html',
  styleUrls: ['./calendar.component.css']
})
export class CalendarComponent {
  public month?: CalendarMonth;

  constructor(http: HttpClient) {
    //http.get<WeatherForecast[]>('/weatherforecast').subscribe(result => {
    http.get<CalendarMonth>('https://localhost:7246/Calendar/oca/2023/1').subscribe(result => {
      this.month = result;
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
