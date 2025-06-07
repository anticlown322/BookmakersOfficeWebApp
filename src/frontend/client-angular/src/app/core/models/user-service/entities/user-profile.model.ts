export interface UserProfile {
  id?: number;                  
  firstName: string;
  lastName: string;
  roles: string[];              
  phoneNumber: string;
  email: string;
  isEmailConfirmed: boolean;
}