import { Component } from '@angular/core';
import * as Highcharts from 'highcharts';
import {WorkoutService} from "../../services/workout.service";
import {WorkoutModel} from "../../models/WorkoutModel";
import {DateFormatterPipe} from "../../pipes/date-formatter.pipe";
import {StateService} from "../../services/state.service";
import {TranslateService} from "@ngx-translate/core";

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss']
})
export class StatisticsComponent {

  chart: Highcharts.Chart;
  pieChart: Highcharts.Chart;

  workouts: WorkoutModel[];

  workoutTypeData = [];
  workoutTypes = [];

  selectedWorkoutType: string = 'Gym';

  constructor(private _workout: WorkoutService, private _state: StateService,
              private _translate: TranslateService, private _dateFormatter: DateFormatterPipe) {
    _workout.list().subscribe(workouts => {
      this.workouts = workouts;

      setTimeout(() => this.getAllData(), 100);
    });

    _state.language.subscribe(() => {
      if (this.chart) {
        this.getAllData();
      }
    });
  }

  private getAllData(): void {
    this.getWorkoutTypeData();
    this.setUpFirstAreaChart();
    this.setUpFirstPieChart();
  }

  private getWorkoutTypeData(): void {
    this.workoutTypes = [];
    this.workoutTypeData = [];
    const workoutTypesName = [];

    this.workouts.forEach(workout => {
      if (!workoutTypesName.includes(workout.type)) {
        this.workoutTypeData.push({ name: this._translate.instant('WORKOUT.TYPES.' + workout.type.toUpperCase()), y: 1});
        workoutTypesName.push(workout.type);
      } else {
        const index = workoutTypesName.indexOf(workout.type);
        this.workoutTypeData[index].y += 1;
      }
    });

    this.workoutTypes = workoutTypesName;
  }

  private setUpFirstAreaChart(): void {
    const that = this;
    const data = this.getFirstAreaChartData(this.selectedWorkoutType);

    this.chart = Highcharts.chart('area-chart', {
      chart: { type: 'area' },
      title: {
        text: that._translate.instant('STATISTICS.SUM_VOLUMEN_IN_ONE_WORKOUT')
      },
      series: [{
        type: 'area',
        name: that._translate.instant('WORKOUT.WORKOUTS'),
        data : data
      }],
      rangeSelector: {
        enabled: true
      },
      navigator: {
        enabled: true,
        series: {
          type: 'area'
        },
        yAxis: {
          min: 0,
          max: 3,
          reversed: true,
          categories: []
        }
      },
      xAxis: {
        type: 'datetime',
        // tickInterval: 1000 * 3600 * 24 *30 // 1 month
      },
      tooltip: {
        useHTML: true,
        formatter: function() {
          const workout: WorkoutModel = data.find(that => that.x === this.x && that.y === this.y).workout;
          let result = `<b>${that._translate.instant('WORKOUT.DATE')}:</b> ${that._dateFormatter.transform(workout.date, that._state.language.value, false)} <br>`;

          if (workout.exercises.length !== 0) {
            let exercises = '<ul>';

            for (let exercise of workout.exercises) {
              exercises += '<li>' + exercise.name;

              const setsReps = [];

              for(let set of exercise.sets) {
                setsReps.push(set.reps)
              }

              let sets = ' (' + setsReps.join(", ") + ')';

              exercises += sets;

              if (exercise.equipment) {
                exercises += `<ul><li>${exercise.equipment}</li></ul>`
              }
              exercises += '</li>';
            }

            exercises += '</ul>';

            result += `<b>${that._translate.instant('WORKOUT.EXERCISES')}: </b>` + exercises
          } else {
            result += `<b>${that._translate.instant('WORKOUT.DISTANCE')}:</b> ${workout.distance}km <br>`;
            result += `<b>${that._translate.instant('WORKOUT.DURATION')}:</b> ${workout.duration}`;
          }

          return result;
        }
      }
    });
  }

  private setUpFirstPieChart(): void {
    const that = this;

    this.pieChart = Highcharts.chart('pie-chart', {
      chart: { type: 'pie' },
      title: {
        text: that._translate.instant('STATISTICS.DISTRIBUTION_BY_WORKOUT_TYPES')
      },
      series: [{
        type: 'pie',
        name: that._translate.instant('STATISTICS.QUANTITY'),
        data : this.workoutTypeData
      }],
    });
  }

  getFirstAreaChartData(type: string): any[] {
    const data = [];

    this.workouts.filter(workout => workout.type === type).forEach(workout => {
      let volumen = 0;

      workout.exercises.forEach(exercise => {
        exercise.sets.forEach(set => {
          volumen += set.reps * (set.weight === 0 ? 1 : set.weight);
        });
      });

      if (volumen === 0) {
        volumen = workout.distance;
      }

      data.push({x: Date.parse(workout.date.toString()), y: volumen, workout: workout})
    });

    if (this.chart) {
      this.chart.series[0].setData(data);
    }

    return data;
  }
}
