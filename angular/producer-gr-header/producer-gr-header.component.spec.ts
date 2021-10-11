import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProducerGrHeaderComponent } from './producer-gr-header.component';

describe('ProducerGrHeaderComponent', () => {
  let component: ProducerGrHeaderComponent;
  let fixture: ComponentFixture<ProducerGrHeaderComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProducerGrHeaderComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProducerGrHeaderComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
