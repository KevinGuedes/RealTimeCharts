import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from "rxjs";
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { GenerateDataRequest } from './requests/generate-data.request';
import { DataGenerationRate } from '../models/data-generation-rate.enum';
import { SignalrService } from './signalr.service';
import { DataType } from '../models/data-type.enum';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private _apiUrl: string = environment.dataApiUrl;

  constructor(
    private readonly _http: HttpClient,
    private readonly _signalrService: SignalrService,
  ) { }

  public generateData(max: number, step: number, rate: DataGenerationRate, dataType: DataType): Promise<void> {
    const request = new GenerateDataRequest(max, step, rate, dataType, this._signalrService.GetConnectionId())

    return this._http.post<GenerateDataRequest>(`${this._apiUrl}/generate`, request).pipe(
      map(response => response),
      catchError(error => this.errorHandler(error))
    ).toPromise();
  }

  private errorHandler(error: any): Observable<any> {
    console.error(error.message);
    return throwError(error)
  }
}
