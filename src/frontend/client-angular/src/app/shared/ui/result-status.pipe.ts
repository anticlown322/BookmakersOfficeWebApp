import { Pipe, PipeTransform } from '@angular/core';
import { ResultStatus } from '../../core/models/sport-data-service/entities/match-result/result-status.enum';

@Pipe({ name: 'resultStatus', standalone: true })
export class ResultStatusPipe implements PipeTransform {
  transform(status: ResultStatus): string {
    switch(status) {
      case ResultStatus.Running: return 'В процессе';
      case ResultStatus.Ended: return 'Завершён';
      case ResultStatus.Interrupted: return 'Прерван';
      case ResultStatus.Canceled: return 'Отменён';
      default: return 'Неизвестно';
    }
  }
}