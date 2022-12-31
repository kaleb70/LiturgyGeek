import { Component, AfterViewChecked } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ActivatedRoute } from '@angular/router'
import { Tooltip } from 'bootstrap';
import 'src/lib/date.extensions';

@Component({
  selector: 'app-day',
  templateUrl: './day.component.html',
  styleUrls: ['./day.component.css']
})
export class DayComponent implements AfterViewChecked {
  public calendarKey: string = "";
  public year: number = 0;
  public month: number = 0;
  public day: number = 0;

  public isDefaultDate: boolean = false;

  public prevDay: Date = new Date().addDays(-1);
  public nextDay: Date = new Date().addDays(1);

  public result?: CalendarDay;

  private tooltipsInvalid: boolean;

  constructor(http: HttpClient, route: ActivatedRoute) {

    this.tooltipsInvalid = false;

    route.params.subscribe(params => {
      const now = new Date();
      this.calendarKey = params['calendarKey'];
      this.year = +(params['year'] ?? now.getFullYear());
      this.month = +(params['month'] ?? now.getMonth() + 1);
      this.day = +(params['day'] ?? now.getDate());

      this.isDefaultDate = params['year'] == null;

      var queryDate = new Date(this.year, this.month - 1, this.day);
      this.prevDay = queryDate.addDays(-1);
      this.nextDay = queryDate.addDays(1);

      http.get<CalendarDay>(`https://localhost:7246/Calendar/${this.calendarKey}/${this.year}/${this.month}/${this.day}`).subscribe(result => {
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

  getMonthRouterLink(): string {
    var result = "/calendar/" + this.calendarKey;
    if (!this.isDefaultDate)
      result += `/month/${this.year}/${this.month}`;

    return result;
  }
}

interface CalendarDay {
  year: number;
  month: number;
  day: number;
  monthName: string;
  items: CalendarDayItemDetail[];
  headingClass: string;
}

interface CalendarDayItemDetail {
  title: string;
  elaboration: string;
  class: string;
}
