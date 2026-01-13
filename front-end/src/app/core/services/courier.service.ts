import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { map } from 'rxjs/operators';
import { CourierRequest, CourierResponse } from '../models';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class CourierService {

  // Use relative URLs so the Angular dev server proxy forwards requests to the backend
  private apiUrl = '/api/Delivery';

  constructor(private http: HttpClient) {}

 calculate(payload: CourierRequest): Observable<CourierResponse[]> {
    return this.http.post<any>(this.apiUrl, payload).pipe(
      map(res => {
        const items = Array.isArray(res)
          ? res
          : Array.isArray(res?.data)
          ? res.data
          : Array.isArray(res?.packages)
          ? res.packages
          : [];

        if (!Array.isArray(items) || items.length === 0) {
          return [];
        }

        return items.map((item: any): CourierResponse => ({
          packageId: item.id ?? item.pkg_id1,
          discount: item.discount ?? item.discount1 ?? 0,
          totalCost:
            item.totalCost ??
            item.total_cost1 ??
            item.deliveryCost ??
            0,
          deliveryTimeHours:
            item.etaHours ??
            item.estimated_delivery_time1_in_hours ??
            0
        }));
      })
    );
  }
} 
