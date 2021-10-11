import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { MultiAxisGraphicData, OverviewMetricsInfo } from 'src/app/shared';
import { ProducerGraphicService } from 'src/app/shared/services';

@Component({
  selector: 'app-producer-gr-follows',
  templateUrl: './producer-gr-follows.component.html',
  styleUrls: ['./producer-gr-follows.component.css']
})
export class ProducerGrFollowsComponent implements OnInit, OnChanges {

  @Input() time: any;
  @Input() dateSelected: any;
  @Input() userId: number;
  @Input() provinceId: number;
  @Input() showEverything: boolean = false;

  public basicData: any;
  public basicOptions: any;
  public loading = true;
  public tooltipDeviceStatus: boolean = false;
  public followsDifference: number;
  public showBeVisible: boolean = false;

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
    if ((changes.time && !changes.time.firstChange) ||
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
    if (this.time != '') {
      this._producerGraphicService.getOverviewProducerAnalytics(this.userId, this.provinceId, this.time).subscribe((data: OverviewMetricsInfo) => {
        this.followsDifference = data.Follows.TotalPreviousPeriodPercentage;
      });
    }
  }

  private _loadData() {
    var month = this.time.substr(0, this.time.indexOf('/'));
    var year = this.time.substr(this.time.indexOf('/') + 1);
    if (month < 10 && year == 2021 && !this.showEverything) {
      this.loading = false;
      this.showBeVisible = false;
    } else {
      this.showBeVisible = true;
      this.loading = true;
      if (this.time != '') {
        this._producerGraphicService.getProducerFollows(this.userId, this.provinceId, this.time)
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
  }
  clickTooltipMobileShow() {
    this.tooltipDeviceStatus = !this.tooltipDeviceStatus;
  }

}
