import { ComponentFixture, TestBed } from '@angular/core/testing';

import { StatusDropdown } from './status-dropdown';

describe('StatusDropdown', () => {
  let component: StatusDropdown;
  let fixture: ComponentFixture<StatusDropdown>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [StatusDropdown]
    })
    .compileComponents();

    fixture = TestBed.createComponent(StatusDropdown);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
