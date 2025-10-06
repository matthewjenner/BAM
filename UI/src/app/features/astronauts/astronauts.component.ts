import { Component, OnInit, ChangeDetectorRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import { ApiService } from '../../core/services/api.service';
import { PersonAstronaut, CreatePersonRequest } from '../../core/models/person.model';
import { RANK_OPTIONS } from '../../core/constants/rank-options';

@Component({
  selector: 'app-astronauts',
  standalone: true,
  imports: [CommonModule, RouterModule, ReactiveFormsModule],
  templateUrl: './astronauts.component.html',
  styleUrls: ['./astronauts.component.scss'],
})
export class AstronautsComponent implements OnInit {
  astronauts: PersonAstronaut[] = [];
  loading = true;
  error: string | null = null;

  // Modal functionality
  showAddModal = false;
  addForm: FormGroup;
  submitting = false;

  // Rank options for dropdown (shared constant)
  rankOptions = RANK_OPTIONS;

  constructor(
    private apiService: ApiService,
    private cdr: ChangeDetectorRef,
    private fb: FormBuilder
  ) {
    this.addForm = this.fb.group({
      name: ['', Validators.required],
    });
  }

  ngOnInit(): void {
    this.loadAstronauts();
  }

  loadAstronauts(): void {
    console.log('Starting to load astronauts...');
    this.loading = true;
    this.error = null;
    this.astronauts = []; // Clear previous data

    // Add a fallback timeout in case the HTTP request hangs
    const fallbackTimeout = setTimeout(() => {
      console.error('Request timed out after 15 seconds');
      this.loading = false;
      this.error = 'Request timed out. Please check if the API is running.';
      this.cdr.detectChanges(); // Force change detection
    }, 15000);

    this.apiService.getPeople().subscribe({
      next: (astronauts) => {
        clearTimeout(fallbackTimeout);
        console.log('Received astronauts:', astronauts);
        this.astronauts = astronauts || [];
        this.loading = false;
        this.error = null;
        console.log('Component state after update:', {
          loading: this.loading,
          error: this.error,
          astronautsCount: this.astronauts.length,
        });
        this.cdr.detectChanges(); // Force change detection
      },
      error: (error) => {
        clearTimeout(fallbackTimeout);
        console.error('Error loading astronauts:', error);
        this.astronauts = [];
        this.loading = false;
        this.error = 'Failed to load astronauts. Please try again.';
        this.cdr.detectChanges(); // Force change detection
      },
    });
  }

  // Modal functionality methods
  openAddModal(): void {
    this.showAddModal = true;
    this.addForm.reset();
  }

  closeAddModal(): void {
    this.showAddModal = false;
    this.addForm.reset();
    this.submitting = false;
  }

  onSubmitAdd(): void {
    if (this.addForm.valid) {
      this.submitting = true;
      const formValue = this.addForm.value;

      // Create the astronaut with basic info
      const createRequest: CreatePersonRequest = {
        name: formValue.name,
      };

      this.apiService.createPerson(createRequest).subscribe({
        next: (response) => {
          console.log('Astronaut created successfully:', response);
          this.submitting = false;
          this.closeAddModal();
          this.loadAstronauts(); // Refresh the list
        },
        error: (error) => {
          console.error('Error creating astronaut:', error);
          this.submitting = false;
        },
      });
    }
  }
}
