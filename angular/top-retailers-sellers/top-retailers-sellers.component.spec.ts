import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TopRetailersSellersComponent } from './top-retailers-sellers.component';

describe('TopRetailersSellersComponent', () => {
  let component: TopRetailersSellersComponent;
  let fixture: ComponentFixture<TopRetailersSellersComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TopRetailersSellersComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(TopRetailersSellersComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
