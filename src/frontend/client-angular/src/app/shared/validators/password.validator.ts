import { AbstractControl, ValidationErrors } from "@angular/forms";

  export function passwordValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value) return null;
    
    const hasUpperCase = /[A-Z]/.test(value);
    const hasLowerCase = /[a-z]/.test(value);
    const hasNumber = /\d/.test(value);
    
    const errors: ValidationErrors = {};
    if (!hasUpperCase) errors["missingUpperCase"] = true;
    if (!hasLowerCase) errors["missingLowerCase"] = true;
    if (!hasNumber) errors["missingNumber"] = true;
    
    return Object.keys(errors).length ? errors : null;
  }