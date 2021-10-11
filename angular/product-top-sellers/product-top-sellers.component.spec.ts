import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProductTopSellersComponent } from './product-top-sellers.component';

describe('ProductTopSellersComponent', () => {
  let component: ProductTopSellersComponent;
  let fixture: ComponentFixture<ProductTopSellersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProductTopSellersComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProductTopSellersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
