import { Injectable } from '@angular/core';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError, timeout } from 'rxjs';
import { catchError, map } from 'rxjs/operators';
import {
  PersonAstronaut,
  AstronautDuty,
  CreatePersonRequest,
  UpdatePersonRequest,
  CreateAstronautDutyRequest,
  ApiResponse,
} from '../models/person.model';

@Injectable({
  providedIn: 'root',
})
export class ApiService {
  private readonly baseUrl = 'https://localhost:7204';

  constructor(private http: HttpClient) {}

  // Person endpoints
  getPeople(): Observable<PersonAstronaut[]> {
    console.log('Making API request to:', `${this.baseUrl}/Person`);
    return this.http.get<any>(`${this.baseUrl}/Person`).pipe(
      timeout(10000), // 10 second timeout
      map((response) => {
        console.log('Raw API response:', response);
        if (response.people && Array.isArray(response.people)) {
          console.log('Extracted people array:', response.people);
          return response.people;
        }
        console.log('No people array found in response, returning empty array');
        return [];
      }),
      catchError((error) => {
        console.error('API request failed:', error);
        return this.handleError(error);
      })
    );
  }

  getPersonByName(name: string): Observable<PersonAstronaut | null> {
    return this.http.get<any>(`${this.baseUrl}/Person/${encodeURIComponent(name)}`).pipe(
      map((response) => {
        if (response.data) {
          return response.data;
        }
        return null;
      }),
      catchError(this.handleError)
    );
  }

  createPerson(request: CreatePersonRequest): Observable<ApiResponse<{ id: number }>> {
    return this.http
      .post<ApiResponse<{ id: number }>>(`${this.baseUrl}/Person`, `"${request.name}"`, {
        headers: { 'Content-Type': 'application/json' },
      })
      .pipe(catchError(this.handleError));
  }

  updatePerson(
    name: string,
    request: UpdatePersonRequest
  ): Observable<ApiResponse<{ id: number }>> {
    return this.http
      .put<ApiResponse<{ id: number }>>(
        `${this.baseUrl}/Person/${encodeURIComponent(name)}`,
        request
      )
      .pipe(catchError(this.handleError));
  }

  // Astronaut Duty endpoints
  getAstronautDutiesByName(
    name: string
  ): Observable<{ person: PersonAstronaut; duties: AstronautDuty[] } | null> {
    console.log(
      'Making API request to:',
      `${this.baseUrl}/AstronautDuty/${encodeURIComponent(name)}`
    );
    return this.http.get<any>(`${this.baseUrl}/AstronautDuty/${encodeURIComponent(name)}`).pipe(
      timeout(10000),
      map((response) => {
        console.log('Raw API response for astronaut duties:', response);
        if (response.person && response.astronautDuties) {
          const data = {
            person: response.person,
            duties: response.astronautDuties,
          };
          console.log('Extracted astronaut duties data:', data);
          return data;
        }
        console.log('No astronaut duties data found in response, returning null');
        return null;
      }),
      catchError((error) => {
        console.error('API request for astronaut duties failed:', error);
        return this.handleError(error);
      })
    );
  }

  createAstronautDuty(
    request: CreateAstronautDutyRequest
  ): Observable<ApiResponse<{ id: number }>> {
    return this.http
      .post<ApiResponse<{ id: number }>>(`${this.baseUrl}/AstronautDuty`, request)
      .pipe(catchError(this.handleError));
  }

  private handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred';

    if (error.error instanceof ErrorEvent) {
      // Client-side error
      errorMessage = `Error: ${error.error.message}`;
    } else {
      // Server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
    }

    console.error('API Error:', errorMessage);
    return throwError(() => new Error(errorMessage));
  }
}
