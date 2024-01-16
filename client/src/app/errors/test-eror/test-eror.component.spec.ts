import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestErorComponent } from './test-eror.component';

describe('TestErorComponent', () => {
  let component: TestErorComponent;
  let fixture: ComponentFixture<TestErorComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [TestErorComponent]
    })
    .compileComponents();
    
    fixture = TestBed.createComponent(TestErorComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
