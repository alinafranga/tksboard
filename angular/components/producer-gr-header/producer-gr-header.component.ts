import { Component, Input, OnInit, SimpleChanges } from '@angular/core';
import { GraphicService } from '../../../shared/services';
import { GraphicModel, WeedFilterModel } from '../../../shared';

@Component({
  selector: 'app-producer-gr-header',
  templateUrl: './producer-gr-header.component.html',
  styleUrls: ['./producer-gr-header.component.css']
})
export class ProducerGrHeaderComponent implements OnInit {

  @Input() userId: number;
  @Input() provinceId: number;

  public chartData: any;
  public pieOptions: any;
  public pieLabels: any;
  public pieColors: any;
  public piePercentage: any;
  public loading: boolean = true;
  public tooltipDeviceStatus: boolean = false;
  public totalProducts: number;
  public date: any;
  public showData = false;

  private DEFAULT_COLORS = ['#3366CC', '#DC3912', '#FF9900', '#109618', '#990099',
    '#3B3EAC', '#0099C6', '#DD4477', '#66AA00', '#B82E2E',
    '#316395', '#994499', '#22AA99', '#AAAA11', '#6633CC',
    '#E67300', '#8B0707', '#329262', '#5574A6', '#3B3EAC']

  constructor(private _graphicService: GraphicService) {
    this.pieOptions = {
      responsive: true,
      legend: {
        display: false
      },
      aspectRatio: 1,
      tooltips: {
        callbacks: {
          label: function (tooltipItem, data) {
            let allData = data.datasets[tooltipItem.datasetIndex].data;
            let tooltipLabel = data.labels[tooltipItem.index];
            let tooltipData = allData[tooltipItem.index];
            let total = 0;
            for (let i in allData) {
              total += allData[i];
            }
            let tooltipPercentage = tooltipData;
            return tooltipLabel + ': ' + tooltipPercentage;
          }
        },
        backgroundColor: '#747474',
      }
    };
  }

  ngOnChanges(changes: SimpleChanges): void {
    if ((changes.provinceId && !changes.provinceId.firstChange)
      || (changes.userId && !changes.userId.firstChange)) {
      this._loadData();
    }
  }

  ngOnInit(): void {
    this._loadData();
  }

  private _loadData() {
    this.date = new Date();
    this.loading = true;
    if (!this.provinceId) {
      this.provinceId = -1;
    }

    this._graphicService.getProductsInMarketByProvince(this.userId, this.provinceId).subscribe((data: GraphicModel[]) => {
      this.pieLabels = this._getLabels(data);
      this.pieColors = this._configureDefaultColours(data);
      let pieData = this._getValues(data);
      this.piePercentage = this._getPiePercentage(data);

      this.showData = data.length > 0;

      this.chartData = {
        labels: this.pieLabels,
        datasets: [
          {
            data: pieData,
            backgroundColor: this.pieColors,
            hoverBackgroundColor: this.pieColors,
          }]
      };
      this.loading = false;
    });
  }
  private _getPiePercentage(data: GraphicModel[]): any {
    let values = [];
    let fullList = 0;
    for (let t = 0; t < data.length; t++) {
      fullList += data[t].Value;
    }

    for (let t = 0; t < data.length; t++) {
      var perc = data[t].Value;
      values.push(Math.round(perc));
    }
    return values;
  }

  private _configureDefaultColours(data: GraphicModel[]): string[] {
    let customColours = []
    if (data.length) {

      customColours = data.map((element, idx) => {
        return this.DEFAULT_COLORS[idx % this.DEFAULT_COLORS.length];
      });
    }

    return customColours;
  }

  private _getValues(types: GraphicModel[]) {
    let values = [];
    this.totalProducts = 0;
    for (let t = 0; t < types.length; t++) {
      values.push(types[t].Value);
      this.totalProducts = this.totalProducts + types[t].Value;
    }

    return values;
  }

  private _getLabels(types: GraphicModel[]) {
    let labels = [];

    for (let t = 0; t < types.length; t++) {
      labels.push(types[t].Label);
    }

    return labels;
  }
  clickTooltipMobileShow() {
    this.tooltipDeviceStatus = !this.tooltipDeviceStatus;
  }
}
