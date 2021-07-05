import { Pipe, PipeTransform } from '@angular/core';
import {DatePipe} from "@angular/common";

@Pipe({
  name: 'dateFormatter'
})
export class DateFormatterPipe implements PipeTransform {

  constructor(private datePipe: DatePipe) {
  }

  transform(value: Date, language: string, isDateTime = true): unknown {
    const date = new Date(value);

    const format = language === 'hu'
      ? 'yyyy.MM.dd' + (isDateTime ? ' HH:mm' : '')
      : (isDateTime ? 'medium' : 'mediumDate');
    return this.datePipe.transform(date, format);
  }

}
