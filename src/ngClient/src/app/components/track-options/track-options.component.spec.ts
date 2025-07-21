import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TrackOptionsComponent } from './track-options.component';

describe('TrackOptionsComponent', () => {
  let component: TrackOptionsComponent;
  let fixture: ComponentFixture<TrackOptionsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [TrackOptionsComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TrackOptionsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
