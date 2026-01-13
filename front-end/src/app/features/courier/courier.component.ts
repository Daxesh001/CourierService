import { Component, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatTableModule } from '@angular/material/table';
import { CourierService } from '../../core/services/courier.service';
import { CourierResponse } from '../../core/models/response.model';
import { CourierRequest } from '../../core/models/request.model';

@Component({
  selector: 'app-courier',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule,
    MatToolbarModule,
    MatCardModule,
    MatFormFieldModule,
    MatInputModule,
    MatButtonModule,
    MatIconModule,
    MatTableModule
  ],
  templateUrl: './courier.component.html',
  styleUrls: ['./courier.component.scss']
})
export class CourierComponent {

  results: CourierResponse[] = [];
  displayedColumns = ['packageId', 'discount', 'totalCost', 'deliveryTimeHours'];

  form!: FormGroup;

  constructor(private fb: FormBuilder,
              private courierService: CourierService) {
    this.form = this.fb.group({
      baseDeliveryCost: [100, Validators.required],
      totalPackage: [5, Validators.required],
      packages: this.fb.array([]),
      vehicles: this.fb.group({
        numberOfVehicles: [2],
        maxSpeedKmH: [70],
        maxCarryWeightKg: [200]
      })
    });

    this.addPackage();
  }

  get packages(): FormArray {
    return this.form.get('packages') as FormArray;
  }

  addPackage() {
    const max = this.form.get('totalPackage')?.value ?? Infinity;
    if (this.packages.length >= max) {
      return;
    }

    this.packages.push(
      this.fb.group({
        id: ['', Validators.required],
        weightKg: [0, Validators.required],
        distanceKm: [0, Validators.required],
        offerCode: ['NA']
      })
    );
  }

  removePackage(i: number) {
    this.packages.removeAt(i);
  }

  submit() {
    if (this.form.invalid) {
      // Mark all controls as touched so inline validation messages are shown
      this.form.markAllAsTouched();
      return;
    }

    const vehiclesGroup = this.form.get('vehicles') as FormGroup;
    const payload: CourierRequest = {
      baseDeliveryCost: Number(this.form.get('baseDeliveryCost')?.value ?? 0),
      packages: this.packages.controls.map(g => ({
        id: g.get('id')?.value ?? '',
        weightKg: Number(g.get('weightKg')?.value ?? 0),
        distanceKm: Number(g.get('distanceKm')?.value ?? 0),
        offerCode: g.get('offerCode')?.value ?? 'NA'
      })),
      vehicles: {
        numberOfVehicles: Number(vehiclesGroup.get('numberOfVehicles')?.value ?? 1),
        maxSpeedKmH: Number(vehiclesGroup.get('maxSpeedKmH')?.value ?? 0),
        maxCarryWeightKg: Number(vehiclesGroup.get('maxCarryWeightKg')?.value ?? 0)
      }
    };


    this.courierService.calculate(payload).subscribe({
      next: (data) => {
        this.results = data;
      },
      error: err => console.error('Calculation API error', err)
    });
  }
}

