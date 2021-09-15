import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import * as DShape from 'd3-shape';
import { DataGenerationRate } from 'src/app/models/data-generation-rate.enum';
import { DataPoint } from 'src/app/models/data-point.model';
import { DataTypeName } from 'src/app/models/data-type-name.enum';
import { DataType } from 'src/app/models/data-type.enum';
import { DataService } from 'src/app/services/data.service';
import { SignalrService } from 'src/app/services/signalr.service';

@Component({
  selector: 'app-charts',
  templateUrl: './charts.component.html',
  styleUrls: ['./charts.component.scss']
})
export class ChartsComponent implements OnInit {

  //#region Chart Setup
  public data: any[] = [];
  public view: [number, number] = [900, 350];
  public curve: DShape.CurveFactory = DShape.curveBasis;
  public colorSchemeLine = { domain: ['#7aa3e5'] };
  public colorSchemePolar = { domain: ['#eb4646'] };
  public legendTitle: string = 'Legend';
  public yLabelName: string = 'Value';
  //#endregion

  public generationRate = DataGenerationRate;
  public generationRateKeys!: any[];
  public dataTypeName = DataTypeName;
  public dataTypes = DataType;
  public dataTypeNameEntries!: any[];
  public dataCounter: number = 0;
  public dataForm: FormGroup;

  public get formControl() {
    return this.dataForm.controls;
  }

  constructor(
    private readonly _signalrService: SignalrService,
    private readonly _dataService: DataService,
  ) {
    this.dataTypeNameEntries = Object.entries(DataTypeName);
    this.generationRateKeys = Object.keys(this.generationRate).filter(key => !isNaN(Number(key))).map(Number);

    this._signalrService.dataReceived.subscribe((dataPoint: DataPoint) => {
      this.dataCounter++;
      this.pushData(dataPoint);
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
    const selectedDataTypeName = DataTypeName[this.dataForm.value.type as keyof typeof DataTypeName]
    this.data.push({ name: selectedDataTypeName, series: [] })

    await this._dataService.generateData(this.dataForm.value.rate, this.dataForm.value.type)
      .then(() => {
        console.log(`${selectedDataTypeName} Data generation started`)
      })
      .catch(error => console.error(error.message));
  }

  public clearData(): void {
    this.data = []
    this.data = [...this.data];
    this.dataCounter = 0;
  }

  private pushData(dataPoint: DataPoint): void {
    this.data[0].series.push({ "name": dataPoint.Name, "value": dataPoint.Value })
    this.data = [...this.data];
  }
}
