import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProducerGrProductDataComponent } from './producer-gr-product-data.component';

describe('ProducerGrProductDataComponent', () => {
  let component: ProducerGrProductDataComponent;
  let fixture: ComponentFixture<ProducerGrProductDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProducerGrProductDataComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProducerGrProductDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
