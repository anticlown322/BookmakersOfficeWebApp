import { UserBalance } from "./user-balance.model";
import { UserProfile } from "./user-profile.model";

export interface User {
  id: string;
  email: string;
  userName?: string;            
  refreshToken?: string;
  refreshTokenExpiryTime?: Date;
  profile: UserProfile;
  balance: UserBalance;
}