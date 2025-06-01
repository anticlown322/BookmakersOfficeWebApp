import { Role } from "../../../shared/enums/role.enum";

export interface UserRegistrationRequest {
    firstName?: string;
    lastName?: string;
    userName?: string;
    password?: string;
    email?: string;
    phoneNumber?: string;
    roles?: Role[]; 
}