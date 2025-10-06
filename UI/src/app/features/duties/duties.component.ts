import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { ApiService } from '../../core/services/api.service';
import { PersonAstronaut, CreateAstronautDutyRequest } from '../../core/models/person.model';
import { RANK_OPTIONS } from '../../core/constants/rank-options';

@Component({
  selector: 'app-duties',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './duties.component.html',
  styleUrls: ['./duties.component.scss'],
})
export class DutiesComponent implements OnInit {
  dutyForm: FormGroup;
  astronauts: PersonAstronaut[] = [];
  loading = true;
  submitting = false;

  // Rank options for dropdown (shared constant)
  rankOptions = RANK_OPTIONS;

  constructor(
    private fb: FormBuilder,
    private apiService: ApiService,
    private cdr: ChangeDetectorRef
  ) {
    this.dutyForm = this.fb.group({
      name: ['', Validators.required],
      rank: ['', Validators.required],
      dutyTitle: ['', Validators.required],
      dutyStartDate: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadAstronauts();
  }

  private loadAstronauts(): void {
    this.loading = true;
    this.astronauts = []; // Clear previous data

    this.apiService.getPeople().subscribe({
      next: (astronauts) => {
        console.log('Received astronauts for duties:', astronauts);
        this.astronauts = astronauts || [];
        this.loading = false;
        this.cdr.detectChanges(); // Force change detection to update the dropdown
      },
      error: (error) => {
        console.error('Error loading astronauts:', error);
        this.astronauts = [];
        this.loading = false;
      },
    });
  }

  onSubmit(): void {
    if (this.dutyForm.valid) {
      this.submitting = true;
      const formValue = this.dutyForm.value;
      const request: CreateAstronautDutyRequest = {
        name: formValue.name,
        rank: formValue.rank,
        dutyTitle: formValue.dutyTitle,
        dutyStartDate: new Date(formValue.dutyStartDate),
      };

      this.apiService.createAstronautDuty(request).subscribe({
        next: (response) => {
          console.log('Duty created successfully:', response);
          this.dutyForm.reset();
          this.submitting = false;
          this.loadAstronauts(); // Refresh the list
        },
        error: (error) => {
          console.error('Error creating duty:', error);
          this.submitting = false;
        },
      });
    }
  }
}
