import { TestBed, ComponentFixture } from '@angular/core/testing';
import { HttpClientTestingModule, HttpTestingController } from '@angular/common/http/testing';
import { NoopAnimationsModule } from '@angular/platform-browser/animations';

import { CourierComponent } from './courier.component';
import { CourierService } from '../../core/services/courier.service';

describe('CourierComponent (standalone)', () => {
  let fixture: ComponentFixture<CourierComponent>;
  let component: CourierComponent;
  let httpMock: HttpTestingController;
  let service: CourierService;

  const samplePackages = [
    { id: 'PKG1', deliveryCost: 750, discount: 0, totalCost: 750, etaHours: 0.43 },
    { id: 'PKG2', deliveryCost: 1475, discount: 0, totalCost: 1475, etaHours: 1.79 },
    { id: 'PKG3', deliveryCost: 2350, discount: 0, totalCost: 2350, etaHours: 1.43 },
  ];

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [
        CourierComponent,     // standalone component import
        HttpClientTestingModule,
        NoopAnimationsModule,
      ],
      providers: []
    }).compileComponents();

    fixture = TestBed.createComponent(CourierComponent);
    component = fixture.componentInstance;
    httpMock = TestBed.inject(HttpTestingController);
    service = TestBed.inject(CourierService);

    // Trigger initial change detection which will run ngOnInit
    fixture.detectChanges();

    // Flush the GET results request created in ngOnInit()
    const getReq = httpMock.expectOne('/api/Delivery/sample');
    expect(getReq.request.method).toBe('GET');
    getReq.flush({ packages: samplePackages, maxDeliveryTimeHours: 3.57, totalCost: 4575 });

    fixture.detectChanges();
  });

  afterEach(() => {
    httpMock.verify();
  });

  it('should create the component', () => {
    expect(component).toBeTruthy();
  });

  it('should initialize reactive form with default values', () => {
    expect(component.form).toBeDefined();
    expect(component.form.get('baseDeliveryCost')?.value).toBe(100);
    expect(component.form.get('totalPackage')?.value).toBe(5);
    const vehicles = component.form.get('vehicles');
    expect(vehicles).toBeTruthy();
    expect(vehicles?.get('numberOfVehicles')?.value).toBe(2);
    expect(vehicles?.get('maxSpeedKmH')?.value).toBe(70);
    expect(vehicles?.get('maxCarryWeightKg')?.value).toBe(200);
  });

  it('should add a package to FormArray', () => {
    const initial = component.packages.length;
    component.addPackage();
    expect(component.packages.length).toBe(initial + 1);
  });

  it('should remove a package from FormArray', () => {
    // ensure at least two packages
    component.packages.clear();
    component.addPackage();
    component.addPackage();
    expect(component.packages.length).toBeGreaterThanOrEqual(2);
    component.removePackage(0);
    expect(component.packages.length).toBe(1);
  });

  it('should call submit() without error and POST payload', () => {
    spyOn(console, 'error');

    // Make form valid by setting required package id
    component.form.get('baseDeliveryCost')?.setValue(123);
    if (component.packages.length === 0) component.addPackage();
    component.packages.at(0).get('id')?.setValue('PKG1');

    component.submit();

    const req = httpMock.expectOne('/api/Delivery');
    expect(req.request.method).toBe('POST');
    expect(req.request.body).toBeTruthy();
    req.flush([]);
    expect(console.error).not.toHaveBeenCalled();
  });

  it('should render table rows when results data is present', (done) => {
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      fixture.detectChanges();
      const rows = fixture.nativeElement.querySelectorAll('table tbody tr, tr.mat-row, table tr');
      // Filter out header row if present
      const dataRows = Array.from(rows).filter((r: any) => (r.querySelectorAll ? r.querySelectorAll('td').length > 0 : true));
      expect(dataRows.length).toBe(samplePackages.length);
      const firstRowText = (dataRows[0] as HTMLElement)?.textContent || '';
      expect(firstRowText).toContain('PKG1');
      done();
    });
  });
});
