import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { UIChart } from 'primeng';
import { ProvinceService } from '../../shared/services';
import { ProvinceModel, SelectItemModel, UserModel } from '../../shared/models';


@Component({
  selector: 'app-producer-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit {

  @Input() producerId: number;
  @Input() showEverything: boolean = false;

  public currentUser: UserModel;
  public currentUserId: number;
  public selectedProvince: any;
  public loading: boolean = false;
  public provinceList: any[];
  public selectedProvinceId: number = -1;
  @ViewChild('chart') chart: UIChart;

  constructor(private _provinceService: ProvinceService) { }

  ngOnInit(): void {

    if (localStorage.getItem("user") != null) {
      this.currentUser = JSON.parse(localStorage.getItem("user"));
    }
    if (this.producerId && this.producerId > 0) {
      this.currentUserId = this.producerId;
    } else {
      this.currentUserId = this.currentUser.Id;
    }
    this._loadProvinces();
  }

  private _loadProvinces() {
    this._provinceService.getProvinces().subscribe((data: ProvinceModel[]) => {
      this.provinceList = [];

      let p = new SelectItemModel();
      p.value = "-1";
      p.label = "All Provinces";
      this.provinceList.push(p);

      for (let i = 0; i < data.length; i++) {
        let p = new SelectItemModel();
        p.value = data[i].Id.toString();
        p.label = data[i].ProvinceName;
        this.provinceList.push(p);
      }
    });
  }

  public onProvinceChange(event: any) {
    this.selectedProvince = this.provinceList.find(x => x.value == event.value);
    this.selectedProvinceId = this.selectedProvince.value;
  }
}
