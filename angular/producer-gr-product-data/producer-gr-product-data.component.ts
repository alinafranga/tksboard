import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { GraphicModel, UrlUtil, UserType, Util, WeedModel, WeedStatus } from 'src/app/shared';
import { ProducerGraphicService, ProductService } from 'src/app/shared/services';

@Component({
  selector: 'app-producer-gr-product-data',
  templateUrl: './producer-gr-product-data.component.html',
  styleUrls: ['./producer-gr-product-data.component.css']
})
export class ProducerGrProductDataComponent implements OnInit, OnChanges {

  @Input() selectedWeedId: number;
  @Input() selectedProvinceId: number;
  @Input() userId: number;

  public loading: boolean = false;
  public weed: any;
  public totalThc = "";
  public totalCbd = "";
  public date: any;
  public imageUrl: string;
  public brandUrl: string;
  public provinces: number[];
  public shouldShowGraphs: boolean;
  public timeList = [];
  public selectedPeriod: any = "";
  public selectedPeriodDisplay: any;

  constructor(private _userType: UserType, private _urlUtil: UrlUtil, private _producerGraphicService: ProducerGraphicService,
    private _productService: ProductService, private _util: Util, public weedStatus: WeedStatus) { }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes.selectedWeedId && !changes.selectedWeedId.firstChange) ||
      (changes.selectedProvinceId && !changes.selectedProvinceId.firstChange)) {
      this._loadData();
    }
  }

  ngOnInit(): void {
    this._loadData();
    this._loadTime();
  }

  private _loadData() {
    this.loading = true;
    this._productService.getWeedById(this.selectedWeedId, this._userType.Producer).subscribe((data: WeedModel) => {
      if (data != null) {
        this.weed = data;
        this.date = new Date();
        if (data.WeedGallery.length > 0) {
          let img = data.WeedGallery.filter(d => d.FileName.indexOf('.png') > 0 || d.FileName.indexOf('.jpg') > 0 || d.FileName.indexOf('.jpeg') > 0);
          if (img.length > 0) {
            this.imageUrl = this._util.galleryUrl + "original_" + encodeURIComponent(img[0].FileName);
          } else {
            this.imageUrl = null;
          }
        } else {
          this.imageUrl = null;
        }
        this.brandUrl = data.BrandImgUrl ? this._util.baseUrl + "/images/brandLogo/" + data.BrandImgUrl : null;

        this._productService.getProvincesByWeedId(this.selectedWeedId).subscribe((prov: number[]) => {
          this.provinces = prov;
          this.shouldShowGraphs = this.provinces.indexOf(this.selectedProvinceId) >= 0 || this.selectedProvinceId == -1;

          this.loading = false;
        });
      }
    });
  }

  public generateUrl(product: any) {
    var strain = product.Strain ? product.Strain : '-';
    let currentUser = JSON.parse(localStorage.getItem("user"));
    let url = '';

    if (this.weed.Id) {
      if (currentUser.UserTypeId == this._userType.Admin) {
        url = `/super/product/${this._urlUtil.fixedEncodeURIComponent(product.Brand)}/${encodeURIComponent(product.WeedType.Name)}/${this._urlUtil.fixedEncodeURIComponent(strain)}/${this._urlUtil.fixedEncodeURIComponent(product.Name)}-${product.Id}`;
      } else {
        url = `/producer/product/${this._urlUtil.fixedEncodeURIComponent(product.Brand)}/${encodeURIComponent(product.WeedType.Name)}/${this._urlUtil.fixedEncodeURIComponent(strain)}/${this._urlUtil.fixedEncodeURIComponent(product.Name)}-${product.Id}`;
      }
    }
    return url;
  }

  private _loadTime() {
    let list = [];
    this.date = new Date();
    this._producerGraphicService.getProductAvailableMonths(this.selectedWeedId, this.selectedProvinceId).subscribe((data: GraphicModel[]) => {
      for (var i = 0; i < data.length; i++) {
        list.push({ label: data[i].Label, value: data[i].Attribute });
      }

      this.timeList = list;

      if (list.length == 0) {
        list.push({ label: "Not available", value: "0/0" });
      } else {
        list.splice(0, 0, { label: "All time", value: "0/0" });
      }
      this.selectedPeriod = list.length > 1 ? list[1].value : list[0].value;
    });
  }

  public onTimeChange(event) {
    var found = this.timeList.filter(x => x.value == event.value);
    if (found.length > 0) {
      this.selectedPeriodDisplay = found[0].label;
    }
  }
}
