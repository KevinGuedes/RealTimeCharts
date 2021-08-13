import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import * as DShape from 'd3-shape';
import { DataGenerationRate } from 'src/app/models/data-generation-rate.enum';
import { DataType } from 'src/app/models/data-type.enum';
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
  public curve: DShape.CurveFactory = DShape.curveBasis;
  public colorSchemeLine = { domain: ['#7aa3e5'] };
  public colorSchemePolar = { domain: ['#aae3f5'] };
  public colorSchemeNumber = { domain: ['#192f36'] };
  public legendTitle: string = 'Data';
  public yLabelName: string = 'Value';
  public dataCounter: number = 0;

  public dataTypes = DataType;
  public dataTypeKeys!: any[];
  public generationRate = DataGenerationRate;
  public generationRateKeys!: any[];
  public dataForm: FormGroup;

  constructor(
    private readonly _signalrService: SignalrService,
    private readonly _dataService: DataService,
  ) {
    this.dataTypeKeys = Object.keys(this.dataTypes).filter(type => !isNaN(Number(type)));
    this.generationRateKeys = Object.keys(this.generationRate).filter(type => !isNaN(Number(type)));

    this._signalrService.heartDataReceived.subscribe(data => {
      this.dataCounter++;
      this.pushData(data);
    })

    this.dataForm = new FormGroup({
      type: new FormControl(null, [
        Validators.required,
      ]),
      rate: new FormControl(null, [
        Validators.required,
      ]),
    });
  }

  ngOnInit(): void {
  }

  public onSelect(data: any): void {
    console.log('Item clicked', data);
  }

  public onActivate(data: any): void {
    console.log('Activate', data);
  }

  public onDeactivate(data: any): void {
    console.log('Deactivate', data);
  }

  public async generateData(): Promise<void> {
    const rate = DataGenerationRate[this.dataForm.value.rate];
    const type = DataType[this.dataForm.value.type];

    console.log(rate)
    await this._dataService.generateHeartData(360, 10, DataGenerationRate.High)
      .then(() => {
        console.log("Heart data generation started")
      })
      .catch(error => console.error(error.message));
  }

  public clearData(): void {
    console.log(this.dataForm)
    this.data[0].series = []
    this.data = [...this.data];
    this.dataCounter = 0;
  }

  private pushData(data: any): void {
    this.data[0].series.push({ "name": data.Name, "value": data.Value })
    this.data = [...this.data];
  }
}
