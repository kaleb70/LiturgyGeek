import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http'
import { Observable, of, throwError } from 'rxjs';
import { catchError, retry } from 'rxjs/operators';

@Injectable({
  providedIn: 'root'
})
export class CalendarService {

  constructor(private http: HttpClient) { }

  getMonth(calendarKey: string, year: number, month: number): Observable<CalendarMonth> {
    return this.http.get<CalendarMonth>(`/api/Calendar/${calendarKey}/${year}/${month}`);
  }
}

export interface CalendarMonth {
  traditionKey: string;
  calendarKey: string;
  year: number;
  month: number;
  monthName: string;
  weeks: CalendarWeekSummary[];
}

export interface CalendarWeekSummary {
  days: CalendarDaySummary[];
  hasHeadlines: boolean;
}

export interface CalendarDaySummary {
  year: number;
  month: number;
  day: number;
  monthName: string;
  headlines: CalendarDaySummaryItem[];
  items: CalendarDaySummaryItem[];
  headingClass: string;
}

export interface CalendarDaySummaryItem {
  summary: string;
  elaboration: string;
  class: string;
}
