import { ComponentFixture, TestBed } from '@angular/core/testing';

import { WorkoutCardInfoComponent } from './workout-card-info.component';

describe('WorkoutCardInfoComponent', () => {
  let component: WorkoutCardInfoComponent;
  let fixture: ComponentFixture<WorkoutCardInfoComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ WorkoutCardInfoComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(WorkoutCardInfoComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
