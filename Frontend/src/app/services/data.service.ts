import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from "rxjs";
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { GenerateDataRequest } from './requests/generate-data.request';
import { DataGenerationRate } from '../models/data-generation-rate.enum';
import { SignalrService } from './signalr.service';
import { DataType } from '../models/data-type.enum';
import { DataTypeName } from '../models/data-type-name.enum';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private _apiUrl: string = environment.dataApiUrl;

  constructor(
    private readonly _http: HttpClient,
    private readonly _signalrService: SignalrService,
  ) { }

  public generateData(rate: DataGenerationRate, dataTypeName: string): Promise<void> {
    const dataType = this.getDataTypeByDataTypeName(DataTypeName[dataTypeName as keyof typeof DataTypeName]);
    const request = new GenerateDataRequest(rate, dataType, this._signalrService.GetConnectionId())

    return this._http.post<GenerateDataRequest>(`${this._apiUrl}/generate`, request).pipe(
      map(response => response),
      catchError(error => this.errorHandler(error))
    ).toPromise();
  }

  private errorHandler(error: any): Observable<any> {
    console.error(error.message);
    return throwError(error)
  }

  public getDataTypeByDataTypeName(name: DataTypeName): DataType {
    const dataTypeNameKey = this.getEnumKeyByEnumValue(DataTypeName, name) as keyof typeof DataType;
    return DataType[dataTypeNameKey];
  }

  private getEnumKeyByEnumValue<T extends { [index: string]: string }>(enumType: T, enumValue: string): keyof T | null {
    let keys = Object.keys(enumType).filter(key => enumType[key] == enumValue);
    return keys.length > 0 ? keys[0] : null;
  }
}
