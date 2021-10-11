import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProducerGrProductOverviewComponent } from './producer-gr-product-overview.component';

describe('ProducerGrProductOverviewComponent', () => {
  let component: ProducerGrProductOverviewComponent;
  let fixture: ComponentFixture<ProducerGrProductOverviewComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProducerGrProductOverviewComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProducerGrProductOverviewComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
