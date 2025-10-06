import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import {
  PersonAstronaut,
  AstronautDuty,
  UpdatePersonRequest,
} from '../../core/models/person.model';
import { RANK_OPTIONS } from '../../core/constants/rank-options';

@Component({
  selector: 'app-astronaut-detail',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './astronaut-detail.component.html',
  styleUrls: ['./astronaut-detail.component.scss'],
})
export class AstronautDetailComponent implements OnInit {
  astronaut: PersonAstronaut | null = null;
  duties: AstronautDuty[] = [];
  loading = true;
  error: string | null = null;
  astronautName: string = '';

  // Edit functionality
  isEditing = false;
  editForm: FormGroup;
  submitting = false;

  // Rank options for dropdown (shared constant)
  rankOptions = RANK_OPTIONS;

  constructor(
    private route: ActivatedRoute,
    private apiService: ApiService,
    private cdr: ChangeDetectorRef,
    private fb: FormBuilder
  ) {
    this.editForm = this.fb.group({
      name: ['', Validators.required],
      currentRank: [''],
      careerStartDate: [''],
      careerEndDate: [''],
    });
  }

  ngOnInit(): void {
    this.route.params.subscribe((params) => {
      const name = params['name'];
      if (name) {
        this.astronautName = decodeURIComponent(name);
        this.loadAstronautDetails(name);
      }
    });
  }

  loadAstronautDetails(name: string): void {
    console.log('Loading astronaut details for:', name);
    this.loading = true;
    this.error = null;
    this.astronaut = null;
    this.duties = [];

    // Add a fallback timeout in case the HTTP request hangs
    const fallbackTimeout = setTimeout(() => {
      console.error('Request timed out after 15 seconds');
      this.loading = false;
      this.error = 'Request timed out. Please try again.';
      this.cdr.detectChanges();
    }, 15000);

    this.apiService.getAstronautDutiesByName(name).subscribe({
      next: (data) => {
        clearTimeout(fallbackTimeout);
        console.log('Received astronaut details:', data);

        if (data && data.person) {
          this.astronaut = data.person;
          this.duties = data.duties || [];
        } else {
          // No details found - this is normal for astronauts without duties
          this.astronaut = null;
          this.duties = [];
        }

        this.loading = false;
        this.error = null;
        console.log('Component state after update:', {
          loading: this.loading,
          error: this.error,
          hasAstronaut: !!this.astronaut,
          dutiesCount: this.duties.length,
        });
        this.cdr.detectChanges();
      },
      error: (error) => {
        clearTimeout(fallbackTimeout);
        console.error('Error loading astronaut details:', error);
        this.astronaut = null;
        this.duties = [];
        this.loading = false;
        this.error = 'Failed to load astronaut details. Please try again.';
        this.cdr.detectChanges();
      },
    });
  }

  // Edit functionality methods
  startEdit(): void {
    if (this.astronaut) {
      this.isEditing = true;
      this.editForm.patchValue({
        name: this.astronaut.name,
        currentRank: this.astronaut.currentRank || '',
        careerStartDate: this.astronaut.careerStartDate
          ? this.formatDateForInput(this.astronaut.careerStartDate)
          : '',
        careerEndDate: this.astronaut.careerEndDate
          ? this.formatDateForInput(this.astronaut.careerEndDate)
          : '',
      });
    }
  }

  cancelEdit(): void {
    this.isEditing = false;
    this.editForm.reset();
  }

  saveEdit(): void {
    if (this.editForm.valid && this.astronaut) {
      this.submitting = true;
      const formValue = this.editForm.value;
      const updateRequest: UpdatePersonRequest = {
        name: formValue.name,
        currentRank: formValue.currentRank || undefined,
        careerStartDate: formValue.careerStartDate
          ? new Date(formValue.careerStartDate)
          : undefined,
        careerEndDate: formValue.careerEndDate ? new Date(formValue.careerEndDate) : undefined,
      };

      this.apiService.updatePerson(this.astronaut.name, updateRequest).subscribe({
        next: (response) => {
          console.log('Astronaut updated successfully:', response);
          this.submitting = false;
          this.isEditing = false;
          // Reload astronaut details to get updated data
          this.loadAstronautDetails(this.astronautName);
        },
        error: (error) => {
          console.error('Error updating astronaut:', error);
          this.submitting = false;
        },
      });
    }
  }

  private formatDateForInput(date: Date | string): string {
    const dateObj = typeof date === 'string' ? new Date(date) : date;
    return dateObj.toISOString().split('T')[0];
  }
}
