import { Component } from '@angular/core';
import * as Highcharts from 'highcharts';
import {WorkoutService} from "../../services/workout.service";
import {WorkoutModel} from "../../models/WorkoutModel";

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

  constructor(private _workout: WorkoutService) {
    _workout.list().subscribe(workouts => {
      this.workouts = workouts;

      this.getWorkoutTypeData();
      this.setUpFirstAreaChart();
      this.setUpFirstPieChart();

    })
  }

  private getWorkoutTypeData(): void {
    this.workouts.forEach(workout => {
      const types = this.workoutTypeData.map(({name}) => name);

      if (!types.includes(workout.type)) {
        this.workoutTypeData.push({ name: workout.type, y: 1});
      } else {
        const index = types.indexOf(workout.type);
        this.workoutTypeData[index].y += 1;
      }
    });

    this.workoutTypes = this.workoutTypeData.map(({name}) => name);
  }

  private setUpFirstAreaChart(): void {
    this.chart = Highcharts.chart('area-chart', {
      chart: { type: 'area' },
      title: {
        text: 'Egy edzésen elvégzett összes edzésmunka'
      },
      series: [{
        type: 'area',
        name: 'Workouts',
        data : this.getFirstAreaChartData()
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
    });
  }

  private setUpFirstPieChart(): void {
    this.pieChart = Highcharts.chart('pie-chart', {
      chart: { type: 'pie' },
      title: {
        text: 'Edzés típusok megoszlása'
      },
      series: [{
        type: 'pie',
        name: 'Darabszám',
        data : this.workoutTypeData
      }],
    });
  }

  getFirstAreaChartData(type: string = 'Gym'): any[] {
    const data = [];

    this.workouts.filter(workout => workout.type === type).forEach(workout => {
      let volume = 0;

      workout.exercises.forEach(exercise => {
        exercise.sets.forEach(set => {
          volume += set.reps * (set.weight === 0 ? 1 : set.weight);
        });
      });

      data.push({x: Date.parse(workout.date.toString()), y: volume})
    });

    if (this.chart) {
      this.chart.series[0].setData(data);
    }

    return data;
  }
}
