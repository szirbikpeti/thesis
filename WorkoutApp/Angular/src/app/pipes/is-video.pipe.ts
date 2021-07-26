import { Pipe, PipeTransform } from '@angular/core';

@Pipe({
  name: 'isVideo'
})
export class IsVideoPipe implements PipeTransform {

  transform(format: string): boolean {
    return format.startsWith('video');
  }
}
