export interface RankOption {
  value: string;
  label: string;
}

export const RANK_OPTIONS: RankOption[] = [
  { value: '2LT', label: '2LT - Second Lieutenant' },
  { value: '1LT', label: '1LT - First Lieutenant' },
  { value: 'CPT', label: 'CPT - Captain' },
  { value: 'MAJ', label: 'MAJ - Major' },
  { value: 'LTC', label: 'LTC - Lieutenant Colonel' },
  { value: 'COL', label: 'COL - Colonel' },
  { value: 'Brig Gen', label: 'Brig Gen - Brigadier General' },
  { value: 'Maj Gen', label: 'Maj Gen - Major General' },
  { value: 'Lt Gen', label: 'Lt Gen - Lieutenant General' },
  { value: 'Gen', label: 'Gen - General' },
];
