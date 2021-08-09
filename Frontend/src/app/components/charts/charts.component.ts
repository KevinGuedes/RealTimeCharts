import { Component, OnInit } from '@angular/core';
import * as DShape from 'd3-shape';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-charts',
  templateUrl: './charts.component.html',
  styleUrls: ['./charts.component.scss']
})
export class ChartsComponent implements OnInit {
  data: any[] = [{
    name: "USA",
    series: [
      { "name": 0, "value": 2.5 },
    ]
  }];
  view: [number, number] = [700, 300];
  private currentX: number = 0;
  // options
  legend: boolean = true;
  showLabels: boolean = true;
  animations: boolean = true;
  xAxis: boolean = true;
  yAxis: boolean = true;
  showYAxisLabel: boolean = true;
  showXAxisLabel: boolean = true;
  xAxisLabel: string = 'Year';
  yAxisLabel: string = 'Population';
  curve: any = DShape.curveBasis;
  colorSchemeLine = {
    domain: ['#7aa3e5', '#a8385d', '#aae3f5']
  };
  colorSchemePolar = {
    domain: ['#aae3f5']
  };
  constructor(
    private readonly _signalrService: SignalrService,
  ) {
    this._signalrService.heartDataReceived.subscribe(data => {
      console.log(data, "recebido");
      this.teste()
    })
  }

  ngOnInit(): void {
  }


  onSelect(data: any): void {
    console.log('Item clicked', JSON.parse(JSON.stringify(data)));
  }

  onActivate(data: any): void {
    console.log('Activate', JSON.parse(JSON.stringify(data)));
  }

  onDeactivate(data: any): void {
    console.log('Deactivate', JSON.parse(JSON.stringify(data)));
  }

  teste(): void {
    const data = [
      { "name": 15, "value": 2.029 },
      { "name": 30, "value": 1.451 },
      { "name": 45, "value": 0.879 },
      { "name": 60, "value": 0.451 },
      { "name": 75, "value": 0.297 },
      { "name": 90, "value": 0.5 },
      { "name": 105, "value": 0.297 },
      { "name": 120, "value": 0.451 },
      { "name": 135, "value": 0.879 },
      { "name": 150, "value": 1.451 },
      { "name": 165, "value": 2.029 },
      { "name": 180, "value": 2.5 },
      { "name": 195, "value": 2.805 },
      { "name": 210, "value": 2.951 },
      { "name": 225, "value": 3 },
      { "name": 240, "value": 3.049 },
      { "name": 255, "value": 3.195 },
      { "name": 270, "value": 3.5 },
      { "name": 285, "value": 3.195 },
      { "name": 300, "value": 3.049 },
      { "name": 315, "value": 3 },
      { "name": 330, "value": 2.951 },
      { "name": 345, "value": 2.805 },
      { "name": 360, "value": 2.5 }
    ]
    if (this.currentX == data.length)
      return;

    this.data[0].series.push(data[this.currentX]),
      this.data = [...this.data];
    this.currentX++;
  }

  public teste2(): void {
    this._signalrService.sendNewMessage()
  }

}
