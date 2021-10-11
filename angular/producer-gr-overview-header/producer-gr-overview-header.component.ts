import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { OverviewMetricsInfo } from 'src/app/shared';
import { ProducerGraphicService } from 'src/app/shared/services';

@Component({
  selector: 'app-producer-gr-overview-header',
  templateUrl: './producer-gr-overview-header.component.html',
  styleUrls: ['./producer-gr-overview-header.component.css']
})
export class ProducerGrOverviewHeaderComponent implements OnInit, OnChanges {

  @Input() userId: number;
  @Input() provinceId: number;
  @Input() date: any;
  @Input() showEverything: boolean = false;

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
  public showBeVisible: boolean = false;

  constructor(private _producerGraphicService: ProducerGraphicService) {

  }

  ngOnChanges(changes: SimpleChanges): void {

    if ((changes.date && !changes.date.firstChange) || (changes.provinceId && !changes.provinceId.firstChange)) {
      this._getOverviewAnalyticsInfo();
    }
  }

  ngOnInit(): void {
    this._getOverviewAnalyticsInfo();
  }

  private _getOverviewAnalyticsInfo() {
    var month = this.date.substr(0, this.date.indexOf('/'));
    var year = this.date.substr(this.date.indexOf('/') + 1);
    if ((month == 0 && year == 0) || (month >= 10 && year == 2021) || (year > 2021)) {
      this.showBeVisible = true;
    } else {
      this.showBeVisible = false;
    }

    if (this.date != '') {
      this._producerGraphicService.getOverviewProducerAnalytics(this.userId, this.provinceId, this.date).subscribe((data: OverviewMetricsInfo) => {
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
