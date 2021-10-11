import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ProducerGrFollowsComponent } from './producer-gr-follows.component';

describe('ProducerGrFollowsComponent', () => {
  let component: ProducerGrFollowsComponent;
  let fixture: ComponentFixture<ProducerGrFollowsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ProducerGrFollowsComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ProducerGrFollowsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
