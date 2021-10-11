import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProducerGrProductComponent } from './producer-gr-product.component';

describe('ProducerGrProductComponent', () => {
  let component: ProducerGrProductComponent;
  let fixture: ComponentFixture<ProducerGrProductComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProducerGrProductComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProducerGrProductComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
