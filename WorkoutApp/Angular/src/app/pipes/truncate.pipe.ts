import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'truncate'
})
export class TruncatePipe implements PipeTransform {

  transform(value: string, length: number = 8): string {
    if (value.length < length) {
      return value;
    }

    const newValue = value.substring(0, length);

    if (newValue === value) {
      return value;
    }

    return newValue + '...';
  }
}
