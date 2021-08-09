import { Component, OnInit } from '@angular/core';
import * as DShape from 'd3-shape';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-charts',
  templateUrl: './charts.component.html',
  styleUrls: ['./charts.component.scss']
})
export class ChartsComponent implements OnInit {
  data: any[] = [{ name: "Heart", series: [/*{ "name": 0, "value": 0 }*/] }];
  view: [number, number] = [700, 300];
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
  colorSchemeLine = { domain: ['#7aa3e5', '#a8385d', '#aae3f5'] };
  colorSchemePolar = { domain: ['#aae3f5'] };

  constructor(
    private readonly _signalrService: SignalrService,
  ) {
    this._signalrService.heartDataReceived.subscribe(data => {
      this.pushData(data);
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

  public fetchHeartData(): void {

  }

  public clearChart(): void {
    this.data[0].series = []
    this.data = [...this.data];
  }

  private pushData(data: any): void {
    this.data[0].series.push({ "name": data.Name, "value": data.Value })
    this.data = [...this.data];
  }
}
