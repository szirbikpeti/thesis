import { Component, OnInit } from '@angular/core';
import * as Highcharts from 'highcharts';

@Component({
  selector: 'app-statistics',
  templateUrl: './statistics.component.html',
  styleUrls: ['./statistics.component.scss']
})
export class StatisticsComponent implements OnInit {

  Highcharts: typeof Highcharts = Highcharts;
  chartOptions: Highcharts.Options = {
    series: [{
      showInNavigator: true,
      type: 'area',
      data : [{x: Date.UTC(2014,0,1), y: 1},
        {x: Date.UTC(2014,1,1), y: 2},
        {x: Date.UTC(2014,2,1), y: 3},
        {x: Date.UTC(2014,3,1), y: 4},
        {x: Date.UTC(2014,4,1), y: 5},
        {x: Date.UTC(2014,5,1), y: 6},
        {x: Date.UTC(2014,6,1), y: 7},
        {x: Date.UTC(2014,7,1), y: 8},
        {x: Date.UTC(2014,8,1), y: 9},
        {x: Date.UTC(2014,9,1), y: 10},
        {x: Date.UTC(2014,10,1), y: 11},
        {x: Date.UTC(2014,11,1), y: 12},
        {x: Date.UTC(2015,0,1), y: 1},
        {x: Date.UTC(2015,1,1), y: 2},
        {x: Date.UTC(2015,2,1), y: 3},
        {x: Date.UTC(2015,3,1), y: 4},
        {x: Date.UTC(2015,4,1), y: 5}]
    }],
    title: {
      text: 'Title here'
    },
    rangeSelector: {
      enabled:true,
      // selected: 4
    },
    navigator: {
      enabled: true
    },
    xAxis: {
      type: 'datetime',
      tickInterval: 1000 * 3600 * 24 *30 // 1 month
    },
  };

  constructor() { }

  ngOnInit(): void {
  }

}
