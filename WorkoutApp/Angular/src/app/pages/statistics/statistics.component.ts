import { Component } from '@angular/core';
import * as Highcharts from 'highcharts';
import {WorkoutService} from "../../services/workout.service";
import {WorkoutModel} from "../../models/WorkoutModel";
import {DateFormatterPipe} from "../../pipes/date-formatter.pipe";
import {StateService} from "../../services/state.service";
import {TranslateService} from "@ngx-translate/core";
import {ExerciseModel} from "../../models/ExerciseModel";

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss']
})
export class StatisticsComponent {

  workoutTypeAreaChart: Highcharts.Chart;
  exerciseAreaChart: Highcharts.Chart;
  pieChart: Highcharts.Chart;

  workouts: WorkoutModel[];

  workoutTypeData = [];
  workoutTypes = [];
  exerciseNames = [];

  selectedWorkoutType: string;
  selectedExercise: string;

  constructor(private _workout: WorkoutService, private _state: StateService,
              private _translate: TranslateService, private _dateFormatter: DateFormatterPipe) {
    _workout.list().subscribe(workouts => {
      this.workouts = workouts;

      setTimeout(() => this.getAllData(true), 100);
    });

    _state.language.subscribe(() => {
      if (this.workoutTypeAreaChart && this.exerciseAreaChart) {
        this.getAllData();
      }
    });
  }

  private getAllData(isFirstCall = false): void {
    const workoutType = this.getWorkoutTypeData();
    const exerciseName = this.getExerciseNames();

    if (isFirstCall) {
      this.selectedWorkoutType = workoutType;
      this.selectedExercise = exerciseName;
    }

    this.setUpSumVolumenInOneWorkoutAreaChart();
    this.setUpSumVolumenInOneExerciseAreaChart();
    this.setUpDistributionByWorkoutTypesPieChart();
  }

  private getWorkoutTypeData(): string {
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

    return this.workoutTypes[0];
  }

  private getExerciseNames(): string {
    this.workouts.forEach(workout => {
      workout.exercises.forEach(exercise => {
        const exerciseName = exercise.name.toLowerCase();
        if (!this.exerciseNames.includes(exerciseName)) {
          this.exerciseNames.push(exerciseName);
        }
      });
    });

    return this.exerciseNames[0];
  }

  private setUpSumVolumenInOneWorkoutAreaChart(): void {
    const that = this;
    const data = this.getSumVolumenInOneWorkoutAreaChartData(this.selectedWorkoutType);

    this.workoutTypeAreaChart = Highcharts.chart('workout-type-area-chart', {
      title: {
        text: that._translate.instant('STATISTICS.SUM_VOLUMEN_IN_ONE_WORKOUT')
      },
      series: [{
        type: 'area',
        name: that._translate.instant('WORKOUT.WORKOUTS'),
        data : data
      }],
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
            result += `<b>Volumen:</b> ${this.y}kg <br>`;

            let exercises = '<ul>';

            for (let exercise of workout.exercises) {
              exercises += '<li>' + exercise.name;

              const setsReps = [];

              for(let set of exercise.sets) {
                if (set.weight === 0 && set.duration) {
                  setsReps.push(set.duration);
                  continue;
                }

                setsReps.push(set.reps);
              }

              let sets = ' (' + setsReps.join(", ") + ')';

              exercises += sets;

              if (exercise.equipment) {
                exercises += `<ul><li>${that._translate.instant('WORKOUT.EXERCISE_EQUIPMENT')}: ${exercise.equipment}</li></ul>`
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

  private setUpSumVolumenInOneExerciseAreaChart(): void {
    const that = this;
    const data = this.getSumVolumenInOneExerciseAreaChartData(this.selectedExercise);

    this.exerciseAreaChart = Highcharts.chart('exercise-area-chart', {
      title: {
        text: that._translate.instant('STATISTICS.SUM_VOLUMEN_IN_ONE_EXERCISE')
      },
      series: [{
        type: 'area',
        name: that._translate.instant('WORKOUT.EXERCISE'),
        data : data
      }],
      xAxis: {
        type: 'datetime',
      },
      tooltip: {
        useHTML: true,
        formatter: function() {
          const exercise: ExerciseModel = data.find(that => that.x === this.x && that.y === this.y).exercise;
          let result = `<b>${that._translate.instant('WORKOUT.DATE')}:</b> ${that._dateFormatter.transform(new Date(this.x), that._state.language.value, false)} <br>`;

          result += `<b>Volumen:</b> ${this.y}kg <br>`;

          if (exercise.equipment) {
            result += `<b>${that._translate.instant('WORKOUT.EXERCISE_EQUIPMENT')}:</b> ${exercise.equipment}<br>`
          }

          let sets = '<ul>';

          for (let set of exercise.sets) {
            let weight = '';
            let duration = '';

            if (set.weight !== 0) {
              weight = set.weight + 'kg -> ';
            }

            if (set.duration) {
              duration = ` -> ${set.duration}`;
            }

            sets += `<li>${weight}${set.reps}db${duration}</li>`
          }

          sets += '</ul>';

          result += `<b>${that._translate.instant('WORKOUT.SETS')}: </b>` + sets

          return result;
        }
      }
    });
  }

  private setUpDistributionByWorkoutTypesPieChart(): void {
    const that = this;

    this.pieChart = Highcharts.chart('distribution-pie-chart', {
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

  getSumVolumenInOneWorkoutAreaChartData(type: string): any[] {
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

    if (this.workoutTypeAreaChart) {
      this.workoutTypeAreaChart.series[0].setData(data);
    }

    return data;
  }

  getSumVolumenInOneExerciseAreaChartData(exerciseName: string): any[] {
    const data = [];

    this.workouts.forEach(workout => {

      workout.exercises.forEach(exercise => {
        let volumen = 0;
        if (exercise.name.toLowerCase() !== exerciseName.toLowerCase()) {
          return;
        }

        exercise.sets.forEach(set => {
          volumen += set.reps * (set.weight === 0 ? 1 : set.weight);
        });

        if (volumen !== 0) {
          data.push({x: Date.parse(workout.date.toString()), y: volumen, exercise: exercise})
        }
      });
    });

    if (this.exerciseAreaChart) {
      this.exerciseAreaChart.series[0].setData(data);
    }

    return data;
  }
}
