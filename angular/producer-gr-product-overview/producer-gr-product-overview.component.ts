import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { OverviewMetricsInfo } from 'src/app/shared';
import { ProducerGraphicService } from 'src/app/shared/services';

@Component({
  selector: 'app-producer-gr-product-overview',
  templateUrl: './producer-gr-product-overview.component.html',
  styleUrls: ['./producer-gr-product-overview.component.css']
})
export class ProducerGrProductOverviewComponent implements OnInit, OnChanges {

  @Input() selectedWeedId: number;
  @Input() selectedProvinceId: number;
  @Input() selectedPeriodId: string;

  public impressions: number = 0;
  public impressionsDifference: number = 0;
  public views: number = 0;
  public viewsDifference: number = 0;
  public list: number = 0;
  public listDifference: number = 0;
  public follow: number = 0;
  public followDifference: number = 0;
  public tooltipDeviceImpressions: boolean = false;
  public tooltipDeviceStatusViews: boolean = false;
  public tooltipDeviceStatusLists: boolean = false;
  public tooltipDeviceStatusFollows: boolean = false;
  public tooltipDeviceStatusNotifications: boolean = false;

  constructor(private _producerGraphicService: ProducerGraphicService) {

  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes.selectedPeriodId && !changes.selectedPeriodId.firstChange) || (changes.provinceId && !changes.provinceId.firstChange)) {
      this._getOverviewAnalyticsInfo();
    }
  }

  ngOnInit(): void {
    this._getOverviewAnalyticsInfo();
  }

  private _getOverviewAnalyticsInfo() {
    if (this.selectedPeriodId != '') {
      this._producerGraphicService.getOverviewByProductAnalytics(this.selectedWeedId, this.selectedProvinceId, this.selectedPeriodId).subscribe((data: OverviewMetricsInfo) => {
        this.impressions = data.Impressions.TotalMetric;
        this.impressionsDifference = data.Impressions.TotalPreviousPeriodPercentage;

        this.views = data.Views.TotalMetric;
        this.viewsDifference = data.Views.TotalPreviousPeriodPercentage;

        this.list = data.Lists.TotalMetric;
        this.listDifference = data.Lists.TotalPreviousPeriodPercentage;

        this.follow = data.Follows.TotalMetric;
        this.followDifference = data.Follows.TotalPreviousPeriodPercentage;
      });
    }
  }

  clickTooltipMobileShowImpressions() {
    this.tooltipDeviceImpressions = !this.tooltipDeviceImpressions;
  }
  clickTooltipMobileShowViews() {
    this.tooltipDeviceStatusViews = !this.tooltipDeviceStatusViews;
  }
  clickTooltipMobileShowLists() {
    this.tooltipDeviceStatusLists = !this.tooltipDeviceStatusLists;
  }
  clickTooltipMobileShowFollows() {
    this.tooltipDeviceStatusFollows = !this.tooltipDeviceStatusFollows;
  }
  clickTooltipMobileShowNotifications() {
    this.tooltipDeviceStatusNotifications = !this.tooltipDeviceStatusNotifications;
  }
}
