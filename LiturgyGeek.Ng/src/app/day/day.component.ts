import { Component } from '@angular/core';
import { MatDatepickerInputEvent } from '@angular/material/datepicker';
import { Router, ActivatedRoute } from '@angular/router'
import { CalendarService, CalendarDay } from '../services/calendar.service'

@Component({
  selector: 'app-day',
  templateUrl: './day.component.html',
  styleUrls: ['./day.component.scss']
})
export class DayComponent {
  public calendarKey: string = "";
  public year: number = 0;
  public month: number = 0;
  public day: number = 0;

  public isDefaultDate: boolean = false;

  public dateShown: Date = new Date();
  private today: Date = new Date().asDateOnly();

  public prevDay: Date = this.today.addDays(-1);
  public nextDay: Date = this.today.addDays(1);

  public result?: CalendarDay;

  constructor(public router: Router, route: ActivatedRoute, calendarService: CalendarService) {

    route.params.subscribe(params => {

      var today = new Date().asDateOnly();

      var calendarKey = params['calendarKey'] ?? 'oca';
      var year = +(params['year'] ?? this.today.getFullYear());
      var month = +(params['month'] ?? this.today.getMonth() + 1);
      var day = +(params['day'] ?? this.today.getDate());

      calendarService.getDay(calendarKey, year, month, day).subscribe(result => {
        this.today = today;

        this.calendarKey = calendarKey;
        this.year = year;
        this.month = month;
        this.day = day;

        this.isDefaultDate = params['year'] == null;

        this.dateShown = new Date(this.year, this.month - 1, this.day);
        this.prevDay = this.dateShown.addDays(-1);
        this.nextDay = this.dateShown.addDays(1);

        this.result = result;
      }, error => console.error(error));
    });
  }

  public onDateChange(event: MatDatepickerInputEvent<Date>) {
    this.router.navigate([`/calendar/${this.calendarKey}/day/${event.value!.getFullYear()}/${event.value!.getMonth() + 1}/${event.value!.getDate()}`]);
  }

}
