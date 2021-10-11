import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { RetailerSalesData, Util } from 'src/app/shared';
import { ProducerGraphicService } from 'src/app/shared/services';

@Component({
  selector: 'app-product-top-sellers',
  templateUrl: './product-top-sellers.component.html',
  styleUrls: ['./product-top-sellers.component.css']
})
export class ProductTopSellersComponent implements OnInit, OnChanges {

  @Input() selectedWeedId: number;
  @Input() selectedProvinceId: number;
  @Input() selectedPeriodId: string;

  public list: RetailerSalesData[];
  public loading: boolean = false;
  public tooltipDeviceStatus: boolean = false;
  public showSales: boolean = false;

  constructor(private _util: Util, private _producerGraphicService: ProducerGraphicService) { }

  ngOnInit(): void {
    this._getRetailers("sales");
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes.selectedPeriodId && !changes.selectedPeriodId.firstChange) ||
      (changes.selectedProvinceId && !changes.selectedProvinceId.firstChange)) {
      this._getRetailers("sales");
    }
  }

  private _getRetailers(orderBy: string) {
    this.loading = true;
    this._producerGraphicService.getTop5RetailersForProduct(this.selectedWeedId, this.selectedProvinceId, this.selectedPeriodId, orderBy).subscribe((data: RetailerSalesData[]) => {
      this.list = data;
      this.loading = false;
    });
  }

  public clickTooltipMobileShow() {
    this.tooltipDeviceStatus = !this.tooltipDeviceStatus;
  }

  public getUrl(url: string) {
    return this._util.baseUrl + "/images/companyLogo/" + encodeURIComponent(url);
  }

  public handleChangeReports(item: any) {
    if (this.showSales) {
      this._getRetailers("unit");
    } else {
      this._getRetailers("sales");
    }
  }
}
