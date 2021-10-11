import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { RetailerSalesData, Util } from 'src/app/shared';
import { ProducerGraphicService } from 'src/app/shared/services';

@Component({
  selector: 'app-top-retailers-sellers',
  templateUrl: './top-retailers-sellers.component.html',
  styleUrls: ['./top-retailers-sellers.component.css']
})
export class TopRetailersSellersComponent implements OnInit, OnChanges {

  @Input() userId: number;
  @Input() provinceId: number;
  @Input() date: any;

  public list: RetailerSalesData[];
  public loading: boolean = false;
  public tooltipDeviceStatus: boolean = false;
  public showSales: boolean = false;

  constructor(private _util: Util, private _producerGraphicService: ProducerGraphicService) { }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes.date && !changes.date.firstChange) || (changes.provinceId && !changes.provinceId.firstChange)) {
      this._getRetailers("sales");
    }
  }

  ngOnInit(): void {
    this._getRetailers("sales");
  }

  private _getRetailers(orderBy: string) {
    this.loading = true;
    var month = this.date.substr(0, this.date.indexOf('/'));
    var year = this.date.substr(this.date.indexOf('/') + 1);
    if (month < 10 && year == 2021) {
      this.loading = false;
      this.list = [];
    } else {
      this._producerGraphicService.getTop5Retailers(this.userId, this.provinceId, this.date, orderBy).subscribe((data: RetailerSalesData[]) => {
        this.list = data;
        this.loading = false;
      });
    }
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
