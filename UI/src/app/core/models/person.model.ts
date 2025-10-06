export interface Person {
  personId: number;
  name: string;
  currentRank?: string;
  currentDutyTitle?: string;
  careerStartDate?: Date;
  careerEndDate?: Date;
}

export interface AstronautDuty {
  id: number;
  personId: number;
  rank: string;
  dutyTitle: string;
  dutyStartDate: Date;
  dutyEndDate?: Date;
}

export interface PersonAstronaut extends Person {
  astronautDuties?: AstronautDuty[];
}

export interface CreatePersonRequest {
  name: string;
}

export interface UpdatePersonRequest {
  name?: string;
  currentRank?: string;
  currentDutyTitle?: string;
  careerStartDate?: Date;
  careerEndDate?: Date;
}

export interface CreateAstronautDutyRequest {
  name: string;
  rank: string;
  dutyTitle: string;
  dutyStartDate: Date;
}

export interface ApiResponse<T> {
  success: boolean;
  message: string;
  data?: T;
  responseCode?: number;
}
