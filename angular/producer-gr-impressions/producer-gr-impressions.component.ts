import { Component, Input, OnChanges, OnInit, SimpleChanges } from '@angular/core';
import { MultiAxisGraphicData, OverviewMetricsInfo } from '../../../shared';
import { ProducerGraphicService } from '../../../shared/services';

@Component({
  selector: 'app-producer-gr-impressions',
  templateUrl: './producer-gr-impressions.component.html',
  styleUrls: ['./producer-gr-impressions.component.css']
})
export class ProducerGrImpressionsComponent implements OnInit, OnChanges {

  @Input() time: any;
  @Input() dateSelected: any;
  @Input() userId: number;
  @Input() provinceId: number;

  public basicData: any;
  public basicOptions: any;
  public loading = true;
  public tooltipDeviceStatus: boolean = false;
  public impressionsDifference: number;

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
    if(this.time != ''){
      this._producerGraphicService.getOverviewProducerAnalytics(this.userId, this.provinceId, this.time).subscribe((data: OverviewMetricsInfo) => {
        this.impressionsDifference = data.Impressions.TotalPreviousPeriodPercentage;
      });
    }
  }

  private _loadData() {
    this.loading = true;
    if(this.time != ''){
      this._producerGraphicService.getProducerImpressions(this.userId, this.provinceId, this.time)
      .subscribe((data: MultiAxisGraphicData) => {
        let labels = data.CurrentPeriod.map(a => a.Label);
        let valuesCurrent = data.CurrentPeriod.map(a => a.Value);
        let valuesPrevious = data.PreviousPeriod.map(a => a.Value);
        this.basicData = {
          labels: labels,
          datasets: [
            {
              label: 'Current Selected month ',
              data: valuesCurrent,
              fill: false,
              borderColor: '#58d0ff',
            },
            {
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
