import { PackageModel } from './package.model';
import { VehicleModel } from './vehicle.model';

export interface CourierRequest {
  baseDeliveryCost: number;
  packages: PackageModel[];
  vehicles: VehicleModel;
}
