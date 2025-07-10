import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BdsignlevelComponent } from './bdsignlevel.component';

describe('BdsignlevelComponent', () => {
  let component: BdsignlevelComponent;
  let fixture: ComponentFixture<BdsignlevelComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ BdsignlevelComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(BdsignlevelComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
