import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { Router } from '@angular/router';
import { Util, WeedFilterModel } from 'src/app/shared';
import { ProductService } from 'src/app/shared/services';

@Component({
  selector: 'app-producer-gr-product',
  templateUrl: './producer-gr-product.component.html',
  styleUrls: ['./producer-gr-product.component.css']
})
export class ProducerGrProductComponent implements OnInit, OnChanges {

  @Input() userId: number;
  @Input() selectedWeedId: number;
  @Input() provinceId: number;

  public loading = false;
  public nrProducts = 0;
  public productList: WeedFilterModel[] = [];

  constructor(private _util: Util, private _productService: ProductService, private _router: Router) { }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes.selectedWeedId && !changes.selectedWeedId.firstChange) ||
      (changes.provinceId && !changes.provinceId.firstChange)) {
      this._loadData();
    }
  }

  ngOnInit(): void {
    this._loadData();
  }

  private _loadData() {
    if (!this.selectedWeedId || this.selectedWeedId == 0) {
      this.loading = true;
      this._productService.getAllWeedsByProducerId(this.userId, this.provinceId).subscribe((data: WeedFilterModel[]) => {
        this.loading = false;
        data.forEach(x => x.UniqueId = x.IsBasicProduct + '-' + x.WeedId);
        this.nrProducts = data.length;
        this.productList = data;
      });
    }
  }

  public getTotalTerpenes(item: WeedFilterModel) {
    return item.Terpenes ? item.Terpenes + (item.Unit ?? '%') : "-";
  }

  public getThc(item: WeedFilterModel) {
    let totalThc = "";
    if (item.IsBasicProduct) {
      if (item.SummaryThc != null && item.SummaryStartThc != null) {
        if (item.SummaryThc != item.SummaryStartThc) {
          totalThc = item.SummaryStartThc + (item.Unit ?? '%') +
            ' to ' + item.SummaryThc + (item.Unit ?? '%');
        } else {
          totalThc = item.SummaryStartThc + (item.Unit ?? '%');
        }
      } else {
        if (item.SummaryThc == null && item.SummaryStartThc == null) {
          totalThc = '-';
        } else {
          if (item.SummaryThc != null) {
            totalThc = item.SummaryThc + (item.Unit ?? '%');
          } else {
            totalThc = item.SummaryStartThc + (item.Unit ?? '%');
          }
        }
      }
    } else {
      return item.SummaryThc ? item.SummaryThc + (item.Unit ?? '%') : "-";
    }
    return totalThc;
  }

  public getCbd(item: WeedFilterModel) {
    let totalCbd = "";
    if (item.IsBasicProduct) {
      if (item.SummaryCbd != null && item.SummaryStartCbd != null) {
        if (item.SummaryCbd != item.SummaryStartCbd) {
          totalCbd = item.SummaryStartCbd + (item.Unit ?? '%') +
            ' to ' + item.SummaryCbd + (item.Unit ?? '%');
        } else {
          totalCbd = item.SummaryStartCbd + (item.Unit ?? '%');
        }
      } else {
        if (item.SummaryCbd == null && item.SummaryStartCbd == null) {
          totalCbd = '-';
        } else {
          if (item.SummaryCbd != null) {
            totalCbd = item.SummaryCbd + (item.Unit ?? '%');
          } else {
            totalCbd = item.SummaryStartCbd + (item.Unit ?? '%');
          }
        }
      }
    } else {
      return item.SummaryCbd ? item.SummaryCbd + (item.Unit ?? '%') : "-";
    }
    return totalCbd;
  }

  public getUrl(url: string) {
    return this._util.galleryUrl + encodeURIComponent(url);
  }

  public generateUrl(product: WeedFilterModel) {
    let url = this._router.url;
    if (url.indexOf('#') > 0) {
      url = this._router.url.substr(0, this._router.url.indexOf('#'));
    }
    return url + '#product' + "-" + product.WeedId;
  }

  public goToUrl(item: WeedFilterModel) {
    this.selectedWeedId = item.WeedId;
    let url = this._router.url;
    if (url.indexOf('#') > 0) {
      url = this._router.url.substr(0, this._router.url.indexOf('#'));
    }
    this._router.routeReuseStrategy.shouldReuseRoute = () => false;
    this._router.onSameUrlNavigation = 'reload';
    this._router.navigateByUrl(url + '#product' + "-" + this.selectedWeedId);
  }
}
