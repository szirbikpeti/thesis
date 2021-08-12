import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'truncate'
})
export class TruncatePipe implements PipeTransform {

  transform(value: string, length: number = 8): string {
    if (value.length < length) {
      return value;
    }

    const truncateValue = value.substring(0, length);
    const result = truncateValue + '...';

    if (truncateValue === value  || result.length >= value.length) {
      return value;
    }

    return result;
  }
}
