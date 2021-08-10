import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, throwError } from "rxjs";
import { catchError, map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { DataPoint } from 'src/app/models/data-point.model';

@Injectable({
  providedIn: 'root'
})
export class DataService {

  private _apiUrl: string = environment.dataApiUrl;

  constructor(
    private _http: HttpClient,
  ) { }

  public generateHeartData(): Promise<void> {
    return this._http.get<DataPoint[]>(`${this._apiUrl}/heart`).pipe(
      map(c => c),
      catchError(error => this.errorHandler(error))
    ).toPromise();
  }

  private errorHandler(error: any): Observable<any> {
    console.error(error.message);
    return throwError(error)
  }
}
