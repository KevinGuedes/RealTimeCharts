import { Component, OnInit } from '@angular/core';
import * as DShape from 'd3-shape';
import { DataService } from 'src/app/services/data.service';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-charts',
  templateUrl: './charts.component.html',
  styleUrls: ['./charts.component.scss']
})
export class ChartsComponent implements OnInit {
  public data: any[] = [{ name: "Heart", series: [] }];
  public view: [number, number] = [700, 300];
  public legend: boolean = true;
  public showLabels: boolean = true;
  public animations: boolean = true;
  public xAxis: boolean = true;
  public yAxis: boolean = true;
  public showYAxisLabel: boolean = true;
  public showXAxisLabel: boolean = true;
  public xAxisLabel: string = 'Year';
  public yAxisLabel: string = 'Population';
  public curve: DShape.CurveFactory = DShape.curveBasis;
  public colorSchemeLine = { domain: ['#7aa3e5'] };
  public colorSchemePolar = { domain: ['#aae3f5'] };
  public showFetchMessage: boolean = false;

  constructor(
    private readonly _signalrService: SignalrService,
    private readonly _dataService: DataService,
  ) {
    this._signalrService.heartDataReceived.subscribe(data => {
      this.pushData(data);
    })
  }

  ngOnInit(): void {
  }

  public onSelect(data: any): void {
    console.log('Item clicked', JSON.parse(JSON.stringify(data)));
  }

  public onActivate(data: any): void {
    console.log('Activate', JSON.parse(JSON.stringify(data)));
  }

  public onDeactivate(data: any): void {
    console.log('Deactivate', JSON.parse(JSON.stringify(data)));
  }

  public async generateHeartData(): Promise<void> {
    this.showFetchMessage = true;
    await this._dataService.generateHeartData(360, 10)
      .then(() => {
        console.log("Generating heart data")
      })
      .catch(error => console.error(error.message));
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
