import { Routes } from '@angular/router';

export const routes: Routes = [
  {
    path: '',
    loadComponent: () => import('./features/home/home.component').then((m) => m.HomeComponent),
  },
  {
    path: 'astronauts',
    loadComponent: () =>
      import('./features/astronauts/astronauts.component').then((m) => m.AstronautsComponent),
  },
  {
    path: 'duties',
    loadComponent: () =>
      import('./features/duties/duties.component').then((m) => m.DutiesComponent),
  },
  {
    path: 'astronaut/:name',
    loadComponent: () =>
      import('./features/astronaut-detail/astronaut-detail.component').then(
        (m) => m.AstronautDetailComponent
      ),
  },
  {
    path: '**',
    redirectTo: '',
  },
];
