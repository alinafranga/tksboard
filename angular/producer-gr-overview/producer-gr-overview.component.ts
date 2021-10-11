import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { GraphicModel, UserType, WeedFilterModel } from '../../../shared';
import { ProducerGraphicService } from '../../../shared/services';

@Component({
  selector: 'app-producer-gr-overview',
  templateUrl: './producer-gr-overview.component.html',
  styleUrls: ['./producer-gr-overview.component.css']
})
export class ProducerGrOverviewComponent implements OnInit {


  @Input() provinceId: number;
  @Input() data: WeedFilterModel[];
  @Input() userId: number;
  @Input() showEverything: boolean = false;

  public timeList = [];
  public selectedPeriod: any = "";
  public selectedType: any = "";
  public typeList = [];
  public tab: number = 1;
  public selectedWeedId: number;
  public selectedPeriodDisplay: any;
  public isAdmin = false;

  constructor(private _producerGraphicService: ProducerGraphicService, private _router: Router, private _userType: UserType) { }

  ngOnInit(): void {
    if (this.checkTab() == 2) {
      let url = this._router.url.toLowerCase().indexOf('#product-');
      let prod = this._router.url.substr(url);
      prod = prod.replace("#product-", "");
      if (prod.length > 0) {
        this.selectedWeedId = +prod;
      }
    }
    var user = JSON.parse(localStorage.getItem("user"));
    this.isAdmin = user.UserTypeId == this._userType.Admin;
    this.loadTime();
  }

  private loadTime() {
    let list = [];
    this._producerGraphicService.getAvailableMonths(this.userId, this.provinceId).subscribe((data: GraphicModel[]) => {
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

  public goTab(tabName: string, selectedWeedId: number) {
    let url = this._router.url;
    this._router.routeReuseStrategy.shouldReuseRoute = () => false;
    this._router.onSameUrlNavigation = 'reload';
    if (url.toLowerCase().indexOf('producer-dash') >= 0) {
      if (url.indexOf('#') > 0) {
        url = this._router.url.substr(0, this._router.url.indexOf('#'));
      }
      if (!selectedWeedId || selectedWeedId == 0) {
        this._router.navigateByUrl(url + '#' + tabName);
      } else {
        this._router.navigateByUrl(url + '#' + tabName + "-" + selectedWeedId);
      }
    } else {
      if (!selectedWeedId || selectedWeedId == 0) {
        this._router.navigateByUrl('/producer/dashboard' + '#' + tabName);
      } else {
        this._router.navigateByUrl('/producer/dashboard' + '#' + tabName + "-" + selectedWeedId);
      }
    }
  }

  public checkTab() {
    if (this._router.url.toLowerCase().endsWith('#overview'))
      return 1;
    if (this._router.url.toLowerCase().indexOf('#product') >= 0)
      return 2;
    return 1;
  }

  public onTimeChange(event) {
    var found = this.timeList.filter(x => x.value == event.value);
    if (found.length > 0) {
      this.selectedPeriodDisplay = found[0].label;
    }
  }
}
