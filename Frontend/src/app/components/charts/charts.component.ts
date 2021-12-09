import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { DataGenerationRate } from 'src/app/models/data-generation-rate.enum';
import { DataPoint } from 'src/app/models/data-point.model';
import { DataTypeName } from 'src/app/models/data-type-name.enum';
import { DataService } from 'src/app/services/data.service';
import { SignalrService } from 'src/app/services/signalr.service';
import * as DShape from 'd3-shape';

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
  public colorSchemeLine = { domain: new Array<string>() };
  public colorSchemePolar = { domain: new Array<string>() };
  public legendTitle: string = 'Legend';
  public yLabelName: string = 'Value';
  //#endregion

  public generationRate = DataGenerationRate;
  public generationRateKeys!: any[];
  public dataTypeNameKeys!: any[];
  public dataCounter: number = 0;
  public dataForm!: FormGroup;
  public isReceivingData: boolean = false;
  public showFailMessage: boolean = false;
  public connectedWithSignalR!: boolean;
  public dataTypeNameInProcess!: string;

  public get formControl() {
    return this.dataForm.controls;
  }

  constructor(
    private readonly _signalrService: SignalrService,
    private readonly _dataService: DataService,
  ) { }

  ngOnInit(): void {
    this.dataTypeNameKeys = Object.keys(DataTypeName);
    this.generationRateKeys = Object.keys(this.generationRate).filter(key => !isNaN(Number(key))).map(Number);
    this.subscribeToSignalREvents();

    this.dataForm = new FormGroup({
      type: new FormControl(null, [
        Validators.required,
      ]),
      rate: new FormControl(null, [
        Validators.required,
      ]),
    });
  }

  private subscribeToSignalREvents(): void {
    this._signalrService.connectionStatus.subscribe((status: boolean) => {
      if (status) {
        this.connectedWithSignalR = true;
        return;
      }

      this.connectedWithSignalR = false;
      this.isReceivingData = false
      this.showFailMessage = true;
    })

    this._signalrService.dataReceived.subscribe((dataPoint: DataPoint) => {
      this.dataCounter++;
      this.pushData(dataPoint);
    })

    this._signalrService.dataGenerationFinished.subscribe((success: boolean) => {
      this.isReceivingData = false;

      if (success) {
        console.log(`${this.dataTypeNameInProcess} Data generation finished`)
        return;
      }

      this.showFailMessage = true;
      console.warn(`Failed to generate ${this.dataTypeNameInProcess} data`)
      this.clearData();
    })
  }

  private pushData(dataPoint: DataPoint): void {
    let currentSerieIndex = this.data.length - 1;
    this.data[currentSerieIndex].series.push({ "name": dataPoint.name, "value": dataPoint.value })
    this.data = [...this.data];
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

  public dataTypeNameByKey(key: string): string {
    return DataTypeName[key as keyof typeof DataTypeName]
  }

  public async generateData(): Promise<void> {
    this.isReceivingData = true;
    this.dataTypeNameInProcess = this.dataTypeNameByKey(this.dataForm.value.type);
    this.data.push({ name: this.dataTypeNameInProcess, series: [] })

    if (this.dataForm.value.type == 'Heart')
      this.appendColorsToColorSchemes('#eb4646');
    else
      this.appendColorsToColorSchemes();

    await this._dataService.generateData(this.dataForm.value.rate, this.dataForm.value.type)
      .then(_ => console.log(`${this.dataTypeNameInProcess} Data generation started`))
      .catch(error => console.error(error.message));
  }

  public clearData(): void {
    this.colorSchemeLine.domain = new Array<string>();
    this.colorSchemePolar.domain = new Array<string>();
    this.data = []
    this.dataCounter = 0;
  }

  private appendColorsToColorSchemes(color?: string): void {
    const randomColor = color ? color : `rgb(${Math.floor(Math.random() * 256)}, ${Math.floor(Math.random() * 256)}, ${Math.floor(Math.random() * 256)})`;
    this.colorSchemeLine.domain.push(randomColor);
    this.colorSchemePolar.domain.push(randomColor);
  }
}
