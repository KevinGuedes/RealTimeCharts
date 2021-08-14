import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, throwError } from "rxjs";
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { GenerateHeartDataRequest } from './requests/generate-heart-data.request';
import { DataGenerationRate } from '../models/data-generation-rate.enum';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private _apiUrl: string = environment.dataApiUrl;

  constructor(
    private _http: HttpClient,
  ) { }

  public generateHeartData(max: number, step: number, rate: DataGenerationRate): Promise<void> {
    const request = new GenerateHeartDataRequest(max, step, rate)

    return this._http.post<GenerateHeartDataRequest>(`${this._apiUrl}/heart`, request).pipe(
      map(response => response),
      catchError(error => this.errorHandler(error))
    ).toPromise();
  }

  private errorHandler(error: any): Observable<any> {
    console.error(error.message);
    return throwError(error)
  }
}
