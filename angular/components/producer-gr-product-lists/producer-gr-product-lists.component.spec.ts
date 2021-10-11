import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProducerGrProductListsComponent } from './producer-gr-product-lists.component';

describe('ProducerGrProductListsComponent', () => {
  let component: ProducerGrProductListsComponent;
  let fixture: ComponentFixture<ProducerGrProductListsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProducerGrProductListsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProducerGrProductListsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
