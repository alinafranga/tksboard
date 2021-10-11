import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { MultiAxisGraphicData, OverviewMetricsInfo } from '../../../shared';
import { ProducerGraphicService } from '../../../shared/services';

@Component({
  selector: 'app-producer-gr-product-follows',
  templateUrl: './producer-gr-product-follows.component.html',
  styleUrls: ['./producer-gr-product-follows.component.css']
})
export class ProducerGrProductFollowsComponent implements OnInit, OnChanges {

  @Input() date: any;
  @Input() dateSelected: any;
  @Input() weedId: number;
  @Input() provinceId: number;

  public basicData: any;
  public basicOptions: any;
  public loading = true;
  public tooltipDeviceStatus: boolean = false;
  public followsDifference: number;

  constructor(private _producerGraphicService: ProducerGraphicService) {
    this.basicOptions = {
      legend: {
        display: false
      },
      aspectRatio: 3.3,
      tooltips: {
        backgroundColor: '#747474',
      },
      scales: {
        xAxes: [{
          ticks: {
            fontColor: '#999999',
            fontSize: 12,
            fontFamily: 'Campton',
          },
          gridLines: {
            color: 'transparent',
            display: false
          }
        }],
        yAxes: [{
          ticks: {
            beginAtZero: true,
            fontColor: '#999999',
            fontSize: 12,
            fontFamily: 'Campton',

          },
          gridLines: {
            color: 'transparent',
            display: false
          }
        }]
      }
    };
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes.date && !changes.date.firstChange) ||
      (changes.provinceId && !changes.provinceId.firstChange)) {
      this._loadData();
      this._getOverviewAnalyticsInfo();
    }
  }

  ngOnInit(): void {
    this._loadData();
    this._getOverviewAnalyticsInfo();
  }


  private _getOverviewAnalyticsInfo() {
    if (this.date != '') {
      this._producerGraphicService.getOverviewByProductAnalytics(this.weedId, this.provinceId, this.date).subscribe((data: OverviewMetricsInfo) => {
        this.followsDifference = data.Follows.TotalPreviousPeriodPercentage;
      });
    }
  }

  private _loadData() {
    this.loading = true;
    if (this.date != '') {
      this._producerGraphicService.getProductFollows(this.weedId, this.provinceId, this.date)
        .subscribe((data: MultiAxisGraphicData) => {
          let labels = data.CurrentPeriod.map(a => a.Label);
          let valuesCurrent = data.CurrentPeriod.map(a => a.Value);
          let valuesPrevious = data.PreviousPeriod.map(a => a.Value);
          this.basicData = {
            labels: labels,
            datasets: [
              {
                type: 'bar',
                label: 'Current Selected month ',
                data: valuesCurrent,
                fill: false,
                backgroundColor: '#BF5BFF',
                barThickness: 12,
              },
              {
                type: 'line',
                label: 'Previous Month',
                data: valuesPrevious,
                borderDash: [5, 5],
                fill: false,
                borderColor: '#dde1e5',
              }
            ]
          };

          this.loading = false;
        });
    }
  }
  clickTooltipMobileShow() {
    this.tooltipDeviceStatus = !this.tooltipDeviceStatus;
  }
}
