import { TestBed } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';

import { CourierService } from './courier.service';
import { CourierRequest } from '../models/request.model';
import { CourierResponse } from '../models/response.model';

describe('CourierService', () => {
  let service: CourierService;
  let httpMock: HttpTestingController;

  const sampleBackendPackages = [
    { id: 'PKG1', deliveryCost: 750, discount: 0, totalCost: 750, etaHours: 0.43 },
    { id: 'PKG2', deliveryCost: 1475, discount: 0, totalCost: 1475, etaHours: 1.79 }
  ];

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [ HttpClientTestingModule ],
      providers: [ CourierService ]
    });

    service = TestBed.inject(CourierService);
    httpMock = TestBed.inject(HttpTestingController);
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });

  it('should call POST calculate API using HttpClient', (done) => {
    const payload: CourierRequest = {
      baseDeliveryCost: 100,
      packages: [{ id: 'PKG1', weightKg: 10, distanceKm: 10, offerCode: 'NA' }],
      vehicles: { numberOfVehicles: 1, maxSpeedKmH: 60, maxCarryWeightKg: 200 }
    };

    const mockResponse = [{ packageId: 'PKG1', discount: 0, totalCost: 750, deliveryTimeHours: 0.43 }];

    service.calculate(payload).subscribe(res => {
      expect(res).toEqual(mockResponse);
      done();
    });

    const req = httpMock.expectOne('/api/Delivery');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toEqual(payload);
    req.flush(mockResponse);
  });

  it('should call GET results API and map packages[] shape', (done) => {
    const backend = { packages: sampleBackendPackages, totalCost: 2225, maxDeliveryTimeHours: 3.5 };

    service.getResults().subscribe((res: CourierResponse[]) => {
      expect(Array.isArray(res)).toBeTrue();
      expect(res.length).toBe(sampleBackendPackages.length);
      expect(res[0]).toEqual({
        packageId: 'PKG1',
        discount: 0,
        totalCost: 750,
        deliveryTimeHours: 0.43
      } as CourierResponse);
      done();
    });

    const req = httpMock.expectOne('/api/Delivery/sample');
    expect(req.request.method).toBe('GET');
    req.flush(backend);
  });

  it('should correctly map backend fields pkg_id1/discount1/total_cost1/estimated_delivery_time1_in_hours', (done) => {
    const backend = [
      {
        pkg_id1: 'PKG-A',
        discount1: 10,
        total_cost1: 900,
        estimated_delivery_time1_in_hours: 2.5
      }
    ];

    service.getResults().subscribe((res: CourierResponse[]) => {
      expect(res.length).toBe(1);
      expect(res[0]).toEqual({
        packageId: 'PKG-A',
        discount: 10,
        totalCost: 900,
        deliveryTimeHours: 2.5
      } as CourierResponse);
      done();
    });

    const req = httpMock.expectOne('/api/Delivery/sample');
    expect(req.request.method).toBe('GET');
    req.flush(backend);
  });
});
